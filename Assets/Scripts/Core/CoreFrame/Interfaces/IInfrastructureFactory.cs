using Elder.Core.Common.Interfaces;
using System;

namespace Elder.Core.CoreFrame.Interfaces
{
    public interface IInfrastructureFactory : IDisposable
    {
          public bool TryCreateInfrastructure(Type type, IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister, out IInfrastructure infrastructure);
    }
}