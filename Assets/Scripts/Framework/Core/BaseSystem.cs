using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Core
{
    public abstract class BaseSystem : IGameSystem
    {
        protected IGameSystemProvider Provider { get; private set; }

        public bool TryInjectDependency(IGameSystemProvider provider)
        {
            Provider = provider;
            return OnInjectDependency();
        }

        public virtual bool TryInitialize() => true;

        public virtual bool TryPostInitialize() => true;

        protected virtual bool OnInjectDependency() => true;
    }
}
