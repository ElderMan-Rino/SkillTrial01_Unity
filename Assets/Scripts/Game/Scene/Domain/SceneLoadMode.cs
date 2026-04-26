namespace Elder.SkillTrial.Scene.Domain
{
    public enum SceneLoadMode : byte
    {
        Single,               // 기존 씬 전부 언로드 후 교체
        Additive,             // 기존 씬 위에 추가, 이전 씬은 수동 언로드
        AdditiveKeepPrevious  // Additive + 이전 씬 자동 유지
    }
}
