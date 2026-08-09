using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.Core.Interfaces;
using UnityEngine;

namespace Elder.Framework.Bgm.Interfaces
{
    // [설계]
    // ISceneLoader/AddressableSceneLoader와 동일한 역할의 Infra 어댑터.
    // IAssetProvider.GetAssetAsync<AudioClip>(key) 호출을 캡슐화하여
    // BGMSystem이 IAssetProvider를 직접 의존하지 않도록 분리 (SRP/OCP).
    public interface IBGMAssetLoader : ISystemComponent
    {
        public UniTask<IAssetHandle<AudioClip>> LoadClipAsync(string addressableKey);
    }
}
