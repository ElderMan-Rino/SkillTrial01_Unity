using Elder.Framework.Core.Interfaces;
using Elder.Framework.Time.App;
using Elder.Framework.Time.Interfaces;

namespace Elder.Framework.Time.Installer
{
    public readonly struct TimeInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.Register<ITimeService, TimeService>();
            registry.Register<ITimerService, TimerService>();
        }
    }
}
