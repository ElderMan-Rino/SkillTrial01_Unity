using Elder.Framework.Asset.Installer;
using Elder.Framework.Boot.Installer;
using Elder.Framework.Data.Installer;
using Elder.Framework.Flux.Installer;
using Elder.Framework.Input.Installer;
using Elder.Framework.Log.Installer;
using Elder.Framework.Scene.Installer;
using VContainer;
using VContainer.Unity;

namespace Elder.Framework.MainFrame.Installer
{
    public readonly struct FrameworkInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            new LoggerInstaller().Install(builder);
            new FluxInstaller().Install(builder);
            new AssetSystemInstaller().Install(builder);
            new SceneSystemInstaller().Install(builder);
            new InputInstaller().Install(builder);
            new DataInstaller().Install(builder);
            new BootStrapperInstaller().Install(builder);
        }
    }
}