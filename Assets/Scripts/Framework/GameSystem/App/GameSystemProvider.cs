using Elder.Framework.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.GameSystem.App
{
    internal sealed class GameSystemProvider : GameSystemProviderBase
    {
        internal GameSystemProvider(Dictionary<Type, ISystemComponent> systemComponents, List<ISystemComponent> orderedSystems) : base(systemComponents, orderedSystems) {}

        public override bool TryGetSystem<T>(out T system)
        {
            if (_systemComponents.TryGetValue(typeof(T), out var obj) && obj is T typed)
            {
                system = typed;
                return true;
            }

            system = null;
            return false;
        }

        public override bool TryGetSystems<T>(ref List<T> results)
        {
            bool found = false;
            for (int i = 0; i < _orderedSystems.Count; i++)
            {
                if (_orderedSystems[i] is T typed)
                {
                    results.Add(typed);
                    found = true;
                }
            }
            return found;
        }
    }
}
