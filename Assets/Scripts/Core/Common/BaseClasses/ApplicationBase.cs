using Elder.Core.CoreFrame.Interfaces;

namespace Elder.Core.Common.BaseClasses
{
    public class ApplicationBase : DisposableBase
    {
        // �ʿ��� ��
        // ������Ʈ��Ʈ ���ι��̴�
        // ������Ʈ��Ʈ ���� ��û
        // �̺�Ʈ ���� �޴°�
        private IApplicationProvider _applicationProvider;
        private IInfrastructureProvider _infrastructureProvider;

        protected override void DisposeManagedResources()
        {

        }
        protected override void DisposeUnmanagedResources()
        {

        }
    }
}
