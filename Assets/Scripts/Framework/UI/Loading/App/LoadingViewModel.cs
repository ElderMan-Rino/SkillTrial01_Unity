using System;
using UnityEngine;

namespace Elder.Framework.UI.Loading.App
{
    internal sealed class LoadingViewModel : IDisposable
    {
        private float _targetProgress;
        private float _displayProgress;

        public float DisplayProgress => _displayProgress;
        public string StatusText { get; private set; } = string.Empty;
        public bool IsVideoRequested { get; private set; }
        public string VideoAssetKey { get; private set; } = string.Empty;

        public event Action OnProgressChanged;
        public event Action OnStatusChanged;
        public event Action OnVideoRequested;
        public event Action OnVideoStopped;

        public void SetTargetProgress(float target)
        {
            _targetProgress = Mathf.Clamp01(target);
        }

        // 매 프레임 호출 — 보간 처리
        public bool Tick(float deltaTime, float smoothSpeed = 2f)
        {
            float prev = _displayProgress;
            _displayProgress = Mathf.MoveTowards(_displayProgress, _targetProgress, deltaTime * smoothSpeed);
            if (Mathf.Abs(_displayProgress - prev) > 0.0001f)
            {
                OnProgressChanged?.Invoke();
                return true;
            }
            return false;
        }

        public void SetStatus(string text)
        {
            StatusText = text;
            OnStatusChanged?.Invoke();
        }

        public void RequestVideo(string assetKey)
        {
            IsVideoRequested = true;
            VideoAssetKey = assetKey;
            OnVideoRequested?.Invoke();
        }

        public void StopVideo()
        {
            IsVideoRequested = false;
            OnVideoStopped?.Invoke();
        }

        public void Dispose()
        {
            OnProgressChanged = null;
            OnStatusChanged = null;
            OnVideoRequested = null;
            OnVideoStopped = null;
        }
    }
}
