using Elder.Framework.Blob.Infra;
using Elder.Framework.Crypto.Interfaces;
using Elder.Framework.Data.App;
using Elder.Framework.Data.Interfaces;
using Elder.SkillTrial.Resources.Data;
using VContainer;
using VContainer.Unity;

namespace Elder.Framework.Data.Installer
{
    public readonly struct DataInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<BlobDataDeserializer>(Lifetime.Singleton);
            builder.Register<IDataDeserializer, EncryptedBlobDataDeserializer>(Lifetime.Singleton);
            builder.RegisterEntryPoint<DataProvider>(Lifetime.Singleton).As<IDataProvider>().As<IDataSheetLoader>();
        }
    }
}
