using Elder.Framework.Asset.Installers;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.Installers;
using Elder.Framework.Singal.Installers;
using Elder.TestModule.UITest.Infra;

namespace Elder.TestModule.UITest.Interfaces
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