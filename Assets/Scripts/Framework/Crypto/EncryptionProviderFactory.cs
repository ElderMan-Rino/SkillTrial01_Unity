using Elder.Framework.Crypto.Infra;
using Elder.Framework.Crypto.Interfaces;

namespace Elder.Framework.Crypto
{
    // AesEncryptionProvider(internal)를 외부(에디터 포함)에 노출하기 위한 팩토리.
    // 구현 타입을 직접 참조하지 않고 IEncryptionProvider 인터페이스로만 반환.
    public static class EncryptionProviderFactory
    {
        // keyPartB는 생성자 내부에서 ZeroMemory로 소거됨 — 호출 측에서 복사본을 전달해야 함
        public static IEncryptionProvider CreateAes(byte[] keyPartB)
        {
            return new AesEncryptionProvider(keyPartB);
        }
    }
}
