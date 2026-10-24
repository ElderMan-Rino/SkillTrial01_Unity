using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.App;

namespace Elder.Framework.UI.Interfaces
{
    public interface IUISystem : IFrameworkComponent
    {
        public bool TryFindUIView<T>(out T view) where T : UIViewBase;
        public TPresenter Create<TPresenter>() where TPresenter : IUIPresenter, new();
    }
}