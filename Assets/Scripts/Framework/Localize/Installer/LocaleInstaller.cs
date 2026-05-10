using Elder.Framework.Localize.App;
using Elder.Framework.Localize.Interfaces;
using VContainer;
using VContainer.Unity;

namespace Elder.Framework.Localize.Installer
{
    public readonly struct LocaleInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<LocaleSystem>(Lifetime.Singleton).As<ILocaleSystem>();
        }
    }
}
