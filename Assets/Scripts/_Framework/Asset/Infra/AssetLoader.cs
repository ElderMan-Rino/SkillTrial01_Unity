using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core.Definitions;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Elder.Framework.Asset.Infra
{
    public sealed class AssetLoader : IAssetLoadService
    {
        public Type ClassType => typeof(AssetLoader);

        public int Scope => GlobalScope.Id;

        public async UniTask EnsureInitializedAsync()
        {
            await Addressables.InitializeAsync().Task;
        }

        public async UniTask<AsyncOperationHandle<T>> LoadAssetAsync<T>(string key) where T : UnityEngine.Object
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
            await handle.Task;
            return handle;
        }

        public async UniTask<AsyncOperationHandle<IList<UnityEngine.Object>>> LoadAssetsAsync(string label)
        {
            var handle = Addressables.LoadAssetsAsync<UnityEngine.Object>(label);
            await handle.Task;
            return handle;
        }

        public void Release(AsyncOperationHandle targetHandle)
        {
            Addressables.Release(targetHandle);
        }

        public async UniTask<bool> IsLoadableAsync<T>(string key) where T : UnityEngine.Object
        {
            var handle = Addressables.LoadResourceLocationsAsync(key, typeof(T));
            var locations = await handle.Task;
            var isLoadable = locations.Count > 0;
            Addressables.Release(handle);
            return isLoadable;
        }

        public async UniTask<bool> IsSceneLoadableAsync(string key)
        {
            var handle = Addressables.LoadResourceLocationsAsync(key, typeof(SceneInstance));
            var locations = await handle.Task;
            var isLoadable = locations.Count > 0;
            Addressables.Release(handle);
            return isLoadable;
        }

        public IReadOnlyCollection<string> GetAllKeys()
        {
            var keys = new HashSet<string>(); // [HEAP]
            foreach (var locator in Addressables.ResourceLocators)
            {
                foreach (var key in locator.Keys)
                {
                    if (key is string stringKey)
                    {
                        keys.Add(stringKey);
                    }
                }
            }

            return keys;
        }
    }
}