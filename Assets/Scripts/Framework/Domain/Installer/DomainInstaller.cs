using Elder.Framework.Domain.Events;
using Elder.Framework.Domain.Infra;
using VContainer;
using VContainer.Unity;

namespace Elder.Framework.Domain.Installer
{
    public readonly struct DomainInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<FluxDomainEventDispatcher>(Lifetime.Singleton).As<IDomainEventDispatcher>();
        }
    }
}
