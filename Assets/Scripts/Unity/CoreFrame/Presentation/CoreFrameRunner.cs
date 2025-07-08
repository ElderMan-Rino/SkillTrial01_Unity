using Elder.Core.CoreFrame.Application;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Unity.CoreFrame.Infrastructure;
using Elder.Unity.CoreFrame.Infrastructure.Factories;
using System;
using UnityEngine;

namespace Elder.Unity.CoreFrame.Presentation
{
    public class CoreFrameRunner : MonoBehaviour
    {
        private IDisposable _applicationDisposable;

        private void Awake()
        {
            RegisterDontDestroyOnLoad();

            var infrastructrueFactory = CreateInfrastructureFactory();
            var coreFrameInfra = CreateCoreFrameInfra(infrastructrueFactory);
            var applicationFactory = CreateApplicaionFactory();
            CreateCoreFrameApplication(coreFrameInfra, applicationFactory);
        }
        private ApplicationFactory CreateApplicaionFactory()
        {
            return new ApplicationFactory();
        }
        private InfrastructureFactory CreateInfrastructureFactory()
        {
            var infraFactory = new InfrastructureFactory();
            return infraFactory;
        }
        private CoreFrameInfrastructure CreateCoreFrameInfra(IInfrastructureFactory infrastructureFactory)
        {
            return new(infrastructureFactory);
        }
        private void CreateCoreFrameApplication(CoreFrameInfrastructure coreFrameInfra, ApplicationFactory applycationFactory)
        {
            _applicationDisposable = new CoreFrameApplication(coreFrameInfra, coreFrameInfra, coreFrameInfra, applycationFactory);
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
            _applicationDisposable.Dispose();
            _applicationDisposable = null;
        }
    }
}
