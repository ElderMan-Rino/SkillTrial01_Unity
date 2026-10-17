# C# Coding Conventions (MSDN + Project)

Base: MSDN C# conventions. Stack: Unity + VContainer + UniTask + ECS Blob + Flux messaging.
**Core principles: Optimization · Maintainability · Clarity**

## 1. Naming
| Target | Style |
|---|---|
| Class/Interface/Struct/Method/Property/Event/Enum/const | PascalCase |
| Local/Parameter | camelCase |
| private/protected/static field | `_camelCase` |
| Interface | `I` prefix |
| Flux message struct | `Fx` prefix (`FxSceneTransition`) |
| Installer / Async method / Domain event | `*Installer` / `*Async` / `*Event` suffix |

## 2. Access Modifiers
- Always explicit — omission forbidden (interface members too: explicit `public`).
- Least privilege. Implementation classes: `internal sealed`, exposed only via interface. `public` only for cross-assembly contracts.
- Inheritance hooks: `protected virtual` / `protected override`.
- Explicit interface implementation (`void IDisposable.Dispose()`): no modifier (language spec).

## 3. Interfaces & SOLID
- **ISP**: split interfaces by role (e.g. `IEngineAssetLoader` / `IEngineAssetReleaser`), no god-interfaces.
- **SRP**: one reason to change per class.
- **LSP**: implementations must not throw exceptions outside the contract or narrow guarantees; never cast to implementation types (`handle is BlobDataHandle<T>` forbidden — also OCP violation).
- **DIP**: fields/ctor params are interface types, injected via VContainer.

## 4. Control Flow (if)
- Single statement: inline, no braces — `if (handle is null) return;`
- Multi-statement / else / else if: braces + newline (Allman) required. `else DoB();` without braces forbidden.
- Guard clauses first (early return), no deep nesting.

## 5. Files & Folders
```
{Module}/ Interfaces | App | Domain | Infra | Installer | Messages | Definitions
```
File name = class name. One public type per file. Interface file: `I{Name}.cs`.

## 6. Syntax (MSDN)
- `var` only when RHS type is obvious; keyword types (`string` not `System.String`); target-typed `new()`.
- Auto-properties / expression-bodied read-only props; `{ get; private set; }`.
- Null: `is null` / `is not null`; use `?.`.
- Strings: `$""` interpolation only, never `string.Format`/`+`. Hot path: check log level first (`if (_logger.IsErrorEnabled) ...`) or avoid string creation entirely. // [HEAP]

## 7. Layout
- Allman braces. using order: System → third-party (alphabetical) → `Elder.*`.
- Member order: static/const → private readonly → private fields → ctor → public properties → public methods → protected methods → private methods.

## 8. Comments
- Comment WHY only, never WHAT. XML doc on public API optional.
- **Mandatory tags** on any occurrence:

| Tag | When |
|---|---|
| `// [HEAP]` | new class, lambda capture, string creation, `params`, LINQ |
| `// [BOXING]` | value type → interface/object |
| `// [GC PRESSURE]` | repeated allocation in hot path/loop |
| `// [AOT RISK]` | reflection, `MakeGenericType`, deep generic nesting |

## 9. Performance / Heap
Heap sources (tag required): `new` class, closure lambdas, `params`, boxing, LINQ, string interpolation.
Avoidance: `readonly struct` messages, `in` params, `stackalloc`/`Span<T>`, `ArrayPool<T>`, `static` lambdas (C# 9), `Unsafe.As` (no boxing).
**Hot path (Update/FixedUpdate) forbidden**: `new` class, LINQ, string interpolation, boxing, closures, `params`, `foreach` on non-List/Array. Use indexed `for`.

## 10. AOT (IL2CPP)
Forbidden/tagged: `MakeGenericType`/`MakeGenericMethod`, `Activator.CreateInstance`, `dynamic`, deep generic nesting.
Safe: generic constraints (`where T : struct, IFluxMessage`), non-generic base class entry points, MessagePack/MemoryPack (codegen, not reflection).
Limits: ≤2 generic params, ≤2 nesting levels (`A<B<T>>` max), reflection → `#if UNITY_EDITOR` only.

## 11. Async
- All async methods return `UniTask`, suffix `*Async`. Fire-and-forget: `UniTaskVoid` + `.Forget()`. `async void` forbidden.
- Async handler wrappers: copy struct to local before invoking (`var captured = msg; handler(captured).Forget();`).

## 12. Dispose
- Always store `Subscribe<T>()` return as `SubscriptionToken`, dispose it in `Dispose()`.
- `DisposableBase` 4 steps: `OnDisposing` → `DisposeManagedResources` → `DisposeUnmanagedResources` → `FinalizeDispose`; only `Dispose()` is public.

## 13. DDD
- `Entity<TId>`: identity by Id. `ValueObject`: equality by value (`GetEqualityComponents` allocates enumerator — low-frequency domain logic only).
- `AggregateRoot<TId>`: collect `DomainEvents` via `RaiseDomainEvent`; ApplicationService calls `DispatchAndClearEvents` then persists.
