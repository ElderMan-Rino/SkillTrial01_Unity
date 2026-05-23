namespace Elder.Framework.Core.Interfaces
{
    public interface IGameSystem
    {
        public bool TryInjectDependency(IGameSystemProvider provider);
        public bool TryInitialize();
        public bool TryPostInitialize();
    }
}