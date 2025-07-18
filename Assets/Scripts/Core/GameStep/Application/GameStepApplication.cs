using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.GameStep.Interfaces;

namespace Elder.Core.GameStep.Application
{
    public class GameStepApplication : ApplicationBase, IGameStepApplication
    {
        private IGameStepExecutor _gameStepExecutor;

        public override ApplicationType AppType => ApplicationType.Persistent;

        public override bool TryInitialize(IApplicationProvider appProvider, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister)
        {
            return base.TryInitialize(appProvider, infraProvider, infraRegister);
        }
        public override bool TryPostInitialize()
        {
            return base.TryPostInitialize();
        }
    }
}