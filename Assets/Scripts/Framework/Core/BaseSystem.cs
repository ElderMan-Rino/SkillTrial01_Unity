using Cysharp.Threading.Tasks;
using Elder.Framework.Common.Base;
using Elder.Framework.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Core
{
    public abstract class BaseSystem : DisposableBase, IGameSystem
    {
        protected IGameSystemProvider _systemProvider;

        public void InjectDependency(IGameSystemProvider provider)
        {
            _systemProvider = provider;
            HandleInjectDependency();
        }

        protected abstract void HandleInjectDependency();
        protected bool TryGetSystem<T>(out T system) where T : class, ISystemComponent => _systemProvider.TryGetSystem<T>(out system);
        protected bool TryGetSystems<T>(ref List<T> results) where T : class, ISystemComponent => _systemProvider.TryGetSystems<T>(ref results);
        protected bool TryDisposeSystem(Type systemType) => _systemProvider.TryDisposeSystem(systemType);
        public virtual UniTask InitializeAsync() => UniTask.CompletedTask;
        public virtual UniTask PostInitializeAsync() => UniTask.CompletedTask;
        public override void PreDispose() { }
    }
}
