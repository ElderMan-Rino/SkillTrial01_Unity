using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.FluxMessage.Interfaces;
using Elder.Core.GameLevel.Interfaces;
using Elder.Core.GameLevel.Messages;
using Elder.Core.LoadingStatus.Applictaion;
using Elder.Core.Logging.Helpers;
using Elder.Core.Logging.Interfaces;
using System;

namespace Elder.Core.GameLevel.Application
{
    public class GameLevelApplication : ApplicationBase, IGameLevelApplication
    {
        private ILoggerEx _logger;
        private IGameLevelExecutor _gameLevelExecutor;
        private IDisposable _gameLevelChangeSubToken;

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
            if (!TrySubscribeToGameLevelChange())
                return false;

            if (!TryBindGameLevelExecutor())
                return false;

            return true;
        }
        private bool TrySubscribeToGameLevelChange()
        {
            if (!TryGetApplication<IFluxRouter>(out var fluxRouter))
                return false;

            _gameLevelChangeSubToken = fluxRouter.Subscribe<FxRequestGameLevelChange>(HandleFxRequestGameLevelChange);
            return true;
        }
        private void HandleFxRequestGameLevelChange(in FxRequestGameLevelChange message)
        {
            RequestGameLevelChange(message.GameLevelKey);
        }
        private void RequestGameLevelChange(string gameLevelKey)
        {
            _gameLevelExecutor.RequestGameLevelChange(gameLevelKey);
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
        public override void PreDispose()
        {
            DisposeGameLevelChangeSubToken();
            base.PreDispose();
        }
        protected override void DisposeManagedResources()
        {
            ClearGameLevelExecutor();
            ClearLogger();
            base.DisposeManagedResources();
        }
        private void DisposeGameLevelChangeSubToken()
        {
            _gameLevelChangeSubToken.Dispose();
            _gameLevelChangeSubToken = null;
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