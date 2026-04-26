using Elder.Framework.Data.Interfaces;
using System;
using Unity.Entities;
using Unity.Entities.Serialization;

namespace Elder.Framework.Blob.Infra
{
    internal sealed class BlobDataDeserializer : IDataDeserializer
    {
        public unsafe IDataHandle<T> Deserialize<T>(byte[] data) where T : unmanaged
        {
            if (data is null || data.Length == 0)
                throw new ArgumentException("Data is empty");

            return Deserialize<T>(data, data.Length);
        }

        // ArrayPool 버퍼 + 유효 길이를 받는 내부 오버로드 (EncryptedBlobDataDeserializer에서 사용)
        internal unsafe IDataHandle<T> Deserialize<T>(byte[] data, int length) where T : unmanaged
        {
            if (data is null || length == 0)
                throw new ArgumentException("Data is empty");

            fixed (byte* ptr = data)
            {
                var reader = new MemoryBinaryReader(ptr, length);
                BlobAssetReference<T> blobRef = reader.Read<T>();
                reader.Dispose();
                return new BlobDataHandle<T>(blobRef);
            }
        }
    }
}
