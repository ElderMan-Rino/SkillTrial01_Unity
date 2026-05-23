using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Scene.Messages
{
    public readonly struct SceneTransitionCompletedSignal : ISignal
    {
        public readonly string LoadedSceneKey;

        public SceneTransitionCompletedSignal(string loadedSceneKey)
        {
            LoadedSceneKey = loadedSceneKey;
        }
    }
}
