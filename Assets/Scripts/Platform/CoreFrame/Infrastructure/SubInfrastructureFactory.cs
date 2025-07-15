using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Platform.Logging.Infrastructure;
using Elder.Platform.Logging.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Platform.CoreFrame.Infrastructure.Factories
{
    public class SubInfrastructureFactory : DisposableBase, ISubInfrastructureFactory
    {
        private Dictionary<Type, Func<ISubInfrastructure>> _constructers;

        public SubInfrastructureFactory()
        {
            InitializeConstructers();
        }
        public void InitializeConstructers()
        {
            _constructers = new()
            {
                { typeof(IUnityLogAdapter), () => new UnityLogAdapter() },
            };
        }
        public bool TryCreateSubInfra(Type type, out ISubInfrastructure subInfra)
        {
            subInfra = null;
            if (!_constructers.TryGetValue(type, out var constructer))
                return false;

            subInfra = constructer.Invoke();
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
