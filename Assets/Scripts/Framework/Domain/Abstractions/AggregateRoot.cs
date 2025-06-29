using Elder.Framework.Domain.Events;
using System.Collections.Generic;

namespace Elder.Framework.Domain.Abstractions
{
    public abstract class AggregateRoot<TId> : Entity<TId>
    {
        // [HEAP] 초기화 시 1회 할당
        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

        protected AggregateRoot(TId id) : base(id) { }

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
