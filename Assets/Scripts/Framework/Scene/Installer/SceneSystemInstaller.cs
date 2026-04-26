using Elder.Framework.Scene.App;
using Elder.Framework.Scene.Infra;
using Elder.Framework.Scene.Interfaces;
using VContainer;
using VContainer.Unity;

namespace Elder.Framework.Scene.Installer
{
    public readonly struct SceneSystemInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<SceneContextFactory>(Lifetime.Singleton).As<ISceneContextFactory>();
            builder.Register<AddressableSceneLoader>(Lifetime.Singleton).As<ISceneLoader>();
            builder.RegisterEntryPoint<SceneTransitionCoordinator>(Lifetime.Singleton).As<ISceneTransitionCoordinator>();
        }
    }
}
