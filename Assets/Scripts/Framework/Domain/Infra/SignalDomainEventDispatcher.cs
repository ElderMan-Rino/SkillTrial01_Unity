using Cysharp.Threading.Tasks;
using Elder.Framework.Domain.Events;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Domain.Infra
{
    internal sealed class SignalDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly ISignalRouter _router;

        public SignalDomainEventDispatcher(ISignalRouter router)
        {
            _router = router;
        }

        public UniTask DispatchAsync(IDomainEvent domainEvent)
        {
            _router.Publish(new SignalDomainEvent(domainEvent));
            return UniTask.CompletedTask;
        }
    }
}
