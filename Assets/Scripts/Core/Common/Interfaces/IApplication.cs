using System;

namespace Elder.Core.Common.Interfaces
{
    public interface IApplication : IDisposable
    {
        public void Initialize();
        public void PostInitialize();
    }
}
