using Elder.Framework.Core.Interfaces;
using Elder.TestModule.Common.Boot;
using Elder.TestModule.UITest.Infra;
using Elder.TestModule.UITest.Interfaces;
using System;

namespace Elder.TestModule.UITest.Boot
{
    internal sealed class UITestContext : ModuleSceneContext
    {
        protected override Action<IComponentRegistry> GetInstallAction()
        {
            return UITestInstaller.Install;
        }

        protected override Type GetStartCoordinatorType()
        {
            return typeof(UITestCoordinator);
        }
    }
}