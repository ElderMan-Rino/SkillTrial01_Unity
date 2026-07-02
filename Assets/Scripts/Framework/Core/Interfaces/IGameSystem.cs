using Cysharp.Threading.Tasks;

namespace Elder.Framework.Core.Interfaces
{
    public interface IGameSystem : ISystemComponent
    {
        public void InjectDependency(IGameSystemProvider provider);
        public UniTask InitializeAsync();
        public UniTask PostInitializeAsync();
    }
}