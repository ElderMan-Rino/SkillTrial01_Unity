using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.App;

namespace Elder.Framework.UI.Interfaces
{
    public interface IUISystem : ISystemComponent
    {
        public UniTask OnSceneLoadedAsync();
        public UniTask PrepareAsync<TView>() where TView : UIViewBase;
        public UniTask<TView> ShowAsync<TView>(string addressableKey = null) where TView : UIViewBase;
        public UniTask HideAsync<TView>() where TView : UIViewBase;
        public UniTask ReleaseAsync<TView>() where TView : UIViewBase;
        public bool TryGetView<TView>(out TView view) where TView : UIViewBase;
        public UniTask PopAsync();
        public UniTask ClearStackAsync();
        public int StackCount { get; }
    }
}
