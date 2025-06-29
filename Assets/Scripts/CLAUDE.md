# C# Coding Conventions (MSDN + Project-Based)

This project follows **MSDN C# coding conventions** as a baseline, with the rules below taking priority.
Uses Unity + VContainer + UniTask + ECS Blob + Flux messaging architecture.

> **Three core principles: Optimization · Maintainability · Clarity**
> All code that causes heap allocation must include a `// [HEAP]` comment.
> All patterns with AOT (IL2CPP) code bloat risk must include a `// [AOT RISK]` comment.

---

## 1. Naming Rules

### Casing Rules (MSDN Standard)

| Target | Style | Example |
|------|------|------|
| Class / Interface / Struct | PascalCase | `FluxRouter`, `IFluxMessage` |
| Method | PascalCase | `LoadSheetAsync`, `GetData` |
| Property | PascalCase | `MainSceneName`, `DomainEvents` |
| Event | PascalCase | `OnInitialized` |
| Enum type and values | PascalCase | `LogLevel.Warning` |
| Local variable / Parameter | camelCase | `targetSceneKey`, `assetName` |
| private field | `_camelCase` (underscore prefix) | `_router`, `_dataHandles` |
| protected field | `_camelCase` (underscore prefix) | `_baseState`, `_cachedTransform` |
| static field | `_camelCase` (underscore prefix) | `_globalTokenIdCounter` |
| const | PascalCase | `BootStrapSceneKey` |
| Interface | `I` prefix | `IFluxRouter`, `IDataProvider` |

### Project-Specific Naming

| Target | Rule | Example |
|------|------|------|
| Flux message struct | `Fx` prefix | `FxInitializeSystem`, `FxSceneTransition` |
| Installer | `*Installer` suffix | `FluxInstaller`, `DataInstaller` |
| Async method | `*Async` suffix | `LoadSheetAsync`, `GetAssetAsync` |
| Domain event | `*Event` suffix or meaningful past tense | `DataLoadedEvent` |

---

## 2. Access Modifier Rules

### Core Principles

- **All members must explicitly declare an access modifier** (omission is strictly forbidden).
- **Principle of Least Privilege**: use the narrowest access modifier required.
- Prefer `internal` for implementation classes; expose only through interfaces.

### public Rules

Use only when external access (from another assembly or layer) is strictly required.

```csharp
// GOOD: interface contract that must be publicly exposed
public interface IFluxRouter
{
    void Publish<T>(in T message) where T : struct;
    SubscriptionToken Subscribe<T>(MessageHandler<T> handler) where T : struct;
}

// GOOD: Application Service exposed to DI container
public class DataProvider : IDataProvider
{
    public void Initialize() { }     // interface implementation → public required
    public void Dispose() { }        // IDisposable implementation → public required
}
```

### private Rules

All members used only within the class must be declared `private`.
Fields must follow the `_camelCase` format.

```csharp
public class DataProvider : IDataProvider, IDisposable
{
    // GOOD: private fields use _ prefix + camelCase
    private readonly IFluxRouter _router;
    private readonly IAssetProvider _asset;
    private SubscriptionToken _subscription;
    private bool _isInitialized;

    // GOOD: private methods use PascalCase (same as function rule)
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

### protected Rules

Use when a member needs to be accessible only to derived classes in an inheritance hierarchy.
Used for template method patterns via `protected virtual` / `protected override`.

```csharp
public abstract class DisposableBase : IDisposable
{
    // GOOD: hook methods to be overridden by derived classes
    protected virtual void OnDisposing() { }
    protected virtual void DisposeManagedResources() { }
    protected virtual void DisposeUnmanagedResources() { }
    protected virtual void FinalizeDispose() { }

    // GOOD: only Dispose is public
    public void Dispose()
    {
        OnDisposing();
        DisposeManagedResources();
        DisposeUnmanagedResources();
        FinalizeDispose();
    }
}
```

### internal Rules

Accessible only within the same assembly. Use as the default access modifier for implementation classes.

```csharp
// GOOD: implementation class is internal, exposed only through interface
internal sealed class FluxRouter : IFluxRouter
{
    private readonly Dictionary<Type, object> _handlerContainers = new();

