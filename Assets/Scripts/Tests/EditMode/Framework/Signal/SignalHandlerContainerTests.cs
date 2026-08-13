using Elder.Framework.Signal.Definitions;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using NUnit.Framework;

namespace Elder.Framework.Tests.EditMode.Framework.Signal
{
    public class SignalHandlerContainerTests
    {
        private readonly struct DummySignal : ISignal
        {
            public readonly int Value;
            public DummySignal(int value) => Value = value;
        }

        [Test]
        public void Publish_SingleHandler_InvokesWithMessage()
        {
            var container = new SignalHandlerContainer<DummySignal>();
            int received = -1;
            container.SetLastTokenId(1);
            container.Add((in DummySignal signal) => received = signal.Value);

            container.Publish(new DummySignal(42));

            Assert.AreEqual(42, received);
        }

        [Test]
        public void Publish_MultipleHandlers_InvokesAllInAddOrder()
        {
            var container = new SignalHandlerContainer<DummySignal>();
            var order = new System.Collections.Generic.List<int>();
            container.SetLastTokenId(1);
            container.Add((in DummySignal signal) => order.Add(1));
            container.SetLastTokenId(2);
            container.Add((in DummySignal signal) => order.Add(2));

            container.Publish(new DummySignal(0));

            CollectionAssert.AreEqual(new[] { 1, 2 }, order);
        }

        [Test]
        public void Remove_ExistingToken_HandlerNoLongerInvoked()
        {
            var container = new SignalHandlerContainer<DummySignal>();
            bool invoked = false;
            container.SetLastTokenId(1);
            container.Add((in DummySignal signal) => invoked = true);

            container.Remove(1);
            container.Publish(new DummySignal(0));

            Assert.IsFalse(invoked);
        }

        [Test]
        public void Remove_MiddleToken_RemainingHandlersStillInvoked()
        {
            var container = new SignalHandlerContainer<DummySignal>();
            var invoked = new System.Collections.Generic.List<int>();
            container.SetLastTokenId(1);
            container.Add((in DummySignal signal) => invoked.Add(1));
            container.SetLastTokenId(2);
            container.Add((in DummySignal signal) => invoked.Add(2));
            container.SetLastTokenId(3);
            container.Add((in DummySignal signal) => invoked.Add(3));

            container.Remove(2);
            container.Publish(new DummySignal(0));

            CollectionAssert.AreEquivalent(new[] { 1, 3 }, invoked);
        }

        [Test]
        public void Remove_UnknownToken_DoesNothing()
        {
            var container = new SignalHandlerContainer<DummySignal>();
            bool invoked = false;
            container.SetLastTokenId(1);
            container.Add((in DummySignal signal) => invoked = true);

            container.Remove(999);
            container.Publish(new DummySignal(0));

            Assert.IsTrue(invoked);
        }

        [Test]
        public void Dispose_ClearsAllHandlers()
        {
            var container = new SignalHandlerContainer<DummySignal>();
            bool invoked = false;
            container.SetLastTokenId(1);
            container.Add((in DummySignal signal) => invoked = true);

            container.Dispose();
            container.Publish(new DummySignal(0));

            Assert.IsFalse(invoked);
        }
    }
}
