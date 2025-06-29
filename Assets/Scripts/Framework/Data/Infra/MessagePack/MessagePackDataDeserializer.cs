using Elder.Framework.Data.Interfaces;
using MessagePack;

namespace Elder.Framework.Data.Infra.MessagePack
{
    public class MessagePackDataDeserializer : IDataDeserializer
    {
        public T Deserialize<T>(byte[] data)
        {
            var options = MessagePackSerializerOptions.Standard;
            return MessagePackSerializer.Deserialize<T>(data, options);
        }
    }
}