namespace Elder.Core.Common.Enums
{
    public enum FluxPhase 
    {
        Pre,    // 가장 먼저 실행 (데이터 검증, 준비 등)
        Normal, // 일반 실행 (메인 로직)
        Post,    // 가장 나중 실행 (UI 갱신, 로그, 뒤처리 등)
        Max,
    }
}