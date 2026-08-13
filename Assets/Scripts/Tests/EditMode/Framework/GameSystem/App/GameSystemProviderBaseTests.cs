using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Core.Interfaces.Base;
using Elder.Framework.GameSystem.App;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Tests.EditMode.Framework.GameSystem.App
{
    public class GameSystemProviderBaseTests
    {
        [Test]
        public void TryGetSystem_RootProviderLocalHit_ReturnsTrue()
        {
            var dummy = new DummySystem();
            var provider = GameSystemProviderBase.Create(
                new Dictionary<Type, ISystemComponent> { { typeof(DummySystem), dummy } },
                new List<ISystemComponent> { dummy });

            Assert.IsTrue(provider.TryGetSystem<DummySystem>(out var system));
            Assert.AreSame(dummy, system);
        }

        [Test]
        public void TryGetSystem_RootProviderMiss_ReturnsFalse()
        {
            var provider = GameSystemProviderBase.Create(
                new Dictionary<Type, ISystemComponent>(),
                new List<ISystemComponent>());

            Assert.IsFalse(provider.TryGetSystem<DummySystem>(out _));
        }

        [Test]
        public void TryGetSystem_ScopedProviderLocalHit_DoesNotConsultParent()
        {
            var dummy = new DummySystem();
            var parent = new FakeParentProvider();
            var provider = GameSystemProviderBase.CreateScoped(parent,
                new Dictionary<Type, ISystemComponent> { { typeof(DummySystem), dummy } },
                new List<ISystemComponent> { dummy });

            Assert.IsTrue(provider.TryGetSystem<DummySystem>(out var system));
            Assert.AreSame(dummy, system);
            Assert.IsFalse(parent.TryGetSystemCalled);
        }

        [Test]
        public void TryGetSystem_ScopedProviderLocalMiss_FallsBackToParent()
        {
            var parentDummy = new DummySystem();
            var parent = new FakeParentProvider(parentDummy);
            var provider = GameSystemProviderBase.CreateScoped(parent,
                new Dictionary<Type, ISystemComponent>(),
                new List<ISystemComponent>());

            Assert.IsTrue(provider.TryGetSystem<DummySystem>(out var system));
            Assert.AreSame(parentDummy, system);
        }

        [Test]
        public void TryGetSystem_ScopedProviderMissOnBoth_ReturnsFalse()
        {
            var parent = new FakeParentProvider();
            var provider = GameSystemProviderBase.CreateScoped(parent,
                new Dictionary<Type, ISystemComponent>(),
                new List<ISystemComponent>());

            Assert.IsFalse(provider.TryGetSystem<DummySystem>(out _));
        }

        [Test]
        public void TryGetSystems_ScopedProviderUnionOfLocalAndParent_ReturnsBothAndTrue()
        {
            var localDummy = new DummySystem();
            var parentDummy = new DummySystem();
            var parent = new FakeParentProvider(parentDummy);
            var provider = GameSystemProviderBase.CreateScoped(parent,
                new Dictionary<Type, ISystemComponent> { { typeof(DummySystem), localDummy } },
                new List<ISystemComponent> { localDummy });

            var results = new List<DummySystem>();
            var found = provider.TryGetSystems(ref results);

            Assert.IsTrue(found);
            Assert.AreEqual(2, results.Count);
            CollectionAssert.Contains(results, localDummy);
            CollectionAssert.Contains(results, parentDummy);
        }

        [Test]
        public void TryDisposeSystem_ExistingSystem_RemovesAndReturnsTrue()
        {
            var dummy = new DummySystem();
            var provider = GameSystemProviderBase.Create(
                new Dictionary<Type, ISystemComponent> { { typeof(DummySystem), dummy } },
                new List<ISystemComponent> { dummy });

            Assert.IsTrue(provider.TryDisposeSystem(typeof(DummySystem)));
            Assert.IsFalse(provider.TryGetSystem<DummySystem>(out _));
        }

        [Test]
        public void TryDisposeSystem_DisposableSystem_CallsPreDisposeThenDispose()
        {
            var disposable = new FakeDisposableSystem();
            var provider = GameSystemProviderBase.Create(
                new Dictionary<Type, ISystemComponent> { { typeof(FakeDisposableSystem), disposable } },
                new List<ISystemComponent> { disposable });

            provider.TryDisposeSystem(typeof(FakeDisposableSystem));

            Assert.IsTrue(disposable.PreDisposeCalled);
            Assert.IsTrue(disposable.DisposeCalled);
        }

        [Test]
        public void TryDisposeSystem_NotRegistered_ReturnsFalse()
        {
            var provider = GameSystemProviderBase.Create(
                new Dictionary<Type, ISystemComponent>(),
                new List<ISystemComponent>());

            Assert.IsFalse(provider.TryDisposeSystem(typeof(DummySystem)));
        }

        [Test]
        public void InjectAll_GameSystemEntries_CallsInjectDependencyWithSelf()
        {
            var gameSystem = new FakeGameSystem();
            var provider = GameSystemProviderBase.Create(
                new Dictionary<Type, ISystemComponent> { { typeof(FakeGameSystem), gameSystem } },
                new List<ISystemComponent> { gameSystem });

            provider.InjectAll();

            Assert.AreSame(provider, gameSystem.InjectedProvider);
        }

        [Test]
        public void InjectAll_NonGameSystemEntry_IsSkipped()
        {
            var dummy = new DummySystem();
            var provider = GameSystemProviderBase.Create(
                new Dictionary<Type, ISystemComponent> { { typeof(DummySystem), dummy } },
                new List<ISystemComponent> { dummy });

            Assert.DoesNotThrow(() => provider.InjectAll());
        }

        [Test]
        public void InjectAll_InjectDependencyThrows_WrapsInInvalidOperationException()
        {
            var gameSystem = new FakeGameSystem { ThrowOnInject = true };
            var provider = GameSystemProviderBase.Create(
                new Dictionary<Type, ISystemComponent> { { typeof(FakeGameSystem), gameSystem } },
                new List<ISystemComponent> { gameSystem });

            var ex = Assert.Throws<InvalidOperationException>(() => provider.InjectAll());
            StringAssert.Contains($"[DI] InjectDependency failed for {nameof(FakeGameSystem)}", ex.Message);
        }

        [Test]
        public void InitializeAllAsync_GameSystemEntries_AwaitsAllInitializeAsync()
        {
            var first = new FakeGameSystem();
            var second = new FakeGameSystem();
            var provider = GameSystemProviderBase.Create(
                new Dictionary<Type, ISystemComponent> { { typeof(FakeGameSystem), first } },
                new List<ISystemComponent> { first, second });

            provider.InitializeAllAsync().GetAwaiter().GetResult();

            Assert.IsTrue(first.InitializeCalled);
            Assert.IsTrue(second.InitializeCalled);
        }

        [Test]
        public void PostInitializeAllAsync_GameSystemEntries_AwaitsAllPostInitializeAsync()
        {
            var first = new FakeGameSystem();
            var second = new FakeGameSystem();
            var provider = GameSystemProviderBase.Create(
                new Dictionary<Type, ISystemComponent> { { typeof(FakeGameSystem), first } },
                new List<ISystemComponent> { first, second });

            provider.PostInitializeAllAsync().GetAwaiter().GetResult();

            Assert.IsTrue(first.PostInitializeCalled);
            Assert.IsTrue(second.PostInitializeCalled);
        }

        private sealed class DummySystem : ISystemComponent { }

        private sealed class FakeDisposableSystem : ISystemComponent, IDisposableBase
        {
            public bool PreDisposeCalled { get; private set; }
            public bool DisposeCalled { get; private set; }

            public void PreDispose()
            {
                PreDisposeCalled = true;
            }

            public void Dispose()
            {
                DisposeCalled = true;
            }
        }

        private sealed class FakeGameSystem : IGameSystem
        {
            public bool ThrowOnInject { get; set; }
            public bool InitializeCalled { get; private set; }
            public bool PostInitializeCalled { get; private set; }
            public IGameSystemProvider InjectedProvider { get; private set; }

            public void InjectDependency(IGameSystemProvider provider)
            {
                if (ThrowOnInject) throw new Exception("boom");
                InjectedProvider = provider;
            }

            public UniTask InitializeAsync()
            {
                InitializeCalled = true;
                return UniTask.CompletedTask;
            }

            public UniTask PostInitializeAsync()
            {
                PostInitializeCalled = true;
                return UniTask.CompletedTask;
            }
        }

        private sealed class FakeParentProvider : IGameSystemProvider
        {
            private readonly DummySystem _dummy;

            public bool TryGetSystemCalled { get; private set; }

            public FakeParentProvider(DummySystem dummy = null)
            {
                _dummy = dummy;
            }

            public bool TryGetSystem<T>(out T system) where T : class, ISystemComponent
            {
                TryGetSystemCalled = true;
                if (_dummy is T typed)
                {
                    system = typed;
                    return true;
                }
                system = null;
                return false;
            }

            public bool TryGetSystems<T>(ref List<T> results) where T : class, ISystemComponent
            {
                if (_dummy is T typed)
                {
                    results.Add(typed);
                    return true;
                }
                return false;
            }

            public bool TryDisposeSystem(Type systemType) => false;

            public void InjectAll() { }

            public UniTask InitializeAllAsync() => UniTask.CompletedTask;

            public UniTask PostInitializeAllAsync() => UniTask.CompletedTask;
        }
    }
}
