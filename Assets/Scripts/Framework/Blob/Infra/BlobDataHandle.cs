using Elder.Framework.Blob.Interfaces;
using Elder.Framework.Data.Interfaces;
using Unity.Entities;

namespace Elder.Framework.Blob.Infra
{
    internal sealed class BlobDataHandle<T> : IBlobDataHandle<T> where T : unmanaged
    {
        private readonly BlobAssetReference<T> _data;
        private bool _isDisposed;

        public BlobDataHandle(BlobAssetReference<T> data)
        {
            _data = data;
        }

        public bool IsCreated => _data.IsCreated;

        public bool TryGetData(out T data)
        {
            if (_data.IsCreated)
            {
                // BlobArray/BlobString 포함 타입은 복사 시 내부 오프셋 무효화 — GetRef() 사용
                data = _data.Value;
                return true;
            }
            data = default;
            return false;
        }

        public BlobAssetReference<T> GetRawReference() => _data;

        public ref T GetRef() => ref _data.Value;

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            if (_data.IsCreated) _data.Dispose();
        }
    }
}