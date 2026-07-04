using Cysharp.Threading.Tasks;
using Elder.Framework.Asset.Interfaces;
using Elder.Framework.BGM.Interfaces;
using Elder.Framework.Core;
using System;
using UnityEngine;

namespace Elder.Framework.BGM.Infra
{
    // [설계]
    // AddressableSceneLoader와 동일한 위치의 Infra 어댑터.
    // IAssetProvider 의존은 이 클래스 안에서만 존재 -> BGMSystem은 여기로만 접근.
    internal sealed class BGMAssetLoader : BaseSystem, IBGMAssetLoader
    {
        private IAssetProvider _assetProvider;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<IAssetProvider>(out _assetProvider))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IAssetProvider)}");
        }

        public async UniTask<IAssetHandle<AudioClip>> LoadClipAsync(string addressableKey)
        {
            // [설계] TODO: _assetProvider.GetAssetAsync<AudioClip>(addressableKey) 위임만 수행
            return await _assetProvider.GetAssetAsync<AudioClip>(addressableKey);
        }
    }
}
