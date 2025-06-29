using Elder.Framework.Blob.Interfaces;
using System.Collections.Generic;
using Unity.Entities;

namespace Elder.Framework.Blob.App
{
    internal class BlobList<T> : List<BlobAssetReference<T>>, IBlobList where T : unmanaged
    {
        public void Dispose()
        {
            foreach (var blob in this)
            {
                if (blob.IsCreated) blob.Dispose(); // unmanaged 메모리 안전 해제
            }
            Clear();
        }
    }
}