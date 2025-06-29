#if UNITY_EDITOR
using MessagePack;

namespace Elder.SkillTrial.Resources.Data
{
    [MessagePackObject]
    public readonly struct BlobActorTraitEditorData
    {
        [Key(0)] public readonly int                              TraitId;
        [Key(1)] public readonly BlobActorTraitPropertyEditorData[] Properties;

        [SerializationConstructor]
        public BlobActorTraitEditorData(int traitId, BlobActorTraitPropertyEditorData[] properties)
        {
            TraitId    = traitId;
            Properties = properties;
        }
    }
}
#endif
