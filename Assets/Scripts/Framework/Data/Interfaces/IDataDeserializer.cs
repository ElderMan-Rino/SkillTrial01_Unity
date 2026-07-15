using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Blob.Interfaces
{
    public interface IDataDeserializer : ISystemComponent
    {
        public unsafe IBlobDataHandle<T> Deserialize<T>(byte[] data) where T : unmanaged;
       
    }
}