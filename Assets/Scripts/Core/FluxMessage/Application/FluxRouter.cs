using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
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
        private Dictionary<Type, Dictionary<FluxPhase, List<Delegate>>> _messageHandlers;

        public override bool TryInitialize(IApplicationProvider appProvider, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister)
        {
            if (!TryBindLogger())
                return false;

            if (!base.TryInitialize(appProvider, infraProvider, infraRegister))
                return false;

            _messageHandlers = new();
            return true;
        }

        private bool TryBindLogger()
        {
            _logger = LogFacade.GetLoggerFor<FluxRouter>();
            return _logger != null;
        }

        public IDisposable Subscribe<T>(MessageHandler<T> handler, FluxPhase phase) where T : struct, IFluxMessage
        {
            var messageType = typeof(T);
            if (!_messageHandlers.TryGetValue(messageType, out var phaseMessageHandlers))
            {
                _logger.Error($"FluxRouter Subscribe Failed Target Phase = {phase}");
                return null;
            }

            if (!phaseMessageHandlers.TryGetValue(phase, out var list))
            {
                list = new List<Delegate>();
                phaseMessageHandlers[phase] = list;
            }
            list.Add(handler);
            return new SubscriptionToken<T>(this, handler, phase);
        }

        public void Unsubscribe<T>(MessageHandler<T> handler, FluxPhase phase) where T : struct, IFluxMessage
        {
            var messageType = typeof(T);
            if (!_messageHandlers.TryGetValue(messageType, out var phaseMessageHandlers))
            {
                _logger.Error($"FluxRouter Unsubscribe Failed Target Phase = {phase}");
                return;
            }

            if (!phaseMessageHandlers.TryGetValue(phase, out var list))
                return;

            list.Remove(handler);
            if (list.Count > 0)
                return;

            phaseMessageHandlers.Remove(phase);
        }

        public void Publish<T>(in T message) where T : struct, IFluxMessage
        {
            var messageType = typeof(T);
            if (!_messageHandlers.TryGetValue(messageType, out var messageHandlers))
                return;

            for (int i = 0; i < (int)FluxPhase.Max; ++i)
            {
                var fluxPhase = (FluxPhase)i;
                if (!messageHandlers.TryGetValue(fluxPhase, out var handlers))
                    continue;

                for (int j = 0; j < handlers.Count; ++j)
                {
                    var handler = (MessageHandler<T>)handlers[i];
                    handler(in message);
                }
            }
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