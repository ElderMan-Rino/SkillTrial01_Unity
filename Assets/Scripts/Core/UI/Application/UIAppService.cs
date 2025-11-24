using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.FluxMessage.Interfaces;
using Elder.Core.GameLevel.Messages;
using Elder.Core.Logging.Helpers;
using Elder.Core.Logging.Interfaces;
using Elder.Core.UI.Interfaces;
using System;

namespace Elder.Core.UI.Application
{
    public class UIAppService : ApplicationBase, IUIAppService
    {
        private ILoggerEx _logger;
        private IUIViewInfrastructure _uiViewInfra;
        private IDisposable _loadGameLevelStateSubToken;

        public override bool TryInitialize(IApplicationProvider appProvider, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister)
        {
            if (!TryBindLogger())
                return false;
            
            base.TryInitialize(appProvider, infraProvider, infraRegister);
            RequireUIViewInfra();
            return true;
        }
        private bool TryBindLogger()
        {
            _logger = LogFacade.GetLoggerFor<UIAppService>();
            return _logger != null;
        }
        private void RequireUIViewInfra()
        {
            RequireInfrastructure<IUIViewInfrastructure>();
        }
        public override bool TryPostInitialize()
        {
            if (!TryBindUIViewInfra())
                return false;

            if (!TrySubscribeToLoadGameLevelState())
                return false;

            return true;
        }
        private bool TrySubscribeToLoadGameLevelState()
        {
            if (!TryGetApplication<IFluxRouter>(out var fluxRouter))
                return false;

            _loadGameLevelStateSubToken = fluxRouter.Subscribe<FxLoadGameLevelState>(HandleFxLoadGameLevelState, FluxPhase.Pre);
            return true;
        }
        private void HandleFxLoadGameLevelState(in FxLoadGameLevelState message)
        {
            if (message.CurrentLoadState != LoadGameLevelState.UnloadLoading)
                return;

            RequestRegisterViews();
        }
        private void RequestRegisterViews()
        {
            _uiViewInfra.RegisterViews();
        }
        private bool TryBindUIViewInfra()
        {
            if (!TryGetInfrastructure<IUIViewInfrastructure>(out var uiViewInfra))
            {
                _logger.Error("Failed to retrieve IUIViewInfrastructure from infrastructure. It may not be initialized or registered.");
                return false;
            }
            _uiViewInfra = uiViewInfra;
            return true;
        }
        public override void PreDispose()
        {
            DisposeLoadGameLevelStateSubToken();
            base.PreDispose();
        }
        private void DisposeLoadGameLevelStateSubToken()
        {
            _loadGameLevelStateSubToken.Dispose();
            _loadGameLevelStateSubToken = null;
        }
        protected override void DisposeManagedResources()
        {
            ClearUIViewInfra();
        }
        private void ClearUIViewInfra()
        {
            _uiViewInfra = null;
        }
    }
}