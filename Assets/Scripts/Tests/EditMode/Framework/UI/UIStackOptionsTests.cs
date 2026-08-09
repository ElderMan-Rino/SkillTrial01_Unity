using Elder.Framework.UI.Definitions;
using NUnit.Framework;

namespace Elder.Framework.Tests.EditMode.Framework.UI
{
    public class UIStackOptionsTests
    {
        [Test]
        public void Default_HasMaxStackSizeOf16()
        {
            Assert.AreEqual(16, UIStackOptions.Default.MaxStackSize);
        }

        [Test]
        public void Constructor_PositiveValue_SetsMaxStackSize()
        {
            var options = new UIStackOptions(5);

            Assert.AreEqual(5, options.MaxStackSize);
        }

        [Test]
        public void Constructor_ZeroOrNegativeValue_ClampsToOne()
        {
            var zero = new UIStackOptions(0);
            var negative = new UIStackOptions(-3);

            Assert.AreEqual(1, zero.MaxStackSize);
            Assert.AreEqual(1, negative.MaxStackSize);
        }
    }
}
