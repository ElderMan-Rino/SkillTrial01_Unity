using Cysharp.Threading.Tasks;
using Elder.Framework.Core.Interfaces;
using Elder.Framework.Scene.Interfaces;
using System;

namespace Elder.Framework.Scene.Infra
{
    // 설계 의도:
    //  SceneService는 "씬 전환"이라는 유스케이스의 오케스트레이션만 담당하고,
    //  실제 Unity 씬 전환은 ISceneLoader, 컴포넌트 생명주기는 IComponentRegistry(+IScopeUnloader)에 위임한다(SRP/DIP).
    //
    // EnterSceneAsync 흐름:
    //  1. _registry.UnloadScope(currentScope)   : 이전 scope 컴포넌트 PreDispose -> Dispose -> PostDispose
    //  2. _sceneLoader.LoadSceneAsync(sceneName) : 실제 Unity 씬 전환 (await)
    //  3. installAction(_registry)               : 새 scope Install (Regist<T>() 체인)
    //  4. _registry.InjectDependencies()          : 새로 등록된 컴포넌트에 의존성 주입
    //  5. startCoordinatorType 기동               : FxModuleStartSignal 발행 -> 구독자(FlowHandler)가 Coordinator 시작 호출
    //
    // 미해결 지점(코드화 전 확인 필요):
    //  - 4번: InjectDependencies()가 전체 _componentEntrys를 순회하므로, 기존 scope(GlobalScope 등)에
    //    이미 주입된 컴포넌트까지 재주입될 위험이 있음. Registry 쪽에 scope 인덱스/주입 완료 플래그를 두고
    //    "새로 등록된 것만" 주입하도록 ComponentRegistry를 보강해야 함.
    //  - 5번: startCoordinatorType(런타임 Type)으로 TryGetComponent<T>를 직접 호출하려면 MakeGenericType이
    //    필요한데 AOT 금지 규칙(#10) 위반. 현재는 TestBootEntry와 동일하게 Signal 발행 방식을 그대로 사용하고,
    //    구독자가 각자 컴포넌트를 resolve하도록 둔다. Coordinator 기동을 Registry 책임으로 당길지는 추후 결정.
    //  - currentScope를 SceneService가 어떻게 추적할지(이전 EnterSceneAsync 호출의 scope 값을 내부 필드로
    //    들고 있을지, 호출자가 매번 명시할지)는 아직 미정.
    //
    // 앱 진입(최초 부팅) 경로 확정: IMainFrameworkRoot.BootModuleAsync로 분리(SceneService 오버로드 기각).
    //  - 이유(SRP/DDD): SceneService는 "씬 전환" 단일 책임만 유지. 부팅은 Root(이미 Initialize를 보유)
    //    경계에 속하는 별개 유스케이스이며 TestBootEntry.Run이 쓰던 패턴과 합류시킴.
    //  - 상세는 IMainFrameworkRoot.BootModuleAsync 선언부 주석 참조.
    internal sealed class SceneService : ISceneService, IDependencyInjectable
    {
        public Type ClassType => typeof(SceneService);

        public int Scope => throw new NotImplementedException();

        public void InjectDependency(IComponentProvider provider)
        {
        }

        public UniTask EnterSceneAsync(string sceneName, int scope, Action<IComponentRegistry> installAction, Type startCoordinatorType)
        {
            throw new NotImplementedException();
        }
    }

