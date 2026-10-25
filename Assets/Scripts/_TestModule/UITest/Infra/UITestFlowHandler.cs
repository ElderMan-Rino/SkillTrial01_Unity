using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using Elder.Framework.Signal.Messages;
using Elder.TestModule.Common.Definitions;
using Elder.TestModule.UITest.Interfaces;
using Elder.TestModule.UITest.Messages;
using System;

namespace Elder.TestModule.UITest.Infra
{
    internal sealed class UITestFlowHandler : IFrameworkComponent, IDependencyInjectable, IDisposable
    {
        private IUITestCoordinator _coordinator;
        private SignalToken _moduleStartToken;
        private SignalToken _openPanelToken;
        private SignalToken _openPopupToken;

        public Type ClassType => typeof(UITestFlowHandler);

        public int Scope => TestScope.Id;

        public void InjectDependency(IComponentProvider provider)
        {
            if (!provider.TryGetComponent(out ISignalSubscriber subscriber))
                throw new InvalidOperationException($"Failed to resolve {nameof(ISignalSubscriber)} for {nameof(UITestFlowHandler)}.");

            if (!provider.TryGetComponent(out _coordinator))
                throw new InvalidOperationException($"Failed to resolve {nameof(IUITestCoordinator)} for {nameof(UITestFlowHandler)}.");

            DisposeTokens();

            _moduleStartToken = subscriber.Subscribe<FxModuleStartSignal>(HandleModuleStart);
            _openPanelToken = subscriber.Subscribe<FxOpenPanelRequestedSignal>(HandleOpenPanelRequested);
            _openPopupToken = subscriber.Subscribe<FxOpenPopupRequestedSignal>(HandleOpenPopupRequested);
        }

        public void Dispose()
        {
            DisposeTokens();
            _coordinator = null;
        }

        private void DisposeTokens()
        {
            _moduleStartToken.Dispose();
            _openPanelToken.Dispose();
            _openPopupToken.Dispose();
        }

        // FlowHandler는 Signal -> Coordinator 호출만 담당, View/Presenter 구성은 모른다 (UITestCoordinator.StartAsync 플랜 참고).
        private void HandleModuleStart(in FxModuleStartSignal signal)
        {
            if (signal.ModuleType != typeof(UITestCoordinator)) return;

            _coordinator.StartAsync().Forget();
        }

        private void HandleOpenPanelRequested(in FxOpenPanelRequestedSignal signal)
        {
            _coordinator.OpenTestPanelAsync().Forget();
        }

        private void HandleOpenPopupRequested(in FxOpenPopupRequestedSignal signal)
        {
            _coordinator.OpenTestPopupAsync().Forget();
        }
    }
}
