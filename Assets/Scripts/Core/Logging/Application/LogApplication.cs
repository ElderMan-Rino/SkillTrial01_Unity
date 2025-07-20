using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.Logging.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Core.Logging.Application
{
    public class LogApplication : ApplicationBase, ILoggerPublisher
    {
        private ILogEventDispatcher _logEventDispatcher;
        private Dictionary<Type, Logger> _loggerContainer;

        public override ApplicationType AppType => ApplicationType.Persistent;

        public override bool TryInitialize(IApplicationProvider appProvider, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister)
        {
            base.TryInitialize(appProvider, infraProvider, infraRegister);
            InitializeLoggerContainer();
            RequireLoggingInfra();
            return true;
        }
        private void RequireLoggingInfra()
        {
            RequireInfrastructure<ILogEventDispatcher>();
        }
        private void InitializeLoggerContainer()
        {
            _loggerContainer = new();
        }
        public override bool TryPostInitialize()
        {
            return TryBindLogEventHandler();
        }
        private bool TryBindLogEventHandler()
        {
            if (!TryGetInfrastructure<ILogEventDispatcher>(out var logEventDispatcher))
                return false;

            _logEventDispatcher = logEventDispatcher;
            return true;
        }
        public ILoggerEx GetLogger<T>() where T : class
        {
            var type = typeof(T);
            if (!_loggerContainer.TryGetValue(type, out var targetLogger))
            {
                targetLogger = new Logger(type, PublishLogEvent);
                _loggerContainer[type] = targetLogger;
            }
            return targetLogger;
        }
        private void PublishLogEvent(LogEvent logEvent)
        {
            _logEventDispatcher.DispatchLogEvent(logEvent);
        }
        protected override void DisposeManagedResources()
        {
            DisposeLoggerContainer();
            ClearLogEventDispatcher();
        }
        private void DisposeLoggerContainer()
        {
            foreach (var logger in _loggerContainer.Values)
                logger.Dispose();
            _loggerContainer = null;
        }
        private void ClearLogEventDispatcher()
        {
            _logEventDispatcher = null;
        }
        protected override void DisposeUnmanagedResources()
        {

        }
    }
}
