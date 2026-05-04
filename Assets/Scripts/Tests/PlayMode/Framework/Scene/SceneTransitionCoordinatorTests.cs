using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Elder.Framework.Blob.Interfaces;
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Flux.Infra;
using Elder.Framework.Flux.Helpers;
using Elder.Framework.Scene.App;
using Elder.Framework.Scene.Domain.Data;
using Elder.Framework.Scene.Interfaces;
using Elder.Framework.Scene.Messages;
using Elder.SkillTrial.Scene.Domain;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Elder.Framework.Tests.Scene
{
    internal sealed class SceneTransitionCoordinatorTests
    {
        private FluxRouter _router;
        private FakeSceneLoader _fakeLoader;
        private FakeSceneContextFactory _fakeContextFactory;
        private FakeDataProvider _fakeDataProvider;
        private SceneTransitionCoordinator _coordinator;

        // ─── Setup / Teardown ─────────────────────────────────────────────────

        [SetUp]
        public void SetUp()
        {
            _router = new FluxRouter();
            _fakeLoader = new FakeSceneLoader();
            _fakeContextFactory = new FakeSceneContextFactory();
            _fakeDataProvider = new FakeDataProvider();

            _coordinator = new SceneTransitionCoordinator(
                _fakeLoader,
                _fakeContextFactory,
                _router,
                _fakeDataProvider);

            _coordinator.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            _coordinator.Dispose();
            _router.Dispose();
            _fakeDataProvider.Dispose();
        }

        // ─── Tests ────────────────────────────────────────────────────────────

        [UnityTest]
        public IEnumerator Transition_PublishesStartedAndCompleted_WhenSceneKeyExists()
            => UniTask.ToCoroutine(async () =>
            {
                _fakeDataProvider.SetupScene("GameScene", "addressable/GameScene", SceneLoadMode.Single);

                var startedKeys = new List<string>();
                var completedKeys = new List<string>();

                _router.Subscribe<FxSceneTransitionStarted>((in FxSceneTransitionStarted m) => startedKeys.Add(m.TargetSceneKey));
                _router.Subscribe<FxSceneTransitionCompleted>((in FxSceneTransitionCompleted m) => completedKeys.Add(m.LoadedSceneKey));

                _router.Publish(new FxSceneTransition("GameScene"));

                await UniTask.DelayFrame(3);

                Assert.AreEqual(1, startedKeys.Count);
                Assert.AreEqual("GameScene", startedKeys[0]);
                Assert.AreEqual(1, completedKeys.Count);
                Assert.AreEqual("GameScene", completedKeys[0]);
            });

        [UnityTest]
        public IEnumerator Transition_DoesNotPublishStarted_WhenSceneKeyMissing()
            => UniTask.ToCoroutine(async () =>
            {
                _fakeDataProvider.SetupScene("GameScene", "addressable/GameScene", SceneLoadMode.Single);

                var startedCount = 0;
                _router.Subscribe<FxSceneTransitionStarted>((in FxSceneTransitionStarted _) => startedCount++);

                _router.Publish(new FxSceneTransition("UnknownScene"));

                await UniTask.DelayFrame(3);

                Assert.AreEqual(0, startedCount);
            });

        [UnityTest]
        public IEnumerator Transition_InProgress_IgnoresDuplicateRequest()
            => UniTask.ToCoroutine(async () =>
            {
                _fakeDataProvider.SetupScene("GameScene", "addressable/GameScene", SceneLoadMode.Single);
                _fakeLoader.LoadDelay = 5;

                var startedCount = 0;
                _router.Subscribe<FxSceneTransitionStarted>((in FxSceneTransitionStarted _) => startedCount++);

                _router.Publish(new FxSceneTransition("GameScene"));
                _router.Publish(new FxSceneTransition("GameScene")); // duplicate while InProgress

                await UniTask.DelayFrame(12);

                Assert.AreEqual(1, startedCount);
            });

        [UnityTest]
        public IEnumerator Transition_Single_LoadsTempThenNewScene()
            => UniTask.ToCoroutine(async () =>
            {
                // Single path: SwapWithTempAsync loads TempScene + target = 2 loads, 0 unloads (no prior scene)
                _fakeDataProvider.SetupScene("GameScene", "addressable/GameScene", SceneLoadMode.Single);

                _router.Publish(new FxSceneTransition("GameScene"));
                await UniTask.DelayFrame(3);

                Assert.AreEqual(2, _fakeLoader.LoadCallCount);   // TempScene + GameScene
                Assert.AreEqual(1, _fakeLoader.UnloadCallCount);  // TempScene unloaded after swap
            });

        [UnityTest]
        public IEnumerator Transition_Additive_LoadsWithoutTempScene()
            => UniTask.ToCoroutine(async () =>
            {
                // AdditiveKeepPrevious path: LoadAdditiveAsync — no TempScene
                _fakeDataProvider.SetupScene("HudScene", "addressable/HudScene", SceneLoadMode.AdditiveKeepPrevious);

                _router.Publish(new FxSceneTransition("HudScene"));
                await UniTask.DelayFrame(3);

                Assert.AreEqual(1, _fakeLoader.LoadCallCount);
                Assert.AreEqual(0, _fakeLoader.UnloadCallCount);
            });

        [UnityTest]
        public IEnumerator Transition_Single_SecondTransition_UnloadsPreviousScene()
            => UniTask.ToCoroutine(async () =>
            {
                _fakeDataProvider.SetupScene("SceneA", "addressable/SceneA", SceneLoadMode.Single);
                _fakeDataProvider.SetupScene("SceneB", "addressable/SceneB", SceneLoadMode.Single);

                _router.Publish(new FxSceneTransition("SceneA"));
                await UniTask.DelayFrame(3);

                var unloadAfterFirst = _fakeLoader.UnloadCallCount; // TempScene only

                _router.Publish(new FxSceneTransition("SceneB"));
                await UniTask.DelayFrame(3);

                // Second swap: TempScene unload + SceneA unload = 2 additional unloads
                Assert.AreEqual(unloadAfterFirst + 2, _fakeLoader.UnloadCallCount);
            });

        [UnityTest]
        public IEnumerator Dispose_UnsubscribesFromRouter_NoCallbackAfterDispose()
            => UniTask.ToCoroutine(async () =>
            {
                _fakeDataProvider.SetupScene("GameScene", "addressable/GameScene", SceneLoadMode.Single);

                var completedCount = 0;
                _router.Subscribe<FxSceneTransitionCompleted>((in FxSceneTransitionCompleted _) => completedCount++);

                _coordinator.Dispose();

                _router.Publish(new FxSceneTransition("GameScene"));
                await UniTask.DelayFrame(3);

                Assert.AreEqual(0, completedCount);
            });

        // ─── Fakes ────────────────────────────────────────────────────────────

        private sealed class FakeSceneLoader : ISceneLoader
        {
            public int LoadCallCount { get; private set; }
            public int UnloadCallCount { get; private set; }
            public int LoadDelay { get; set; } = 0;

            public async UniTask<SceneInstance> LoadSceneAsync(string key, LoadSceneMode loadMode = LoadSceneMode.Additive, bool activateOnLoad = true)
            {
                if (LoadDelay > 0) await UniTask.DelayFrame(LoadDelay);
                LoadCallCount++;
                return default;
            }

            public UniTask UnloadSceneAsync(SceneInstance sceneInstance)
            {
                UnloadCallCount++;
                return UniTask.CompletedTask;
            }
        }

        private sealed class FakeSceneContextFactory : ISceneContextFactory
        {
            public SceneLoadContext Create(string mainSceneName)
            {
                var ctx = new SceneLoadContext();
                ctx.Initialize(mainSceneName);
                return ctx;
            }

            public void Release(SceneLoadContext context) => context.Dispose();
        }

        private sealed class FakeDataProvider : IDataProvider, System.IDisposable
        {
            // 여러 씬 키를 지원하기 위해 마지막 SetupScene 호출값만 유지
            private BlobAssetReference<SceneInfoRoot> _blobRef;
            private bool _hasData;

            // 여러 씬을 동일 Blob에 추가하려면 rows를 누적해야 하나,
            // 현재 테스트는 씬당 독립 테스트이므로 단일 Row Blob으로 충분
            public void SetupScene(string key, string addressableKey, SceneLoadMode loadMode)
            {
                if (_hasData)
                {
                    _blobRef.Dispose();
                    _hasData = false;
                }

                // [HEAP] BlobBuilder는 내부에서 NativeArray 확보 — 테스트 초기화 시 1회만 실행
                var builder = new BlobBuilder(Allocator.Temp);
                ref var root = ref builder.ConstructRoot<SceneInfoRoot>();
                var rowArray = builder.Allocate(ref root.Rows, 1);

                rowArray[0].Id = 1;
                rowArray[0].LoadMode = loadMode;
                builder.AllocateString(ref rowArray[0].Key, key);
                builder.AllocateString(ref rowArray[0].AddressableKey, addressableKey);

                _blobRef = builder.CreateBlobAssetReference<SceneInfoRoot>(Allocator.Persistent);
                builder.Dispose();
                _hasData = true;
            }

            public IDataHandle<T> GetData<T>() where T : unmanaged
            {
                if (typeof(T) == typeof(SceneInfoRoot) && _hasData)
                {
                    // [BOXING] object cast — テスト経路のみ、ホットパス外
                    return (IDataHandle<T>)(object)new FakeBlobDataHandle<SceneInfoRoot>(_blobRef);
                }
                return null;
            }

            public IReadOnlyList<IDataHandle<T>> GetAllData<T>() where T : unmanaged => null;

            public void Dispose()
            {
                if (_hasData) _blobRef.Dispose();
                _hasData = false;
            }
        }

        private sealed class FakeBlobDataHandle<T> : IBlobDataHandle<T> where T : unmanaged
        {
            private readonly BlobAssetReference<T> _ref;

            public FakeBlobDataHandle(BlobAssetReference<T> blobRef) => _ref = blobRef;

            public BlobAssetReference<T> GetRawReference() => _ref;

            public bool TryGetData(out T data)
            {
                data = _ref.Value;
                return true;
            }

            public void Dispose() { }
        }
    }
}
