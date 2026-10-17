/*
using Cysharp.Threading.Tasks;
using Elder.Framework.Bgm.Interfaces;
using Elder.Framework.Core;
using Elder.Framework.Blob.Interfaces;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Signal.Ecs.Components;
using Elder.Framework.Signal.Ecs.Interfaces;
using System;
using Unity.Entities;

namespace Elder.Framework.Bgm.App
{
    // [설계 - BGMSystem]
    // 책임: (1) BGMPlaySignal/BGMStopSignal 구독·수신
    //       (2) BGMInfoRoot 대상 row lookup (SceneChanger.TryResolveSceneRow와 동일한
    //           StringHashHelper.ToStableHash 기반 스캔 방식으로, bgmKey -> addressableKey 변환)
    //       (3) 재생 상태 관리 (재생 중인 AudioClip 핸들 보관, 정지, 루프, 크로스페이드)
    //
    // 절대 IAssetProvider를 직접 참조하지 않음 -> IBGMAssetLoader에 위임 (SceneChanger가
    // ISceneLoader에 위임하는 구조를 그대로 미러링). 에셋 취득/해제 전략이 바뀌어도
    // BGMSystem 코드는 변경되지 않아야 함 (OCP).
    //
    // AudioSource 재생 자체는 Unity API이므로 추후 별도 Infra 어댑터
    // (예: IBGMPlaybackDriver/AudioSourceBGMPlaybackDriver)로 분리할 수도 있음 — 여기서는
    // 껍질 단계라 미정. 크로스페이드 등 시간 기반 처리는 UniTask 기반으로 구현 예정.
    internal sealed class BGMSystem : BaseSystem, IBGMSystem, ISignalEcsConsumer
    {
        private IBGMAssetLoader _assetLoader;
        private IDataProvider _dataProvider;
        private ILoggerEx _logger;

        // [설계] 현재 재생 중인 클립 핸들 보관 (Dispose 책임: BGMSystem)
        // private IAssetHandle<AudioClip> _activeClipHandle;
        // private AudioSource _audioSource;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<IBGMAssetLoader>(out _assetLoader)) throw new InvalidOperationException($"[DI] Required system not found: {nameof(IBGMAssetLoader)}");
            if (!TryGetSystem<IDataProvider>(out _dataProvider)) throw new InvalidOperationException($"[DI] Required system not found: {nameof(IDataProvider)}");
            if (TryGetSystem<ILoggerPublisher>(out var loggerPublisher)) _logger = loggerPublisher.GetLogger<BGMSystem>();
        }

        public void ConsumeSignals(EntityManager entityManager)
        {
            // [HEAP] LINQ 미사용 — SystemAPI 미가용 컨텍스트이므로 EntityQuery 직접 구성
            using (var playQuery = entityManager.CreateEntityQuery(typeof(BGMPlaySignalComponent)))
            using (var playEntities = playQuery.ToEntityArray(Unity.Collections.Allocator.Temp))
            {
                for (int i = 0; i < playEntities.Length; i++)
                {
                    var signal = entityManager.GetComponentData<BGMPlaySignalComponent>(playEntities[i]);
                    PlayAsync(signal.BGMKey.ToString()).Forget();
                    entityManager.DestroyEntity(playEntities[i]);
                }
            }

            using (var stopQuery = entityManager.CreateEntityQuery(typeof(BGMStopSignalComponent)))
            using (var stopEntities = stopQuery.ToEntityArray(Unity.Collections.Allocator.Temp))
            {
                for (int i = 0; i < stopEntities.Length; i++)
                {
                    Stop();
                    entityManager.DestroyEntity(stopEntities[i]);
                }
            }
        }

        public async UniTask PlayAsync(string bgmKey)
        {
            // [설계] TODO: TryResolveBGMRow(bgmKey, out addressableKey, out loop 등) 구현
            // if (!TryResolveBGMRow(bgmKey, out string addressableKey)) return;
            // var handle = await _assetLoader.LoadClipAsync(addressableKey);
            // 기존 재생 중인 클립 Dispose 후 신규 핸들 교체 + AudioSource.clip 할당 + Play
            await UniTask.CompletedTask;
        }

        public void Stop()
        {
            // [설계] TODO: AudioSource.Stop() + _activeClipHandle?.Dispose()
        }

        protected override void DisposeManagedResources()
        {
            // [설계] TODO: _activeClipHandle?.Dispose()
        }

        // [설계] TODO: SceneChanger.TryResolveSceneRow 미러링
        // private bool TryResolveBGMRow(string bgmKey, out string addressableKey)
        // {
        //     var blobRef = _dataProvider.GetData<BGMInfoRoot>();
        //     int targetHash = StringHashHelper.ToStableHash(bgmKey);
        //     for (int i = 0; i < blobRef.Value.Rows.Length; i++) { ... }
        // }
    }
}

*/
