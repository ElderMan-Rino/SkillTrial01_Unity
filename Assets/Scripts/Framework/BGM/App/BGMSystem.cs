using Cysharp.Threading.Tasks;
using Elder.Framework.BGM.Interfaces;
using Elder.Framework.BGM.Messages;
using Elder.Framework.Core;
using Elder.Framework.Blob.Interfaces;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using System;
using UnityEngine;

namespace Elder.Framework.BGM.App
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
    internal sealed class BGMSystem : BaseSystem, IBGMSystem
    {
        private IBGMAssetLoader _assetLoader;
        private IDataProvider _dataProvider;
        private ISignalRouter _router;
        private ILoggerEx _logger;

        private SignalToken _playSubscription;
        private SignalToken _stopSubscription;

        // [설계] 현재 재생 중인 클립 핸들 보관 (Dispose 책임: BGMSystem)
        // private IAssetHandle<AudioClip> _activeClipHandle;
        // private AudioSource _audioSource;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<IBGMAssetLoader>(out _assetLoader))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IBGMAssetLoader)}");
            if (!TryGetSystem<IDataProvider>(out _dataProvider))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(IDataProvider)}");
            if (!TryGetSystem<ISignalRouter>(out _router))
                throw new InvalidOperationException($"[DI] Required system not found: {nameof(ISignalRouter)}");
            if (TryGetSystem<ILoggerPublisher>(out var loggerPublisher)) _logger = loggerPublisher.GetLogger<BGMSystem>();
        }

        public override UniTask InitializeAsync()
        {
            // [HEAP] Subscribe — 시스템 초기화 시 1회 핸들러 등록
            _playSubscription = _router.Subscribe<BGMPlaySignal>(HandleBGMPlay);
            _stopSubscription = _router.Subscribe<BGMStopSignal>(HandleBGMStop);
            return UniTask.CompletedTask;
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
            _playSubscription.Dispose();
            _stopSubscription.Dispose();
            // [설계] TODO: _activeClipHandle?.Dispose()
        }

        private void HandleBGMPlay(in BGMPlaySignal signal)
        {
            PlayAsync(signal.BGMKey).Forget();
        }

        private void HandleBGMStop(in BGMStopSignal signal)
        {
            Stop();
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
