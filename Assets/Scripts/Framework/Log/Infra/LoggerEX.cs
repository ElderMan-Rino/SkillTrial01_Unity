using Elder.Framework.Common.Base;
using Elder.Framework.Log.Definitions;
using Elder.Framework.Log.Interfaces;
using System;

namespace Elder.Framework.Log.Infra
{
    internal sealed class LoggerEX : DisposableBase, ILoggerEx
    {
        private Type _ownerType;
        private LogHandler _logHandler;

        public LoggerEX(Type ownerType, LogHandler logHandler)
        {
            _ownerType = ownerType;
            _logHandler = logHandler;
        }

        [System.Diagnostics.Conditional("ENABLE_LOGGING")]
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

        protected override void DisposeManagedResources()
        {
            ClearLogAction();
        }

        private void ClearLogAction()
        {
            _logHandler = null;
        }
    }
}