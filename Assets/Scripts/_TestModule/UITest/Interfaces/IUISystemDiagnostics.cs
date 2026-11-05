using Elder.Framework.UI.Interfaces;
using R3;
using System.Collections.Generic;

namespace Elder.TestModule.UITest.Interfaces
{
    // IUISystem 본계약과 분리된 진단 전용 확장 (ISP). UISystem(_Framework)은 구체 타입으로만
    // 노출되며, UISystemTest가 이를 어댑팅해 IUISystemTest 경로로 노출한다.
    public interface IUISystemDiagnostics
    {
        public IReadOnlyList<IUIEntryHandle> GetSceneStackEntries();
        public IReadOnlyList<IUIEntryHandle> GetGlobalLayerEntries();

        // Open/Close 경로 모두 .Forget()된 비동기 체인 뒤에서 스택을 변경하므로,
        // 호출 직후 프레임 추측 대신 실제 변경 완료 시점에 알림을 받기 위한 스트림.
        public Observable<Unit> OnStackChanged { get; }
    }
}
