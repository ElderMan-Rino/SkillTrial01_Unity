using Elder.Core.FluxMessage.Interfaces;

namespace Elder.Core.GameLevel.Messages
{
    public readonly struct FxRequestGameLevelChange : IFluxMessage
    {
        public readonly string GameLevelKey;

        public FxRequestGameLevelChange(string gameLevelKey)
        {
            GameLevelKey = gameLevelKey;
        }
    }
}