using Elder.Framework.Signal.Definitions;
using Elder.Framework.Signal.Helpers;

namespace Elder.Framework.Signal.Interfaces
{
    public interface ISignalRouter
    {
        public void Publish<T>(in T signal) where T : struct, ISignal;
        public SignalToken Subscribe<T>(SignalHandler<T> handler) where T : struct, ISignal;
        public SignalToken SubscribeAsync<T>(AsyncSignalHandler<T> signal) where T : struct, ISignal;
    }
}