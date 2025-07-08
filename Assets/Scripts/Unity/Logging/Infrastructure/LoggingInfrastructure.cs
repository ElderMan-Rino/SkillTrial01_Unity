using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.Logging.Application;
using Elder.Core.Logging.Interfaces;

namespace Elder.Unity.Logging.Infrastructure
{
    public class LoggingInfrastructure : InfrastructureBase, ILogEventHandler
    {
        public override InfrastructureType InfraType => InfrastructureType.Persistent;

        public override void Initialize(IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister)
        {
            base.Initialize(infraProvider, infraRegister);
        }
        public void HandleLogEvent(LogEvent logEvent)
        {

        }
        protected override void DisposeManagedResources()
        {

        }

        protected override void DisposeUnmanagedResources()
        {

        }
    }
}
