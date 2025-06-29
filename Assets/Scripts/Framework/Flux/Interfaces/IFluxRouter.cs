using Elder.Framework.Flux.Definitions;
using Elder.Framework.Flux.Helpers;

namespace Elder.Framework.Flux.Interfaces
{
    public interface IFluxRouter 
    {
        public void Publish<T>(in T message) where T : struct, IFluxMessage;
        public SubscriptionToken Subscribe<T>(MessageHandler<T> handler) where T : struct, IFluxMessage;
    }
}