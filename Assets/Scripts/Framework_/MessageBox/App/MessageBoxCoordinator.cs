/*
using Cysharp.Threading.Tasks;
using Elder.Framework.MessageBox.Definitions;
using Elder.Framework.MessageBox.Infra;
using Elder.Framework.MessageBox.Interfaces;
using Elder.Framework.MessageBox.Messages;
using Elder.Framework.Signal.Helpers;
using System;
using System.Collections.Generic;

namespace Elder.Framework.MessageBox.App
{
    // [TODO] Pool 설계 (동시 다중 표시 지원)
    // 현재 UISystem/UIPresenterFactory는 Type당 인스턴스 1개 고정(Dictionary<Type,...>) 구조라
    // MessageBox를 동시에 여러 개 띄우는 요구사항을 처리할 수 없음. 아래 구조로 이 Coordinator 내부에서
    // 별도 풀을 직접 관리하고, UISystem.ShowAsync<MessageBoxView>() 경로는 MessageBox에 한해 사용하지 않는다.
    //
    // 1. Coordinator가 IMessageBoxPresenter 풀을 직접 소유:
    //    private readonly ObjectPool<IMessageBoxPresenter> _presenterPool;
    //    - Factory 델리게이트: () => new MessageBoxPresenter() 로 생성 (UIPresenterFactory 미경유 — 그건 싱글턴 전용)
    //    - View도 동일하게 Coordinator가 직접 Instantiate/Pool 관리 (Addressables 인스턴스 N개 유지)
    //
    // 2. RequestAsync 호출 시:
    //    - _presenterPool.Get()으로 Presenter+View 인스턴스 1세트 대여
    //    - presenter.SetViewModel(new MessageBoxViewModel(message, buttonType)) 로 내용 주입
    //    - View 활성화(SetActive) + 화면 배치(스택/레이어 순서는 Coordinator가 직접 계산 — UISystem 스택 관리 미사용)
    //
    // 3. 결과 수신(HandleMessageBoxResult) 시:
    //    - 해당 requestId에 매핑된 인스턴스 세트를 조회 (Dictionary<Guid, (IMessageBoxPresenter, MessageBoxView)>)
    //    - View 비활성화 후 _presenterPool.Release(presenter)로 반납 (Presenter.DisposeManagedResources는 호출하지 않음 — 재사용 전제)
    //
    // 4. 확인 필요 사항 (구현 전 결정):
    //    - View 인스턴스도 Presenter처럼 풀링할지, 혹은 View는 필요 시 Addressables Instantiate/Destroy 반복할지
    //    - 여러 MessageBox 동시 표시 시 레이아웃(겹침 방지, z-order)을 누가 계산할지 — Coordinator 후보
    //    - UISystem의 UILayer/Confirm 그룹 정책과 별도 풀 View들의 sortingOrder를 어떻게 정합시킬지
    internal sealed class MessageBoxCoordinator : UI.App.UICoordinatorBase, IMessageBoxCoordinator
    {
        // [HEAP] RequestId별 대기 TaskCompletionSource 저장 — 요청 빈도가 낮아 허용
        private readonly Dictionary<Guid, UniTaskCompletionSource<MessageBoxResult>> _pendingRequests = new();

        private SignalToken _resultSubscription;

        protected override void OnInjectDependency()
        {
            _resultSubscription = _router.Subscribe<MessageBoxResultSignal>(HandleMessageBoxResult);
        }

        public override async UniTask PrepareAsync()
        {
            await _uiSystem.PrepareAsync<MessageBoxView>();
        }

        public override async UniTask ShowAsync()
        {
            await _uiSystem.ShowAsync<MessageBoxView>();
        }

        public override async UniTask HideAsync()
        {
            await _uiSystem.HideAsync<MessageBoxView>();
        }

        public async UniTask<MessageBoxResult> RequestAsync(string message, MessageBoxButtonType buttonType, MessageBoxDismissPolicy dismissPolicy = MessageBoxDismissPolicy.Normal)
        {
            var requestId = Guid.NewGuid();
            var tcs = new UniTaskCompletionSource<MessageBoxResult>();
            _pendingRequests[requestId] = tcs;

            _router.Publish(new MessageBoxRequestSignal(requestId, message, buttonType, dismissPolicy));

            return await tcs.Task;
        }

        private void HandleMessageBoxResult(in MessageBoxResultSignal signal)
        {
            if (!_pendingRequests.TryGetValue(signal.RequestId, out var tcs)) return;

            _pendingRequests.Remove(signal.RequestId);
            tcs.TrySetResult(signal.Result);
        }

        public UniTask<MessageBoxResult> OpenMessageBoxViewAsync(string descKey)
        {
            throw new NotImplementedException();
        }
    }
}

*/
