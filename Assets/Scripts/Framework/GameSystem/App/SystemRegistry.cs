using Elder.Framework.Core.Interfaces;
using Elder.Framework.GameSystem.Domain;
using System;
using System.Collections.Generic;

namespace Elder.Framework.GameSystem.App
{
    internal sealed class SystemRegistry : ISystemRegistry
    {
        // [HEAP] 빌드 시점 1회 할당
        private readonly List<RegistryEntryBase> _entries = new();
        // [HEAP] 빌드 시점 1회 할당 — 등록된 인스턴스 즉시 조회용
        private readonly Dictionary<Type, ISystemComponent> _registeredInstances = new();

        public void Register<T, TImpl>() where T : ISystemComponent where TImpl : class, T, new()
        {
            // [HEAP] new TImpl() — 등록 시점 1회
            var instance = new TImpl();
            _registeredInstances[typeof(T)] = instance;
            _entries.Add(new RegistryEntry<T>(instance));
        }

        public ISharedRegistration RegisterShared<T>() where T : ISystemComponent, new()
        {
            // [HEAP] SharedEntry + new TImpl() — 빌드 시점 1회
            var instance = new T();
            var entry = new SharedEntry(instance);
            _entries.Add(entry);
            return entry;
        }

        public void RegisterInstance<T>(T instance) where T : ISystemComponent
        {
            _registeredInstances[typeof(T)] = instance;
            _entries.Add(new RegistryEntry<T>(instance));
        }

        public bool TryGetRegistered<T>(out T instance)
        {
            if (_registeredInstances.TryGetValue(typeof(T), out var obj) && obj is T typed)
            {
                instance = typed;
                return true;
            }
            instance = default;
            return false;
        }

        public List<T> GetAllRegistered<T>()
        {
            // [HEAP] 반환용 List — 빌드/초기화 시점 1회 호출 허용
            var result = new List<T>();
            var seen = new HashSet<ISystemComponent>();
            for (int i = 0; i < _entries.Count; i++)
            {
                var inst = _entries[i].Instance;
                if (inst is T typed && seen.Add(inst))
                    result.Add(typed);
            }
            return result;
        }

        public IGameSystemProvider Build()
        {
            // [HEAP] Dictionary + List 초기화 — 빌드 시점 1회
            var map = new Dictionary<Type, ISystemComponent>(_entries.Count * 2);
            var systems = new List<IGameSystem>();

            for (int i = 0; i < _entries.Count; i++)
                _entries[i].Populate(map);

            // 등록 순서대로 IGameSystem 수집 (중복 제거)
            var seen = new HashSet<IGameSystem>();
            for (int i = 0; i < _entries.Count; i++)
            {
                var instance = _entries[i].Instance;
                if (instance is IGameSystem gs && seen.Add(gs)) systems.Add(gs);
            }

            return new GameSystemProvider(map, systems);
        }

       

       
    }
}
