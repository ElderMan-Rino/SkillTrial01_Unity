using Cysharp.Threading.Tasks;
using Elder.Framework.Boot.App;
using Elder.Framework.Boot.Interfaces;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.GameSystem.App;
using Elder.Framework.Composition;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Installer;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.MainFrame.Infra.Configs;
using Elder.Framework.MainFrame.Installer;
using Elder.SkillTrial.Data.Installer;
using System;
using UnityEngine;

namespace Elder.Framework.MainFrame.Infra
{
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
    // ├─ 2. Initialize (프레임워크 데이터 초기화)
    // │     역할: 마스터 데이터(Blob), 암호화 키, 로케일, 설정 로드
    // │     필요 구현:
    // │       - InitializeGameModeOrchestrator : GameModeOrchestrator
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
    public sealed class FrameworkRoot : MonoBehaviour
    {
        private IGameSystemProvider _provider;

        public void Initialize(FrameworkSettings settings)
        {
            DontDestroyOnLoad(gameObject);
            _provider = BuildRegistry(settings).Build();
            BootProvider(_provider);
        }

        private SystemRegistry BuildRegistry(FrameworkSettings settings)
        {
            var registry = new SystemRegistry();
            new GameInstaller().Install(registry);
            new FrameworkInstaller().Install(registry, settings);
            return registry;
        }

        private void BootProvider(IGameSystemProvider provider)
        {
            BootAsync(provider).Forget();
        }
        
        private async UniTask BootAsync(IGameSystemProvider provider)
        {
            if (provider.TryGetSystem<ILoggerPublisher>(out var publisher))
                LogFacade.InjectProvider(publisher);

            provider.InjectAll();
            await provider.InitializeAllAsync();
            await provider.PostInitializeAllAsync();

            if (provider.TryGetSystem<IGameBootEntryPoint>(out var startService))
                startService.Run();
        }

        private void OnDestroy()
        {
            if (_provider is IDisposable disposable) disposable.Dispose();
            LogFacade.CleanUp();
        }
    }
}
