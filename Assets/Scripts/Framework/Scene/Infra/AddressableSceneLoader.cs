using Cysharp.Threading.Tasks;
using Elder.Framework.Common.Base;
using Elder.Framework.Scene.Interfaces;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Elder.Framework.Scene.Infra
{
    // ✅ OK: internal sealed — 구현 클래스 접근 수정자 준수
    // ✅ OK: 폴더 위치 — Infra/ (Addressables Unity API 어댑터)
    // ❌ VIOLATION: BaseSystem/BaseSystemComponent 미상속 — IGameSystem 라이프사이클(InjectDependency/Initialize 등) 없음
    //   제안: BaseSystem 상속 추가 또는 IGameSystemRegistry에 등록하지 않는 순수 팩토리로만 사용 시 명시적 주석 필요
    internal sealed class AddressableSceneLoader : ISceneLoader
    {
        public async UniTask<SceneInstance> LoadSceneAsync(string key, LoadSceneMode loadMode = LoadSceneMode.Additive, bool activateOnLoad = true)
        {
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(key, loadMode, activateOnLoad);
            SceneInstance sceneInstance = await handle.ToUniTask();
            return sceneInstance;
        }

        public async UniTask UnloadSceneAsync(SceneInstance sceneInstance)
        {
            var handle = Addressables.UnloadSceneAsync(sceneInstance);
            await handle.ToUniTask();
        }
    }
}