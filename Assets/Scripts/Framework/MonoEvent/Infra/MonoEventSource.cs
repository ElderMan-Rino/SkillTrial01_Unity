using R3;
using UnityEngine;

namespace Elder.Framework.MonoEvent.Infra
{
    internal sealed class MonoEventSource : MonoBehaviour
    {
        // [HEAP] Subject 5개: MonoEventSystem 초기화 시 1회 할당
        internal readonly Subject<float> UpdateSubject = new();
        internal readonly Subject<float> FixedUpdateSubject = new();
        internal readonly Subject<float> LateUpdateSubject = new();
        internal readonly Subject<bool> ApplicationPauseSubject = new();
        internal readonly Subject<Unit> ApplicationQuitSubject = new();

        private void Update() => UpdateSubject.OnNext(UnityEngine.Time.unscaledDeltaTime);
        private void FixedUpdate() => FixedUpdateSubject.OnNext(UnityEngine.Time.fixedUnscaledDeltaTime);
        private void LateUpdate() => LateUpdateSubject.OnNext(UnityEngine.Time.unscaledDeltaTime);
        private void OnApplicationPause(bool pauseStatus) => ApplicationPauseSubject.OnNext(pauseStatus);
        private void OnApplicationQuit() => ApplicationQuitSubject.OnNext(Unit.Default);

        private void OnDestroy()
        {
            UpdateSubject.Dispose();
            FixedUpdateSubject.Dispose();
            LateUpdateSubject.Dispose();
            ApplicationPauseSubject.Dispose();
            ApplicationQuitSubject.Dispose();
        }
    }
}
