using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Infra;
using Elder.Framework.Log.Interfaces;
using VContainer;
using VContainer.Unity;

namespace Elder.Framework.Log.Installer
{
    public readonly struct LoggerInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<UnityLogAdapter>(Lifetime.Singleton).As<ILogAdapter>();
            builder.Register<LogPublisher>(Lifetime.Singleton).As<ILoggerPublisher>();

            builder.RegisterBuildCallback(HandleRegisterBuildCallback);
        }

        private static void HandleRegisterBuildCallback(IObjectResolver resolver)
        {
            var publisher = resolver.Resolve<ILoggerPublisher>();
            LogFacade.InjectProvider(publisher);
        }
    }
}