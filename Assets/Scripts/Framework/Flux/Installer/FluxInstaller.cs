using Elder.Framework.Flux.Infra;
using Elder.Framework.Flux.Interfaces;
using VContainer;
using VContainer.Unity;

namespace Elder.Framework.Flux.Installer
{
    public readonly struct FluxInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<FluxRouter>(Lifetime.Singleton).As<IFluxRouter>();
        }
    }
}