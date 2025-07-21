using Elder.Core.Common.Interfaces;
using Elder.Core.FluxMessage.Delegates;

namespace Elder.Core.FluxMessage.Interfaces
{
    public interface IFluxRouter : IApplication
    {
        public void Subscribe<T>(MessageHandler<T> handler) where T : struct, IFluxMessage;
        public void Unsubscribe<T>(MessageHandler<T> handler) where T : struct, IFluxMessage;
        public void Publish<T>(in T message) where T : struct, IFluxMessage;
    }
}