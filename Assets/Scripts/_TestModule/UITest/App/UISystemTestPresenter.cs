using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Signal.Interfaces;
using Elder.Framework.Signal.Messages;
using Elder.Framework.UI.App;
using Elder.Framework.UI.Interfaces;
using Elder.TestModule.UITest.Interfaces;
using Elder.TestModule.UITest.Messages;
using R3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elder.TestModule.UITest.App
{
    // 버튼 요청을 Fx*RequestedSignal로 발행만 한다. 실제 Open/Close 처리는 구독자(FlowHandler/Coordinator) 쪽 책임.
    internal sealed class UISystemTestPresenter : UIPresenterBase
    {
        private ILoggerEx _logger = LogFacade.GetLogger<UISystemTestPresenter>();
        private ISignalPublisher _signalPublisher;
        private IUISystemTest _testModuleUISystem;
        private UISystemTestView _view;
        private IDisposable _subscriptions;

        protected override void OnInjectDependency()
        {
            if (!_provider.TryGetComponent<ISignalPublisher>(out _signalPublisher))
            {
                _logger.Error($"Failed to resolve {nameof(ISignalPublisher)}."); // [HEAP]
                return;
            }

            if (!_provider.TryGetComponent<IUISystemTest>(out _testModuleUISystem))
            {
                _logger.Error($"Failed to resolve {nameof(IUISystemTest)}."); // [HEAP]
            }
        }

        public override void BindView(IUIView view)
        {
            if (view is not UISystemTestView systemTestView)
            {
                _logger.Error($"Failed to bind: view is not {nameof(UISystemTestView)}."); // [HEAP]
                return;
            }

            if (_testModuleUISystem is null)
            {
                _logger.Error($"Failed to bind: {nameof(IUISystemTest)} is null."); // [HEAP]
                return;
            }

            _view = systemTestView;
            _subscriptions = Disposable.Combine // [HEAP]
            (
                _view.OnOpenPanelRequested.Subscribe(HandleOpenPanelRequested),
                _view.OnOpenPopupRequested.Subscribe(HandleOpenPopupRequested),
                _view.OnCloseAllRequested.Subscribe(HandleCloseAllRequested),
                _view.OnCloseTopRequested.Subscribe(HandleCloseTopRequested),
                _testModuleUISystem.OnStackChanged.Subscribe(_ => RefreshStackHierarchy())
            );

            RefreshStackHierarchy();
        }

        protected override void OnDispose()
        {
            _subscriptions?.Dispose();
            _subscriptions = null;
            _view = null;
            _signalPublisher = null;
            _testModuleUISystem = null;
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

        private void RefreshStackHierarchy()
        {
            var builder = new StringBuilder(); // [HEAP]

            AppendEntries(builder, "Scene", _testModuleUISystem.GetSceneStackEntries());
            AppendEntries(builder, "Global", _testModuleUISystem.GetGlobalLayerEntries());

            if (builder.Length == 0)
                builder.Append("Stack: (empty)");

            _view.SetStackHierarchyText(builder.ToString());
        }

        private static void AppendEntries(StringBuilder builder, string sectionName, IReadOnlyList<IUIEntryHandle> entries)
        {
            if (entries.Count == 0)
                return;

            if (builder.Length > 0)
                builder.Append('\n');

            builder.Append(sectionName).Append(':');

            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                builder.Append('\n').Append(entry.ViewHandle.ViewType.Name).Append(" #").Append(entry.DisplayIndex); // [HEAP]
            }
        }
    }
}
