using Elder.Framework.Data.Interfaces;
using System;
using Unity.Entities;

namespace Elder.Framework.Blob.Infra.Extensions
{
    public static class DataProviderBlobExtensions
    {
        public static BlobAssetReference<T> GetBlobReference<T>(this IDataProvider provider) where T : unmanaged
        {
            var handle = provider.GetData<T>();

            // 안전한 캐스팅 (타입 체크)
            if (handle is BlobDataHandle<T> blobHandle)
            {
                return blobHandle.Data;
            }

            // 만약 Blob 데이터가 아닌데 요청했다면 명확한 에러를 뱉어줍니다.
            if (handle == null)
                throw new Exception($"데이터를 찾을 수 없습니다: {typeof(T).Name}");
            else
                throw new InvalidOperationException($"데이터가 Blob 형식이 아닙니다. 현재 타입: {handle.GetType().Name}");
        }
    }
}