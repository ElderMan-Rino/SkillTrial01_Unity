using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;
using System.Collections.Generic;
using System;
using Elder.Unity.Logging.Infrastructure;
using Elder.Core.Logging.Interfaces;

namespace Elder.Core.CoreFrame.Application
{
    public class CoreFrameApplication : DisposableBase, IApplicationProvider
    {
        private IInfrastructureProvider _infraProvider;
        private IInfrastructureRegister _infraRegister;
        private IInfrastructureDisposer _infraDisposer;
        private IApplicationFactory _appFactory;

        private Dictionary<Type, IApplication> _persistentApp;

        public CoreFrameApplication(IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister, IInfrastructureDisposer infraDisposer, IApplicationFactory applicationFactory)
        {
            InjectInfrastructureProvider(infraProvider);
            InjectInfrastructureRegister(infraRegister);
            InjectInfrastructureDisposer(infraDisposer);

            InjectApplicationFactory(applicationFactory);

            InitializeInfraPersistence();
            InitializeAppPersistence();
        }
        private void InjectApplicationFactory(IApplicationFactory appFactory)
        {
            _appFactory = appFactory;
        }
        private void InitializeAppPersistence()
        {

        }
        private void InitializeInfraPersistence()
        {
            RequestRegisterLogger();
        }
        private void RequestRegisterLogger()
        {
            _infraRegister.RegisterInfrastructure<ILogEventHandler>();
        }
        private void InjectInfrastructureDisposer(IInfrastructureDisposer infraDisposer)
        {
            _infraDisposer = infraDisposer;
        }
        private void InjectInfrastructureRegister(IInfrastructureRegister infraRegister)
        {
            _infraRegister = infraRegister;
        }
        private void InjectInfrastructureProvider(IInfrastructureProvider infraProvider)
        {
            _infraProvider = infraProvider;
        }
        public bool TryGetApplication<T>(out T targetApplication) where T : class, IApplication
        {
            var type = typeof(T);
            if (_persistentApp.TryGetValue(type, out var persistent))
            {
                targetApplication = persistent as T;
                return targetApplication != null;
            }
            targetApplication = null;
            return true;
        }
        /*
         * DisposeÀÇ ¼ø¼­ 
         * [ Unity EntryPoint (MonoBehaviour or bootstrap) ]
                ¡é
        [ GameMainFrameApplication ]  ¡ç Owns
                ¡é
        [ UnityMainInfrastructure ]   ¡ç Owns
                ¡é
        [ UnityInputAdapter, FirebaseLoggerAdapter, etc. ]
         */

        protected override void DisposeManagedResources()
        {
            ClearInfraProvider();
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
