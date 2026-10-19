using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Elder.Framework.Core.Interfaces
{
    public interface IComponentRegistry
    {
        public ISharedRegistration Regist<T>() where T : class, IFrameworkComponent, new();
        public void MapAlias(Type aliasType, Type classType);
        public void InjectDependencies();
        public UniTask InitializeComponentsAsync(CancellationToken cancellationToken);
        public UniTask PostInitializeComponentsAsync(CancellationToken cancellationToken);
    }
}
