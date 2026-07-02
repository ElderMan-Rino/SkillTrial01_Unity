using Cysharp.Threading.Tasks;
using Elder.Framework.UI.Interfaces;

namespace Elder.Framework.GameMode.Splash.Interfaces
{
    internal interface ISplashCoordinator : IUICoordinator
    {
        public void SetPresenter(ISplashPresenter presenter);
        public UniTask OnTransitionCompletedAsync();
    }
}
