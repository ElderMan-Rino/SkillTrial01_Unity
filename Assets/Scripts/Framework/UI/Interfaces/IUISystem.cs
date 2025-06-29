using Elder.Framework.Core.Interfaces;
using Elder.Framework.UI.Loading.Definitions;
using Elder.Framework.UI.Loading.Interfaces;

namespace Elder.Framework.UI.Interfaces
{
    public interface IUISystem : IGameSystem
    {
        public ILoadingReporter ShowLoading(bool showBackground = true, LoadingVisualConfig visualConfig = default);
        public void HideLoading();
        public void TickLoading(float deltaTime);
    }
}
