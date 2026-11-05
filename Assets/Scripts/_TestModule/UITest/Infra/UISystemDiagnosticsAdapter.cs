using Elder.Framework.UI.Infra;
using Elder.Framework.UI.Interfaces;
using Elder.TestModule.UITest.Interfaces;
using R3;
using System.Collections.Generic;

namespace Elder.TestModule.UITest.Infra
{
    // UISystem(_Framework, 구체 타입)의 내부 상태를 IUISystemDiagnostics로 노출하는 TestModule 측 어댑터.
    // _Framework는 _TestModule을 참조할 수 없으므로(asmdef), 진단 인터페이스는 여기서 구현한다.
    internal sealed class UISystemDiagnosticsAdapter : IUISystemDiagnostics
    {
        private readonly UISystem _uiSystem;

        public UISystemDiagnosticsAdapter(UISystem uiSystem)
        {
            _uiSystem = uiSystem;
        }

        public Observable<Unit> OnStackChanged => _uiSystem.OnStackChanged;

        public IReadOnlyList<IUIEntryHandle> GetSceneStackEntries()
        {
            return _uiSystem.GetSceneStackEntries();
        }

        public IReadOnlyList<IUIEntryHandle> GetGlobalLayerEntries()
        {
            return _uiSystem.GetGlobalLayerEntries();
        }
    }
}
