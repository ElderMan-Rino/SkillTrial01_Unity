using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Scene.Messages
{
    public readonly struct SceneTransitionSignal : ISignal
    {
        public readonly string TargetSceneKey;

        public SceneTransitionSignal(string targetSceneKey)
        {
            TargetSceneKey = targetSceneKey;
        }
    }
}
