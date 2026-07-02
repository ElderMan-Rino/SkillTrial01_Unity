using Cysharp.Threading.Tasks;
using Elder.Framework.GameMode.Splash.Infra;
using Elder.Framework.GameMode.Splash.Interfaces;
using Elder.Framework.Scene.Definitions;
using Elder.Framework.Scene.Messages;
using Elder.Framework.UI.App;

namespace Elder.Framework.GameMode.Splash.App
{
    internal sealed class SplashCoordinator : UICoordinatorBase, ISplashCoordinator
    {
        private ISplashPresenter _presenter;

        public void SetPresenter(ISplashPresenter presenter) => _presenter = presenter;

        public override async UniTask PrepareAsync()
        {
            await _uiSystem.PrepareAsync<SplashView>();
        }

        public override async UniTask ShowAsync()
        {
            await _uiSystem.ShowAsync<SplashView>();
        }

        public async UniTask OnTransitionCompletedAsync()
        {
            await _presenter.PlaySplashSequenceAsync();
            _router.Publish(new SceneTransitionSignal(SceneConstants.PreloadSceneKey));
        }

        public override async UniTask HideAsync()
        {
            await _uiSystem.HideAsync<SplashView>();
        }
    }
}
