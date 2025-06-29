using Elder.Framework.Audio.Interfaces;
using Elder.Framework.Core;
using Elder.Framework.Settings.Definitions;
using Elder.Framework.Settings.Interfaces;
using UnityEngine;

namespace Elder.Framework.Audio.App
{
    internal sealed class AudioSystem : BaseSystem, IAudioSystem, ISettingsApplicable
    {
        public float MasterVolume { get; private set; } = 1f;
        public float BgmVolume    { get; private set; } = 1f;
        public float SfxVolume    { get; private set; } = 1f;

        public void SetMasterVolume(float value) => MasterVolume = Mathf.Clamp01(value);
        public void SetBgmVolume(float value)    => BgmVolume    = Mathf.Clamp01(value);
        public void SetSfxVolume(float value)    => SfxVolume    = Mathf.Clamp01(value);

        public void ApplySettings(IAppSettingsStore store)
        {
            MasterVolume = store.GetFloat(SettingsKeys.AudioMaster, 1f);
            BgmVolume    = store.GetFloat(SettingsKeys.AudioBgm,    1f);
            SfxVolume    = store.GetFloat(SettingsKeys.AudioSfx,    1f);
        }
    }
}
