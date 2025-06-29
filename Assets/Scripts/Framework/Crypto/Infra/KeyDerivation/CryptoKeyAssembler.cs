using System;
using System.Security.Cryptography;

namespace Elder.Framework.Crypto.Infra.KeyDerivation
{
    // RFC 5869 HKDF-SHA256 수동 구현 (Unity .NET Standard 2.1 — HKDF 클래스 미지원)
    // 파생 키는 stackalloc Span에만 존재하며 사용 후 즉시 소거.
    internal static class CryptoKeyAssembler
    {
        private const int KeySize = 32;
        private const int HashLen = 32; // SHA-256 출력 크기

        // partA: 바이너리 분산 상수(salt), partB: asset 저장값(IKM), info: 도메인 구분 라벨
        public static void DeriveKey(
            ReadOnlySpan<byte> partA,
            ReadOnlySpan<byte> partB,
            ReadOnlySpan<byte> info,
            Span<byte> outputKey)
        {
            if (outputKey.Length != KeySize)
                throw new ArgumentException($"outputKey must be {KeySize} bytes");

            // HKDF-Extract: HMAC-SHA256(salt=partA, IKM=partB) → PRK
            Span<byte> prk = stackalloc byte[HashLen];
            HmacSha256(partA, partB, prk);

            // HKDF-Expand: T(1) = HMAC-SHA256(PRK, info || 0x01) — KeySize <= HashLen이므로 1라운드
            // [HEAP] info 복사 버퍼 — stackalloc, 관리 힙 미사용
            Span<byte> expandInput = stackalloc byte[info.Length + 1];
            info.CopyTo(expandInput);
            expandInput[info.Length] = 0x01;
            HmacSha256(prk, expandInput, outputKey);

            prk.Clear();
            expandInput.Clear();
        }

        // HMAC-SHA256(key, data) → output (output.Length == HashLen 보장 필요)
        // [HEAP] HMACSHA256 인스턴스 — 초기화 시 1회, using으로 즉시 해제
        private static void HmacSha256(ReadOnlySpan<byte> key, ReadOnlySpan<byte> data, Span<byte> output)
        {
            // [HEAP] key/data ToArray — .NET Standard 2.1 HMACSHA256 API 제약
            using var hmac = new HMACSHA256(key.ToArray());
            byte[] result = hmac.ComputeHash(data.ToArray());
            result.AsSpan().CopyTo(output);
            CryptographicOperations.ZeroMemory(result);
        }
    }
}
