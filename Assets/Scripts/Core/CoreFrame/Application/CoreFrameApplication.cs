using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.FluxMessage.Interfaces;
using Elder.Core.GameLevel.Constants;
using Elder.Core.GameLevel.Messages;
using Elder.Core.Logging.Helpers;
using Elder.Core.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elder.Core.CoreFrame.Application
{
    public class CoreFrameApplication : DisposableBase, IApplicationProvider
    {
        private IInfrastructureProvider _infraProvider;
        private IInfrastructureRegister _infraRegister;
        private IInfrastructureDisposer _infraDisposer;

        private IApplicationFactory _appFactory;

        private IApplication _logApp;
        private Dictionary<Type, IApplication> _persistentApps;
        private Dictionary<Type, IApplication> _sceneApps;

        private ILoggerEx _logger;

        public CoreFrameApplication(IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister, IInfrastructureDisposer infraDisposer, IApplicationFactory applicationFactory)
        {
            InjectInfrastructureProvider(infraProvider);
            InjectInfrastructureRegister(infraRegister);
            InjectInfrastructureDisposer(infraDisposer);
            InjectApplicationFactory(applicationFactory);
        }
        public bool TryInitialize()
        {
            InitializePersistentAppsContainer();
            InitializeaSceneAppsContainer();

            if (!TryInitializeLogSystem())
                throw new InvalidOperationException("Log Provider not initialized.");

            if (!TryInitializePersistentApps())
            {
                _logger.Error("Failed to initialize persistent applications.");
                return false;
            }
            return true;
        }
        private bool TryInitializePersistentApps()
        {
            // 나중에 xml등으로 데이터 처리하는게 편할듯
            RegisterApplication<IFluxRouter>();
            return true;
        }
        private void InitializePersistentAppsContainer()
        {
            _persistentApps = new();
        }
        private void InitializeaSceneAppsContainer()
        {
            _sceneApps = new();
        }
        private bool TryInitializeLogSystem()
        {
            if (!TryInitializeLogApp())
                return false;

            if (!TryBindLogServiceToFacade())
                return false;

            if (!TryPostInitializeLogApp())
                return false;

            if (!TryBindLogger())
                return false;

            return true;
        }
        private bool TryBindLogger()
        {
            _logger = LogFacade.GetLoggerFor<CoreFrameApplication>();
            return _logger != null;
        }
        private bool TryPostInitializeLogApp()
        {
            return _logApp.TryPostInitialize();
        }
        private bool TryBindLogServiceToFacade()
        {
            if (_logApp is not ILoggerPublisher loggerPublisher)
                return false;

            LogFacade.InjectProvider(loggerPublisher);
            return true;
        }
        private bool TryInitializeLogApp()
        {
            if (!TryCreateApplication<ILoggerPublisher>(out var logApp))
                return false;

            _logApp = logApp;
            return true;
        }
        private void InjectApplicationFactory(IApplicationFactory appFactory)
        {
            _appFactory = appFactory;
        }
        public void RegisterApplication<T>() where T : IApplication
        {
            var type = typeof(T);
            if (_persistentApps.ContainsKey(type) || _sceneApps.ContainsKey(type))
                return;

            if (!TryCreateApplication<T>(out var app))
                return;

            var container = app.AppType switch
            {
                ApplicationType.Persistent => _persistentApps,
                ApplicationType.Scene => _sceneApps,
                _ => null
            };
            container[type] = app;
        }
        private bool TryCreateApplication<T>(out IApplication app) where T : IApplication
        {
            var type = typeof(T);
            return _appFactory.TryCreateApplication(type, this, _infraProvider, _infraRegister, out app);
        }
        private void InjectInfrastructureDisposer(IInfrastructureDisposer infraDisposer)
        {
            _infraDisposer = infraDisposer;
        }
        private void InjectInfrastructureRegister(IInfrastructureRegister infraRegister)
        {
            _infraRegister = infraRegister;
        }
        private void InjectInfrastructureProvider(IInfrastructureProvider infraProvider)
        {
            _infraProvider = infraProvider;
        }
        public bool TryGetApplication<T>(out T targetApp) where T : class, IApplication
        {
            var type = typeof(T);
            if (_persistentApps.TryGetValue(type, out var persistent))
            {
                targetApp = persistent as T;
                return targetApp != null;
            }

            if (_sceneApps.TryGetValue(type, out var scene))
            {
                targetApp = scene as T;
                return targetApp != null;
            }
            
            targetApp = null;
            return false;
        }
        public bool TryGetApplications<T>(out T[] targetApps) where T : class, IApplication
        {
            var matches = _persistentApps.Values.Concat(_sceneApps.Values).OfType<T>().ToArray();
            if (matches.Length > 0)
            {
                targetApps = matches;
                return true;
            }
            targetApps = null;
            return false;
        }
        public void RequestRunInitialScene()
        {
            if (!TryGetApplication<IFluxRouter>(out var fluxRouter))
            {
                _logger.Error("Failed to run initial scene: IFluxRouter is not registered in the application. Please ensure the messaging infrastructure is initialized before requesting scene changes.");
                return;
            }
            // 이것도 XML로 가져와야함
            fluxRouter.Publish<FxRequestGameLevelChange>(new FxRequestGameLevelChange(GameLevelConstants.INITIAL_SCENE_KEY));
        }
        protected override void DisposeManagedResources()
        {
            PreDisposeSceneApps();
            PreDisposeSceneInfras();

            DisposeSceneApps();
            DisposeSceneInfras();

            PreDisposePersistentApps();
            PreDisposePersistentInfras();

            DisposePersistentApps();
            DisposePersistentInfras();

            ClearInfraRegister();
            ClearInfraProvider();
        }
        private void PreDisposeSceneApps()
        {
            foreach (var app in _sceneApps.Values)
                app.PreDispose();
        }
        private void PreDisposeSceneInfras()
        {
            _infraDisposer.PreDiposeSceneInfras();
        }
        private void PreDisposePersistentApps()
        {
            foreach (var app in _persistentApps.Values)
                app.PreDispose();
        }
        private void PreDisposePersistentInfras()
        {
            _infraDisposer.PreDisposePersistentInfras();
        }
        private void DisposePersistentInfras()
        {
            _infraDisposer.DisposePersistentInfras();
        }
        private void DisposeSceneInfras()
        {
            _infraDisposer.DiposeSceneInfras();
        }
        private void DisposeSceneApps()
        {
            foreach (var app in _sceneApps.Values)
                app.Dispose();

            _sceneApps.Clear();
            _sceneApps = null;
        }
        private void DisposePersistentApps()
        {
            foreach (var app in _persistentApps.Values)
                app.Dispose();

            _persistentApps.Clear();
            _persistentApps = null;
        }
        private void ClearInfraRegister()
        {
            _infraRegister = null;
        }
        private void ClearInfraProvider()
        {
            _infraProvider = null;
        }
        protected override void DisposeUnmanagedResources()
        {

        }
        protected override void CompleteDispose()
        {
            DisposeApplicationFactory();
            ClearLogger();
            CleanUpLogFacade();
            DisposeLogApps();
            DisposeLogInfra();
            DisposeInfra();
        }
        private void ClearLogger()
        {
            _logger = null;
        }
        private void CleanUpLogFacade()
        {
            LogFacade.CleanUp();
        }
        private void DisposeLogApps()
        {
            _logApp.Dispose();
            _logApp = null;
        }
        private void DisposeLogInfra()
        {
            _infraDisposer.DisposeLogInfras();
        }
        private void DisposeInfra()
        {
            _infraDisposer.Dispose();
            _infraDisposer = null;
        }
        private void DisposeApplicationFactory()
        {
            _appFactory.Dispose();
            _appFactory = null;
        }
    }
}