    // ----------------------------------------------------------------------------------------
    // [설계 기록] 모듈(씬/미니게임) 단위 install -> inject -> start 공통 흐름 (Main / Test 양쪽 공통)
    // ----------------------------------------------------------------------------------------
    //
    // 전제: 진입 경로는 두 가지이며, 공통 3단계(install/inject/start)는 동일하되 "이전 상태 정리"와
    // "씬 로드" 유무만 다르다.
    //
    //  (A) 씬 전환 진입 (예: Title -> Main, 미니게임 A -> 미니게임 B)
    //      - 이전 scope가 존재 -> UnloadScope 필요.
    //      - 실제 Unity 씬 로드 필요 (ISceneLoader).
    //      - 창구: ISceneService.EnterSceneAsync(...)
    //
    //  (B) 앱 진입(최초 부팅) 진입 (예: TitleScene/TestScene 등에서 바로 시작, 씬 전환이 아니라
    //      그 씬이 곧 시작점인 경우. TestBootEntry.Run이 이미 이 패턴)
    //      - 이전 scope가 없음 -> UnloadScope 불필요.
    //      - 씬 로드 불필요(이미 해당 씬 위에서 실행 중).
    //      - 창구(확정): IMainFrameworkRoot.BootModuleAsync(installAction, startCoordinatorType).
    //        SceneService 오버로드 방식은 기각 - SceneService에 "씬 없는 부팅" 책임이 섞여 SRP 위반,
    //        TestBootEntry와 책임 중복됨.
    //
    // 공통 3단계(두 경로가 공유):
    //  1. installAction(_registry)         : 모듈 전용 컴포넌트 Regist<T>() 체인
    //  2. _registry.InjectDependencies()   : 새로 등록된 컴포넌트에 의존성 주입
    //  3. FxModuleStartSignal(startCoordinatorType) 발행
    //       -> FlowHandler(Signal 타입당 범용 구독자, 모듈 수와 무관하게 1~2개만 존재)가 수신
    //            -> startCoordinatorType으로 Coordinator를 resolve(Type 기반, MakeGenericType 없이
    //               구독자 측에서 TryGetComponent 분기 - AOT 규칙 #10 준수)
    //                 -> Coordinator.StartAsync() : 모듈별 고유 부팅(View-Presenter 결합, 데이터 로드,
    //                    초기 상태 세팅)을 캡슐화. View/데이터 책임은 FlowHandler가 아니라 여기 둔다
    //                    (SRP: FlowHandler는 라우팅만 / DIP: FlowHandler -> Coordinator -> UISystem
    //                    방향 고정 / ISP: FlowHandler는 Coordinator의 좁은 계약만 안다).
    //
    // (A) 씬 전환 경로 전체 흐름:
    //  1. _registry.UnloadScope(currentScope)    : 이전 scope 컴포넌트 PreDispose -> Dispose -> PostDispose
    //  2. _sceneLoader.LoadSceneAsync(sceneName)  : 실제 Unity 씬 전환 (await)
    //  3~5. 공통 3단계(installAction -> InjectDependencies -> FxModuleStartSignal)
    //
    // 100개 미니게임 확장 문제(IInstallerCatalog, 가칭):
    //  - 호출부가 매번 installAction/startCoordinatorType을 직접 알아야 하면 100개 모듈 분기가 호출부에
    //    누적되어 OCP 위반. 부팅 시점에 (key, installAction, startCoordinatorType) 묶음을 사전 등록해두고,
    //    호출부는 key만 넘기도록 중간 조회 계층(IInstallerCatalog)을 둔다.
    //      - 등록: IInstallerCatalog.Register(key, installAction, startCoordinatorType)  (부팅 시 100개 전부)
    //      - 조회: ISceneService.EnterSceneAsync(key) 내부에서 _catalog.Resolve(key)로 installAction/
    //        startCoordinatorType을 얻어 기존 EnterSceneAsync(sceneName, scope, installAction,
    //        startCoordinatorType)에 위임
    //      - 아직 코드화 전. 정적 목록 vs Addressable 기반 지연 등록 여부는 미결정.
    //
    // 미해결 지점(코드화 전 확인 필요):
    //  - InjectDependencies()가 전체 _componentEntrys를 순회하므로, 기존 scope(GlobalScope 등)에
    //    이미 주입된 컴포넌트까지 재주입될 위험. Registry 쪽에 scope 인덱스/주입 완료 플래그를 두고
    //    "새로 등록된 것만" 주입하도록 ComponentRegistry를 보강해야 함.
    //  - currentScope를 SceneService가 어떻게 추적할지(내부 필드로 들고 있을지, 호출자가 매번 명시할지)
    //    미정.
    //  - (B) 앱 진입 경로 창구 확정: IMainFrameworkRoot.BootModuleAsync(installAction, startCoordinatorType).
    // ----------------------------------------------------------------------------------------
}
