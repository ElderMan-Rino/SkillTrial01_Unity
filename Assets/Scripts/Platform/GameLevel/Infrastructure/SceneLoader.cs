using Cysharp.Threading.Tasks;
using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Enums;
using Elder.Core.FluxMessage.Interfaces;
using Elder.Core.GameLevel.Constants;
using Elder.Core.GameLevel.Interfaces;
using Elder.Core.GameLevel.Messages;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Elder.Platform.GameLevel.Infrastructure
{
    public class SceneLoader : InfrastructureBase, IGameLevelExecutor
    {
        /*
         * DDD 구조에서 위의 씬 전환 로직을 반영하려면 **도메인 중심 설계** 원칙을 따르되, 유니티의 기술적 요구사항(씬 전환, 리소스 로드, UI 업데이트 등)을 반영한 구조가 필요합니다. 아래에 제안하는 구조는 \*\*"유스케이스 중심의 Application 레이어 + 책임 분리된 도메인/인프라 구현 + 메시지 기반 흐름"\*\*을 기반으로 구성됩니다.
         * ---
         * ## ? 전제: 유비쿼터스 언어 및 주요 컨셉
         * | 개념                                 | 설명                              |
         * | ---------------------------------- | ------------------------------- |
| **SceneChangeRequested**           | 메시지: 씬 변경 요청을 나타냄               |
| **SceneLoadService**               | 도메인: 씬을 로드하는 도메인 서비스            |
| **ResourceLoadService**            | 도메인: 리소스를 로드하는 도메인 서비스          |
| **ProgressReporter**               | 도메인 or 인프라: 진행률과 현재 작업 상태를 퍼블리시 |
| **SceneLoaderInfra**               | 인프라: 실제 유니티의 씬 전환 처리            |
| **ResourceLoaderInfra**            | 인프라: 애셋 번들, 주소값 등 실제 리소스 로드 처리  |
| **ProgressPublisherInfra (Rx 기반)** | 인프라: UI와 연결된 프로그레스 퍼블리셔         |

---

## ?? 레이어 구조 (DDD + 유니티 특화)

```
Application
│
├── SceneChangeApplication ← 메시지 수신 지점
│   ├── SceneLoadService (도메인 서비스)
│   └── ResourceLoadService (도메인 서비스)
│
├── ProgressService
│   └── ProgressReporter (Rx로 UI에 알림)
│
└── 메시지 시스템 (Flux 또는 MessageBus)
Domain
│
├── Interfaces
│   ├── ISceneLoader
│   ├── IResourceLoader
│   └── IProgressReporter
Infrastructure
│
├── UnitySceneLoader : ISceneLoader
├── AddressableResourceLoader : IResourceLoader
└── RxProgressReporter : IProgressReporter
```

---

## ?? 처리 플로우 (시나리오 기준)

### 0. 메시지로 씬 변경 요청

```csharp
_messageBus.Publish(new SceneChangeRequested("NextScene"));
```

### 1. SceneChangeApplication이 메시지 수신

```csharp
public class SceneChangeApplication : ISceneChangeApplication, IMessageListener<SceneChangeRequested>
{
    public async void OnMessage(SceneChangeRequested message)
    {
        _progressReporter.Report(0f, "로딩 씬 로드 중...");
        await _sceneLoader.LoadSceneAsync("Loading");

        _progressReporter.Report(0.1f, "기존 씬 언로드 중...");
        await _sceneLoader.UnloadSceneAsync(_currentScene);

        _progressReporter.Report(0.2f, "새 씬 로드 중...");
        await _sceneLoader.LoadSceneAsync(message.NextScene);

        _progressReporter.Report(0.4f, "리소스 로드 중...");
        await _resourceLoader.LoadAllResourcesAsync(message.NextScene, progress =>
        {
            _progressReporter.Report(progress.Percent, progress.Description);
        });

        _progressReporter.Report(1f, "로딩 씬 정리 중...");
        await _sceneLoader.UnloadSceneAsync("Loading");
    }
}
```

---

## ?? 각 역할 분리 클래스

### ? 1. ISceneLoader

```csharp
public interface ISceneLoader
{
    UniTask LoadSceneAsync(string sceneName);
    UniTask UnloadSceneAsync(string sceneName);
}
```

### ? 2. IResourceLoader

```csharp
public interface IResourceLoader
{
    UniTask LoadAllResourcesAsync(string sceneName, Action<LoadProgress> onProgress);
}

public struct LoadProgress
{
    public float Percent;
    public string Description;
}
```

### ? 3. IProgressReporter

```csharp
public interface IProgressReporter
{
    void Report(float percent, string description);
}
```

---

## ?? 인프라 예시

```csharp
public class UnitySceneLoader : ISceneLoader
{
    public async UniTask LoadSceneAsync(string sceneName)
    {
        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public async UniTask UnloadSceneAsync(string sceneName)
    {
        await SceneManager.UnloadSceneAsync(sceneName);
    }
}

public class AddressableResourceLoader : IResourceLoader
{
    public async UniTask LoadAllResourcesAsync(string sceneName, Action<LoadProgress> onProgress)
    {
        // 예시: Addressables 로드
        var resourceList = GetResourcesForScene(sceneName);
        int total = resourceList.Count;
        for (int i = 0; i < total; i++)
        {
            onProgress?.Invoke(new LoadProgress
            {
                Percent = (float)i / total,
                Description = $"리소스 {i + 1}/{total} 로드 중..."
            });

            await Addressables.LoadAssetAsync<object>(resourceList[i]);
        }
    }
}
```

---

## ??? UI 연결: RxProgressReporter

```csharp
public class RxProgressReporter : IProgressReporter
{
    private Subject<(float, string)> _progressSubject = new Subject<(float, string)>();

    public IObservable<(float, string)> ProgressStream => _progressSubject;

    public void Report(float percent, string description)
    {
        _progressSubject.OnNext((percent, description));
    }
}
```

---

## ?? 마무리 요약

| 분리 항목       | 담당 클래스                                            |
| ----------- | ------------------------------------------------- |
| 메시지 기반 시작   | `SceneChangeRequested` → `SceneChangeApplication` |
| 씬 로딩        | `ISceneLoader`, `UnitySceneLoader`                |
| 리소스 로딩      | `IResourceLoader`, `AddressableResourceLoader`    |
| 프로그레스 UI 전달 | `IProgressReporter`, `RxProgressReporter`         |

이 구조를 기반으로 하면, 씬 전환 로직을 **유스케이스 단위로 Application에 캡슐화**하고, 씬 로딩/리소스 로딩/프로그레스 로직을 **개별 인터페이스로 분리하여 테스트와 유지보수가 용이**해집니다.

---

원하시면 이 구조를 기반으로 전체 코드 스캐폴드(폴더 구조 + 클래스 뼈대)도 제공해드릴 수 있어요. 원하시나요?

         * 씬 변경을 따로 요청하고 
         * 씬을 로딩씬으로 변경할 경우 
         * 로딩 퍼센테이지 및 내용을 따로 특정 어플리케이션에 전달하고 
         * 표기하는 곳에서 퍼센테이즈는 현재 걸린 것들 기준 퍼센테이즈들의 총 합  / 계산할 퍼센테이지의 수
         * 표기 방식은 어떻게 처리해야할까?
         * 
         */

        
        private async UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadType)
        {
            await SceneManager.LoadSceneAsync(sceneName, loadType);
        }
        private async UniTask UnloadSceneAsync(string sceneName)
        {
            await SceneManager.UnloadSceneAsync(sceneName);
        }
        private async UniTask UnloadSceneAsync(Scene targetScene)
        {
            await SceneManager.UnloadSceneAsync(targetScene);
        }
        private async UniTask LoadSceneWithProgressAsync(string targetScene)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = false;

            while (!asyncOperation.isDone)
            {
                //Report(asyncOperation.progress); // 진행 상태 (0~1)
                if (asyncOperation.progress >= 0.9f)
                    asyncOperation.allowSceneActivation = true; // 최종 씬 로딩 완료 후 씬 활성화

                await UniTask.Yield(); // 프레임마다 진행 상태 업데이트
            }
        }

        public void RequestGameLevelChange(string gameLevelKey)
        {
            /*
             * 네, **SceneLoader (인프라 계층)**가 GameLevelKey를 받아서

데이터(테이블, DB, ScriptableObject 등)에서 씬 정보 조회

씬이 Builtin인지 Addressable인지 판단

해당 타입에 맞는 로드 함수(SceneManager.LoadSceneAsync 또는 Addressables.LoadSceneAsync) 호출

을 하는 것은 역할과 책임 분리에 매우 적절하고 권장되는 설계입니다.
             */
            ChangeSceneAsync(gameLevelKey).Forget();
        }
        private async UniTask ChangeSceneAsync(string targetScene)
        {
            PublishCurrentGameLevelState(GameLevelLoadState.LoadLoading);
            await LoadSceneAsync(GameLevelConstants.LOADING_SCENE_KEY, LoadSceneMode.Additive);

            var currentScene = SceneManager.GetActiveScene();
            await UnloadSceneAsync(currentScene);
            await LoadSceneWithProgressAsync(targetScene);

            // 여기서 리소스 로드 추가 

            /*
             * 리소스 로더 인터페이스 관련 처리
             * 제가 제안한 요지는 ISceneResourceLoader와 IRuntimeLoader(또는 IGlobalResourceLoader 등)를 분리하자는 것입니다.
             * 이는 책임 분리(SRP) 와 계층 간 결합도 최소화를 위한 설계입니다.
             * 
             * 
             * 
씬 종류	로드 API	비고
Builtin 씬	SceneManager.LoadSceneAsync	Build Settings에 등록 필요
Addressable 씬	Addressables.LoadSceneAsync	별도 패키징, Addressable 관리
AssetBundle 씬	AssetBundle + 씬 활성화	커스텀 로드 프로세스 필요
            */

            PublishCurrentGameLevelState(GameLevelLoadState.UnloadLoading);
            await UnloadSceneAsync(GameLevelConstants.LOADING_SCENE_KEY);

            PublishCurrentGameLevelState(GameLevelLoadState.ChangeComplete);
        }
        private void PublishCurrentGameLevelState(GameLevelLoadState state)
        {
            if (!TryGetApplication<IFluxRouter>(out var fluxRouter))
                return;
            fluxRouter.Publish<FxLoadGameLevelState>(new FxLoadGameLevelState(state));
        }
        //// 이름 고민해야함 
        //// 데이터로 빼야함 
        // 이것도 인터페이스로 바꾸고 
        //private ReactiveCommand<float> _reportProgress;
        //private ReactiveCommand<Unit> _changeComplete;
        //public IObservable<float> OnChangeProgressed => _reportProgress;
        //public IObservable<Unit> OnChangeCompleted => _changeComplete;

        //public override bool Initialize()
        //{
        //    _reportProgress = new();
        //    _changeComplete = new();
        //    return true;
        //}
        //public void Report(float value)
        //{
        //    _reportProgress.Execute(value);
        //}

        //protected override void DisposeManagedResources()
        //{
        //    _reportProgress.Dispose();
        //    _reportProgress = null;

        //    _changeComplete.Dispose();
        //    _changeComplete = null;
        //}

        //protected override void ReleaseUnmanagedResources()
        //{

        //}
    }
}