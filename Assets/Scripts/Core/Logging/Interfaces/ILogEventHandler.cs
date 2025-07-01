using Elder.Core.Logging.Application;

namespace Elder.Core.Logging.Interfaces
{
    public interface ILogEventHandler
    {
        public void HandleLogEvent(LogEvent logEvent);
    }
}
