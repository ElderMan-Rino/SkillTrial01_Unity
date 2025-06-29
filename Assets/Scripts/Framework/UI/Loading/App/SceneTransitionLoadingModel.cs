using System;
using Elder.Framework.UI.Loading.Interfaces;
using UnityEngine;

namespace Elder.Framework.UI.Loading.App
{
    internal sealed class SceneTransitionLoadingModel : ILoadingModel, ILoadingReporter
    {
        private float _progress;
        private string _statusText = string.Empty;
        private bool _isComplete;

        public float Progress => _progress;
        public string StatusText => _statusText;
        public bool IsComplete => _isComplete;

        public event Action OnChanged;

        public void Report(float progress, string statusText)
        {
            _progress = Mathf.Clamp01(progress);
            _statusText = statusText ?? string.Empty;
            OnChanged?.Invoke();
        }

        public void Complete()
        {
            _progress = 1f;
            _isComplete = true;
            OnChanged?.Invoke();
        }
    }
}
