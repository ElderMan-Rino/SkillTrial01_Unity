using Elder.Framework.Core.Interfaces;
using Elder.Framework.Signal.Definitions;
using Elder.Framework.Signal.Helpers;

namespace Elder.Framework.Signal.Interfaces
{
    public interface ISignalSubscriber : IFrameworkComponent
    {
        public SignalToken Subscribe<T>(SignalHandler<T> handler) where T : struct, ISignal;
    }
}