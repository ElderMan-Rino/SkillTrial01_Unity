using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.App;
using Elder.Framework.UI.Definitions;
using Elder.Framework.UI.Interfaces;

namespace Elder.Framework.UI.Installer
{
    public readonly struct UISystemInstaller
    {
        private readonly UIStackOptions _options;

        public UISystemInstaller() : this(UIStackOptions.Default) { }

        public UISystemInstaller(UIStackOptions options)
        {
            _options = options;
        }

        public void Install(IGameSystemRegistry registry)
        {
            // [HEAP] 각 인스턴스 생성 — 기동 시 1회
            registry.RegisterInstance<UISystem>(new UISystem(_options)).As<IUISystem>();
            registry.Register<UIViewFactory>().As<IUIViewFactory>();
            registry.Register<UIPresenterFactory>().As<IUIPresenterFactory>().As<IUIPresenterRegistry>();
            registry.Register<UIViewModelFactory>().As<IUIViewModelFactory>().As<IUIViewModelRegistry>();
        }
    }
}
