using Elder.Framework.Core.Interfaces.Base;
using NUnit.Framework;

namespace Elder.Framework.Tests.EditMode.Framework.Core.Interfaces.Base
{
    public class DisposableBaseTests
    {
        private sealed class TestDisposable : DisposableBase
        {
            public int PreDisposeCallCount;
            public int OnDisposingCallCount;
            public int DisposeManagedResourcesCallCount;
            public int DisposeUnmanagedResourcesCallCount;
            public int FinalizeDisposeCallCount;

            public override void PreDispose() => PreDisposeCallCount++;
            protected override void OnDisposing() => OnDisposingCallCount++;
            protected override void DisposeManagedResources() => DisposeManagedResourcesCallCount++;
            protected override void DisposeUnmanagedResources() => DisposeUnmanagedResourcesCallCount++;
            protected override void FinalizeDispose() => FinalizeDisposeCallCount++;
        }

        [Test]
        public void Dispose_CallsHooksInOrder()
        {
            var target = new TestDisposable();

            target.Dispose();

            Assert.AreEqual(1, target.OnDisposingCallCount);
            Assert.AreEqual(1, target.DisposeManagedResourcesCallCount);
            Assert.AreEqual(1, target.DisposeUnmanagedResourcesCallCount);
            Assert.AreEqual(1, target.FinalizeDisposeCallCount);
        }

        [Test]
        public void Dispose_CalledTwice_HooksRunOnlyOnce()
        {
            var target = new TestDisposable();

            target.Dispose();
            target.Dispose();

            Assert.AreEqual(1, target.OnDisposingCallCount);
            Assert.AreEqual(1, target.DisposeManagedResourcesCallCount);
        }

        [Test]
        public void PreDispose_NotCalledAutomaticallyByDispose()
        {
            var target = new TestDisposable();

            target.Dispose();

            Assert.AreEqual(0, target.PreDisposeCallCount);
        }
    }
}
