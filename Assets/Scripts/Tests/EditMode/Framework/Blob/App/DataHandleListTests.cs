using Elder.Framework.Blob.App;
using Elder.Framework.Blob.Interfaces;
using NUnit.Framework;
using Unity.Entities;

namespace Elder.Framework.Tests.EditMode.Framework.Blob.App
{
    public class DataHandleListTests
    {
        private struct DummyBlob { }

        private sealed class SpyHandle : IBlobDataHandle<DummyBlob>
        {
            public int DisposeCallCount;

            public bool IsCreated => false;

            public bool TryGetData(out DummyBlob data)
            {
                data = default;
                return false;
            }

            public BlobAssetReference<DummyBlob> GetRawReference() => default;

            public ref DummyBlob GetRef() => throw new System.NotSupportedException();

            public void Dispose() => DisposeCallCount++;
        }

        [Test]
        public void Constructor_StoresGivenScope()
        {
            var list = new DataHandleList<DummyBlob>(7);

            Assert.AreEqual(7, list.Scope);
        }

        [Test]
        public void Add_IncreasesCount()
        {
            var list = new DataHandleList<DummyBlob>(0);
            var handle = new SpyHandle();

            list.Add(handle);

            Assert.AreEqual(1, list.Count);
            Assert.AreSame(handle, list[0]);
        }

        [Test]
        public void DisposeAll_DisposesEveryHandleAndClears()
        {
            var list = new DataHandleList<DummyBlob>(0);
            var first = new SpyHandle();
            var second = new SpyHandle();
            list.Add(first);
            list.Add(second);

            list.DisposeAll();

            Assert.AreEqual(1, first.DisposeCallCount);
            Assert.AreEqual(1, second.DisposeCallCount);
            Assert.AreEqual(0, list.Count);
        }
    }
}
