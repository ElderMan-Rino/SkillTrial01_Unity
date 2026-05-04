using Elder.Framework.Common.Base;
using NUnit.Framework;

namespace Elder.Framework.Tests.Common
{
    internal sealed class DisposableBaseTests
    {
        [Test]
        public void Dispose_CallsAllFourHooks_InOrder()
        {
            var log = new System.Collections.Generic.List<string>();
            var subject = new OrderTrackerDisposable(log);

            subject.Dispose();

            Assert.AreEqual(new[] { "OnDisposing", "Managed", "Unmanaged", "Finalize" }, log);
        }

        [Test]
        public void Dispose_CalledTwice_HooksCalledOnce()
        {
            var count = 0;
            var subject = new CountingDisposable(() => count++);
            subject.Dispose();
            subject.Dispose();

            Assert.AreEqual(1, count);
        }

        [Test]
        public void Dispose_ManagedResourcesHook_NotCalledFromFinalizer()
        {
            // Simulates finalizer path (disposing=false): managed hook must be skipped.
            var managedCalled = false;
            var subject = new ManagedTrackingDisposable(() => managedCalled = true);
            subject.SimulateFinalizer();

            Assert.IsFalse(managedCalled);
        }

        // ─── Helpers ───────────────────────────────────────────────────────────

        private sealed class OrderTrackerDisposable : DisposableBase
        {
            private readonly System.Collections.Generic.List<string> _log;
            public OrderTrackerDisposable(System.Collections.Generic.List<string> log) { _log = log; }
            protected override void OnDisposing() => _log.Add("OnDisposing");
            protected override void DisposeManagedResources() { _log.Add("Managed"); base.DisposeManagedResources(); }
            protected override void DisposeUnmanagedResources() => _log.Add("Unmanaged");
            protected override void FinalizeDispose() => _log.Add("Finalize");
        }

        private sealed class CountingDisposable : DisposableBase
        {
            private readonly System.Action _onManaged;
            public CountingDisposable(System.Action onManaged) { _onManaged = onManaged; }
            protected override void DisposeManagedResources() { _onManaged(); base.DisposeManagedResources(); }
        }

        private sealed class ManagedTrackingDisposable : DisposableBase
        {
            private readonly System.Action _onManaged;
            public ManagedTrackingDisposable(System.Action onManaged) { _onManaged = onManaged; }
            protected override void DisposeManagedResources() { _onManaged(); base.DisposeManagedResources(); }
            // Expose the protected Dispose(bool) path for testing the finalizer branch
            public void SimulateFinalizer() => Dispose(false);
        }
    }
}
