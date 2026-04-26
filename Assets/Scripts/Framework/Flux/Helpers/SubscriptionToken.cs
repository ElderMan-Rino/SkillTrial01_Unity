using Elder.Framework.Flux.Interfaces;
using System;

namespace Elder.Framework.Flux.Helpers
{
    public struct SubscriptionToken
    {
        private long _tokenId; // ���� ID �߰�
        private Type _messageType;
        private IFluxCancellable _fluxCancellable;

        public SubscriptionToken(long tokenId, Type messageType, IFluxCancellable fluxCancellable)
        {
            _tokenId = tokenId;
            _messageType = messageType;
            _fluxCancellable = fluxCancellable;
        }

        public void Dispose()
        {
            _fluxCancellable?.Unsubscribe(_messageType, _tokenId);
            _messageType = null;
            _fluxCancellable = null;
        }

        public static SubscriptionToken Empty { get; } = new SubscriptionToken();
    }
}