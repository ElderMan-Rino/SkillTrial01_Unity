using System.Collections.Generic;

namespace Elder.Framework.Data.Interfaces
{
    public interface IDataProvider
    {
        public IDataHandle<T> GetData<T>() where T : unmanaged;
        public IReadOnlyList<IDataHandle<T>> GetAllData<T>() where T : unmanaged;
    }
}