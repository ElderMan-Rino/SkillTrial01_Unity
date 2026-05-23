using System;

namespace Elder.Framework.Core.Interfaces
{
    public interface ISystemRegistry
    {
        public void Register<TInterface, TImpl>() where TImpl : class, TInterface, new();
        public ISharedRegistration RegisterShared<TImpl>() where TImpl : class, new();
        public ISharedRegistration RegisterSharedFactory<T>(Func<IGameSystemProvider, T> factory) where T : class;
        public void RegisterInstance<TInterface>(TInterface instance);
        public void RegisterFactory<TInterface>(Func<IGameSystemProvider, TInterface> factory);
        public IGameSystemProvider Build();
    }
}
