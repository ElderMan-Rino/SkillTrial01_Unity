using Elder.Framework.Boot.App;
using Elder.Framework.Boot.Infra.Unity;
using Elder.Framework.Boot.Interfaces;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Boot.Installer
{
    public readonly struct BootStrapperInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.Register<IStartupEnvironment, UnityStartupEnvironment>();
            // [HEAP] 등록 시점 1회
            registry.RegisterInstance<GameBootStrapper>(new GameBootStrapper());
            registry.RegisterInstance<GameStartService>(new GameStartService());
        }
    }
}