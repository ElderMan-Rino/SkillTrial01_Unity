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
        private IDisposable _loadingStartedSubToken;
        private ILoadingProgressTracker _loadingProgressTracker;

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

            _loadingStartedSubToken = fluxRouter.Subscribe<FxLoadingStarted>(HandleFxLoadingStared, FluxPhase.Normal);
            return true;
        }

        private void HandleFxLoadingStared(in FxLoadingStarted message)
        {
            // 여기서 UI 열기 요청 처리 
        }

        public override void PreDispose()
        {
            DisposeLoadingSubToken();
            base.PreDispose();
        }

        private void DisposeLoadingSubToken()
        {
            _loadingStartedSubToken?.Dispose();
            _loadingStartedSubToken = null;
        }

        protected override void DisposeManagedResources()
        {
            ClearLogger();
            base.DisposeManagedResources();
        }

        private void ClearLogger()
        {
            _logger = null;
        }
    }
}