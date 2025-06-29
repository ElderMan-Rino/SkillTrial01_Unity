using Elder.Framework.Core.Interfaces;
using System;

namespace Elder.Framework.Signal.Interfaces
{
    public interface ISignalCancellable : IGameSystem
    {
        public void Unsubscribe(Type signalType, long tokenId);
    }
}