using Elder.Framework.Asset.Infra;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Core.UI.Installers;
using Elder.Framework.Singal.Installers;
using Elder.TestModule.AssetLoad.Infra;
using Elder.TestModule.AssetLoad.Interfaces;

namespace Elder.TestModule.AssetLoad.Installers
{
    public static class AssetLaodTestInstaller
    {
        public static void Install(IComponentRegistry registry)
        {
            SignalInstaller.Install(registry);
            UISystemInstaller.Install(registry);
            
            registry.Regist<AssetLoader>().As<IAssetLoadService>();
            registry.Regist<AssetCacheTest>().As<IAssetCacheTest>();

            registry.Regist<AssetLoadTestCoordinator>().As<IAssetLoadTestCoordinator>();
        }
    }
}
