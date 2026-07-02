using Elder.Framework.Core.Interfaces;
using System;

namespace Elder.Framework.UI.Interfaces
{
    public interface IUIPresenter : IDisposable
    {
        public void InjectProvider(IGameSystemProvider provider);
    }
}