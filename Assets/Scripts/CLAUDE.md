# C# 코딩 컨벤션 (MSDN + 프로젝트 기반)

이 프로젝트는 **MSDN C# 코딩 컨벤션**을 기준으로 하며, 아래 규칙이 우선 적용됩니다.  
Unity + VContainer + UniTask + ECS Blob + Flux 메시징 구조를 사용합니다.

> **코드 작성 3대 원칙: 최적화 · 유지보수성 · 명확성**  
> 힙 할당이 발생하는 코드에는 반드시 `// [HEAP]` 주석을 명시한다.  
> AOT(IL2CPP) 환경의 코드 폭발(Code Bloat) 위험이 있는 패턴은 반드시 `// [AOT RISK]` 주석을 명시한다.

---

## 1. 네이밍 규칙

### 대소문자 규칙 (MSDN 기준)

| 대상 | 방식 | 예시 |
|------|------|------|
| 클래스 / 인터페이스 / 구조체 | PascalCase | `FluxRouter`, `IFluxMessage` |
| 메서드 | PascalCase | `LoadSheetAsync`, `GetData` |
| 프로퍼티 | PascalCase | `MainSceneName`, `DomainEvents` |
| 이벤트 | PascalCase | `OnInitialized` |
| 열거형 타입 및 값 | PascalCase | `LogLevel.Warning` |
| 로컬 변수 / 파라미터 | camelCase | `targetSceneKey`, `assetName` |
| private 필드 | `_camelCase` (언더스코어 접두사) | `_router`, `_dataHandles` |
| static 필드 | `_camelCase` (언더스코어 접두사) | `_globalTokenIdCounter` |
| const | PascalCase | `BootStrapSceneKey` |
| 인터페이스 | `I` 접두사 | `IFluxRouter`, `IDataProvider` |

### 프로젝트 특화 네이밍

| 대상 | 규칙 | 예시 |
|------|------|------|
| Flux 메시지 struct | `Fx` 접두사 | `FxInitializeSystem`, `FxSceneTransition` |
| Installer | `*Installer` 접미사 | `FluxInstaller`, `DataInstaller` |
| 비동기 메서드 | `*Async` 접미사 | `LoadSheetAsync`, `GetAssetAsync` |
| 도메인 이벤트 | `*Event` 접미사 또는 의미 있는 과거형 | `DataLoadedEvent` |

---

## 2. 접근 제한자 (Access Modifier) 규칙

### 기본 원칙

- **모든 멤버에 접근 제한자를 반드시 명시한다** (생략 절대 금지).
- **최소 권한 원칙(Principle of Least Privilege)**: 필요한 최소 범위의 접근 제한자를 사용한다.
- 구현 클래스는 `internal`을 선호하고, 인터페이스를 통해서만 외부에 노출한다.

### public 규칙

외부(다른 어셈블리 또는 레이어)에서 반드시 접근해야 하는 경우에만 사용한다.

```csharp
// GOOD: 외부에 공개가 필요한 인터페이스 계약
public interface IFluxRouter
{
    void Publish<T>(in T message) where T : struct;
    SubscriptionToken Subscribe<T>(MessageHandler<T> handler) where T : struct;
}

// GOOD: DI 컨테이너에 노출할 Application Service
public class DataProvider : IDataProvider
{
    public void Initialize() { }     // 인터페이스 구현 → public 필수
    public void Dispose() { }        // IDisposable 구현 → public 필수
}
```

### private 규칙

클래스 내부에서만 사용하는 모든 멤버는 `private`을 명시한다.  
필드는 반드시 `_camelCase` 형태로 작성한다.

