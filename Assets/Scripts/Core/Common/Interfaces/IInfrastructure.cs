using Elder.Core.Common.Enums;
using Elder.Core.CoreFrame.Interfaces;
using System;

namespace Elder.Core.Common.Interfaces
{
    public interface IInfrastructure : IDisposable
    {
        public InfrastructureType InfraType { get; }
        public bool TryInitialize(IInfrastructureProvider infraProvider, IInfrastructureRegister infraRegister, ISubInfrastructureCreator subInfraCreator, IApplicationProvider appProvider);
        public void PreDispose();
    }
}
