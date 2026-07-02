using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Data.Interfaces
{
    public interface IDataDeserializer : ISystemComponent
    {
        public unsafe IDataHandle<T> Deserialize<T>(byte[] data) where T : unmanaged;
       
    }
}