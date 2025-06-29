using Cysharp.Threading.Tasks;
using Elder.Framework.Common.Base;
using Elder.Framework.Scene.Interfaces;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Elder.Framework.Scene.Infra
{
    public class AddressableSceneLoader : DisposableBase, ISceneLoader
    {
        public async UniTask<SceneInstance> LoadSceneAsync(string key, LoadSceneMode loadMode = LoadSceneMode.Additive, bool activateOnLoad = true)
        {
            var handle = Addressables.LoadSceneAsync(key, loadMode, activateOnLoad);
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