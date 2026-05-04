using Elder.Framework.Asset.App;
using NUnit.Framework;
using UnityEngine;

namespace Elder.Framework.Tests.Asset
{
    internal sealed class AssetHandleTests
    {
        // ─── Dispose / release ─────────────────────────────────────────────────

        [Test]
        public void Dispose_InvokesReleaseAction()
        {
            var released = false;
            var handle = new AssetHandle<ScriptableObject>(null, () => released = true);

            handle.Dispose();

            Assert.IsTrue(released);
        }

        [Test]
        public void Dispose_CalledTwice_ReleaseActionInvokedOnce()
        {
            var count = 0;
            var handle = new AssetHandle<ScriptableObject>(null, () => count++);

            handle.Dispose();
            handle.Dispose();

            Assert.AreEqual(1, count);
        }

        [Test]
        public void Dispose_NullReleaseAction_DoesNotThrow()
        {
            var handle = new AssetHandle<ScriptableObject>(null, null);
            Assert.DoesNotThrow(() => handle.Dispose());
        }

        // ─── Asset property ────────────────────────────────────────────────────

        [Test]
        public void Asset_ReturnsProvidedReference()
        {
            var obj = ScriptableObject.CreateInstance<ScriptableObject>();
            var handle = new AssetHandle<ScriptableObject>(obj, null);

            Assert.AreSame(obj, handle.Asset);

            Object.DestroyImmediate(obj);
        }
    }
}
