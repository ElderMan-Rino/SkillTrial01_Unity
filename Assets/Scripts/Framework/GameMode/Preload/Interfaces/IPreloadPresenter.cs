using Cysharp.Threading.Tasks;
using Elder.Framework.UI.Interfaces;

namespace Elder.Framework.GameMode.Preload.Interfaces
{
    internal interface IPreloadPresenter : IUIPresenter, IUIPresenterLifecycle, IUIPresenterPreparable
    {
        public void SetViewModel(IPreloadViewModel viewModel);
        public UniTask LoadResourcesAsync();
    }
}
