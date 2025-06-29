using Elder.Framework.Time.Interfaces;
using UnityEngine;

namespace Elder.Framework.Time.Domain
{
    internal sealed class GameClock : IGameClock
    {
        private readonly IGameClock _parent;

        private float _ownScale = 1f;
        private bool _isPaused;
        private float _elapsed;
        private float _unscaledElapsed;
        private float _deltaTime;
        private float _unscaledDeltaTime;

        public float DeltaTime => _deltaTime;
        public float UnscaledDeltaTime => _unscaledDeltaTime;
        public float ElapsedTime => _elapsed;
        public float UnscaledElapsedTime => _unscaledElapsed;
        public float TimeScale => _isPaused ? 0f : _ownScale;
        public bool IsPaused => _isPaused;

        internal GameClock(IGameClock parent = null)
        {
            _parent = parent;
        }

        public void SetTimeScale(float scale) => _ownScale = Mathf.Max(0f, scale);
        public void Pause() => _isPaused = true;
        public void Resume() => _isPaused = false;

        internal void Tick(float parentUnscaledDelta)
        {
            _unscaledDeltaTime = parentUnscaledDelta;
            _unscaledElapsed += parentUnscaledDelta;

            // 부모 배율 상속: 부모가 없으면 원본값 사용
            float parentScale = _parent is null ? 1f : _parent.TimeScale;
            float effectiveScale = _isPaused ? 0f : _ownScale * parentScale;

            _deltaTime = parentUnscaledDelta * effectiveScale;
            _elapsed += _deltaTime;
        }
    }
}
