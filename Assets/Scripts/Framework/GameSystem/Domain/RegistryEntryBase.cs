using Elder.Framework.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.GameSystem.Domain
{
    internal abstract class RegistryEntryBase
    {
        public readonly ISystemComponent Instance;

        public RegistryEntryBase(ISystemComponent instance)
        {
            Instance = instance;
        }

        public abstract void Populate(Dictionary<Type, ISystemComponent> map);
    }
}