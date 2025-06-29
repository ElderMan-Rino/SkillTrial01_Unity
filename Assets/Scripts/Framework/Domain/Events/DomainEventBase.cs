using System;

namespace Elder.Framework.Domain.Events
{
    public abstract class DomainEventBase : IDomainEvent
    {
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
    }
}
