using Elder.Framework.Composition.Internal;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.GameSystem.App;
using System;
using System.Collections.Generic;


namespace Elder.Framework.Composition
{
    internal sealed class ScopedSystemRegistry : IGameSystemRegistry
    {
        private readonly IGameSystemProvider _parent;
        private readonly Dictionary<Type, Registration> _registrations = new();

        internal ScopedSystemRegistry(IGameSystemProvider parent)
        {
            _parent = parent;
        }

        public IRegistrationBuilder Register<T>() where T : class, ISystemComponent, new()
        {
            var registration = new Registration(typeof(T), new T());
            registration.AddServiceType(typeof(T));
            _registrations.Add(typeof(T), registration);
            return new RegistrationBuilder(registration);
        }

        public IRegistrationBuilder RegisterInstance<T>(T instance) where T : class, ISystemComponent
        {
            var registration = new Registration(typeof(T), instance);
            _registrations.Add(typeof(T), registration);
            return new RegistrationBuilder(registration);
        }

        public IGameSystemProvider Build()
        {
            var systemComponents = new Dictionary<Type, ISystemComponent>(_registrations.Count);
            var orderedSystems = new List<ISystemComponent>(_registrations.Count);
            foreach (var registration in _registrations)
            {
                orderedSystems.Add(registration.Value.Instance);
                foreach (var serviceType in registration.Value.ServiceTypes)
                    systemComponents[serviceType] = registration.Value.Instance;
            }
            return new ScopedGameSystemProvider(_parent, systemComponents, orderedSystems);
        }
    }
}