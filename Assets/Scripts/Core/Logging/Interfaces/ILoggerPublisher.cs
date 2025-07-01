using Elder.Core.Common.Interfaces;

namespace Elder.Core.Logging.Interfaces
{
    public interface ILoggerPublisher : IApplication
    {
        public ILoggerEx GetLogger<T>() where T : class;
    }
}
