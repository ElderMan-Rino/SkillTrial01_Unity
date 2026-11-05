using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Definitions;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.Framework.UI.App;
using Elder.Framework.UI.Infra;
using Elder.Framework.UI.Interfaces;
using Elder.TestModule.UITest.Interfaces;
using R3;
using System;
using System.Collections.Generic;

namespace Elder.TestModule.UITest.Infra
{
    // IUISystem 본계약은 UISystem(실 서비스)에 그대로 위임하고, 진단 조회만
    // UISystemDiagnosticsAdapter를 통해 노출한다. UISystem을 감싸는 래퍼 (ISP + DIP 유지).
    public sealed class UISystemTest : IUISystemTest, IDependencyInjectable
    {
        private ILoggerEx _logger = LogFacade.GetLogger<UISystemTest>();
        private IUISystem _uiSystem;
        private IUISystemDiagnostics _diagnostics;

        public Type ClassType => typeof(UISystemTest);
        public int Scope => GlobalScope.Id;

        public void InjectDependency(IComponentProvider provider)
        {
            if (!provider.TryGetComponent<IUISystem>(out _uiSystem))
            {
                _logger.Error($"Failed to resolve {nameof(IUISystem)}."); // [HEAP]
                return;
            }

            if (!provider.TryGetComponent<UISystem>(out var concreteUISystem))
            {
                _logger.Error($"Failed to resolve concrete {nameof(UISystem)}."); // [HEAP]
                return;
            }

            _diagnostics = new UISystemDiagnosticsAdapter(concreteUISystem); // [HEAP]
        }

        public bool TryFindUIView<T>(out T view) where T : UIViewBase
        {
            return _uiSystem.TryFindUIView<T>(out view);
        }

        public TPresenter CreatePresenter<TPresenter>() where TPresenter : IUIPresenter, new()
        {
            return _uiSystem.CreatePresenter<TPresenter>();
        }

        public UniTask<IUIViewHandle> CreateViewByAssetKeyAsync<TView>(string assetKey) where TView : UIViewBase
        {
            return _uiSystem.CreateViewByAssetKeyAsync<TView>(assetKey);
        }

        public UniTask<IUIViewHandle> CreateViewByDataKeyAsync<TView>(int dataKey) where TView : UIViewBase
        {
            return _uiSystem.CreateViewByDataKeyAsync<TView>(dataKey);
        }

        public IUIViewHandle CreateSceneViewHandle<TView>(TView view) where TView : UIViewBase
        {
            return _uiSystem.CreateSceneViewHandle(view);
        }

        public IUIEntryHandle CreateEntry(int scope, IUIViewHandle viewHandle, UIPresenterBase presenter, IUIRebindStrategy rebindStrategy)
        {
            return _uiSystem.CreateEntry(scope, viewHandle, presenter, rebindStrategy);
        }

        public UniTask OpenGlobalAsync(IUIEntryHandle entry)
        {
            return _uiSystem.OpenGlobalAsync(entry);
        }

        public UniTask OpenSceneAsync(IUIEntryHandle entry)
        {
            return _uiSystem.OpenSceneAsync(entry);
        }

        public UniTask ClearSceneScopeAsync(int scope)
        {
            return _uiSystem.ClearSceneScopeAsync(scope);
        }

        public UniTask ClearGlobalAsync()
        {
            return _uiSystem.ClearGlobalAsync();
        }

        public IReadOnlyList<IUIEntryHandle> GetSceneStackEntries()
        {
            return _diagnostics.GetSceneStackEntries();
        }

        public IReadOnlyList<IUIEntryHandle> GetGlobalLayerEntries()
        {
            return _diagnostics.GetGlobalLayerEntries();
        }

        public Observable<Unit> OnStackChanged => _diagnostics.OnStackChanged;
    }
}
