using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.UI.Interfaces
{
    public interface IUICoordinator : ISystemComponent
    {
        public UniTask PrepareAsync();
        public UniTask ShowAsync();
        public UniTask HideAsync();
    }
}
