using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.GameMode.Interfaces;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Scene.Domain.Models;
using Elder.Framework.Scene.Interfaces;
using Elder.Framework.Scene.Messages;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using Elder.Framework.UI.Interfaces;
using System;

namespace Elder.Framework.GameMode.App
{
    internal sealed class TransitionDirector : BaseSystem, ITransitionDirector
    {
        private ISignalRouter _router;
        private ISceneChanger _sceneLoader;
        private IGameModeOrchestratorFactory _factory;
        private IGameSystemRegistryFactory _registryFactory;
        private IUISystem _uiSystem;
        private ILoggerEx _logger;
        private IGameModeOrchestrator _activeOrchestrator;

        private SignalToken _subscription;

        private SceneTransitionState _state = SceneTransitionState.Idle;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<ISignalRouter>(out _router))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(ISignalRouter)}");
            if (!TryGetSystem<ISceneChanger>(out _sceneLoader))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(ISceneChanger)}");
            if (!TryGetSystem<IGameModeOrchestratorFactory>(out _factory))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IGameModeOrchestratorFactory)}");
            if (!TryGetSystem<IGameSystemRegistryFactory>(out _registryFactory))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IGameSystemRegistryFactory)}");
            if (!TryGetSystem<IUISystem>(out _uiSystem))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IUISystem)}");
            if (!TryGetSystem<ILoggerPublisher>(out var loggerPublisher))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(ILoggerPublisher)}");
            _logger = loggerPublisher.GetLogger<TransitionDirector>();
        }

        public override UniTask InitializeAsync()
        {
            _subscription = _router.Subscribe<SceneTransitionSignal>(HandleSceneTransition);
            return UniTask.CompletedTask;
        }

        private void HandleSceneTransition(in SceneTransitionSignal signal)
        {
            if (_state == SceneTransitionState.InProgress)
            {
                // [HEAP] 문자열 보간
                _logger.Warn($"씬 전환 중 중복 요청 무시: {signal.TargetSceneKey}");
                return;
            }
            ProcessAsync(signal.TargetSceneKey).Forget();
        }

        private async UniTaskVoid ProcessAsync(string targetSceneKey)
        {
            _state = SceneTransitionState.InProgress;
            try
            {
                _router.Publish(new SceneTransitionStartedSignal(targetSceneKey));
                await TeardownActiveModeAsync();
                await LoadSceneAsync(targetSceneKey);
                await LoadOrchestratorAsync(targetSceneKey);
                _router.Publish(new SceneTransitionCompletedSignal(targetSceneKey));
            }
            catch (Exception ex)
            {
                // [HEAP] 문자열 보간
                _logger.Error($"씬 전환 예외 [{targetSceneKey}]: {ex.Message}");
            }
            finally
            {
                _state = SceneTransitionState.Idle;
            }
        }

        private async UniTask TeardownActiveModeAsync()
        {
            if (_activeOrchestrator is null) return;
            await _activeOrchestrator.TeardownAsync();
            DisposeActiveOrchestrator();
        }

        private async UniTask LoadSceneAsync(string targetSceneKey)
        {
            var isSuccess = await _sceneLoader.ExecuteAsync(targetSceneKey);
            if (!isSuccess)
            {
                // [HEAP] 문자열 보간 + new InvalidOperationException
                _logger.Error($"씬 전환 실패: {targetSceneKey}");
                throw new InvalidOperationException($"씬 전환 실패: {targetSceneKey}");
            }
        }

        private async UniTask LoadOrchestratorAsync(string targetSceneKey)
        {
            await _uiSystem.OnSceneLoadedAsync();
            if (_factory.HaveOrchestrator(targetSceneKey))
            {
                var registry = _registryFactory.CreateRegistry(_systemProvider);
                if (!_factory.TryCreateOrchestrator(targetSceneKey, registry, out _activeOrchestrator))
                {
                    // [HEAP] 문자열 보간 + new InvalidOperationException
                    throw new InvalidOperationException($"게임 모드 설정 실패: {targetSceneKey}");
                }

                bool isPrepared = await _activeOrchestrator.TryPrepareAsync();
                if (!isPrepared)
                {
                    // [HEAP] 문자열 보간 + new InvalidOperationException
                    _logger.Error($"게임 모드 설정 실패: {targetSceneKey}");
                    DisposeActiveOrchestrator();
                    throw new InvalidOperationException($"게임 모드 설정 실패: {targetSceneKey}");
                }

                bool isActivated = await _activeOrchestrator.TryActivateAsync();
                if (!isActivated)
                {
                    // [HEAP] 문자열 보간 + new InvalidOperationException
                    _logger.Error($"게임 모드 활성화 실패: {targetSceneKey}");
                    DisposeActiveOrchestrator();
                    throw new InvalidOperationException($"게임 모드 활성화 실패: {targetSceneKey}");
                }
            }
        }

        protected override void DisposeManagedResources()
        {
            _subscription.Dispose();
            DisposeActiveOrchestrator();
        }

        private void DisposeActiveOrchestrator()
        {
            if (_activeOrchestrator is IDisposableBase disposable)
            {
                disposable.PreDispose();
                disposable.Dispose();
            }
            _activeOrchestrator = null;
        }
    }
}
