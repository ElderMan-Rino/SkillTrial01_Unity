using Cysharp.Threading.Tasks;
using Elder.Framework.Common.Base;
using Elder.Framework.Common.Interfaces;
using Elder.Framework.Common.Messages;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Data.Messages;
using Elder.Framework.Localize.Interfaces;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Common.Utils;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using Elder.SkillTrial.Resources.Data;
using System;

namespace Elder.Framework.Data.App
{
    internal sealed class DataInitializer : DisposableBase, IScopedSystem
    {
        private readonly ISignalRouter _router;
        private readonly IDataSheetLoader _sheetLoader;
        private readonly IGameDataLoader _gameDataLoader;
        private readonly IDataProvider _dataProvider;
        private readonly ILocaleSystem _localeSystem;
        private readonly ILoggerEx _logger;

        private SignalToken _initSubscription;

        public DataInitializer(ISignalRouter router, IDataSheetLoader sheetLoader, IGameDataLoader gameDataLoader, IDataProvider dataProvider, ILocaleSystem localeSystem, ILoggerPublisher loggerPublisher)
        {
            _router = router;
            _sheetLoader = sheetLoader;
            _gameDataLoader = gameDataLoader;
            _dataProvider = dataProvider;
            _localeSystem = localeSystem;
            _logger = loggerPublisher.GetLogger<DataInitializer>();
        }

        public void Initialize()
        {
            // [HEAP] Subscribe 내부 핸들러 래핑 객체 1회 할당 (초기화 시점)
            _initSubscription = _router.Subscribe<SystemInitializeSignal>(HandleInitializeSystem);
        }

        public void OnRelease()
        {
            Dispose();
        }

        protected override void DisposeManagedResources()
        {
            _initSubscription.Dispose();
            base.DisposeManagedResources();
        }

        private void HandleInitializeSystem(in SystemInitializeSignal signal)
        {
            LoadBaseDataAsync().Forget();
        }

        private async UniTaskVoid LoadBaseDataAsync()
        {
            try
            {
                await LoadSheetAsync(SheetKey.BootstrapDataHash);
                var bootDataHandle = _dataProvider.GetData<BootstrapDataRoot>();
                if (!bootDataHandle.TryGetData(out var bootStrapData))
                    throw new InvalidOperationException("Failed to get BootstrapDataRoot from data provider.");

                for (int i = 0; i < bootStrapData.Rows.Length; ++i)
                    await LoadSheetAsync(StringHashHelper.ToStableHash(ref bootStrapData.Rows[i].Key));

                await LoadLocaleDataAsync();

                _router.Publish(new BaseDataInitializedSignal());
            }
            catch (Exception ex)
            {
                _logger.Error($"<color=white>[BlobLoad] === FAILED: {ex.Message}\n{ex.StackTrace}</color>");  // [HEAP] 문자열 보간
            }
        }

        private async UniTask LoadLocaleDataAsync()
        {
            await LoadSheetAsync(SheetKey.BootstrapLocaleSheetHash);
            var handle = _dataProvider.GetData<BootstrapLocaleSheetRoot>();
            if (!handle.TryGetData(out var localeSheet))
                throw new InvalidOperationException("Failed to get BootstrapLocaleSheetRoot from data provider.");

            var currentLocale = ResolveLanguageType(_localeSystem.GetLanguageCode());

            //for (int i = 0; i < localeSheet.Rows.Length; ++i)
            //{
            //    ref var row = ref localeSheet.Rows[i];
            //    if (row.LocaleType != currentLocale) continue;
            //    await LoadSheetAsync(StringHashHelper.ToStableHash(ref row.SheetName));
            //}
        }

        private static LanguageType ResolveLanguageType(string code) => code switch
        {
            "Ko" => LanguageType.Ko,
            "Jp" => LanguageType.Jp,
            _    => LanguageType.En,
        };

        private async UniTask LoadSheetAsync(int dataHash)
        {
            await _gameDataLoader.LoadAsync(_sheetLoader, dataHash);
        }
    }
}
