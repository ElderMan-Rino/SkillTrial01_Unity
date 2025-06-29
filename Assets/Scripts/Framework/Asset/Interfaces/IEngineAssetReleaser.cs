using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IEngineAssetReleaser
    {
        public void Release(AsyncOperationHandle handle);
    }
}