using Elder.Framework.Common.Base;
using Elder.Framework.Flux.Definitions;
using Elder.Framework.Flux.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Flux.Helpers
{
    public class MessageHandlerContainer<T> : DisposableBase, IMessageHandler where T : struct, IFluxMessage
    {
        private readonly Dictionary<long, MessageHandler<T>> _handlers = new();

        public void Publish(in T message)
        {
            foreach (var handler in _handlers.Values)
                handler?.Invoke(message);
        }

        public void Add(long handlerId, MessageHandler<T> handler)
        {
            _handlers.TryAdd(handlerId, handler);
        }

        public void Remove(long handlerId)
        {
            _handlers.Remove(handlerId);
        }

        protected override void DisposeManagedResources()
        {
            DisposeHandlers();
            base.DisposeManagedResources();
        }

        private void DisposeHandlers()
        {
            _handlers.Clear();
        }
    }
}