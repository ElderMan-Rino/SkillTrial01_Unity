using Cysharp.Threading.Tasks;
using Elder.Framework.Common.Base;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Scene.Domain.Models;
using Elder.Framework.Scene.Interfaces;
using Elder.Framework.Scene.Messages;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using System;

namespace Elder.Framework.Scene.App
{
    internal sealed class SceneTransitionCoordinator : DisposableBase, ISceneTransitionCoordinator
    {
        private readonly ISignalRouter _router;
        private readonly ISceneTransitionExecutor _executor;
        private readonly ILoggerEx _logger;

        private SignalToken _sceneTransitionSubscription;
        private SceneTransitionState _state = SceneTransitionState.Idle;

        public SceneTransitionCoordinator(ISignalRouter router, ISceneTransitionExecutor executor, ILoggerPublisher loggerPublisher)
        {
            _router = router;
            _executor = executor;
            _logger = loggerPublisher.GetLogger<SceneTransitionCoordinator>();
        }

        public void Initialize()
        {
            // [HEAP] Subscribe 내부 핸들러 래핑 객체 1회 할당 (초기화 시점)
            _sceneTransitionSubscription = _router.Subscribe<SceneTransitionSignal>(HandleSceneTransition);
        }

        protected override void DisposeManagedResources()
        {
            _sceneTransitionSubscription.Dispose();
            base.DisposeManagedResources();
        }

        private void HandleSceneTransition(in SceneTransitionSignal signal)
        {
            if (_state == SceneTransitionState.InProgress)
            {
                _logger.Warn($"씬 전환 중 중복 요청 무시: {signal.TargetSceneKey}");  // [HEAP] 문자열 보간
                return;
            }

            ProcessSceneTransitionAsync(signal.TargetSceneKey).Forget();
        }

        private async UniTaskVoid ProcessSceneTransitionAsync(string targetSceneKey)
        {
            _state = SceneTransitionState.InProgress;

            try
            {
                _router.Publish(new SceneTransitionStartedSignal(targetSceneKey));

                var success = await _executor.ExecuteAsync(targetSceneKey);
                if (!success)
                    _logger.Error($"씬 전환 실패: {targetSceneKey}");  // [HEAP] 문자열 보간
                else
                    _router.Publish(new SceneTransitionCompletedSignal(targetSceneKey));
            }
            catch (Exception ex)
            {
                _logger.Error($"씬 전환 예외 [{targetSceneKey}]: {ex.Message}");  // [HEAP] 문자열 보간
            }
            finally
            {
                _state = SceneTransitionState.Idle;
            }
        }
    }
}
