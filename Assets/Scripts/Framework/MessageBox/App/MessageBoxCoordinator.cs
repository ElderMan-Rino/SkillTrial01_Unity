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
    }
}
