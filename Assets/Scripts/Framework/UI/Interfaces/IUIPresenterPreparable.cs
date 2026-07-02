using Cysharp.Threading.Tasks;

namespace Elder.Framework.UI.Interfaces
{
    // Presenter가 구현 — UISystem.PrepareAsync<TView>() 호출 시점에 에셋 로드 등 준비 작업 실행
    public interface IUIPresenterPreparable
    {
        public UniTask PrepareAsync();
    }
}
