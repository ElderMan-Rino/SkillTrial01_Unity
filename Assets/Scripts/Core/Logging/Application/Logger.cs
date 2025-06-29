using Elder.Core.Common.BaseClasses;
using Elder.Core.Logging.Interfaces;
using System;

namespace Elder.Core.Logging.Application
{
    public class Logger : DisposableBase, ILoggerEx
    {
        private Type _ownerType;
        private IObserver<LogEvent> _logObserver;
        public Logger(Type ownerType, IObserver<LogEvent> logObserver) 
        {
            _ownerType = ownerType;
            _logObserver = logObserver;
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
            DisposeEmitLogAction();
        }
        private void DisposeEmitLogAction()
        {
            _logObserver = null;
        }
        protected override void DisposeUnmanagedResources()
        {

        }
        [System.Diagnostics.Conditional("LOGGER")]
        private void PublishLog(in LogEvent logEvent)
        {
            _logObserver.OnNext(logEvent);
        }
    }
}
