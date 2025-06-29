using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elder.Framework.Asset.Infra
{
    internal class AddressableResourceLoader : IEngineAssetLoader, IEngineAssetReleaser
    {
        public async UniTask<AsyncOperationHandle<T>> LoadAsync<T>(string key)
            where T : UnityEngine.Object
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
            await handle.ToUniTask();
            return handle;
        }

        public async UniTask<AsyncOperationHandle<IList<UnityEngine.Object>>> LoadAllAsync(string label)
        {
            var handle = Addressables.LoadAssetsAsync<UnityEngine.Object>(label, null);
            await handle.ToUniTask();
            return handle;
        }

        public void Release(AsyncOperationHandle handle)
        {
            if (handle.IsValid()) Addressables.Release(handle);
        }
    }
}