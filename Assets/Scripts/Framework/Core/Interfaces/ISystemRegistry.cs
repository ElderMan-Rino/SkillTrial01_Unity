using System.Collections.Generic;

namespace Elder.Framework.Core.Interfaces
{
    public interface ISystemRegistry
    {
        public void Register<TInterface, TImpl>() where TInterface : ISystemComponent where TImpl : class, TInterface, new();
        public ISharedRegistration RegisterShared<T>() where T : ISystemComponent, new();
        public void RegisterInstance<TInterface>(TInterface instance) where TInterface : ISystemComponent;
        public bool TryGetRegistered<T>(out T instance);
        public List<T> GetAllRegistered<T>();
        public IGameSystemProvider Build();
    }
}
