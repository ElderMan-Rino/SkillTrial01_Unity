using Elder.Core.Logging.Application;
using System;

namespace Elder.Core.Logging.Interfaces
{
    public interface ILogAdapter : IDisposable
    {
        public void DispatchLogEvent(LogEvent logEvent);
    }
}
