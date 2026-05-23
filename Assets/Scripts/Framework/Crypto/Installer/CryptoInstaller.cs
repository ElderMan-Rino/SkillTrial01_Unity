using Elder.Framework.Core.Interfaces;
using Elder.Framework.Crypto.Infra;
using Elder.Framework.Crypto.Interfaces;
using Elder.Framework.Data.Interfaces;
using System.Text;

namespace Elder.Framework.Crypto.Installer
{
    public readonly struct CryptoInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.RegisterFactory<IEncryptionProvider>(provider =>
            {
                provider.TryResolve<IDataConfig>(out var config);
                // KeyPartB: FrameworkSettings에서 UTF-8 bytes로 변환
                // [HEAP] Encoding.UTF8.GetBytes — 초기화 시 1회
                byte[] keyPartB = Encoding.UTF8.GetBytes(config.EncryptionKeyPartB);
                return new AesEncryptionProvider(keyPartB);
            });
        }
    }
}
