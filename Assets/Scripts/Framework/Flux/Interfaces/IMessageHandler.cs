using System;

namespace Elder.Framework.Flux.Interfaces
{
    public interface IMessageHandler : IDisposable
    {
        public void Remove(long tokenId);
    }
}