using Elder.Core.Common.BaseClasses;
using Elder.Core.Logging.Interfaces;
using System;

namespace Elder.Core.Logging.Application
{
    public class Logger : DisposableBase, ILoggerEx
    {
        private Type _ownerType;
        private Action<LogEvent> _logAction;
        public Logger(Type ownerType, Action<LogEvent> logAction) 
        {
            _ownerType = ownerType;
            _logAction = logAction;
        }
        public void Error(string message)
        {
            PublishLog(LogEvent.Error(_ownerType, message));
        }
        public void Info(string message)
        {
            PublishLog(LogEvent.Info(_ownerType, message));
        }
        public void Debug(string message)
        {
            PublishLog(LogEvent.Debug(_ownerType, message));
        }
        public void Warning(string message)
        {
            PublishLog(LogEvent.Warning(_ownerType, message));
        }

        protected override void DisposeManagedResources()
        {
            ClearLogAction();
        }
        private void ClearLogAction()
        {
            _logAction = null;
        }
        protected override void DisposeUnmanagedResources()
        {

        }
        [System.Diagnostics.Conditional("ENABLE_LOGGING")]
        private void PublishLog(in LogEvent logEvent)
        {
            _logAction?.Invoke(logEvent);
        }
    }
}
