using Elder.Core.Common.Enums;
using Elder.Core.FluxMessage.Interfaces;

namespace Elder.Core.GameLevel.Messages
{
    public readonly struct FxLoadGameLevelState : IFluxMessage
    {
        public readonly LoadGameLevelState CurrentLoadState;

        public FxLoadGameLevelState(LoadGameLevelState currentLoadState)
        {
            CurrentLoadState = currentLoadState;
        }
    }
}