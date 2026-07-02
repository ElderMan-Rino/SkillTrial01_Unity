using Elder.Framework.Asset.Interfaces;
using Elder.Framework.UI.Interfaces;
using UnityEngine;

namespace Elder.Framework.UI.App
{
    internal sealed class UIHandle
    {
        internal GameObject Instance { get; }
        internal IAssetHandle<GameObject> AssetHandle { get; }
        internal IUIPresenterLifecycle Lifecycle { get; private set; }
        private readonly bool _isSceneBound;

        internal UIHandle(GameObject instance, IAssetHandle<GameObject> assetHandle, IUIPresenterLifecycle lifecycle = null)
        {
            Instance = instance;
            AssetHandle = assetHandle;
            Lifecycle = lifecycle;
            _isSceneBound = false;
        }

        internal UIHandle(GameObject instance, IUIPresenterLifecycle lifecycle = null)
        {
            Instance = instance;
            AssetHandle = null;
            Lifecycle = lifecycle;
            _isSceneBound = true;
        }

        internal void SetLifecycle(IUIPresenterLifecycle lifecycle) => Lifecycle = lifecycle;

        internal void Dispose()
        {
            if (!_isSceneBound && Instance is not null) Object.Destroy(Instance);
            AssetHandle?.Dispose();
        }
    }
}
