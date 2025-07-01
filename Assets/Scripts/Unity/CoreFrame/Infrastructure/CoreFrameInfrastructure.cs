using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Core.Logging.Application;
using Elder.Core.Logging.Interfaces;
using Elder.Unity.Logging.Infrastructure;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elder.Unity.CoreFrame.Infrastructure
{
    public class CoreFrameInfrastructure : MonoBehaviour, IDisposable, IInfrastructureProvider
    {
        private Dictionary<Type, IInfrastructure> _persistentInfra;
        private Dictionary<Type, IInfrastructure> _sceneInfra;

        private void Awake()
        {
            SetupPersistence();
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

        }
    }
}
