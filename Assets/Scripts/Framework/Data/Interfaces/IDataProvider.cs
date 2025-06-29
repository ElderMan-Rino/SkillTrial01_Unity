using System.Collections.Generic;

namespace Elder.Framework.Data.Interfaces
{
    public interface IDataProvider
    {
        public T GetData<T>(int id) where T : class, IDataRecord;
        public IReadOnlyList<T> GetAllData<T>() where T : class, IDataRecord;
    }
}