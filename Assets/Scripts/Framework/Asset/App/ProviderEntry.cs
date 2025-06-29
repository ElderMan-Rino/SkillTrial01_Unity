using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elder.Framework.Asset.App
{
    internal sealed class ProviderEntry
    {
        internal AsyncOperationHandle Handle;
        internal UnityEngine.Object Asset;
        internal int RefCount;
        // [HEAP] 키당 1회만 할당 — GetAssetAsync 반복 호출 시 재사용
        internal Action ReleaseAction;
    }
}
