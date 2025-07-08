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
            // �� �ᱹ �ϳ��δ� ó���� �� �Ǵµ�
            // ���⼭ ����Ƽ �α� �߰� ��û
            // ����Ƽ �α׸� �����ͼ� 
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
