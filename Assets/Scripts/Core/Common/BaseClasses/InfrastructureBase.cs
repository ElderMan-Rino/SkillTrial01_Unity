
using Elder.Core.Common.Enums;
using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;

namespace Elder.Core.Common.BaseClasses
{
    public abstract class InfrastructureBase : DisposableBase, IInfrastructure
    {
        private IInfrastructureProvider _infraProvider;
        private IInfrastructureRegister _infraRegister;

        public virtual InfrastructureType InfraType { get; }

        public virtual void Initialize(IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister)
        {
            InjectInfraProvider(infraProvider);
            InjectInfraRegister(infraRegister);
        }
        private void InjectInfraProvider(IInfrastructureProvider infraProvider)
        {
            _infraProvider = infraProvider;
        }
        private void InjectInfraRegister(IInfrastructureRegister infraRegister)
        {
            _infraRegister = infraRegister;
        }
        protected override void DisposeManagedResources()
        {

        }

        protected override void DisposeUnmanagedResources()
        {

        }
    }
}