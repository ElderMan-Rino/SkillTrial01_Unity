using System;

namespace Elder.Framework.Core.UI.Interfaces
{
    // Presenter가 소유하는 화면 상태 보관 계약. View Instance가 Sleep(Destroy)되어도
    // Presenter와 함께 유지되어, Wake 시 재생성된 View에 그대로 재바인딩된다.
    public interface IUIViewModel : IDisposable
    {
    }
}
