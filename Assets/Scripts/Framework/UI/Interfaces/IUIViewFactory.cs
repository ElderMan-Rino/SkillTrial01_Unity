using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.App;

namespace Elder.Framework.UI.Interfaces
{
    public interface IUIViewFactory : ISystemComponent
    {
        public UniTask<TView> CreateAsync<TView>(string addressableKey) where TView : UIViewBase;
        public UniTask<TView> CreateAsync<TView>() where TView : UIViewBase;
        public void Release<TView>() where TView : UIViewBase;
    }
}
