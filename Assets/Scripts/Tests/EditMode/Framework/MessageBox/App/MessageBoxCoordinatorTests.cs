using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.MessageBox.App;
using Elder.Framework.MessageBox.Definitions;
using Elder.Framework.MessageBox.Messages;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using Elder.Framework.UI.App;
using Elder.Framework.UI.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Tests.EditMode.Framework.MessageBox.App
{
    public class MessageBoxCoordinatorTests
    {
        [Test]
        public void RequestAsync_ResultSignalWithMatchingRequestId_ResolvesWithResult()
        {
            var router = new FakeSignalRouter();
            var provider = new FakeSystemProvider(new FakeUISystem(), router);
            var coordinator = new MessageBoxCoordinator();
            coordinator.InjectDependency(provider);

            var task = coordinator.RequestAsync("message", MessageBoxButtonType.Ok);
            var requestId = router.LastPublishedRequest.RequestId;
            router.RaiseResult(new MessageBoxResultSignal(requestId, MessageBoxResult.Ok));

            Assert.AreEqual(MessageBoxResult.Ok, task.GetAwaiter().GetResult());
        }

        [Test]
        public void HandleMessageBoxResult_UnmatchedRequestId_DoesNotThrowAndLeavesRequestPending()
        {
            var router = new FakeSignalRouter();
            var provider = new FakeSystemProvider(new FakeUISystem(), router);
            var coordinator = new MessageBoxCoordinator();
            coordinator.InjectDependency(provider);

            var task = coordinator.RequestAsync("message", MessageBoxButtonType.Ok);

            Assert.DoesNotThrow(() => router.RaiseResult(new MessageBoxResultSignal(Guid.NewGuid(), MessageBoxResult.Cancel)));
            Assert.IsFalse(task.Status.IsCompleted());
        }

        [Test]
        public void InjectDependency_UISystemMissing_ThrowsInvalidOperationException()
        {
            var provider = new FakeSystemProvider(null, new FakeSignalRouter());
            var coordinator = new MessageBoxCoordinator();

            Assert.Throws<InvalidOperationException>(() => coordinator.InjectDependency(provider));
        }

        [Test]
        public void InjectDependency_SignalRouterMissing_ThrowsInvalidOperationException()
        {
            var provider = new FakeSystemProvider(new FakeUISystem(), null);
            var coordinator = new MessageBoxCoordinator();

            Assert.Throws<InvalidOperationException>(() => coordinator.InjectDependency(provider));
        }

        [Test]
        public void PrepareAsync_Called_DelegatesToUISystemPrepare()
        {
            var uiSystem = new FakeUISystem();
            var provider = new FakeSystemProvider(uiSystem, new FakeSignalRouter());
            var coordinator = new MessageBoxCoordinator();
            coordinator.InjectDependency(provider);

            coordinator.PrepareAsync().GetAwaiter().GetResult();

            Assert.IsTrue(uiSystem.PrepareCalled);
        }

        [Test]
        public void ShowAsync_Called_DelegatesToUISystemShow()
        {
            var uiSystem = new FakeUISystem();
            var provider = new FakeSystemProvider(uiSystem, new FakeSignalRouter());
            var coordinator = new MessageBoxCoordinator();
            coordinator.InjectDependency(provider);

            coordinator.ShowAsync().GetAwaiter().GetResult();

            Assert.IsTrue(uiSystem.ShowCalled);
        }

        [Test]
        public void HideAsync_Called_DelegatesToUISystemHide()
        {
            var uiSystem = new FakeUISystem();
            var provider = new FakeSystemProvider(uiSystem, new FakeSignalRouter());
            var coordinator = new MessageBoxCoordinator();
            coordinator.InjectDependency(provider);

            coordinator.HideAsync().GetAwaiter().GetResult();

            Assert.IsTrue(uiSystem.HideCalled);
        }

        private sealed class FakeSystemProvider : IGameSystemProvider
        {
            private readonly IUISystem _uiSystem;
            private readonly ISignalRouter _router;

            public FakeSystemProvider(IUISystem uiSystem, ISignalRouter router)
            {
                _uiSystem = uiSystem;
                _router = router;
            }

            public bool TryGetSystem<T>(out T system) where T : class, ISystemComponent
            {
                if (typeof(T) == typeof(IUISystem) && _uiSystem is not null)
                {
                    system = (T)_uiSystem;
                    return true;
                }

                if (typeof(T) == typeof(ISignalRouter) && _router is not null)
                {
                    system = (T)_router;
                    return true;
                }

                system = null;
                return false;
            }

            public bool TryGetSystems<T>(ref List<T> system) where T : class, ISystemComponent => false;

            public bool TryDisposeSystem(Type systemType) => false;

            public void InjectAll() { }

            public UniTask InitializeAllAsync() => UniTask.CompletedTask;

            public UniTask PostInitializeAllAsync() => UniTask.CompletedTask;
        }

        private sealed class FakeSignalRouter : ISignalRouter
        {
            private SignalHandler<MessageBoxResultSignal> _resultHandler;

            public MessageBoxRequestSignal LastPublishedRequest { get; private set; }

            public void Publish<T>(in T signal) where T : struct, ISignal
            {
                if (signal is MessageBoxRequestSignal request) LastPublishedRequest = request;
            }

            public SignalToken Subscribe<T>(SignalHandler<T> handler) where T : struct, ISignal
            {
                if (handler is SignalHandler<MessageBoxResultSignal> resultHandler) _resultHandler = resultHandler;
                return SignalToken.Empty;
            }

            public void RaiseResult(in MessageBoxResultSignal signal)
            {
                _resultHandler?.Invoke(in signal);
            }
        }

        private sealed class FakeUISystem : IUISystem
        {
            public bool PrepareCalled { get; private set; }
            public bool ShowCalled { get; private set; }
            public bool HideCalled { get; private set; }

            public int StackCount => 0;

            public UniTask OnSceneLoadedAsync() => UniTask.CompletedTask;

            public UniTask PrepareAsync<TView>() where TView : UIViewBase
            {
                PrepareCalled = true;
                return UniTask.CompletedTask;
            }

            public UniTask<TView> ShowAsync<TView>(string addressableKey = null) where TView : UIViewBase
            {
                ShowCalled = true;
                return UniTask.FromResult<TView>(null);
            }

            public UniTask<TView> ShowAsync<TView>(string label, string assetName) where TView : UIViewBase
            {
                ShowCalled = true;
                return UniTask.FromResult<TView>(null);
            }

            public UniTask HideAsync<TView>() where TView : UIViewBase
            {
                HideCalled = true;
                return UniTask.CompletedTask;
            }

            public UniTask ReleaseAsync<TView>() where TView : UIViewBase => UniTask.CompletedTask;

            public bool TryGetView<TView>(out TView view) where TView : UIViewBase
            {
                view = null;
                return false;
            }

            public UniTask PopAsync() => UniTask.CompletedTask;

            public UniTask ClearStackAsync() => UniTask.CompletedTask;
        }
    }
}
