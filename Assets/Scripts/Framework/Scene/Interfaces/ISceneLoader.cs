using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Elder.Framework.Scene.Interfaces
{
    public interface ISceneLoader 
    {
        public UniTask<SceneInstance> LoadSceneAsync(string key, LoadSceneMode loadMode = LoadSceneMode.Additive, bool activateOnLoad = true);
        public UniTask UnloadSceneAsync(SceneInstance sceneInstance);
    }
}