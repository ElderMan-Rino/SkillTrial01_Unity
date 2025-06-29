using Elder.Framework.Core.Interfaces;
using Elder.Framework.Domain.Events;
using Elder.Framework.Domain.Infra;

namespace Elder.Framework.Domain.Installer
{
    public readonly struct DomainInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            // [HEAP] 등록 시점 1회
            registry.RegisterInstance<IDomainEventDispatcher>(new SignalDomainEventDispatcher());
        }
    }
}
