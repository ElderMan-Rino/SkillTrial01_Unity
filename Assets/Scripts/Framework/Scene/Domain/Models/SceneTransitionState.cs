// ✅ OK: internal enum — 접근 수정자 준수
// ✅ OK: 폴더 위치 — Domain/Models/ (씬 상태 도메인 모델)
// ❌ VIOLATION: TransitionDirector(GameMode 모듈)에서만 사용 — Scene.Domain이 아닌 GameMode.Domain에 속해야 할 가능성
//   제안: 사용 모듈 기준으로 GameMode/Domain/Models/SceneTransitionState.cs로 이동 검토
namespace Elder.Framework.Scene.Domain.Models
{
    internal enum SceneTransitionState : byte
    {
        Idle,
        InProgress,
    }
}
