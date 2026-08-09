using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core;
using Elder.Framework.UI.Definitions;
using Elder.Framework.UI.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elder.Framework.UI.App
{
    // ──────────────────────────────────────────────────────────────────────
    // [UI System Architecture]
    //
    // 전체 흐름:
    //   씬 전환 완료
    //     │
    //     ▼
    //   XxxOrchestrator.RegisterSystems()
    //     └─ registry.Register<XxxCoordinator>().As<IUICoordinator>()
    //        registry.Register<XxxSubCoordinator>()   ← Sub: As 없이 등록
    //     │
    //     ▼ TryPrepareAsync() → Build() → InjectAll() → InitializeAll()
    //     │
    //     ▼
    //   XxxCoordinator.ShowAsync()                    ← Main Coordinator (씬 단위)
    //     └─ UISystem.ShowAsync<XxxView>("Xxx/XxxView")
    //          ├─ IAssetProvider.GetAssetAsync<GameObject>()  [HEAP] 씬 진입 시 1회
    //          └─ Object.Instantiate()                        [HEAP] 씬 진입 시 1회
    //     │
    //     ▼
    //   XxxView (MonoBehaviour)
    //   OnXxxButton (IObservable / UnityEvent)
    //     │
    //     ▼
    //   XxxCoordinator.HandleXxxButton()
    //     └─ Router.Publish(new SceneTransitionSignal("TargetScene"))
    //
    // ──────────────────────────────────────────────────────────────────────
    // [레이어 책임]
    //
    //   IUISystem (UISystem)
    //     - Prefab Addressables 로드 · 캐시 · Instantiate · Destroy
    //     - ShowAsync / HideAsync / ReleaseAsync / TryGetView
    //     - UI 스택 관리: PopAsync / ClearStackAsync / StackCount
    //     - Presenter가 IUIPresenterLifecycle 구현 시 → 조건부 캐스팅으로 호출
    //
    //   UICoordinatorBase (XxxCoordinator)
    //     - UISystem.ShowAsync 요청 (생성 위임)
    //     - View 이벤트 구독 → Router.Publish (Signal 발행 유일 주체)
    //     - Sub Coordinator는 Main Coordinator가 TryGetSystem으로 직접 참조·제어
    //
    //   XxxOrchestrator (GameModeOrchestrator)
    //     - RegisterSystems()에서 Coordinator 등록
    //     - TryActivateAsync() → coordinator.ShowAsync()
    //     - TeardownAsync()    → coordinator.HideAsync()
    //
    // ──────────────────────────────────────────────────────────────────────
    // [Presenter 라이프사이클 콜백]
    //
    //   Presenter가 IUIPresenterLifecycle을 구현한 경우에만 호출됨
    //   UISystem은 UIHandle.Lifecycle(nullable)을 통해 조건부 캐스팅 없이 호출
    //
    //   ShowAsync<TView>(presenter)
    //     → instance 생성 → UIHandle(instance, assetHandle, presenter as IUIPresenterLifecycle)
    //     → Lifecycle?.OnViewShownAsync()
    //
    //   HideAsync<TView>()
    //     → SetActive(false) → Lifecycle?.OnViewHiddenAsync()
    //
    //   ReleaseAsync<TView>() / PopAsync() / ClearStackAsync()
    //     → handle.Dispose() → Lifecycle?.OnViewReleasedAsync()
    //
    //   Signal 발행 흐름:
    //     UISystem (라이프사이클 호출)
    //       └─ presenter.OnViewShownAsync()
    //            └─ Router.Publish(new FxInputBlock()) or FxBgmChange() 등
    //
    // ──────────────────────────────────────────────────────────────────────
    // [UI 스택 정책]
    //
    //   ShowAsync<TView>()
    //     ├─ 이미 스택에 있으면 → SetActive(true) + 스택 맨 위로 이동
    //     └─ 없으면 → 로드 + Instantiate + 스택 Push
    //          └─ MaxStackSize 초과 시 → 가장 오래된(Bottom) View를 자동 Release
    //
    //   PopAsync()      → 스택 Top(가장 최근) View Release
    //   ClearStackAsync() → 전체 스택 Release
    //   StackCount      → 현재 스택 크기
    //
    //   MaxStackSize → UIStackOptions.MaxStackSize (기본값 16)
    //                  UISystemInstaller(new UIStackOptions(N)) 으로 변경
    //
    // ──────────────────────────────────────────────────────────────────────
    // [MVPVM 상태 스냅샷 패턴]
    //
    //   Signal
    //     │
    //     ▼
    //   Model  ← Signal 수신, 도메인 상태 보유
    //     │
    //     ▼
    //   Presenter
    //     ├─ Model 구독
    //     ├─ diff 판단 (이전 Snapshot과 비교)
    //     ├─ _viewModel.Apply(in newSnapshot)   ← readonly struct, [HEAP] 없음
    //     └─ _view.Refresh()                    ← 신호만 전달, 데이터 없음
    //     │
    //     ▼
    //   ViewModel  ← TSnapshot(readonly struct) 보유 — 상태 스냅샷 단일 진실 공급원
    //     │  Snapshot { get; private set; }
    //     │  Apply(in TSnapshot next) → Snapshot = next
    //     │
    //     ▼
    //   View
    //     ├─ ViewModel 참조 보유 (Presenter는 모름 — MVPVM 규칙)
    //     └─ Refresh() → _viewModel.Snapshot 읽어 UI 갱신 (Pull)
    //
    // [레이어 참조 규칙]
    //   View      → ViewModel O  /  Presenter X  /  Model X
    //   Presenter → View O  /  ViewModel O  /  Model O  /  Signal 발행 X
    //   Model     → Signal 수신 O  /  View X  /  Presenter X
    //   Coordinator → Signal 발행 O (유일 주체)  /  Presenter 생성·소유 O
    //
    // [TSnapshot 규칙]
    //   - 반드시 readonly struct
    //   - 모든 필드 readonly
    //   - Presenter에서 in 키워드로 전달 (복사 비용 최소화)
    //
    // ──────────────────────────────────────────────────────────────────────
    // [금지 사항]
    //   - View / ViewModel이 Signal Bus 직접 사용 금지
    //   - Coordinator 외부에서 UISystem.ShowAsync 직접 호출 금지
    //   - Sub Coordinator를 As<IUICoordinator>로 등록 금지
    //     (TryGetSystem 중복 충돌 방지)
    //   - Snapshot을 class로 정의 금지 ([HEAP] 발생)
    //   - View.Refresh(in snapshot) 형태로 데이터 Push 금지
    //     (View는 ViewModel을 통해 Pull해야 함)
    //   - UISystem이 Presenter 구체 타입을 직접 참조 금지
    //     (IUIPresenterLifecycle 인터페이스만 허용)
    // ──────────────────────────────────────────────────────────────────────
    // [TODO] Registry 브릿지 / 레이어드 Depth / 입력 잠금 설계
    //
    //   1. Registry 브릿지 (MessageBoxView 등 boot-preload 라벨 자산 대응)
    //      - 현재 UISystem은 _assetProvider(IAssetProvider, key 기반)만 보유
    //        → FrameworkRoot.PreloadBootAssetsAsync가 IAssetRegistry(label 기반)로
    //          올려둔 자산(MessageBoxView 등)을 UISystem이 가져올 경로가 없음
    //      - 필요 작업:
    //          a) IAssetRegistry _assetRegistry 필드 추가, HandleInjectDependency에서
    //             optional(TryGetSystem 실패해도 throw 안 함)로 주입
    //          b) ShowAsync<TView>(string label, string assetName) 오버로드 추가
    //             → ShowFromRegistryAsync<TView> 헬퍼에서 _assetRegistry.Get<GameObject>(label, assetName) 사용
    //             (동기 반환 — LoadAndShowAsync와 달리 await 불필요)
    //          c) UIHandle 생성자 보강 필요: 현재 두 생성자 모두
    //             "Instantiate는 했으나 IAssetHandle은 없음(Registry 소유)" 케이스를 표현 못함
    //             → Dispose 시 Object.Destroy는 호출하되 assetHandle.Release는 스킵하는
    //               제3의 생성자 또는 ownership enum 필요
    //
    //   2. 레이어드 Depth (윈도우 스택처럼 위로 올라오는 방식)
    //      - 현재 _stack(LinkedList<Type>)은 MRU 순서만 관리하는 논리적 스택이며
    //        SetAsLastSibling / sortingOrder 등 실제 렌더링 순서에는 관여하지 않음
    //      - 권장안: 단일 depth 카운터보다 UILayer enum 기반 다중 Canvas 분리,
    //        "일반 그룹"과 "확인 팝업 그룹" 두 그룹으로 분리
    //          [일반 그룹] Fullscreen / Popup / Overlay / Toast — 그룹 내부 상대적 스택
    //            → sortingOrder 대역 분리 (예: 0 / 100 / 200 / 300)
    //          [확인 팝업 그룹] Confirm — 확인/취소/구매/구매취소 등 사용자 응답을 요구하는
    //            범용 액션 팝업. 일반 그룹 전체보다 항상 위, 별도 최상단 고정 대역
    //            → sortingOrder 예: 1000+ (일반 그룹 최대값과 겹치지 않도록 고정 오프셋)
    //            → PopAsync 등 일반 그룹의 스택 연산 대상에서 제외 (뒤로가기로 안 닫힘)
    //          → 동일 레이어 내부에서는 transform.SetAsLastSibling()으로 순서 결정
    //      - _stack도 Dictionary<UILayer, LinkedList<Type>>로 레이어별 분리 필요
    //        → PopAsync()는 전역 스택이 아닌 "현재 상호작용 가능한 최상위 일반 그룹 레이어(Popup 등)"의
    //          top을 대상으로 해야 함 (Confirm 그룹은 버튼 콜백 등 별도 API로만 닫힘)
    //      - UIStackOptions를 레이어별 sortingOrder 기준값을 포함하도록 확장 고려
    //        (추후 MessagePack/Blob 데이터화 예정 항목과 동일 위치)
    //
    //   3. 입력 잠금(Input Block) — 각 UI가 요청하는 방식이 아니라 UISystem/레이어에서 중앙 처리
    //      - Presenter가 직접 Signal을 발행해 잠금을 요청하는 방식은 채택하지 않음
    //        (레이어 참조 규칙: Presenter → Signal 발행 X 위반)
    //      - Presenter마다 잠금 요청/해제를 개별 관리하면 해제 누락, 중복 잠금 등
    //        생명주기 불일치 위험이 큼
    //      - 대안: ShowAsync/HideAsync/ReleaseAsync 등 UISystem의 라이프사이클 지점에서
    //        자동 처리
    //          - UILayer가 Popup/Fullscreen처럼 "차단 레이어"로 분류되면
    //            ShowAsync 시 해당 레이어 아래로 raycast-blocking 오버레이를 자동 추가
    //          - HideAsync/ReleaseAsync/PopAsync 시 해당 오버레이도 함께 정리
    //          - 레이어별 차단 여부는 UILayer enum 또는 UIStackOptions 쪽에
    //            선언적으로 정의 (per-UI 개별 요청이 아니라 레이어 단위 정책)
    //      - 즉, "각 UI가 요청" ✕ / "UISystem이 레이어 정책에 따라 자동 처리" ○
    //
    //   4. UI 전용 에셋 테이블 (Registry 브릿지 항목 1과 연계, 라벨 하드코딩 대체)
    //      - 현재 SplashPresenter처럼 Presenter마다 라벨을 const string으로 하드코딩
    //        (예: private const string SplashLabel = "SplashSprites";)
    //        → UI 종류가 늘어날수록(Confirm/MessageBoxView 등) Presenter마다 상수 중복 → 스케일 안 됨
    //      - 기존 3테이블(BootRoot / AssetInfoEntryBootRoot / AssetInfoEntryPreloadRoot)은
    //        모두 (Label, Id, AssetType) 동일 스키마이며 "로드 시점"(부트 블로킹/지연 프리로드/일반)
    //        기준으로만 분리되어 있음 — "에셋 카테고리" 기준 분리가 아니므로 UI 전용 필드
    //        (ViewKey, AssetName 등)를 넣기에 적합하지 않음 (스키마 오염)
    //      - 제안: 신규 UI 전용 테이블 (가칭 UIAssetEntryRoot)
    //          - 스키마: ViewKey(TView 매핑) / Label / AssetName / [UILayer](선택)
    //          - ShowAsync<TView>(string label, string assetName) 오버로드가 이 테이블을
    //            조회하여 Registry.Get<GameObject>(label, assetName) 호출
    //          - 항목 2의 UILayer(레이어드 Depth) 설계와 필드 공유 가능
    //      - AssetInfoEntryBaker.cs와 동일한 Bake 패턴(BlobBuilder + MessagePack DTO)으로
    //        구현 가능 — 신규 Bake_UIAssetEntry 메서드 + 전용 Blob Root 타입 필요
    // ──────────────────────────────────────────────────────────────────────
    internal sealed class UISystem : BaseSystem, IUISystem
    {
        private readonly UIStackOptions _options;
        private IAssetProvider _assetProvider;
        private IAssetRegistry _assetRegistry;
        private IUIPresenterFactory _presenterFactory;

        // [HEAP] Dictionary · LinkedList 초기화 — 기동 시 1회
        private readonly Dictionary<Type, UIHandle> _handles = new();
        private readonly LinkedList<Type> _stack = new();
        // [HEAP] Dictionary 초기화 — 기동 시 1회, ShowAsync 동시 호출 가드
        private readonly Dictionary<Type, UniTask> _pendingShows = new();

        public int StackCount => _stack.Count;

        public UISystem(UIStackOptions options)
        {
            _options = options;
        }

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<IAssetProvider>(out _assetProvider))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IAssetProvider)}");
            if (!TryGetSystem<IUIPresenterFactory>(out _presenterFactory))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IUIPresenterFactory)}");
            TryGetSystem<IAssetRegistry>(out _assetRegistry);
        }

        public async UniTask PrepareAsync<TView>() where TView : UIViewBase
        {
            var lifecycle = _presenterFactory.GetLifecycleForView<TView>();
            if (lifecycle is IUIPresenterPreparable preparable) await preparable.PrepareAsync();
        }

        public async UniTask<TView> ShowAsync<TView>(string addressableKey = null) where TView : UIViewBase
        {
            var type = typeof(TView);
            var lifecycle = _presenterFactory.GetLifecycleForView<TView>();

            if (_handles.TryGetValue(type, out var existing))
            {
                existing.Instance.SetActive(true);
                _stack.Remove(type);
                _stack.AddLast(type);
                existing.SetLifecycle(lifecycle);
                if (lifecycle is not null) await lifecycle.OnViewShownAsync();
                return existing.Instance.GetComponent<TView>();
            }

            if (_pendingShows.TryGetValue(type, out var pending))
            {
                await pending;
                return await ShowAsync<TView>(addressableKey);
            }

            var loadTask = LoadAndShowAsync<TView>(type, lifecycle, addressableKey);
            _pendingShows[type] = loadTask.AsUniTask();
            try
            {
                return await loadTask;
            }
            finally
            {
                _pendingShows.Remove(type);
            }
        }

        public async UniTask<TView> ShowAsync<TView>(string label, string assetName) where TView : UIViewBase
        {
            if (_assetRegistry is null)
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IAssetRegistry)}");

            var type = typeof(TView);
            var lifecycle = _presenterFactory.GetLifecycleForView<TView>();

            if (_handles.TryGetValue(type, out var existing))
            {
                existing.Instance.SetActive(true);
                _stack.Remove(type);
                _stack.AddLast(type);
                existing.SetLifecycle(lifecycle);
                if (lifecycle is not null) await lifecycle.OnViewShownAsync();
                return existing.Instance.GetComponent<TView>();
            }

            if (_pendingShows.TryGetValue(type, out var pending))
            {
                await pending;
                return await ShowAsync<TView>(label, assetName);
            }

            var loadTask = LoadAndShowAsync<TView>(type, lifecycle, label, assetName);
            _pendingShows[type] = loadTask.AsUniTask();
            try
            {
                return await loadTask;
            }
            finally
            {
                _pendingShows.Remove(type);
            }
        }

        private async UniTask<TView> LoadAndShowAsync<TView>(Type type, IUIPresenterLifecycle lifecycle, string label, string assetName) where TView : UIViewBase
        {
            if (_stack.Count >= _options.MaxStackSize)
                await ReleaseOldestAsync();

            // [HEAP] Instantiate — 씬 진입 시 1회
            var instance = GameObject.Instantiate(_assetRegistry.Get<GameObject>(label, assetName));
            _handles[type] = new UIHandle(instance, isRegistryOwned: true, lifecycle);
            _stack.AddLast(type);

            if (lifecycle is not null) await lifecycle.OnViewShownAsync();
            return instance.GetComponent<TView>();
        }

        private async UniTask<TView> LoadAndShowAsync<TView>(Type type, IUIPresenterLifecycle lifecycle, string addressableKey) where TView : UIViewBase
        {
            if (_stack.Count >= _options.MaxStackSize)
                await ReleaseOldestAsync();

            // [HEAP] Addressables 로드 — 씬 진입 시 1회
            var assetHandle = await _assetProvider.GetAssetAsync<GameObject>(addressableKey);
            // [HEAP] Instantiate — 씬 진입 시 1회
            var instance = GameObject.Instantiate(assetHandle.Asset);
            _handles[type] = new UIHandle(instance, assetHandle, lifecycle);
            _stack.AddLast(type);

            if (lifecycle is not null) await lifecycle.OnViewShownAsync();
            return instance.GetComponent<TView>();
        }

        public async UniTask HideAsync<TView>() where TView : UIViewBase
        {
            if (!_handles.TryGetValue(typeof(TView), out var handle)) return;
            handle.Instance.SetActive(false);
            if (handle.Lifecycle is not null) await handle.Lifecycle.OnViewHiddenAsync();
        }

        public async UniTask ReleaseAsync<TView>() where TView : UIViewBase
        {
            var type = typeof(TView);
            if (!_handles.TryGetValue(type, out var handle)) return;
            _handles.Remove(type);
            _stack.Remove(type);
            try
            {
                if (handle.Lifecycle is not null) await handle.Lifecycle.OnViewReleasedAsync();
            }
            finally
            {
                handle.Dispose();
            }
        }

        public async UniTask PopAsync()
        {
            if (_stack.Last is null) return;
            var type = _stack.Last.Value;
            _stack.RemoveLast();
            if (!_handles.TryGetValue(type, out var handle)) return;
            _handles.Remove(type);
            try
            {
                if (handle.Lifecycle is not null) await handle.Lifecycle.OnViewReleasedAsync();
            }
            finally
            {
                handle.Dispose();
            }
        }

        public async UniTask ClearStackAsync()
        {
            // [HEAP] 핸들 스냅샷 — ClearStack 시 1회 할당, foreach 중 예외 시 Dispose 보장
            var snapshot = new List<UIHandle>(_handles.Values);
            _handles.Clear();
            _stack.Clear();

            foreach (var handle in snapshot)
            {
                try
                {
                    if (handle.Lifecycle is not null) await handle.Lifecycle.OnViewReleasedAsync();
                }
                finally
                {
                    handle.Dispose();
                }
            }
        }

        public bool TryGetView<TView>(out TView view) where TView : UIViewBase
        {
            view = null;
            if (!_handles.TryGetValue(typeof(TView), out var handle)) return false;
            view = handle.Instance.GetComponent<TView>();
            return view is not null;
        }

        public UniTask OnSceneLoadedAsync()
        {
            // [HEAP] FindObjectsByType — 씬 전환 시 1회, 이후 참조 없음
            var sceneViews = GameObject.FindObjectsByType<UIViewBase>(FindObjectsSortMode.None);
            int uiLayer = LayerMask.NameToLayer("UI");
            for (int i = 0; i < sceneViews.Length; i++)
            {
                var view = sceneViews[i];
                if (view.gameObject.layer != uiLayer) continue;
                var type = view.GetType();
                if (!_handles.ContainsKey(type))
                {
                    _handles[type] = new UIHandle(view.gameObject);
                    _stack.AddLast(type);
                }
            }
            return UniTask.CompletedTask;
        }

        public override UniTask InitializeAsync() => UniTask.CompletedTask;
        public override UniTask PostInitializeAsync() => UniTask.CompletedTask;

        protected override void DisposeManagedResources()
        {
            foreach (var handle in _handles.Values)
                handle.Dispose();
            _handles.Clear();
            _stack.Clear();
            _assetProvider = null;
            _presenterFactory = null;
        }

        private async UniTask ReleaseOldestAsync()
        {
            if (_stack.First is null) return;
            var oldest = _stack.First.Value;
            _stack.RemoveFirst();
            if (!_handles.TryGetValue(oldest, out var handle)) return;
            _handles.Remove(oldest);
            try
            {
                if (handle.Lifecycle is not null) await handle.Lifecycle.OnViewReleasedAsync();
            }
            finally
            {
                handle.Dispose();
            }
        }
    }
}
