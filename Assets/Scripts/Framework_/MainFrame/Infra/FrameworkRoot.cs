/*
using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Blob.Definitions;
using Elder.Framework.Blob.Infra.Extensions;
using Elder.Framework.Blob.Interfaces;
using Elder.Framework.Boot.Definitions;
using Elder.Framework.Boot.Interfaces;
using Elder.Framework.Common.Utils;
using Elder.Framework.Composition;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.GameSystem.App;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Installer;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.MainFrame.Infra.Configs;
using Elder.Framework.MainFrame.Installer;
using Elder.SkillTrial.Data.Installer;
using Elder.SkillTrial.Resources.Data;
using System;
using UnityEngine;

namespace Elder.Framework.MainFrame.Infra
{
    // ──────────────────────────────────────────────────────────────────────
    // [PROJECT GOAL] 프레임워크 방향성 — 멀티 게임 컨테이너
    //
    // 64-in-1 미니게임 합본 팩 → SRPG(BG3 스타일) 테스트베드로 확장 예정.
    // 공통 레이어: Shell → GameHost(IGameEntry/IGameRule/IGameSession) → Game.{Id}
    // Addressables 라벨: shell / game-hub / game-{id} / game-shared
    // ECS World는 게임별 격리, 공통 시스템(Actor/Effect/Physics/Timer/Camera)은 ISystem(Burst)+IGameWorldProvider로 브릿지.
    // 데이터: Blob(BlobAssetReference, 마스터·읽기전용) vs IUserRepository(MessagePack+AES, 가변·유저 데이터) 이원화, DualWrite+PendingSync로 동기화.
    // Input: IInputProvider(Infra)/IInputContext(Domain)/IInputPlatformAdapter(Platform) 3계층, 멀티플랫폼 대응.
    // UI: MVPVM + Coordinator, 단방향 흐름(Model→Presenter→ViewModel→View), Signal Bus는 Model만 수신.
    // Signal: 이벤트/흐름 제어(단방향 pub-sub) 담당, R3는 상태 동기화(reactive stream) 담당으로 역할 분리.
    //   AOT(IL2CPP) 코드 폭발 방지 3전략: ①타입 이레이저(void*+TypeId, 참조타입 불가) ②정적 제네릭 캐시 ③클래스 풀링(스코프 기반 파괴).
    //   Signal 구조체는 64Byte 이하로 제한.
    // GameMode: GameModeOrchestrator 단일 베이스 + 모듈 합성(IFlowModule/IUICoordinator), IGameSystemRegistry로 시스템 등록/해제.
    //
    // [PROJECT GOAL] SRPG 전환 목표 시스템 (2026-09-01 확정, 64-in-1 이후 로드맵)
    // 확정: 네비메시 지형, 액터 시스템, 조작 시스템, 저장/로드, 퀘스트, 업적, 시나리오 진행, 다이얼로그, 카메라 시스템(컷씬 포함).
    // 그리드 좌표계는 명시적으로 배제 — BG3 스타일 자유 이동, 사거리/범위는 네비메시 위 반경(radius) 계산으로 대체.
    // 그리드 배제로 파생되는 필요 시스템: 커버/고저차, 턴 오더/이니셔티브, 스킬/어빌리티(범위·직선 타겟팅+프리캐스팅 프리뷰),
    //   상태이상/버프-디버프, 파티/로스터 편성, 인벤토리/장비, AI(적 행동 결정).
    // 서브모드 전환은 ISubModeStack(Push/Pop)+ISubModeContext로 일반화 — 메인 모드(전투/탐험)는 파괴되지 않고 스택에 유지, 기존 64-in-1 미니게임도 ISubModeContext 구현체로 편입.
    // 마일스톤: Q3 Input계층+GameFlow골격 → Q3후반 World레이어 → Q4초 턴제전투 프로토타입 → Q4중 SubMode전환검증 → Q4후반 에디터툴+통합테스트.
    //
    // ❌ VIOLATION: bootStrapper.Start() 주석 처리 — 부트스트랩 진입점 비활성화 (GameStartService 미등록과 연계)
    //
    // ──────────────────────────────────────────────────────────────────────
    // [MISSING] 목표 게임 플로우 전체 설계 — 현재 미구현 목록
    //
    // 목표 흐름:
    //   Splash → Initialize → Start → Main → Start → Main (루프)
    //
    // ┌─ 1. Splash (앱 최초 진입)
    // │     역할: 로고/로딩 UI 표시, 에셋 번들/Addressables 카탈로그 초기화
    // │     필요 구현:
    // │       - SplashGameModeOrchestrator : GameModeOrchestrator
    // │           → TryPrepareAsync(): 카탈로그 업데이트, 필수 에셋 프리로드
    // │           → TeardownAsync(): Splash UI 해제
    // │       - FrameworkInstaller에 SplashGameModeOrchestrator를 팩토리에 등록
    // │       - GameBootEntryPoint.Run()이 "Splash" SceneKey로 SceneTransitionSignal 발행
    // │         (현재는 "Initialize"로 바로 전환 — Splash 씬 없으면 생략 가능)
    // │
    // ├─ 2. Initialize(Preload) (프레임워크 데이터 초기화)
    // │     역할: 마스터 데이터(Blob), 암호화 키, 로케일, 설정 로드
    // │     필요 구현:
    // │       - PreloadOrchestrator : GameModeOrchestrator
    // │           → TryPrepareAsync():
    // │               1) IAssetSystem으로 마스터 데이터 에셋 로드
    // │               2) IBlobIndexRegistry로 Blob 등록
    // │               3) ILocaleSystem.ApplySettings() 호출
    // │               4) ISettingsStore 로드
    // │               5) 완료 후 "Start" SceneKey로 SceneTransitionSignal 발행
    // │           → TeardownAsync(): 로딩 UI 해제
    // │       - IGameBootEntryPoint.Run()이 "Initialize" SceneKey 발행 (현재 구현됨 ✅)
    // │       - TransitionDirector가 LoadOrchestratorAsync에서 InitializeOrchestrator 생성
    // │
    // ├─ 3. Start (게임 로비/메뉴 진입 준비)
    // │     역할: 유저 데이터 로드, 세션 초기화, 로비 UI 준비
    // │     필요 구현:
    // │       - StartGameModeOrchestrator : GameModeOrchestrator
    // │           → TryPrepareAsync():
    // │               1) IUserRepository로 유저 데이터 로드
    // │               2) 세션 상태 초기화
    // │               3) 완료 후 "Main" SceneKey로 SceneTransitionSignal 발행
    // │           → TeardownAsync(): 세션 데이터 정리
    // │
    // ├─ 4. Main (메인 게임 루프)
    // │     역할: 게임 플레이, UI 인터랙션, 게임 선택
    // │     필요 구현:
    // │       - MainGameModeOrchestrator : GameModeOrchestrator
    // │           → TryPrepareAsync(): 메인 씬 UI 바인딩, 게임 목록 표시
    // │           → TeardownAsync(): UI 해제, 진행 중 게임 세션 정리
    // │       - 게임 시작 이벤트 발생 시 "Start" SceneKey로 SceneTransitionSignal 재발행
    // │         → Start → Main 루프 구성
    // │
    // ├─ 5. Start → Main 루프 조건
    // │     - MainGameModeOrchestrator에서 "게임 시작" 액션 수신 시
    // │       _router.Publish(new SceneTransitionSignal("Start")) 발행
    // │     - StartGameModeOrchestrator가 유저 데이터/세션 재초기화 후
    // │       _router.Publish(new SceneTransitionSignal("Main")) 발행
    // │     → TransitionDirector.ProcessAsync가 이 루프를 자동으로 처리
    // │
    // └─ 6. 현재 미구현 / 연결 누락 목록
    //       - IGameModeOrchestratorFactory에 각 씬키별 Orchestrator 등록 없음
    //         → GameModeOrchestratorFactory.cs 구현 필요 (현재 NotImplementedException)
    //       - TransitionDirector가 GameMode Installer에 등록되지 않음
    //         → GameModeInstaller 신설 후 FrameworkInstaller에서 호출 필요
    //       - LoadOrchestratorAsync 완료 후 SceneTransitionCompletedSignal 미발행
    //         → TransitionDirector.ProcessAsync finally 블록에 Publish 추가 필요
    //       - ISignalRouter가 SignalInstaller에 등록되지 않음
    //         → SignalInstaller.Install()에 SignalRouter 등록 필요
    //       - GameBootEntryPoint가 BootStrapperInstaller에서 주석 처리로 미등록
    //         → BootStrapperInstaller 주석 해제 필요
    //       - FrameworkSettings가 ScriptableObject 기반 — MessagePack 구조체로 교체 필요
    //         → IBootConfig 구현체를 MessagePack 직렬화 + Addressables 바이너리 로드로 전환
    // ──────────────────────────────────────────────────────────────────────
    // [STATUS] 게임 시스템 완성도 실측 (2026-09-05 조사) — Framework 공용 vs 실게임(SRPG) 전용 분리
    //
    // ■ Framework 공용 계층 (모든 Game 프로젝트가 공유)
    //   UI ~45%      : UI 코어(UISystem/UICoordinatorBase/UIPresenterBase) 및 GameMode/Splash 전 계층 실동작.
    //                  MessageBox는 배선만 존재 — MessageBoxPresenter 대부분 빈 UniTask.CompletedTask,
    //                  OpenMessageBoxViewAsync는 NotImplementedException. View가 Presenter 경유 없이 직접 호출되어 MVPVM 단방향 원칙 위반.
    //   Input ~5%    : Input/Infra/Generated/GameInputActions.cs(자동생성)만 존재. asmdef/AssemblyInfo.cs 없음.
    //                  IInputProvider/IInputContext/IInputPlatformAdapter 3계층 전부 미구현(Domain/Platform 폴더 자체 없음).
    //   유저데이터 세이브/로드 ~35% : Blob/App/DataProvider.cs, Blob/Infra/EncryptedBlobDataDeserializer.cs(ArrayPool+GCHandle.Pinned+ZeroMemory),
    //                  Crypto/Infra/AesEncryptionProvider.cs(AES-256-CBC+HKDF+PKCS7) 모두 견고하게 실동작하나 마스터(읽기전용) 데이터 전용.
    //                  IUserRepository(가변 유저 데이터)는 설계 언급만 있고 파일 전무.
    //   사운드/그래픽 옵션 ~15% : Bgm/App/BGMSystem.cs는 Signal 배선만 있고 PlayAsync/Stop 전부 TODO(UniTask.CompletedTask만 반환).
    //                  Settings/App/SettingsApplyService.cs 골격은 동작하나 ISettingsApplicable 구현체는 LocaleSystem 하나뿐이고
    //                  TryGetString 실패 시 return으로 폴백 로직이 죽은 코드가 되는 버그 존재.
    //                  PlayerPrefsSettingsStore에 미사용 ISignalRouter 의존성 잔존(VIOLATION).
    //   → Framework 공용 계층 평균 완성도 약 25% 수준. 배선(DI+Signal 등록)은 되어 있으나 알맹이가 없는 패턴이
    //     MessageBox/BGM/Settings에서 반복 확인됨.
    //
    // ■ 실게임(SRPG) 전용 계층 (Game.{Id} 프로젝트별 구현 — Framework와 분리)
    //   Actor 생성 ~5% : LocomotionBridge/BlinkBridge/FaceAnimationBridge 등 6개 파일뿐인 껍데기 단계.
    //                  스폰/팩토리/스탯/ECS Blob 연동 전무.
    //   맵 ~5%       : World/ 폴더는 .gitkeep만 존재하는 빈 폴더. NavMesh 코드 전무.
    //                  Scene/App/SceneChanger.cs(Blob 기반 씬 전환, Framework 소속)는 동작하나 "맵" 자체와는 무관.
    //   퀘스트 0%     : 관련 코드 전무.
    //   업적 0%       : 관련 코드 전무.
    //   (미착수, 코드 없음) : 턴 오더/이니셔티브, 스킬/어빌리티, 상태이상/버프-디버프,
    //                  파티/로스터 편성, 인벤토리/장비, AI, 다이얼로그, 카메라(컷씬), 커버/고저차.
    //   → 실게임 계층 평균 완성도 약 3% 수준. SRPG 전환의 실질 작업량 대부분이 여기 집중.
    //
    // → 전체(Framework+실게임) 종합 완성도 약 5~10% 수준.
    //   우선순위: Framework는 Input 3계층부터, 실게임은 Actor/맵부터 착수 권장(둘 다 SRPG 전환의 선행 조건).
    // ──────────────────────────────────────────────────────────────────────
    public sealed class FrameworkRoot : MonoBehaviour
    {
        private IGameSystemProvider _provider;
        private ILoggerEx _logger;

        public void Initialize(FrameworkSettings settings, SceneBootMode bootMode)
        {
            Debug.Log("[TEMP] FrameworkRoot.Initialize 진입");
            DontDestroyOnLoad(gameObject);
            _provider = BuildRegistry(settings).Build();
            Debug.Log("[TEMP] Registry Build 완료");
            gameObject.AddComponent<FrameworkTickRelay>().Initialize(_provider);
            BootProvider(_provider, bootMode);
        }

        private SystemRegistry BuildRegistry(FrameworkSettings settings)
        {
            var registry = new SystemRegistry();
            new GameInstaller().Install(registry);
            new FrameworkInstaller().Install(registry, settings);
            return registry;
        }

        private void BootProvider(IGameSystemProvider provider, SceneBootMode bootMode)
        {
            BootAsync(provider, bootMode).Forget();
        }

        private async UniTask BootAsync(IGameSystemProvider provider, SceneBootMode bootMode)
        {
            Debug.Log("[TEMP] BootAsync 진입");
            if (provider.TryGetSystem<ILoggerPublisher>(out var publisher))
                LogFacade.InjectProvider(publisher);
            _logger = LogFacade.GetLoggerFor<FrameworkRoot>();

            provider.InjectAll();
            Debug.Log("[TEMP] InjectAll 완료");
            await provider.InitializeAllAsync();
            Debug.Log("[TEMP] InitializeAllAsync 완료");
            await provider.PostInitializeAllAsync();
            Debug.Log("[TEMP] PostInitializeAllAsync 완료");

            var preloadResult = await PreloadBootAssetsAsync(provider);
            Debug.Log($"[TEMP] PreloadBootAssetsAsync 결과={preloadResult}");
            if (!preloadResult)
            {
                _logger.Error($"{nameof(PreloadBootAssetsAsync)} failed — boot aborted.");
                return;
            }

            bool hasStartService = provider.TryGetSystem<IGameBootEntryPoint>(out var startService);
            Debug.Log($"[TEMP] IGameBootEntryPoint 조회={hasStartService}");
            if (hasStartService)
                startService.Run(bootMode);
        }

        private async UniTask<bool> PreloadBootAssetsAsync(IGameSystemProvider provider)
        {
            if (!provider.TryGetSystem<IDataProvider>(out var dataProvider))
            {
                _logger.Error($"{nameof(IDataProvider)} not found.");
                Debug.LogError($"[TEMP] {nameof(IDataProvider)} not found.");
                return false;
            }

            if (!dataProvider.TryGetBlobData<AssetInfoEntryBootRoot>(out var assetInfoEntrys))
            {
                _logger.Error($"{nameof(AssetInfoEntryBootRoot)} not found.");
                Debug.LogError($"[TEMP] {nameof(AssetInfoEntryBootRoot)} not found.");
                return false;
            }

            if (!provider.TryGetSystem<IAssetRegistry>(out var registry))
            {
                _logger.Error($"{nameof(IAssetRegistry)} not found.");
                Debug.LogError($"[TEMP] {nameof(IAssetRegistry)} not found.");
                return false;
            }

            int rowCount = assetInfoEntrys.Rows.Length;
            Debug.Log($"[TEMP][Preload] rowCount={rowCount}"); // [HEAP] 문자열 보간

            var tasks = new UniTask[rowCount]; // [HEAP] 1회 할당
            for (int i = 0; i < rowCount; i++)
            {
                string label = assetInfoEntrys.Rows[i].Label.ToString(); // [HEAP] BlobString.ToString()
                Debug.Log($"[TEMP][Preload] Row[{i}] Label='{label}' — RegisterAsync 호출"); // [HEAP] 문자열 보간
                tasks[i] = registry.RegisterAsync(label);
            }
            await UniTask.WhenAll(tasks); // [HEAP] WhenAll 내부 배열 참조
            Debug.Log("[TEMP][Preload] PreloadBootAssetsAsync 전체 완료");
            return true;
        }

        private void OnDestroy()
        {
            if (_provider is IDisposable disposable) disposable.Dispose();
            LogFacade.CleanUp();
        }
    }
}
*/
