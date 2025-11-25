using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.Loading.Interfaces.Feedback;

namespace Elder.Platform.Loading.Infrastructure.Feedback
{
    public class LoadingProgressTracker : InfrastructureBase, ILoadingProgressTracker
    {
        public override InfrastructureType InfraType => InfrastructureType.Persistent;

        public override bool TryInitialize(IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister, ISubInfrastructureCreator subInfraCreator, IApplicationProvider appProvider)
        {
            return base.TryInitialize(infraProvider, infraRegister, subInfraCreator, appProvider);
        }
    }
}