using Elder.Framework.Common.Base;
using Elder.Framework.Flux.Definitions;
using Elder.Framework.Flux.Interfaces;
using System.Collections.Generic;

namespace Elder.Framework.Flux.Helpers
{
    internal sealed class MessageHandlerContainer<T> : DisposableBase, IMessageHandler where T : struct, IFluxMessage
    {
        private readonly Dictionary<long, MessageHandler<T>> _handlers = new();
        private long _lastTokenId;

        public void Publish(in T message)
        {
            foreach (var handler in _handlers.Values)
                handler?.Invoke(in message);
        }

        public void Add(MessageHandler<T> handler)
        {
            _handlers.TryAdd(_lastTokenId, handler);
        }

        public void SetLastTokenId(long tokenId)
        {
            _lastTokenId = tokenId;
        }

        public void Remove(long handlerId)
        {
            _handlers.Remove(handlerId);
        }

        protected override void DisposeManagedResources()
        {
            _handlers.Clear();
            base.DisposeManagedResources();
        }
    }
}