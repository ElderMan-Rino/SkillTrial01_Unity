using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.TestModule.UITest.Interfaces
{
    public interface IUITestCoordinator : IFrameworkComponent
    {
        public UniTask StartAsync();
        public UniTask OpenPanelAsync();
        public UniTask OpenPopupAsync();
        public UniTask CloseAllAsync();
        public UniTask CloseTopAsync();
    }
}