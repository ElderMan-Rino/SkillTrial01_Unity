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
    // Panel 닫기 요청을 FxCloseTopRequestedSignal로 발행만 한다. 실제 Close 처리는 UISystem 책임.
    internal sealed class UITestPanelPresenter : UIPresenterBase
    {
        private ILoggerEx _logger = LogFacade.GetLogger<UITestPanelPresenter>();
        private ISignalPublisher _signalPublisher;
        private UITestPanelView _view;
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
            if (view is not UITestPanelView panelView)
            {
                _logger.Error($"Failed to bind: view is not {nameof(UITestPanelView)}."); // [HEAP]
                return;
            }

            _subscriptions?.Dispose();

            _view = panelView;
            _subscriptions = _view.OnCloseRequested.Subscribe(HandleCloseRequested);
        }

        protected override void OnDispose()
        {
            _subscriptions?.Dispose();
            _subscriptions = null;
            _view = null;
            _signalPublisher = null;
            _logger = null;
        }

        private void HandleCloseRequested(Unit _)
        {
            _signalPublisher.Publish(new FxCloseTopRequestedSignal());
        }
    }
}
