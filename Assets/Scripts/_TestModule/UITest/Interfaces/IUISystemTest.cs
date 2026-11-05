using Elder.Framework.UI.Interfaces;

namespace Elder.TestModule.UITest.Interfaces
{
    // 진단 전용 확장. IUISystem 본계약은 건드리지 않고 테스트 모듈에서만 진단 조회/알림을 노출한다 (ISP).
    public interface IUISystemTest : IUISystem, IUISystemDiagnostics
    {
    }
}
