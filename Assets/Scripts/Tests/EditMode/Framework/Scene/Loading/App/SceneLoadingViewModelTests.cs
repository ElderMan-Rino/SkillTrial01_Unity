using Elder.Framework.Scene.Loading.App;
using Elder.Framework.Scene.Loading.Definitions;
using NUnit.Framework;

namespace Elder.Framework.Tests.EditMode.Framework.Scene.Loading.App
{
    public class SceneLoadingViewModelTests
    {
        [Test]
        public void Apply_StoresGivenSnapshot()
        {
            var viewModel = new SceneLoadingViewModel();

            viewModel.Apply(new SceneLoadingSnapshot(0.5f));

            Assert.AreEqual(0.5f, viewModel.Snapshot.Progress);
        }

        [Test]
        public void Dispose_ResetsSnapshotToDefault()
        {
            var viewModel = new SceneLoadingViewModel();
            viewModel.Apply(new SceneLoadingSnapshot(1f));

            viewModel.Dispose();

            Assert.AreEqual(default(SceneLoadingSnapshot).Progress, viewModel.Snapshot.Progress);
        }
    }
}