    public void Publish<T>(in T message) where T : struct
    {
        if (!_handlerContainers.TryGetValue(typeof(T), out var container)) return;
        // [HEAP] Unsafe.As cast without boxing (boxing avoidance)
        Unsafe.As<MessageHandlerContainer<T>>(container).Publish(in message);
    }
}
```

### Access Modifier Priority Summary

```
public → protected → internal → protected internal → private protected → private
```

| Scenario | Recommended Modifier |
|----------|----------------|
| Exposing interface contract | `public` |
| Implementing interface member | `public` (except explicit implementation) |
| Implementation class itself | `internal sealed` |
| Hook method for inheritance | `protected virtual` |
| Class-internal field/method | `private` |
| Same-assembly utility | `internal` |

---

## 3. Interface Rules

### Explicit Access Modifiers on Interface Members

While C# interface members are implicitly `public abstract`,
**this project requires `public` to be explicitly declared on all interface members.**

```csharp
// GOOD: explicit public on interface members
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

// BAD: omitting access modifier (relying on implicit public)
public interface IDataProvider
{
    void Initialize();     // ← no access modifier, forbidden
    bool TryGetData<T>(string key, out T data) where T : unmanaged;
}
```

### Interface Design Principles (ISP)

Separate interfaces by role. Do not pack multiple responsibilities into a single interface.

```csharp
// GOOD: separated by role
public interface IEngineAssetLoader
{
    public UniTask<IAssetHandle<T>> LoadAsync<T>(string key) where T : UnityEngine.Object;
}

public interface IEngineAssetReleaser
{
    public void Release(string key);
}

// BAD: all responsibilities crammed into one interface
public interface IAssetManager   // [VIOLATION: ISP]
{
    UniTask Load();
    void Release();
    void Preload();
    void Clear();
}
```

### Explicit Interface Implementation

Use when access should only be through the interface type, and direct calls on the implementation type should be blocked.
In this case, do not add an access modifier (C# language specification).

```csharp
internal sealed class SpecialRouter : IFluxRouter, IDisposable
{
    // GOOD: explicit implementation - no access modifier (language spec)
    void IDisposable.Dispose()
    {
        CleanupResources();
    }

    // GOOD: regular implementation - explicit public
    public void Publish<T>(in T message) where T : struct { }
}
```

---

## 4. Control Flow (if) Rules

### Core Principles

- **Single-statement if**: write inline as `if (condition) statement;` — **no braces, same line**.
- **Multi-statement / block if**: must use braces + newline (Allman style).
- **else / else if**: always use braces + newline.

```csharp
// ✅ GOOD: single guard clause — inline
if (handle is null) return;
if (!_isInitialized) return;
if (!_entries.TryGetValue(key, out var entry)) return;

// ✅ GOOD: single assignment/call — inline
if (isVisible) Enable();
if (count > 0) ProcessQueue();

// ✅ GOOD: multiple statements — braces + newline required
if (isReady)
{
    LoadData();
    NotifyReady();
}

// ✅ GOOD: else if / else — braces + newline required
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

// ❌ BAD: multiple statements without braces
if (isReady)
    LoadData();
    NotifyReady();  // ← always executes! bug-prone

// ❌ BAD: single statement with unnecessary newline
if (handle is null)
    return;         // ← guard clause is clearer on one line

// ❌ BAD: else without braces
if (isReady) DoA();
else DoB();         // ← forbidden
```

### Guard Clause Pattern (Early Return First)

Use guard clauses to eliminate nested ifs by handling failure conditions first.

```csharp
// GOOD: guard clause removes nesting
public void Publish<T>(in T message) where T : struct
{
    if (!_handlerContainers.TryGetValue(typeof(T), out var container)) return;
    if (!container.HasHandlers) return;

    container.Publish(in message);
}

// BAD: deep nesting
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

## 5. File & Folder Structure

### Layered Architecture Folder Rules

