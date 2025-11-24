using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.Loading.Interfaces.Status;
using Elder.Core.Logging.Helpers;
using Elder.Core.Logging.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Core.Loading.Application.Status
{
    public class LoadingStatusApplication : ApplicationBase, ILoadingStatusApplication
    {
        private ILoggerEx _logger;
        private ILoadingStatusProvider _loadingStatusProvider;
        private Dictionary<Type, ILoadingStatusReporter> _statusReporters;

        public override ApplicationType AppType => ApplicationType.Persistent; 

        public override bool TryInitialize(IApplicationProvider appProvider, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister)
        {
            if (!TryBindLogger())
                return false;

            if (!base.TryInitialize(appProvider, infraProvider, infraRegister))
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
            ClearStatusReporters();
            ClearLogger();
            base.DisposeManagedResources();
        }
        private void ClearLogger()
        {
            _logger = null;
        }
        private void ClearStatusReporters()
        {
            _statusReporters.Clear();
            _statusReporters = null;
        }
    }
}