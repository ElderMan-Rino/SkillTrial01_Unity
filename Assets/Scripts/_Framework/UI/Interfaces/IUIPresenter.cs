using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using System;

namespace Elder.Framework.UI.Interfaces
{
    // View의 Observable을 구독해 도메인 서비스로 중계하는 계약. View와 달리 MonoBehaviour가 아니므로 DI로 생성/주입.
    public interface IUIPresenter : IDisposable, IDependencyInjectable
    {
        public IUIViewModel ViewModel { get; }

        public void BindView(IUIView view);

        // 스택 Pop 직전 호출. false 반환 시 닫힘을 취소한다.
        public UniTask<bool> RequestCloseAsync();
    }
}
