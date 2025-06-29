using Elder.Framework.Audio.App;
using Elder.Framework.Audio.Interfaces;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Settings.Interfaces;

namespace Elder.Framework.Audio.Installer
{
    public readonly struct AudioInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.RegisterShared<AudioSystem>()
                .As<IAudioSystem>()
                .As<ISettingsApplicable>();
        }
    }
}
