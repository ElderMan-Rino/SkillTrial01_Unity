namespace Elder.Framework.Actor.Interfaces
{
    // ECS(Burst) 측 FaceAnimationSystem이 계산한 표정 상태(현재 웨이트, 목표 표정 이름 등)를
    // Animator(UnityChan.FaceUpdate 대체)에 적용하기 위한 브릿지 계약.
    // ISystem은 UnityEngine.Object(Animator)를 직접 다룰 수 없으므로 이 인터페이스를 경유한다.
    public interface IFaceAnimationBridge
    {
        public void SetFaceLayerWeight(float weight);
        public void CrossFadeFace(string faceStateName, float fadeDuration);
    }
}
