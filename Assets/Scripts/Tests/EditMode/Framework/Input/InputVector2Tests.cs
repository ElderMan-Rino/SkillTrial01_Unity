using Elder.Framework.Input.Domain.Values;
using NUnit.Framework;

namespace Elder.Framework.Tests.Input
{
    internal sealed class InputVector2Tests
    {
        [Test]
        public void Constructor_SetsXAndY()
        {
            var v = new InputVector2(1f, -0.5f);

            Assert.AreEqual(1f, v.X);
            Assert.AreEqual(-0.5f, v.Y);
        }

        [Test]
        public void Zero_BothComponentsAreZero()
        {
            Assert.AreEqual(0f, InputVector2.Zero.X);
            Assert.AreEqual(0f, InputVector2.Zero.Y);
        }

        [Test]
        public void IsReadonlyStruct_CopyDoesNotAffectOriginal()
        {
            var original = new InputVector2(3f, 4f);
            var copy = original;   // struct copy

            Assert.AreEqual(original.X, copy.X);
            Assert.AreEqual(original.Y, copy.Y);
        }
    }
}
