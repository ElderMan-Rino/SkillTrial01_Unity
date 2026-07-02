using Elder.Framework.Core.Interfaces;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.GameMode.Interfaces;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Scene.App;
using Elder.Framework.Scene.Infra;
using Elder.Framework.Scene.Interfaces;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Scene.Installer
{
    public readonly struct SceneSystemInstaller
    {
        public void Install(IGameSystemRegistry registry)
        {
            registry.Register<SceneContextFactory>().As<ISceneContextFactory>();
            registry.Register<AddressableSceneLoader>().As<ISceneLoader>();
            registry.Register<SceneChanger>().As<ISceneChanger>();
        }
    }
}
