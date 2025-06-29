using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Unity.Common.BaseClasses;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Elder.Unity.CoreFrame.Infrastructure
{
    public class CoreFrameInfrastructure : DisposableMono, IInfrastructureProvider
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
        protected override void DisposeManagedResources()
        {

        }
        protected override void DisposeUnmanagedResources()
        {

        }
        protected override void CompleteDispose()
        {
            
        }
       
    }
}
