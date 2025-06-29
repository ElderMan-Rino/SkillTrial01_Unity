using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Audio.Interfaces
{
    public interface IAudioSystem : IGameSystem
    {
        public float MasterVolume { get; }
        public float BgmVolume    { get; }
        public float SfxVolume    { get; }

        public void SetMasterVolume(float value);
        public void SetBgmVolume(float value);
        public void SetSfxVolume(float value);
    }
}
