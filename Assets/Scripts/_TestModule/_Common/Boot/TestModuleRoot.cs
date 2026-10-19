using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Infra;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Infra;
using Elder.Framework.Log.Installers;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Root.Interfaces;
using Elder.Framework.Signal.Interfaces;
using Elder.Framework.Signal.Messages;
using Elder.Framework.Singal.Installers;
using System;
using System.Threading;

namespace Elder.TestModule.Common.Boot
{
    internal sealed class TestModuleRoot : IMainFrameworkRoot, IComponentProvider
    {
        private readonly ComponentRegistry _registry = new();
        private ILoggerEx _logger;

     
        public bool TryGetComponent<T>(out T target) where T : IFrameworkComponent
        {
            return _registry.TryGetComponent(out target);
        }

        public async UniTask BootModuleAsync(Action<IComponentRegistry> installAction, Type startCoordinatorType, CancellationToken cancellationToken)
        {
            BootstrapLogger();
            installAction?.Invoke(_registry);
            _registry.InjectDependencies();
            await _registry.InitializeComponentsAsync(cancellationToken);
            await _registry.PostInitializeComponentsAsync(cancellationToken);

            if (!TryGetComponent<ISignalPublisher>(out var publisher))
            {
                _logger.Error($"Failed to resolve {nameof(ISignalPublisher)} after {nameof(SignalInstaller)} installation."); // [HEAP]
                throw new InvalidOperationException($"Failed to resolve {nameof(ISignalPublisher)} after {nameof(SignalInstaller)} installation."); // [HEAP]
            }

            publisher.Publish(new FxModuleStartSignal(startCoordinatorType));
        }

        private void BootstrapLogger()
        {
            LoggerInstaller.Install(_registry);
            if (!TryGetComponent<ILoggerPublisher>(out var logPublisher))
                throw new InvalidOperationException($"Failed to resolve {nameof(LogPublisher)} after {nameof(LoggerInstaller)} installation.");
            LogFacade.InjectProvider(logPublisher);
            _logger = LogFacade.GetLogger<TestModuleRoot>();
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
