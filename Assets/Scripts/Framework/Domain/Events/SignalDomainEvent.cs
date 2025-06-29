using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Domain.Events
{
    public readonly struct SignalDomainEvent : ISignal
    {
        public readonly IDomainEvent Payload;

        public SignalDomainEvent(IDomainEvent payload)
        {
            Payload = payload;
        }
    }
}
