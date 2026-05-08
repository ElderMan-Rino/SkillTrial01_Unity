using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Common.Messages;
using Elder.Framework.Common.Base;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Data.Messages;
using Elder.Framework.Flux.Helpers;
using Elder.Framework.Flux.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
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
        private readonly IGameDataLoader _gameDataLoader;
        private ILoggerEx _logger;

        // [HEAP] 초기화 시 1회 할당
        private readonly Dictionary<Type, object> _dataHandles = new();
        private SubscriptionToken _initSubscription;

        public DataProvider(IFluxRouter router, IAssetProvider assetProvider, IDataDeserializer deserializer, IGameDataLoader gameDataLoader)
        {
            _router = router;
            _assetProvider = assetProvider;
            _deserializer = deserializer;
            _gameDataLoader = gameDataLoader;
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
            _logger.Info($"<color=white>[BlobLoad] Loading: {assetName} ({typeof(T).Name})</color>");  // [HEAP] 문자열 보간
            var handle = await _assetProvider.GetAssetAsync<TextAsset>(assetName);
            if (handle.Asset is null)
            {
                _logger.Warn($"<color=white>[BlobLoad] FAIL - Asset not found: {assetName}</color>");  // [HEAP] 문자열 보간
                return;
            }

            try
            {
                var dataHandle = _deserializer.Deserialize<T>(handle.Asset.bytes);
                GetOrCreateList<T>().Add(dataHandle);
                _logger.Info($"<color=white>[BlobLoad] OK - {assetName} ({typeof(T).Name}) loaded successfully</color>");  // [HEAP] 문자열 보간
            }
            catch (Exception ex)
            {
                _logger.Error($"<color=white>[BlobLoad] FAIL - Deserialize error: {assetName} | {ex.Message}</color>");  // [HEAP] 문자열 보간
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
                _logger.Info("<color=white>[BlobLoad] === Start loading all Blob Data ===</color>");
                // 여기서 기본 BaseData들을 로드해서 
                // 해당 데이터에 있는 것들을 가지고 쭉 로드 진행
                // 언어 데이터는 시스템중 언어 시스템의 언어를 가져와서 처리 
                // GeneratedBlobLoader가 확장 혹은 변경되야됨
                // 다른 데이터들을 로드하는 것은 
                // 어드레서블 에셋을 다운로드 한 다음 로드 과정이 추가되어야함 
                // 
                await _gameDataLoader.LoadAllAsync(this);
                _router.Publish(new FxBaseDataInitialized());
                _logger.Info("<color=white>[BlobLoad] === All Blob Data loaded successfully ===</color>");
            }
            catch (Exception ex)
            {
                _logger.Error($"<color=white>[BlobLoad] === FAILED: {ex.Message}\n{ex.StackTrace}</color>");  // [HEAP] 문자열 보간
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