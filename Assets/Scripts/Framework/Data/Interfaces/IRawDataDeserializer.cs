using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Blob.Interfaces
{
    public interface IRawDataDeserializer : ISystemComponent
    {
        public unsafe IBlobDataHandle<T> Deserialize<T>(byte[] data, int length) where T : unmanaged;
    }
}