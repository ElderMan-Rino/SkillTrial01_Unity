using Elder.Framework.Core.Interfaces;
using Unity.Entities;

namespace Elder.Framework.Blob.Interfaces
{
    public interface IDataProvider : IGameSystem
    {
        public bool TryGetBlobReference<T>(out BlobAssetReference<T> reference) where T : unmanaged;
    }
}
