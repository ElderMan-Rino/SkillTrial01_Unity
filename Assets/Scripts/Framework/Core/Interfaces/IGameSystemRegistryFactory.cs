namespace Elder.Framework.Core.Interfaces
{
    public interface IGameSystemRegistryFactory : ISystemComponent
    {
        public IGameSystemRegistry CreateRegistry(IGameSystemProvider parent);
    }
}