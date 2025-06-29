using Elder.Core.Common.Enums;
using System;

namespace Elder.Core.Logging.Application
{
    public readonly struct LogEvent
    {
        public readonly Type OwnerType;
        public readonly LogLevel Level;
        public readonly string Message;

        private LogEvent(Type ownerType, LogLevel level, string message)
        {
            OwnerType = ownerType;
            Level = level;
            Message = message;
        }
        public static LogEvent Debug(Type ownerType, string message)
        {
            return new LogEvent(ownerType, LogLevel.Debug, message);
        }
        public static LogEvent Info(Type ownerType, string message)
        {
            return new LogEvent(ownerType, LogLevel.Info, message);
        }
        public static LogEvent Warning(Type ownerType, string message)
        {
            return new LogEvent(ownerType, LogLevel.Warning, message);
        }
        public static LogEvent Error(Type ownerType, string message)
        {
            return new LogEvent(ownerType, LogLevel.Error, message);
        }
    }
}
