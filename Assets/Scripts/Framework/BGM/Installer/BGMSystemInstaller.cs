using Elder.Framework.BGM.App;
using Elder.Framework.BGM.Infra;
using Elder.Framework.BGM.Interfaces;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.BGM.Installer
{
    public readonly struct BGMSystemInstaller
    {
        public void Install(IGameSystemRegistry registry)
        {
            registry.Register<BGMAssetLoader>().As<IBGMAssetLoader>();
            registry.Register<BGMSystem>().As<IBGMSystem>();
        }
    }
}
