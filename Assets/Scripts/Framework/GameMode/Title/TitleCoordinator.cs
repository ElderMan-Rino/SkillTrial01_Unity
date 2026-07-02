using Cysharp.Threading.Tasks;
using Elder.Framework.Scene.Messages;
using Elder.Framework.Scene.Definitions;
using Elder.Framework.UI.App;

namespace Elder.Framework.GameMode.Title
{
    internal sealed class TitleCoordinator : UICoordinatorBase
    {
        public override async UniTask ShowAsync()
        {
            // [HEAP] Addressables 로드 + Instantiate — 씬 진입 시 1회
            // var view = await UISystem.ShowAsync<TitleView>(ViewKey);
            // view.OnStartButton.Subscribe(HandleStartButton).AddTo(_disposables);
            await UniTask.CompletedTask;
        }

        public override async UniTask HideAsync()
        {
            // await UISystem.HideAsync<TitleView>();
            await UniTask.CompletedTask;
        }

        private void HandleStartButton()
        {
            _router.Publish(new SceneTransitionSignal(SceneConstants.MainSceneKey));
        }

        protected override void DisposeManagedResources()
        {
            // UISystem.ReleaseAsync<TitleView>().Forget();
            base.DisposeManagedResources();
        }

        public override UniTask PrepareAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
