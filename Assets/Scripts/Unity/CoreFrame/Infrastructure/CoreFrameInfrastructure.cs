using Elder.Core.Common.Enums;
using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Unity.CoreFrame.Infrastructure
{
    public class CoreFrameInfrastructure : IInfrastructureDisposer, IInfrastructureProvider, IInfrastructureRegister
    {
        private IInfrastructureFactory _infrastructureFactory;

        private Dictionary<Type, IInfrastructure> _persistentInfras;
        private Dictionary<Type, IInfrastructure> _sceneInfras;

        public CoreFrameInfrastructure(IInfrastructureFactory infrastructureFactory)
        {
            InjectInfrastructureFactory(infrastructureFactory);
            InitializePersistenceInfrastructures();
        }
        private void InjectInfrastructureFactory(IInfrastructureFactory infrastructureFactory)
        {
            _infrastructureFactory = infrastructureFactory;
        }
        private void InitializePersistenceInfrastructures()
        {
            _persistentInfras = new();
        }
        public bool TryGetInfrastructure<T>(out T targetInfrastructure) where T : class, IInfrastructure
        {
            targetInfrastructure = null;
            if (_persistentInfras.TryGetValue(typeof(T), out var persistent))
            {
                targetInfrastructure = persistent as T;
                return targetInfrastructure != null;
            }

            if (_sceneInfras.TryGetValue(typeof(T), out var scene))
            {
                targetInfrastructure = scene as T;
                return targetInfrastructure != null;
            }
            return false;
        }
        public void RegisterInfrastructure<T>() where T : IInfrastructure
        {
            var type = typeof(T);
            if (_persistentInfras.ContainsKey(type) || _sceneInfras.ContainsKey(type))
                return;
            
            if (!_infrastructureFactory.TryCreateInfrastructure(type, this, this, out var infrastructure))
                return;
             
            var infraContainer = infrastructure.InfraType switch
            {
                InfrastructureType.Persistent => _persistentInfras,
                InfrastructureType.Scene => _sceneInfras,
                _ => null
            };
            if (infraContainer == null)
                return;

            infraContainer[type] = infrastructure;
        }
        public void DiposeSceneInfras()
        {
            foreach (var infra in _sceneInfras.Values)
                infra.Dispose();
            _sceneInfras.Clear();
            _sceneInfras = null;
        }
        public void DisposePersistentInfras()
        {
            foreach (var infra in _persistentInfras.Values)
                infra.Dispose();
            _persistentInfras.Clear();
            _persistentInfras = null;
        }

        public void Dispose()
        {
            DisposeInfrastructureFactory();
        }
        private void DisposeInfrastructureFactory()
        {
            _infrastructureFactory.Dispose();
            _infrastructureFactory = null;
        }
    }
}
