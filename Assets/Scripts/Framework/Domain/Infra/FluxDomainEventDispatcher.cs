using Cysharp.Threading.Tasks;
using Elder.Framework.Domain.Events;
using Elder.Framework.Flux.Interfaces;

namespace Elder.Framework.Domain.Infra
{
    internal sealed class FluxDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IFluxRouter _router;

        public FluxDomainEventDispatcher(IFluxRouter router)
        {
            _router = router;
        }

        public UniTask DispatchAsync(IDomainEvent domainEvent)
        {
            _router.Publish(new FluxDomainEvent(domainEvent));
            return UniTask.CompletedTask;
        }
    }
}
