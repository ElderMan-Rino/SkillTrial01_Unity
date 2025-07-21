using Elder.Core.Common.Interfaces;

namespace Elder.Core.LoadingStatus.Interfaces
{
    public interface ILoadingStatusApplication : IApplication
    {
        public bool TryRegisterReporter<T>(T reporter) where T : class, ILoadingStatusReporter;
    }
}