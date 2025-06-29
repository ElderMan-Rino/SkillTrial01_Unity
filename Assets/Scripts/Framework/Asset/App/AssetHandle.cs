using Elder.Framework.Asset.Interfaces;
using System;

namespace Elder.Framework.Asset.App
{
    internal class AssetHandle<T> : IAssetHandle<T> where T : UnityEngine.Object
    {
        public T Asset { get; }

        private readonly Action _releaseAction;
        private bool _isDisposed;

        internal AssetHandle(T asset, Action releaseAction)
        {
            Asset = asset;
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