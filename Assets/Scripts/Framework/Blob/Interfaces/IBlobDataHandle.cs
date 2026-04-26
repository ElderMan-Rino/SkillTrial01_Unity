using Elder.Framework.Data.Interfaces;
using Unity.Entities;

namespace Elder.Framework.Blob.Interfaces
{
    public interface IBlobDataHandle<T> : IDataHandle<T> where T : unmanaged
    {
        public BlobAssetReference<T> GetRawReference();
    }
}
