using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.FluxMessage.Interfaces;
using Elder.Core.Loading.Application.Status;
using Elder.Core.Loading.Interfaces.Feedback;
using Elder.Core.Loading.Messages;
using Elder.Core.Logging.Helpers;
using Elder.Core.Logging.Interfaces;
using System;

namespace Elder.Core.Loading.Application.Feedback
{
    public class LoadingFeedbackApplication : ApplicationBase, ILoadingFeedbackApplication
    {
        private ILoggerEx _logger;
        private IDisposable _loadingToken;
        public override ApplicationType AppType => ApplicationType.Persistent;
        
        public override bool TryInitialize(IApplicationProvider appProvider, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister)
        {
            if (!TryBindLogger())
                return false;

            return base.TryInitialize(appProvider, infraProvider, infraRegister);
        }
        
        private bool TryBindLogger()
        {
            _logger = LogFacade.GetLoggerFor<LoadingStatusApplication>();
            return _logger != null;
        }
        
        public override bool TryPostInitialize()
        {
            if (!TrySubscribeToStartLoadingFeedback())
                return false;

            return base.TryPostInitialize();
        }

        private bool TrySubscribeToStartLoadingFeedback()
        {
            if (!TryGetApplication<IFluxRouter>(out var fluxRouter))
                return false;

            _loadingToken = fluxRouter.Subscribe<FxLoadingStared>(HandleFxLoadingStared, FluxPhase.Normal);
            return true;
        }

        private void HandleFxLoadingStared(in FxLoadingStared message)
        {

        }
    }
}