using Elder.Core.FluxMessage.Interfaces;

namespace Elder.Core.GameLevel.Messages
{
    public readonly struct FxRequestMainLevelChange : IFluxMessage
    {
        public readonly string MainLevelKey;

        public FxRequestMainLevelChange(string gameLevelKey)
        {
            MainLevelKey = gameLevelKey;
        }
    }
}