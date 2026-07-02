using System;

namespace Elder.Framework.Core.Interfaces
{
    public interface IDisposableBase : IDisposable
    {
        public void PreDispose();
    }
}