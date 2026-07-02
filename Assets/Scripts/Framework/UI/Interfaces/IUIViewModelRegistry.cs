using Elder.Framework.Core.Interfaces;
using System;

namespace Elder.Framework.UI.Interfaces
{
    public interface IUIViewModelRegistry : ISystemComponent
    {
        public void Register<TViewModel>(Func<TViewModel> factory) where TViewModel : class;
    }
}
