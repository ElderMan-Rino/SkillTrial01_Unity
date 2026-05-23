using Cysharp.Threading.Tasks;
using System;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Elder.Framework.Scene.Interfaces
{
    public interface ISceneLoader : IDisposable
    {
        public UniTask<SceneInstance> LoadSceneAsync(string key, LoadSceneMode loadMode = LoadSceneMode.Additive, bool activateOnLoad = true);
        public UniTask UnloadSceneAsync(SceneInstance sceneInstance);
    }
}