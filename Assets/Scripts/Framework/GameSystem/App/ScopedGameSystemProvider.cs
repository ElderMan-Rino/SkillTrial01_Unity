using Elder.Framework.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.GameSystem.App
{
    internal sealed class ScopedGameSystemProvider : GameSystemProviderBase
    {
        private readonly IGameSystemProvider _parent;

        public ScopedGameSystemProvider(IGameSystemProvider parent, Dictionary<Type, ISystemComponent> systemComponents, List<ISystemComponent> orderedSystems) : base(systemComponents, orderedSystems)
        {
            _parent = parent;
        }

        public override bool TryGetSystem<T>(out T system)
        {
            if (_systemComponents.TryGetValue(typeof(T), out var obj) && obj is T typed)
            {
                system = typed;
                return true;
            }
            return _parent.TryGetSystem<T>(out system);
        }

        public override bool TryGetSystems<T>(ref List<T> results)
        {
            bool found = false;
            foreach (var component in _systemComponents.Values)
            {
                if (component is not T typed) continue;
                results.Add(typed);
                found = true;
            }
            bool parentFound = _parent.TryGetSystems<T>(ref results);
            return found || parentFound;
        }
    }
}
