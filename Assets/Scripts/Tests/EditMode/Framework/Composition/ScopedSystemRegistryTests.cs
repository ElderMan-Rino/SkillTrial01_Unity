using Cysharp.Threading.Tasks;
using Elder.Framework.Composition;
using Elder.Framework.Core.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Tests.EditMode.Framework.Composition
{
    public class ScopedSystemRegistryTests
    {
        [Test]
        public void Register_SingleSystem_BuildContainsSystem()
        {
            var registry = new ScopedSystemRegistry(new FakeParentProvider());
            registry.Register<DummySystem>();

            var provider = registry.Build();

            Assert.IsTrue(provider.TryGetSystem<DummySystem>(out var system));
            Assert.IsNotNull(system);
        }

        [Test]
        public void RegisterInstance_GivenInstance_BuildReturnsSameInstance()
        {
            var registry = new ScopedSystemRegistry(new FakeParentProvider());
            var instance = new DummySystem();
            registry.RegisterInstance(instance).As<DummySystem>();

            var provider = registry.Build();

            Assert.IsTrue(provider.TryGetSystem<DummySystem>(out var system));
            Assert.AreSame(instance, system);
        }

        [Test]
        public void Register_As_ExposesAdditionalServiceType()
        {
            var registry = new ScopedSystemRegistry(new FakeParentProvider());
            registry.Register<DummySystem>().As<IDummyMarker>();

            var provider = registry.Build();

            Assert.IsTrue(provider.TryGetSystem<IDummyMarker>(out var marker));
            Assert.IsNotNull(marker);
        }

        [Test]
        public void Register_DuplicateKey_ThrowsArgumentException()
        {
            var registry = new ScopedSystemRegistry(new FakeParentProvider());
            registry.RegisterInstance(new DummySystem()).As<DummySystem>();

            Assert.Throws<ArgumentException>(() =>
                registry.RegisterInstance(new DummySystem()).As<DummySystem>());
        }

        [Test]
        public void Register_DuplicateKeyViaRegisterT_ThrowsArgumentException()
        {
            var registry = new ScopedSystemRegistry(new FakeParentProvider());
            registry.Register<DummySystem>();

            Assert.Throws<ArgumentException>(() => registry.Register<DummySystem>());
        }

        private interface IDummyMarker : ISystemComponent { }

        private sealed class DummySystem : IDummyMarker { }

        private sealed class FakeParentProvider : IGameSystemProvider
        {
            public bool TryGetSystem<T>(out T system) where T : class, ISystemComponent
            {
                system = null;
                return false;
            }

            public bool TryGetSystems<T>(ref List<T> system) where T : class, ISystemComponent => false;

            public bool TryDisposeSystem(Type systemType) => false;

            public void InjectAll() { }

            public UniTask InitializeAllAsync() => UniTask.CompletedTask;

            public UniTask PostInitializeAllAsync() => UniTask.CompletedTask;
        }
    }
}
