using Elder.Framework.Core.Interfaces;
using Elder.Framework.Settings.App;
using Elder.Framework.Settings.Infra;
using Elder.Framework.Settings.Interfaces;

namespace Elder.Framework.Settings.Installer
{
    public readonly struct SettingsInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.Register<IAppSettingsStore, PlayerPrefsSettingsStore>();
            registry.RegisterShared<SettingsApplyService>()
                .As<SettingsApplyService>();

            // ISettingsApplicable 구현체들을 수집해 SettingsApplyService에 등록
            var applicables = registry.GetAllRegistered<ISettingsApplicable>();
            if (registry.TryGetRegistered<SettingsApplyService>(out var service))
            {
                for (int i = 0; i < applicables.Count; i++)
                    service.RegisterApplicable(applicables[i]);
            }
        }
    }
}
