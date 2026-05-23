using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Common.Base;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Log.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elder.Framework.Data.App
{
    internal sealed class DataProvider : DisposableBase, IDataProvider, IDataSheetLoader
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IDataDeserializer _deserializer;
        private readonly ILoggerEx _logger;

        // [HEAP] 초기화 시 1회 할당
        private readonly Dictionary<Type, object> _dataHandles = new();

        public DataProvider(IAssetProvider assetProvider, IDataDeserializer deserializer, ILoggerPublisher loggerPublisher)
        {
            _assetProvider = assetProvider;
            _deserializer = deserializer;
            _logger = loggerPublisher.GetLogger<DataProvider>();
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

        public async UniTask LoadSheetAsync<T>(string assetName) where T : unmanaged
        {
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
            foreach (var listObj in _dataHandles.Values)  // [HEAP] Dictionary.Values 열거자 할당
            {
                if (listObj is IList list)
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
