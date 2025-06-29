using Elder.Framework.Data.App;
using Elder.Framework.Data.Infra.MessagePack;
using Elder.Framework.Data.Interfaces;
using VContainer;
using VContainer.Unity;

namespace Elder.Framework.Data.Installer
{
    public readonly struct DataInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<MessagePackDataDeserializer>(Lifetime.Singleton).As<IDataDeserializer>();
            builder.Register<DataProvider>(Lifetime.Singleton).As<IDataProvider>();
        }
    }
}