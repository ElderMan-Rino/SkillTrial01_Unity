using Cysharp.Threading.Tasks;
using Elder.Framework.Domain.Abstractions;
using Elder.Framework.Domain.Events;

namespace Elder.Framework.Application.Services
{
    public abstract class ApplicationServiceBase
    {
        private readonly IDomainEventDispatcher _eventDispatcher;

        protected ApplicationServiceBase(IDomainEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        protected async UniTask DispatchAndClearEvents<TId>(AggregateRoot<TId> aggregate)
        {
            foreach (var domainEvent in aggregate.DomainEvents)
                await _eventDispatcher.DispatchAsync(domainEvent);

            aggregate.ClearDomainEvents();
        }
    }
}
