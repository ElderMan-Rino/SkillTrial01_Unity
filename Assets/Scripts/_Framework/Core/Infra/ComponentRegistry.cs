using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Root.Infra;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Core.Infra
{
    public sealed class ComponentRegistry : IComponentProvider, IComponentRegistry
    {
        private ILoggerEx _logger = LogFacade.GetLogger<ComponentRegistry>();

        // key는 실제 class의 타입이 되어야함
        private readonly Dictionary<Type, ComponentEntry> _componentEntrys = new();
        private readonly Dictionary<Type, Type> _aliasTypes = new();


        public bool TryGetComponent<T>(out T target) where T : IFrameworkComponent
        {
            var requestType = typeof(T);
            if (!_componentEntrys.TryGetValue(requestType, out var entry) &&
                (!_aliasTypes.TryGetValue(requestType, out var classType) || !_componentEntrys.TryGetValue(classType, out entry)))
            {
                target = default;
                return false;
            }

            target = (T)entry.Instance;
            return target != null;
        }

        public ISharedRegistration Regist<T>() where T : class, IFrameworkComponent, new()
        {
            var classType = typeof(T);
            if (_componentEntrys.ContainsKey(classType))
            {
                _logger.Error($"Component type already registered: {classType}"); // [HEAP]
                throw new InvalidOperationException($"Component type already registered: {classType}"); // [HEAP]
            }

            _componentEntrys[classType] = new ComponentEntry(new T(), this);
            return _componentEntrys[classType];
        }

        public void MapAlias(Type aliasType, Type classType)
        {
            if (!_aliasTypes.TryAdd(aliasType, classType))
            {
                _logger.Error($"Component type already registered: {classType}"); // [HEAP]
                throw new InvalidOperationException($"Component type already registered: {classType}"); // [HEAP]
            }
        }

        public void InjectDependencies()
        {
            foreach (var entry in _componentEntrys.Values)
            {
                if (entry.Instance is IDependencyInjectable injectable)
                    injectable.InjectDependency(this);
            }
        }

        public void ClearAliasTypes()
        {
            _aliasTypes.Clear();
        }

        public void PreDisposeComponents()
        {
            foreach (var entry in _componentEntrys.Values)
            {
                entry.PreDispose();
            }
        }

        public void DisposeComponents()
        {
            foreach (var entry in _componentEntrys.Values)
            {
                entry.Dispose();
            }
        }

        public void PostDisposeComponents()
        {
            foreach (var entry in _componentEntrys.Values)
            {
                entry.PostDispose();
            }
            _componentEntrys.Clear();
        }

       
    }
}
