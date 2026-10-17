using Elder.Framework.Core.Infra;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Infra;
using Elder.Framework.Log.Installers;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Root.Interfaces;
using System;

namespace Elder.TestModule.Common.Boot
{
    internal sealed class TestModuleRoot : IMainFrameworkRoot, IComponentProvider
    {
        private readonly ComponentRegistry _registry = new();
        private readonly Action<IComponentRegistry> _installAction;
        private ILoggerEx _logger;

        public TestModuleRoot(Action<IComponentRegistry> installAction)
        {
            _installAction = installAction;
        }

        public void Initialize()
        {
            BootstrapLogger();
            _installAction(_registry);
            _registry.InjectDependencies();
        }

        private void BootstrapLogger()
        {
            LoggerInstaller.Install(_registry);
            if (!TryGetComponent<ILoggerPublisher>(out var logPublisher))
                throw new InvalidOperationException($"Failed to resolve {nameof(LogPublisher)} after {nameof(LoggerInstaller)} installation.");
            LogFacade.InjectProvider(logPublisher);
            _logger = LogFacade.GetLogger<TestModuleRoot>();
        }

        public bool TryGetComponent<T>(out T target) where T : IFrameworkComponent
        {
            return _registry.TryGetComponent(out target);
        }

        public void MapAlias(Type aliasType, Type classType)
        {
            _registry.MapAlias(aliasType, classType);
        }

        public void ShutDown()
        {
            _registry.ClearAliasTypes();
            _registry.PreDisposeComponents();
            _registry.DisposeComponents();
            LogFacade.CleanUp();
            _registry.PostDisposeComponents();
        }
    }
}
