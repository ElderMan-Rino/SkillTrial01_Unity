using Elder.Core.Common.Interfaces;

namespace Elder.Core.CoreFrame.Interfaces
{
    public interface IApplicationProvider 
    {
        public bool TryGetApplication<T>(out T targetApp) where T : class, IApplication;
        public bool TryGetApplications<T>(out T[] targetApps) where T : class, IApplication;

        /*
         * �������� Best Practice
         * �б�/���ǿ����� IApplicationProvider�� ���
         * �� ��: �ٸ� ���� ���³� ����� �޾ƿ;� �� ��

         * ����/���� ������ �޽��� �Ǵ� Mediator/EventBus ��
         * �� ��: "A�� ������ B�� ����ž� �Ѵ�" ���� �帧�� �����ϰ� ����

         * Application �� ������ ���� �������� ����
         * �� ��: ���� Application�� ���� ���� Application�� ����/����
         */
    }
}