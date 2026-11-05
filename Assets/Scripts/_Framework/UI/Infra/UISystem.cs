using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Definitions;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.App;
using Elder.Framework.UI.Definitions;
using Elder.Framework.UI.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Infra;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Signal.Definitions;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using Elder.Framework.Signal.Messages;
using R3;
using System;
using System.Collections.Generic;

namespace Elder.Framework.UI.Infra
{
    public sealed class UISystem : IUISystem, IDependencyInjectable, IDisposable
    {
        private const int SavedViewCount = 5;
        private const int MaxStackCount = 20;

        private readonly List<IUIEntryHandle> _uiStacks = new();
        private readonly List<IUIEntryHandle> _globalLayer = new();
        private readonly ReactiveCommand<Unit> _stackChanged = new(); // [HEAP]

        private SignalToken _closeTopToken;
        private SignalToken _closeAllToken;
        private bool _isClosing;

        // 진단(TestModule) 전용 알림 통로. Open/Close/Clear 변경 완료 시점에만 Execute된다.
        public Observable<Unit> OnStackChanged => _stackChanged;

        // top에서 부터 n( SavedViewCount )개까지만 view를 보존 ( gameobject만 삭제 - 생성 처리한 것들만 )
        // n개의 범위에 돌아오면 다시 view를 생성 ( 생성 처리한 것들만 )
        // 다시 복원되면 viewmodel에 따라 페이지, 스크롤, ui 선택 상태등 복원

        // MaxStackCount 초과 시 가장 오래된(bottom) entry를 Dispose하여 메모리 상한을 보장한다.
        // Sleep은 View GameObject만 파괴할 뿐 Presenter/ViewModel/Entry 자체는 유지하므로 스택이 무한히 쌓이는 것을 막지 못한다.

        // [TODO] 남은 작업:
        // 1. 페이지/스크롤/선택 상태 복원: IUIViewModel에 상태 필드 추가 후 Presenter.BindView 시점에 재적용.

        // [설계 확정] 글로벌 UI(상단바/설정/토스트 등)는 _globalLayer에, 씬 전속 UI(HUD/미니맵 등)는
        // _uiStacks에 분리 보관. 글로벌은 "항상 떠 있는 레이어 집합", 씬은 "top이 덮이고 복원되는 스택".
        // [해소] 호출 순서는 ComponentRegistry가 단계 전체 완료 후 다음 단계로 넘어가는 구조라 보장됨:
        //    PreDisposeComponents(전체, 씬 scope 정리) 완료 -> DisposeComponents(전체, ClearGlobalAsync 포함) 시작.

        private ILoggerEx _logger = LogFacade.GetLogger<UISystem>();
        private IUIViewFinder _viewFinder;
        private IUIViewFactory _viewFactory;
        private IUIPresenterFactory _presenterFactory;

        public Type ClassType => typeof(UISystem);

        public int Scope => GlobalScope.Id;

        public void InjectDependency(IComponentProvider provider)
        {
            if (!provider.TryGetComponent<IUIViewFinder>(out _viewFinder))
                _logger.Error($"Failed to resolve {nameof(IUIViewFinder)}.");  // [HEAP]

            if (!provider.TryGetComponent<IUIViewFactory>(out _viewFactory))
                _logger.Error($"Failed to resolve {nameof(IUIViewFactory)}."); // [HEAP]

            if (!provider.TryGetComponent<IUIPresenterFactory>(out _presenterFactory))
                _logger.Error($"Failed to resolve {nameof(IUIPresenterFactory)}."); // [HEAP]

            if (provider.TryGetComponent<ISignalSubscriber>(out var subscriber))
            {
                _closeTopToken = subscriber.Subscribe<FxCloseTopRequestedSignal>(HandleCloseTopRequested);
                _closeAllToken = subscriber.Subscribe<FxCloseAllRequestedSignal>(HandleCloseAllRequested);
            }
            else
            {
                _logger.Error($"Failed to resolve {nameof(ISignalSubscriber)}."); // [HEAP]
            }
        }

        public void Dispose()
        {
            for (var i = _uiStacks.Count - 1; i >= 0; i--)
            {
                var entry = _uiStacks[i];
                _uiStacks.RemoveAt(i);
                entry.ViewHandle.Dispose();
                entry.Presenter.Dispose();
            }

            ClearGlobalAsync().Forget();
            _closeTopToken.Dispose();
            _closeAllToken.Dispose();
            _stackChanged.Dispose();
        }

        public bool TryFindUIView<T>(out T view) where T : UIViewBase
        {
            return _viewFinder.TryFindUIView<T>(out view);
        }

        public TPresenter CreatePresenter<TPresenter>() where TPresenter : IUIPresenter, new()
        {
            return _presenterFactory.Create<TPresenter>();
        }

        public async UniTask<IUIViewHandle> CreateViewByAssetKeyAsync<TView>(string assetKey) where TView : UIViewBase
        {
            return await _viewFactory.CreateByAssetKeyAsync<TView>(assetKey);
        }

        // [TODO] 데이터 테이블 연동 전까지는 틀만 존재.
        public async UniTask<IUIViewHandle> CreateViewByDataKeyAsync<TView>(int dataKey) where TView : UIViewBase
        {
            return await _viewFactory.CreateByDataKeyAsync<TView>(dataKey);
        }

