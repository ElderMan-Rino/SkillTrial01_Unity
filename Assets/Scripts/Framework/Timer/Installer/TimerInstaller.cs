using Elder.Framework.Core.Interfaces;
using Elder.Framework.Timer;
using Elder.Framework.Timer.Interfaces;

namespace Elder.Framework.Timer.Installer
{
    public readonly struct TimerInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.Register<ITimerService, TimerService>();
        }
    }
}
