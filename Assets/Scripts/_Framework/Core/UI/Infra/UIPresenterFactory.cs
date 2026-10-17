using Elder.Framework.Core.Definitions;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Core.UI.Interfaces;
using System;

namespace Elder.Framework.Core.UI.Infra
{
    internal sealed class UIPresenterFactory : IUIPresenterFactory, IFrameworkComponent, IDependencyInjectable
    {
        private IComponentProvider _componentProvider;

        public Type ClassType => typeof(UIPresenterFactory);
        public int Scope => GlobalScope.Id;

        public void InjectDependency(IComponentProvider provider)
        {
            _componentProvider = provider;
        }

        public TPresenter Create<TPresenter>() where TPresenter : IUIPresenter, new()
        {
            var presenter = new TPresenter();
            presenter.InjectDependency(_componentProvider);

            return presenter;
        }
    }
}
