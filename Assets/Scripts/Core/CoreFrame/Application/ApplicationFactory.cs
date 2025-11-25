using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.FluxMessage.Application;
using Elder.Core.FluxMessage.Interfaces;
using Elder.Core.GameLevel.Application;
using Elder.Core.GameLevel.Interfaces;
using Elder.Core.Loading.Application.Feedback;
using Elder.Core.Loading.Interfaces;
using Elder.Core.Loading.Interfaces.Feedback;
using Elder.Core.Logging.Application;
using Elder.Core.Logging.Interfaces;
using Elder.Core.UI.Application;
using Elder.Core.UI.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Core.CoreFrame.Application
{
    public class ApplicationFactory : DisposableBase, IApplicationFactory
    {
        private Dictionary<Type, Func<IApplication>> _constructers;

        public ApplicationFactory()
        {
            InitializeConstructers();
        }
        private void InitializeConstructers()
        {
            _constructers = new()
            {
                { typeof(ILoggerPublisher), () => new LogApplication () },
                { typeof(IFluxRouter), () => new FluxRouter() },
                { typeof(IMainLevelApplication), () => new MainLevelApplication() },
                { typeof(IUIAppService), () => new UIAppService() },
                { typeof(ILoadingFeedbackApplication), () => new LoadingFeedbackApplication() }
            };
        }
        public bool TryCreateApplication(Type type, IApplicationProvider appProvider, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister, out IApplication application)
        {
            if (!_constructers.TryGetValue(type, out var constructer))
            {
                application = null;
                return false;
            }
            application = constructer.Invoke();
            if (!application.TryInitialize(appProvider, infraProvider, infraRegister))
                return false;
            return true;
        }

        protected override void DisposeManagedResources()
        {
            DisposeConstructers();
        }
        private void DisposeConstructers()
        {
            _constructers.Clear();
            _constructers = null;
        }

        protected override void DisposeUnmanagedResources()
        {

        }
    }
}