using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.Logging.Application;
using System;
using UnityEngine;

namespace Elder.Unity.Logging.Infrastructure
{
    public class UnityLogger : SingletonBase<UnityLogger>
    {
        private IDisposable _logSubscription;

        [System.Diagnostics.Conditional("UNITY_LOGGER")]
        public void SubscribeToLogAppplication()
        {
            _logSubscription = LogApplication.In.LogEvents.Subscribe(HandleLogEvent);
        }
        private void HandleLogEvent(LogEvent logEvent)
        {
            switch (logEvent.Level)
            {
                case LogLevel.Debug:
                    LogDebug(logEvent);
                    break;
                case LogLevel.Info:
                    LogInfo(logEvent);
                    break;
                case LogLevel.Warning:
                    LogWarning(logEvent);
                    break;
                case LogLevel.Error:
                    LogError(logEvent);
                    break;
            }
        }

        private void LogDebug(LogEvent logEvent)
        {
            Debug.Log(FormatLogMessage(logEvent));
        }

        private void LogInfo(LogEvent logEvent)
        {
            Debug.Log(FormatLogMessage(logEvent));
        }

        private void LogWarning(LogEvent logEvent)
        {
            Debug.LogWarning(FormatLogMessage(logEvent));
        }

        private void LogError(LogEvent logEvent)
        {
            Debug.LogError(FormatLogMessage(logEvent));
        }

        private string FormatLogMessage(LogEvent logEvent)
        {
            return $"[{logEvent.Level}] <{logEvent.OwnerType.Name}> {logEvent.Message}";
        }
        protected override void DisposeManagedResources()
        {
            DisposeLogSubscription();
        }
        private void DisposeLogSubscription()
        {
            _logSubscription.Dispose();
            _logSubscription = null;
        }

        protected override void DisposeUnmanagedResources()
        {

        }
    }
}
