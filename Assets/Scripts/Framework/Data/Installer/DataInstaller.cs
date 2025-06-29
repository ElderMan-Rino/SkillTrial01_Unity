using Elder.Framework.Blob.Infra;
using Elder.Framework.Common.Interfaces;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Data.App;
using Elder.Framework.Data.Interfaces;

namespace Elder.Framework.Data.Installer
{
    public readonly struct DataInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.Register<BlobDataDeserializer, BlobDataDeserializer>();
            // [HEAP] 등록 시점 1회
            registry.RegisterInstance<IDataDeserializer>(new EncryptedBlobDataDeserializer());
            registry.RegisterShared<DataProvider>()
                .As<IDataProvider>()
                .As<IDataSheetLoader>();
            registry.RegisterShared<DataInitializer>()
                .As<IScopedSystem>()
                .As<DataInitializer>();
        }
    }
}
