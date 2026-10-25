using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.App;

namespace Elder.Framework.UI.Interfaces
{
    public interface IUISystem : IFrameworkComponent
    {
        public bool TryFindUIView<T>(out T view) where T : UIViewBase;
        public TPresenter CreatePresenter<TPresenter>() where TPresenter : IUIPresenter, new();
        public UniTask<TView> CreateViewAsync<TView>() where TView : UIViewBase;
        public void RegisterEntry(IUIEntryHandle entry);
        public UniTask OpenAsync(IUIEntryHandle entry);
    }
}