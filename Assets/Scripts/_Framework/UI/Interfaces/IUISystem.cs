using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.App;

namespace Elder.Framework.UI.Interfaces
{
    public interface IUISystem : IFrameworkComponent
    {
        public bool TryFindUIView<T>(out T view) where T : UIViewBase;
        public TPresenter CreatePresenter<TPresenter>() where TPresenter : IUIPresenter, new();
        public UniTask<IUIViewHandle> CreateViewByAssetKeyAsync<TView>(string assetKey) where TView : UIViewBase;

        // [TODO] 데이터 테이블 연동 전까지는 틀만 존재.
        public UniTask<IUIViewHandle> CreateViewByDataKeyAsync<TView>(int dataKey) where TView : UIViewBase;
        public IUIViewHandle CreateSceneViewHandle<TView>(TView view) where TView : UIViewBase;

        // rebindStrategy: Wake 시 Entry가 보관해 재바인딩을 위임. 바인딩 방식을 전략으로 분리해 모든 View가 동일한 Sleep/Wake 흐름을 타도록 UISystem 계층에서 일괄 처리.
        public IUIEntryHandle CreateEntry(int scope, IUIViewHandle viewHandle, UIPresenterBase presenter, IUIRebindStrategy rebindStrategy);
        public UniTask OpenGlobalAsync(IUIEntryHandle entry);
        public UniTask OpenSceneAsync(IUIEntryHandle entry);
        public UniTask ClearSceneScopeAsync(int scope);
        public UniTask ClearGlobalAsync();
    }
}