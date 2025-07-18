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
         * �� ������ ���� ��û�ϰ� 
         * ���� �ε������� ������ ��� 
         * �ε� �ۼ������� �� ������ ���� Ư�� ���ø����̼ǿ� �����ϰ� 
         * ǥ���ϴ� ������ �ۼ�������� ���� �ɸ� �͵� ���� �ۼ���������� �� ��  / ����� �ۼ��������� ��
         * ǥ�� ����� ��� ó���ؾ��ұ�?
         * 
         */

        //private async UniTask TryChangeScene(string targetScene)
        //{
        //    var currentScene = SceneManager.GetActiveScene();
        //    await LoadSceneAsync(LOADING_SCENE, LoadSceneMode.Additive);
        //    await UnloadSceneAsync(currentScene);

        //    // �÷��� ������Ʈ �ʿ� -> Component��
        //    // �߰��� ���ҽ� �ý��� �����ͼ� �����ϴ� �� �߰� �ʿ� 
        //    // ���⼭ �÷����� �����ָ� ��

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
                //Report(asyncOperation.progress); // ���� ���� (0~1)
                if (asyncOperation.progress >= 0.9f)
                    asyncOperation.allowSceneActivation = true; // ���� �� �ε� �Ϸ� �� �� Ȱ��ȭ

                await UniTask.Yield(); // �����Ӹ��� ���� ���� ������Ʈ
            }
        }

        public void RequestGameStepChange()
        {

        }


        //// �̸� ����ؾ��� 
        //// �����ͷ� ������ 

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