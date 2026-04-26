using Elder.Framework.Flux.Interfaces;

namespace Elder.Framework.Domain.Events
{
    public readonly struct FluxDomainEvent : IFluxMessage
    {
        public readonly IDomainEvent Payload;

        public FluxDomainEvent(IDomainEvent payload)
        {
            Payload = payload;
        }
    }
}
