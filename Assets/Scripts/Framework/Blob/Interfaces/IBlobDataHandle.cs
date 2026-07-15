using System;
using Unity.Entities;

namespace Elder.Framework.Blob.Interfaces
{
    public interface IBlobDataHandle<T> : IDisposable where T : unmanaged
    {
        public bool IsCreated { get; }
        public bool TryGetData(out T data);
        public BlobAssetReference<T> GetRawReference();
        public ref T GetRef();
    }
}
