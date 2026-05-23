using Elder.Framework.Asset.Installer;
using Elder.Framework.Boot.Installer;
using Elder.Framework.Common.Installer;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Crypto.Installer;
using Elder.Framework.Data.Installer;
using Elder.Framework.Domain.Installer;
using Elder.Framework.Input.Installer;
using Elder.Framework.Localize.Installer;
using Elder.Framework.Log.Installer;
using Elder.Framework.Scene.Installer;
using Elder.Framework.Signal.Installer;

namespace Elder.Framework.MainFrame.Installer
{
    public readonly struct FrameworkInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            new LoggerInstaller().Install(registry);
            new SignalInstaller().Install(registry);
            new CommonInstaller().Install(registry);
            new DomainInstaller().Install(registry);
            new AssetSystemInstaller().Install(registry);
            new SceneSystemInstaller().Install(registry);
            new InputInstaller().Install(registry);
            new CryptoInstaller().Install(registry);  // DataInstaller보다 먼저 등록
            new LocaleInstaller().Install(registry);
            new DataInstaller().Install(registry);
            new BootStrapperInstaller().Install(registry);
        }
    }
}