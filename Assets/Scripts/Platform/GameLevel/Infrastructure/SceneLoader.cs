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
         * DDD �������� ���� �� ��ȯ ������ �ݿ��Ϸ��� **������ �߽� ����** ��Ģ�� ������, ����Ƽ�� ����� �䱸����(�� ��ȯ, ���ҽ� �ε�, UI ������Ʈ ��)�� �ݿ��� ������ �ʿ��մϴ�. �Ʒ��� �����ϴ� ������ \*\*"�������̽� �߽��� Application ���̾� + å�� �и��� ������/������ ���� + �޽��� ��� �帧"\*\*�� ������� �����˴ϴ�.
         * ---
         * ## ? ����: �������ͽ� ��� �� �ֿ� ����
         * | ����                                 | ����                              |
         * | ---------------------------------- | ------------------------------- |
| **SceneChangeRequested**           | �޽���: �� ���� ��û�� ��Ÿ��               |
| **SceneLoadService**               | ������: ���� �ε��ϴ� ������ ����            |
| **ResourceLoadService**            | ������: ���ҽ��� �ε��ϴ� ������ ����          |
| **ProgressReporter**               | ������ or ������: ������� ���� �۾� ���¸� �ۺ��� |
| **SceneLoaderInfra**               | ������: ���� ����Ƽ�� �� ��ȯ ó��            |
| **ResourceLoaderInfra**            | ������: �ּ� ����, �ּҰ� �� ���� ���ҽ� �ε� ó��  |
| **ProgressPublisherInfra (Rx ���)** | ������: UI�� ����� ���α׷��� �ۺ���         |

---

## ?? ���̾� ���� (DDD + ����Ƽ Ưȭ)

```
Application
��
������ SceneChangeApplication �� �޽��� ���� ����
��   ������ SceneLoadService (������ ����)
��   ������ ResourceLoadService (������ ����)
��
������ ProgressService
��   ������ ProgressReporter (Rx�� UI�� �˸�)
��
������ �޽��� �ý��� (Flux �Ǵ� MessageBus)
Domain
��
������ Interfaces
��   ������ ISceneLoader
��   ������ IResourceLoader
��   ������ IProgressReporter
Infrastructure
��
������ UnitySceneLoader : ISceneLoader
������ AddressableResourceLoader : IResourceLoader
������ RxProgressReporter : IProgressReporter
```

---

## ?? ó�� �÷ο� (�ó����� ����)

### 0. �޽����� �� ���� ��û

```csharp
_messageBus.Publish(new SceneChangeRequested("NextScene"));
```

### 1. SceneChangeApplication�� �޽��� ����

```csharp
public class SceneChangeApplication : ISceneChangeApplication, IMessageListener<SceneChangeRequested>
{
    public async void OnMessage(SceneChangeRequested message)
    {
        _progressReporter.Report(0f, "�ε� �� �ε� ��...");
        await _sceneLoader.LoadSceneAsync("Loading");

        _progressReporter.Report(0.1f, "���� �� ��ε� ��...");
        await _sceneLoader.UnloadSceneAsync(_currentScene);

        _progressReporter.Report(0.2f, "�� �� �ε� ��...");
        await _sceneLoader.LoadSceneAsync(message.NextScene);

        _progressReporter.Report(0.4f, "���ҽ� �ε� ��...");
        await _resourceLoader.LoadAllResourcesAsync(message.NextScene, progress =>
        {
            _progressReporter.Report(progress.Percent, progress.Description);
        });

        _progressReporter.Report(1f, "�ε� �� ���� ��...");
        await _sceneLoader.UnloadSceneAsync("Loading");
    }
}
```

---

## ?? �� ���� �и� Ŭ����

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

## ?? ������ ����

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
        // ����: Addressables �ε�
        var resourceList = GetResourcesForScene(sceneName);
        int total = resourceList.Count;
        for (int i = 0; i < total; i++)
        {
            onProgress?.Invoke(new LoadProgress
            {
                Percent = (float)i / total,
                Description = $"���ҽ� {i + 1}/{total} �ε� ��..."
            });

            await Addressables.LoadAssetAsync<object>(resourceList[i]);
        }
    }
}
```

---

## ??? UI ����: RxProgressReporter

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

## ?? ������ ���

| �и� �׸�       | ��� Ŭ����                                            |
| ----------- | ------------------------------------------------- |
| �޽��� ��� ����   | `SceneChangeRequested` �� `SceneChangeApplication` |
| �� �ε�        | `ISceneLoader`, `UnitySceneLoader`                |
| ���ҽ� �ε�      | `IResourceLoader`, `AddressableResourceLoader`    |
| ���α׷��� UI ���� | `IProgressReporter`, `RxProgressReporter`         |

�� ������ ������� �ϸ�, �� ��ȯ ������ **�������̽� ������ Application�� ĸ��ȭ**�ϰ�, �� �ε�/���ҽ� �ε�/���α׷��� ������ **���� �������̽��� �и��Ͽ� �׽�Ʈ�� ���������� ����**�����ϴ�.

---

���Ͻø� �� ������ ������� ��ü �ڵ� ��ĳ����(���� ���� + Ŭ���� ����)�� �����ص帱 �� �־��. ���Ͻó���?

         * �� ������ ���� ��û�ϰ� 
         * ���� �ε������� ������ ��� 
         * �ε� �ۼ������� �� ������ ���� Ư�� ���ø����̼ǿ� �����ϰ� 
         * ǥ���ϴ� ������ �ۼ�������� ���� �ɸ� �͵� ���� �ۼ���������� �� ��  / ����� �ۼ��������� ��
         * ǥ�� ����� ��� ó���ؾ��ұ�?
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
                //Report(asyncOperation.progress); // ���� ���� (0~1)
                if (asyncOperation.progress >= 0.9f)
                    asyncOperation.allowSceneActivation = true; // ���� �� �ε� �Ϸ� �� �� Ȱ��ȭ

                await UniTask.Yield(); // �����Ӹ��� ���� ���� ������Ʈ
            }
        }

        public void RequestGameLevelChange(string gameLevelKey)
        {
            /*
             * ��, **SceneLoader (������ ����)**�� GameLevelKey�� �޾Ƽ�

������(���̺�, DB, ScriptableObject ��)���� �� ���� ��ȸ

���� Builtin���� Addressable���� �Ǵ�

�ش� Ÿ�Կ� �´� �ε� �Լ�(SceneManager.LoadSceneAsync �Ǵ� Addressables.LoadSceneAsync) ȣ��

�� �ϴ� ���� ���Ұ� å�� �и��� �ſ� �����ϰ� ����Ǵ� �����Դϴ�.
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

            // ���⼭ ���ҽ� �ε� �߰� 

            /*
             * ���ҽ� �δ� �������̽� ���� ó��
             * ���� ������ ������ ISceneResourceLoader�� IRuntimeLoader(�Ǵ� IGlobalResourceLoader ��)�� �и����ڴ� ���Դϴ�.
             * �̴� å�� �и�(SRP) �� ���� �� ���յ� �ּ�ȭ�� ���� �����Դϴ�.
             * 
             * 
             * 
�� ����	�ε� API	���
Builtin ��	SceneManager.LoadSceneAsync	Build Settings�� ��� �ʿ�
Addressable ��	Addressables.LoadSceneAsync	���� ��Ű¡, Addressable ����
AssetBundle ��	AssetBundle + �� Ȱ��ȭ	Ŀ���� �ε� ���μ��� �ʿ�
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
        //// �̸� ����ؾ��� 
        //// �����ͷ� ������ 
        // �̰͵� �������̽��� �ٲٰ� 
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