using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elder.Framework.Asset.Infra
{
    internal sealed class AddressableAssetLoader : IEngineAssetLoader, IEngineAssetReleaser
    {
        public async UniTask<AsyncOperationHandle<T>> LoadAsync<T>(string key)
            where T : UnityEngine.Object
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
            await handle.ToUniTask();
            return handle;
        }

        public async UniTask<AsyncOperationHandle<IList<T>>> LoadAllAsync<T>(string label) where T : UnityEngine.Object
        {
            var handle = Addressables.LoadAssetsAsync<T>(label, null);
            await handle.ToUniTask();
            return handle;
        }

        public void Release(AsyncOperationHandle handle)
        {
            if (handle.IsValid()) Addressables.Release(handle);
        }
    }
}