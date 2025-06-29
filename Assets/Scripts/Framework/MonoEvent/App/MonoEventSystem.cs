using Elder.Framework.Core;
using Elder.Framework.MonoEvent.Infra;
using Elder.Framework.MonoEvent.Interfaces;
using R3;
using UnityEngine;

namespace Elder.Framework.MonoEvent.App
{
    internal sealed class MonoEventSystem : BaseSystem, IMonoEventBus
    {
        private MonoEventSource _source;

        public Observable<float> OnUpdate => _source.UpdateSubject;
        public Observable<float> OnFixedUpdate => _source.FixedUpdateSubject;
        public Observable<float> OnLateUpdate => _source.LateUpdateSubject;
        public Observable<bool> OnApplicationPause => _source.ApplicationPauseSubject;
        public Observable<Unit> OnApplicationQuit => _source.ApplicationQuitSubject;

        public override bool TryInitialize()
        {
            // [HEAP] GameObject + MonoBehaviour 생성 — 초기화 시 1회
            var go = new GameObject("[MonoEventSource]");
            Object.DontDestroyOnLoad(go);
            _source = go.AddComponent<MonoEventSource>();
            return true;
        }

        protected override void OnDispose()
        {
            if (_source is not null)
            {
                Object.Destroy(_source.gameObject);
                _source = null;
            }
        }
    }
}
