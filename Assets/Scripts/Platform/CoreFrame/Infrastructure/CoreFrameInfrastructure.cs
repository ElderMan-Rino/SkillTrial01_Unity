using Elder.Core.Common.Enums;
using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Platform.CoreFrame.Infrastructure
{
    public class CoreFrameInfrastructure : IInfrastructureDisposer, IInfrastructureProvider, IInfrastructureRegister, ISubInfrastructureCreator, IApplicationProvider
    {
        private IInfrastructureFactory _infraFactory;
        private ISubInfrastructureFactory _subInfraFactory;
        private IApplicationProvider _appProvider;

        private Dictionary<Type, IInfrastructure> _persistentInfras;
        private Dictionary<Type, IInfrastructure> _sceneInfras;
        private Dictionary<Type, IInfrastructure> _logInfras;

        public CoreFrameInfrastructure(IInfrastructureFactory infrastructureFactory, ISubInfrastructureFactory subInfraFactory)
        {
            InjectInfrastructureFactory(infrastructureFactory);
            InjectSubInfrastructureFactory(subInfraFactory);

            InitializeLogInfrasContainer();
            InitializePersistenceInfrasContainer();
            InitializeSceneInfraContainer();
        }
        private void InjectSubInfrastructureFactory(ISubInfrastructureFactory subInfraFactory)
        {
            _subInfraFactory = subInfraFactory;
        }
        private void InjectInfrastructureFactory(IInfrastructureFactory infrastructureFactory)
        {
            _infraFactory = infrastructureFactory;
        }
        public void InjectAppProvider(IApplicationProvider appProvider)
        {
            _appProvider = appProvider;
        }
        private void InitializeSceneInfraContainer()
        {
            _sceneInfras = new();
        }
        private void InitializePersistenceInfrasContainer()
        {
            _persistentInfras = new();
        }
        private void InitializeLogInfrasContainer()
        {
            _logInfras = new();
        }
        public bool TryGetInfrastructure<T>(out T targetInfrastructure) where T : class, IInfrastructure
        {
            targetInfrastructure = null;
            var type = typeof(T);
            if (_persistentInfras.TryGetValue(type, out var persistentInfra))
            {
                targetInfrastructure = persistentInfra as T;
                return targetInfrastructure != null;
            }

            if (_sceneInfras.TryGetValue(type, out var sceneInfra))
            {
                targetInfrastructure = sceneInfra as T;
                return targetInfrastructure != null;
            }

            if (_logInfras.TryGetValue(type, out var logInfra))
            {
                targetInfrastructure = logInfra as T;
                return targetInfrastructure != null;
            }
            return false;
        }
        public void RegisterInfrastructure<T>() where T : IInfrastructure
        {
            var type = typeof(T);
            if (_persistentInfras.ContainsKey(type) || _sceneInfras.ContainsKey(type))
                return;

            if (!TryCreateInfrastructure(type, out var infrastructure))
                return;

            var infraContainer = infrastructure.InfraType switch
            {
                InfrastructureType.Persistent => _persistentInfras,
                InfrastructureType.Scene => _sceneInfras,
                InfrastructureType.Log => _logInfras,
                _ => null
            };
            if (infraContainer == null)
                return;

            infraContainer[type] = infrastructure;
        }
        public bool TryGetApplication<T>(out T targetApp) where T : class, IApplication
        {
            return _appProvider.TryGetApplication<T>(out targetApp);
        }
        public bool TryGetApplications<T>(out T[] targetApps) where T : class, IApplication
        {
            return _appProvider.TryGetApplications<T>(out targetApps);
        }
        private bool TryCreateInfrastructure(Type type, out IInfrastructure infra)
        {
            return _infraFactory.TryCreateInfrastructure(type, this, this, this, _appProvider, out infra);
        }
        public bool TryCreateSubInfra<T>(out ISubInfrastructure subInfra) where T : ISubInfrastructure
        {
            return _subInfraFactory.TryCreateSubInfra(typeof(T), out subInfra);
        }

        public void DisposeLogInfras()
        {
            ClearUpInfras(_logInfras);
            _logInfras = null;
        }
        private void ClearUpInfras(Dictionary<Type, IInfrastructure> infras)
        {
            foreach (var infra in infras.Values)
                infra.Dispose();
            infras.Clear();
        }
        public void DiposeSceneInfras()
        {
            ClearUpInfras(_sceneInfras);
            _sceneInfras = null;
        }
        public void DisposePersistentInfras()
        {
            ClearUpInfras(_persistentInfras);
            _persistentInfras = null;
        }
        public void Dispose()
        {
            ClearAppProvider();
            DiposeSubInfrastructureFactory();
            DisposeInfrastructureFactory();
        }
        private void ClearAppProvider()
        {
            _appProvider = null;
        }
        private void DisposeInfrastructureFactory()
        {
            _infraFactory.Dispose();
            _infraFactory = null;
        }
        private void DiposeSubInfrastructureFactory()
        {
            _subInfraFactory.Dispose();
            _subInfraFactory = null;
        }

        public void PreDiposeSceneInfras()
        {
            foreach (var infra in _sceneInfras.Values)
                infra.PreDispose();
        }
        public void PreDisposePersistentInfras()
        {
            foreach (var infra in _persistentInfras.Values)
                infra.PreDispose();
        }
    }
}
