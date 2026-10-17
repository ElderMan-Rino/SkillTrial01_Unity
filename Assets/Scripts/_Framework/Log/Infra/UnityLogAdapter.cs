using Elder.Framework.Core.Definitions;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Definitions;
using Elder.Framework.Log.Definitions.Enums;
using Elder.Framework.Log.Interfaces;
using System;
using UnityEngine;

namespace Elder.Framework.Log.Infra
{
    internal sealed class UnityLogAdapter : IFrameworkComponent, IDependencyInjectable, IPreDisposable
    {
        private ILogEventSource _logEventSource;

        public int Scope => GlobalScope.Id;
        public Type ClassType => typeof(UnityLogAdapter);

        public void InjectDependency(IComponentProvider provider)
        {
            if (!provider.TryGetComponent<ILogEventSource>(out _logEventSource))
                throw new InvalidOperationException($"{nameof(ILogEventSource)} not found for {nameof(UnityLogAdapter)}.");

            _logEventSource.OnLogPublished += HandleLogPublished;
        }

        private void HandleLogPublished(in LogEvent logEvent)
        {
            var logType = ToUnityLogType(logEvent.Level);
            Debug.unityLogger.Log(logType, FormatLogMessage(logEvent));
        }

        private string FormatLogMessage(in LogEvent logEvent)
        {
            return $"[{logEvent.Level}] <{logEvent.OwnerType.Name}> {logEvent.Message}";  // [HEAP] 문자열 보간
        }

        private LogType ToUnityLogType(LogLevel level) => level switch
        {
            LogLevel.Warning => LogType.Warning,
            LogLevel.Error => LogType.Error,
            _ => LogType.Log,
        };

        public void PreDispose()
        {
            if (_logEventSource is null)
                return;

            _logEventSource.OnLogPublished -= HandleLogPublished;
            _logEventSource = null;
        }
    }
}
