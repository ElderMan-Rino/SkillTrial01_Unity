using Elder.Framework.Core.Interfaces;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Scene.App;
using Elder.Framework.Scene.Infra;
using Elder.Framework.Scene.Interfaces;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Scene.Installer
{
    public readonly struct SceneSystemInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.Register<ISceneContextFactory, SceneContextFactory>();
            registry.Register<ISceneLoader, AddressableSceneLoader>();
            registry.RegisterFactory<ISceneTransitionExecutor>(p =>
            {
                p.TryResolve<ISceneLoader>(out var loader);
                p.TryResolve<ISceneContextFactory>(out var factory);
                p.TryResolve<IDataProvider>(out var dataProvider);
                p.TryResolve<ILoggerPublisher>(out var logger);
                return new SceneTransitionExecutor(loader, factory, dataProvider, logger);
            });
            registry.RegisterFactory<ISceneTransitionCoordinator>(p =>
            {
                p.TryResolve<ISignalRouter>(out var router);
                p.TryResolve<ISceneTransitionExecutor>(out var executor);
                p.TryResolve<ILoggerPublisher>(out var logger);
                return new SceneTransitionCoordinator(router, executor, logger);
            });
        }
    }
}
