using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Localize.Interfaces
{
    public interface ILocaleSystem : IGameSystem
    {
        public string GetLanguageCode();
        public void SetLanguageCode(string languageCode);
    }
}
