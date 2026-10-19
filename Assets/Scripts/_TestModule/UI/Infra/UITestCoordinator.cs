using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using Elder.TestModule.AssetLoad.Infra;
using Elder.TestModule.Common.Definitions;
using Elder.TestModule.UI.Interfaces;
using System;
using UnityEngine;

namespace Elder.TestModule.UI.Infra
{

    public class UITestCoordinator : IUITestCoordinator, IDependencyInjectable, IPreDisposable, IDisposable
    {
        private ILoggerEx _logger = LogFacade.GetLogger<UITestCoordinator>();

        public Type ClassType => typeof(UITestCoordinator);

        public int Scope => TestScope.Id;

    

        public void InjectDependency(IComponentProvider provider)
        {

        }

        // MVP+VM 연동 플랜 (TestModule UI):
        // UITestFlowHandler.HandleModuleStart -> StartAsync() 진입
        //   -> IUISystem.TryFindUIView<TView>()로 씬에 이미 존재하는 View(MonoBehaviour)를 탐색
        //      (Addressable 동적 로드가 아니라 씬에 배치된 View를 그대로 사용하는 경로이므로 IUIViewFactory는 사용하지 않는다)
        //   -> 탐색 성공 시 IUIPresenterFactory.Create<TPresenter>()로 Presenter 생성 (new + InjectDependency)
        //   -> presenter.BindView(view) 호출로 View의 Observable <-> Presenter 핸들러 연결 (AssetLoadTestPresenter.BindView 참고)
        //   -> Presenter 내부에서 GetOrCreateViewModel<TViewModel>(factory)로 VM 생성/재사용 (Sleep 중에도 Presenter와 함께 유지)
        // 의존 방향: UITestFlowHandler -> UITestCoordinator -> IUISystem/IUIPresenterFactory (역방향 없음, Coordinator는 Flux를 모름)
        public UniTask StartAsync()
        {
            throw new NotImplementedException();
        }

        public void PreDispose()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}