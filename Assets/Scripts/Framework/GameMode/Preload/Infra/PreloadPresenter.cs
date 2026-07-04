using Cysharp.Threading.Tasks;
using Elder.Framework.GameMode.Preload.Interfaces;
using Elder.Framework.UI.App;
using Elder.Framework.UI.Interfaces;
using System;

namespace Elder.Framework.GameMode.Preload.Infra
{
    internal sealed class PreloadPresenter : UIPresenterBase, IPreloadPresenter
    {
        private IUISystem _uiSystem;
        private IPreloadViewModel _viewModel;

        protected override void OnInjectDependency()
        {
            if (!TryGetSystem<IUISystem>(out _uiSystem))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IUISystem)}");
        }

        public void SetViewModel(IPreloadViewModel viewModel) => _viewModel = viewModel;

        public UniTask PrepareAsync() => UniTask.CompletedTask;

        public UniTask OnViewShownAsync()
        {
            if (!_uiSystem.TryGetView<PreloadView>(out var view)) return UniTask.CompletedTask;
            view.Refresh(0f);
            return UniTask.CompletedTask;
        }

        public async UniTask LoadResourcesAsync()
        {
            // TODO: 필요 리소스/추가 데이터 로드 로직 구현 예정
            if (!_uiSystem.TryGetView<PreloadView>(out var view)) return;
            view.Refresh(1f);
            await UniTask.CompletedTask;
        }

        public UniTask OnViewHiddenAsync() => UniTask.CompletedTask;

        public UniTask OnViewReleasedAsync() => UniTask.CompletedTask;

        protected override void DisposeManagedResources()
        {
            _uiSystem = null;
        }
    }
}
