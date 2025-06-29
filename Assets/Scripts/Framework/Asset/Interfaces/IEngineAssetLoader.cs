using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IEngineAssetLoader : IGameSystem
    {
        public UniTask<AsyncOperationHandle<T>> LoadAsync<T>(string key) where T : UnityEngine.Object;
        public UniTask<AsyncOperationHandle<IList<UnityEngine.Object>>> LoadAllAsync(string label);
    }
}