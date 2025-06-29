using Elder.Framework.Common.Base;
using Elder.Framework.Log.Definitions;
using Elder.Framework.Log.Definitions.Enums;
using Elder.Framework.Log.Interfaces;
using System;
using UnityEngine;

namespace Elder.Framework.Log.Infra
{
    [Serializable]
    public class UnityLogAdapter : DisposableBase, ILogAdapter
    {
        public void DispatchLogEvent(in LogEvent logEvent)
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
    }
}