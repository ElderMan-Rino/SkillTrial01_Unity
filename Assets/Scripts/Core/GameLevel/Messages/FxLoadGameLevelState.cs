using Elder.Core.Common.Enums;
using Elder.Core.FluxMessage.Interfaces;

namespace Elder.Core.GameLevel.Messages
{
    public readonly struct FxLoadGameLevelState : IFluxMessage
    {
        public readonly GameLevelLoadState CurrentLoadState;

        public FxLoadGameLevelState(GameLevelLoadState currentLoadState)
        {
            CurrentLoadState = currentLoadState;
        }
    }
}