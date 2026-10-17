using Elder.Framework.Core.Definitions;
using Elder.Framework.Signal.Definitions;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Elder.Framework.Signal.Infra
{
    public sealed class SignalHub : ISignalPublisher, ISignalSubscriber, ISignalCancellable
    {
        private long _globalTokenIdCounter;

        private readonly Dictionary<Type, ISignalHandler> _handlerContainers = new();

        public Type ClassType => typeof(SignalHub);

        public int Scope => GlobalScope.Id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish<T>(in T signal) where T : struct, ISignal
        {
            if (!_handlerContainers.TryGetValue(typeof(T), out var containerBase)) return;
           
            var container = Unsafe.As<SignalHandlerContainer<T>>(containerBase);
            container.Publish(in signal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SignalToken Subscribe<T>(SignalHandler<T> handler) where T : struct, ISignal
        {
            var messageType = typeof(T);
            if (!_handlerContainers.TryGetValue(messageType, out var containerBase))
            {
                var newContainer = new SignalHandlerContainer<T>();
                _handlerContainers[messageType] = newContainer;
                containerBase = newContainer;
            }

            var container = Unsafe.As<SignalHandlerContainer<T>>(containerBase);
            var tokenId = Interlocked.Increment(ref _globalTokenIdCounter);
            container.SetLastTokenId(tokenId);
            container.Add(handler);

            return new SignalToken(tokenId, messageType, this);
        }

        public void Unsubscribe(Type messageType, long tokenId)
        {
            if (!_handlerContainers.TryGetValue(messageType, out var containerBase))
                return;

            containerBase.Remove(tokenId);
        }
    }
}