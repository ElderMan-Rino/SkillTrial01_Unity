using Elder.Framework.Boot.App;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Boot.Interfaces;
using Elder.Framework.GameSystem.App;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.MainFrame.Infra.Configs;
using Elder.Framework.MainFrame.Installer;
using Elder.SkillTrial.Data.Installer;
using UnityEngine;

namespace Elder.Framework.MainFrame
{
    public class FrameworkRoot : MonoBehaviour
    {
        private IGameSystemProvider _provider;

        public void Initialize(FrameworkSettings settings)
        {
            DontDestroyOnLoad(gameObject);

            var registry = new SystemRegistry();
            registry.RegisterInstance<IBootConfig>(settings);

            new GameInstaller().Install(registry);
            new FrameworkInstaller().Install(registry);

            var provider = registry.Build();
            _provider = provider;

            if (provider.TryGetSystem<ILoggerPublisher>(out var publisher))
                LogFacade.InjectProvider(publisher);

            provider.InjectAll();
            provider.InitializeAll();
            provider.PostInitializeAll();

            if (provider.TryGetSystem<GameBootStrapper>(out var bootStrapper))
                bootStrapper.Start();
        }

        private void OnDestroy()
        {
            _provider?.DisposeAll();
            LogFacade.CleanUp();
        }
    }
}
