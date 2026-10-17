using Elder.Framework.Asset.Infra;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Asset.Installers
{
    public static class AssetSystemInstaller 
    {
        public static void Install(IComponentRegistry registry)
        {
            registry.Regist<AssetLoader>().As<IAssetLoadService>();
            registry.Regist<AssetCache>().As<IAssetCache>();
        }
    }
}