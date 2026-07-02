using Elder.Framework.Crypto.Infra;
using Elder.Framework.Crypto.Interfaces;

// ❌ VIOLATION: 폴더 위치 — Crypto/ 루트에 있으나 App/ 또는 Infra/ 하위 폴더에 위치해야 함
//   제안: Crypto/App/EncryptionProviderFactory.cs로 이동
namespace Elder.Framework.Crypto
{
    // AesEncryptionProvider(internal)를 외부(에디터 포함)에 노출하기 위한 팩토리.
    // 구현 타입을 직접 참조하지 않고 IEncryptionProvider 인터페이스로만 반환.
    // ✅ OK: public static — 에디터 외부 노출 목적의 팩토리로 의도적 public 사용
    public static class EncryptionProviderFactory
    {
        // keyPartB는 생성자 내부에서 ZeroMemory로 소거됨 — 호출 측에서 복사본을 전달해야 함
        public static IEncryptionProvider CreateAes(byte[] keyPartB)
        {
            return new AesEncryptionProvider(keyPartB);
        }
    }
}
