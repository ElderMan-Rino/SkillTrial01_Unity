using Elder.Core.Common.Interfaces;

namespace Elder.Core.UI.Interfaces
{
    public interface IUIViewInfrastructure : IInfrastructure
    {
        // view ���� ��û
        // view ��ġ ��û (���� ���ԵǾ� �ִ� view���� ���)
        public void RegisterViews();
    }
}