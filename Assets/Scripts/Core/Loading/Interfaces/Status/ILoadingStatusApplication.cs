using Elder.Core.Common.Interfaces;
using System;

namespace Elder.Core.Loading.Interfaces.Status
{
    public interface ILoadingStatusApplication : IApplication
    {
        public bool TryRegisterReporter<T>(T reporter) where T : class, ILoadingStatusReporter;
    }
}