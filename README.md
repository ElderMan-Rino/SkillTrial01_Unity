-----

# Unity DDD Core Framework (SkillTrial01)

  

## 📖 프로젝트 개요 (Overview)

이 프로젝트는 **Unity 엔진의 강한 결합도(Coupling)를 해결**하고, 유지보수와 확장성이 뛰어난 게임을 개발하기 위해 설계된 **DDD(Domain-Driven Design) 기반의 C\# 프레임워크**입니다.

기존 `MonoBehaviour` 중심의 스파게티 코드를 지양하고, **Model - Application - Infrastructure** 계층을 명확히 분리하여 비즈니스 로직의 순수성을 보장하고 테스트 용이성을 확보하는 데 중점을 두었습니다.

### 🎯 핵심 목표 (Key Objectives)

  * **DDD (도메인 주도 설계) 적용:** 비즈니스 로직(`Application`)과 기술적 구현(`Infrastructure`)을 철저히 분리하여 의존성 역전 원칙(DIP)을 실현했습니다.
  * **Flux 패턴 도입:** 데이터 흐름의 단방향성을 보장하고, 이벤트 기반의 느슨한 결합(Decoupling)을 구현했습니다.
  * **모듈화된 시스템:** 각 기능(Scene, Sound, UI, Logging 등)을 독립적인 Application 단위로 관리하여 유연한 확장이 가능합니다.
  * **자체적인 생명주기 관리:** Unity의 `Awake` / `Start` / `Update` 이벤트에 직접 의존하지 않고, 프레임워크 내부에서 제어되는 정교한 Lifecycle을 가집니다.

-----

## 🏗️ 아키텍처 (Architecture)

이 프레임워크는 **Layered Architecture**를 따르며, 다음과 같이 역할을 분담합니다.

| Layer | Role | Components | Feature |
| :--- | :--- | :--- | :--- |
| **Presentation** | Unity 엔진과 사용자의 상호작용 담당 | `CoreFrameRunner` (MonoBehaviour), `Bootstrapper` | 로직을 직접 수행하지 않고 Application 계층으로 위임 |
| **Application** | 게임의 핵심 비즈니스 로직 수행 (순수 C\#) | `CoreFrameApplication`, `FluxRouter`, `MainLevelApplication` | `UnityEngine`에 직접 의존하지 않으며 인터페이스를 통해 인프라 사용 |
| **Infrastructure** | 실제 기술적 구현 담당 (리소스, 씬, 로그 등) | `CoreFrameInfrastructure`, `SceneLoader`, `UnityLogAdapter` | Application 계층의 인터페이스(`IInfrastructure`)를 구현하여 주입 |
| **Domain/Model** | 시스템 데이터 구조 및 규칙 정의 | `LogEvent`, `IFluxMessage`, `GameLevelConstants` | 모든 계층에서 공유되는 데이터 및 인터페이스 |

-----

## 🛠️ 핵심 시스템 (Core Systems)

### 1\. CoreFrame (프레임워크 코어)

게임의 진입점(Entry Point)이자 전체 생명주기를 관리하는 컨테이너입니다.

  * **Bootstrapper:** `[RuntimeInitializeOnLoadMethod]`를 사용하여 별도의 씬 설정 없이도 게임 시작 시 자동으로 프레임워크를 구동합니다.
  * **DI Container:** `CoreFrameApplication`이 내부적으로 하위 앱(`PersistentApps`, `SceneApps`)과 인프라(`Infrastructure`)의 의존성을 주입(Injection)하고 관리합니다.

<!-- end list -->

```csharp
// Bootstrapper.cs
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
public static void Entry()
{
    // 게임 시작 시 프레임워크 자동 초기화 및 실행
    CreateCoreFrameRunner();
}
```

### 2\. Flux Messaging System (이벤트 통신)

컴포넌트 간의 직접적인 참조를 없애기 위해 **Flux 패턴**을 적용한 메시지 라우터입니다.

  * **단방향 흐름:** `Publish` (Action) $\rightarrow$ `Router` (Dispatcher) $\rightarrow$ `Handler` (Store) $\rightarrow$ Logic 실행
  * **FluxRouter:** 구조체(`struct`) 기반의 메시지 타입을 사용하여 가비지 컬렉션(GC) 부하를 최소화했습니다.

<!-- end list -->

```csharp
// 메시지 발행 (Publish)
_fluxRouter.Publish(new FxRequestMainLevelChange(GameLevelConstants.LOBBY_SCENE));

// 메시지 구독 (Subscribe)
_fluxRouter.Subscribe<FxRequestMainLevelChange>(OnLevelChangeRequested, FluxPhase.Domain);
```

### 3\. Level Management (씬 관리)

Unity의 `SceneManager`를 직접 사용하지 않고, 래핑된 인프라를 통해 안전하게 씬을 전환합니다.

  * **MainLevelApplication:** 씬 전환 요청을 받아 로딩 화면 표시, 기존 리소스 해제, 비동기 로딩 대기 등의 전체 흐름을 제어합니다.
  * **SceneLoader (Infra):** 실제 `SceneManager` API를 호출하거나 Addressable 로딩을 수행하는 구현체입니다.

### 4\. Logging System (로그 시스템)

개발 환경(Unity Editor)과 배포 환경(File, Server)에 따라 유연하게 로그를 남길 수 있도록 추상화했습니다.

  * **LogFacade:** 어디서든 접근 가능한 정적 진입점으로, 코드 곳곳에서 `Debug.Log` 대신 사용됩니다.
  * **LogApplication & UnityLogAdapter:** 실제 Unity 콘솔이나 파일로 로그를 출력하는 구현부입니다.

-----

## 📂 폴더 구조 (Directory Structure)

```text
Assets/Scripts
├── Core                    # [Application/Domain Layer] 순수 C# 로직 (Unity 의존성 없음)
│   ├── CoreFrame           # 프레임워크 메인 로직 (Application LifeCycle)
│   ├── FluxMessage         # 메시징 시스템 (Flux Pattern)
│   ├── GameLevel           # 레벨(씬) 관리 로직
│   └── Logging             # 로그 시스템 인터페이스 및 로직
│
└── Platform                # [Infrastructure/Presentation Layer] Unity 종속 구현
    ├── Bootstrappers       # 게임 시작 진입점 (Entry Point)
    ├── CoreFrame           # CoreFrame의 Unity 구현체 (Runner, Initializer)
    └── Logging             # Unity Console 로그 어댑터 등
```

-----

## 💡 개발 환경 및 컨벤션 (Environment & Convention)

  * **Language:** C\# 9.0+
  * **Engine:** Unity 2022.3 LTS
  * **Code Style:** Allman Style (Braces on new line)
      * `private int _variable;` (Private fields: `_` + camelCase)
      * `public int Variable { get; set; }` (Public properties/methods: PascalCase)
      * `void Method(int variable)` (Parameters/Local variables: camelCase)

-----

### 📝 Note

이 프로젝트는 대규모 게임 개발 시 발생할 수 있는 스파게티 코드 문제를 방지하고, **O.C.P (개방-폐쇄 원칙)** 및 \*\*D.I.P (의존성 역전 원칙)\*\*를 준수하여 오랫동안 유지보수 가능한 아키텍처를 구축하기 위해 제작되었습니다.
