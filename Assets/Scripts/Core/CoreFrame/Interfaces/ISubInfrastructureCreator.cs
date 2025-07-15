using Elder.Core.Common.Interfaces;
using System;

namespace Elder.Core.CoreFrame.Interfaces
{
    public interface ISubInfrastructureCreator
    {
        public bool TryCreateSubInfra<T>(out ISubInfrastructure subInfra) where T : ISubInfrastructure;
    }
}