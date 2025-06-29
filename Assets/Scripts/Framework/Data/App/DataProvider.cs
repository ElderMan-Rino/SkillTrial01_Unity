using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Boot.Messages;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Flux.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace Elder.Framework.Data.App
{
    public class DataProvider : IDisposable, IDataProvider, IInitializable
    {
        private readonly IFluxRouter _router;
        private readonly IDataDeserializer _deserializer;
        private readonly IAssetProvider _assetProvider;
        private readonly IDataConfig _dataConfig;

        private ILoggerEx _logger;
        private GameDataContainer _gameData;

        public DataProvider(IFluxRouter router, IDataDeserializer deserializer, IAssetProvider assetProvider, IDataConfig dataConfig)
        {
            _router = router;
            _deserializer = deserializer;
            _assetProvider = assetProvider;
            _dataConfig = dataConfig;
        }

        public void Initialize()
        {
            InitializeLogger();
            SubscribeToFluxEvent();
        }

        private void SubscribeToFluxEvent()
        {
            _router.Subscribe<FxInitializeSystem>(HandleInitializeSystem);
        }

        private void HandleInitializeSystem(in FxInitializeSystem message)
        {
            // 비동기 로드를 실행하고 잊습니다 (Fire and Forget)
            LoadBaseDataAsync().Forget();
        }

        private async UniTaskVoid LoadBaseDataAsync()
        {
            var baseDataKey = _dataConfig.BaseDataKey;
            if (string.IsNullOrEmpty(baseDataKey))
            {
                _logger.Error("BaseDataKey is null or empty in FrameworkSettings!");
                return;
            }

            _logger.Info($"Starting to load Base Data using key: {baseDataKey}");

            try
            {
                // 1. Asset System을 통해 바이너리 파일(TextAsset) 로드
                var handle = await _assetProvider.GetAssetAsync<TextAsset>(baseDataKey);

                // 2. 바이너리 데이터를 GameDataContainer 객체로 역직렬화
                _gameData = _deserializer.Deserialize<GameDataContainer>(handle.Asset.bytes);
                _gameData.Initialize(); // 내부 Dictionary 매핑 초기화

                // 3. 파싱이 끝났으므로 원본 TextAsset의 메모리 해제 (최적화)
                handle.Dispose();

                _logger.Info("Base Data loaded and parsed successfully.");

                // TODO: 데이터 로드가 완료되었음을 알리는 Flux 메시지 발행 고려 (예: FxDataLoaded)
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to load Base Data: {ex.Message}");
            }
        }

        public T GetData<T>(int id) where T : class, IDataRecord
        {
            if (_gameData == null)
            {
                _logger.Warn("GameData has not been loaded yet!");
                return null;
            }
            return _gameData.GetData<T>(id);
        }

        public IReadOnlyList<T> GetAllData<T>() where T : class, IDataRecord
        {
            if (_gameData == null)
            {
                _logger.Warn("GameData has not been loaded yet!");
                return Array.Empty<T>();
            }
            return _gameData.GetAllData<T>();
        }

        private void InitializeLogger()
        {
            _logger = LogFacade.GetLoggerFor<DataProvider>();
        }

        public void Dispose()
        {
            _gameData = null;
        }
    }
}