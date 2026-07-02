using Cysharp.Threading.Tasks;

namespace Elder.Framework.UI.Interfaces
{
    // Presenter가 구현 — UISystem이 Show/Hide/Release 시점에 조건부 캐스팅으로 호출
    // View가 아닌 Presenter에서 Signal을 발행하는 진입점
    public interface IUIPresenterLifecycle
    {
        public UniTask OnViewShownAsync();
        public UniTask OnViewHiddenAsync();
        public UniTask OnViewReleasedAsync();
    }
}
