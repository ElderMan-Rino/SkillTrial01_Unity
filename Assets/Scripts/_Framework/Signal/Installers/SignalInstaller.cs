using Elder.Framework.Core.Interfaces;
using Elder.Framework.Signal.Infra;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Singal.Installers
{
    public static class SignalInstaller
    {
        public static void Install(IComponentRegistry registry)
        {
            registry.Regist<SignalHub>().As<ISignalPublisher>().As<ISignalSubscriber>();
        }
    }
}