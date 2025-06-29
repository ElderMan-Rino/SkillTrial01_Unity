using Elder.Framework.Common.App;
using Elder.Framework.Common.Interfaces;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Common.Installer
{
    public readonly struct CommonInstaller : ISystemRegistrar
    {
        public void Install(ISystemRegistry registry)
        {
            registry.RegisterShared<ScopeDisposeSystem>().As<IScopeDisposeSystem>();
        }
    }
}
