using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IEngineAssetLoader : ISystemComponent
    {
        public UniTask<AsyncOperationHandle<T>> LoadAsync<T>(string key) where T : UnityEngine.Object;
        public UniTask<AsyncOperationHandle<IList<T>>> LoadAllAsync<T>(string label) where T : UnityEngine.Object;
    }
}