using Unity.Entities;

namespace Elder.SkillTrial.Scene.Domain
{
    public struct SceneRow
    {
        public int Id;
        public BlobString Key;           // 게임 로직용 키 ("Main")
        public BlobString AddressableKey; // Addressables 로드 키
        public SceneLoadMode LoadMode;
    }
}
