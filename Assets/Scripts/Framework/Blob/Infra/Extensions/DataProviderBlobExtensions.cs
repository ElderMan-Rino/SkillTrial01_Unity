using Elder.Framework.Blob.Interfaces;
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

            if (handle is null)
                throw new InvalidOperationException($"데이터를 찾을 수 없습니다: {typeof(T).Name}");  // [HEAP] 예외 경로 문자열 보간

            if (handle is IBlobDataHandle<T> blobHandle)
                return blobHandle.GetRawReference();

            throw new InvalidOperationException(
                $"핸들 타입이 IBlobDataHandle<{typeof(T).Name}>을 구현하지 않습니다. 실제 타입: {handle.GetType().Name}");  // [HEAP] 예외 경로 문자열 보간
        }

        public static ref T GetBlobData<T>(this IDataProvider provider) where T : unmanaged
        {
            var handle = provider.GetData<T>();

            if (handle is null)
                throw new InvalidOperationException($"데이터를 찾을 수 없습니다: {typeof(T).Name}");  // [HEAP] 예외 경로 문자열 보간

            if (handle is IBlobDataHandle<T> blobHandle)
                return ref blobHandle.GetRef();

            throw new InvalidOperationException(
                $"핸들 타입이 IBlobDataHandle<{typeof(T).Name}>을 구현하지 않습니다. 실제 타입: {handle.GetType().Name}");  // [HEAP] 예외 경로 문자열 보간
        }
    }
}