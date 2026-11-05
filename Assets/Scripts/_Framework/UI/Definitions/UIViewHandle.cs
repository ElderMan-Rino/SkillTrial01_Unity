using Elder.Framework.Asset.Interfaces;
using Elder.Framework.UI.Interfaces;
using System;
using UnityEngine;

namespace Elder.Framework.UI.Definitions
{
    // IUIViewFactory가 생성하는 기본 View 핸들. 인스턴스 Destroy와 Addressable 해제를 함께 책임진다.
    internal sealed class UIViewHandle : IUIViewHandle
    {
        private readonly IAssetHandle _assetHandle;
        private readonly Type _viewType;
        private GameObject _instance;
        private IUIView _view;

        public IUIView View => _view;

        public Type ViewType => _viewType;

        public UIViewHandle(GameObject instance, IUIView view, IAssetHandle assetHandle, Type viewType)
        {
            _instance = instance;
            _view = view;
            _assetHandle = assetHandle;
            _viewType = viewType;
        }

        public void DestroyViewOnly()
        {
            if (_instance is null)
                return;

            _view.UnbindRoot();
            UnityEngine.Object.Destroy(_instance);
            _instance = null;
            _view = null;
        }

        public void Recreate()
        {
            var prefab = _assetHandle.GetResult<GameObject>();
            _instance = UnityEngine.Object.Instantiate(prefab);
            _view = (IUIView)(object)_instance.GetComponent(_viewType); // [AOT RISK] GetComponent(Type)은 리플렉션 아님, IL2CPP 코드젠 대상
        }

        public void Dispose()
        {
            DestroyViewOnly();
            _assetHandle?.Dispose();
        }
    }
}
