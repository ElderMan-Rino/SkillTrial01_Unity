using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.Logging.Application;
using Elder.Core.Logging.Interfaces;
using Elder.Platform.Logging.Interfaces;
using System.Collections.Generic;

namespace Elder.Platform.Logging.Infrastructure
{
    public class LoggingInfrastructure : InfrastructureBase, ILogEventDispatcher
    {
        private List<ILogAdapter> _logAdapters;

        public override InfrastructureType InfraType => InfrastructureType.Persistent;

        public override bool TryInitialize(IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister, ISubInfrastructureCreator subInfraCreator)
        {
            base.TryInitialize(infraProvider, infraRegister, subInfraCreator);

            InitializeLogAdapterContainer();

            RegistLogAdapter<IUnityLogAdapter>();
            return true;
        }
        private void InitializeLogAdapterContainer()
        {
            _logAdapters = new();
        }
        private void RegistLogAdapter<T>() where T : ISubInfrastructure
        {
            if (!TryCreateSubInfra<T>(out var subInfra))
                return;

            if (subInfra is not ILogAdapter logAdapter)
                return;

            _logAdapters.Add(logAdapter);
        }
        public void DispatchLogEvent(in LogEvent logEvent)
        {
            foreach (var logAdpater in _logAdapters)
                logAdpater.DispatchLogEvent(logEvent);
        }
        protected override void DisposeManagedResources()
        {
            DisposeLogAdapters();
        }
        private void DisposeLogAdapters()
        {
            foreach (var logAdapter in _logAdapters)
                logAdapter.Dispose();

            _logAdapters.Clear();
            _logAdapters = null;
        }
        protected override void DisposeUnmanagedResources()
        {

        }
    }
}
