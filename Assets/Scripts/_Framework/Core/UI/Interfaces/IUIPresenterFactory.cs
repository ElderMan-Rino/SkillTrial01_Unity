using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Core.UI.Interfaces
{
    // TPresenter를 생성하고 의존성을 주입하는 계약. 타입 식별은 typeof(TPresenter)로 충분하므로 Id 불필요.
    public interface IUIPresenterFactory : IFrameworkComponent
    {
        public TPresenter Create<TPresenter>() where TPresenter : IUIPresenter, new();
    }
}
