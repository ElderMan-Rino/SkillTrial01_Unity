using Elder.Core.Common.Enums;
using Elder.Core.Common.Interfaces;
using Elder.Core.FluxMessage.Delegates;
using System;

namespace Elder.Core.FluxMessage.Interfaces
{
    public interface IFluxRouter : IApplication
    {
        public IDisposable Subscribe<T>(MessageHandler<T> handler, FluxPhase phase) where T : struct, IFluxMessage;
        public void Publish<T>(in T message) where T : struct, IFluxMessage;
    }
}