using Elder.Framework.Core.Interfaces;
using Elder.Framework.Crypto.Infra;
using Elder.Framework.Crypto.Interfaces;
using Elder.Framework.Boot.Interfaces;
using System.Text;

namespace Elder.Framework.Crypto.Installer
{
    public readonly struct CryptoInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.TryGetRegistered<IBootConfig>(out var config);
            // KeyPartB: FrameworkSettings에서 UTF-8 bytes로 변환
            // [HEAP] Encoding.UTF8.GetBytes — 등록 시점 1회
            byte[] keyPartB = Encoding.UTF8.GetBytes(config.EncryptionKeyPartB);
            registry.RegisterInstance<IEncryptionProvider>(new AesEncryptionProvider(keyPartB));
        }
    }
}
