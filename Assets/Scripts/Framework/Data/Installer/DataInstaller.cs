using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Blob.Infra;
using Elder.Framework.Common.Interfaces;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Crypto.Interfaces;
using Elder.Framework.Data.App;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Localize.Interfaces;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Data.Installer
{
    public readonly struct DataInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.Register<BlobDataDeserializer, BlobDataDeserializer>();
            registry.RegisterFactory<IDataDeserializer>(p =>
            {
                p.TryResolve<IEncryptionProvider>(out var encryption);
                p.TryResolve<BlobDataDeserializer>(out var inner);
                return new EncryptedBlobDataDeserializer(encryption, inner);
            });
            registry.RegisterSharedFactory<DataProvider>(p =>
            {
                p.TryResolve<IAssetProvider>(out var asset);
                p.TryResolve<IDataDeserializer>(out var deser);
                p.TryResolve<ILoggerPublisher>(out var logger);
                return new DataProvider(asset, deser, logger);
            })
            .As<IDataProvider>()
            .As<IDataSheetLoader>();
            registry.RegisterSharedFactory<DataInitializer>(p =>
            {
                p.TryResolve<ISignalRouter>(out var router);
                p.TryResolve<IDataSheetLoader>(out var sheetLoader);
                p.TryResolve<IGameDataLoader>(out var gameDataLoader);
                p.TryResolve<IDataProvider>(out var dataProvider);
                p.TryResolve<ILocaleSystem>(out var localeSystem);
                p.TryResolve<ILoggerPublisher>(out var logger);
                return new DataInitializer(router, sheetLoader, gameDataLoader, dataProvider, localeSystem, logger);
            })
            .As<IScopedSystem>()
            .As<DataInitializer>();
        }
    }
}
