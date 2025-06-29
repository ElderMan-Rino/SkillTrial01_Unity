using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Data.Interfaces
{
    public interface IDataProvider : IGameSystem
    {
        public IDataHandle<T> GetData<T>() where T : unmanaged;
    }
}