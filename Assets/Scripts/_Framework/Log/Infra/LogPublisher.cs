using Elder.Framework.Core.Interfaces;
using Elder.Framework.Core.Definitions;
using Elder.Framework.Log.Definitions;
using Elder.Framework.Log.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Log.Infra
{
    internal sealed class LogPublisher :  ILoggerPublisher, ILogEventSource, IPostDisposable
    {
        public int Scope => GlobalScope.Id;
        public Type ClassType => typeof(LogPublisher);

        private readonly Dictionary<Type, LoggerEX> _loggerContainer = new();

        public event LogEmitHandler OnLogPublished;

        public ILoggerEx GetLogger<T>() where T : class
        {
            return GetLogger(typeof(T));
        }

        public ILoggerEx GetLogger(Type type)
        {
            if (!_loggerContainer.TryGetValue(type, out var targetLogger))
            {
                targetLogger = new LoggerEX(type, PublishLogEvent); // [HEAP] 타입당 1회
                _loggerContainer[type] = targetLogger;
            }
            return targetLogger;
        }

        private void PublishLogEvent(in LogEvent logEvent)
        {
            OnLogPublished?.Invoke(logEvent);
        }

        public void PostDispose()
        {
            DisposeLoggerEXContainer();
        }

        private void DisposeLoggerEXContainer()
        {
            foreach (var loggerEX in _loggerContainer)
                loggerEX.Value.Dispose();
            _loggerContainer.Clear();
        }

       
    }
}

