namespace Elder.Framework.Actor.Interfaces
{
    // ECS(Burst) 측 BlinkSystem이 Unity.Mathematics.Random으로 계산한 블렌드셰이프 웨이트를
    // SkinnedMeshRenderer(UnityChan.AutoBlink 대체)에 적용하기 위한 브릿지 계약.
    // AutoBlink는 Animator를 거치지 않고 블렌드셰이프를 직접 제어하므로 별도 브릿지로 분리한다.
    public interface IBlinkBridge
    {
        public void SetBlinkWeight(int blendShapeIndex, float weight);
    }
}
