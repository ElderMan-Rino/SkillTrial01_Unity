using System;

namespace Elder.Framework.Domain.Events
{
    public interface IDomainEvent
    {
        public DateTime OccurredAt { get; }
    }
}
