using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core.UI.Interfaces;
using UnityEngine;

namespace Elder.Framework.Core.UI.Definitions
{
    // IUIViewFactory가 생성하는 기본 View 핸들. 인스턴스 Destroy와 Addressable 해제를 함께 책임진다.
    internal sealed class UIViewHandle : IUIViewHandle
    {
        private readonly IAssetHandle _assetHandle;
        private GameObject _instance;
        private IUIView _view;

        public IUIView View => _view;

        public UIViewHandle(GameObject instance, IUIView view, IAssetHandle assetHandle)
        {
            _instance = instance;
            _view = view;
            _assetHandle = assetHandle;
        }

        public void DestroyViewOnly()
        {
            if (_instance is null)
                return;

            Object.Destroy(_instance);
            _instance = null;
            _view = null;
        }

        public void Dispose()
        {
            DestroyViewOnly();
            _assetHandle?.Dispose();
        }
    }
}
