using Elder.Framework.Core;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Composition
{
    internal sealed class GameSystemRegistryFactory : BaseSystem, IGameSystemRegistryFactory
    {
        protected override void HandleInjectDependency() {}

        public IGameSystemRegistry CreateRegistry(IGameSystemProvider parent)
        {
            return new ScopedSystemRegistry(parent);
        }
    }
}
