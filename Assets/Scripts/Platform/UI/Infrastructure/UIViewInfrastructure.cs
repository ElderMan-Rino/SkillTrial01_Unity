using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.UI.Interfaces;

namespace Elder.Platform.UI.Infrastructure
{
    public class UIViewInfrastructure : InfrastructureBase, IUIViewInfrastructure
    {
        public override InfrastructureType InfraType => InfrastructureType.Persistent;

        public void RegisterViews()
        {

        }
    }
}