using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Definitions;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.App;
using Elder.Framework.UI.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Infra;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Signal.Definitions;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using Elder.Framework.Signal.Messages;
using System;
using System.Collections.Generic;

namespace Elder.Framework.UI.Infra
{
    public sealed class UISystem : IUISystem, IDependencyInjectable, IDisposable
    {
        private const int SavedViewCount = 5;

        private readonly List<IUIEntryHandle> _uiStacks = new();

        private SignalToken _closeTopToken;
        private SignalToken _closeAllToken;

        // top에서 부터 n( SavedViewCount )개까지만 view를 보존 ( gameobject만 삭제 - 생성 처리한 것들만 )
        // n개의 범위에 돌아오면 다시 view를 생성 ( 생성 처리한 것들만 )
        // 다시 복원되면 viewmodel에 따라 페이지, 스크롤, ui 선택 상태등 복원

        // [TODO] IUIEntryHandle 기반 Stack/메모리 절약 구조 (틀만 존재, 로직 미구현):
        // 1. OpenAsync(IUIEntryHandle entry) 구현:
        //    - RegisterEntry 중복 호출 제거, OpenAsync 내부에서 _uiStacks.Add(entry) 수행.
        //    - _uiStacks.Count > SavedViewCount 초과 시 index 0 (가장 오래된 entry) entry.Sleep() 호출.
        // 2. UIEntryHandle.Sleep()/Wake(IUIViewHandle) 구현:
        //    - Sleep: entry.ViewHandle.DestroyViewOnly()로 GameObject만 제거, Presenter/ViewModel은 유지.
        //    - Wake: 새로 생성된 View GameObject로 Presenter.BindView(newViewHandle.View) 재호출.
        //      BindView 내부에서 기존 ViewModel을 그대로 재사용하므로 VM은 재생성되지 않음 (UIPresenterBase.GetOrCreateViewModel 참고).
        // 3. 페이지/스크롤/선택 상태 복원:
        //    - IUIViewModel에 상태 필드 추가 (현재 빈 계약) 후 Presenter.BindView 시점에 재적용.
        // 4. CloseTopAsync 보완:
        //    - RequestCloseAsync()는 현재 항상 true 반환(틀만 존재) — entry 내부 패널/확인창 우선 닫힘 로직 추가 필요.
        //    - Sleep 상태 entry close 시 ViewHandle이 null일 수 있음 — Dispose 전 null 체크 보강.
        // 5. CloseAllAsync 구현:
        //    - 전체 _uiStacks를 역순으로 RequestCloseAsync -> ViewHandle.Dispose/Presenter.Dispose 후 Clear.
        // 6. IUIViewHandle.DestroyViewOnly() 계약 자체가 아직 미정의 — 추가 필요.

        private ILoggerEx _logger = LogFacade.GetLogger<UISystem>();
        private IUIViewFinder _viewFinder;
        private IUIPresenterFactory _presenterFactory;

        public Type ClassType => typeof(UISystem);

        public int Scope => GlobalScope.Id;

        public void InjectDependency(IComponentProvider provider)
        {
            if (!provider.TryGetComponent<IUIViewFinder>(out _viewFinder))
                _logger.Error($"Failed to resolve {nameof(IUIViewFinder)}.");  // [HEAP]

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
            _closeTopToken.Dispose();
            _closeAllToken.Dispose();
        }

        public bool TryFindUIView<T>(out T view) where T : UIViewBase
        {
            return _viewFinder.TryFindUIView<T>(out view);
        }

        public TPresenter CreatePresenter<TPresenter>() where TPresenter : IUIPresenter, new()
        {
            return _presenterFactory.Create<TPresenter>();
        }

        public UniTask<TView> CreateViewAsync<TView>() where TView : UIViewBase
        {
            return UniTask.FromResult();
        }

        public void RegisterEntry(IUIEntryHandle entry)
        {
            _uiStacks.Add(entry);
        }

        public UniTask OpenAsync(IUIEntryHandle entry)
        {
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
            if (_uiStacks.Count == 0)
                return;

            var topIndex = _uiStacks.Count - 1;
            var topEntry = _uiStacks[topIndex];
            var shouldClose = await topEntry.RequestCloseAsync();
            if (!shouldClose)
                return;

            _uiStacks.RemoveAt(topIndex);
            topEntry.ViewHandle.Dispose();
            topEntry.Presenter.Dispose();
        }

        private UniTask CloseAllAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}