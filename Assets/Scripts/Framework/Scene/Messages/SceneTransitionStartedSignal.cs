using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Scene.Messages
{
    public readonly struct SceneTransitionStartedSignal : ISignal
    {
        public readonly string TargetSceneKey;

        public SceneTransitionStartedSignal(string targetSceneKey)
        {
            TargetSceneKey = targetSceneKey;
        }
    }
}
