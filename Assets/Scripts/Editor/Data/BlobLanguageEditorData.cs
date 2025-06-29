using MessagePack;

namespace Elder.SkillTrial.Resources.Data
{
    [MessagePackObject]
    public readonly struct BlobLanguageEditorData
    {
        [Key(0)] public readonly int Id;
        [Key(1)] public readonly string Key;
        [Key(2)] public readonly string Ko;
        [Key(3)] public readonly string En;

        [SerializationConstructor]
        public BlobLanguageEditorData(int id, string key, string ko, string en)
        {
            Id = id;
            Key = key;
            Ko = ko;
            En = en;
        }
    }
}
