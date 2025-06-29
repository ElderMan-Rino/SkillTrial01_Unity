using Elder.Framework.Core;
using Elder.Framework.Log.Definitions;
using Elder.Framework.Log.Definitions.Enums;
using Elder.Framework.Log.Interfaces;
using System;
using UnityEngine;

namespace Elder.Framework.Log.Infra
{
    [Serializable]
    internal sealed class UnityLogAdapter : BaseSystem, ILogAdapter
    {
        public void DispatchLogEvent(in LogEvent logEvent)
        {
            var logType = ToUnityLogType(logEvent.Level);
            Debug.unityLogger.Log(logType, FormatLogMessage(logEvent));
        }

        private static LogType ToUnityLogType(LogLevel level) => level switch
        {
            LogLevel.Warning => LogType.Warning,
            LogLevel.Error   => LogType.Error,
            _                => LogType.Log,
        };

        private static string FormatLogMessage(in LogEvent logEvent)
        {
            return $"[{logEvent.Level}] <{logEvent.OwnerType.Name}> {logEvent.Message}";  // [HEAP] 문자열 보간
        }
    }
}