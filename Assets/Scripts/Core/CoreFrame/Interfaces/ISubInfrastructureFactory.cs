using Elder.Core.Common.Interfaces;
using System;

namespace Elder.Core.CoreFrame.Interfaces
{
    public interface ISubInfrastructureFactory : IDisposable
    {
        public bool TryCreateSubInfra(Type type, out ISubInfrastructure subInfra);
    }
}