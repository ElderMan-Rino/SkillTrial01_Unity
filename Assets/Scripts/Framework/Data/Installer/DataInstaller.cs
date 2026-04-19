using Elder.Framework.Blob.Infra;
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
            builder.Register<BlobDataDeserializer>(Lifetime.Singleton).As<IDataDeserializer>();
            builder.RegisterEntryPoint<DataProvider>(Lifetime.Singleton).As<IDataProvider>();
        }
    }
}