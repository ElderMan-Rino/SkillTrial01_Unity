using MessagePack;
using MessagePack.Formatters;
using Unity.Collections;

// 프레임워크의 핵심 인프라 네임스페이스
namespace Elder.Framework.Data.Infra.MessagePack.Formatters
{
    public class FixedString32Formatter : IMessagePackFormatter<FixedString32Bytes>
    {
        public void Serialize(ref MessagePackWriter writer, FixedString32Bytes value, MessagePackSerializerOptions options)
        {
            // 데이터를 string 형태로 직렬화하여 호환성 유지
            // writer.WriteString(value.ToString());
        }

        public FixedString32Bytes Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            // 바이너리의 UTF-8 바이트를 FixedString으로 즉시 복사
            var s = reader.ReadString();
            return s == null ? default : (FixedString32Bytes)s;
        }
    }
}