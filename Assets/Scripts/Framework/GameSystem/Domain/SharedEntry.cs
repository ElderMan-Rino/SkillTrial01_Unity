using Elder.Framework.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.GameSystem.Domain
{
    internal sealed class SharedEntry : RegistryEntryBase, ISharedRegistration
    {
        private readonly List<Type> _aliases = new();

        public SharedEntry(ISystemComponent instance) : base(instance)
        {
        }

        // [HEAP] As<>() 호출마다 타입 1회 추가 — 등록 시점
        public ISharedRegistration As<TInterface>()
        {
            _aliases.Add(typeof(TInterface));
            return this;
        }

        public override void Populate(Dictionary<Type, ISystemComponent> map)
        {
            for (int i = 0; i < _aliases.Count; i++)
            {
                if (!map.ContainsKey(_aliases[i])) map[_aliases[i]] = Instance;
            }
        }
    }
}