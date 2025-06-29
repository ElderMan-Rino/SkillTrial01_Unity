using Elder.Framework.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.GameSystem.Domain
{
    internal sealed class RegistryEntry<T> : RegistryEntryBase where T : ISystemComponent
    {
        public RegistryEntry(ISystemComponent instance) : base(instance)
        {
        }

        public override void Populate(Dictionary<Type, ISystemComponent> map)
        {
            var type = typeof(T);
            if (!map.ContainsKey(type)) map[type] = Instance;
        }
    }
}