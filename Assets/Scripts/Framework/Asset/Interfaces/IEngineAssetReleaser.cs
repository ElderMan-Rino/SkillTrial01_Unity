using Elder.Framework.Core.Interfaces;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elder.Framework.Asset.Interfaces
{
    public interface IEngineAssetReleaser : ISystemComponent
    {
        public void Release(AsyncOperationHandle handle);
    }
}