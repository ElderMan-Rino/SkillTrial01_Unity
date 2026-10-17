using Elder.Framework.Core.Definitions;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Core.UI.App;
using Elder.Framework.Core.UI.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Infra;
using Elder.Framework.Log.Interfaces;
using System;

namespace Elder.Framework.Core.UI.Infra
{
    public sealed class UISystem : IUISystem, IDependencyInjectable
    {
        // 얘가 stack도 관리해야하니까 presenter를 만드는게 맞고 ( 특정 제일 위에 있는 스택부터 - n개까지만 보존하는 형태 )
        // 코디네이터에게는 presenter가 들어간 handle을 뿌려주는게 맞을거같은데?

        private ILoggerEx _logger = LogFacade.GetLogger<UISystem>();
        private IUIViewFinder _viewFinder;
        private IUIPresenterFactory _presenterFactory;

        public Type ClassType => typeof(UISystem);

        public int Scope => GlobalScope.Id;

        public void InjectDependency(IComponentProvider provider)
        {
            if(!provider.TryGetComponent<IUIViewFinder>(out _viewFinder))
                _logger.Error($"Failed to resolve {nameof(IUIViewFinder)}.");  // [HEAP]
        }

        public bool TryFindUIView<T>(out T view) where T : UIViewBase
        {
            return _viewFinder.TryFindUIView<T>(out view);
        }

        // UI Open 흐름 (예: Inventory):
        // FxOpenInventory 발행
        //   -> InventoryFlowHandler (Flux 구독자, Coordinator를 참조)
        //     -> IInventoryCoordinator.OpenInventoryAsync() 호출
        //       -> UISystem/PresenterFactory가 View+ViewModel+Presenter 조합, 스택 관리
        //       -> Coordinator는 handle(Presenter 인터페이스)만 받아 Show/Hide 흐름 제어
        // 의존 방향: FlowHandler -> Coordinator -> UISystem (역방향 참조 없음, Coordinator는 FlowHandler/Flux를 모름)
    }
}