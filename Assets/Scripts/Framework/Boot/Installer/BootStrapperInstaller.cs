using Elder.Framework.Boot.App;
using Elder.Framework.Boot.Infra;
using Elder.Framework.Boot.Interfaces;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Boot.Installer
{
    public readonly struct BootStrapperInstaller
    {
        public void Install(IGameSystemRegistry registry)
        {
            registry.Register<UnityStartupEnvironment>().As<IStartupEnvironment>();
            registry.Register<GameBootEntryPoint>().As<IGameBootEntryPoint>();
        }
    }
}