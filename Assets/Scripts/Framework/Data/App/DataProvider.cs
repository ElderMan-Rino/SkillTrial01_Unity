using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Boot.Messages;
using Elder.Framework.Common.Base;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Data.Messages;
using Elder.Framework.Flux.Helpers;
using Elder.Framework.Flux.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.SkillTrial.Resources.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace Elder.Framework.Data.App
{
    internal sealed class DataProvider : DisposableBase, IDataProvider, IDataSheetLoader, IInitializable
    {
        private readonly IFluxRouter _router;
        private readonly IAssetProvider _assetProvider;
        private readonly IDataDeserializer _deserializer;
        private ILoggerEx _logger;

        // [HEAP] 초기화 시 1회 할당
        private readonly Dictionary<Type, object> _dataHandles = new();
        private SubscriptionToken _initSubscription;

        public DataProvider(IFluxRouter router, IAssetProvider assetProvider, IDataDeserializer deserializer)
        {
            _router = router;
            _assetProvider = assetProvider;
            _deserializer = deserializer;
        }

        public void Initialize()
        {
            _logger = LogFacade.GetLoggerFor<DataProvider>();
            // [HEAP] Subscribe 내부 핸들러 래핑 객체 1회 할당 (초기화 시점)
            _initSubscription = _router.Subscribe<FxInitializeSystem>(HandleInitializeSystem);
        }

        public IDataHandle<T> GetData<T>() where T : unmanaged
        {
            if (_dataHandles.TryGetValue(typeof(T), out var listObj))
            {
                var list = (List<IDataHandle<T>>)listObj;
                if (list.Count > 0) return list[0];
            }
            return null;
        }

        public IReadOnlyList<IDataHandle<T>> GetAllData<T>() where T : unmanaged
        {
            if (_dataHandles.TryGetValue(typeof(T), out var listObj))
                return (List<IDataHandle<T>>)listObj;

            return Array.Empty<IDataHandle<T>>();
        }

        public async UniTask LoadSheetAsync<T>(string assetName) where T : unmanaged
        {
            var handle = await _assetProvider.GetAssetAsync<TextAsset>(assetName);
            if (handle.Asset is null)
            {
                _logger.Warn($"Failed to load blob asset: {assetName}");  // [HEAP] 문자열 보간
                return;
            }

            try
            {
                var dataHandle = _deserializer.Deserialize<T>(handle.Asset.bytes);
                GetOrCreateList<T>().Add(dataHandle);
            }
            finally
            {
                handle.Dispose();
            }
        }

        protected override void DisposeManagedResources()
        {
            _initSubscription.Dispose();

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
            base.DisposeManagedResources();
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
                await GeneratedBlobLoader.LoadAllDataAsync(this);
                _router.Publish(new FxBaseDataInitialized());
                _logger.Info("All Blob Data loaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to load Blob Data: {ex.Message}\n{ex.StackTrace}");  // [HEAP] 문자열 보간
            }
        }

        private List<IDataHandle<T>> GetOrCreateList<T>() where T : unmanaged
        {
            if (!_dataHandles.TryGetValue(typeof(T), out var listObj))
            {
                // [HEAP] 첫 로드 시 타입당 1회 할당
                listObj = new List<IDataHandle<T>>();
                _dataHandles[typeof(T)] = listObj;
            }
            return (List<IDataHandle<T>>)listObj;
        }
    }
}