```csharp
public class DataProvider : IDataProvider, IDisposable
{
    // GOOD: private 필드는 _ 접두사 + camelCase
    private readonly IFluxRouter _router;
    private readonly IAssetProvider _asset;
    private SubscriptionToken _subscription;
    private bool _isInitialized;

    // GOOD: private 메서드는 PascalCase (함수 규칙 동일)
    private void HandleInitializeSystem(in FxInitializeSystem msg)
    {
        LoadDataAsync().Forget();
    }

    private async UniTaskVoid LoadDataAsync()
    {
        await _asset.LoadAsync<SomeData>("key");
    }
}
```

### protected 규칙

상속 구조에서 파생 클래스에만 공개가 필요한 경우에 사용한다.  
`protected virtual` / `protected override`를 통한 템플릿 메서드 패턴에 활용한다.

```csharp
public abstract class DisposableBase : IDisposable
{
    // GOOD: 파생 클래스에서 재정의할 훅 메서드
    protected virtual void OnDisposing() { }
    protected virtual void DisposeManagedResources() { }
    protected virtual void DisposeUnmanagedResources() { }
    protected virtual void FinalizeDispose() { }

    // GOOD: 외부에 공개되는 Dispose만 public
    public void Dispose()
    {
        OnDisposing();
        DisposeManagedResources();
        DisposeUnmanagedResources();
        FinalizeDispose();
    }
}
```

### internal 규칙

같은 어셈블리 내부에서만 접근 가능. 구현 클래스의 기본 접근 제한자로 사용한다.

```csharp
// GOOD: 구현 클래스는 internal, 인터페이스로만 외부 노출
internal sealed class FluxRouter : IFluxRouter
{
    private readonly Dictionary<Type, object> _handlerContainers = new();

    public void Publish<T>(in T message) where T : struct
    {
        if (!_handlerContainers.TryGetValue(typeof(T), out var container)) return;
        // [HEAP] Unsafe.As로 박싱 없이 캐스팅 (박싱 회피)
        Unsafe.As<MessageHandlerContainer<T>>(container).Publish(in message);
    }
}
```

### 접근 제한자 우선순위 요약

```
public → protected → internal → protected internal → private protected → private
```

| 시나리오 | 권장 접근 제한자 |
|----------|----------------|
| 인터페이스 계약 노출 | `public` |
| 인터페이스 멤버 구현 | `public` (인터페이스 명시 구현 제외) |
| 구현 클래스 자체 | `internal sealed` |
| 상속용 훅 메서드 | `protected virtual` |
| 클래스 내부 전용 필드/메서드 | `private` |
| 동일 어셈블리 한정 유틸리티 | `internal` |

---

## 3. 인터페이스 규칙

### 인터페이스 멤버 접근 제한자 명시

C#의 인터페이스 멤버는 기본적으로 `public abstract`이지만,  
**이 프로젝트에서는 인터페이스 멤버에도 반드시 `public`을 명시한다.**

```csharp
// GOOD: 인터페이스 멤버에 public 명시
public interface IDataProvider
{
    public void Initialize();
    public void Dispose();
    public bool TryGetData<T>(string key, out T data) where T : unmanaged;
}

public interface IFluxRouter
{
    public void Publish<T>(in T message) where T : struct;
    public SubscriptionToken Subscribe<T>(MessageHandler<T> handler) where T : struct;
    public void Unsubscribe(SubscriptionToken token);
}

// BAD: 접근 제한자 생략 (암묵적 public에 의존)
public interface IDataProvider
{
    void Initialize();     // ← 접근 제한자 없음, 금지
    bool TryGetData<T>(string key, out T data) where T : unmanaged;
}
```

### 인터페이스 설계 원칙 (ISP)

역할 단위로 인터페이스를 분리한다. 하나의 인터페이스에 여러 책임을 몰아넣지 않는다.

```csharp
// GOOD: 역할별 분리
public interface IEngineAssetLoader
{
    public UniTask<IAssetHandle<T>> LoadAsync<T>(string key) where T : UnityEngine.Object;
}

public interface IEngineAssetReleaser
{
    public void Release(string key);
}

// BAD: 단일 인터페이스에 모든 책임 집중
public interface IAssetManager   // [VIOLATION: ISP]
{
    UniTask Load();
    void Release();
    void Preload();
    void Clear();
}
```

