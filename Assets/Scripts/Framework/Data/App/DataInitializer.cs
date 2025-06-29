using Cysharp.Threading.Tasks;
using Elder.Framework.Blob.Interfaces;
using Elder.Framework.Common.Interfaces;
using Elder.Framework.Common.Messages;
using Elder.Framework.Common.Utils;
using Elder.Framework.Core;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Data.Messages;
using Elder.Framework.Localize.Interfaces;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using Elder.SkillTrial.Resources.Data;
using System;
using System.Threading;

namespace Elder.Framework.Data.App
{
    internal sealed class DataInitializer : BaseSystem, IScopedSystem
    {
        private ISignalRouter _router;
        private IDataSheetLoader _sheetLoader;
        private IGameDataLoader _gameDataLoader;
        private IDataProvider _dataProvider;
        private ILocaleSystem _localeSystem;
        private ILoggerEx _logger;

        private SignalToken _initSubscription;
        private CancellationTokenSource _cts;

        protected override bool OnInjectDependency()
        {
            TryGetSystem<ISignalRouter>(out _router);
            TryGetSystem<IDataSheetLoader>(out _sheetLoader);
            TryGetSystem<IGameDataLoader>(out _gameDataLoader);
            TryGetSystem<IDataProvider>(out _dataProvider);
            TryGetSystem<ILocaleSystem>(out _localeSystem);
            if (TryGetSystem<ILoggerPublisher>(out var pub))
                _logger = pub.GetLogger<DataInitializer>();
            return true;
        }

        public override bool TryInitialize()
        {
            _cts = new CancellationTokenSource(); // [HEAP] 1회 할당
            // [HEAP] Subscribe 내부 핸들러 래핑 객체 1회 할당 (초기화 시점)
            _initSubscription = _router.Subscribe<SystemInitializeSignal>(HandleInitializeSystem);
            return true;
        }

        public void OnRelease()
        {
            Dispose();
        }

        protected override void OnDispose()
        {
            _initSubscription.Dispose();
            _cts.Cancel();
            _cts.Dispose();
        }

        private void HandleInitializeSystem(in SystemInitializeSignal signal)
        {
            LoadBaseDataAsync(_cts.Token).Forget();
        }

        private async UniTaskVoid LoadBaseDataAsync(CancellationToken ct)
        {
            try
            {
                await LoadSheetAsync(SheetKey.BootstrapDataHash, ct);
                var bootDataHandle = _dataProvider.GetData<BootstrapDataRoot>() as IBlobDataHandle<BootstrapDataRoot>;
                if (bootDataHandle is null || !bootDataHandle.IsCreated)
                    throw new InvalidOperationException("Failed to get BootstrapDataRoot from data provider.");

                var sheetHashes = CollectSheetHashes(bootDataHandle);
                for (int i = 0; i < sheetHashes.Length; ++i)
                    await LoadSheetAsync(sheetHashes[i], ct);

                await LoadLocaleDataAsync(ct);

                _router.Publish(new BaseDataInitializedSignal());
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _logger.Error($"<color=white>[BlobLoad] === FAILED: {ex.Message}\n{ex.StackTrace}</color>");  // [HEAP] 문자열 보간
            }
        }

        private async UniTask LoadLocaleDataAsync(CancellationToken ct)
        {
            await LoadSheetAsync(SheetKey.BootstrapLocaleKeyHash, ct);
            var handle = _dataProvider.GetData<BootstrapLocaleKeyRoot>() as IBlobDataHandle<BootstrapLocaleKeyRoot>;
            if (handle is null || !handle.IsCreated)
                throw new InvalidOperationException("Failed to get BootstrapLocaleSheetRoot from data provider.");

            var currentLocale = ResolveLanguageType(_localeSystem.GetLanguageCode());
            var localeHashes = CollectLocaleSheetHashes(handle, currentLocale);
            for (int i = 0; i < localeHashes.Length; ++i)
                await LoadSheetAsync(localeHashes[i], ct);
        }

        private static int[] CollectLocaleSheetHashes(IBlobDataHandle<BootstrapLocaleKeyRoot> handle, LanguageType currentLocale)
        {
            ref var root = ref handle.GetRef();

            int count = 0;
            for (int i = 0; i < root.Rows.Length; ++i)
                if (root.Rows[i].LocaleType == currentLocale) ++count;

            var hashes = new int[count]; // [HEAP]
            int idx = 0;
            for (int i = 0; i < root.Rows.Length; ++i)
            {
                if (root.Rows[i].LocaleType != currentLocale) continue;
                hashes[idx++] = StringHashHelper.ToStableHash(ref root.Rows[i].SheetName);
            }
            return hashes;
        }

        private static int[] CollectSheetHashes(IBlobDataHandle<BootstrapDataRoot> handle)
        {
            ref var root = ref handle.GetRef();
            var hashes = new int[root.Rows.Length]; // [HEAP]
            for (int i = 0; i < root.Rows.Length; ++i)
                hashes[i] = StringHashHelper.ToStableHash(ref root.Rows[i].DataKey);
            return hashes;
        }

        private static LanguageType ResolveLanguageType(string code) => code switch
        {
            "Ko" => LanguageType.Ko,
            "Jp" => LanguageType.Jp,
            _ => LanguageType.En,
        };

        private async UniTask LoadSheetAsync(int dataHash, CancellationToken ct)
        {
            await _gameDataLoader.LoadAsync(_sheetLoader, dataHash).AttachExternalCancellation(ct);
        }
    }
}
