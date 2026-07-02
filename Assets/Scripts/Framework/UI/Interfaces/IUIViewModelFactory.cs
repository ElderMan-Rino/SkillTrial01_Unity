using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.UI.Interfaces
{
    // ViewModel은 TSnapshot(readonly struct)을 보유 — 상태 스냅샷 단일 진실 공급원
    public interface IUIViewModelFactory : ISystemComponent
    {
        public TViewModel Create<TViewModel>() where TViewModel : class;
        public void Release<TViewModel>() where TViewModel : class;
    }
}
