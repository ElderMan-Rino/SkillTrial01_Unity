using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Infra;
using Elder.Framework.Log.Interfaces;

namespace Elder.Framework.Log.Installer
{
    public readonly struct LoggerInstaller
    {
        public void Install(IGameSystemRegistry registry)
        {
            registry.Register<UnityLogAdapter>().As<ILogAdapter>();
            registry.Register<LogPublisher>().As<ILoggerPublisher>();
        }
    }
}