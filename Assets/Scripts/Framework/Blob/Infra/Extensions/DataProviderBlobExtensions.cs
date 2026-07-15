using Elder.Framework.Blob.Interfaces;
using System;
using Unity.Entities;

namespace Elder.Framework.Blob.Infra.Extensions
{
    public static class DataProviderBlobExtensions
    {
        public static BlobAssetReference<T> GetBlobReference<T>(this IDataProvider provider) where T : unmanaged
        {
            if (!provider.TryGetBlobReference<T>(out var reference))
                throw new InvalidOperationException($"데이터를 찾을 수 없습니다: {typeof(T).Name}");  // [HEAP] 예외 경로 문자열 보간

            return reference;
        }

        public static ref T GetBlobData<T>(this IDataProvider provider) where T : unmanaged
        {
            if (!provider.TryGetBlobReference<T>(out var reference))
                throw new InvalidOperationException($"데이터를 찾을 수 없습니다: {typeof(T).Name}");  // [HEAP] 예외 경로 문자열 보간

            return ref reference.Value;
        }
    }
}
