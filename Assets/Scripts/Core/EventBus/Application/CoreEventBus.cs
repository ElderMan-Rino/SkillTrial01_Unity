public class CoreEventBus 
{
    /*
     * ? ��� ���
 �ý��� �̸�	��ġ/�Ҽ�	å�� ���� (DDD ����)
 CoreEventBus	Core Layer	? Application Layer (�Ϲ�������)
 RuntimeEventBus	Infrastructure Layer	? Infrastructure Layer (�Ǵ� Presentation)

 ?? ���� ����
 ?? 1. CoreEventBus: Core/Application Layer�� ���ؾ� �ϴ� ����
 ����	����
 ? ���� ����ϳ���?	������ ��ü (Emit), Application ���� (Publish/Subscribe)
 ? � �̺�Ʈ?	DomainEvent, ApplicationEvent �� ����Ͻ� �߽�
 ? Unity ����?	? ���� ���� (���� C# ��ü ���)
 ? ���	Application Layer �Ǵ� Core Layer�� ��ġ

 CoreEventBus�� �����ΰ� ���ø����̼� ������ �޽����� �����ϴ� �߽����� �ý����̹Ƿ�,
 �ݵ�� Unity�� �����ϰ� �����ϰ� ����Ǿ�� �ϰ�,
 �Ϲ������δ� Application Layer�� å���Դϴ�.

 ?? 2. RuntimeEventBus: Infrastructure Layer�� ���ؾ� �ϴ� ����
 ����	����
 ? ���� ����ϳ���?	Unity ������Ʈ, UI, ������Ʈ, MonoBehaviour
 ? � �̺�Ʈ?	OnHpChanged, OnAnimationEnd, OnClicked ��
 ? Unity ����?	? ���� (MonoBehaviour, GameObject ��)
 ? ���	Infrastructure Layer (Unity Adapter ����)

 RuntimeEventBus�� Unity ��Ÿ�� ��ü�� ���� Ŀ�´����̼��� ����ϹǷ�,
 Unity ����������Ŭ �� MonoBehaviour ��ݰ� �����ϰ� ����Ǿ�� �ϸ�,
 ���� Infrastructure �Ǵ� Presentation ������ ���ԵǾ�� �մϴ�.

 ?? ���� ���� (���� �� å�� ����)
 markdown
 ����
 ����
 /Core
   /Domain
     - IEvent (��Ŀ �������̽�)
     - DomainEventBase
   /Application
     - CoreEventBus.cs ?
     - ICoreEventBus.cs ?
     - IDomainEventHandler<T> ?

 /Infrastructure
   /Unity
     - RuntimeEventBus.cs ?
     - UnityLogHandler.cs
     - HpBarView.cs
 ? ��� ����
 �ý���	�Ҽ� ����	����
 CoreEventBus	? Application Layer (Core)	����Ͻ� �̺�Ʈ ���� / Unity ����
 RuntimeEventBus	? Infrastructure Layer (Unity ��Ÿ��)	MonoBehaviour / UI ������

 �� �� ������ ������ ���� �� ħ�� ����,
 ������/���ø����̼��� ����Ͻ� ������ Unity ��Ÿ�� �ý����� ��Ȯ�ϰ� �и��� �� �ֽ��ϴ�.
     */
}
