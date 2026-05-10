using Elder.Framework.Boot.App;
using Elder.Framework.Boot.Interfaces;
using Elder.Framework.Boot.Messages;
using Elder.Framework.Common.Messages;
using Elder.Framework.Data.Messages;
using Elder.Framework.Flux.Definitions;
using Elder.Framework.Flux.Infra;
using Elder.Framework.Flux.Interfaces;
using NUnit.Framework;

namespace Elder.Framework.Tests.Boot
{
    internal sealed class GameBootStrapperTests
    {
        private FluxRouter _router;
        private GameBootStrapper _bootstrapper;

        [SetUp]
        public void SetUp()
        {
            _router = new FluxRouter();
        }

        [TearDown]
        public void TearDown()
        {
            _bootstrapper?.Dispose();
            _router.Dispose();
        }

        // ─── Start — init scene not active ────────────────────────────────────

        [Test]
        public void Start_WhenInitSceneNotActive_DoesNotPublishInitializeSystem()
        {
            var published = false;
            _router.Subscribe<FxInitializeSystem>(
                (MessageHandler<FxInitializeSystem>)((in FxInitializeSystem _) => published = true));

            _bootstrapper = new GameBootStrapper(_router, new StubStartupEnvironment(false));
            _bootstrapper.Start();

            Assert.IsFalse(published);
        }

        // ─── Start — init scene active ─────────────────────────────────────────

        [Test]
        public void Start_WhenInitSceneActive_PublishesFxInitializeSystem()
        {
            var published = false;
            _router.Subscribe<FxInitializeSystem>(
                (MessageHandler<FxInitializeSystem>)((in FxInitializeSystem _) => published = true));

            _bootstrapper = new GameBootStrapper(_router, new StubStartupEnvironment(true));
            _bootstrapper.Start();

            Assert.IsTrue(published);
        }

        [Test]
        public void Start_WhenInitSceneActive_PublishesFxPostInitializeSystem()
        {
            var published = false;
            _router.Subscribe<FxPostInitializeSystem>(
                (MessageHandler<FxPostInitializeSystem>)((in FxPostInitializeSystem _) => published = true));

            _bootstrapper = new GameBootStrapper(_router, new StubStartupEnvironment(true));
            _bootstrapper.Start();

            Assert.IsTrue(published);
        }

        // ─── FxBaseDataInitialized → FxSystemInitializeEnd ───────────────────

        [Test]
        public void OnBaseDataInitialized_PublishesFxSystemInitializeEnd()
        {
            var published = false;
            _router.Subscribe<FxSystemInitializeEnd>(
                (MessageHandler<FxSystemInitializeEnd>)((in FxSystemInitializeEnd _) => published = true));

            _bootstrapper = new GameBootStrapper(_router, new StubStartupEnvironment(true));
            _bootstrapper.Start();

            _router.Publish(new FxBaseDataInitialized());

            Assert.IsTrue(published);
        }

        [Test]
        public void OnBaseDataInitialized_SecondPublication_NoSecondSystemInitializeEnd()
        {
            var count = 0;
            _router.Subscribe<FxSystemInitializeEnd>(
                (MessageHandler<FxSystemInitializeEnd>)((in FxSystemInitializeEnd _) => count++));

            _bootstrapper = new GameBootStrapper(_router, new StubStartupEnvironment(true));
            _bootstrapper.Start();

            _router.Publish(new FxBaseDataInitialized());
            _router.Publish(new FxBaseDataInitialized());

            // Subscription disposed after first — second publish must not trigger again
            Assert.AreEqual(1, count);
        }

        // ─── Dispose ───────────────────────────────────────────────────────────

        [Test]
        public void Dispose_AfterStart_BaseDataInitializedNoLongerPublishesSystemInitializeEnd()
        {
            var count = 0;
            _router.Subscribe<FxSystemInitializeEnd>(
                (MessageHandler<FxSystemInitializeEnd>)((in FxSystemInitializeEnd _) => count++));

            _bootstrapper = new GameBootStrapper(_router, new StubStartupEnvironment(true));
            _bootstrapper.Start();
            _bootstrapper.Dispose();

            _router.Publish(new FxBaseDataInitialized());

            Assert.AreEqual(0, count);
        }

        [Test]
        public void Dispose_CalledTwice_DoesNotThrow()
        {
            _bootstrapper = new GameBootStrapper(_router, new StubStartupEnvironment(true));
            _bootstrapper.Dispose();
            Assert.DoesNotThrow(() => _bootstrapper.Dispose());
        }

        // ─── Stubs ─────────────────────────────────────────────────────────────

        private sealed class StubStartupEnvironment : IStartupEnvironment
        {
            private readonly bool _active;
            public StubStartupEnvironment(bool active) { _active = active; }
            public bool IsInitSceneActive() => _active;
        }
    }
}
