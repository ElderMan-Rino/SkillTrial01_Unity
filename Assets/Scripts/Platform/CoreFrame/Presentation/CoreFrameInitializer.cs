using Elder.Core.CoreFrame.Application;
using Elder.Core.CoreFrame.Interfaces;
using Elder.Platform.CoreFrame.Infrastructure.Factories;
using Elder.Platform.CoreFrame.Infrastructure;
using System;

namespace Elder.Platform.CoreFrame.Presentation
{
    public sealed class CoreFrameInitializer
    {
        public CoreFrameApplication Initialize()
        {
            var infraFactory = CreateInfrastructureFactory();
            var subInfraFactory = CreateSubInfrastructureFactory();
            var coreFrameInfra = CreateCoreFrameInfra(infraFactory, subInfraFactory);
            var appFactory = CreateApplicationFactory();

            var coreFrameApp = CreateAndInitializeApplication(coreFrameInfra, appFactory);
            coreFrameInfra.InjectAppProvider(coreFrameApp);

            if (!coreFrameApp.TryInitialize())
                throw new InvalidOperationException("ILogEventDispatcher infrastructure is not initialized or not registered. Please check the log event dispatcher configuration.");

            return coreFrameApp;
        }

        private CoreFrameApplication CreateAndInitializeApplication(CoreFrameInfrastructure infra, ApplicationFactory appFactory)
            => new CoreFrameApplication(infra, infra, infra, appFactory);

        private ApplicationFactory CreateApplicationFactory() => new ApplicationFactory();

        private InfrastructureFactory CreateInfrastructureFactory() => new InfrastructureFactory();

        private SubInfrastructureFactory CreateSubInfrastructureFactory() => new SubInfrastructureFactory();

        private CoreFrameInfrastructure CreateCoreFrameInfra(IInfrastructureFactory infraFactory, ISubInfrastructureFactory subInfraFactory)
            => new(infraFactory, subInfraFactory);
    }
}