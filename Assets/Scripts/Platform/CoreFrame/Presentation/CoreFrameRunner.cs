using System;
using UnityEngine;

namespace Elder.Platform.CoreFrame.Presentation
{
    public class CoreFrameRunner : MonoBehaviour
    {
        private IDisposable _coreFrameAppDisposable;

        private void Awake()
        {
            RegisterDontDestroyOnLoad();
            InitializeCoreFrame();
        }

        private void InitializeCoreFrame()
        {
            var initializer = new CoreFrameInitializer();
            var app = initializer.Initialize();
            _coreFrameAppDisposable = app;
        }

        private void RegisterDontDestroyOnLoad()
        {
            DontDestroyOnLoad(this);
        }

        private void OnDestroy()
        {
            DisposeApplication();
        }

        private void DisposeApplication()
        {
            _coreFrameAppDisposable?.Dispose();
            _coreFrameAppDisposable = null;
        }
    }
}
