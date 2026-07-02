using Elder.Framework.Core.Interfaces;
using Elder.Framework.Settings.App;
using Elder.Framework.Settings.Infra;
using Elder.Framework.Settings.Interfaces;

namespace Elder.Framework.Settings.Installer
{
    public readonly struct SettingsInstaller
    {
        // ✅ OK: 폴더 위치 — Installer/ (DI 등록)
        // ✅ OK: public readonly struct — Installer 패턴 준수
        public void Install(IGameSystemRegistry registry)
        {
            registry.Register<PlayerPrefsSettingsStore>().As<IAppSettingsStore>();
            // ❌ VIOLATION: SettingsApplyService 미등록 — ISettingsApplicable 적용 서비스 비활성화
            //registry.RegisterShared<SettingsApplyService>().As<SettingsApplyService>();

            // ❌ VIOLATION: GetAllRegistered/TryGetRegistered — IGameSystemRegistry에 존재하지 않는 API 사용
            //var applicables = registry.GetAllRegistered<ISettingsApplicable>();
            //if (registry.TryGetRegistered<SettingsApplyService>(out var service))
            //{
            //    for (int i = 0; i < applicables.Count; i++)
            //        service.RegisterApplicable(applicables[i]);
            //}
        }
    }
}
