using Elder.Framework.Boot.App;
using Elder.Framework.Boot.Infra.Unity;
using Elder.Framework.Boot.Interfaces;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Boot.Installer
{
    public readonly struct BootStrapperInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.Register<IStartupEnvironment, UnityStartupEnvironment>();
            registry.RegisterFactory<GameBootStrapper>(p =>
            {
                p.TryResolve<ISignalRouter>(out var router);
                p.TryResolve<IStartupEnvironment>(out var startUp);
                return new GameBootStrapper(router, startUp);
            });
            registry.RegisterFactory<GameStartService>(p =>
            {
                p.TryResolve<ISignalRouter>(out var router);
                return new GameStartService(router);
            });
        }
    }
}