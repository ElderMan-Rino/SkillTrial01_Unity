using System;

namespace Elder.Framework.Core.Interfaces
{
    public interface IComponentRegistry
    {
        public ISharedRegistration Regist<T>() where T : class, IFrameworkComponent, new();
        public void MapAlias(Type aliasType, Type classType);
    }
}
