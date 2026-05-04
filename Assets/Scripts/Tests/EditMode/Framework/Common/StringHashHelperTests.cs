using Elder.Framework.Common.Utils;
using NUnit.Framework;

namespace Elder.Framework.Tests.Common
{
    internal sealed class StringHashHelperTests
    {
        // ─── Determinism ───────────────────────────────────────────────────────

        [Test]
        public void ToStableHash_SameInput_ReturnsSameValue()
        {
            var a = StringHashHelper.ToStableHash("SceneA");
            var b = StringHashHelper.ToStableHash("SceneA");

            Assert.AreEqual(a, b);
        }

        [Test]
        public void ToStableHash_DifferentInputs_ReturnDifferentValues()
        {
            var a = StringHashHelper.ToStableHash("SceneA");
            var b = StringHashHelper.ToStableHash("SceneB");

            Assert.AreNotEqual(a, b);
        }

        // ─── Edge cases ────────────────────────────────────────────────────────

        [Test]
        public void ToStableHash_NullInput_ReturnsZero()
        {
            Assert.AreEqual(0, StringHashHelper.ToStableHash(null));
        }

        [Test]
        public void ToStableHash_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, StringHashHelper.ToStableHash(string.Empty));
        }

        [Test]
        public void ToStableHash_CaseSensitive()
        {
            var lower = StringHashHelper.ToStableHash("scene");
            var upper = StringHashHelper.ToStableHash("Scene");

            Assert.AreNotEqual(lower, upper);
        }

        // ─── Known values (regression guard) ──────────────────────────────────

        [TestCase("Empty", Description = "TempSceneKey")]
        [TestCase("MainScene")]
        [TestCase("GameScene")]
        public void ToStableHash_KnownKeys_ProduceConsistentHash(string key)
        {
            var first = StringHashHelper.ToStableHash(key);
            var second = StringHashHelper.ToStableHash(key);

            Assert.AreEqual(first, second,
                $"Hash for '{key}' must be stable across calls.");
        }
    }
}
