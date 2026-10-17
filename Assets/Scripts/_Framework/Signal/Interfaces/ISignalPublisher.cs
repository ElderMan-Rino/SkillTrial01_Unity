using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Signal.Interfaces
{
    public interface ISignalPublisher : IFrameworkComponent
    {
        public void Publish<T>(in T signal) where T : struct, ISignal;
    }
}