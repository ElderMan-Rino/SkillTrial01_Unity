using Elder.Core.Common.Interfaces;
using Elder.Core.Logging.Application;

namespace Elder.Core.Logging.Interfaces
{
    public interface ILogEventDispatcher : IInfrastructure
    {
        public void DispatchLogEvent(in LogEvent logEvent);
    }
}
