using System;

namespace Elder.Framework.Data.Interfaces
{
    public interface IDataHandle<T> : IDisposable
    {
        bool TryGetData(out T data);
    }
}