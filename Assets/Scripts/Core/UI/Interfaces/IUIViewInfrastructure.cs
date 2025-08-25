using Elder.Core.Common.Interfaces;

namespace Elder.Core.UI.Interfaces
{
    public interface IUIViewInfrastructure : IInfrastructure
    {
        // view 생성 요청
        // view 서치 요청 (씬에 포함되어 있는 view들을 등록)
        public void RegisterViews();
    }
}