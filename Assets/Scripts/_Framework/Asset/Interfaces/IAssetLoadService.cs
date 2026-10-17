using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IAssetLoadService : IFrameworkComponent
    {
        public UniTask EnsureInitializedAsync();
        public UniTask<AsyncOperationHandle<T>> LoadAssetAsync<T>(string key) where T : UnityEngine.Object;
        public UniTask<AsyncOperationHandle<IList<UnityEngine.Object>>> LoadAssetsAsync(string label);
        public void Release(AsyncOperationHandle handle);
        public IReadOnlyCollection<string> GetAllKeys();
        public UniTask<bool> IsLoadableAsync<T>(string key) where T : UnityEngine.Object;
        public UniTask<bool> IsSceneLoadableAsync(string key);
    }
}