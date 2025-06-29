namespace Elder.Framework.Core.Interfaces
{
    public interface IGameSystem : ISystemComponent
    {
        public bool TryInjectDependency(IGameSystemProvider provider);
        public bool TryInitialize();
        public bool TryPostInitialize();
        public void TryDispose();
    }
}