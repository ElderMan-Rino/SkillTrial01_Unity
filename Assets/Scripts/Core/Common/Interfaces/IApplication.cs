using Elder.Core.Common.Enums;
using Elder.Core.CoreFrame.Interfaces;
using System;

namespace Elder.Core.Common.Interfaces
{
    public interface IApplication : IDisposable
    {
        public ApplicationType AppType { get; }
        public bool TryInitialize(IApplicationProvider appProvider, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister);
        public bool TryPostInitialize();
        public void PreDispose();
    }
}
