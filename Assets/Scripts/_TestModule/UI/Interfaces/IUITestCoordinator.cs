using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.TestModule.UI.Interfaces
{
    public interface IUITestCoordinator : IFrameworkComponent
    {
        public UniTask StartAsync();
    }
}