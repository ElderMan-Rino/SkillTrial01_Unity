using Elder.Framework.Core.Interfaces;
using Elder.TestModule.AssetLoad.Infra;
using Elder.TestModule.AssetLoad.Installers;
using Elder.TestModule.Common.Boot;
using System;

namespace Elder.TestModule.AssetLoad.Boot
{
    internal sealed class AssetLoadTestSceneContext : ModuleSceneContext
    {
        protected override Action<IComponentRegistry> GetInstallAction()
        {
            return AssetLaodTestInstaller.Install;
        }

        protected override Type GetStartCoordinatorType()
        {
            return typeof(AssetLoadTestCoordinator);
        }
    }
}
