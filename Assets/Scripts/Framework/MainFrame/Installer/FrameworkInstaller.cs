using Elder.Framework.Asset.Installer;
using Elder.Framework.Boot.Installer;
using Elder.Framework.Boot.Interfaces;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Crypto.Installer;
using Elder.Framework.Blob.Installer;
using Elder.Framework.GameMode.Installer;
using Elder.Framework.Localize.Installer;
using Elder.Framework.Log.Installer;
using Elder.Framework.MonoEvent.Installer;
using Elder.Framework.Scene.Installer;
using Elder.Framework.Settings.Installer;
using Elder.Framework.Signal.Installer;
using Elder.Framework.UI.Installer;

namespace Elder.Framework.MainFrame.Installer
{
    public readonly struct FrameworkInstaller
    {
        public void Install(IGameSystemRegistry registry, IBootConfig bootConfig)
        {
            new LoggerInstaller().Install(registry);
            new SignalInstaller().Install(registry);
            new AssetSystemInstaller().Install(registry);
            new SceneSystemInstaller().Install(registry);
            new CryptoInstaller().Install(registry, bootConfig);
            new MonoEventInstaller().Install(registry);
            new UISystemInstaller().Install(registry);
            new GameModeInstaller().Install(registry);
            new LocaleInstaller().Install(registry);
            new SettingsInstaller().Install(registry);  // ISettingsApplicable 수집
            new DataInstaller().Install(registry);
            new BootStrapperInstaller().Install(registry);
        }
    }
}
