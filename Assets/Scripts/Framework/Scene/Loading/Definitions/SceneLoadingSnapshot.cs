namespace Elder.Framework.Scene.Loading.Definitions
{
    public readonly struct SceneLoadingSnapshot
    {
        public readonly float Progress;

        public SceneLoadingSnapshot(float progress)
        {
            Progress = progress;
        }
    }
}
