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
    public sealed class SignalRouter : BaseSystemComponent, ISignalRouter, ISignalCancellable
    {
        private long _globalTokenIdCounter;

        private readonly Dictionary<Type, ISignalHandler> _handlerContainers = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish<T>(in T message) where T : struct, ISignal
        {
            if (!_handlerContainers.TryGetValue(typeof(T), out var containerBase))
                return;

            // [AOT NOTE] Unsafe.As<SignalHandlerContainer<T>> — T당 별도 코드 생성, ~80종 이내로 허용 범위
            var container = Unsafe.As<SignalHandlerContainer<T>>(containerBase);
            container.Publish(in message);
        }

        public SignalToken Subscribe<T>(SignalHandler<T> handler) where T : struct, ISignal
        {
            // [HEAP] 람다 — handler 캡처 클로저, 초기화 시 1회만 발생
            return RegisterHandler<T>(container => container.Add(handler));
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

            // [AOT NOTE] Unsafe.As<SignalHandlerContainer<T>> — T당 별도 코드 생성, ~80종 이내로 허용 범위
            var container = Unsafe.As<SignalHandlerContainer<T>>(containerBase);
            var tokenId = Interlocked.Increment(ref _globalTokenIdCounter);
            container.SetLastTokenId(tokenId);
            addAction(container);

            return new SignalToken(tokenId, messageType, this);
        }

        protected override void DisposeManagedResources()
        {
            DisposeHandlerContainers();
        }

        private void DisposeHandlerContainers()
        {
            // [HEAP] foreach on Dictionary allocates enumerator — Dispose path, acceptable
            foreach (var pair in _handlerContainers)
                pair.Value.Dispose();
            _handlerContainers.Clear();
        }
    }
}