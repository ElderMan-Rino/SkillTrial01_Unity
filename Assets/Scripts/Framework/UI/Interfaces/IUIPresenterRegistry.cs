using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.App;
using System;

namespace Elder.Framework.UI.Interfaces
{
    public interface IUIPresenterRegistry : ISystemComponent
    {
        public void Register<TInterface, TPresenter>(Func<TPresenter> factory)
            where TInterface : IUIPresenter
            where TPresenter : TInterface;

        public void RegisterForView<TView, TInterface>()
            where TView : UIViewBase
            where TInterface : class, IUIPresenter, IUIPresenterLifecycle;
    }
}
