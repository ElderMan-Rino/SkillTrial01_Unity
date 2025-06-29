using Elder.Framework.Asset.App;
using Elder.Framework.Asset.Infra;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Asset.Installer
{
    public readonly struct AssetSystemInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.RegisterShared<AddressableAssetLoader>().As<IEngineAssetLoader>().As<IEngineAssetReleaser>();
            // [HEAP] 등록 시점 1회
            registry.RegisterInstance<IAssetRegistry>(new AssetRegistry());
            registry.RegisterInstance<IAssetProvider>(new AssetSystem());
        }
    }
}