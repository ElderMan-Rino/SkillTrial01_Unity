using Elder.Framework.Core.Interfaces;
using Elder.Framework.Signal.Infra;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Signal.Installer
{
    public readonly struct SignalInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.RegisterShared<SignalRouter>()
                .As<ISignalRouter>()
                .As<ISignalCancellable>();
        }
    }
}