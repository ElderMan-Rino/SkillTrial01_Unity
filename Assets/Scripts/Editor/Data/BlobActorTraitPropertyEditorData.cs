#if UNITY_EDITOR
using MessagePack;

namespace Elder.SkillTrial.Resources.Data
{
    [MessagePackObject]
    public readonly struct BlobActorTraitPropertyEditorData
    {
        [Key(0)] public readonly string Key;   // property 이름 — 런타임에 FNV 해시로 변환
        [Key(1)] public readonly float  Value;

        [SerializationConstructor]
        public BlobActorTraitPropertyEditorData(string key, float value)
        {
            Key   = key;
            Value = value;
        }
    }
}
#endif
