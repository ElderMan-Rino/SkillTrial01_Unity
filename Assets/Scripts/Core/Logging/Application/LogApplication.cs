using Elder.Core.Common.BaseClasses;
using Elder.Core.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Elder.Core.Logging.Application
{
    public class LogApplication : SingletonBase<LogApplication>
    {
        private Dictionary<Type, Logger> _loggerContainer;
        private Subject<LogEvent> _logPublisher;

        public IObservable<LogEvent> LogEvents => _logPublisher;

        public LogApplication()
        {
            InitializeLoggerContainer();
            InitializeLogPublisher();
        }
        private void InitializeLogPublisher()
        {
            _logPublisher = new();
        }
        private void InitializeLoggerContainer()
        {
            _loggerContainer = new();
        }
        public ILoggerEx CreateLogger<T>() where T : class
        {
            var type = typeof(T);
            if (!_loggerContainer.TryGetValue(type, out var targetLogger))
            {
                targetLogger = new Logger(type, _logPublisher);
                _loggerContainer[type] = targetLogger;
            }
            return targetLogger;
        }
        protected override void DisposeManagedResources()
        {
            DisposeLogPublisher();
            DisposeLoggerContainer();
        }
        private void DisposeLoggerContainer()
        {
            foreach (var logger in _loggerContainer.Values)
                logger.Dispose();
            _loggerContainer = null;
        }
        private void DisposeLogPublisher()
        {
            _logPublisher.Dispose();
            _logPublisher = null;
        }
        protected override void DisposeUnmanagedResources()
        {

        }
    }
}
