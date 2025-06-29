using Elder.Framework.Log.Interfaces;
using System;

namespace Elder.Framework.Log.Helper
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
            return GetLoggerFor(typeof(T));
        }

        public static ILoggerEx GetLoggerFor(Type type)
        {
            if (_provider == null)
                throw new InvalidOperationException("Log Provider not initialized.");
            return _provider.GetLogger(type);
        }

        public static void CleanUp()
        {
            _provider = null;
        }
    }
}