using Elder.Core.Common.Interfaces;

namespace Elder.Core.GameLevel.Interfaces
{
    public interface IGameLevelExecutor : IInfrastructure
    {
        public void RequestGameLevelChange();
    }
}