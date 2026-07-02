using Cysharp.Threading.Tasks;
using Elder.Framework.Core;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Localize.Interfaces;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Signal.Interfaces;
using System;
using System.Threading;

namespace Elder.Framework.Data.App
{
    internal sealed class DataInitializer : BaseSystem, IDataInitializer
    {
        private ISignalRouter _router;
        private IDataSheetLoader _sheetLoader;
        private IGameDataLoader _gameDataLoader;
        private IBootstrapDataLoader _bootstrapLoader;
        private IDataProvider _dataProvider;
        private ILocaleSystem _localeSystem;
        private ILoggerEx _logger;

        private CancellationTokenSource _cts;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<ISignalRouter>(out _router))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(ISignalRouter)}");
            if (!TryGetSystem<IDataSheetLoader>(out _sheetLoader))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IDataSheetLoader)}");
            if (!TryGetSystem<IGameDataLoader>(out _gameDataLoader))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IGameDataLoader)}");
            if (!TryGetSystem<IBootstrapDataLoader>(out _bootstrapLoader))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IBootstrapDataLoader)}");
            if (!TryGetSystem<IDataProvider>(out _dataProvider))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IDataProvider)}");
            if (!TryGetSystem<ILocaleSystem>(out _localeSystem))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(ILocaleSystem)}");
            if (TryGetSystem<ILoggerPublisher>(out var pub))
                _logger = pub.GetLogger<DataInitializer>();
        }

        public override UniTask InitializeAsync()
        {
            _cts = new CancellationTokenSource(); // [HEAP] 1회 할당
            return UniTask.CompletedTask;
        }

        public override async UniTask PostInitializeAsync()
        {
            await LoadBaseDataAsync(_cts.Token);
        }

        private async UniTask LoadBaseDataAsync(CancellationToken ct)
        {
            try
            {
                await _bootstrapLoader.LoadBootstrapAsync(_sheetLoader, _dataProvider, _localeSystem.GetLanguageCode(), ct);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _logger.Error($"<color=white>[BlobLoad] === FAILED: {ex.Message}\n{ex.StackTrace}</color>");  // [HEAP] 문자열 보간
            }
        }

        public override void PreDispose()
        {
            _cts.Cancel();
        }

        protected override void DisposeManagedResources()
        {
            _cts.Dispose();
        }
    }
}
