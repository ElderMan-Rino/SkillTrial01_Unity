using Elder.Framework.Common.Base;
using Elder.Framework.Log.Definitions;
using Elder.Framework.Log.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Log.Infra
{
    public class LogPublisher : DisposableBase, ILoggerPublisher
    {
        private readonly Dictionary<Type, LoggerEX> _loggerContainer = new();
        private readonly List<ILogAdapter> _logAdapters;

        public LogPublisher(IEnumerable<ILogAdapter> logAdapters)
        {
            _logAdapters = new(logAdapters);
        }

        public ILoggerEx GetLogger<T>() where T : class
        {
            return GetLogger(typeof(T));
        }

        public ILoggerEx GetLogger(Type type)
        {
            if (!_loggerContainer.TryGetValue(type, out var targetLogger))
            {
                targetLogger = new LoggerEX(type, PublishLogEvent);
                _loggerContainer[type] = targetLogger;
            }
            return targetLogger;
        }

        private void PublishLogEvent(in LogEvent logEvent)
        {
            foreach (var adapater in _logAdapters)
                adapater.DispatchLogEvent(logEvent);
        }

        protected override void DisposeManagedResources()
        {
            DisposeLoggerEXContainer();
            DisposeLogAdapters();
        }

        private void DisposeLogAdapters()
        {
            _logAdapters.Clear();
        }

        private void DisposeLoggerEXContainer()
        {
            foreach (var loggerEX in _loggerContainer.Values)
                loggerEX.Dispose();
            _loggerContainer.Clear();
        }
    }
}