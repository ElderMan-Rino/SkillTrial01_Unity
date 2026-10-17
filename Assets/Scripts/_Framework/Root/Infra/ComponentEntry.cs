using Elder.Framework.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Root.Infra
{
    public sealed class ComponentEntry : ISharedRegistration, IPreDisposable, IDisposable, IPostDisposable
    {
        private readonly HashSet<Type> _aliasTypes = new ();
        private readonly IComponentRegistry _registry;
        public IFrameworkComponent Instance { get; private set; }
        public IReadOnlyCollection<Type> AliasTypes => _aliasTypes;
        

        public ComponentEntry(IFrameworkComponent instance, IComponentRegistry registry)
        {
            Instance = instance;
            _registry = registry;
        }

        public void AddAlias(Type aliasType)
        {
            _aliasTypes.Add(aliasType);
        }

        public void PreDispose()
        {
            if (Instance is IPreDisposable preDisposer) preDisposer?.PreDispose();
        }

        public void PostDispose()
        {
            if (Instance is IPostDisposable postDisposer) postDisposer?.PostDispose();
            _aliasTypes.Clear();
            Instance = null;
        }

        public void Dispose()
        {
            if (Instance is IDisposable disposer) disposer?.Dispose();
        }

        public ISharedRegistration As<T>() where T : IFrameworkComponent
        {
            _registry.MapAlias(typeof(T), Instance.ClassType);
            return this;
        }
    }
}