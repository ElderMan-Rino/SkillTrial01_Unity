using System;

namespace Elder.Framework.Signal.Interfaces
{
    public interface ISignalHandler : IDisposable
    {
        public void Remove(long tokenId);
    }
}