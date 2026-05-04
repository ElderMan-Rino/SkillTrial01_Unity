using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elder.Framework.Asset.App
{
    internal sealed class ProviderEntry
    {
        public AsyncOperationHandle Handle;
        public UnityEngine.Object Asset;
        public int RefCount;
        // [HEAP] 키당 1회만 할당 — GetAssetAsync 반복 호출 시 재사용
        public Action ReleaseAction;
    }
}
