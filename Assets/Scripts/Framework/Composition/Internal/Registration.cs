using Elder.Framework.Common.Base;
using Elder.Framework.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Composition.Internal
{
    internal sealed class Registration : DisposableBase
    {
        private readonly HashSet<Type> _serviceTypes = new();

        public readonly Type ImplementationType;
        public readonly ISystemComponent Instance;
        public IReadOnlyCollection<Type> ServiceTypes => _serviceTypes;

        public Registration(Type implementationType, ISystemComponent instance)
        {
            ImplementationType = implementationType;
            Instance = instance;
        }

        public void AddServiceType(Type serviceType)
        {
            if (_serviceTypes.Contains(serviceType)) return;
            _serviceTypes.Add(serviceType);
        }

        public override void PreDispose() { }

        protected override void DisposeManagedResources()
        {
            _serviceTypes.Clear();
        }
    }
}