using Cysharp.Threading.Tasks;
using Elder.Framework.UI.Interfaces;

namespace Elder.Framework.GameMode.Splash.Interfaces
{
    internal interface ISplashPresenter : IUIPresenter, IUIPresenterLifecycle, IUIPresenterPreparable
    {
        public void SetViewModel(ISplashViewModel viewModel);
        public UniTask PlaySplashSequenceAsync();
    }
}
