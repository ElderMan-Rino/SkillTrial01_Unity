using System;

namespace Elder.Core.Common.Interfaces
{
    public interface IInfrastructure : IDisposable
    {
        public void Initialize();
    }
}
