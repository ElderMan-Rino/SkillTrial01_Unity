using Elder.Framework.Blob.Infra;
using Elder.Framework.Crypto.Interfaces;
using Elder.Framework.Data.App;
using Elder.Framework.Data.Interfaces;
using VContainer;
using VContainer.Unity;

namespace Elder.Framework.Data.Installer
{
    public readonly struct DataInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // 내부 구현체 — EncryptedBlobDataDeserializer가 위임 대상으로 직접 resolve
            builder.Register<BlobDataDeserializer>(Lifetime.Singleton);

            // IDataDeserializer → 복호화 데코레이터
            builder.Register<IDataDeserializer>(resolver =>
                new EncryptedBlobDataDeserializer(
                    resolver.Resolve<IEncryptionProvider>(),
                    resolver.Resolve<BlobDataDeserializer>()),
                Lifetime.Singleton);

            builder.RegisterEntryPoint<DataProvider>(Lifetime.Singleton)
                .As<IDataProvider>()
                .As<IDataSheetLoader>();
        }
    }
}
