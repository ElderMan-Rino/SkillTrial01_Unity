using Elder.Framework.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.GameSystem.App
{
    internal sealed class GameSystemProvider : IGameSystemProvider
    {
        private readonly Dictionary<Type, ISystemComponent> _map;
        // [HEAP] 빌드 시점 1회 할당 — IGameSystem 구현체 순서 보장용
        private readonly List<IGameSystem> _systems;

        internal GameSystemProvider(Dictionary<Type, ISystemComponent> map, List<IGameSystem> systems)
        {
            _map = map;
            _systems = systems;
        }

        public bool TryGetSystem<T>(out T system) where T : class, ISystemComponent
        {
            if (_map.TryGetValue(typeof(T), out var obj) && obj is T typed)
            {
                system = typed;
                return true;
            }

            system = null;
            return false;
        }

        public void InjectAll()
        {
            for (int i = 0; i < _systems.Count; i++)
                _systems[i].TryInjectDependency(this);
        }

        public void InitializeAll()
        {
            for (int i = 0; i < _systems.Count; i++)
                _systems[i].TryInitialize();
        }

        public void PostInitializeAll()
        {
            for (int i = 0; i < _systems.Count; i++)
                _systems[i].TryPostInitialize();
        }

        public void DisposeAll()
        {
            for (int i = _systems.Count - 1; i >= 0; i--)
                _systems[i].TryDispose();
        }
    }
}
