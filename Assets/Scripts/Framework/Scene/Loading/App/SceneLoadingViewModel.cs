using Elder.Framework.Common.Base;
using Elder.Framework.Scene.Loading.Definitions;
using Elder.Framework.Scene.Loading.Interfaces;

namespace Elder.Framework.Scene.Loading.App
{
    internal sealed class SceneLoadingViewModel : DisposableBase, ISceneLoadingViewModel
    {
        private SceneLoadingSnapshot _snapshot;

        public SceneLoadingSnapshot Snapshot => _snapshot;

        public void Apply(SceneLoadingSnapshot snapshot) => _snapshot = snapshot;

        public override void PreDispose() { }

        protected override void DisposeManagedResources()
        {
            _snapshot = default;
        }
    }
}
