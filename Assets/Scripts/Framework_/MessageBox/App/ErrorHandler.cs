/*
using Cysharp.Threading.Tasks;
using Elder.Framework.Blob.Infra.Extensions;
using Elder.Framework.Blob.Interfaces;
using Elder.Framework.Core;
using Elder.Framework.MessageBox.Interfaces;
using Elder.Framework.MessageBox.Messages;
using Elder.Framework.Signal.Helpers;
using Elder.Framework.Signal.Interfaces;
using System;

namespace Elder.Framework.MessageBox
{
    // [MISSING] 전역 에러→메세지박스 처리 흐름 설계 (SettingsApplyService.HandlePreSystemReady 참고)
    //   1) LogPublisher가 LogLevel.Error 디스패치 시 LogErrorSignal(errorCode 등) 발행 예정
    //      → LoggerEX/LogEvent에 시그널 발행용 매개변수 추가 필요 (미구현)
    //   2) 본 핸들러는 SystemReadySignal 수신 이후에만 LogErrorSignal 구독 시작
    //      → UISystem 초기화 보장 전에는 MessageBoxCoordinator.RequestAsync가 정상 동작하지 않음
    //   3) LogErrorSignal 수신 시 errorCode로 정책(message, buttonType, action) 조회
    //      → dismissPolicy는 본 핸들러 책임 아님 (MessageBox는 Ok/Cancel 버튼 바인딩만 담당)
    //   4) 정책이 Fatal인 경우, MessageBoxCoordinator.RequestAsync 호출 전후로
    //      Input 시스템에 입력 차단 시그널 발행/해제 (Input 모듈 미구현, 스텁 상태)
    //      → 배경 클릭/뒤로가기 무시는 MessageBox가 아닌 Input 차단으로 구현
    //   5) MessageBoxCoordinator.RequestAsync(message, buttonType) 결과(Ok/Cancel)에 따라
    //      후속 처리(BootErrorAction 형태: QuitGame/NavigateToTitle/Close) 실행
    public class ErrorHandler : BaseSystem
    {
        private ISignalRouter _router;
        private IMessageBoxCoordinator _messageBoxCoordinator;
        private IDataProvider _dataProvider;

        private SignalToken _subscription;

        protected override void HandleInjectDependency()
        {
            if (!TryGetSystem<ISignalRouter>(out _router)) throw new InvalidOperationException($"[DI] Required system not found: {nameof(ISignalRouter)}");
            if (!TryGetSystem<IMessageBoxCoordinator>(out _messageBoxCoordinator)) throw new InvalidOperationException($"[DI] Required system not found: {nameof(IMessageBoxCoordinator)}");
            if (!TryGetSystem<IDataProvider>(out _dataProvider)) throw new InvalidOperationException($"[DI] Required system not found: {nameof(IDataProvider)}");
        }

        public override UniTask InitializeAsync()
        {
            _subscription = _router.Subscribe<ErrorOccurredSignal>(HandleErrorOccurred);
            return UniTask.CompletedTask;
        }

        private void HandleErrorOccurred(in ErrorOccurredSignal signal)
        {
            // signal에서 받은 에러코드를 가지고 데이터에서 꺼내와서 처리 진행
            //if(_dataProvider.TryGetBlobData<>(out var errorCodeData))
            //_messageBoxCoordinator.OpenMessageBoxViewAsync();
        }

        protected override void DisposeManagedResources()
        {
            _subscription.Dispose();
        }
    }
}

*/
