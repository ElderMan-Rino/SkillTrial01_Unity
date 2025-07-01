public class CoreEventBus 
{
    /*
     * ? 결론 요약
 시스템 이름	위치/소속	책임 계층 (DDD 기준)
 CoreEventBus	Core Layer	? Application Layer (일반적으로)
 RuntimeEventBus	Infrastructure Layer	? Infrastructure Layer (또는 Presentation)

 ?? 세부 설명
 ?? 1. CoreEventBus: Core/Application Layer에 속해야 하는 이유
 기준	이유
 ? 누가 사용하나요?	도메인 객체 (Emit), Application 서비스 (Publish/Subscribe)
 ? 어떤 이벤트?	DomainEvent, ApplicationEvent 등 비즈니스 중심
 ? Unity 의존?	? 전혀 없음 (순수 C# 객체 기반)
 ? 결론	Application Layer 또는 Core Layer에 위치

 CoreEventBus는 도메인과 애플리케이션 사이의 메시지를 전달하는 중심적인 시스템이므로,
 반드시 Unity와 무관하게 순수하게 설계되어야 하고,
 일반적으로는 Application Layer의 책임입니다.

 ?? 2. RuntimeEventBus: Infrastructure Layer에 속해야 하는 이유
 기준	이유
 ? 누가 사용하나요?	Unity 컴포넌트, UI, 오브젝트, MonoBehaviour
 ? 어떤 이벤트?	OnHpChanged, OnAnimationEnd, OnClicked 등
 ? Unity 의존?	? 있음 (MonoBehaviour, GameObject 등)
 ? 결론	Infrastructure Layer (Unity Adapter 계층)

 RuntimeEventBus는 Unity 런타임 객체들 간의 커뮤니케이션을 담당하므로,
 Unity 라이프사이클 및 MonoBehaviour 기반과 밀접하게 연결되어야 하며,
 따라서 Infrastructure 또는 Presentation 계층에 포함되어야 합니다.

 ?? 구성 예시 (폴더 및 책임 기준)
 markdown
 복사
 편집
 /Core
   /Domain
     - IEvent (마커 인터페이스)
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
 ? 결론 정리
 시스템	소속 계층	이유
 CoreEventBus	? Application Layer (Core)	비즈니스 이벤트 전달 / Unity 무관
 RuntimeEventBus	? Infrastructure Layer (Unity 런타임)	MonoBehaviour / UI 반응용

 → 이 기준을 따르면 계층 간 침범 없이,
 도메인/애플리케이션의 비즈니스 로직과 Unity 런타임 시스템을 명확하게 분리할 수 있습니다.
     */
}
