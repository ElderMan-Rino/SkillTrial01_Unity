using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.GameLevel.Interfaces;
using Elder.Core.Logging.Interfaces;
using Elder.Platform.GameLevel.Infrastructure;
using Elder.Platform.Logging.Infrastructure;
using System;
using System.Collections.Generic;

namespace Elder.Platform.CoreFrame.Infrastructure.Factories
{
    public class InfrastructureFactory : DisposableBase, IInfrastructureFactory
    {
        private Dictionary<Type, Func<IInfrastructure>> _constructers;

        public InfrastructureFactory()
        {
            InitializeConstructers();
        }

        private void InitializeConstructers()
        {
            _constructers = new()
            {
                { typeof(ILogEventDispatcher), () => new LoggingInfrastructure() },
                { typeof(IGameLevelExecutor), () => new SceneLoader() },
            };
        }
        public bool TryCreateInfrastructure(Type type, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister, ISubInfrastructureCreator subInfraCreator, IApplicationProvider appProvider, out IInfrastructure infrastructure)
        {
            infrastructure = null;
            if (!_constructers.TryGetValue(type, out var constructer))
                return false;

            infrastructure = constructer.Invoke();
            return infrastructure.TryInitialize(infraProvider, infraRegister, subInfraCreator, appProvider);
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