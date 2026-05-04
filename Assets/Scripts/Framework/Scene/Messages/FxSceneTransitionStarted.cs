using Elder.Framework.Flux.Interfaces;

namespace Elder.Framework.Scene.Messages
{
    public readonly struct FxSceneTransitionStarted : IFluxMessage
    {
        public readonly string TargetSceneKey;

        public FxSceneTransitionStarted(string targetSceneKey)
        {
            TargetSceneKey = targetSceneKey;
        }
    }
}
