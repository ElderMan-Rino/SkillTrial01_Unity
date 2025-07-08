using Elder.Core.Common.Interfaces;
using System;

namespace Elder.Core.CoreFrame.Interfaces
{
    public interface IApplicationFactory
    {
        public bool TryCreateApplication(Type type, out IApplication application);
    }
}
