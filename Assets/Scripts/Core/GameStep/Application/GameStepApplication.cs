using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.GameStep.Interfaces;
using Elder.Core.LoadingStatus.Applictaion;
using Elder.Core.Logging.Helpers;
using Elder.Core.Logging.Interfaces;

namespace Elder.Core.GameStep.Application
{
    public class GameStepApplication : ApplicationBase, IGameStepApplication
    {
        private ILoggerEx _logger;
        private IGameStepExecutor _gameStepExecutor;

        public override ApplicationType AppType => ApplicationType.Persistent;

        public override bool TryInitialize(IApplicationProvider appProvider, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister)
        {
            if (!TryBindLogger())
                return false;

            base.TryInitialize(appProvider, infraProvider, infraRegister);
            RequireGameStepInfra();
            return true;
        }
        private void RequireGameStepInfra()
        {
            RequireInfrastructure<IGameStepExecutor>();
        }
        private bool TryBindLogger()
        {
            _logger = LogFacade.GetLoggerFor<LoadingStatusApplication>();
            return _logger != null;
        }
        public override bool TryPostInitialize()
        {
            if (!base.TryPostInitialize())
                return false;

            if (!TryBindGameStepExecutor())
                return false;

            return true;
        }
        private bool TryBindGameStepExecutor()
        {
            if (!TryGetInfrastructure<IGameStepExecutor>(out var gameStepExecutor))
            {
                _logger.Error("Failed to retrieve IGameStepExecutor from infrastructure. It may not be initialized or registered.");
                return false;
            }
            _gameStepExecutor = gameStepExecutor;
            return true;
        }
        protected override void DisposeManagedResources()
        {
            ClearGameStepExecutor();
            ClearLogger();
            base.DisposeManagedResources();
        }
        private void ClearLogger()
        {
            _logger = null;
        }
        private void ClearGameStepExecutor()
        {
            _gameStepExecutor = null;
        }
    }
}