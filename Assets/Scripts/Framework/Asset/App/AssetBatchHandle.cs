using Elder.Framework.Asset.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Asset.App
{
    internal class AssetBatchHandle<T> : IAssetBatchHandle<T> where T : UnityEngine.Object
    {
        public IReadOnlyList<T> Assets { get; }

        private readonly Action _releaseAction;
        private bool _isDisposed;

        internal AssetBatchHandle(IReadOnlyList<T> assets, Action releaseAction)
        {
            Assets = assets;
            _releaseAction = releaseAction;
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _releaseAction?.Invoke();
            _isDisposed = true;
        }
    }
}