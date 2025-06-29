using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Settings.Interfaces
{
    public interface IAppSettingsStore : IGameSystem
    {
        public void Load();
        public void Save();
        public string GetString(string key, string defaultValue = "");
        public float GetFloat(string key, float defaultValue = 0f);
        public int GetInt(string key, int defaultValue = 0);
        public void SetString(string key, string value);
        public void SetFloat(string key, float value);
        public void SetInt(string key, int value);
        public bool HasKey(string key);
    }
}