```
{Module}/
  Interfaces/    - Interfaces (contract definitions)
  App/           - Application services (Use Cases)
  Domain/        - Domain models (Entity, VO, Aggregate)
  Infra/         - Infrastructure implementations (Unity API, external libraries)
  Installer/     - VContainer DI registration
  Messages/      - Flux messages (IFluxMessage implementations)
  Definitions/   - Enums, delegates, constants
```

- File name must match class name exactly
- Only one public type per file
- Interface file naming: `I{Name}.cs`

---

## 6. Language Syntax Style (MSDN Standard)

### Type Declarations

```csharp
// GOOD: use var only when type is clearly apparent
var container = new MessageHandlerContainer<T>();
MessageHandler<T> handler = GetHandler();  // explicit type when unclear

// GOOD: use keyword string instead of System.String
string name = "value";

// GOOD: use new() target-type inference
private readonly Dictionary<Type, object> _dataHandles = new();
```

### Properties

```csharp
// GOOD: read-only properties use auto-property or expression body
public string MainSceneName { get; private set; }
public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

// GOOD: use init-only properties
public TId Id { get; protected set; }
```

### Null Handling

```csharp
// GOOD: use is null / is not null for null checks (MSDN recommended)
if (handle is null) return;
if (provider is not null) provider.Load();

// GOOD: use null-conditional operator
_router?.Publish(new FxInitializeSystem());
```

### String

```csharp
// GOOD: use string interpolation
// [HEAP] string interpolation creates a new string object at runtime (use only in low-frequency paths like logs)
_logger.Error($"Failed to load: {assetName}");

// AVOID: direct string.Format calls
_logger.Error(string.Format("Failed to load: {0}", assetName));

// GOOD: avoid string creation entirely on hot paths
// (check log level before interpolating)
if (_logger.IsErrorEnabled) _logger.Error($"Failed: {assetName}");
```

---

## 7. Code Layout

### Braces (Allman Style, MSDN Default)

```csharp
// GOOD
public void Initialize()
{
    _logger = LogFacade.GetLoggerFor<DataProvider>();
    _subscription = _router.Subscribe<FxInitializeSystem>(HandleInitializeSystem);
}

// BAD: single-line braces (for complex methods)
public void Initialize() { _logger = LogFacade.GetLoggerFor<DataProvider>(); }
```

### using Sort Order

```csharp
// 1. System namespaces
using System;
using System.Collections.Generic;

// 2. Third-party (alphabetical)
using Cysharp.Threading.Tasks;
using MessagePack;
using Unity.Entities;
using UnityEngine;
using VContainer;
using VContainer.Unity;

// 3. Internal project (Elder.*)
using Elder.Framework.Data.Interfaces;
using Elder.Framework.Flux.Interfaces;
```

### Member Declaration Order

```csharp
public class Example
{
    // 1. private const / static readonly
    private static long _globalCounter;

    // 2. private readonly fields
    private readonly IFluxRouter _router;

    // 3. private fields
    private SubscriptionToken _subscription;

    // 4. Constructor
    public Example(IFluxRouter router) { }

    // 5. public properties
    public string Name { get; }

    // 6. public methods (including interface implementations)
    public void Initialize() { }

    // 7. protected methods
    protected override void DisposeManagedResources() { }

    // 8. private methods
    private void HandleMessage(in FxInitializeSystem msg) { }
}
```

---

## 8. Comment Rules

```csharp
// GOOD: comment only when WHY is non-obvious
// Unsafe.As avoids boxing from Dictionary lookup — performance optimization
var container = Unsafe.As<MessageHandlerContainer<T>>(containerBase);

// GOOD: XML doc comments on public API only (optional)
/// <summary>Must call Dispose after unsubscribing.</summary>
public SubscriptionToken Subscribe<T>(MessageHandler<T> handler) where T : struct { }

// BAD: comments that describe WHAT (the code itself should explain)
// get the handler
var handler = GetHandler();
```

### Required Performance Comment Tags

When any of the following situations arise, the corresponding tag **must** be included as a comment.

