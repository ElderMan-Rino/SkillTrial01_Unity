using Elder.Framework.Log.Infra;
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

        public static ILoggerEx GetLogger<T>() where T : class
        {
            return GetLogger(typeof(T));
        }

        public static ILoggerEx GetLogger(Type type)
        {
            if (_provider is null) return NullLoggerEx.Instance;
            return _provider.GetLogger(type);
        }

        public static void CleanUp()
        {
            _provider = null;
        }
    }
}
