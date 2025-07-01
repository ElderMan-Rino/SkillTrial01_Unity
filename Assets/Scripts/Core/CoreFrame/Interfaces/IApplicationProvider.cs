using Elder.Core.Common.Interfaces;

namespace Elder.Core.CoreFrame.Interfaces
{
    public interface IApplicationProvider 
    {
        public bool TryGetApplication<T>(out T targetApplication) where T : IApplication;

        /*
         * 현실적인 Best Practice
         * 읽기/질의용으로 IApplicationProvider를 허용
         * → 예: 다른 앱의 상태나 결과를 받아와야 할 때

         * 행위/로직 전달은 메시지 또는 Mediator/EventBus 사
         * → 예: "A가 끝나면 B가 실행돼야 한다" 같은 흐름은 느슨하게 연결

         * Application 간 참조는 하위 계층에만 위
         * → 예: 상위 Application이 여러 하위 Application을 조합/조정
         */
    }
}