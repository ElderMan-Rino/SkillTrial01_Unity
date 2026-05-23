using System;
using System.Collections.Generic;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.GameSystem.App
{
    public sealed class SystemRegistry : ISystemRegistry
    {
        // [HEAP] 빌드 시점 1회 할당
        private readonly List<RegistryEntry> _entries = new();

        public void Register<TInterface, TImpl>() where TImpl : class, TInterface, new()
        {
            _entries.Add(new RegistryEntry(typeof(TInterface), _ => new TImpl()));
        }

        public ISharedRegistration RegisterShared<TImpl>() where TImpl : class, new()
        {
            // [HEAP] SharedEntry — 빌드 시점 1회
            var entry = new SharedEntry(_ => new TImpl());
            _entries.Add(entry);
            return entry;
        }

        public void RegisterInstance<TInterface>(TInterface instance)
        {
            _entries.Add(new RegistryEntry(typeof(TInterface), _ => instance));
        }

        public ISharedRegistration RegisterSharedFactory<T>(Func<IGameSystemProvider, T> factory) where T : class
        {
            // [HEAP] SharedEntry — 빌드 시점 1회
            var entry = new SharedEntry(p => factory(p));
            _entries.Add(entry);
            return entry;
        }

        public void RegisterFactory<TInterface>(Func<IGameSystemProvider, TInterface> factory)
        {
            // [HEAP] 람다 캡처 — 빌드 시점 1회
            _entries.Add(new RegistryEntry(typeof(TInterface), p => factory(p)));
        }

        public IGameSystemProvider Build()
        {
            // [HEAP] Dictionary 초기화 — 빌드 시점 1회
            var map = new Dictionary<Type, object>(_entries.Count * 2);
            var provider = new GameSystemProvider(map);

            for (int i = 0; i < _entries.Count; i++)
                _entries[i].Populate(provider, map);

            return provider;
        }

        private class RegistryEntry
        {
            private readonly Type _interfaceType;
            private readonly Func<GameSystemProvider, object> _factory;

            public RegistryEntry(Type interfaceType, Func<GameSystemProvider, object> factory)
            {
                _interfaceType = interfaceType;
                _factory = factory;
            }

            public virtual void Populate(GameSystemProvider provider, Dictionary<Type, object> map)
            {
                if (!map.ContainsKey(_interfaceType))
                    map[_interfaceType] = _factory(provider);
            }
        }

        private sealed class SharedEntry : RegistryEntry, ISharedRegistration
        {
            // [HEAP] As<>() 호출마다 타입 1개 추가 — 빌드 시점
            private readonly List<Type> _aliases = new();
            private readonly Func<GameSystemProvider, object> _sharedFactory;

            public SharedEntry(Func<GameSystemProvider, object> factory) : base(null, null)
            {
                _sharedFactory = factory;
            }

            public ISharedRegistration As<TInterface>()
            {
                _aliases.Add(typeof(TInterface));
                return this;
            }

            public override void Populate(GameSystemProvider provider, Dictionary<Type, object> map)
            {
                if (_aliases.Count == 0) return;

                var instance = _sharedFactory(provider);
                for (int i = 0; i < _aliases.Count; i++)
                {
                    if (!map.ContainsKey(_aliases[i]))
                        map[_aliases[i]] = instance;
                }
            }
        }
    }
}
