using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Helpers;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Root.Infra;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Elder.Framework.Core.Infra
{
    public sealed class ComponentRegistry : IComponentProvider, IComponentRegistry
    {
        private const int MaxChunkSize = 4;

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

        public async UniTask InitializeComponentsAsync(CancellationToken cancellationToken)
        {
            var instances = new List<IFrameworkComponent>(_componentEntrys.Count); // [HEAP]
            foreach (var entry in _componentEntrys.Values)
                instances.Add(entry.Instance);

            await UniTaskChunkHelper.RunInChunksAsync(instances, MaxChunkSize, InitializeOneAsync, cancellationToken);
        }

        public async UniTask PostInitializeComponentsAsync(CancellationToken cancellationToken)
        {
            var instances = new List<IFrameworkComponent>(_componentEntrys.Count); // [HEAP]
            foreach (var entry in _componentEntrys.Values)
                instances.Add(entry.Instance);

            await UniTaskChunkHelper.RunInChunksAsync(instances, MaxChunkSize, PostInitializeOneAsync, cancellationToken);
        }

        private static UniTask InitializeOneAsync(IFrameworkComponent instance, CancellationToken cancellationToken)
        {
            if (instance is IInitializable initializable)
                return initializable.InitializeAsync(cancellationToken);

            return UniTask.CompletedTask;
        }

        private static UniTask PostInitializeOneAsync(IFrameworkComponent instance, CancellationToken cancellationToken)
        {
            if (instance is IPostInitializable postInitializable)
                return postInitializable.PostInitializeAsync(cancellationToken);

            return UniTask.CompletedTask;
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
