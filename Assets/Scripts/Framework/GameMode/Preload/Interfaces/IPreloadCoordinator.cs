using Cysharp.Threading.Tasks;
using Elder.Framework.UI.Interfaces;

namespace Elder.Framework.GameMode.Preload.Interfaces
{
    internal interface IPreloadCoordinator : IUICoordinator
    {
        public void SetPresenter(IPreloadPresenter presenter);
        public UniTask OnTransitionCompletedAsync();
    }
}
