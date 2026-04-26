using Elder.Framework.Crypto.Infra.KeyDerivation;
using Elder.Framework.Crypto.Interfaces;
using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Elder.Framework.Crypto.Infra
{
    // AES-256-CBC. 파일 포맷: [IV 16B][PKCS7 암호화 데이터]
    // 키 파생: 생성자에서 HKDF 1회 수행 → GCHandle.Pinned 고정 캐싱
    // 복호화 버퍼: ArrayPool 대여 + GCHandle.Pinned(GC 이동 방지) + ZeroMemory
    // 보안 트레이드오프: 파생 키가 앱 생존 기간 동안 메모리 상주.
    //   GCHandle.Pinned으로 GC 이동 차단, Dispose에서 ZeroMemory 보장.
    //   1~2단계 공격 저항 수준 유지.
    internal sealed class AesEncryptionProvider : IEncryptionProvider
    {
        private const int IvSize = 16;
        private const int KeySize = 32;
        private const int BlockSize = 16;

        // KeyPartA: 바이너리에 분산된 상수 조각. 평문 키 문자열 없음.
        private static readonly byte[] _keyPartA =
        {
            0x4B, 0x7F, 0x2A, 0xD1, 0x9E, 0x53, 0xC8, 0x06,
            0xB3, 0x71, 0xEA, 0x2F, 0x5D, 0x94, 0x18, 0x3C,
            0xA7, 0x60, 0xF5, 0x1B, 0x8E, 0x47, 0xD9, 0x0A,
            0x6C, 0xB8, 0x35, 0xE2, 0x79, 0xC4, 0x51, 0x9F
        };

        // HKDF 도메인 분리 라벨 ("ElderBlob" UTF-8)
        private static readonly byte[] _hkdfInfo =
        {
            0x45, 0x6C, 0x64, 0x65, 0x72, 0x42, 0x6C, 0x6F, 0x62
        };

        // [HEAP] 파생 키 캐시 — 초기화 1회 할당, GCHandle.Pinned으로 GC 이동 방지
        private readonly byte[] _derivedKey;
        private readonly GCHandle _derivedKeyPin;

        public AesEncryptionProvider(byte[] keyPartB)
        {
            // [HEAP] 파생 키 버퍼 — 생성자에서 1회만 할당
            _derivedKey = new byte[KeySize];
            _derivedKeyPin = GCHandle.Alloc(_derivedKey, GCHandleType.Pinned);

            // HKDF 키 파생 1회 수행 후 캐시
            CryptoKeyAssembler.DeriveKey(_keyPartA, keyPartB, _hkdfInfo, _derivedKey);

            // keyPartB는 파생 완료 후 즉시 소거 (외부에서 전달된 배열도 소거)
            CryptographicOperations.ZeroMemory(keyPartB);
        }

        public void Dispose()
        {
            // 파생 키 소거 후 핀 해제
            CryptographicOperations.ZeroMemory(_derivedKey);
            if (_derivedKeyPin.IsAllocated) _derivedKeyPin.Free();
        }

        public int GetEncryptedSize(int plaintextLength)
        {
            int paddedLen = (plaintextLength / BlockSize + 1) * BlockSize;
            return IvSize + paddedLen;
        }

        public int GetDecryptedMaxSize(int ciphertextLength)
        {
            return ciphertextLength - IvSize;
        }

        public int Encrypt(ReadOnlySpan<byte> plaintext, Span<byte> destination)
        {
            using var aes = CreateAes(out byte[] ivBuffer);

            ivBuffer.CopyTo(destination);

            // [HEAP] ToArray 2회 — .NET Standard 2.1 Aes API 제약. 에디터(굽기)에서만 호출.
            aes.Key = _derivedKey;
            using var encryptor = aes.CreateEncryptor();
            // [HEAP] TransformFinalBlock — 프레임워크 제약
            byte[] encrypted = encryptor.TransformFinalBlock(plaintext.ToArray(), 0, plaintext.Length);

            encrypted.CopyTo(destination[IvSize..]);
            CryptographicOperations.ZeroMemory(encrypted);
            return IvSize + encrypted.Length;
        }

        public int Decrypt(ReadOnlySpan<byte> ciphertext, Span<byte> destination)
        {
            if (ciphertext.Length <= IvSize)
                throw new ArgumentException("Ciphertext too short");

            int encryptedBodyLen = ciphertext.Length - IvSize;

            // [HEAP] ArrayPool 대여 — new byte[] 대신 풀 재사용
            byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent(encryptedBodyLen);
            var pinHandle = GCHandle.Alloc(rentedBuffer, GCHandleType.Pinned);

            try
            {
                using var aes = CreateAes(ciphertext[..IvSize]);
                aes.Key = _derivedKey;

                using var decryptor = aes.CreateDecryptor();
                // [HEAP] TransformFinalBlock — 프레임워크 제약. PKCS7 패딩 제거 후 실제 평문 반환.
                byte[] decrypted = decryptor.TransformFinalBlock(
                    ciphertext[IvSize..].ToArray(), 0, encryptedBodyLen);

                decrypted.CopyTo(destination);
                int actualLength = decrypted.Length;

                CryptographicOperations.ZeroMemory(decrypted);
                return actualLength;
            }
            finally
            {
                CryptographicOperations.ZeroMemory(rentedBuffer.AsSpan(0, encryptedBodyLen));
                pinHandle.Free();
                ArrayPool<byte>.Shared.Return(rentedBuffer);
            }
        }

        // 암호화용 — IV 자동 생성
        private static Aes CreateAes(out byte[] iv)
        {
            var aes = CreateAesBase();
            aes.GenerateIV();
            iv = aes.IV;
            return aes;
        }

        // 복호화용 — IV 외부 주입
        private static Aes CreateAes(ReadOnlySpan<byte> iv)
        {
            var aes = CreateAesBase();
            // [HEAP] ToArray — .NET Standard 2.1 Aes.IV setter API 제약
            aes.IV = iv.ToArray();
            return aes;
        }

        private static Aes CreateAesBase()
        {
            var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            return aes;
        }
    }
}
