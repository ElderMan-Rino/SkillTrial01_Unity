using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.GameMode.App;
using Elder.Framework.GameMode.Splash.Infra;
using Elder.Framework.GameMode.Splash.Interfaces;
using Elder.Framework.Scene.Definitions;
using Elder.Framework.Scene.Messages;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using Elder.Framework.UI.Interfaces;
using System;

namespace Elder.Framework.GameMode.Splash.App
{
    internal sealed class SplashOrchestrator : GameModeOrchestrator
    {
        private SignalToken _completedSubscription;
        private ISplashCoordinator _coordinator;

        public SplashOrchestrator(IGameSystemRegistry registry) : base(registry) { }

        public override Type GameModeType => typeof(SplashOrchestrator);

        protected override void RegisterSystems(IGameSystemRegistry registry)
        {
            registry.Register<SplashCoordinator>().As<ISplashCoordinator>();
        }

        protected override async UniTask<bool> OnPrepareAsync()
        {
            if (!_provider.TryGetSystem<IUIPresenterRegistry>(out var presenterRegistry)) return false;
            if (!_provider.TryGetSystem<IUIPresenterFactory>(out var presenterFactory)) return false;
            if (!_provider.TryGetSystem<IUIViewModelRegistry>(out var viewModelRegistry)) return false;
            if (!_provider.TryGetSystem<IUIViewModelFactory>(out var viewModelFactory)) return false;
            if (!_provider.TryGetSystem<ISplashCoordinator>(out _coordinator)) return false;
            if (!_provider.TryGetSystem<ISignalRouter>(out var router)) return false;

            // [HEAP] 씬 진입 시 타입당 1회 등록
            viewModelRegistry.Register<ISplashViewModel>(() => new SplashViewModel());
            presenterRegistry.Register<ISplashPresenter, SplashPresenter>(() => new SplashPresenter());
            presenterRegistry.RegisterForView<SplashView, ISplashPresenter>();

            var viewModel = viewModelFactory.Create<ISplashViewModel>();
            var presenter = presenterFactory.Create<ISplashPresenter>();
            presenter.SetViewModel(viewModel);
            _coordinator.SetPresenter(presenter);

            // [HEAP] Subscribe — 씬 진입 시 1회 핸들러 등록
            _completedSubscription = router.Subscribe<SceneTransitionCompletedSignal>(HandleTransitionCompleted);

            await _coordinator.PrepareAsync();
            return true;
        }

        public override UniTask<bool> TryActivateAsync()
        {
            _coordinator.ShowAsync().Forget();
            return UniTask.FromResult(true);
        }

        public override async UniTask TeardownAsync()
        {
            _completedSubscription.Dispose();

            if (_coordinator is not null)
                await _coordinator.HideAsync();

            if (_provider.TryGetSystem<IUIPresenterFactory>(out var presenterFactory))
                presenterFactory.Release<ISplashPresenter>();

            if (_provider.TryGetSystem<IUIViewModelFactory>(out var viewModelFactory))
                viewModelFactory.Release<ISplashViewModel>();

            if (_provider.TryGetSystem<IUISystem>(out var uiSystem))
                await uiSystem.ReleaseAsync<SplashView>();
        }

        private void HandleTransitionCompleted(in SceneTransitionCompletedSignal signal)
        {
            if (signal.LoadedSceneKey != SceneConstants.SplashSceneKey) return;
            _coordinator.OnTransitionCompletedAsync().Forget();
        }
    }
}
