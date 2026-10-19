using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using System;
using System.Threading;

namespace Elder.Framework.Root.Interfaces
{
    public interface IMainFrameworkRoot
    {
        public UniTask BootModuleAsync(Action<IComponentRegistry> installAction, Type startCoordinatorType, CancellationToken cancellationToken);
        // 인터페이스에 dispose 관련을 추가해야함
        // 추가로 유니티 이벤트 받았을때 처리하는 부분들도 추가해야함
        public void ShutDown();

        // 설계 결정: 앱 진입(최초 부팅) 창구는 ISceneService가 아닌 이곳에 둔다.
        //  - SRP: ISceneService는 "씬 전환"(UnloadScope + ISceneLoader) 단일 책임만 유지, 씬 로드가
        //    필요 없는 최초 부팅 경로를 섞지 않음.
        //  - DDD: 부팅은 Root(애그리거트 루트, Initialize를 이미 보유)의 경계에 속하는 유스케이스이고,
        //    씬 전환과는 서로 다른 바운디드 컨텍스트.
        //  - TestBootEntry.Run이 수행하던 install -> inject -> signal 흐름을 이 메서드로 흡수해
        //    Main/Test 양쪽 부팅 경로를 단일화한다(OCP: SceneService 쪽 분기 증가 없음).
        //
        // 흐름:
        //  1. installAction(_registry)       : 모듈 전용 컴포넌트 Regist<T>() 체인
        //  2. _registry.InjectDependencies() : 새로 등록된 컴포넌트에 의존성 주입
        //  3. FxModuleStartSignal(startCoordinatorType) 발행
        //       -> FlowHandler가 수신 -> Coordinator.StartAsync()
    }
}