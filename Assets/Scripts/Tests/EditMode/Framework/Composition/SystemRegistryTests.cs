using Elder.Framework.Composition;
using Elder.Framework.Core.Interfaces;
using NUnit.Framework;
using System;

namespace Elder.Framework.Tests.EditMode.Framework.Composition
{
    public class SystemRegistryTests
    {
        [Test]
        public void Register_SingleSystem_BuildContainsSystem()
        {
            var registry = new SystemRegistry();
            registry.Register<DummySystem>();

            var provider = registry.Build();

            Assert.IsTrue(provider.TryGetSystem<DummySystem>(out var system));
            Assert.IsNotNull(system);
        }

        [Test]
        public void RegisterInstance_GivenInstance_BuildReturnsSameInstance()
        {
            var registry = new SystemRegistry();
            var instance = new DummySystem();
            registry.RegisterInstance(instance).As<DummySystem>();

            var provider = registry.Build();

            Assert.IsTrue(provider.TryGetSystem<DummySystem>(out var system));
            Assert.AreSame(instance, system);
        }

        [Test]
        public void Register_As_ExposesAdditionalServiceType()
        {
            var registry = new SystemRegistry();
            registry.Register<DummySystem>().As<IDummyMarker>();

            var provider = registry.Build();

            Assert.IsTrue(provider.TryGetSystem<IDummyMarker>(out var marker));
            Assert.IsNotNull(marker);
        }

        [Test]
        public void Register_DuplicateKey_OverwritesPreviousRegistration()
        {
            var registry = new SystemRegistry();
            var first = new DummySystem();
            var second = new DummySystem();

            registry.RegisterInstance(first).As<DummySystem>();
            registry.RegisterInstance(second).As<DummySystem>();

            var provider = registry.Build();

            Assert.IsTrue(provider.TryGetSystem<DummySystem>(out var system));
            Assert.AreSame(second, system);
        }

        [Test]
        public void Build_NoRegistrations_ReturnsEmptyProvider()
        {
            var registry = new SystemRegistry();

            var provider = registry.Build();

            Assert.IsFalse(provider.TryGetSystem<DummySystem>(out _));
        }

        private interface IDummyMarker : ISystemComponent { }

        private sealed class DummySystem : IDummyMarker { }
    }
}
