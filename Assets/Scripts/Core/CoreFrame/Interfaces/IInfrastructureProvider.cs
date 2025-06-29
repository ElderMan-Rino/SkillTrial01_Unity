using Elder.Core.Common.Interfaces;

namespace Elder.Core.CoreFrame.Interfaces
{
    public interface IInfrastructureProvider
    {
        public bool TryGetInfrastructure<T>(out T targetInfrastructure) where T : IInfrastructure;
        public void RegisterInfrastructure<T>() where T : IInfrastructure;
    }
}
