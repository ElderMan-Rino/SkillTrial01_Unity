using System.Collections.Generic;

namespace Elder.Framework.Core.Interfaces
{
    public interface IComponentProvider
    {
        public bool TryGetComponent<T>(out T target) where T : IFrameworkComponent;
    }
}