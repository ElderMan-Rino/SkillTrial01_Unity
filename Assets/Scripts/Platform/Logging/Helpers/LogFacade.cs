using Elder.Core.Logging.Interfaces;
using System;

namespace Elder.Core.Logging.Helpers
{
    public static class LogFacade 
    {
        private static ILoggerPublisher _provider;

        public static void InjectProvider(ILoggerPublisher provider)
        {
            _provider = provider;
        }
        public static ILoggerEx GetLoggerFor<T>() where T : class
        {
            if (_provider == null)
                throw new InvalidOperationException("Log Provider not initialized.");
            return _provider.GetLogger<T>();
        }
        public static void CleanUp()
        {
            _provider = null;
        }
    }
}