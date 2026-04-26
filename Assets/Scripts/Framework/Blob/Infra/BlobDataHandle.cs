using Elder.Framework.Blob.Interfaces;
using Elder.Framework.Data.Interfaces;
using Unity.Entities;

namespace Elder.Framework.Blob.Infra
{
    internal sealed class BlobDataHandle<T> : IBlobDataHandle<T> where T : unmanaged
    {
        private readonly BlobAssetReference<T> _data;

        public BlobDataHandle(BlobAssetReference<T> data)
        {
            _data = data;
        }

        public bool TryGetData(out T data)
        {
            if (_data.IsCreated)
            {
                data = _data.Value;
                return true;
            }
            data = default;
            return false;
        }

        public BlobAssetReference<T> GetRawReference() => _data;

        public void Dispose()
        {
            if (_data.IsCreated)
                _data.Dispose();
        }
    }
}