| Tag | Description | Example Situations |
|------|------|-----------|
| `// [HEAP]` | Heap allocation occurs | `new`, lambda capture, boxing, string creation, `params`, LINQ |
| `// [AOT RISK]` | IL2CPP code bloat risk | Excessive generic combinations, reflection, `MakeGenericType` |
| `// [GC PRESSURE]` | Repeated heap allocations may trigger GC | Repeated `new` in hot path, closure capture loops |
| `// [BOXING]` | Value type boxing occurs | Interface casting, storing in object, non-generic collections |

```csharp
// [HEAP] lambda with closure capture causes heap allocation — forbidden in hot path
Action callback = () => HandleComplete(result);

// [BOXING] boxing occurs when passing enum as object
object boxed = LogLevel.Warning;   // [BOXING]

// [GC PRESSURE] string interpolation in loop causes repeated heap allocation
for (int i = 0; i < 1000; i++)
{
    Log($"Item {i}");  // [GC PRESSURE] — move outside loop or use StringBuilder
}

// [AOT RISK] runtime generic instantiation causes IL2CPP code bloat
Type generic = typeof(List<>).MakeGenericType(someType);  // [AOT RISK]
```

---

## 9. Performance Optimization & Heap Allocation Prevention

### Common Patterns That Cause Heap Allocation (must include [HEAP])

```csharp
// [HEAP] class instance creation via new
var list = new List<int>();

// [HEAP] lambda capturing outer variable creates closure object on heap
int captured = 10;
Action a = () => Debug.Log(captured);  // [HEAP] closure

// [HEAP] params keyword internally allocates an array
void Log(params object[] args) { }
Log("a", "b");  // [HEAP] object[] array created on every call

// [BOXING] value type → interface/object conversion causes boxing
IComparable boxed = 42;           // [BOXING]
object obj = someEnum;             // [BOXING]

// [HEAP] LINQ internally allocates enumerator objects on heap
var filtered = list.Where(x => x > 0).ToList();  // [HEAP]
```

### Heap Allocation Avoidance Patterns

```csharp
// GOOD: use struct as value type (no heap allocation)
public readonly struct FxInitializeSystem : IFluxMessage { }

// GOOD: use in parameter to avoid struct copy cost
public void Publish<T>(in T message) where T : struct { }

// GOOD: use stackalloc for stack-based arrays (no heap allocation)
Span<int> buffer = stackalloc int[64];

// GOOD: reuse arrays with ArrayPool
var array = ArrayPool<byte>.Shared.Rent(256);
try { /* use */ }
finally { ArrayPool<byte>.Shared.Return(array); }

// GOOD: capture-free static lambda (C# 9+) — no heap allocation
private static readonly Action<int> s_log = static x => Debug.Log(x);

// GOOD: type reinterpretation via Unsafe.As without boxing
// [no HEAP] Unsafe.As only reinterprets the reference, no new object created
var container = Unsafe.As<MessageHandlerContainer<T>>(containerBase);
```

### Hot Path Checklist

The following are forbidden in Update / FixedUpdate / per-frame functions:

- `new` class instantiation (structs are allowed)
- LINQ usage
- String interpolation / string.Format
- Boxing-inducing code
- Lambda closure captures
- `params` array creation
- `foreach` on collections other than List/Array (enumerator heap allocation)

```csharp
// BAD: heap allocation every frame
private void Update()
{
    var enemies = _enemies.Where(e => e.IsAlive).ToList();  // [HEAP][GC PRESSURE]
    foreach (var e in enemies)
        e.Tick(Time.deltaTime);
}

// GOOD: update with no heap allocation
private void Update()
{
    for (int i = 0; i < _enemies.Count; i++)
    {
        if (_enemies[i].IsAlive) _enemies[i].Tick(Time.deltaTime);
    }
}
```

---

## 10. AOT (IL2CPP) Considerations

Unity IL2CPP uses Ahead-of-Time compilation, so runtime code generation is not possible.
All patterns that cause code bloat or AOT errors must include a `// [AOT RISK]` comment.

### Dangerous Patterns

