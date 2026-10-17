using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.TestModule.AssetLoad.Infra;
using Elder.TestModule.Common.Definitions;
using Elder.TestModule.UI.Interfaces;
using System;
using UnityEngine;

namespace Elder.TestModule.UI.Infra
{

    public class UITestCoordinator : IUITestCoordinator, IDependencyInjectable, IPreDisposable, IDisposable
    {
        private ILoggerEx _logger = LogFacade.GetLogger<UITestCoordinator>();

        public Type ClassType => typeof(UITestCoordinator);

        public int Scope => TestScope.Id;

    

        public void InjectDependency(IComponentProvider provider)
        {
            
        }

        public void PreDispose()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}