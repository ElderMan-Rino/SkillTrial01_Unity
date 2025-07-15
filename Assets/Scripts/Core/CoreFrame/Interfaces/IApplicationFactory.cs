using Elder.Core.Common.Interfaces;
using System;

namespace Elder.Core.CoreFrame.Interfaces
{
    public interface IApplicationFactory : IDisposable
    {
        public bool TryCreateApplication(Type type, IApplicationProvider appProvider, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister, out IApplication application);
    }
}
