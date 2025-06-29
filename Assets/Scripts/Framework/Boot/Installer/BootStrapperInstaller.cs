using Elder.Framework.Boot.App;
using Elder.Framework.Boot.Infra.Unity;
using Elder.Framework.Boot.Interfaces;
using VContainer;
using VContainer.Unity;

namespace Elder.Framework.Boot.Installer
{
    public readonly struct BootStrapperInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<UnityStartupEnvironment>(Lifetime.Singleton).As<IStartupEnvironment>();
            builder.RegisterEntryPoint<GameBootStrapper>();
        }
    }
}