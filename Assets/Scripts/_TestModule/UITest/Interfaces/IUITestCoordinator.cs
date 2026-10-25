using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.TestModule.UITest.Interfaces
{
    public interface IUITestCoordinator : IFrameworkComponent
    {
        public UniTask StartAsync();
        public UniTask OpenTestPanelAsync();
        public UniTask OpenTestPopupAsync();
    }
}