using Elder.Framework.Core.Interfaces;
using Elder.TestModule.Common.Boot;
using Elder.TestModule.UI.Infra;
using Elder.TestModule.UI.Interfaces;
using System;

namespace Elder.TestModule.UI.Boot
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