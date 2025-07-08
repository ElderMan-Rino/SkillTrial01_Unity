using Elder.Core.Common.Interfaces;
using Elder.Core.Logging.Application;

namespace Elder.Core.Logging.Interfaces
{
    public interface ILogEventHandler : IInfrastructure
    {
        public void HandleLogEvent(LogEvent logEvent);
    }
}
