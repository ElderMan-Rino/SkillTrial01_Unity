using System;

namespace Elder.Framework.Flux.Interfaces
{
    public interface IFluxCancellable
    {
        public void Unsubscribe(Type messageType, long tokenId);
    }
}