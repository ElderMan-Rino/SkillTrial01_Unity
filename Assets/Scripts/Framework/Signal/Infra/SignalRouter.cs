using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.Signal.Definitions;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Elder.Framework.Signal.Infra
{
    internal sealed class SignalRouter : BaseSystem, ISignalRouter, ISignalCancellable
    {
        private long _globalTokenIdCounter;

        private readonly Dictionary<Type, ISignalHandler> _handlerContainers = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish<T>(in T message) where T : struct, ISignal
        {
            if (!_handlerContainers.TryGetValue(typeof(T), out var containerBase))
                return;

            var container = Unsafe.As<SignalHandlerContainer<T>>(containerBase);
            container.Publish(in message);
        }

        public SignalToken Subscribe<T>(SignalHandler<T> handler) where T : struct, ISignal
        {
            // [HEAP] 람다 — handler 캡처 클로저, 초기화 시 1회만 발생
            return RegisterHandler<T>(container => container.Add(handler));
        }

        public SignalToken SubscribeAsync<T>(AsyncSignalHandler<T> handler) where T : struct, ISignal
        {
            // [HEAP] 람다 클로저 — handler 캡처, 초기화 시 1회만 발생
            SignalHandler<T> wrapper = (in T msg) =>
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

        private SignalToken RegisterHandler<T>(Action<SignalHandlerContainer<T>> addAction)
            where T : struct, ISignal
        {
            var messageType = typeof(T);
            if (!_handlerContainers.TryGetValue(messageType, out var containerBase))
            {
                // [HEAP] 메시지 타입당 최초 구독 시 1회 할당
                var newContainer = new SignalHandlerContainer<T>();
                _handlerContainers[messageType] = newContainer;
                containerBase = newContainer;
            }

            var container = Unsafe.As<SignalHandlerContainer<T>>(containerBase);
            var tokenId = Interlocked.Increment(ref _globalTokenIdCounter);
            container.SetLastTokenId(tokenId);
            addAction(container);

            return new SignalToken(tokenId, messageType, this);
        }

        protected override void DisposeManagedResources()
        {
            foreach (var pair in _handlerContainers)  // Dictionary enumerator is a value-type struct — no heap alloc
                pair.Value.Dispose();
            _handlerContainers.Clear();
            base.DisposeManagedResources();
        }
    }
}