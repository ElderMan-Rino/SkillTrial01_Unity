using Elder.Framework.Data.Interfaces;
using Elder.SkillTrial.Resources.Data.Resolvers;
using MessagePack;
using MessagePack.Resolvers;
using Unity.Entities;
using Unity.Entities.Serialization;

namespace Elder.Framework.MessagePack.Infra.MessagePack
{
    public class MessagePackDataDeserializer : IDataDeserializer
    {
        private readonly MessagePackSerializerOptions _options;

        public MessagePackDataDeserializer()
        {
            StaticCompositeResolver.Instance.Register(
                GeneratedResolver.Instance,
                StandardResolver.Instance
            );

            _options = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
            global::MessagePack.MessagePackSerializer.DefaultOptions = _options;
        }

        public BlobAssetReference<T> DeserializeBlob<T>(byte[] data) where T : unmanaged
        {
            unsafe
            {
                fixed (byte* ptr = data)
                {
                    using var reader = new MemoryBinaryReader(ptr, data.Length);
                    // 정상 작동: 구조체 T(예: TestSheetRoot)를 읽어서 BlobAssetReference<T> 반환
                    return reader.Read<T>();
                }
            }
        }

        public IDataHandle<T> Deserialize<T>(byte[] data) where T : unmanaged
        {
            return null;
        }
    }
}