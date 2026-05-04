using System;

namespace Elder.Framework.Data.Interfaces
{
    public interface IDataHandle<T> : IDisposable
    {
        public bool TryGetData(out T data);
    }
}