using Elder.Framework.Asset.Installers;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Core.UI.Installers;
using Elder.Framework.Singal.Installers;

namespace Elder.Framework.Core.Installers
{
    public static class CoreInstaller
    {
        public static void Install(IComponentRegistry registry)
        {
            SignalInstaller.Install(registry);
            AssetSystemInstaller.Install(registry);
            UISystemInstaller.Install(registry);
            
            // TODO: 추가 예정 (순서 고정)
            // 1. DataSystemInstaller.Install(registry);
            // 3. IndicatorInstaller.Install(registry);
            // 4. MessageBoxInstaller.Install(registry);
            // 5. SceneTransitionInstaller.Install(registry);
        }
    }
}