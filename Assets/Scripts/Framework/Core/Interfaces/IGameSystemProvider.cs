using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Core.Interfaces
{
    public interface IGameSystemProvider
    {
        public bool TryGetSystem<T>(out T system) where T : class, ISystemComponent;
        public bool TryGetSystems<T>(ref List<T> system) where T : class, ISystemComponent;
        public bool TryDisposeSystem(Type systemType);
        public void InjectAll();
        public UniTask InitializeAllAsync();
        public UniTask PostInitializeAllAsync();
    }
}