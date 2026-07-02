using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.App;

namespace Elder.Framework.UI.Interfaces
{
    public interface IUIPresenterFactory : ISystemComponent
    {
        public TInterface Create<TInterface>() where TInterface : class, IUIPresenter;
        public void Release<TInterface>() where TInterface : class, IUIPresenter;
        public IUIPresenterLifecycle GetLifecycleForView<TView>() where TView : UIViewBase;
    }
}
