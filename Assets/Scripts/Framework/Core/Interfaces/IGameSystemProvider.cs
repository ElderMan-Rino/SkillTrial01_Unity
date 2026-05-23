namespace Elder.Framework.Core.Interfaces
{
    public interface IGameSystemProvider
    {
        public bool TryGetSystem<T>(out T system) where T : IGameSystem;
        public bool TryResolve<T>(out T instance);
    }
}
