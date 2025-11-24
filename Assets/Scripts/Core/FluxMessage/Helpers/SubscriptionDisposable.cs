using Elder.Core.Common.Enums;
using Elder.Core.FluxMessage.Application;
using Elder.Core.FluxMessage.Delegates;
using Elder.Core.FluxMessage.Interfaces;
using System;

namespace Elder.Core.FluxMessage.Helpers
{
    public struct SubscriptionToken<T> : IDisposable where T : struct, IFluxMessage
    {
        private FluxRouter _router;
        private MessageHandler<T> _handler;
        private FluxPhase _fluxPhase;

        public SubscriptionToken(FluxRouter router, MessageHandler<T> handler, FluxPhase phase)
        {
            _router = router;
            _handler = handler;
            _fluxPhase = phase;
        }

        public void Dispose()
        {
            _router?.Unsubscribe(_handler, _fluxPhase);
            _router = null;
            _handler = null;
        }
    }
}