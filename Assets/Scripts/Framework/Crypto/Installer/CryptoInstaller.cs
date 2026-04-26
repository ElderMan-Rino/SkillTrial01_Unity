using Elder.Framework.Crypto.Infra;
using Elder.Framework.Crypto.Interfaces;
using Elder.Framework.Data.Interfaces;
using System;
using System.Text;
using VContainer;
using VContainer.Unity;

namespace Elder.Framework.Crypto.Installer
{
    public readonly struct CryptoInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<IEncryptionProvider>(resolver =>
            {
                var config = resolver.Resolve<IDataConfig>();
                // KeyPartB: FrameworkSettings에서 UTF-8 bytes로 변환
                // [HEAP] Encoding.UTF8.GetBytes — 초기화 시 1회
                byte[] keyPartB = Encoding.UTF8.GetBytes(config.EncryptionKeyPartB);
                return new AesEncryptionProvider(keyPartB);
            }, Lifetime.Singleton);
        }
    }
}
