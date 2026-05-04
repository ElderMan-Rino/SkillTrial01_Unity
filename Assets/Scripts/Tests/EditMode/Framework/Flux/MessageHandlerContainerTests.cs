using Elder.Framework.Flux.Definitions;
using Elder.Framework.Flux.Helpers;
using Elder.Framework.Flux.Interfaces;
using NUnit.Framework;

namespace Elder.Framework.Tests.Flux
{
    internal sealed class MessageHandlerContainerTests
    {
        private MessageHandlerContainer<TestMsg> _container;

        [SetUp]
        public void SetUp()
        {
            _container = new MessageHandlerContainer<TestMsg>();
        }

        [TearDown]
        public void TearDown()
        {
            _container.Dispose();
        }

        // ─── Publish ───────────────────────────────────────────────────────────

        [Test]
        public void Publish_WhenEmpty_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _container.Publish(new TestMsg(1)));
        }

        [Test]
        public void Publish_SingleHandler_InvokesWithCorrectValue()
        {
            var received = 0;
            _container.SetLastTokenId(1);
            _container.Add((MessageHandler<TestMsg>)((in TestMsg msg) => received = msg.Value));

            _container.Publish(new TestMsg(42));

            Assert.AreEqual(42, received);
        }

        [Test]
        public void Publish_MultipleHandlers_AllInvoked()
        {
            var a = 0;
            var b = 0;
            _container.SetLastTokenId(1);
            _container.Add((MessageHandler<TestMsg>)((in TestMsg _) => a++));
            _container.SetLastTokenId(2);
            _container.Add((MessageHandler<TestMsg>)((in TestMsg _) => b++));

            _container.Publish(new TestMsg(0));

            Assert.AreEqual(1, a);
            Assert.AreEqual(1, b);
        }

        [Test]
        public void Publish_DuringPublish_UnsubscribeDoesNotThrow()
        {
            // Handler removes itself during publish — snapshot pattern must prevent crash
            _container.SetLastTokenId(1);
            _container.Add((MessageHandler<TestMsg>)((in TestMsg _) => _container.Remove(1)));

            Assert.DoesNotThrow(() => _container.Publish(new TestMsg(0)));
        }

        [Test]
        public void Publish_DuringPublish_NewSubscribeDoesNotInvokeInCurrentCycle()
        {
            var extraCallCount = 0;
            _container.SetLastTokenId(1);
            _container.Add((MessageHandler<TestMsg>)((in TestMsg _) =>
            {
                _container.SetLastTokenId(2);
                _container.Add((MessageHandler<TestMsg>)((in TestMsg __) => extraCallCount++));
            }));

            _container.Publish(new TestMsg(0));

            Assert.AreEqual(0, extraCallCount);
        }

        // ─── Remove ────────────────────────────────────────────────────────────

        [Test]
        public void Remove_ExistingHandler_NotInvokedOnNextPublish()
        {
            var called = false;
            _container.SetLastTokenId(1);
            _container.Add((MessageHandler<TestMsg>)((in TestMsg _) => called = true));

            _container.Remove(1);
            _container.Publish(new TestMsg(0));

            Assert.IsFalse(called);
        }

        [Test]
        public void Remove_NonExistentId_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _container.Remove(999));
        }

        [Test]
        public void Remove_OneOfTwo_OtherStillInvoked()
        {
            var a = 0;
            var b = 0;
            _container.SetLastTokenId(1);
            _container.Add((MessageHandler<TestMsg>)((in TestMsg _) => a++));
            _container.SetLastTokenId(2);
            _container.Add((MessageHandler<TestMsg>)((in TestMsg _) => b++));

            _container.Remove(1);
            _container.Publish(new TestMsg(0));

            Assert.AreEqual(0, a);
            Assert.AreEqual(1, b);
        }

        // ─── Dispose ───────────────────────────────────────────────────────────

        [Test]
        public void Dispose_CalledTwice_DoesNotThrow()
        {
            _container.Dispose();
            Assert.DoesNotThrow(() => _container.Dispose());
        }

        [Test]
        public void Dispose_AfterDispose_PublishDoesNotInvokeHandlers()
        {
            var called = false;
            _container.SetLastTokenId(1);
            _container.Add((MessageHandler<TestMsg>)((in TestMsg _) => called = true));

            _container.Dispose();
            _container.Publish(new TestMsg(0));

            Assert.IsFalse(called);
        }

        // ─── Fixture ───────────────────────────────────────────────────────────

        private readonly struct TestMsg : IFluxMessage
        {
            public readonly int Value;
            public TestMsg(int value) { Value = value; }
        }
    }
}
