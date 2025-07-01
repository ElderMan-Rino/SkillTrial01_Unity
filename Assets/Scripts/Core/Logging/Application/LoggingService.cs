using Elder.Core.Common.BaseClasses;
using Elder.Core.Logging.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Core.Logging.Application
{
    public class LoggingService : DisposableBase, ILoggerPublisher
    {
        private ILogEventHandler _logEventHandler;
        private Dictionary<Type, Logger> _loggerContainer;

        public void Initialize()
        {
            InitializeLoggerContainer();
        }
        private void InitializeLoggerContainer()
        {
            _loggerContainer = new();
        }
        public void PostInitialize()
        {

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

        }
        protected override void DisposeManagedResources()
        {
            DisposeLoggerContainer();
        }
        private void DisposeLoggerContainer()
        {
            foreach (var logger in _loggerContainer.Values)
                logger.Dispose();
            _loggerContainer = null;
        }
        protected override void DisposeUnmanagedResources()
        {

        }

       
    }
}
