using Elder.Framework.Blob.App;
using NUnit.Framework;
using System.Collections.Generic;

namespace Elder.Framework.Tests.EditMode.Framework.Blob.App
{
    public class BlobIndexRegistryTests
    {
        private sealed class DummyRoot { }
        private sealed class OtherRoot { }

        [Test]
        public void TryGetIndex_NoIndexBuilt_ReturnsFalse()
        {
            var registry = new BlobIndexRegistry();

            bool found = registry.TryGetIndex<DummyRoot>(1, out int index);

            Assert.IsFalse(found);
            Assert.AreEqual(-1, index);
        }

        [Test]
        public void BuildIndex_ThenTryGetIndex_ReturnsCorrectIndex()
        {
            var registry = new BlobIndexRegistry();
            registry.BuildIndex<DummyRoot>(new List<int> { 10, 20, 30 });

            bool found = registry.TryGetIndex<DummyRoot>(20, out int index);

            Assert.IsTrue(found);
            Assert.AreEqual(1, index);
        }

        [Test]
        public void TryGetIndex_UnknownId_ReturnsFalse()
        {
            var registry = new BlobIndexRegistry();
            registry.BuildIndex<DummyRoot>(new List<int> { 10, 20 });

            bool found = registry.TryGetIndex<DummyRoot>(999, out int index);

            Assert.IsFalse(found);
        }

        [Test]
        public void TryGetIndex_DifferentRootType_DoesNotMixIndices()
        {
            var registry = new BlobIndexRegistry();
            registry.BuildIndex<DummyRoot>(new List<int> { 10 });

            bool found = registry.TryGetIndex<OtherRoot>(10, out int index);

            Assert.IsFalse(found);
            Assert.AreEqual(-1, index);
        }

        [Test]
        public void BuildIndex_CalledTwiceForSameType_OverwritesPreviousIndex()
        {
            var registry = new BlobIndexRegistry();
            registry.BuildIndex<DummyRoot>(new List<int> { 10, 20 });

            registry.BuildIndex<DummyRoot>(new List<int> { 30 });

            Assert.IsFalse(registry.TryGetIndex<DummyRoot>(10, out _));
            Assert.IsTrue(registry.TryGetIndex<DummyRoot>(30, out int index));
            Assert.AreEqual(0, index);
        }
    }
}
