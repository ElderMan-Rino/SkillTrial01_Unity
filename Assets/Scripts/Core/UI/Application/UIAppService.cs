using Elder.Core.Common.BaseClasses;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.Logging.Helpers;
using Elder.Core.Logging.Interfaces;
using Elder.Core.UI.Interfaces;

namespace Elder.Core.UI.Application
{
    public class UIAppService : ApplicationBase, IUIAppService
    {
        private ILoggerEx _logger;
        private IUIViewInfrastructure _uiViewInfra;

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
            return true;
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