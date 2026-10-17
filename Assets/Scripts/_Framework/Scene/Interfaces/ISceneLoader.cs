using Cysharp.Threading.Tasks;

namespace Elder.Framework.Scene.Interfaces
{
    // Unity SceneManager 직접 호출을 은닉하는 최하위 계층. SceneService는 이 인터페이스만 알고 있음(DIP).
    public interface ISceneLoader
    {
        public UniTask LoadSceneAsync(string sceneName);
    }
}
