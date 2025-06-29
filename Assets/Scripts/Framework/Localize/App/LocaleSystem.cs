using Elder.Framework.Core;
using Elder.Framework.Localize.Interfaces;
using Elder.Framework.Settings.Definitions;
using Elder.Framework.Settings.Interfaces;
using UnityEngine;

namespace Elder.Framework.Localize.App
{
    internal sealed class LocaleSystem : BaseSystem, ILocaleSystem, ISettingsApplicable
    {
        private string _languageCode;

        public LocaleSystem()
        {
            _languageCode = ResolveSystemLanguage(Application.systemLanguage);
        }

        public string GetLanguageCode() => _languageCode;

        public void SetLanguageCode(string languageCode) => _languageCode = languageCode;

        public void ApplySettings(IAppSettingsStore store)
        {
            if (store.HasKey(SettingsKeys.Locale))
            {
                _languageCode = store.GetString(SettingsKeys.Locale);
                return;
            }

            // PlayerPrefs에 저장된 값 없으면 디바이스 언어 기준으로 초기화 후 저장
            _languageCode = ResolveSystemLanguage(Application.systemLanguage);
            store.SetString(SettingsKeys.Locale, _languageCode);
            store.Save();
        }

        private static string ResolveSystemLanguage(SystemLanguage lang) => lang switch
        {
            SystemLanguage.Korean   => "Ko",
            SystemLanguage.Japanese => "Jp",
            _                       => "En",
        };
    }
}
