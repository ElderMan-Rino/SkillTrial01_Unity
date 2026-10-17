using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core.Definitions;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Core.UI.Definitions;
using Elder.Framework.Core.UI.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using System;
using UnityEngine;

namespace Elder.Framework.Core.UI.Infra
{
    internal sealed class UIViewFactory : IUIViewFactory, IDependencyInjectable
    {
        private ILoggerEx _logger = LogFacade.GetLogger<UIViewFactory>();
        private IAssetCache _assetCache;

        public Type ClassType => typeof(UIViewFactory);
        public int Scope => GlobalScope.Id;

        public void InjectDependency(IComponentProvider provider)
        {
            if (!provider.TryGetComponent<IAssetCache>(out _assetCache))
                _logger.Error($"Failed to resolve {nameof(IAssetCache)}."); // [HEAP]
        }

        public async UniTask<IUIViewHandle> CreateAsync<TView>(string assetKey) where TView : IUIView
        {
            var assetHandle = await _assetCache.GetAssetAsync<GameObject>(assetKey);
            var prefab = assetHandle.GetResult<GameObject>();
            if (prefab == null)
            {
                _logger.Error($"Failed to load prefab: {assetKey}."); // [HEAP]
                assetHandle.Dispose();
                return null;
            }

            var instance = GameObject.Instantiate(prefab);
            var view = instance.GetComponent<TView>();
            return new UIViewHandle(instance, view, assetHandle);
        }
    }
}
