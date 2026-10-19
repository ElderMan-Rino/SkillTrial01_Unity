using Elder.Framework.Asset.Installers;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Core.UI.Installers;
using Elder.Framework.Singal.Installers;
using Elder.TestModule.UI.Infra;

namespace Elder.TestModule.UI.Interfaces
{
    public static class UITestInstaller
    {
        public static void Install(IComponentRegistry registry)
        {
            SignalInstaller.Install(registry);
            UISystemInstaller.Install(registry);
            AssetSystemInstaller.Install(registry);

            registry.Regist<UITestCoordinator>().As<IUITestCoordinator>();
            registry.Regist<UITestFlowHandler>();
        }
    }
}