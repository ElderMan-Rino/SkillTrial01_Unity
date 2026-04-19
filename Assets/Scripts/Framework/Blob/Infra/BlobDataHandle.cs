using Elder.Framework.Data.Interfaces;
using System;
using Unity.Entities;

namespace Elder.Framework.Blob.Infra
{
    public class BlobDataHandle<T> : IDataHandle<T> where T : unmanaged
    {
        public readonly BlobAssetReference<T> Data;

        public BlobDataHandle(BlobAssetReference<T> data)
        {
            Data = data;
        }

        // 래퍼가 파괴될 때 안전하게 Unmanaged 메모리를 해제합니다.
        public void Dispose()
        {
            if (Data.IsCreated)
            {
                Data.Dispose();
            }
        }
    }
}