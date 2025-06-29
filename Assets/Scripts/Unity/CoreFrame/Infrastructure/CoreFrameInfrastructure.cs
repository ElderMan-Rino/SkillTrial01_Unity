using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.Logging.Application;
using Elder.Core.Logging.Interfaces;
using Elder.Unity.Common.BaseClasses;
using Elder.Unity.Logging.Infrastructure;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elder.Unity.CoreFrame.Infrastructure
{
    public class CoreFrameInfrastructure : MonoBehaviour, IDisposable, IInfrastructureProvider
    {
        private ILoggerEx _logger = LogApplication.In.CreateLogger<CoreFrameInfrastructure>();

        private Dictionary<Type, IInfrastructure> _persistentInfra;
        private Dictionary<Type, IInfrastructure> _sceneInfra;

        private void Awake()
        {
            InitializeUnityLogger();
            SetupPersistence();
        }
        private void InitializeUnityLogger()
        {
            UnityLogger.In.SubscribeToLogAppplication();
        }
        private void SetupPersistence()
        {
            RegisterDontDestroyOnLoad();
        }
        private void RegisterDontDestroyOnLoad()
        {
            DontDestroyOnLoad(this);
        }
        public bool TryGetInfrastructure<T>(out T targetInfrastructure) where T : IInfrastructure
        {
            targetInfrastructure = default(T);

            return false;
        }
        public void RegisterInfrastructure<T>() where T : IInfrastructure
        {

        }
        public void Dispose()
        {
            // Dispose의 순서
            // Appication에 이벤트 전달
            // Application이 Dipose 실행
            // Dipose안에서 인프라 Dipose호출 
            ClearLogger();
            DisposeeUnityLogger();
        }
        private void ClearLogger()
        {
            _logger = null;
        }
        private void DisposeeUnityLogger()
        {
            UnityLogger.In.Dispose();
        }
    }
}
