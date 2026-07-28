using Elder.Framework.Actor.Interfaces;
using UnityEngine;

namespace Elder.Framework.Actor.Infra
{
    // BlinkState(ECS Component) + BlinkSystem(ECS System, Unity.Mathematics.Random 기반)이 생산한
    // 웨이트 값을 SkinnedMeshRenderer에 옮긴다. 껍데기 단계 — 실제 구현은 추후 작성.
    // 대체 대상: Assets/_ThirdParty/unity-chan!/Unity-chan! Model/Scripts/AutoBlink.cs
    // (원본은 StartCoroutine("RandomChange") 문자열 코루틴 사용 — 이식 시 제거 대상)
    internal sealed class BlinkBridge : MonoBehaviour, IBlinkBridge
    {
        public void SetBlinkWeight(int blendShapeIndex, float weight)
        {
            // TODO: skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, weight) 대체
        }
    }
}
