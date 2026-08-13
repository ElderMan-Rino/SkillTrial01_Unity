using Elder.Framework.Signal.Infra;
using Elder.Framework.Signal.Interfaces;
using NUnit.Framework;

namespace Elder.Framework.Tests.EditMode.Framework.Signal
{
    public class SignalRouterTests
    {
        private readonly struct DummySignal : ISignal
        {
            public readonly int Value;
            public DummySignal(int value) => Value = value;
        }

        private readonly struct OtherSignal : ISignal { }

        [Test]
        public void Publish_NoSubscribers_DoesNotThrow()
        {
            var router = new SignalRouter();

            Assert.DoesNotThrow(() => router.Publish(new DummySignal(1)));
        }

        [Test]
        public void Subscribe_ThenPublish_InvokesHandlerWithMessage()
        {
            var router = new SignalRouter();
            int received = -1;

            router.Subscribe<DummySignal>((in DummySignal signal) => received = signal.Value);
            router.Publish(new DummySignal(7));

            Assert.AreEqual(7, received);
        }

        [Test]
        public void Publish_DifferentMessageType_DoesNotInvokeUnrelatedHandler()
        {
            var router = new SignalRouter();
            bool invoked = false;

            router.Subscribe<DummySignal>((in DummySignal signal) => invoked = true);
            router.Publish(new OtherSignal());

            Assert.IsFalse(invoked);
        }

        [Test]
        public void TokenDispose_UnsubscribesHandler()
        {
            var router = new SignalRouter();
            bool invoked = false;

            var token = router.Subscribe<DummySignal>((in DummySignal signal) => invoked = true);
            token.Dispose();
            router.Publish(new DummySignal(1));

            Assert.IsFalse(invoked);
        }

        [Test]
        public void Subscribe_MultipleHandlersSameType_AllInvoked()
        {
            var router = new SignalRouter();
            int count = 0;

            router.Subscribe<DummySignal>((in DummySignal signal) => count++);
            router.Subscribe<DummySignal>((in DummySignal signal) => count++);
            router.Publish(new DummySignal(1));

            Assert.AreEqual(2, count);
        }

        [Test]
        public void Dispose_ThenPublish_DoesNotInvokeHandler()
        {
            var router = new SignalRouter();
            bool invoked = false;
            router.Subscribe<DummySignal>((in DummySignal signal) => invoked = true);

            router.Dispose();
            router.Publish(new DummySignal(1));

            Assert.IsFalse(invoked);
        }
    }
}
