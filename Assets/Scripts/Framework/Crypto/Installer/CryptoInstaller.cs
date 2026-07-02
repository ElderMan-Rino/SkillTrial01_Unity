using Elder.Framework.Core.Interfaces;
using Elder.Framework.Crypto.Infra;
using Elder.Framework.Crypto.Interfaces;
using Elder.Framework.Boot.Interfaces;
using System.Text;

namespace Elder.Framework.Crypto.Installer
{
    public readonly struct CryptoInstaller 
    {
        public void Install(IGameSystemRegistry registry, IBootConfig config)
        {
            byte[] keyPartB = Encoding.UTF8.GetBytes(config.EncryptionKeyPartB); // [HEAP] UTF8 인코딩 결과 byte[] 할당
            registry.RegisterInstance<AesEncryptionProvider>(new AesEncryptionProvider(keyPartB)).As<IEncryptionProvider>();
        }
    }
}
