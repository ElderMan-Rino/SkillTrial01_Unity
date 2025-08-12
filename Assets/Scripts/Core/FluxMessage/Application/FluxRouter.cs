using Elder.Core.Common.BaseClasses;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.FluxMessage.Delegates;
using Elder.Core.FluxMessage.Interfaces;
using Elder.Core.Logging.Helpers;
using Elder.Core.Logging.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Core.FluxMessage.Application
{
    public class FluxRouter : ApplicationBase, IFluxRouter
    {
        private ILoggerEx _logger;
        private Dictionary<Type, object> _messageHandlers;

        public override bool TryInitialize(IApplicationProvider appProvider, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister)
        {
            if (!TryBindLogger())
                return false;

            if (!base.TryInitialize(appProvider, infraProvider, infraRegister))
                return false;

            InitializeMessageHandlersContainer();
            return true;
        }
        private void InitializeMessageHandlersContainer()
        {
            _messageHandlers = new();
        }
        private bool TryBindLogger()
        {
            _logger = LogFacade.GetLoggerFor<FluxRouter>();
            return _logger != null;
        }
        public void Subscribe<T>(MessageHandler<T> handler) where T : struct, IFluxMessage
        {
            var messageType = typeof(T);
            if (!_messageHandlers.TryGetValue(messageType, out var handlersObj))
            {
                handlersObj = new Dictionary<int, MessageHandler<T>>();
                _messageHandlers[messageType] = handlersObj;
            }
            var handlers = (Dictionary<int, MessageHandler<T>>)handlersObj;

            var target = handler.Target;
            int key = target != null ? target.GetHashCode() : handler.Method.GetHashCode();
            handlers[key] = handler; 
        }
        public void Unsubscribe<T>(MessageHandler<T> handler) where T : struct, IFluxMessage
        {
            var messageType = typeof(T);
            if (!_messageHandlers.TryGetValue(messageType, out var handlersObj))
                return;

            var handlers = (Dictionary<int, MessageHandler<T>>)handlersObj;
            var target = handler.Target;
            int key = target != null ? target.GetHashCode() : handler.Method.GetHashCode();

            handlers.Remove(key);

            if (handlers.Count > 0)
                return;

            _messageHandlers.Remove(messageType);
        }
        public void Publish<T>(in T message) where T : struct, IFluxMessage
        {
            var messageType = typeof(T);
            if (!_messageHandlers.TryGetValue(messageType, out var handlersObj))
                return;

            var handlers = (Dictionary<int, MessageHandler<T>>)handlersObj;
            foreach (var handler in handlers.Values)
                handler(in message);
        }

        protected override void DisposeManagedResources()
        {
            DisposeMessageHandlers();
            ClearLogger();
            base.DisposeManagedResources();
        }
        private void ClearLogger()
        {
            _logger = null;
        }
        private void DisposeMessageHandlers()
        {
            foreach (var handlerObj in _messageHandlers.Values)
            {
                if (handlerObj is not System.Collections.IDictionary dic)
                {
                    _logger.Error($"Invalid handler object found during disposal. Expected IDictionary but got: {handlerObj?.GetType().FullName ?? "null"}");
                    continue;
                }
                dic.Clear();
            }
            _messageHandlers.Clear();
            _messageHandlers = null;
        }
    }
}