using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Root.Infra
{
    internal sealed class RootRegistrationEntry : ISharedRegistration
    {
        private readonly IFrameworkComponent _instance;
        private readonly IComponentRegistry _registry;

        public RootRegistrationEntry(IFrameworkComponent instance, IComponentRegistry owner)
        {
            _instance = instance;
            _registry = owner;
        }

        public ISharedRegistration As<T>() where T : IFrameworkComponent
        {
            var classType = _instance.ClassType;
            var interfaceType = typeof(T);
            _registry.MapAlias(interfaceType, classType);
            return this;
        }
    }
}
