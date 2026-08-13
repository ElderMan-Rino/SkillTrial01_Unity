using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using NUnit.Framework;
using System;

namespace Elder.Framework.Tests.EditMode.Framework.Signal
{
    public class SignalTokenTests
    {
        private sealed class SpyCancellable : ISignalCancellable
        {
            public Type UnsubscribedType;
            public long UnsubscribedTokenId;
            public int CallCount;

            public void Unsubscribe(Type signalType, long tokenId)
            {
                UnsubscribedType = signalType;
                UnsubscribedTokenId = tokenId;
                CallCount++;
            }
        }

        [Test]
        public void Dispose_ValidToken_CallsUnsubscribeWithSameArgs()
        {
            var spy = new SpyCancellable();
            var token = new SignalToken(5, typeof(int), spy);

            token.Dispose();

            Assert.AreEqual(1, spy.CallCount);
            Assert.AreEqual(typeof(int), spy.UnsubscribedType);
            Assert.AreEqual(5, spy.UnsubscribedTokenId);
        }

        [Test]
        public void Dispose_EmptyToken_DoesNotThrow()
        {
            var token = SignalToken.Empty;

            Assert.DoesNotThrow(() => token.Dispose());
        }
    }
}
