using Elder.Framework.Core.Interfaces;
using Elder.Framework.Core.UI.Infra;
using Elder.Framework.Core.UI.Interfaces;

namespace Elder.Framework.Core.UI.Installers
{
    public static class UISystemInstaller
    {
        public static void Install(IComponentRegistry registry)
        {
            registry.Regist<UIViewFactory>().As<IUIViewFactory>();
            registry.Regist<UIPresenterFactory>().As<IUIPresenterFactory>();
            registry.Regist<UIViewFinder>().As<IUIViewFinder>();
            registry.Regist<UISystem>().As<IUISystem>();
        }
    }
}
