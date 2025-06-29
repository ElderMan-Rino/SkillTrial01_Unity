using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elder.Framework.Asset.App
{
    internal class LabelEntry
    {
        public AsyncOperationHandle Handle;

        private readonly Dictionary<AssetKey, UnityEngine.Object> _assets = new();

        public void AddAsset(Type type, string assetName, UnityEngine.Object asset)
        {
            var key = new AssetKey(type, assetName);
            _assets.TryAdd(key, asset);
        }

        public bool TryGetAsset(Type type, string assetName, out UnityEngine.Object asset)
        {
            var key = new AssetKey(type, assetName);
            return _assets.TryGetValue(key, out asset);
        }
    }
}