```csharp
// [AOT RISK] MakeGenericType / MakeGenericMethod — may cause AOT errors in IL2CPP
Type genericType = typeof(List<>).MakeGenericType(runtimeType);

// [AOT RISK] Activator.CreateInstance<T> — reflection-based instantiation
object instance = Activator.CreateInstance(type);

// [AOT RISK] dynamic keyword — not supported in IL2CPP
dynamic value = GetValue();

// [AOT RISK] excessive generic nesting — causes code bloat
// e.g.: Handler<Wrapper<Message<Data<T>>>> generates separate code per T combination
public class Handler<T> where T : struct
{
    public Container<Wrapper<T>> GetContainer() => new();  // [AOT RISK] combination explosion
}
```

### Safe Patterns

```csharp
// GOOD: constrain generics to limit code bloat
public void Publish<T>(in T message) where T : struct, IFluxMessage { }

// GOOD: extract common logic into a non-generic base class
public abstract class HandlerContainerBase
{
    public abstract void Publish(object message);  // non-generic entry point
}

public sealed class MessageHandlerContainer<T> : HandlerContainerBase where T : struct
{
    public override void Publish(object message)
    {
        // [BOXING] object → T unboxing occurs, but only once at entry point
        Publish((T)message);
    }

    public void Publish(in T message) { /* core logic */ }
}

// GOOD: use AOT-compatible serialization like MessagePack / MemoryPack
// Instead of Newtonsoft.Json reflection-based serialization, use code-generation approach
[MessagePackObject]
public readonly struct PlayerData
{
    [Key(0)] public readonly int Id;
    [Key(1)] public readonly string Name;
}
```

### Generic Design Guide

| Situation | Recommended Approach |
|------|-----------|
| Number of generic parameters | Limit to 1–2. Reconsider design for 3+. |
| Generic nesting depth | Up to 2 levels (`A<B<T>>` maximum) |
| Generic + interface combination | Must explicitly specify constraints |
| When reflection is needed | Isolate to Editor-only code (`#if UNITY_EDITOR`) |

---

## 11. Async Patterns

```csharp
// GOOD: all async methods return UniTask
public async UniTask LoadSheetAsync<T>(string assetName) where T : unmanaged { }

// GOOD: fire-and-forget uses UniTaskVoid + Forget()
private async UniTaskVoid LoadBaseDataAsync()
{
    await GeneratedBlobLoader.LoadAllDataAsync(this);
}
LoadBaseDataAsync().Forget();

// GOOD: capture variable before calling async handler wrapper
// [HEAP] lambda closure — allocated once at initialization, not on hot path
MessageHandler<T> wrapper = (in T msg) =>
{
    var captured = msg;        // struct copy (stack)
    handler(captured).Forget();
};

// AVOID: async void is forbidden
private async void BadMethod() { }  // ← forbidden
```

---

## 12. Interfaces & SOLID

### Single Responsibility Principle (SRP)

Each class/interface should have only one reason to change.
Separate responsibilities into distinct classes.

```csharp
// GOOD: each class handles exactly one responsibility
internal sealed class AesEncryptionProvider : IEncryptionProvider { }          // encryption/decryption only
internal sealed class EncryptedBlobDataDeserializer : IDataDeserializer { }    // delegates deserialization after decrypt only
internal sealed class SceneBridgeSystem : IInitializable, IDisposable { }      // ECS singleton registration only

// BAD: one class handles encryption + serialization + ECS registration
internal sealed class SceneDataManager  // [VIOLATION: SRP] — 3 reasons to change
{
    public void Encrypt(byte[] data) { }
    public IDataHandle<T> Deserialize<T>(byte[] data) where T : unmanaged { }
    public void RegisterToEcs() { }
}
```

### Liskov Substitution Principle (LSP)

Derived types (implementations) must fully substitute their base types (interfaces/abstract classes).
Implementations must not narrow the interface contract or throw additional exceptions.

