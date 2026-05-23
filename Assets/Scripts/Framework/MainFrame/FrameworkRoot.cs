using Elder.Framework.Boot.App;
using Elder.Framework.Common.App;
using Elder.Framework.Common.Interfaces;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Data.App;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.GameSystem.App;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.MainFrame.Infra.Configs;
using Elder.Framework.MainFrame.Installer;
using Elder.Framework.Scene.Interfaces;
using Elder.SkillTrial.Data.Installer;
using UnityEngine;

namespace Elder.Framework.MainFrame
{
    public class FrameworkRoot : MonoBehaviour
    {
        [SerializeField] private FrameworkSettings _frameworkSettings;

        private IGameSystemProvider _provider;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            var registry = new SystemRegistry();
            registry.RegisterInstance<IDataConfig>(_frameworkSettings);

            new GameInstaller().Install(registry);
            new FrameworkInstaller().Install(registry);

            _provider = registry.Build();

            OnBuilt(_provider);
        }

        private static void OnBuilt(IGameSystemProvider provider)
        {
            if (provider.TryResolve<ILoggerPublisher>(out var publisher))
                LogFacade.InjectProvider(publisher);

            provider.TryResolve<ScopeDisposeSystem>(out var scopeDispose);
            provider.TryResolve<DataInitializer>(out var dataInitializer);

            scopeDispose?.Initialize();
            scopeDispose?.Register(dataInitializer);

            if (provider.TryResolve<ISceneTransitionCoordinator>(out var coordinator))
                coordinator.Initialize();

            dataInitializer?.Initialize();

            if (provider.TryResolve<GameStartService>(out var startService))
                startService.Initialize();

            if (provider.TryResolve<GameBootStrapper>(out var bootStrapper))
                bootStrapper.Start();
        }

        private void OnDestroy()
        {
            LogFacade.CleanUp();
        }
    }
}
