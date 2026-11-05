using Cysharp.Threading.Tasks;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Signal.Interfaces;
using Elder.Framework.Signal.Messages;
using Elder.Framework.UI.App;
using Elder.Framework.UI.Interfaces;
using Elder.TestModule.UITest.Messages;
using R3;
using System;

namespace Elder.TestModule.UITest.App
{
    // Popup 확인/취소 요청을 FxCloseTopRequestedSignal로 발행만 한다. 실제 Close 처리는 UISystem 책임.
    // Cancel은 RequestCloseAsync 취소 훅 검증용으로 최초 1회 거부 후 재시도 시 닫힘을 허용한다.
    internal sealed class UITestPopupPresenter : UIPresenterBase
    {
        private ILoggerEx _logger = LogFacade.GetLogger<UITestPopupPresenter>();
        private ISignalPublisher _signalPublisher;
        private UITestPopupView _view;
        private IDisposable _subscriptions;
        private bool _cancelConfirmed;

        protected override void OnInjectDependency()
        {
            if (!_provider.TryGetComponent<ISignalPublisher>(out _signalPublisher))
            {
                _logger.Error($"Failed to resolve {nameof(ISignalPublisher)}."); // [HEAP]
                return;
            }
        }

        public override void BindView(IUIView view)
        {
            if (view is not UITestPopupView popupView)
            {
                _logger.Error($"Failed to bind: view is not {nameof(UITestPopupView)}."); // [HEAP]
                return;
            }

            _subscriptions?.Dispose();

            _view = popupView;
            _subscriptions = Disposable.Combine // [HEAP]
            (
                _view.OnConfirmRequested.Subscribe(HandleConfirmRequested),
                _view.OnCancelRequested.Subscribe(HandleCancelRequested)
            );
        }

        protected override void OnDispose()
        {
            _subscriptions?.Dispose();
            _subscriptions = null;
            _view = null;
            _signalPublisher = null;
            _logger = null;
        }

        public override UniTask<bool> RequestCloseAsync()
        {
            if (_cancelConfirmed)
                return UniTask.FromResult(true);

            _cancelConfirmed = true;
            _logger.Info("Cancel rejected once for RequestCloseAsync verification."); // [HEAP]
            return UniTask.FromResult(false);
        }

        private void HandleConfirmRequested(Unit _)
        {
            _cancelConfirmed = true;
            _signalPublisher.Publish(new FxCloseTopRequestedSignal());
        }

        private void HandleCancelRequested(Unit _)
        {
            _signalPublisher.Publish(new FxCloseTopRequestedSignal());
        }
    }
}
