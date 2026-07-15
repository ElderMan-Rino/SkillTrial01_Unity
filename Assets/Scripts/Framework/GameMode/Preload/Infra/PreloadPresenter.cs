using Cysharp.Threading.Tasks;
using Elder.Framework.GameMode.Preload.Definitions;
using Elder.Framework.GameMode.Preload.Interfaces;
using Elder.Framework.UI.App;
using Elder.Framework.UI.Interfaces;
using System;

namespace Elder.Framework.GameMode.Preload.Infra
{
    internal sealed class PreloadPresenter : UIPresenterBase, IPreloadPresenter
    {
        private IUISystem _uiSystem;
        private IPreloadViewModel _viewModel;

        protected override void OnInjectDependency()
        {
            if (!TryGetSystem<IUISystem>(out _uiSystem))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IUISystem)}");
        }

        public void SetViewModel(IPreloadViewModel viewModel) => _viewModel = viewModel;

        public UniTask PrepareAsync() => UniTask.CompletedTask;

        public UniTask OnViewShownAsync()
        {
            if (!_uiSystem.TryGetView<PreloadView>(out var view)) return UniTask.CompletedTask;
            view.Refresh(0f);
            return UniTask.CompletedTask;
        }

        public async UniTask LoadResourcesAsync()
        {
            // [Steam/콘솔/모바일 공통 - SRPG 전환 계획서 "[1. 공통]" 단계]
            // 플랫폼별 단계(패치 확인, 다운로드 등)는 범위 밖 - IAddressableLoadPolicy 등으로 별도 분기 예정
            // [Preload 제외 - 지연 로드]
            // - 개별 유닛 밸런스 인스턴스, 맵/스테이지 데이터
            // - 스킬 개별 VFX 시퀀스, AI 행동 패턴 상세
            // - 미니게임 본 콘텐츠(맵 구성/기믹 애셋)
            // [HEAP] 델리게이트 배열 + 메서드 그룹 변환 - 씬 진입 시 1회만 생성
            var stages = new Func<int, int, UniTask>[]
            {
                LoadCommonMasterBlobDataAsync,
                LoadCommonAddressableLabelsAsync,
                LoadInputBindingResourcesAsync,
                LoadWorldCommonResourcesAsync,
                PrepareBgmInitialClipsAsync,
                LoadUiCommonShellResourcesAsync,
            };

            for (int i = 0; i < stages.Length; i++)
            {
                await stages[i](i + 1, stages.Length);
            }
        }

        public UniTask OnViewHiddenAsync() => UniTask.CompletedTask;

        public UniTask OnViewReleasedAsync() => UniTask.CompletedTask;

        protected override void DisposeManagedResources()
        {
            _uiSystem = null;
        }

        // TODO: [Blob 마스터 데이터] IDataProvider 연동 예정
        // - SRPG 코어(전역 참조 테이블): UnitBaseTable, JobClassTable, SkillBaseTable,
        //   ElementAffinityTable, StatusEffectBaseTable, TerrainEffectTable
        // - 공통 시스템 참조: RarityTable, CurrencyTable, Enum 매핑 테이블(DataForge 산출물)
        // - 미니게임(규칙 정의만): MinigameRuleTable
        // - 테스트베드 전용: TestbedActorTable(스폰 가능 캐릭터/몬스터 목록),
        //   TestbedAIProfileTable(AI 파라미터 프리셋), TestbedSpawnPatternTable(대량 스폰 스트레스 패턴, 선택)
        private async UniTask LoadCommonMasterBlobDataAsync(int current, int total)
        {
            ReportProgress(current, total, "공통 마스터 데이터 로드 중");
            await UniTask.CompletedTask;
        }

        // TODO: [공통 Addressable 라벨] 프리로드 연동 예정
        // - UI 공통: Label_UI_Common, Label_UI_Icon_Atlas, Label_Font_Locale
        // - SRPG 코어 공통: Label_Grid_Common, Label_StatusEffect_VFX_Common
        // - 공통 오디오: Label_SFX_Common (BGM 제외)
        // - 미니게임 공통(조건부): Label_Minigame_Common (로비 노출용 진입 버튼/결과창 UI만)
        // - 테스트베드 전용: Label_Testbed_UI(스폰 패널/AI 튜닝 패널)
        //   ※ Label_Testbed_Actor(캐릭터/몬스터 프리팹)는 스폰 시점 동적 로드 - Preload 제외
        private async UniTask LoadCommonAddressableLabelsAsync(int current, int total)
        {
            ReportProgress(current, total, "공통 리소스 로드 중");
            await UniTask.CompletedTask;
        }

        private async UniTask LoadInputBindingResourcesAsync(int current, int total)
        {
            // TODO: IInputPlatformAdapter가 참조할 플랫폼별 바인딩 데이터 연동 예정
            ReportProgress(current, total, "입력 설정 로드 중");
            await UniTask.CompletedTask;
        }

        private async UniTask LoadWorldCommonResourcesAsync(int current, int total)
        {
            // TODO: IWorldMap 등에서 쓰는 공통 맵 메타/그리드 설정 연동 예정
            ReportProgress(current, total, "월드 데이터 로드 중");
            await UniTask.CompletedTask;
        }

        private async UniTask PrepareBgmInitialClipsAsync(int current, int total)
        {
            // TODO: BGMSystem 재생 로직과 연동한 초기 클립 핸들 준비 예정
            ReportProgress(current, total, "사운드 준비 중");
            await UniTask.CompletedTask;
        }

        private async UniTask LoadUiCommonShellResourcesAsync(int current, int total)
        {
            // TODO: Title/Main 진입 전 공통 프리팹(UIViewFactory/UIViewModelFactory 참조분) 연동 예정
            ReportProgress(current, total, "UI 리소스 로드 중");
            await UniTask.CompletedTask;
        }

        private void ReportProgress(int current, int total, string label)
        {
            _viewModel?.ApplySnapshot(new PreloadLoadingSnapshot(current, total, label));

            if (!_uiSystem.TryGetView<PreloadView>(out var view)) return;
            view.Refresh((float)current / total);
            view.RefreshLabel(label);
        }

        // TODO: [프레임워크 테스트베드 설계 - 임시 기록]
        // 목적: 실제 콘텐츠가 아닌 프레임워크 기능 경로 커버리지 검증
        // - SystemRegistry/GameSystemProvider: 스폰/AI/입력 시스템 등록 및 생명주기(Inject→Init→PostInit)
        // - SignalRouter: UI↔ECS 단방향 이벤트 브릿지
        // - DOTS/ECS + Burst: 다수 몬스터 Job 기반 AI(뱀파이어 서바이버형 추적)
        // - Addressables: 스폰 시점 프리팹 동적 로드/언로드
        // - DataForge/BlobAsset: AI 프로필/액터 스펙 데이터 파이프라인
        // - UIToolkit MVVM: 스폰 패널, AI 튜닝 패널
        // - 하이브리드 브릿지: UI 이벤트→ECS 반영, ECS 상태→UI 반영(폴링 없이 Signal 기반)
        //
        // 핵심 아키텍처 결정:
        // - UI(UIDocument)와 몬스터(순수 ECS)는 완전 분리, SignalRouter가 유일한 연결점
        // - 캐릭터 전환: PossessedTag : IComponentData를 Entity 간 이동시키는 방식,
        //   입력 시스템은 태그 기준 Query
        // - AI 파라미터: 개별 Entity 복사본이 아닌 공유 참조(SystemBase 필드/Singleton Component)로 두어
        //   UI 값 변경 시 한 곳만 갱신하면 즉시 전역 반영 (Entity별 복사 시 매 프레임 순회 갱신 비효율)
        // - Job 스케줄링 타이밍: UI 스레드 값 변경이 다음 프레임 Job 실행 시점에 안전 반영되는지
        //   Dependency 체인 검증 필요

        // TODO: [테스트베드 필요 데이터/리소스 목록 - SRPG 착수 전 선행 구축]
        // 1) Blob 마스터 데이터 (DataForge 산출물)
        //    - TestbedActorTable: ActorId, PrefabKey(Addressable), MoveSpeed, MaxHp, ColliderRadius
        //    - TestbedAIProfileTable: ProfileId, DetectRange, ChaseSpeedMultiplier, AttackInterval
        //    - TestbedSpawnPatternTable(선택): PatternId, SpawnCount, SpawnIntervalSec, SpawnRadius
        // 2) Addressable 리소스
        //    - Label_Testbed_Actor: 몬스터/캐릭터 프리팹(ECS Entity 변환용, GameObjectConversion 또는 Baker)
        //    - Label_Testbed_UI: 스폰 패널 UXML/USS, AI 튜닝 패널 UXML/USS
        // 3) ECS 컴포넌트/정의 (신규 작성 필요)
        //    - PossessedTag : IComponentData - 조종 대상 표시용 태그
        //    - TestbedActorData : IComponentData - MoveSpeed/MaxHp 등 Blob에서 복사되는 개별 값
        //    - TestbedAIProfileSingleton : IComponentData - AI 공유 파라미터(SystemBase 필드 대안)
        //    - TestbedSpawnRequestBufferElement : IBufferElementData(선택) - 스폰 요청 큐
        // 4) Signal 메시지 정의
        //    - SigTestbedSpawnRequested(ActorId, SpawnPosition, Count) - UI → ECS
        //    - SigTestbedPossessionChanged(EntityId) - UI → ECS 또는 ECS → UI
        //    - SigTestbedAIProfileChanged(ProfileId, DetectRange, ChaseSpeedMultiplier, AttackInterval) - UI → ECS
        //    - SigTestbedActorCountChanged(AliveCount) - ECS → UI (폴링 대체)
        // 5) System/Job (신규 작성 필요)
        //    - TestbedActorSpawnSystem: SigTestbedSpawnRequested 구독 → Addressable 로드 → Entity Instantiate
        //    - TestbedChaseJob(IJobEntity, Burst): PossessedTag 대상 추적 이동
        //    - TestbedPossessionSwitchSystem: PossessedTag 이동 처리, 입력 시스템 Query 대상 갱신
        //    - TestbedAIProfileSyncSystem: SigTestbedAIProfileChanged 수신 시 Singleton Component 갱신
        // 6) UI (UIToolkit MVVM)
        //    - TestbedSpawnView/ViewModel: 액터 선택 드롭다운, 스폰 수량 입력, 스폰 버튼
        //    - TestbedAITuningView/ViewModel: DetectRange/ChaseSpeedMultiplier/AttackInterval 슬라이더
        // 7) 검증 계측(선택)
        //    - 생존 Entity 수, 프레임 타임(Job 실행 시간) 표시용 디버그 오버레이

        // TODO: [금토일(3일) 작업 범위 - 실현 가능성 검토 결과 반영]
        // 근거: ECS/DOTS 패키지는 설치만 되어있고 실사용 코드 0건(World/SystemGroup 부트스트랩 전무) → 최대 리스크
        //       Signal/UIToolkit MVVM/DataForge는 기존 패턴 재사용 가능(항목당 반나절~1일)
        //       Addressables는 로드 래퍼만 있고 런타임 Instantiate 스폰 사례 없음
        // 금(Day1): ECS World/SystemGroup 부트스트랩 + IJobEntity 1개 Burst 컴파일 성공까지
        // 토(Day2): PossessedTag 전환 1종 + TestbedAIProfileSingleton 공유 파라미터 1개
        //           + Addressable 동적 스폰 1종(Label_Testbed_Actor 프리팹 1개만)
        // 일(Day3): SigTestbedSpawnRequested/SigTestbedAIProfileChanged 왕복 1쌍 연동
        //           + 최소 UI 패널 1개(스폰 버튼 + AI 파라미터 슬라이더 1개)로 End-to-End 검증
        // 3일 범위 제외(익주 이후로 연기): TestbedSpawnPatternTable, 스폰 패턴 다양화,
        //           AI 튜닝 패널 전체 슬라이더, 디버그 계측 오버레이, TestbedSpawnRequestBufferElement 큐
    }
}
