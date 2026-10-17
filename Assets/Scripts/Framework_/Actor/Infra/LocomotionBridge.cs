/*
using Elder.Framework.Actor.Interfaces;
using UnityEngine;

namespace Elder.Framework.Actor.Infra
{
    // IInputProvider/IInputContext에서 들어온 입력을 ECS 측이 판정한 결과를 Animator에 반영한다.
    // 껍데기 단계 — 실제 구현은 추후 작성.
    // 대체 대상: Assets/_ThirdParty/unity-chan!/Unity-chan! Model/Scripts/IdleChanger.cs
    // (원본은 raw Input.GetKeyDown/GetButton 폴링 + StartCoroutine("RandomChange") 문자열 코루틴 사용 —
    //  이식 시 IInputProvider 기반 입력과 UniTask 기반 처리로 교체 대상)
    internal sealed class LocomotionBridge : MonoBehaviour, ILocomotionBridge
    {
        public void SetIdleState(bool isIdleChanged)
        {
            // TODO: anim.SetBool("IdleChanged", isIdleChanged) 대체
        }

        public bool IsInState(string stateName)
        {
            // TODO: anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) 대체
            return false;
        }
    }
}

*/
