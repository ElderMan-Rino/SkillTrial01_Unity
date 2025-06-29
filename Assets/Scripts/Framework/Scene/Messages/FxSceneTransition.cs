using Elder.Framework.Flux.Interfaces;

namespace Elder.Framework.Scene.Messages
{
    public readonly struct FxSceneTransition : IFluxMessage
    {
        public readonly string TargetSceneKey;

        public FxSceneTransition(string targetSceneKey)
        {
            TargetSceneKey = targetSceneKey;
        }
    }
}