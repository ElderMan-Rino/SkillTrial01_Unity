using Cysharp.Threading.Tasks;
using Elder.Core.Common.BaseClasses;
using Elder.Core.GameStep.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Elder.Platform.GameStep.Infrastructure
{
    public class SceneLoader : InfrastructureBase, IGameStepExecutor
    {
        private const string LOADING_SCENE = "LoadingScene";

        /*
         * 씬 변경을 따로 요청하고 
         * 씬을 로딩씬으로 변경할 경우 
         * 로딩 퍼센테이지 및 내용을 따로 특정 어플리케이션에 전달하고 
         * 표기하는 곳에서 퍼센테이즈는 현재 걸린 것들 기준 퍼센테이즈들의 총 합  / 계산할 퍼센테이지의 수
         * 표기 방식은 어떻게 처리해야할까?
         * 
         */

        //private async UniTask TryChangeScene(string targetScene)
        //{
        //    var currentScene = SceneManager.GetActiveScene();
        //    await LoadSceneAsync(LOADING_SCENE, LoadSceneMode.Additive);
        //    await UnloadSceneAsync(currentScene);

        //    // 플럭스 컴포넌트 필요 -> Component를
        //    // 중간에 리소스 시스템 가져와서 정리하는 것 추가 필요 
        //    // 여기서 플럭스를 날려주면 됨

        //    await LoadSceneWithProgressAsync(targetScene);
        //    await UnloadSceneAsync(LOADING_SCENE);
        //}
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

        public void RequestGameStepChange()
        {

        }


        //// 이름 고민해야함 
        //// 데이터로 빼야함 

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