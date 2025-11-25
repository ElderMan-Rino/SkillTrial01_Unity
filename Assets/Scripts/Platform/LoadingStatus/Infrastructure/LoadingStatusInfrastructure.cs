using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.Loading.Interfaces.Status;

namespace Elder.Platform.LoadingStatus.Infrastructure
{
    public class LoadingStatusInfrastructure : InfrastructureBase, ILoadingStatusProvider
    {
        public override InfrastructureType InfraType => InfrastructureType.Persistent;
    }
}
