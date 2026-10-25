using Cysharp.Threading.Tasks;
using Elder.Framework.UI.App;

namespace Elder.Framework.UI.Interfaces
{
    // Coordinator가 Open 시점에 구성하는 View+Presenter+ViewModel 조합 등록 계약.
    // UISystem은 이 Handle만으로 Stack 관리/Sleep/Wake를 수행하며, 어떤 View/Presenter 타입인지 내부적으로 추론하지 않는다.
    public interface IUIEntryHandle
    {
        public IUIViewHandle ViewHandle { get; }

        public UIPresenterBase Presenter { get; }

        public IUIViewModel ViewModel { get; }

        public void Sleep();

        public void Wake(IUIViewHandle viewHandle);

        // entry 자신의 Close 시도(내부 패널 우선 닫힘, 종료 확인 등)를 수행하고 실제 스택 Pop 여부를 반환.
        public UniTask<bool> RequestCloseAsync();
    }
}
