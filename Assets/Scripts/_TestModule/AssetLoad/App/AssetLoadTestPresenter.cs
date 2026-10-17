using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Core.UI.App;
using Elder.Framework.Core.UI.Interfaces;
using Elder.Framework.Log.Definitions.Enums;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using R3;
using System;

namespace Elder.TestModule.AssetLoad.App
{
    internal sealed class AssetLoadTestPresenter : UIPresenterBase
    {
        private ILoggerEx _logger = LogFacade.GetLogger<AssetLoadTestPresenter>();

        private IAssetCacheTest _assetCacheTest;
        private AssetLoadTestView _view;
        private IDisposable _subscriptions;
        private long _lastTotalAllocatedBytes;

        public override void BindView(IUIView view)
        {
            if (view is not AssetLoadTestView assetLoadTestView)
            {
                _logger.Error($"Failed to bind: view is not {nameof(AssetLoadTestView)}."); // [HEAP]
                return;
            }

            _view = assetLoadTestView;
            _subscriptions = Disposable.Combine // [HEAP]
            (
                _view.OnLoadAssetRequested.Subscribe(_ => HandleLoadAssetRequested()),
                _view.OnListCacheRequested.Subscribe(_ => HandleListCacheRequested()),
                _view.OnReleaseAllRequested.Subscribe(_ => HandleReleaseAllRequested()),
                _view.OnMemoryStatusRequested.Subscribe(_ => HandleMemoryStatusRequested()),
                _view.OnClearLogRequested.Subscribe(_ => _view.ClearLog())
            );
        }

        protected override void OnInjectDependency()
        {
            if (!_provider.TryGetComponent<IAssetCacheTest>(out _assetCacheTest))
            {
                _logger.Error($"Failed to resolve {nameof(IAssetCacheTest)}."); // [HEAP]
                return;
            }
        }

        protected override void OnDispose()
        {
            _subscriptions?.Dispose();
            _subscriptions = null;
            _view = null;
            _logger = null;
        }

        private void HandleLoadAssetRequested()
        {
            LoadAssetAsync().Forget();
        }

        private async UniTaskVoid LoadAssetAsync()
        {
            _view.SetLoadAssetButtonBusy(true);
            try
            {
                await _assetCacheTest.RegisterAllKeysAsync();
                foreach (var key in _assetCacheTest.GetCachedKeys())
                {
                    _view.AppendLog(LogLevel.Info, $"Loaded: {key}"); // [HEAP]
                }
            }
            catch (Exception exception)
            {
                _view.AppendLog(LogLevel.Error, $"Load failed: {exception.Message}"); // [HEAP]
            }
            finally
            {
                _view.SetLoadAssetButtonBusy(false);
            }
        }

        private void HandleListCacheRequested()
        {
            foreach (var key in _assetCacheTest.GetCachedKeys())
            {
                _view.AppendLog(LogLevel.Debug, $"Cached: {key}"); // [HEAP]
            }
        }

        private void HandleReleaseAllRequested()
        {
            ReleaseAllAsync().Forget();
        }

        private async UniTaskVoid ReleaseAllAsync()
        {
            await _assetCacheTest.ReleaseAllAsync();
            _view.AppendLog(LogLevel.Info, "Released all cached assets.");
        }

        private void HandleMemoryStatusRequested()
        {
            var status = _assetCacheTest.GetMemoryStatus();
            var deltaBytes = status.TotalAllocatedBytes - _lastTotalAllocatedBytes;
            _lastTotalAllocatedBytes = status.TotalAllocatedBytes;
            var deltaSign = deltaBytes >= 0 ? "+" : "-";
            _view.AppendLog(LogLevel.Info, $"Cached keys: {status.CachedKeyCount}, Labels: {status.RegisteredLabelCount}, Total allocated: {status.TotalAllocatedBytes / 1024f:F1} KB (Δ {deltaSign}{Math.Abs(deltaBytes) / 1024f:F1} KB)"); // [HEAP]
        }
    }
}
