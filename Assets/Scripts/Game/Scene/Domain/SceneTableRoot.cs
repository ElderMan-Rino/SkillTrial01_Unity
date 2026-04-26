using Unity.Entities;

namespace Elder.SkillTrial.Scene.Domain
{
    public struct SceneTableRoot
    {
        public BlobArray<SceneRow> Rows;
    }
}
