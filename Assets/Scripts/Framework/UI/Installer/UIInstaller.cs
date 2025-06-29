using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.App;
using Elder.Framework.UI.Interfaces;

namespace Elder.Framework.UI.Installer
{
    public readonly struct UIInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.Register<IUISystem, UISystem>();
        }
    }
}
