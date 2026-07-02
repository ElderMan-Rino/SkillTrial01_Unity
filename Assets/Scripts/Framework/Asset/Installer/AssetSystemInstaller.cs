using Elder.Framework.Asset.App;
using Elder.Framework.Asset.Infra;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Asset.Installer
{
    public readonly struct AssetSystemInstaller
    {
        public void Install(IGameSystemRegistry registry)
        {
            registry.Register<AddressableAssetLoader>().As<IEngineAssetLoader>().As<IEngineAssetReleaser>();
            registry.Register<AssetRegistry>().As<IAssetRegistry>();
            registry.Register<AssetSystem>().As<IAssetProvider>();
        }
    }
}