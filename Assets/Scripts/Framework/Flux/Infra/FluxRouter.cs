using Elder.Framework.Common.Base;
using Elder.Framework.Flux.Definitions;
using Elder.Framework.Flux.Helpers;
using Elder.Framework.Flux.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Elder.Framework.Flux.Infra
{
    public class FluxRouter : DisposableBase, IFluxRouter, IFluxCancellable
    {
        private static long _globalTokenIdCounter;

        private readonly Dictionary<Type, IMessageHandler> _handlerContainers = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish<T>(in T message) where T : struct, IFluxMessage
        {
            if (!_handlerContainers.TryGetValue(typeof(T), out var containerBase))
                return;

            var container = Unsafe.As<MessageHandlerContainer<T>>(containerBase);
            container.Publish(message);
        }

        public SubscriptionToken Subscribe<T>(MessageHandler<T> handler) where T : struct, IFluxMessage
        {
            var messageType = typeof(T);
            if (!_handlerContainers.TryGetValue(messageType, out var containerBase))
            {
                var newContainer = new MessageHandlerContainer<T>();
                _handlerContainers[messageType] = newContainer;
                containerBase = newContainer;
            }

            var container = Unsafe.As<MessageHandlerContainer<T>>(containerBase);

            var tokenId = Interlocked.Increment(ref _globalTokenIdCounter);
            container.Add(tokenId, handler);

            return new SubscriptionToken(tokenId, messageType, this);
        }

        public void Unsubscribe(Type messageType, long tokenId)
        {
            if (!_handlerContainers.TryGetValue(messageType, out var containerBase))
                return;

            containerBase.Remove(tokenId);
        }

        protected override void DisposeManagedResources()
        {
            DisposeMessageHandlers();
            base.DisposeManagedResources();
        }

        private void DisposeMessageHandlers()
        {
            foreach (var container in _handlerContainers.Values)
                container.Dispose();
            _handlerContainers.Clear();
        }
    }
}