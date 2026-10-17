using Elder.Framework.Asset.Interfaces;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elder.Framework.Asset.Infra
{
    public sealed class AssetHandle<T> : IAssetHandle where T : UnityEngine.Object
    {
        private readonly AsyncOperationHandle<T> _handle;
        private readonly IAssetLoadService _loader;
        private readonly string _key;
        private readonly Func<string, bool> _releaseRef;

        public T Result => _handle.Result;

        public AssetHandle(in AsyncOperationHandle<T> handle, IAssetLoadService loader, string key, Func<string, bool> releaseRef)
        {
            _handle = handle;
            _loader = loader;
            _key = key;
            _releaseRef = releaseRef;
        }

        public TResult GetResult<TResult>() where TResult : UnityEngine.Object
        {
            return _handle.Result as TResult;
        }

        public void Dispose()
        {
            if (_releaseRef(_key))
                _loader.Release(_handle);
        }
    }
}
