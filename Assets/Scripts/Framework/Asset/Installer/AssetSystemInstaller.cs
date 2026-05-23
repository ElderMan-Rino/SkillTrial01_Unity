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
            registry.RegisterShared<AddressableAssetLoader>()
                .As<IEngineAssetLoader>()
                .As<IEngineAssetReleaser>();
            registry.RegisterFactory<IAssetRegistry>(p =>
            {
                p.TryResolve<IEngineAssetLoader>(out var loader);
                p.TryResolve<IEngineAssetReleaser>(out var releaser);
                return new AssetRegistry(loader, releaser);
            });
            registry.RegisterFactory<IAssetProvider>(p =>
            {
                p.TryResolve<IEngineAssetLoader>(out var loader);
                p.TryResolve<IEngineAssetReleaser>(out var releaser);
                return new AssetSystem(loader, releaser);
            });
        }
    }
}