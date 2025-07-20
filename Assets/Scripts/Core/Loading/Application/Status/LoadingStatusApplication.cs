using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.Loading.Interfaces.Status;
using Elder.Core.Logging.Helpers;
using Elder.Core.Logging.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Core.Loading.Applictaion.Status
{
    public class LoadingStatusApplication : ApplicationBase, ILoadingStatusApplication
    {
        private ILoggerEx _logger;

        private Dictionary<Type, ILoadingStatusReporter> _statusReporters;

        public override ApplicationType AppType => ApplicationType.Persistent; 

        public override bool TryInitialize(IApplicationProvider appProvider, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister)
        {
            if (!base.TryInitialize(appProvider, infraProvider, infraRegister))
                return false;

            if (!TryBindLogger())
                return false;
            
            InitializeReportersContainer();
            return true;
        }
        private bool TryBindLogger()
        {
            _logger = LogFacade.GetLoggerFor<LoadingStatusApplication>();
            return _logger != null;
        }
        private void InitializeReportersContainer()
        {
            _statusReporters = new();
        }
        public bool TryRegisterReporter<T>(T reporter) where T : class, ILoadingStatusReporter
        {
            var type = typeof(T);
            if (!_statusReporters.TryAdd(type, reporter))
            {
                _logger.Error($"Failed to register loading status reporter. Reporter of type '{type.FullName}' is already registered.");
                return false;
            }
            return true;
        }
        protected override void DisposeManagedResources()
        {
            DisposeStatusReporters();
            ClearLogger();
            base.DisposeManagedResources();
        }
        private void ClearLogger()
        {
            _logger = null;
        }
        private void DisposeStatusReporters()
        {
            _statusReporters.Clear();
            _statusReporters = null;
        }
    }
}