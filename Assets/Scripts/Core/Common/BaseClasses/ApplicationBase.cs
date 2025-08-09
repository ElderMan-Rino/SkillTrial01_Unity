using Elder.Core.Common.Enums;
using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;

namespace Elder.Core.Common.BaseClasses
{
    public class ApplicationBase : DisposableBase, IApplication
    {
        private IApplicationProvider _appProvider;
        
        private IInfrastructureProvider _infraProvider;
        private IInfrastructureRegister _infraRegister;

        public virtual ApplicationType AppType { get; }

        public virtual bool TryInitialize(IApplicationProvider appProvider, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister)
        {
            InjectAppProvider(appProvider);
            InjectInfraProvider(infraProvider);
            InjectInfraRegister(infraRegister);
            return true;
        }
        private void InjectInfraRegister(IInfrastructureRegister infraRegister)
        {
            _infraRegister = infraRegister;
        }
        private void InjectInfraProvider(IInfrastructureProvider infraProvider)
        {
            _infraProvider = infraProvider;
        }
        private void InjectAppProvider(IApplicationProvider appProvider)
        {
            _appProvider = appProvider;
        }
        public virtual bool TryPostInitialize()
        {
            return true;
        }
        protected bool TryGetApplication<T>(out T targetApp) where T : class, IApplication
        {
            return _appProvider.TryGetApplication<T>(out targetApp);
        }
        protected bool TryGetApplications<T>(out T[] targetApps) where T : class, IApplication
        {
            return _appProvider.TryGetApplications<T>(out targetApps);
        }
        protected bool TryGetInfrastructure<T>(out T targetInfra) where T : class, IInfrastructure
        {
            return _infraProvider.TryGetInfrastructure<T>(out targetInfra);
        }
        protected void RequireInfrastructure<T>() where T : IInfrastructure
        {
            _infraRegister.RegisterInfrastructure<T>();
        }
        protected override void DisposeManagedResources()
        {
            ClearInfraRegister();
            ClearInfraProvider();
            ClearAppProvider();
        }
        private void ClearInfraRegister()
        {
            _infraRegister = null;
        }
        private void ClearInfraProvider()
        {
            _infraProvider = null;
        }
        private void ClearAppProvider()
        {
            _appProvider = null;
        }
        protected override void DisposeUnmanagedResources()
        {

        }

        public virtual void PreDispose()
        {

        }
    }
}
