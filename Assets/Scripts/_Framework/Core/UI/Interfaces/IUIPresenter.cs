using Elder.Framework.Core.Interfaces;
using System;

namespace Elder.Framework.Core.UI.Interfaces
{
    // View의 Observable을 구독해 도메인 서비스로 중계하는 계약. View와 달리 MonoBehaviour가 아니므로 DI로 생성/주입.
    public interface IUIPresenter : IDisposable, IDependencyInjectable
    {
        public void BindView(IUIView view);
    }
}
