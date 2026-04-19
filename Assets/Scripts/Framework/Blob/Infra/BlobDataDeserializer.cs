using Elder.Framework.Data.Interfaces;
using System;
using Unity.Entities;
using Unity.Entities.Serialization;

namespace Elder.Framework.Blob.Infra
{
    public class BlobDataDeserializer : IDataDeserializer
    {
        public unsafe IDataHandle<T> Deserialize<T>(byte[] data) where T : unmanaged
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Data is empty");

            fixed (byte* ptr = data)
            {
                var reader = new MemoryBinaryReader(ptr, data.Length);
                BlobAssetReference<T> blobRef = reader.Read<T>();
                reader.Dispose();

                // 래퍼 클래스에 담아서 반환합니다.
                return new BlobDataHandle<T>(blobRef);
            }
        }
    }
}