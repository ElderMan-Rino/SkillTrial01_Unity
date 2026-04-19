using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Blob.App;
using Elder.Framework.Blob.Interfaces;
using Elder.Framework.Boot.Messages;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Data.Messages;
using Elder.Framework.Flux.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.SkillTrial.Resources.Data;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using VContainer.Unity;

namespace Elder.Framework.Data.App
{
    public class DataProvider : IDisposable, IDataProvider, IDataSheetLoader, IInitializable
    {
        private readonly IFluxRouter _router;
        private readonly IAssetProvider _assetProvider;
        private readonly IDataDeserializer _deserializer;
        private ILoggerEx _logger;

        private readonly Dictionary<Type, object> _dataHandles = new();

        public DataProvider(IFluxRouter router, IAssetProvider assetProvider, IDataDeserializer deserializer)
        {
            _router = router;
            _assetProvider = assetProvider;
            _deserializer = deserializer;
        }

        public void Initialize()
        {
            _logger = LogFacade.GetLoggerFor<DataProvider>();
            _router.Subscribe<FxInitializeSystem>(HandleInitializeSystem);
        }

        private void HandleInitializeSystem(in FxInitializeSystem message)
        {
            LoadBaseDataAsync().Forget();
        }

        private async UniTaskVoid LoadBaseDataAsync()
        {
            try
            {
                _logger.Info("Starting to load Blob Data...");

                var generatedLoader = new GeneratedBlobLoader();
                await generatedLoader.LoadAllDataAsync(this);

                _router.Publish(new FxBaseDataInitialized());
                _logger.Info("All Blob Data loaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to load Blob Data: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public async UniTask LoadSheetAsync<T>(string assetName) where T : unmanaged
        {
            var handle = await _assetProvider.GetAssetAsync<TextAsset>(assetName);
            if (handle.Asset == null)
            {
                _logger.Warn($"Failed to load blob asset: {assetName}");
                return;
            }
            
            try
            {
                IDataHandle<T> dataHandle = _deserializer.Deserialize<T>(handle.Asset.bytes);
                if (!_dataHandles.TryGetValue(typeof(T), out var listObj))
                {
                    listObj = new List<IDataHandle<T>>();
                    _dataHandles[typeof(T)] = listObj;
                }

                var list = (List<IDataHandle<T>>)listObj;
                list.Add(dataHandle);
            }
            finally
            {
                handle.Dispose(); // TextAsset 해제
            }
        }

        // 단일 조회 (가장 처음 로드된 시트 반환)
        public IDataHandle<T> GetData<T>() where T : unmanaged
        {
            if (_dataHandles.TryGetValue(typeof(T), out var listObj))
            {
                var list = (List<IDataHandle<T>>)listObj;
                if (list.Count > 0) return list[0];
            }

            // IDataHandle은 인터페이스(참조 타입)이므로 default 대신 null을 반환합니다.
            return null;
        }

        // 다중 시트 일괄 조회 (GC 할당 없음)
        public IReadOnlyList<IDataHandle<T>> GetAllData<T>() where T : unmanaged
        {
            if (_dataHandles.TryGetValue(typeof(T), out var listObj))
            {
                return (List<IDataHandle<T>>)listObj;
            }

            return Array.Empty<IDataHandle<T>>();
        }

        public void Dispose()
        {
            // 게임 종료 시 메모리에 남아있는 모든 Blob 데이터 파괴
            foreach (var listObj in _dataHandles.Values)
            {
                if (listObj is System.Collections.IList list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] is IDisposable handle) handle.Dispose();
                    }
                }
            }
            _dataHandles.Clear();
        }
    }
}