```csharp
// GOOD: fully satisfies IDataDeserializer contract
internal sealed class EncryptedBlobDataDeserializer : IDataDeserializer
{
    // contract: throws ArgumentException if data is null or empty — same as base contract
    public IDataHandle<T> Deserialize<T>(byte[] data) where T : unmanaged
    {
        if (data is null || data.Length == 0)
            throw new ArgumentException("Data is empty");
        // ...
    }
}

// BAD: adds exceptions not in contract or narrows return value guarantee
internal sealed class BadDeserializer : IDataDeserializer
{
    public IDataHandle<T> Deserialize<T>(byte[] data) where T : unmanaged
    {
        // [VIOLATION: LSP] NotSupportedException not in base contract
        throw new NotSupportedException("Use only for Scene data");
    }
}

// BAD: direct cast to implementation type — breaks LSP trust
if (handle is BlobDataHandle<T> blobHandle) { }  // [VIOLATION: LSP/OCP]
```

### Open-Closed Principle (OCP)

```csharp
// GOOD: abstracted via interface method — no dependency on implementation
public interface IDataHandle<T> : IDisposable
{
    public bool TryGetData(out T data);
}

// BAD: direct cast to implementation type — requires modification on extension
if (handle is BlobDataHandle<T> blobHandle) { }  // [VIOLATION: OCP]
```

### Dependency Inversion Principle (DIP)

```csharp
// GOOD: depend only on interfaces
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

## 13. Dispose Pattern

```csharp
// GOOD: inherit DisposableBase + always store SubscriptionToken
public class DataProvider : IDataProvider, IDisposable
{
    private readonly IFluxRouter _router;
    private SubscriptionToken _initSubscription;

    public void Initialize()
    {
        // [HEAP] Subscribe allocates handler wrapper object once (at initialization)
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

// GOOD: DisposableBase 4-step pattern
protected override void OnDisposing() { }               // 1. halt processing
protected override void DisposeManagedResources() { }   // 2. release managed resources
protected override void DisposeUnmanagedResources() { } // 3. release unmanaged resources
protected override void FinalizeDispose() { }           // 4. post-cleanup logging
```

---

## 14. DDD Patterns

```csharp
// Entity: equality determined by Id
public abstract class Entity<TId> : IEntity<TId> { }

// ValueObject: equality determined by value; choose readonly struct or class
public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();
    // [HEAP] GetEqualityComponents() returns IEnumerable — enumerator allocation occurs
    // use only in domain logic where comparison frequency is low
}

// AggregateRoot: collect DomainEvents → Dispatch in ApplicationService
public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = new();  // [HEAP] allocated once at initialization
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

    protected void RaiseDomainEvent(IDomainEvent e)
    {
        _domainEvents.Add(e);  // [HEAP] if event object is a class
    }
}

// ApplicationService: call DispatchAndClearEvents then persist to repository
protected async UniTask DispatchAndClearEvents<TId>(AggregateRoot<TId> aggregate) { }
```

---

## 15. Forbidden Patterns

| Forbidden | Alternative |
|-----------|------|
| `async void` | `async UniTaskVoid` + `.Forget()` |
| Ignoring `Subscribe<T>()` return value | Always store `SubscriptionToken` and call `Dispose` |
| Using implementation class as field type | Use interface type |
| `public` implementation class without interface | `internal sealed` + expose through interface |
| Direct `string.Format` usage | String interpolation `$""` |
| GC-inducing lambda closures (hot path) | Capture to local variable first; consider static lambda |
| `dynamic` keyword | Generics + interfaces |
| `MakeGenericType` / `MakeGenericMethod` | Use pre-defined generic types |
| Omitting access modifiers | Always explicit (`public` / `private` / `internal`, etc.) |
| Omitting access modifier on interface members | Explicit `public` |
| Omitting braces on multi-statement if | Always `{ }` + newline |
| LINQ in hot path | `for` loop + direct indexing |
| `new` class in hot path | Object pool (`ObjectPool<T>`) |
| Heap-allocating code without comment | `// [HEAP]`, `// [BOXING]`, `// [GC PRESSURE]` tags required |
| AOT-risky code without comment | `// [AOT RISK]` tag required |
