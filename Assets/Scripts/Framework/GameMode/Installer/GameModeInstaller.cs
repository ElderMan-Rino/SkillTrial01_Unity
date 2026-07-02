using Elder.Framework.Composition;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.GameMode.App;
using Elder.Framework.GameMode.Interfaces;
using Elder.Framework.GameMode.Main.App;
using Elder.Framework.GameMode.Preload.App;
using Elder.Framework.GameMode.Splash.App;
using Elder.Framework.GameMode.Title.App;

namespace Elder.Framework.GameMode.Installer
{
    public readonly struct GameModeInstaller
    {
        public void Install(IGameSystemRegistry registry)
        {
            registry.Register<GameSystemRegistryFactory>().As<IGameSystemRegistryFactory>();
            registry.Register<GameModeOrchestratorFactory>().As<IGameModeOrchestratorFactory>();
            registry.Register<TransitionDirector>().As<ITransitionDirector>();
            registry.Register<SplashOrchestratorRegistrar>();
            //registry.Register<PreloadOrchestratorRegistrar>();
            //registry.Register<TitleOrchestratorRegistrar>();
            //registry.Register<MainOrchestratorRegistrar>();
        }
    }
}
