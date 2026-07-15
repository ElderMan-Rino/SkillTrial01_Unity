using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.Crypto.Interfaces;
using Elder.Framework.Blob.Interfaces;
using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Elder.Framework.Blob.Infra
{
    // 복호화 후 BlobDataDeserializer에 위임하는 데코레이터.
    // 복호화 버퍼: ArrayPool 대여 + GCHandle.Pinned + ZeroMemory.
    // 복호화 완료 즉시 BlobAssetReference(비관리 메모리)로 이전하여 관리 힙 잔류 최소화.
    internal sealed class EncryptedBlobDataDeserializer : BaseSystem, IDataDeserializer
    {
        private IEncryptionProvider _encryption;
        private IRawDataDeserializer _inner;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<IEncryptionProvider>(out _encryption))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IEncryptionProvider)}");
            if (!TryGetSystem<IRawDataDeserializer>(out _inner))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IRawDataDeserializer)}");
        }

        public IBlobDataHandle<T> Deserialize<T>(byte[] data) where T : unmanaged
        {
            if (data is null || data.Length == 0)
                throw new ArgumentException("Data is empty");

            // PKCS7 패딩 제거 전 상한으로 버퍼 예약
            int maxDecryptedSize = _encryption.GetDecryptedMaxSize(data.Length);

            // [HEAP] ArrayPool 대여 — new byte[] 대신 풀 재사용
            byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent(maxDecryptedSize);
            // GC Compaction으로 버퍼 주소가 바뀌는 것을 방지
            var pinHandle = GCHandle.Alloc(rentedBuffer, GCHandleType.Pinned);

            try
            {
                // actualLength: PKCS7 패딩 제거 후 실제 평문 길이
                int actualLength = _encryption.Decrypt(data, rentedBuffer.AsSpan(0, maxDecryptedSize));

                // 복호화 즉시 비관리 메모리(BlobAsset)로 이전
                return _inner.Deserialize<T>(rentedBuffer, actualLength);
            }
            finally
            {
                // 복호화 데이터 소거 후 풀 반환
                CryptographicOperations.ZeroMemory(rentedBuffer.AsSpan(0, maxDecryptedSize));
                pinHandle.Free();
                ArrayPool<byte>.Shared.Return(rentedBuffer);
            }
        }

      

        public override UniTask InitializeAsync() => UniTask.CompletedTask;

        public override UniTask PostInitializeAsync() => UniTask.CompletedTask;
    }
}
