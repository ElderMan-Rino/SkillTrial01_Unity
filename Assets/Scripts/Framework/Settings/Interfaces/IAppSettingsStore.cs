using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Settings.Interfaces
{
    public interface IAppSettingsStore : IGameSystem
    {
        public bool TryGetString(string key, out string value, string defaultValue = "");
        public bool TryGetFloat(string key, out float value, float defaultValue = 0f);
        public bool TryGetInt(string key, out int value, int defaultValue = 0);
        public bool HasKey(string key);
    }
}
