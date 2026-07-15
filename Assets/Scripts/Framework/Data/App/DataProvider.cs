using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core;
using Elder.Framework.Blob.Interfaces;
using Elder.Framework.Log.Interfaces;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Elder.Framework.Blob.App
{
    internal sealed class DataProvider : BaseSystem, IDataProvider, IDataSheetLoader
    {
        private IAssetProvider _assetProvider;
        private IDataDeserializer _deserializer;
        private ILoggerEx _logger;

        // [HEAP] 초기화 시 1회 할당
        private readonly Dictionary<Type, IDataHandleList> _dataHandles = new();

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<IAssetProvider>(out _assetProvider))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IAssetProvider)}");
            if (!TryGetSystem<IDataDeserializer>(out _deserializer))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IDataDeserializer)}");
            if (TryGetSystem<ILoggerPublisher>(out var pub))
                _logger = pub.GetLogger<DataProvider>();
        }

        public bool TryGetBlobReference<T>(out BlobAssetReference<T> reference) where T : unmanaged
        {
            if (_dataHandles.TryGetValue(typeof(T), out var listObj))
            {
                var list = (DataHandleList<T>)listObj;
                if (list.Count > 0)
                {
                    reference = list[0].GetRawReference();
                    return true;
                }
            }
            reference = default;
            return false;
        }

        public async UniTask LoadSheetAsync<T>(string assetName, int scope) where T : unmanaged
        {
            var handle = await _assetProvider.GetAssetAsync<TextAsset>(assetName);
            if (handle.Asset is null)
            {
                _logger.Warn($"<color=white>[BlobLoad] FAIL - Asset not found: {assetName}</color>");  // [HEAP] 문자열 보간
                return;
            }

            try
            {
                byte[] bytes = handle.Asset.bytes;

                await UniTask.SwitchToThreadPool();
                var dataHandle = _deserializer.Deserialize<T>(bytes);  // AES 복호화 + BlobAsset 역직렬화 — 순수 CPU
                await UniTask.SwitchToMainThread();

                GetOrCreateList<T>(scope).Add(dataHandle);
            }
            catch (Exception ex)
            {
                await UniTask.SwitchToMainThread();
                _logger.Error($"<color=white>[BlobLoad] FAIL - Deserialize error: {assetName} | {ex.Message}</color>");  // [HEAP] 문자열 보간
            }
            finally
            {
                handle.Dispose();
            }
        }

        protected override void DisposeManagedResources()
        {
            DisposeDataHandles();
        }


        private DataHandleList<T> GetOrCreateList<T>(int scope) where T : unmanaged
        {
            if (!_dataHandles.TryGetValue(typeof(T), out var listObj))
            {
                // [HEAP] 첫 로드 시 타입당 1회 할당
                listObj = new DataHandleList<T>(scope);
                _dataHandles[typeof(T)] = listObj;
            }
            return (DataHandleList<T>)listObj;
        }

      

        public override UniTask InitializeAsync() => UniTask.CompletedTask;

        public override UniTask PostInitializeAsync() => UniTask.CompletedTask;
        private void DisposeDataHandles()
        {
            foreach (var pair in _dataHandles)  // Dictionary enumerator is a value-type struct — no heap alloc
                pair.Value.DisposeAll();
            _dataHandles.Clear();
        }
    }
}
