namespace Elder.Framework.Core.UI.Interfaces
{
    // 개별 UI 프리팹(View)이 구현하는 생명주기 계약. MonoBehaviour 구현체가 이 인터페이스를 구현.
    // 식별은 구현 타입(typeof(TView))을 그대로 사용하므로 별도 Id 불필요.
    public interface IUIView
    {
        // public void OnOpen(object parameter);   // Stack에 Push되고 활성화될 때. parameter는 Open 호출 측 전달값.
        // public void OnFocus();                  // 다른 UI에 가려졌다가 최상단으로 복귀했을 때 (BringToFront 포함).
        // public void OnBlur();                    // 다른 UI에 가려질 때 (상위에 새 UI가 쌓일 때).
        // public void OnClose();                   // Stack에서 Pop되어 파기되기 전.
        // public bool OnBackKeyPressed();          // 백 키 입력 수신. true 반환 시 이 UI가 입력을 소비(전파 중단).
    }
}
