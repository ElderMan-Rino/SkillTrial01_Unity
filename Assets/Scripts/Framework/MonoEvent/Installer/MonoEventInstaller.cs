using Elder.Framework.Core.Interfaces;
using Elder.Framework.MonoEvent.App;
using Elder.Framework.MonoEvent.Interfaces;

namespace Elder.Framework.MonoEvent.Installer
{
    public readonly struct MonoEventInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.Register<IMonoEventBus, MonoEventSystem>();
        }
    }
}
