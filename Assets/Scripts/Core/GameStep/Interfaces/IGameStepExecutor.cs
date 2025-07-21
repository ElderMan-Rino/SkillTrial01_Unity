using Elder.Core.Common.Interfaces;

namespace Elder.Core.GameStep.Interfaces
{
    public interface IGameStepExecutor : IInfrastructure
    {
        public void RequestGameStepChange();
    }
}