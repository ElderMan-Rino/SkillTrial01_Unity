/*
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Elder.Framework.Boot.Messages;
using Elder.Framework.Core;
using Elder.Framework.Settings.Interfaces;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using System;

namespace Elder.Framework.Settings.App
{
    internal sealed class SettingsApplyService : BaseSystem
    {
        private ISignalRouter _router;
        private IAppSettingsStore _store;
        // [HEAP] 빌드 시점 1회 할당
        private readonly List<ISettingsApplicable> _applicables = new();

        private SignalToken _subscription;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<ISignalRouter>(out _router)) throw new InvalidOperationException($"[DI] Required system not found: {nameof(ISignalRouter)}");
            if (!TryGetSystem<IAppSettingsStore>(out _store)) throw new InvalidOperationException($"[DI] Required system not found: {nameof(IAppSettingsStore)}");
        }

        public override UniTask InitializeAsync()
        {
            // [HEAP] Subscribe 내부 핸들러 래핑 객체 1회 할당 (초기화 시점)
            _subscription = _router.Subscribe<PreSystemReadySignal>(HandlePreSystemReady);
            return UniTask.CompletedTask;
        }

        public override UniTask PostInitializeAsync() => UniTask.CompletedTask;

        public void RegisterApplicable(ISettingsApplicable applicable)
        {
            _applicables.Add(applicable);
        }

        // [MISSING] 전역 에러 처리 흐름 (BootErrorHandler에서 이관, 전역 사용 목적)
        //   1) LogPublisher가 LogLevel.Error 디스패치 시 LogErrorSignal(errorCode 등) 발행 예정
        //      → LoggerEX/LogEvent에 시그널 발행용 매개변수 추가 예정 (미구현, 설계 중)
        //   2) BootErrorHandler 등 에러 구독자는 SystemReadySignal 수신 전까지 LogErrorSignal 구독 시작 금지
        //      → UISystem 포함 전체 시스템 초기화가 끝나야 MessageBoxCoordinator.RequestAsync가
        //        정상 동작(PrepareAsync<MessageBoxView> 완료 보장)하기 때문
        //   3) SystemReadySignal 수신 시점에 구독 시작 → LogErrorSignal 수신 시 정책 조회 후
        //      MessageBoxCoordinator.RequestAsync → 결과에 따라 후속 처리(BootErrorAction) 실행
        private void HandlePreSystemReady(in PreSystemReadySignal signal)
        {
            for (int i = 0; i < _applicables.Count; i++)
                _applicables[i].ApplySettings(_store);

            _router.Publish(new SystemReadySignal());
        }

        protected override void DisposeManagedResources()
        {
            _subscription.Dispose();
        }
    }
}

*/
