using Elder.Core.Common.BaseClasses;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.FluxMessage.Delegates;
using Elder.Core.FluxMessage.Helpers;
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
        private Dictionary<Type, List<Delegate>> _messageHandlers;

        public override bool TryInitialize(IApplicationProvider appProvider, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister)
        {
            if (!TryBindLogger())
                return false;

            if (!base.TryInitialize(appProvider, infraProvider, infraRegister))
                return false;

            _messageHandlers = new Dictionary<Type, List<Delegate>>();
            return true;
        }

        private bool TryBindLogger()
        {
            _logger = LogFacade.GetLoggerFor<FluxRouter>();
            return _logger != null;
        }

        public IDisposable Subscribe<T>(MessageHandler<T> handler) where T : struct, IFluxMessage
        {
            var messageType = typeof(T);
            if (!_messageHandlers.TryGetValue(messageType, out var list))
            {
                list = new List<Delegate>();
                _messageHandlers[messageType] = list;
            }

            list.Add(handler);

            return new SubscriptionToken<T>(this, handler);
        }

        public void Unsubscribe<T>(MessageHandler<T> handler) where T : struct, IFluxMessage
        {
            var messageType = typeof(T);
            if (!_messageHandlers.TryGetValue(messageType, out var list))
                return;

            list.Remove(handler);
            if (list.Count > 0)
                return;

            _messageHandlers.Remove(messageType);
        }

        public void Publish<T>(in T message) where T : struct, IFluxMessage
        {
            var messageType = typeof(T);
            if (!_messageHandlers.TryGetValue(messageType, out var list))
                return;

            var handlers = list as List<MessageHandler<T>>;
            for (int i = 0; i < handlers.Count; i++)
                handlers[i](in message);
        }

        protected override void DisposeManagedResources()
        {
            _messageHandlers.Clear();
            _messageHandlers = null;

            _logger = null;

            base.DisposeManagedResources();
        }

    }
}