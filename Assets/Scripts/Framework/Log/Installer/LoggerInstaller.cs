using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Infra;
using Elder.Framework.Log.Interfaces;

namespace Elder.Framework.Log.Installer
{
    public readonly struct LoggerInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.Register<ILogAdapter, UnityLogAdapter>();
            registry.TryGetRegistered<ILogAdapter>(out var adapter);
            // [HEAP] 배열 1회 할당 — 등록 시점
            registry.RegisterInstance<ILoggerPublisher>(new LogPublisher(new[] { adapter }));
        }
    }
}