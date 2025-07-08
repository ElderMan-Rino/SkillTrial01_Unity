using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.Logging.Application;
using Elder.Core.Logging.Interfaces;
using System.Collections.Generic;

namespace Elder.Unity.Logging.Infrastructure
{
    public class LoggingInfrastructure : InfrastructureBase, ILogEventHandler
    {
        private List<ILoggerAdapter> _logAdapters;

        public override InfrastructureType InfraType => InfrastructureType.Persistent;

        public override void Initialize(IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister)
        {
            base.Initialize(infraProvider, infraRegister);
            InitializeLogAdapterContainer();
            // 흠 결국 하나로는 처리가 안 되는데
            // 여기서 유니티 로그 추가 요청
            // 유니티 로그를 가져와서 
            // 
        }
        private void InitializeLogAdapterContainer()
        {
            _logAdapters = new();
        }
        public void HandleLogEvent(LogEvent logEvent)
        {

        }
        protected override void DisposeManagedResources()
        {
            DisposeLogAdapters();
        }
        private void DisposeLogAdapters()
        {
            _logAdapters.Clear();
            _logAdapters = null;
        }
        protected override void DisposeUnmanagedResources()
        {

        }
    }
}
