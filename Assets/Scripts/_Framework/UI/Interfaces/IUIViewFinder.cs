using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.App;

namespace Elder.Framework.UI.Interfaces
{
    internal interface IUIViewFinder : IFrameworkComponent
    {
        public bool TryFindUIView<T>(out T view) where T : UIViewBase;
    }
}