using Elder.Framework.Common.Utils;
using NUnit.Framework;

namespace Elder.Framework.Tests.EditMode.Framework.Common
{
    public class StringHashHelperTests
    {
        [Test]
        public void ToStableHash_NullString_ReturnsZero()
        {
            var hash = StringHashHelper.ToStableHash(null);

            Assert.AreEqual(0, hash);
        }

        [Test]
        public void ToStableHash_EmptyString_ReturnsZero()
        {
            var hash = StringHashHelper.ToStableHash(string.Empty);

            Assert.AreEqual(0, hash);
        }

        [Test]
        public void ToStableHash_SameInput_ReturnsSameHash()
        {
            var first = StringHashHelper.ToStableHash("Elder.Framework");
            var second = StringHashHelper.ToStableHash("Elder.Framework");

            Assert.AreEqual(first, second);
        }

        [Test]
        public void ToStableHash_DifferentInput_ReturnsDifferentHash()
        {
            var first = StringHashHelper.ToStableHash("Elder.Framework");
            var second = StringHashHelper.ToStableHash("Elder.Framework.Common");

            Assert.AreNotEqual(first, second);
        }

        [Test]
        public void ToStableHash_CaseSensitiveInput_ReturnsDifferentHash()
        {
            var lower = StringHashHelper.ToStableHash("elder");
            var upper = StringHashHelper.ToStableHash("Elder");

            Assert.AreNotEqual(lower, upper);
        }
    }
}
