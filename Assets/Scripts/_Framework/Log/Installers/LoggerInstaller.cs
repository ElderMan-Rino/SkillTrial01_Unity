using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Infra;
using Elder.Framework.Log.Interfaces;

namespace Elder.Framework.Log.Installers
{
    public static class LoggerInstaller
    {
        public static void Install(IComponentRegistry registry)
        {
            registry.Regist<UnityLogAdapter>();
            registry.Regist<LogPublisher>().As<ILoggerPublisher>().As<ILogEventSource>();
        }
    }
}

