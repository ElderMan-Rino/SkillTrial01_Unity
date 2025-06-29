using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Data.Interfaces
{
    public interface IDataDeserializer : IGameSystem
    {
        public IDataHandle<T> Deserialize<T>(byte[] data) where T : unmanaged;
    }
}