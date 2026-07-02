using Elder.Framework.Blob.Infra;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Data.App;
using Elder.Framework.Data.Interfaces;

namespace Elder.Framework.Data.Installer
{
    public readonly struct DataInstaller
    {
        public void Install(IGameSystemRegistry registry)
        {
            registry.Register<BlobDataDeserializer>().As<IRawDataDeserializer>();
            registry.Register<EncryptedBlobDataDeserializer>().As<IDataDeserializer>();
            registry.Register<DataProvider>().As<IDataProvider>().As<IDataSheetLoader>();
            registry.Register<DataInitializer>().As<IDataInitializer>();
        }
    }
}
