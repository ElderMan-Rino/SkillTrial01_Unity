using Cysharp.Threading.Tasks;
using Elder.Framework.GameMode.Preload.Infra;
using Elder.Framework.GameMode.Preload.Interfaces;
using Elder.Framework.Scene.Definitions;
using Elder.Framework.Scene.Messages;
using Elder.Framework.UI.App;

namespace Elder.Framework.GameMode.Preload.App
{
    internal sealed class PreloadCoordinator : UICoordinatorBase, IPreloadCoordinator
    {
        private IPreloadPresenter _presenter;

        public void SetPresenter(IPreloadPresenter presenter) => _presenter = presenter;

        public override async UniTask PrepareAsync()
        {
            await _uiSystem.PrepareAsync<PreloadView>();
        }

        public override async UniTask ShowAsync()
        {
            await _uiSystem.ShowAsync<PreloadView>();
        }

        public async UniTask OnTransitionCompletedAsync()
        {
            await _presenter.LoadResourcesAsync();
            _router.Publish(new SceneTransitionSignal(SceneConstants.TitleSceneKey));
        }

        public override async UniTask HideAsync()
        {
            await _uiSystem.HideAsync<PreloadView>();
        }
    }
}
