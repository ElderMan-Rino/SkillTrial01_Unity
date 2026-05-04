using Elder.Framework.Input.Domain.Values;
using NUnit.Framework;

namespace Elder.Framework.Tests.Input
{
    internal sealed class AxisInputDataTests
    {
        [Test]
        public void Constructor_StoresMoveAndLook()
        {
            var move = new InputVector2(1f, 0f);
            var look = new InputVector2(0f, -1f);

            var data = new AxisInputData(move, look);

            Assert.AreEqual(1f, data.Move.X);
            Assert.AreEqual(0f, data.Move.Y);
            Assert.AreEqual(0f, data.Look.X);
            Assert.AreEqual(-1f, data.Look.Y);
        }

        [Test]
        public void IsReadonlyStruct_DefaultIsZero()
        {
            var data = default(AxisInputData);

            Assert.AreEqual(0f, data.Move.X);
            Assert.AreEqual(0f, data.Move.Y);
            Assert.AreEqual(0f, data.Look.X);
            Assert.AreEqual(0f, data.Look.Y);
        }
    }
}
