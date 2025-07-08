using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.Logging.Interfaces;
using Elder.Unity.Logging.Infrastructure;
using System;
using System.Collections.Generic;

namespace Elder.Unity.CoreFrame.Infrastructure.Factories
{
    public class InfrastructureFactory : DisposableBase, IInfrastructureFactory
    {
        private Dictionary<Type, Func<IInfrastructure>> _constructers;

        public void Initialize()
        {
            _constructers = new()
            {
                { typeof(ILogEventHandler), () => new LoggingInfrastructure() },
            };
        }
        public bool TryCreateInfrastructure(Type type, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister, out IInfrastructure infrastructure)
        {
            infrastructure = null;
            if (!_constructers.TryGetValue(type, out var constructer))
                return false;

            infrastructure = constructer.Invoke();
            infrastructure.Initialize(infraProvider, infraRegister);
            return true;
        }

        protected override void DisposeManagedResources()
        {

        }

        protected override void DisposeUnmanagedResources()
        {

        }

       
    }
}