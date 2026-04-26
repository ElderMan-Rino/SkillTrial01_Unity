using Cysharp.Threading.Tasks;
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
    internal sealed class FluxRouter : DisposableBase, IFluxRouter, IFluxCancellable
    {
        private static long _globalTokenIdCounter;

        private readonly Dictionary<Type, IMessageHandler> _handlerContainers = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish<T>(in T message) where T : struct, IFluxMessage
        {
            if (!_handlerContainers.TryGetValue(typeof(T), out var containerBase))
                return;

            var container = Unsafe.As<MessageHandlerContainer<T>>(containerBase);
            container.Publish(in message);
        }

        public SubscriptionToken Subscribe<T>(MessageHandler<T> handler) where T : struct, IFluxMessage
        {
            // [HEAP] 람다 — handler 캡처 클로저, 초기화 시 1회만 발생
            return RegisterHandler<T>(container => container.Add(handler));
        }

        public SubscriptionToken SubscribeAsync<T>(AsyncMessageHandler<T> handler) where T : struct, IFluxMessage
        {
            // [HEAP] 람다 클로저 — handler 캡처, 초기화 시 1회만 발생
            MessageHandler<T> wrapper = (in T msg) =>
            {
                var captured = msg;
                handler(captured).Forget();
            };
            // [HEAP] 람다 — wrapper 캡처 클로저, 초기화 시 1회만 발생
            return RegisterHandler<T>(container => container.Add(wrapper));
        }

        public void Unsubscribe(Type messageType, long tokenId)
        {
            if (!_handlerContainers.TryGetValue(messageType, out var containerBase))
                return;

            containerBase.Remove(tokenId);
        }

        private SubscriptionToken RegisterHandler<T>(Action<MessageHandlerContainer<T>> addAction)
            where T : struct, IFluxMessage
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
            container.SetLastTokenId(tokenId);
            addAction(container);

            return new SubscriptionToken(tokenId, messageType, this);
        }

        protected override void DisposeManagedResources()
        {
            foreach (var container in _handlerContainers.Values)
                container.Dispose();
            _handlerContainers.Clear();
            base.DisposeManagedResources();
        }
    }
}