using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Data.App;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Flux.Definitions;
using Elder.Framework.Flux.Infra;
using Elder.Framework.Flux.Interfaces;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Elder.Framework.Tests.Data
{
    internal sealed class DataProviderTests
    {
        private FluxRouter _router;
        private StubAssetProvider _assetProvider;
        private StubDeserializer _deserializer;
        private StubGameDataLoader _gameDataLoader;
        private DataProvider _provider;

        [SetUp]
        public void SetUp()
        {
            _router = new FluxRouter();
            _assetProvider = new StubAssetProvider();
            _deserializer = new StubDeserializer();
            _gameDataLoader = new StubGameDataLoader();

            _provider = new DataProvider(_router, _assetProvider, _deserializer, _gameDataLoader);
            _provider.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            _provider.Dispose();
            _router.Dispose();
        }

        // ─── GetData ───────────────────────────────────────────────────────────

        [Test]
        public void GetData_BeforeLoad_ReturnsNull()
        {
            var result = _provider.GetData<DummyData>();
            Assert.IsNull(result);
        }

        [Test]
        public void GetAllData_BeforeLoad_ReturnsEmpty()
        {
            var result = _provider.GetAllData<DummyData>();
            Assert.AreEqual(0, result.Count);
        }

        // ─── LoadSheetAsync — null asset ───────────────────────────────────────

        [UnityTest]
        public IEnumerator LoadSheetAsync_WhenAssetIsNull_DoesNotAddToCache()
        {
            _assetProvider.ReturnNullAsset = true;

            yield return _provider.LoadSheetAsync<DummyData>("missing").ToCoroutine();

            Assert.IsNull(_provider.GetData<DummyData>());
        }

        // ─── LoadSheetAsync — successful load ─────────────────────────────────

        [UnityTest]
        public IEnumerator LoadSheetAsync_WhenDeserializerSucceeds_DataIsRetrievable()
        {
            var fakeHandle = new FakeDataHandle();
            _deserializer.ReturnHandle = fakeHandle;

            yield return _provider.LoadSheetAsync<DummyData>("someKey").ToCoroutine();

            var result = _provider.GetData<DummyData>();
            Assert.IsNotNull(result);
            Assert.AreSame(fakeHandle, result);
        }

        [UnityTest]
        public IEnumerator LoadSheetAsync_CalledTwice_GetAllDataReturnsBoth()
        {
            _deserializer.ReturnHandle = new FakeDataHandle();
            yield return _provider.LoadSheetAsync<DummyData>("a").ToCoroutine();

            _deserializer.ReturnHandle = new FakeDataHandle();
            yield return _provider.LoadSheetAsync<DummyData>("b").ToCoroutine();

            var all = _provider.GetAllData<DummyData>();
            Assert.AreEqual(2, all.Count);
        }

        // ─── LoadSheetAsync — deserializer throws ─────────────────────────────

        [UnityTest]
        public IEnumerator LoadSheetAsync_WhenDeserializerThrows_DataNotAdded()
        {
            _deserializer.ThrowOnDeserialize = true;

            yield return _provider.LoadSheetAsync<DummyData>("bad").ToCoroutine();

            Assert.IsNull(_provider.GetData<DummyData>());
        }

        // ─── FxInitializeSystem → LoadAll → FxBaseDataInitialized ─────────────

        [UnityTest]
        public IEnumerator OnFxInitializeSystem_LoadAllCalledAndBaseDataInitializedPublished()
        {
            var published = false;
            _router.Subscribe<Elder.Framework.Data.Messages.FxBaseDataInitialized>(
                (MessageHandler<Elder.Framework.Data.Messages.FxBaseDataInitialized>)
                ((in Elder.Framework.Data.Messages.FxBaseDataInitialized _) => published = true));

            _router.Publish(new Elder.Framework.Common.Messages.FxInitializeSystem());

            // Wait one frame for the async fire-and-forget to complete
            yield return null;
            yield return null;

            Assert.IsTrue(_gameDataLoader.LoadAllCalled);
            Assert.IsTrue(published);
        }

        // ─── Dispose ───────────────────────────────────────────────────────────

        [UnityTest]
        public IEnumerator Dispose_HandlesAreDisposed()
        {
            var fakeHandle = new FakeDataHandle();
            _deserializer.ReturnHandle = fakeHandle;
            yield return _provider.LoadSheetAsync<DummyData>("k").ToCoroutine();

            _provider.Dispose();

            Assert.IsTrue(fakeHandle.Disposed);
        }

        // ─── Stubs & fixtures ──────────────────────────────────────────────────

        private struct DummyData { }

        private sealed class StubAssetProvider : IAssetProvider
        {
            public bool ReturnNullAsset;

            public UniTask<IAssetHandle<T>> GetAssetAsync<T>(string key) where T : UnityEngine.Object
            {
                if (ReturnNullAsset)
                    return UniTask.FromResult<IAssetHandle<T>>(new NullAssetHandle<T>());

                var text = new TextAsset("dummy");
                // [HEAP] stub allocation in test setup — acceptable
                return UniTask.FromResult<IAssetHandle<T>>(new FakeAssetHandle<T>((T)(object)text));
            }
        }

        private sealed class NullAssetHandle<T> : IAssetHandle<T> where T : UnityEngine.Object
        {
            public T Asset => null;
            public void Dispose() { }
        }

        private sealed class FakeAssetHandle<T> : IAssetHandle<T> where T : UnityEngine.Object
        {
            public T Asset { get; }
            public FakeAssetHandle(T asset) { Asset = asset; }
            public void Dispose() { }
        }

        private sealed class StubDeserializer : IDataDeserializer
        {
            public FakeDataHandle ReturnHandle;
            public bool ThrowOnDeserialize;

            public IDataHandle<T> Deserialize<T>(byte[] data) where T : unmanaged
            {
                if (ThrowOnDeserialize)
                    throw new InvalidOperationException("test error");

                // Test stub: only called with T=DummyData; explicit cast is safe in test context
                return (IDataHandle<T>)(object)ReturnHandle;
            }
        }

        private sealed class StubGameDataLoader : IGameDataLoader
        {
            public bool LoadAllCalled;

            public UniTask LoadAllAsync(IDataSheetLoader loader)
            {
                LoadAllCalled = true;
                return UniTask.CompletedTask;
            }

            public UniTask LoadAsync<T>(IDataSheetLoader loader, string key) where T : unmanaged
                => UniTask.CompletedTask;
        }

        private sealed class FakeDataHandle : IDataHandle<DummyData>
        {
            public bool Disposed;
            public bool TryGetData(out DummyData data) { data = default; return true; }
            public void Dispose() { Disposed = true; }
        }
    }
}
