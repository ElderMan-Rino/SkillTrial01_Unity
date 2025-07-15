using Elder.Core.Common.Enums;
using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;

namespace Elder.Core.Common.BaseClasses
{
    public abstract class InfrastructureBase : DisposableBase, IInfrastructure
    {
        private IInfrastructureProvider _infraProvider;
        private IInfrastructureRegister _infraRegister;
        private ISubInfrastructureCreator _subInfraCreator;

        public virtual InfrastructureType InfraType { get; }

        public virtual void Initialize(IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister, ISubInfrastructureCreator subInfraCreator)
        {
            InjectInfraProvider(infraProvider);
            InjectInfraRegister(infraRegister);
            InjectSubInfraCreator(subInfraCreator);
        }
        private void InjectSubInfraCreator(ISubInfrastructureCreator subInfraCreator)
        {
            _subInfraCreator = subInfraCreator;
        }
        private void InjectInfraProvider(IInfrastructureProvider infraProvider)
        {
            _infraProvider = infraProvider;
        }
        private void InjectInfraRegister(IInfrastructureRegister infraRegister)
        {
            _infraRegister = infraRegister;
        }
        protected bool TryGetInfrastructure<T>(out T targetInfrastructure) where T : class, IInfrastructure
        {
            return _infraProvider.TryGetInfrastructure<T>(out targetInfrastructure);
        }
        protected void RegisterInfrastructure<T>() where T : IInfrastructure
        {
            _infraRegister.RegisterInfrastructure<T>();
        }
        protected bool TryCreateSubInfra<T>(out ISubInfrastructure subInfra) where T : ISubInfrastructure
        {
            return _subInfraCreator.TryCreateSubInfra<T>(out subInfra);
        }
        protected override void DisposeManagedResources()
        {
            ClearSubInfraCreator();
            ClearInfraRegister();
            ClearInfraProvider();
        }
        private void ClearSubInfraCreator()
        {
            _subInfraCreator = null;
        }
        private void ClearInfraRegister()
        {
            _infraRegister = null;
        }
        private void ClearInfraProvider()
        {
            _infraProvider = null;
        }
        protected override void DisposeUnmanagedResources()
        {

        }
    }
}