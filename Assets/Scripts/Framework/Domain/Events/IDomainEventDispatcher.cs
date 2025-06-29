using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Domain.Events
{
    public interface IDomainEventDispatcher : IGameSystem
    {
        public UniTask DispatchAsync(IDomainEvent domainEvent);
    }
}