### 명시적 인터페이스 구현 (Explicit Implementation)

외부에서 인터페이스 타입으로만 접근해야 하고, 구현 타입에서 직접 호출을 막고 싶을 때 사용한다.  
이 경우에는 접근 제한자를 붙이지 않는다 (C# 언어 규격).

```csharp
internal sealed class SpecialRouter : IFluxRouter, IDisposable
{
    // GOOD: 명시적 구현 - 접근 제한자 없음 (언어 규격)
    void IDisposable.Dispose()
    {
        CleanupResources();
    }

    // GOOD: 일반 구현 - public 명시
    public void Publish<T>(in T message) where T : struct { }
}
```

---

## 4. 제어 흐름 (if문) 규칙

### 핵심 원칙

- **단일 문장 if**: 중괄호 없이 `if (조건) 실행문;` 형태로 **같은 줄에** 작성한다.
- **복수 문장 / 블록 if**: 반드시 중괄호 + 줄바꿈(Allman 스타일)을 사용한다.
- **else / else if**: 항상 중괄호 + 줄바꿈을 사용한다.

```csharp
// ✅ GOOD: 단일 Guard clause — 한 줄 처리
if (handle is null) return;
if (!_isInitialized) return;
if (!_entries.TryGetValue(key, out var entry)) return;

// ✅ GOOD: 단일 할당/호출 — 한 줄 처리
if (isVisible) Enable();
if (count > 0) ProcessQueue();

// ✅ GOOD: 복수 문장 — 반드시 중괄호 + 줄바꿈
if (isReady)
{
    LoadData();
    NotifyReady();
}

// ✅ GOOD: else if / else — 반드시 중괄호 + 줄바꿈
if (state == State.Running)
{
    UpdateLogic();
}
else if (state == State.Paused)
{
    PauseLogic();
}
else
{
    StopLogic();
}

// ❌ BAD: 복수 문장인데 중괄호 없음
if (isReady)
    LoadData();
    NotifyReady();  // ← 항상 실행됨! 버그 유발

// ❌ BAD: 단일 문장인데 불필요하게 줄바꿈
if (handle is null)
    return;         // ← Guard clause는 한 줄이 더 명확함

// ❌ BAD: else 중괄호 없음
if (isReady) DoA();
else DoB();         // ← 금지
```

### Guard Clause 패턴 (조기 반환 우선)

중첩 if를 줄이고, 실패 조건을 먼저 처리하는 Guard Clause 패턴을 사용한다.

```csharp
// GOOD: Guard Clause로 중첩 제거
public void Publish<T>(in T message) where T : struct
{
    if (!_handlerContainers.TryGetValue(typeof(T), out var container)) return;
    if (!container.HasHandlers) return;

    container.Publish(in message);
}

// BAD: 깊은 중첩
public void Publish<T>(in T message) where T : struct
{
    if (_handlerContainers.TryGetValue(typeof(T), out var container))
    {
        if (container.HasHandlers)
        {
            container.Publish(in message);
        }
    }
}
```

---

## 5. 파일 및 폴더 구조

### 레이어드 아키텍처 폴더 규칙

```
{Module}/
  Interfaces/    - 인터페이스 (계약 정의)
  App/           - Application 서비스 (Use Case)
  Domain/        - 도메인 모델 (Entity, VO, Aggregate)
  Infra/         - 인프라 구현체 (Unity API, 외부 라이브러리)
  Installer/     - VContainer DI 등록
  Messages/      - Flux 메시지 (IFluxMessage 구현체)
  Definitions/   - 열거형, 델리게이트, 상수
```

- 파일명은 클래스명과 반드시 일치
- 하나의 파일에 하나의 public 타입만 정의
- 인터페이스 파일명: `I{Name}.cs`

---

## 6. 언어 구문 스타일 (MSDN 기준)

### 타입 선언

```csharp
// GOOD: var는 타입이 명확히 드러날 때만 사용
var container = new MessageHandlerContainer<T>();
MessageHandler<T> handler = GetHandler();  // 타입 불명확할 때는 명시

// GOOD: string은 System.String 대신 키워드 사용
string name = "value";

// GOOD: new() 타깃 타입 추론 활용
private readonly Dictionary<Type, object> _dataHandles = new();
```

### 프로퍼티

```csharp
// GOOD: 읽기 전용 프로퍼티는 auto-property 또는 expression body
public string MainSceneName { get; private set; }
public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

// GOOD: init-only 프로퍼티 활용
public TId Id { get; protected set; }
```

### Null 처리

```csharp
// GOOD: null 체크는 is null / is not null 사용 (MSDN 권장)
if (handle is null) return;
if (provider is not null) provider.Load();

// GOOD: null 병합 연산자 활용
_router?.Publish(new FxInitializeSystem());
```

### string

```csharp
// GOOD: 문자열 보간 사용
// [HEAP] 문자열 보간은 런타임에 새 string 객체를 생성함 (로그 등 빈도 낮은 곳에만 사용)
_logger.Error($"Failed to load: {assetName}");

// AVOID: string.Format 직접 호출 지양
_logger.Error(string.Format("Failed to load: {0}", assetName));

// GOOD: 핫패스에서는 string 생성 자체를 피함
// (로그 레벨 체크 후 보간 실행)
if (_logger.IsErrorEnabled) _logger.Error($"Failed: {assetName}");
```

---

## 7. 코드 레이아웃

### 중괄호 (Allman 스타일, MSDN 기본)

```csharp
// GOOD
public void Initialize()
{
    _logger = LogFacade.GetLoggerFor<DataProvider>();
    _subscription = _router.Subscribe<FxInitializeSystem>(HandleInitializeSystem);
}

// BAD: 단일 라인 중괄호 (복잡한 메서드)
public void Initialize() { _logger = LogFacade.GetLoggerFor<DataProvider>(); }
```

### using 정렬 순서

```csharp
// 1. System 네임스페이스
using System;
using System.Collections.Generic;

// 2. 서드파티 (알파벳 순)
using Cysharp.Threading.Tasks;
using MessagePack;
using Unity.Entities;
using UnityEngine;
using VContainer;
using VContainer.Unity;

// 3. 내부 프로젝트 (Elder.*)
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Flux.Interfaces;
```

### 멤버 선언 순서

```csharp
public class Example
{
    // 1. private const / static readonly
    private static long _globalCounter;

    // 2. private readonly fields
    private readonly IFluxRouter _router;

    // 3. private fields
    private SubscriptionToken _subscription;

    // 4. 생성자
    public Example(IFluxRouter router) { }

    // 5. public 프로퍼티
    public string Name { get; }

    // 6. public 메서드 (인터페이스 구현 포함)
    public void Initialize() { }

    // 7. protected 메서드
    protected override void DisposeManagedResources() { }

    // 8. private 메서드
    private void HandleMessage(in FxInitializeSystem msg) { }
}
```

---

## 8. 주석 규칙

```csharp
// GOOD: WHY가 비명확한 경우에만 주석 작성
// Unsafe.As는 Dictionary lookup 박싱을 피하기 위한 성능 최적화
var container = Unsafe.As<MessageHandlerContainer<T>>(containerBase);

// GOOD: 한국어 XML 주석은 public API에만 (선택적)
/// <summary>구독 해제 시 반드시 Dispose 호출 필요</summary>
public SubscriptionToken Subscribe<T>(MessageHandler<T> handler) where T : struct { }

// BAD: WHAT을 설명하는 주석 (코드 자체가 설명해야 함)
// 핸들러를 가져옴
var handler = GetHandler();
```

### 성능 관련 필수 주석 태그

코드에 아래 상황이 발생하면 **반드시** 해당 태그를 주석으로 명시한다.

| 태그 | 설명 | 예시 상황 |
|------|------|-----------|
| `// [HEAP]` | 힙 할당 발생 | `new`, 람다 캡처, 박싱, string 생성, `params`, LINQ |
| `// [AOT RISK]` | IL2CPP 코드 폭발 위험 | 과도한 제네릭 조합, 리플렉션, `MakeGenericType` |
| `// [GC PRESSURE]` | 반복적 힙 할당으로 GC 유발 가능 | 핫패스 내 반복 `new`, 클로저 캡처 루프 |
| `// [BOXING]` | 값 타입 박싱 발생 | interface 캐스팅, object 저장, non-generic 컬렉션 |

```csharp
// [HEAP] 람다는 클로저 캡처 시 힙 할당 발생 — 핫패스에서 사용 금지
Action callback = () => HandleComplete(result);

// [BOXING] enum을 object으로 전달 시 박싱 발생
object boxed = LogLevel.Warning;   // [BOXING]

// [GC PRESSURE] 루프 내 string 보간은 반복 힙 할당 유발
for (int i = 0; i < 1000; i++)
{
    Log($"Item {i}");  // [GC PRESSURE] — 루프 밖으로 분리 또는 StringBuilder 사용
}

// [AOT RISK] 런타임 제네릭 인스턴스화는 IL2CPP 코드 폭발 유발
Type generic = typeof(List<>).MakeGenericType(someType);  // [AOT RISK]
```

---

## 9. 성능 최적화 & 힙 할당 방지

### 힙 할당이 발생하는 주요 패턴 (반드시 [HEAP] 명시)

```csharp
// [HEAP] new로 클래스 인스턴스 생성
var list = new List<int>();

// [HEAP] 람다가 외부 변수를 캡처하면 클로저 객체 힙 할당
int captured = 10;
Action a = () => Debug.Log(captured);  // [HEAP] 클로저

// [HEAP] params 키워드는 내부적으로 배열 할당
void Log(params object[] args) { }
Log("a", "b");  // [HEAP] 매 호출마다 object[] 배열 생성

// [BOXING] 값 타입 → 인터페이스/object 변환 시 박싱
IComparable boxed = 42;           // [BOXING]
object obj = someEnum;             // [BOXING]

// [HEAP] LINQ는 내부적으로 열거자 객체를 힙에 할당
var filtered = list.Where(x => x > 0).ToList();  // [HEAP]
```

### 힙 할당 회피 패턴

```csharp
// GOOD: struct로 값 타입 활용 (힙 할당 없음)
public readonly struct FxInitializeSystem : IFluxMessage { }

// GOOD: in 파라미터로 struct 복사 비용 제거
public void Publish<T>(in T message) where T : struct { }

// GOOD: stackalloc으로 스택 배열 사용 (힙 할당 없음)
Span<int> buffer = stackalloc int[64];

// GOOD: ArrayPool로 배열 재사용
var array = ArrayPool<byte>.Shared.Rent(256);
try { /* 사용 */ }
finally { ArrayPool<byte>.Shared.Return(array); }

// GOOD: 캡처 없는 static 람다 (C# 9+) — 힙 할당 없음
private static readonly Action<int> s_log = static x => Debug.Log(x);

// GOOD: Unsafe.As로 박싱 없이 타입 변환
// [HEAP 없음] Unsafe.As는 참조만 재해석, 새 객체 생성 없음
var container = Unsafe.As<MessageHandlerContainer<T>>(containerBase);
```

### 핫패스(Hot Path) 체크리스트

Update / FixedUpdate / 매 프레임 호출되는 함수에서 아래 항목을 금지한다.

- `new` 클래스 인스턴스 생성 (구조체는 허용)
- LINQ 사용
- string 보간 / string.Format
- 박싱 유발 코드
- 람다 클로저 캡처
- `params` 배열 생성
- `foreach`를 List/Array 이외의 컬렉션에 사용 (열거자 힙 할당)

```csharp
// BAD: 매 프레임 힙 할당
private void Update()
{
    var enemies = _enemies.Where(e => e.IsAlive).ToList();  // [HEAP][GC PRESSURE]
    foreach (var e in enemies)
        e.Tick(Time.deltaTime);
}

// GOOD: 힙 할당 없는 업데이트
private void Update()
{
    for (int i = 0; i < _enemies.Count; i++)
    {
        if (_enemies[i].IsAlive) _enemies[i].Tick(Time.deltaTime);
    }
}
```

---

## 10. AOT(IL2CPP) 주의 사항

Unity IL2CPP 환경에서는 AOT(Ahead-of-Time) 컴파일을 사용하므로, 런타임 코드 생성이 불가능하다.  
코드 폭발(Code Bloat) 및 AOT 오류를 유발하는 패턴에는 반드시 `// [AOT RISK]` 주석을 명시한다.

### 위험 패턴

```csharp
// [AOT RISK] MakeGenericType / MakeGenericMethod — IL2CPP에서 AOT 오류 가능
Type genericType = typeof(List<>).MakeGenericType(runtimeType);

// [AOT RISK] Activator.CreateInstance<T> — 리플렉션 기반 인스턴스화
object instance = Activator.CreateInstance(type);

// [AOT RISK] dynamic 키워드 — IL2CPP 미지원
dynamic value = GetValue();

// [AOT RISK] 과도한 제네릭 중첩 — 코드 폭발(Code Bloat) 유발
// 예: Handler<Wrapper<Message<Data<T>>>> 구조는 T 조합마다 별도 코드 생성
public class Handler<T> where T : struct
{
    public Container<Wrapper<T>> GetContainer() => new();  // [AOT RISK] 조합 폭발
}
```

### 안전 패턴

```csharp
// GOOD: 제네릭 constraint로 범위 제한 → 코드 폭발 억제
public void Publish<T>(in T message) where T : struct, IFluxMessage { }

// GOOD: 비제네릭 베이스 클래스로 공통 로직 분리
public abstract class HandlerContainerBase
{
    public abstract void Publish(object message);  // 비제네릭 진입점
}

public sealed class MessageHandlerContainer<T> : HandlerContainerBase where T : struct
{
    public override void Publish(object message)
    {
        // [BOXING] object → T 언박싱 발생, 단 진입점에서 1회만
        Publish((T)message);
    }

    public void Publish(in T message) { /* 핵심 로직 */ }
}

// GOOD: MessagePack / MemoryPack 등 AOT 대응 직렬화 사용
// Newtonsoft.Json 리플렉션 기반 직렬화 대신 코드 생성 방식 사용
[MessagePackObject]
public readonly struct PlayerData
{
    [Key(0)] public readonly int Id;
    [Key(1)] public readonly string Name;
}
```

### 제네릭 설계 가이드

| 상황 | 권장 방식 |
|------|-----------|
| 제네릭 파라미터 수 | 1~2개로 제한. 3개 이상은 설계 재검토 |
| 제네릭 중첩 depth | 2단계 이내 (`A<B<T>>` 까지만) |
| 제네릭 + 인터페이스 조합 | constraint로 범위 명시 필수 |
| 리플렉션 필요 시 | Editor 전용 코드로 분리 (`#if UNITY_EDITOR`) |

---

## 11. 비동기 패턴

```csharp
// GOOD: 모든 비동기 메서드는 UniTask 반환
public async UniTask LoadSheetAsync<T>(string assetName) where T : unmanaged { }

// GOOD: Fire-and-forget은 UniTaskVoid + Forget()
private async UniTaskVoid LoadBaseDataAsync()
{
    await GeneratedBlobLoader.LoadAllDataAsync(this);
}
LoadBaseDataAsync().Forget();

// GOOD: 비동기 핸들러 래핑 시 변수 캡처 후 호출
// [HEAP] 람다 클로저 — 초기화 시 1회만 할당, 핫패스 아님
MessageHandler<T> wrapper = (in T msg) =>
{
    var captured = msg;        // struct 복사 (스택)
    handler(captured).Forget();
};

// AVOID: async void 사용 금지
private async void BadMethod() { }  // ← 금지
```

---

## 12. 인터페이스 & SOLID

### 단일 책임 원칙 (SRP)

하나의 클래스/인터페이스는 하나의 변경 이유만 가진다.  
역할이 다른 책임은 별도 클래스로 분리한다.

```csharp
// GOOD: 각 클래스가 하나의 책임만 담당
internal sealed class AesEncryptionProvider : IEncryptionProvider { }  // 암복호화만
internal sealed class EncryptedBlobDataDeserializer : IDataDeserializer { }  // 복호화 후 역직렬화 위임만
internal sealed class SceneBridgeSystem : IInitializable, IDisposable { }  // ECS 싱글톤 등록만

// BAD: 하나의 클래스가 암호화 + 직렬화 + ECS 등록을 모두 담당
internal sealed class SceneDataManager  // [VIOLATION: SRP] — 변경 이유가 3가지
{
    public void Encrypt(byte[] data) { }
    public IDataHandle<T> Deserialize<T>(byte[] data) where T : unmanaged { }
    public void RegisterToEcs() { }
}
```

### 리스코프 치환 원칙 (LSP)

파생 타입(구현체)은 기반 타입(인터페이스/추상 클래스)을 완전히 대체할 수 있어야 한다.  
구현체가 인터페이스 계약을 좁히거나 예외를 추가로 던져서는 안 된다.

```csharp
// GOOD: IDataDeserializer 계약을 완전히 충족
internal sealed class EncryptedBlobDataDeserializer : IDataDeserializer
{
    // 계약: data가 null이거나 비어 있으면 ArgumentException — 기반 계약과 동일
    public IDataHandle<T> Deserialize<T>(byte[] data) where T : unmanaged
    {
        if (data is null || data.Length == 0)
            throw new ArgumentException("Data is empty");
        // ...
    }
}

// BAD: 계약에 없는 예외를 추가하거나 반환값 보장을 좁힘
internal sealed class BadDeserializer : IDataDeserializer
{
    public IDataHandle<T> Deserialize<T>(byte[] data) where T : unmanaged
    {
        // [VIOLATION: LSP] 기반 계약에 없는 NotSupportedException 추가
        throw new NotSupportedException("Use only for Scene data");
    }
}

// BAD: 구현 타입에 직접 캐스팅 — LSP 신뢰를 깨는 패턴
if (handle is BlobDataHandle<T> blobHandle) { }  // [VIOLATION: LSP/OCP]
```

### 개방-폐쇄 원칙 (OCP)

```csharp
// GOOD: 인터페이스 메서드로 추상화 — 구현에 의존하지 않음
public interface IDataHandle<T> : IDisposable
{
    public bool TryGetData(out T data);
}

// BAD: 구현 타입에 직접 캐스팅 — 확장 시 수정 필요
if (handle is BlobDataHandle<T> blobHandle) { }  // [VIOLATION: OCP]
```

### 의존성 역전 원칙 (DIP)

```csharp
// GOOD: 인터페이스에만 의존
public class DataProvider : IDataProvider
{
    private readonly IFluxRouter _router;
    private readonly IAssetProvider _asset;
    private readonly IDataDeserializer _deser;

    public DataProvider(IFluxRouter router, IAssetProvider asset, IDataDeserializer deser)
    {
        _router = router;
        _asset  = asset;
        _deser  = deser;
    }
}
```

---

## 13. Dispose 패턴

```csharp
// GOOD: DisposableBase 상속 + SubscriptionToken 반드시 저장
public class DataProvider : IDataProvider, IDisposable
{
    private readonly IFluxRouter _router;
    private SubscriptionToken _initSubscription;

    public void Initialize()
    {
        // [HEAP] Subscribe 내부에서 핸들러 래핑 객체 1회 할당 (초기화 시점)
        _initSubscription = _router.Subscribe<FxInitializeSystem>(HandleInitialize);
    }

    public void Dispose()
    {
        _initSubscription.Dispose();
    }

    private void HandleInitialize(in FxInitializeSystem msg)
    {
        LoadBaseDataAsync().Forget();
    }

    private async UniTaskVoid LoadBaseDataAsync()
    {
        await GeneratedBlobLoader.LoadAllDataAsync(this);
    }
}

// GOOD: DisposableBase 4단계 패턴
protected override void OnDisposing() { }               // 1. 중단 처리
protected override void DisposeManagedResources() { }   // 2. 관리 리소스 해제
protected override void DisposeUnmanagedResources() { } // 3. 비관리 리소스 해제
protected override void FinalizeDispose() { }           // 4. 정리 후 로깅
```

---

## 14. DDD 패턴

```csharp
// Entity: Id로 동등성 판별
public abstract class Entity<TId> : IEntity<TId> { }

// ValueObject: 값으로 동등성 판별, readonly struct 또는 class 선택
public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();
    // [HEAP] GetEqualityComponents()는 IEnumerable 반환 — 열거자 할당 발생
    // 비교 빈도가 낮은 도메인 로직에서만 사용
}

// AggregateRoot: DomainEvent 수집 → ApplicationService에서 Dispatch
public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = new();  // [HEAP] 초기화 시 1회
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

    protected void RaiseDomainEvent(IDomainEvent e)
    {
        _domainEvents.Add(e);  // [HEAP] 이벤트 객체가 클래스인 경우
    }
}

// ApplicationService: DispatchAndClearEvents 호출 후 저장소에 반영
protected async UniTask DispatchAndClearEvents<TId>(AggregateRoot<TId> aggregate) { }
```

---

## 15. 금지 사항

| 금지 항목 | 대안 |
|-----------|------|
| `async void` | `async UniTaskVoid` + `.Forget()` |
| `Subscribe<T>()` 반환값 미저장 | `SubscriptionToken` 항상 보관 후 `Dispose` |
| 구현 클래스를 직접 필드 타입으로 선언 | 인터페이스 타입 사용 |
| 인터페이스 없는 `public` 구현 클래스 | `internal sealed` + 인터페이스 노출 |
| `string.Format` 직접 사용 | 문자열 보간 `$""` 사용 |
| GC 유발 람다 클로저 (핫패스) | 로컬 변수 먼저 캡처 후 사용, static 람다 고려 |
| `dynamic` 키워드 | 제네릭 + 인터페이스 |
| `MakeGenericType` / `MakeGenericMethod` | 사전 정의 제네릭 타입 사용 |
| 접근 제한자 생략 | 반드시 명시 (`public` / `private` / `internal` 등) |
| 인터페이스 멤버 접근 제한자 생략 | `public` 명시 |
| 복수 문장 if 중괄호 생략 | 반드시 `{ }` + 줄바꿈 |
| 핫패스 내 LINQ | `for` 루프 + 직접 인덱싱 |
| 핫패스 내 `new` 클래스 | 오브젝트 풀 (`ObjectPool<T>`) 사용 |
| 힙 할당 코드에 주석 미명시 | `// [HEAP]`, `// [BOXING]`, `// [GC PRESSURE]` 태그 필수 |
| AOT 위험 코드에 주석 미명시 | `// [AOT RISK]` 태그 필수 |
