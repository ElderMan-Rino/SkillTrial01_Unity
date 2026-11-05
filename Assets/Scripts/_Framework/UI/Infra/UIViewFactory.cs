using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core.Definitions;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.Definitions;
using Elder.Framework.UI.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using System;
using UnityEngine;

namespace Elder.Framework.UI.Infra
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

        public async UniTask<IUIViewHandle> CreateByAssetKeyAsync<TView>(string assetKey) where TView : IUIView
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
            var viewType = typeof(TView);
            var view = (IUIView)(object)instance.GetComponent(viewType); // [AOT RISK] GetComponent(Type)은 리플렉션 아님, IL2CPP 코드젠 대상
            return new UIViewHandle(instance, view, assetHandle, viewType);
        }

        // [TODO] 데이터 테이블(Id -> assetKey 매핑) 연동 전까지는 틀만 존재.
        public UniTask<IUIViewHandle> CreateByDataKeyAsync<TView>(int dataKey) where TView : IUIView
        {
            throw new NotImplementedException($"Data table lookup for {nameof(dataKey)} {dataKey} is not implemented yet.");
        }
    }
}
