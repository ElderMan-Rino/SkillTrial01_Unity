using System;
using System.Collections.Generic;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.GameSystem.App
{
    public sealed class GameSystemProvider : IGameSystemProvider
    {
        private readonly Dictionary<Type, object> _map;

        internal GameSystemProvider(Dictionary<Type, object> map)
        {
            _map = map;
        }

        public bool TryGetSystem<T>(out T system) where T : IGameSystem
        {
            if (_map.TryGetValue(typeof(T), out var obj) && obj is T typed)
            {
                system = typed;
                return true;
            }

            system = default;
            return false;
        }

        public bool TryResolve<T>(out T instance)
        {
            if (_map.TryGetValue(typeof(T), out var obj) && obj is T typed)
            {
                instance = typed;
                return true;
            }

            instance = default;
            return false;
        }
    }
}
