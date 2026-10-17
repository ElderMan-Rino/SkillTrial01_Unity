using Elder.Framework.Log.Definitions;
using Elder.Framework.Log.Interfaces;
using System;
using System.Diagnostics;

namespace Elder.Framework.Log.Infra
{
    internal sealed class LoggerEX : IDisposable, ILoggerEx
    {
        private Type _ownerType;
        private LogEmitHandler _logHandler;

        public LoggerEX(Type ownerType, LogEmitHandler logHandler)
        {
            _ownerType = ownerType;
            _logHandler = logHandler;
        }

        [Conditional("ENABLE_LOGGING")]
        private void PublishLog(in LogEvent logEvent)
        {
            _logHandler?.Invoke(logEvent);
        }

        public void Debug(string message)
        {
            PublishLog(LogEvent.Debug(_ownerType, message));
        }

        public void Info(string message)
        {
            PublishLog(LogEvent.Info(_ownerType, message));
        }

        public void Warn(string message)
        {
            PublishLog(LogEvent.Warning(_ownerType, message));
        }

        public void Error(string message)
        {
            PublishLog(LogEvent.Error(_ownerType, message));
        }

        public void Dispose()
        {
            _logHandler = null;
        }
    }
}