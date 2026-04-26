using Cysharp.Threading.Tasks;

namespace Elder.Framework.Domain.Events
{
    public interface IDomainEventDispatcher
    {
        public UniTask DispatchAsync(IDomainEvent domainEvent);
    }
}
