namespace Elder.Framework.Data.Interfaces
{
    public interface IDataDeserializer
    {
        public IDataHandle<T> Deserialize<T>(byte[] data) where T : unmanaged;
    }
}