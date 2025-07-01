using Elder.Core.Common.Interfaces;

namespace Elder.Core.CoreFrame.Interfaces
{
    public interface IApplicationProvider 
    {
        public bool TryGetApplication<T>(out T targetApplication) where T : IApplication;

        /*
         * �������� Best Practice
         * �б�/���ǿ����� IApplicationProvider�� ���
         * �� ��: �ٸ� ���� ���³� ����� �޾ƿ;� �� ��

         * ����/���� ������ �޽��� �Ǵ� Mediator/EventBus ��
         * �� ��: "A�� ������ B�� ����ž� �Ѵ�" ���� �帧�� �����ϰ� ����

         * Application �� ������ ���� �������� ��
         * �� ��: ���� Application�� ���� ���� Application�� ����/����
         */
    }
}