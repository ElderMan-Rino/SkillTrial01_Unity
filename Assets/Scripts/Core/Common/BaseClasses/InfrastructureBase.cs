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
        private IApplicationProvider _appProvider;

        public abstract InfrastructureType InfraType { get; }

        public virtual bool TryInitialize(IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister, ISubInfrastructureCreator subInfraCreator, IApplicationProvider appProvider)
        {
            InjectInfraProvider(infraProvider);
            InjectInfraRegister(infraRegister);
            InjectSubInfraCreator(subInfraCreator);
            InjectAppProvider(appProvider);
            return true;
        }
       
        private void InjectAppProvider(IApplicationProvider appProvider)
        {
            _appProvider = appProvider; 
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
        protected bool TryGetApplication<T>(out T targetApplication) where T : class, IApplication
        {
            return _appProvider.TryGetApplication<T>(out targetApplication);
        }
        protected override void DisposeManagedResources()
        {
            ClearAppProvider();
            ClearSubInfraCreator();
            ClearInfraRegister();
            ClearInfraProvider();
        }
        private void ClearAppProvider()
        {
            _appProvider = null;
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

        public virtual void PreDispose()
        {

        }
    }
}