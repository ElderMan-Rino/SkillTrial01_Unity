using Cysharp.Threading.Tasks;
using Elder.Framework.Blob.Interfaces;
using Elder.Framework.Data.Interfaces;
using Elder.SkillTrial.Resources.Data;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.TestTools;

namespace Elder.Framework.Tests.Data
{
    internal sealed class GeneratedBlobLoaderTests
    {
        private GeneratedBlobLoader _loader;
        private StubSheetLoader _sheetLoader;
        private StubDataProvider _dataProvider;

        [SetUp]
        public void SetUp()
        {
            _loader = new GeneratedBlobLoader();
            _sheetLoader = new StubSheetLoader();
            _dataProvider = new StubDataProvider();
        }

        // ─── LoadAllAsync ──────────────────────────────────────────────────────

        [UnityTest]
        public IEnumerator LoadAllAsync_WithBootstrapRows_LoadsAllExpectedKeys()
        {
            // 7 rows: GraphicsSettings, AudioSettings, ErrorCode, SceneInfo, LocaleSettings,
            //         BootstrapLocale_{0}, ErrorMsgLocale_{0}  → resolved with "Ko"
            var expectedKeys = new HashSet<string>
            {
                SheetKey.BootstrapData,
                SheetKey.GraphicsSettings,
                SheetKey.AudioSettings,
                SheetKey.ErrorCode,
                SheetKey.SceneInfo,
                SheetKey.LocaleSettings,
                "BootstrapLocale_Ko",
                "ErrorMsgLocale_Ko",
            };

            _dataProvider.BootstrapHandle = BuildBootstrapHandle();

            yield return _loader.LoadAllAsync(_sheetLoader, _dataProvider, "Ko").ToCoroutine();

            foreach (var key in expectedKeys)
                Assert.IsTrue(_sheetLoader.LoadedKeys.Contains(key), $"Expected key not loaded: {key}");
        }

        [UnityTest]
        public IEnumerator LoadAllAsync_WithEnglishLanguage_ResolvesEnLocaleKeys()
        {
            _dataProvider.BootstrapHandle = BuildBootstrapHandle();

            yield return _loader.LoadAllAsync(_sheetLoader, _dataProvider, "En").ToCoroutine();

            Assert.IsTrue(_sheetLoader.LoadedKeys.Contains("BootstrapLocale_En"));
            Assert.IsTrue(_sheetLoader.LoadedKeys.Contains("ErrorMsgLocale_En"));
        }

        [UnityTest]
        public IEnumerator LoadAllAsync_WhenBootstrapHandleNull_DoesNotThrow()
        {
            _dataProvider.BootstrapHandle = null;

            yield return _loader.LoadAllAsync(_sheetLoader, _dataProvider, "Ko").ToCoroutine();

            // Only BootstrapData itself was attempted
            Assert.AreEqual(1, _sheetLoader.LoadedKeys.Count);
            Assert.AreEqual(SheetKey.BootstrapData, _sheetLoader.LoadedKeys[0]);
        }

        // ─── Helpers ───────────────────────────────────────────────────────────

        private static FakeBootstrapHandle BuildBootstrapHandle()
        {
            // [HEAP] BlobBuilder allocates during test setup — acceptable
            var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<BootstrapDataRoot>();
            var rows = builder.Allocate(ref root.Rows, 7);

            builder.AllocateString(ref rows[0].Key, "GraphicsSettings");
            builder.AllocateString(ref rows[0].DataKey, SheetKey.GraphicsSettings);
            rows[0].Id = 1;

            builder.AllocateString(ref rows[1].Key, "AudioSettings");
            builder.AllocateString(ref rows[1].DataKey, SheetKey.AudioSettings);
            rows[1].Id = 2;

            builder.AllocateString(ref rows[2].Key, "ErrorCode");
            builder.AllocateString(ref rows[2].DataKey, SheetKey.ErrorCode);
            rows[2].Id = 3;

            builder.AllocateString(ref rows[3].Key, "SceneInfo");
            builder.AllocateString(ref rows[3].DataKey, SheetKey.SceneInfo);
            rows[3].Id = 4;

            builder.AllocateString(ref rows[4].Key, "LocaleSettings");
            builder.AllocateString(ref rows[4].DataKey, SheetKey.LocaleSettings);
            rows[4].Id = 5;

            builder.AllocateString(ref rows[5].Key, "BootstrapLocale");
            builder.AllocateString(ref rows[5].DataKey, "BootstrapLocale_{0}");
            rows[5].Id = 6;

            builder.AllocateString(ref rows[6].Key, "ErrorMsgLocale");
            builder.AllocateString(ref rows[6].DataKey, "ErrorMsgLocale_{0}");
            rows[6].Id = 7;

            var blobRef = builder.CreateBlobAssetReference<BootstrapDataRoot>(Allocator.Temp);
            builder.Dispose();
            return new FakeBootstrapHandle(blobRef);
        }

        // ─── Stubs & fixtures ──────────────────────────────────────────────────

        private sealed class StubSheetLoader : IDataSheetLoader
        {
            // [HEAP] list allocated once per test — acceptable
            public readonly List<string> LoadedKeys = new();

            public UniTask LoadSheetAsync<T>(string assetName) where T : unmanaged
            {
                LoadedKeys.Add(assetName);
                return UniTask.CompletedTask;
            }
        }

        private sealed class StubDataProvider : IDataProvider
        {
            public FakeBootstrapHandle BootstrapHandle;

            public IDataHandle<T> GetData<T>() where T : unmanaged
            {
                if (typeof(T) == typeof(BootstrapDataRoot))
                    return (IDataHandle<T>)(object)BootstrapHandle;
                return null;
            }

            public System.Collections.Generic.IReadOnlyList<IDataHandle<T>> GetAllData<T>() where T : unmanaged
                => System.Array.Empty<IDataHandle<T>>();
        }

        private sealed class FakeBootstrapHandle : IBlobDataHandle<BootstrapDataRoot>
        {
            private BlobAssetReference<BootstrapDataRoot> _ref;

            public FakeBootstrapHandle(BlobAssetReference<BootstrapDataRoot> blobRef) { _ref = blobRef; }

            public bool IsCreated => _ref.IsCreated;

            public bool TryGetData(out BootstrapDataRoot data)
            {
                data = _ref.Value;
                return true;
            }

            public ref BootstrapDataRoot GetRef() => ref _ref.Value;
            public BlobAssetReference<BootstrapDataRoot> GetRawReference() => _ref;

            public void Dispose() { if (_ref.IsCreated) _ref.Dispose(); }
        }
    }
}
