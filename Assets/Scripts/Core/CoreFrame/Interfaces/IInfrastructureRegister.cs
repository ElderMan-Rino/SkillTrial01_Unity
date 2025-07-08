using Elder.Core.Common.Enums;
using Elder.Core.Common.Interfaces;

namespace Elder.Core.CoreFrame.Interfaces
{
    public interface IInfrastructureRegister
    {
        public void RegisterInfrastructure<T>() where T : IInfrastructure;
    }
}
