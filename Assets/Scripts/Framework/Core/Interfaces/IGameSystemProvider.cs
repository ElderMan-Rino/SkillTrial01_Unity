namespace Elder.Framework.Core.Interfaces
{
    public interface IGameSystemProvider
    {
        public bool TryGetSystem<T>(out T system) where T : class, ISystemComponent;
        public void InjectAll();
        public void InitializeAll();
        public void PostInitializeAll();
        public void DisposeAll();
    }
}