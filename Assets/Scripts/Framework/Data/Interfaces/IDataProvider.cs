namespace Elder.Framework.Data.Interfaces
{
    public interface IDataProvider
    {
        public IDataHandle<T> GetData<T>() where T : unmanaged;
    }
}