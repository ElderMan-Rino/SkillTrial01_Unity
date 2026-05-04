using Elder.Framework.Flux.Definitions;
using Elder.Framework.Flux.Helpers;
using Elder.Framework.Flux.Infra;
using Elder.Framework.Flux.Interfaces;
using NUnit.Framework;

namespace Elder.Framework.Tests.Flux
{
    internal sealed class FluxRouterTests
    {
        private FluxRouter _router;

        [SetUp]
        public void SetUp()
        {
            _router = new FluxRouter();
        }

        [TearDown]
        public void TearDown()
        {
            _router.Dispose();
        }

        // ─── Publish ───────────────────────────────────────────────────────────

        [Test]
        public void Publish_WhenNoSubscriber_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _router.Publish(new TestMessage(42)));
        }

        [Test]
        public void Publish_WhenSubscribed_InvokesHandler()
        {
            var received = 0;
            _router.Subscribe<TestMessage>((MessageHandler<TestMessage>)((in TestMessage msg) => received = msg.Value));

            _router.Publish(new TestMessage(7));

            Assert.AreEqual(7, received);
        }

        [Test]
        public void Publish_MultipleSubscribers_AllInvoked()
        {
            var countA = 0;
            var countB = 0;
            _router.Subscribe<TestMessage>((MessageHandler<TestMessage>)((in TestMessage _) => countA++));
            _router.Subscribe<TestMessage>((MessageHandler<TestMessage>)((in TestMessage _) => countB++));

            _router.Publish(new TestMessage(0));

            Assert.AreEqual(1, countA);
            Assert.AreEqual(1, countB);
        }

        [Test]
        public void Publish_DifferentMessageTypes_RoutedIndependently()
        {
            var receivedTest = false;
            var receivedOther = false;
            _router.Subscribe<TestMessage>((MessageHandler<TestMessage>)((in TestMessage _) => receivedTest = true));
            _router.Subscribe<OtherMessage>((MessageHandler<OtherMessage>)((in OtherMessage _) => receivedOther = true));

            _router.Publish(new TestMessage(0));

            Assert.IsTrue(receivedTest);
            Assert.IsFalse(receivedOther);
        }

        // ─── Subscribe / Unsubscribe ────────────────────────────────────────────

        [Test]
        public void Subscribe_ReturnsNonEmptyToken()
        {
            var token = _router.Subscribe<TestMessage>((MessageHandler<TestMessage>)((in TestMessage _) => { }));
            Assert.AreNotEqual(SubscriptionToken.Empty, token);
        }

        [Test]
        public void Unsubscribe_AfterDispose_HandlerNotInvoked()
        {
            var received = false;
            var token = _router.Subscribe<TestMessage>((MessageHandler<TestMessage>)((in TestMessage _) => received = true));
            token.Dispose();

            _router.Publish(new TestMessage(0));

            Assert.IsFalse(received);
        }

        [Test]
        public void Unsubscribe_OneOfTwo_OtherStillInvoked()
        {
            var countA = 0;
            var countB = 0;
            var tokenA = _router.Subscribe<TestMessage>((MessageHandler<TestMessage>)((in TestMessage _) => countA++));
            _router.Subscribe<TestMessage>((MessageHandler<TestMessage>)((in TestMessage _) => countB++));
            tokenA.Dispose();

            _router.Publish(new TestMessage(0));

            Assert.AreEqual(0, countA);
            Assert.AreEqual(1, countB);
        }

        [Test]
        public void Unsubscribe_CalledTwice_DoesNotThrow()
        {
            var token = _router.Subscribe<TestMessage>((MessageHandler<TestMessage>)((in TestMessage _) => { }));
            token.Dispose();

            Assert.DoesNotThrow(() => token.Dispose());
        }

        // ─── Dispose ───────────────────────────────────────────────────────────

        [Test]
        public void Dispose_CalledTwice_DoesNotThrow()
        {
            var router = new FluxRouter();
            router.Dispose();

            Assert.DoesNotThrow(() => router.Dispose());
        }

        [Test]
        public void Dispose_PublishAfterDispose_DoesNotThrow()
        {
            var router = new FluxRouter();
            router.Subscribe<TestMessage>((MessageHandler<TestMessage>)((in TestMessage _) => { }));
            router.Dispose();

            Assert.DoesNotThrow(() => router.Publish(new TestMessage(0)));
        }

        // ─── Test fixtures ─────────────────────────────────────────────────────

        private readonly struct TestMessage : IFluxMessage
        {
            public readonly int Value;
            public TestMessage(int value) { Value = value; }
        }

        private readonly struct OtherMessage : IFluxMessage { }
    }
}
