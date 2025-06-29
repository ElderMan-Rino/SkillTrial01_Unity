using Elder.Framework.Core;
using Elder.Framework.Settings.Interfaces;
using UnityEngine;

namespace Elder.Framework.Settings.Infra
{
    internal sealed class PlayerPrefsSettingsStore : BaseSystem, IAppSettingsStore
    {
        public void Load() { }

        public void Save() => PlayerPrefs.Save();

        public string GetString(string key, string defaultValue = "") => PlayerPrefs.GetString(key, defaultValue);
        public float GetFloat(string key, float defaultValue = 0f)    => PlayerPrefs.GetFloat(key, defaultValue);
        public int GetInt(string key, int defaultValue = 0)            => PlayerPrefs.GetInt(key, defaultValue);

        public void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
        public void SetFloat(string key, float value)   => PlayerPrefs.SetFloat(key, value);
        public void SetInt(string key, int value)       => PlayerPrefs.SetInt(key, value);

        public bool HasKey(string key) => PlayerPrefs.HasKey(key);
    }
}
