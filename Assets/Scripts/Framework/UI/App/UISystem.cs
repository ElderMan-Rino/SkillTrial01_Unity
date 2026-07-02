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
    internal sealed class UISystem : BaseSystem, IUISystem
    {
        private readonly UIStackOptions _options;
        private IAssetProvider _assetProvider;
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
