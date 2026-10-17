using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using System;

namespace Elder.Framework.Scene.Interfaces
{
    // 씬 전환 유스케이스의 오케스트레이션 창구. 실제 전환은 ISceneLoader, 컴포넌트 생명주기는 IComponentRegistry에 위임.
    public interface ISceneService
    {
        public UniTask EnterSceneAsync(string sceneName, int scope, Action<IComponentRegistry> installAction, Type startCoordinatorType);
    }
}
