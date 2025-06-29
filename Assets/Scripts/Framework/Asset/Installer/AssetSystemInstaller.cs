using Elder.Framework.Asset.App;
using Elder.Framework.Asset.Infra;
using Elder.Framework.Asset.Interfaces;
using VContainer;
using VContainer.Unity;

namespace Elder.Framework.Asset.Installer
{
    public readonly struct AssetSystemInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<AddressableResourceLoader>(Lifetime.Singleton).As<IEngineAssetLoader>().As<IEngineAssetReleaser>();
            builder.Register<AssetRegistry>(Lifetime.Singleton).As<IAssetRegistry>();
            builder.Register<AssetSystem>(Lifetime.Singleton).As<IAssetProvider>();
        }
    }
}