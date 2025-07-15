using Elder.Core.CoreFrame.Application;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Platform.CoreFrame.Infrastructure;
using Elder.Platform.CoreFrame.Infrastructure.Factories;
using System;
using UnityEngine;

namespace Elder.Platform.CoreFrame.Presentation
{
    public class CoreFrameRunner : MonoBehaviour
    {
        private IDisposable _applicationDisposable;

        private void Awake()
        {
            RegisterDontDestroyOnLoad();

            var infraFactory = CreateInfrastructureFactory();
            var subInfraeFactory = CreateSubInfrastructureFactory();
            var coreFrameInfra = CreateCoreFrameInfra(infraFactory, subInfraeFactory);
            var applicationFactory = CreateApplicaionFactory();
            CreateCoreFrameApplication(coreFrameInfra, applicationFactory);
        }
        private ApplicationFactory CreateApplicaionFactory()
        {
            return new ApplicationFactory();
        }
        private InfrastructureFactory CreateInfrastructureFactory()
        {
            return new InfrastructureFactory();
        }
        private SubInfrastructureFactory CreateSubInfrastructureFactory()
        {
            return new SubInfrastructureFactory();
        }
        private CoreFrameInfrastructure CreateCoreFrameInfra(IInfrastructureFactory infraFactory, ISubInfrastructureFactory subInfraFactory)
        {
            return new(infraFactory, subInfraFactory);
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
