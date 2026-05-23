using Elder.Framework.Localize.Interfaces;
using UnityEngine;

namespace Elder.Framework.Localize.App
{
    internal sealed class LocaleSystem : ILocaleSystem
    {
        private string _languageCode;

        public LocaleSystem()
        {
            _languageCode = ResolveSystemLanguage(UnityEngine.Application.systemLanguage);
        }

        public string GetLanguageCode() => _languageCode;

        public void SetLanguageCode(string languageCode) => _languageCode = languageCode;

        private static string ResolveSystemLanguage(SystemLanguage lang) => lang switch
        {
            SystemLanguage.Korean   => "Ko",
            SystemLanguage.Japanese => "Jp",
            _                       => "En",
        };
    }
}
