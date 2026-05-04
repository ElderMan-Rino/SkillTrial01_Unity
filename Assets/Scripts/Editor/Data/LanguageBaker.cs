#if UNITY_EDITOR
using Elder.Framework.Crypto;
using MessagePack;
using MessagePack.Resolvers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;

namespace Elder.SkillTrial.Resources.Data
{
    public static class LanguageBaker
    {
        public static void Bake(string sourcePath, string savePath, byte[] encryptionKeyPartB)
        {
            var rawBytes = File.ReadAllBytes(sourcePath);
            var options = MessagePackSerializerOptions.Standard.WithResolver(StandardResolver.Instance);
            var rawList = MessagePackSerializer.Deserialize<List<object[]>>(rawBytes, options);
            var dtoList = rawList.Select(row => new BlobLanguageEditorData(
                System.Convert.ToInt32(row[0]),
                row[1]?.ToString() ?? string.Empty,
                row[2]?.ToString() ?? string.Empty,
                row[3]?.ToString() ?? string.Empty
            )).ToList();

            var builder = new BlobBuilder(Allocator.Temp);
            ref LanguageInfoRoot root = ref builder.ConstructRoot<LanguageInfoRoot>();
            var arrayBuilder = builder.Allocate(ref root.Rows, dtoList.Count);

            for (int i = 0; i < dtoList.Count; i++)
            {
                arrayBuilder[i].Id = dtoList[i].Id;
                builder.AllocateString(ref arrayBuilder[i].Key, dtoList[i].Key);
                builder.AllocateString(ref arrayBuilder[i].Ko, dtoList[i].Ko);
                builder.AllocateString(ref arrayBuilder[i].En, dtoList[i].En);
            }

            var blobRef = builder.CreateBlobAssetReference<LanguageInfoRoot>(Allocator.Temp);
            builder.Dispose();

            var writer = new MemoryBinaryWriter();
            writer.Write(blobRef);

            unsafe
            {
                var plainBytes = new byte[writer.Length];
                Marshal.Copy((System.IntPtr)writer.Data, plainBytes, 0, writer.Length);
                Elder.SkillTrial.Editor.Crypto.BlobEditorEncryptionHelper.WriteEncrypted(plainBytes, savePath, encryptionKeyPartB);
            }

            writer.Dispose();
            blobRef.Dispose();
        }
    }
}
#endif
