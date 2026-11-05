using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.UI.Interfaces
{
    // Addressable 프리팹을 로드해 TView 인스턴스를 생성하는 계약.
    internal interface IUIViewFactory : IFrameworkComponent
    {
        public UniTask<IUIViewHandle> CreateByAssetKeyAsync<TView>(string assetKey) where TView : IUIView;

        // [TODO] 데이터 테이블 연동 전까지는 틀만 존재 — dataKey로 asset 경로를 resolve하는 로직 미구현.
        public UniTask<IUIViewHandle> CreateByDataKeyAsync<TView>(int dataKey) where TView : IUIView;
    }
}
