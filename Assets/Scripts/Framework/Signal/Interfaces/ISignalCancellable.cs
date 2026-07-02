using Elder.Framework.Core.Interfaces;
using System;

namespace Elder.Framework.Signal.Interfaces
{
    public interface ISignalCancellable : ISystemComponent
    {
        public void Unsubscribe(Type signalType, long tokenId);
    }
}