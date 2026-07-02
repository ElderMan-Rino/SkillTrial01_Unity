using Elder.Framework.Core.Interfaces;
using Elder.Framework.Signal.Infra;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Signal.Installer
{
    public readonly struct SignalInstaller
    {
        public void Install(IGameSystemRegistry registry)
        {
            registry.Register<SignalRouter>().As<ISignalRouter>().As<ISignalCancellable>();
        }
    }
}