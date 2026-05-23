using Elder.Framework.Common.App;
using Elder.Framework.Common.Interfaces;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Common.Installer
{
    public readonly struct CommonInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.RegisterSharedFactory<ScopeDisposeSystem>(p =>
            {
                p.TryResolve<ISignalRouter>(out var router);
                return new ScopeDisposeSystem(router);
            })
            .As<IScopeDisposeSystem>()
            .As<ScopeDisposeSystem>();
        }
    }
}
