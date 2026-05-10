using Elder.Framework.Boot.App;
using Elder.Framework.Boot.Definitions.Constants;
using Elder.Framework.Boot.Messages;
using Elder.Framework.Flux.Definitions;
using Elder.Framework.Flux.Infra;
using Elder.Framework.Flux.Interfaces;
using Elder.Framework.Scene.Messages;
using NUnit.Framework;

namespace Elder.Framework.Tests.Boot
{
    internal sealed class GameStartServiceTests
    {
        private FluxRouter _router;
        private GameStartService _service;

        [SetUp]
        public void SetUp()
        {
            _router = new FluxRouter();
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
            _router.Dispose();
        }

        // ─── FxSystemInitializeEnd → FxSceneTransition ────────────────────────

        [Test]
        public void OnSystemInitializeEnd_PublishesFxSceneTransitionToBootstrapScene()
        {
            string targetKey = null;
            _router.Subscribe<FxSceneTransition>(
                (MessageHandler<FxSceneTransition>)((in FxSceneTransition msg) => targetKey = msg.TargetSceneKey));

            _service = new GameStartService(_router);
            _service.Initialize();

            _router.Publish(new FxSystemInitializeEnd());

            Assert.AreEqual(BootConstants.BootStrapSceneKey, targetKey);
        }

        [Test]
        public void OnSystemInitializeEnd_SecondPublication_SceneTransitionPublishedOnce()
        {
            var count = 0;
            _router.Subscribe<FxSceneTransition>(
                (MessageHandler<FxSceneTransition>)((in FxSceneTransition _) => count++));

            _service = new GameStartService(_router);
            _service.Initialize();

            _router.Publish(new FxSystemInitializeEnd());
            _router.Publish(new FxSystemInitializeEnd());

            // Subscription disposed after first — second publish must not trigger another transition
            Assert.AreEqual(1, count);
        }

        // ─── Dispose ───────────────────────────────────────────────────────────

        [Test]
        public void Dispose_AfterInitialize_SystemInitializeEndNoLongerTriggerTransition()
        {
            var count = 0;
            _router.Subscribe<FxSceneTransition>(
                (MessageHandler<FxSceneTransition>)((in FxSceneTransition _) => count++));

            _service = new GameStartService(_router);
            _service.Initialize();
            _service.Dispose();

            _router.Publish(new FxSystemInitializeEnd());

            Assert.AreEqual(0, count);
        }

        [Test]
        public void Dispose_CalledTwice_DoesNotThrow()
        {
            _service = new GameStartService(_router);
            _service.Dispose();
            Assert.DoesNotThrow(() => _service.Dispose());
        }
    }
}