        public IUIViewHandle CreateSceneViewHandle<TView>(TView view) where TView : UIViewBase
        {
            return new SceneUIViewHandle(view.gameObject, view);
        }

        public IUIEntryHandle CreateEntry(int scope, IUIViewHandle viewHandle, UIPresenterBase presenter, IUIRebindStrategy rebindStrategy)
        {
            var displayIndex = CountByViewType(viewHandle.ViewType) + 1;
            return new UIEntryHandle(scope, displayIndex, viewHandle, presenter, presenter.ViewModel, rebindStrategy);
        }

        public UniTask OpenGlobalAsync(IUIEntryHandle entry)
        {
            _globalLayer.Add(entry);
            _stackChanged.Execute(Unit.Default);
            return UniTask.CompletedTask;
        }

        public UniTask OpenSceneAsync(IUIEntryHandle entry)
        {
            _uiStacks.Add(entry);

            if (_uiStacks.Count > MaxStackCount)
            {
                var oldest = _uiStacks[0];
                _uiStacks.RemoveAt(0);
                oldest.ViewHandle.Dispose();
                oldest.Presenter.Dispose();
            }

            RefreshSleepBoundary();
            _stackChanged.Execute(Unit.Default);

            return UniTask.CompletedTask;
        }

        public UniTask ClearSceneScopeAsync(int scope)
        {
            for (var i = _uiStacks.Count - 1; i >= 0; i--)
            {
                var entry = _uiStacks[i];
                if (entry.Scope != scope)
                    continue;

                _uiStacks.RemoveAt(i);
                entry.ViewHandle.Dispose();
                entry.Presenter.Dispose();
            }

            RefreshSleepBoundary();
            _stackChanged.Execute(Unit.Default);

            return UniTask.CompletedTask;
        }

        public UniTask ClearGlobalAsync()
        {
            for (var i = _globalLayer.Count - 1; i >= 0; i--)
            {
                var entry = _globalLayer[i];
                _globalLayer.RemoveAt(i);
                entry.ViewHandle.Dispose();
                entry.Presenter.Dispose();
            }

            _stackChanged.Execute(Unit.Default);

            return UniTask.CompletedTask;
        }

        private void HandleCloseTopRequested(in FxCloseTopRequestedSignal signal)
        {
            CloseTopAsync().Forget();
        }

        private void HandleCloseAllRequested(in FxCloseAllRequestedSignal signal)
        {
            CloseAllAsync().Forget();
        }

        private async UniTask CloseTopAsync()
        {
            if (_isClosing || _uiStacks.Count == 0)
                return;

            _isClosing = true;

            var topIndex = _uiStacks.Count - 1;
            var topEntry = _uiStacks[topIndex];
            var shouldClose = await topEntry.RequestCloseAsync();
            if (!shouldClose)
            {
                _isClosing = false;
                return;
            }

            _uiStacks.RemoveAt(topIndex);
            topEntry.ViewHandle.Dispose();
            topEntry.Presenter.Dispose();

            RefreshSleepBoundary();
            _stackChanged.Execute(Unit.Default);

            _isClosing = false;
        }

        private async UniTask CloseAllAsync()
        {
            if (_isClosing)
                return;

            _isClosing = true;

            for (var i = _uiStacks.Count - 1; i >= 0; i--)
            {
                var entry = _uiStacks[i];
                var shouldClose = await entry.RequestCloseAsync();
                if (!shouldClose)
                    continue;

                _uiStacks.RemoveAt(i);
                entry.ViewHandle.Dispose();
                entry.Presenter.Dispose();
            }

            RefreshSleepBoundary();
            _stackChanged.Execute(Unit.Default);

            _isClosing = false;
        }

        // top 기준 최근 SavedViewCount개는 Wake, 그보다 아래쪽은 Sleep 상태로 맞춘다.
        // Push/Pop/중간 제거(ClearSceneScopeAsync) 이후 항상 이 경계를 재계산해야 한다.
        private void RefreshSleepBoundary()
        {
            var sleepBoundary = _uiStacks.Count - SavedViewCount;
            for (var i = 0; i < _uiStacks.Count; i++)
            {
                var entry = _uiStacks[i];
                if (i < sleepBoundary)
                    entry.Sleep();
                else if (entry.ViewHandle.View is null)
                    entry.Wake();
            }
        }

        public IReadOnlyList<IUIEntryHandle> GetSceneStackEntries()
        {
            return _uiStacks;
        }

        public IReadOnlyList<IUIEntryHandle> GetGlobalLayerEntries()
        {
            return _globalLayer;
        }

        // DisplayIndex 산출용. 씬 스택과 글로벌 레이어를 합쳐 동일 ViewType 생성 개수를 센다.
        private int CountByViewType(Type viewType)
        {
            var count = 0;
            for (var i = 0; i < _uiStacks.Count; i++)
            {
                if (_uiStacks[i].ViewHandle.ViewType == viewType)
                    count++;
            }

            for (var i = 0; i < _globalLayer.Count; i++)
            {
                if (_globalLayer[i].ViewHandle.ViewType == viewType)
                    count++;
            }

            return count;
        }
    }
}