using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.Domain.Events;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Domain.Infra
{
    internal sealed class SignalDomainEventDispatcher : BaseSystem, IDomainEventDispatcher
    {
        private ISignalRouter _router;

        protected override bool OnInjectDependency()
        {
            TryGetSystem<ISignalRouter>(out _router);
            return true;
        }

        public UniTask DispatchAsync(IDomainEvent domainEvent)
        {
            _router.Publish(new SignalDomainEvent(domainEvent));
            return UniTask.CompletedTask;
        }
    }
}
