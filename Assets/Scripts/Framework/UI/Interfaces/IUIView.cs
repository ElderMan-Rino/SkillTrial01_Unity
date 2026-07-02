using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.UI.Interfaces
{
    // View는 ViewModel만 알고 Presenter는 모름 (MVPVM 규칙)
    // Refresh()는 데이터를 받지 않음 — ViewModel.Snapshot을 직접 Pull
    public interface IUIView : ISystemComponent
    {
        public void Show();
        public void Hide();
        public void Refresh();
    }
}
