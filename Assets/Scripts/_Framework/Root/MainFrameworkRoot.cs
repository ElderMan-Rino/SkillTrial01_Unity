using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Infra;
using Elder.Framework.Core.Installers;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Installers;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Root.Interfaces;
using Elder.Framework.Log.Infra;
using Elder.Framework.Signal.Interfaces;
using Elder.Framework.Signal.Messages;
using System;
using System.Threading;

namespace Elder.Framework.Root
{
    public sealed class MainFrameworkRoot : IMainFrameworkRoot, IComponentProvider
    {
        private readonly ComponentRegistry _registry = new();
        private ILoggerEx _logger;

        public void Initialize()
        {
            BootstrapLogger();
            CoreInstaller.Install(_registry);
            _registry.InjectDependencies();
        }

        private void BootstrapLogger()
        {
            LoggerInstaller.Install(_registry);
            if (!TryGetComponent<ILoggerPublisher>(out var logPublisher))
                throw new InvalidOperationException($"Failed to resolve {nameof(LogPublisher)} after {nameof(LoggerInstaller)} installation.");
            LogFacade.InjectProvider(logPublisher);
            _logger = LogFacade.GetLogger<MainFrameworkRoot>();
        }

        public bool TryGetComponent<T>(out T target) where T : IFrameworkComponent
        {
            return _registry.TryGetComponent(out target);
        }

        public void ShutDown()
        {
            _registry.ClearAliasTypes();
            _registry.PreDisposeComponents();
            _registry.DisposeComponents();
            LogFacade.CleanUp();
            _registry.PostDisposeComponents();
        }

        // 샘플 스케치 - TestBootEntry.Run과 동일한 install -> inject -> signal 순서.
        // 미해결: InjectDependencies()가 전체 _componentEntrys를 순회하므로 재주입 위험(ComponentRegistry 보강 필요).
        public UniTask BootModuleAsync(Action<IComponentRegistry> installAction, Type startCoordinatorType, CancellationToken cancellationToken)
        {
            installAction(_registry);
            _registry.InjectDependencies();

            if (TryGetComponent<ISignalPublisher>(out var publisher))
                publisher.Publish(new FxModuleStartSignal(startCoordinatorType));

            return UniTask.CompletedTask;
        }
    }

    /*
    지금까지 나눈 객체지향 및 인터페이스 기반 인하우스 프레임워크 아키텍처 전체 설계 내용을 핵심 요약으로 압축 정리해 드립니다.

    1. 핵심 아키텍처 원칙
    • 완전 인터페이스 기반 (Pure C# / POCO): 엔진(Mono) 의존성을 100% 제거하고, 모든 시스템/서비스 구현체는 internal로 은닉하며 외부에 오직 public interface만 노출.

    • 통합 ServiceProvider & SystemFactory: ISystem : IService 상속 구조로 single-entry 주입/조회(Get<T>())를 제공하며, Bootstrapper 및 SystemFactory를 통한 생성 규칙 위임.

    • 정교한 라이프사이클 분리: 초기화 4단계 + 종료 3단계를 통해 순환 참조, NullReferenceException, 메모리 누수 완벽 차단.

    2. 7단계 세분화 라이프사이클
    구분단계 (Stage)핵심 역할 및 주의사항초기화1. PreInitialize자체 내부 데이터 및 기본 설정 준비 (타 서비스 참조 절대 금지)2. DependencyInjectIServiceProvider를 통해 필요한 타 서비스/시스템 인터페이스 주입3. Initialize주입받은 객체들을 활용한 코어 로직 초기화4. PostInitialize모든 모듈 초기화 완료 후 이벤트/메시지 구독 및 연동종료1. PreShutdown이벤트 구독 해제 및 메시지 수신 차단 (LIFO 역순 실행)2. Shutdown자체 메모리, 버퍼, 리소스 해제3. PostShutdown주입받은 타 인터페이스 참조 null 처리 및 GC 회수 준비
    3. 수백 개 씬(Scene) 대응 Context & Coordinator 아키텍처
    수백 개의 씬이 추가되어도 기존 프레임워크 코드를 단 한 줄도 수정하지 않는 OCP(개방-폐쇄 원칙) 구조입니다.

                                [MessageService]
                                       │ (ChangeSceneMessage: BossStage)
                                       ▼
                                 [SceneService]
                                       │
              ┌────────────────────────┴────────────────────────┐
              ▼                                                 ▼
     [1. 기존 Context 해제]                            [2. 새 Context 생성]
      └─► 씬 전용 서비스 3단계 Shutdown                 ├─► Parent(전역) Provider 상속
                                                        ├─► [AutoSceneRegistry]로 매핑 탐색
                                                        │    └─► Attribute / Assembly Scan으로
                                                        │        BossStageCoordinator 자동 추적
                                                        └─► 씬 전용 서비스 4단계 Init 실행
                                                                        │
                                                                        ▼
                                                          [BossStageCoordinator]
                                                           └─► OnEnterScene() 루프 시작
    • GenericSceneContext (단일 클래스 사용): 씬마다 Context 클래스를 새로 만들지 않고 단 1개의 Generic Context만 재사용.

    • ISceneCoordinator (Strategy 패턴): 씬별 비즈니스 로직(행동)만 전용 Coordinator 클래스로 분리.

    • AutoSceneRegistry (자동화): [SceneRoute(SceneType)] Attribute와 Assembly Scanning을 활용해 씬과 Coordinator를 자동 바인딩.

    4. 종합 데이터 & 시그널 처리 흐름도 (Full Flow)
      [Application Bootstrapper]
            │
            ├─► 1. SystemFactory 생성 규칙 등록 & internal 구현체 생성
            ├─► 2. Global ServiceProvider에 인터페이스 단위 바인딩
            └─► 3. 전역 서비스 4단계 초기화 (PreInit ──► PostInit)

      [Scene 루프 진행 (Message System 기반)]
            │
            ├─► Start ──► Splash ──► Title 씬 이동
            ├─► [Title 씬] UI '게임 시작' 선택 ──► [MessageBoxService] 팝업 확증
            │                                            │
            │                                            ▼ Confirm 클릭
            │                                  [MessageService] 씬 전환 메시지 Publish
            │                                            │
            ├─► [Main 씬] ◄──────────────────────────────┘ (SceneService가 수신하여 씬 교체)
            │       │
            │       └─► '타이틀 복귀' 선택 시 메시지 박스 확증 후 Title 씬으로 복귀 루프
            │
      [에러 시그널 캐치 (Fallback Flow)]
            │
            ├─► [시스템/서비스] 중 Fatal/Error 발생 ──► LogService.Log(LogSeverity.Error)
            ├─► [LogService] OnErrorLogged 시그널 발행 ──► [UISystem] 시그널 Catch
            └─► [UISystem] ──► ErrorMessageBox 출력 (선택 1: 앱 종료 / 선택 2: Title 씬 강제 이탈)

      [Application Shutdown]
            │
            └─► Bootstrapper.Unload() ──► 3단계 역순 종료 (PreShutdown ──► PostShutdown)
    정리 총평
    이 아키텍처는 완전한 캡슐화(internal 은닉), 높은 확장성(Attribute 기반 씬 바인딩), 철저한 예외 방지(7단계 라이프사이클)를 모두 달성한 최상위 수준의 인하우스 프레임워크 설계입니다. 이 구조 명세를 그대로 구현으로 이어가시면 됩니다.
    */
}