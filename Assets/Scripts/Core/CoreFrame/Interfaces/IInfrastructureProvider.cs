using Elder.Core.Common.Interfaces;

namespace Elder.Core.CoreFrame.Interfaces
{
    public interface IInfrastructureProvider
    {
        public bool TryGetInfrastructure<T>(out T targetInfrastructure) where T : class, IInfrastructure;
        /*
        �� ������Ʈ��ó��** ���������� �ٸ� ������(�������̽�)**�� ����ϴ� ���� ���˴ϴ�.
           ��, IInfrastructureProvider�� �����ϴ� ���� ��� ���� �� ������ ���̸� ���� �����ϴ�.
           ������ base�� IInfrastructureProvider�� ����
        */
    }
}
