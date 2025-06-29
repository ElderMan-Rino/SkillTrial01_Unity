using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elder.Framework.Asset.App
{
    internal class ProviderEntry
    {
        public AsyncOperationHandle Handle;
        public Object Asset;
        public int RefCount;
    }
}