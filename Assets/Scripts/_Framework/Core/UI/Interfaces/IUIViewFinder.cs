using Elder.Framework.Core.Interfaces;
using Elder.Framework.Core.UI.App;

namespace Elder.Framework.Core.UI.Interfaces
{
    public interface IUIViewFinder : IFrameworkComponent
    {
        public bool TryFindUIView<T>(out T view) where T : UIViewBase;
    }
}