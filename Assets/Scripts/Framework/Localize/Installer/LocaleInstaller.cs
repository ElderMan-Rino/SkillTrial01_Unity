using Elder.Framework.Core.Interfaces;
using Elder.Framework.Localize.App;
using Elder.Framework.Localize.Interfaces;
using Elder.Framework.Settings.Interfaces;

namespace Elder.Framework.Localize.Installer
{
    public readonly struct LocaleInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.RegisterShared<LocaleSystem>()
                .As<ILocaleSystem>()
                .As<ISettingsApplicable>();
        }
    }
}
