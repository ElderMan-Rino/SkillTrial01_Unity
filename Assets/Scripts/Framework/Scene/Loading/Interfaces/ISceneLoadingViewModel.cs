using Elder.Framework.Scene.Loading.Definitions;

namespace Elder.Framework.Scene.Loading.Interfaces
{
    public interface ISceneLoadingViewModel
    {
        public SceneLoadingSnapshot Snapshot { get; }
        public void Apply(SceneLoadingSnapshot snapshot);
    }
}
