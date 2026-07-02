using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Data.Interfaces
{
    public interface IRawDataDeserializer : ISystemComponent
    {
        public unsafe IDataHandle<T> Deserialize<T>(byte[] data, int length) where T : unmanaged;
    }
}