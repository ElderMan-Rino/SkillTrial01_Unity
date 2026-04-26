using System;

namespace Elder.Framework.Crypto.Interfaces
{
    public interface IEncryptionProvider : IDisposable
    {
        // 암호화된 바이트를 destination에 쓰고 실제 쓴 길이 반환
        public int Encrypt(ReadOnlySpan<byte> plaintext, Span<byte> destination);

        // 복호화된 바이트를 destination에 쓰고 실제 쓴 길이 반환
        public int Decrypt(ReadOnlySpan<byte> ciphertext, Span<byte> destination);

        // 암호화 후 최대 크기 (버퍼 예약용)
        public int GetEncryptedSize(int plaintextLength);

        // 복호화 후 최대 크기 (PKCS7 패딩 제거 전 상한, 버퍼 예약용)
        public int GetDecryptedMaxSize(int ciphertextLength);
    }
}
