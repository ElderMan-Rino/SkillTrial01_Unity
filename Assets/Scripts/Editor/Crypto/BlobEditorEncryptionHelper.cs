#if UNITY_EDITOR
using Elder.Framework.Crypto;
using Elder.Framework.Crypto.Interfaces;
using System;
using System.IO;

namespace Elder.SkillTrial.Editor.Crypto
{
    // Baker에서 호출하는 에디터 전용 암호화 유틸.
    // EncryptionProviderFactory를 통해 런타임과 동일한 AesEncryptionProvider를 사용.
    public static class BlobEditorEncryptionHelper
    {
        // keyPartB는 생성자에서 ZeroMemory로 소거됨 — 호출 측에서 복사본을 전달해야 함
        public static void WriteEncrypted(ReadOnlySpan<byte> plaintext, string savePath, byte[] keyPartB)
        {
            // [HEAP] 에디터 전용, 성능 무관
            using IEncryptionProvider provider = EncryptionProviderFactory.CreateAes(keyPartB);

            int encryptedSize = provider.GetEncryptedSize(plaintext.Length);
            byte[] encryptedBuffer = new byte[encryptedSize];

            int written = provider.Encrypt(plaintext, encryptedBuffer);

            using var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            fileStream.Write(encryptedBuffer, 0, written);
        }
    }
}
#endif
