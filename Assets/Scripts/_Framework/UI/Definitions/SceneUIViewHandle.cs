using Elder.Framework.UI.Interfaces;
using System;
using UnityEngine;

namespace Elder.Framework.UI.Definitions
{
    // 씬에 배치된 View용 핸들. Addressable 자산이 없으므로 Sleep/Wake를 Instantiate/Destroy 대신 SetActive로 수행한다.
    internal sealed class SceneUIViewHandle : IUIViewHandle
    {
        private readonly GameObject _instance;
        private readonly IUIView _viewRef;
        private readonly Type _viewType;
        private IUIView _view;

        public IUIView View => _view;

        public Type ViewType => _viewType;

        public SceneUIViewHandle(GameObject instance, IUIView view)
        {
            _instance = instance;
            _viewRef = view;
            _view = view;
            _viewType = view.GetType();
        }

        public void DestroyViewOnly()
        {
            _instance.SetActive(false);
            _view = null;
        }

        public void Recreate()
        {
            _instance.SetActive(true);
            _view = _viewRef;
        }

        public void Dispose()
        {
            _instance.SetActive(false);
            _view = null;
        }
    }
}
