using Elder.Core.Common.BaseClasses;
using Elder.Core.FluxMessage.Delegates;
using Elder.Core.FluxMessage.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Core.FluxMessage.Application
{
    public class FluxRouter : ApplicationBase, IFluxRouter
    {
        private Dictionary<Type, object> _messageHandlers;

        public void Subscribe<T>(MessageHandler<T> handler) where T : struct, IFluxMessage
        {
            var messageType = typeof(T);
            if (!_messageHandlers.TryGetValue(messageType, out var handlersObj))
            {
                handlersObj = new List<MessageHandler<T>>();
                _messageHandlers[messageType] = handlersObj;
            }
            var handlers = (List<MessageHandler<T>>)handlersObj;
            handlers.Add(handler);
        }
        public void Unsubscribe<T>(MessageHandler<T> handler) where T : struct, IFluxMessage
        {
            var messageType = typeof(T);
            if (!_messageHandlers.TryGetValue(messageType, out var handlersObj))
                return;
            
            var handlers = (List<MessageHandler<T>>)handlersObj;
            handlers.Remove(handler);
            if (handlers.Count > 0)
                return;
            _messageHandlers.Remove(messageType);
        }
        public void Publish<T>(in T message) where T : struct, IFluxMessage
        {
            var messageType = typeof(T);
            if (!_messageHandlers.TryGetValue(messageType, out var handlersObj))
                return;

            var handlers = (List<MessageHandler<T>>)handlersObj;
            foreach (var handler in handlers)
                handler(in message);
        }
        protected override void DisposeManagedResources()
        {
            DisposeMessageHandlers();
            base.DisposeManagedResources();
        }
        private void DisposeMessageHandlers()
        {
            foreach (var handlerObj in _messageHandlers.Values)
            {
                if (handlerObj is not System.Collections.IList list)
                    continue;
                list.Clear();
            }
            _messageHandlers.Clear();
            _messageHandlers = null;
        }
    }
}