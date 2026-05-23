using System;

namespace Elder.Framework.Signal.Interfaces
{
    public interface ISignalCancellable
    {
        public void Unsubscribe(Type signalType, long tokenId);
    }
}