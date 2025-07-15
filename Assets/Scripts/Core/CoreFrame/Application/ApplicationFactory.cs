using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.Logging.Application;
using Elder.Core.Logging.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Core.CoreFrame.Application
{
    public class ApplicationFactory : DisposableBase, IApplicationFactory
    {
        private Dictionary<Type, Func<IApplication>> _constructers;
        
        public ApplicationFactory()
        {
            InitializeConstructers();
        }
        private void InitializeConstructers()
        {
            _constructers = new()
            {
                { typeof(ILoggerPublisher), () => new LogService() },
            };
        }
        public bool TryCreateApplication(Type type, IApplicationProvider appProvider, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister, out IApplication application)
        {
            if (!_constructers.TryGetValue(type, out var constructer))
            {
                 application = null;
                return false;
            }
            application = constructer.Invoke();
            application.TryInitialize(appProvider, infraProvider, infraRegister);
            return true;
        }

        protected override void DisposeManagedResources()
        {
            DisposeConstructers();
        }
        private void DisposeConstructers()
        {
            _constructers.Clear();
            _constructers = null;
        }

        protected override void DisposeUnmanagedResources()
        {

        }
    }
}