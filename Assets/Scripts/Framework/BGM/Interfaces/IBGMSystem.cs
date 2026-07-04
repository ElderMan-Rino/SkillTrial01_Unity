using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.BGM.Interfaces
{
    // [설계]
    // BGMSystem의 책임: 시그널 수신 + BGMInfoRoot row lookup + 재생 상태 관리(재생/정지/루프/크로스페이드).
    // 에셋 취득 자체는 책임지지 않음 -> IBGMAssetLoader(Infra)에 위임.
    // SceneChanger가 ISceneLoader에 위임하는 것과 동일한 구조 (BGMSystem은 IAssetProvider를 직접 참조하지 않음).
    public interface IBGMSystem : ISystemComponent
    {
        public UniTask PlayAsync(string bgmKey);
        public void Stop();
    }
}
