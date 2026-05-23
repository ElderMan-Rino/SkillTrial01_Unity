using Elder.Framework.Core.Interfaces;
using Elder.Framework.Domain.Events;
using Elder.Framework.Domain.Infra;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Domain.Installer
{
    public readonly struct DomainInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.RegisterFactory<IDomainEventDispatcher>(p =>
            {
                p.TryResolve<ISignalRouter>(out var router);
                return new SignalDomainEventDispatcher(router);
            });
        }
    }
}
