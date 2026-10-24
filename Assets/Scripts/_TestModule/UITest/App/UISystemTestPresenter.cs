using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Signal.Interfaces;
using Elder.Framework.UI.App;
using Elder.Framework.UI.Interfaces;
using Elder.TestModule.UITest.Messages;
using R3;
using System;

namespace Elder.TestModule.UITest.App
{
    // 버튼 요청을 Fx*RequestedSignal로 발행만 한다. 실제 Open/Close 처리는 구독자(FlowHandler/Coordinator) 쪽 책임.
    internal sealed class UISystemTestPresenter : UIPresenterBase
    {
        private ILoggerEx _logger = LogFacade.GetLogger<UISystemTestPresenter>();
        private ISignalPublisher _signalPublisher;
        private UISystemTestView _view;
        private IDisposable _subscriptions;

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
            if (view is not UISystemTestView systemTestView)
            {
                _logger.Error($"Failed to bind: view is not {nameof(UISystemTestView)}."); // [HEAP]
                return;
            }

            _view = systemTestView;
            _subscriptions = Disposable.Combine // [HEAP]
            (
                _view.OnOpenPanelRequested.Subscribe(HandleOpenPanelRequested),
                _view.OnOpenPopupRequested.Subscribe(HandleOpenPopupRequested),
                _view.OnCloseAllRequested.Subscribe(HandleCloseAllRequested),
                _view.OnCloseTopRequested.Subscribe(HandleCloseTopRequested)
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

        private void HandleOpenPanelRequested(Unit _)
        {
            _signalPublisher.Publish(new FxOpenPanelRequestedSignal());
        }

        private void HandleOpenPopupRequested(Unit _)
        {
            _signalPublisher.Publish(new FxOpenPopupRequestedSignal());
        }

        private void HandleCloseAllRequested(Unit _)
        {
            _signalPublisher.Publish(new FxCloseAllRequestedSignal());
        }

        private void HandleCloseTopRequested(Unit _)
        {
            _signalPublisher.Publish(new FxCloseTopRequestedSignal());
        }
    }
}
