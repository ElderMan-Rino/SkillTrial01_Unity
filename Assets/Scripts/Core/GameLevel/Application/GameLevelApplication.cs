using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.FluxMessage.Interfaces;
using Elder.Core.GameLevel.Interfaces;
using Elder.Core.GameLevel.Messages;
using Elder.Core.LoadingStatus.Applictaion;
using Elder.Core.Logging.Helpers;
using Elder.Core.Logging.Interfaces;

namespace Elder.Core.GameLevel.Application
{
    public class GameLevelApplication : ApplicationBase, IGameLevelApplication
    {
        private ILoggerEx _logger;
        private IGameLevelExecutor _gameLevelExecutor;
        private IFluxRouter _fluxRouter;

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
            RequireInfrastructure<IGameLevelExecutor>();
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

            if (!TryBindFluxRouter())
                return false;

            if (!TryBindGameLevelExecutor())
                return false;

            SubscribeToFluxRouter();
            return true;
        }
        private bool TryBindFluxRouter()
        {
            if (!TryGetApplication<IFluxRouter>(out var fluxRouter))
                return false;

            _fluxRouter = fluxRouter;
            return true;
        }
        private void SubscribeToFluxRouter()
        {
            _fluxRouter.Subscribe<FxRequestGameLevelChange>(HandleFxRequestGameLevelChange);
        }
        private void HandleFxRequestGameLevelChange(in FxRequestGameLevelChange message)
        {
            RequestGameLevelChange();
        }
        private void RequestGameLevelChange()
        {
            _gameLevelExecutor.RequestGameLevelChange();
        }
        private bool TryBindGameLevelExecutor()
        {
            if (!TryGetInfrastructure<IGameLevelExecutor>(out var gameLevelExecutor))
            {
                _logger.Error("Failed to retrieve IGameLevelExecutor from infrastructure. It may not be initialized or registered.");
                return false;
            }
            _gameLevelExecutor = gameLevelExecutor;
            return true;
        }
        protected override void DisposeManagedResources()
        {
            ClearGameLevelExecutor();
            ClearLogger();
            base.DisposeManagedResources();
        }
        private void ClearLogger()
        {
            _logger = null;
        }
        private void ClearGameLevelExecutor()
        {
            _gameLevelExecutor = null;
        }
    }
}