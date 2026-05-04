using Elder.Framework.Flux.Interfaces;

namespace Elder.Framework.Scene.Messages
{
    public readonly struct FxSceneTransitionCompleted : IFluxMessage
    {
        public readonly string LoadedSceneKey;

        public FxSceneTransitionCompleted(string loadedSceneKey)
        {
            LoadedSceneKey = loadedSceneKey;
        }
    }
}
