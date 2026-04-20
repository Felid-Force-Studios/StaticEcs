---
title: Common Pitfalls
parent: EN
nav_order: 4
---

# Common Pitfalls

A list of frequent mistakes when using StaticEcs. Useful for both developers and AI coding assistants.

___

## Lifecycle Errors

### Forgetting type registration
ALL component, tag, event, link, and multi-component types MUST be registered between `W.Create()` and `W.Initialize()`. Using an unregistered type causes a runtime error.
```csharp
// WRONG: component not registered
W.Create(WorldConfig.Default());
W.Initialize();
var e = W.NewEntity<Position>(0); // RuntimeError!

// CORRECT — manual registration
W.Create(WorldConfig.Default());
W.Types().Component<Position>();
W.Initialize();
var e = W.NewEntity<Position>(0); // OK

// CORRECT — auto-registration of all types from the assembly
W.Create(WorldConfig.Default());
W.Types().RegisterAll();
W.Initialize();
```

### `RegisterAll()` and multi-assembly projects / Unity IL2CPP / WebGL / NativeAOT

`W.Types().RegisterAll()` without arguments scans **exactly one assembly** — the one that declares your `IWorldType` marker (`typeof(TWorld).Assembly`). It does **not** walk the stack and does **not** enumerate loaded assemblies, which means:

- **It is safe on all runtimes**, including Unity IL2CPP, Unity WebGL and NativeAOT, where stack walking (`Assembly.GetCallingAssembly`) returns unreliable results.
- **It will miss ECS types defined in other assemblies.** A common mistake is to keep your `TWorld` marker struct in a "core" / "shared" assembly and your components in a gameplay assembly — the parameterless call then registers nothing.

```csharp
// WRONG — MyWorld lives in Game.Core.dll, components live in Game.Gameplay.dll.
// Only Game.Core.dll is scanned, so no components get registered.
W.Types().RegisterAll();

// CORRECT — list every assembly that contains ECS types.
W.Types().RegisterAll(
    typeof(MyWorld).Assembly,
    typeof(Position).Assembly,
    typeof(AiPlugin).Assembly
);
```

If in doubt, place your `TWorld` marker in the same assembly as your components and use the parameterless form.

### Entity operations before Initialize
`NewEntity`, queries, and all entity operations only work after `W.Initialize()`. Calling them during the `Created` phase (between `Create` and `Initialize`) will fail.

### Calling Create twice
Calling `W.Create()` without `W.Destroy()` first is an error. The world must be destroyed before re-creating.

___

## Entity Handle Errors

### Using Entity after Destroy
`Entity` is a 4-byte uint slot handle with no generation counter. After `Destroy()`, the slot is immediately available for reuse. The old handle now silently points to a completely different entity — or to garbage.
```csharp
var entity = W.NewEntity<Position>(0);
entity.Destroy();
// entity is now INVALID — any use is undefined behavior
entity.Ref<Position>(); // DANGER: may access a different entity's data
```

### Storing Entity across frames
Since Entity has no generation counter, it cannot detect staleness. Never store `Entity` in fields, lists, or other persistent structures. Use `EntityGID` instead.
```csharp
// WRONG
class MySystem { Entity targetEntity; } // Stale after target is destroyed

// CORRECT
class MySystem { EntityGID targetGid; } // Safe — version check detects staleness
// Usage:
if (targetGid.TryUnpack<WT>(out var entity)) {
    // entity is valid and alive
}
```

### Comparing Entity for identity
`Entity` equality is by IdWithOffset (uint) only. Two entities created at different times in the same slot have the same Entity value. Use `EntityGID` for identity comparison.

___

## Component Errors

### Add vs Set semantics
`Add<T>()` without a value is **idempotent** — if the component already exists, it returns a ref to the existing data with NO hooks called. This is NOT an overwrite.

`Set(value)` **always overwrites** — calls OnDelete on old value, overwrites data, calls OnAdd on new value.

```csharp
entity.Set(new Position { Value = Vector3.Zero }); // Sets position
entity.Add<Position>(); // Does NOTHING — returns ref to existing {0,0,0}
entity.Set(new Position { Value = Vector3.One }); // Overwrites: OnDelete(old) → set → OnAdd(new)
```

### Implementing empty hook methods
`ComponentTypeInfo<T>` uses reflection at startup to detect which hooks are implemented. If any hook has a non-empty body, hook dispatch is enabled for ALL instances of that component type. Don't override hooks with empty bodies.
```csharp
// WRONG: empty hook body still causes hook dispatch overhead
public struct Foo : IComponent {
    public void OnAdd<TW>(World<TW>.Entity self) where TW : struct, IWorldType { }
}

// CORRECT: don't implement hooks you don't need (default interface methods are already empty)
public struct Foo : IComponent { }
```

### HasOnDelete vs DataLifecycle
OnDelete hook and DataLifecycle (reset to `DefaultValue`) are mutually exclusive cleanup paths. If a component has an OnDelete hook, the hook handles cleanup — the data is NOT reset. DataLifecycle reset only applies to components without OnDelete. When `noDataLifecycle: true` in config, no initialization or cleanup is performed by the framework.

___

## Query Errors

### Strict mode violations
In the default Strict query mode, modifying filtered component/tag types on OTHER entities during iteration is forbidden. This includes Add/Delete/Enable/Disable of filtered types on entities other than the current one.
```csharp
// WRONG in Strict mode:
foreach (var e in W.Query<All<Position>>().Entities()) {
    otherEntity.Delete<Position>(); // Modifies filtered type on another entity!
}

// CORRECT: use Flexible mode
foreach (var e in W.Query<All<Position>>().EntitiesFlexible()) {
    otherEntity.Delete<Position>(); // OK in Flexible mode
}
```

### Parallel iteration constraints
During `ForParallel`, only modify the CURRENT entity's data. Do not create/destroy entities, modify other entities, or perform structural changes.

### Unnecessary Flexible mode
Flexible mode re-checks bitmasks on every entity, making it significantly slower than Strict. Only use Flexible when you actually need to modify filtered types on other entities during iteration.

___

## Registration Errors

### MultiComponent without Multi wrapper
`IMultiComponent` types must be registered via `W.Types().Multi<Item>()`, not as regular components. They are stored internally as `Multi<Item>` which is the actual component.

### Missing serialization setup
Serialization requires:
1. FFS.StaticPack dependency
2. All types get auto-computed GUIDs. Override via `new ComponentTypeConfig<T>(guid: ...)` for stability across type renames
3. Non-unmanaged components need `Write`/`Read` hook implementations
4. Serialization strategy is auto-detected (`UnmanagedPackArrayStrategy<T>` for unmanaged types, `StructPackArrayStrategy<T>` otherwise)

___

## Resource Errors

### NamedResource caching issue
`NamedResource<T>` caches its internal box reference on first access. If stored as `readonly` or passed by value after first use, the cache copy becomes stale.
```csharp
// WRONG
readonly NamedResource<Config> config = new("main"); // readonly breaks cache

// CORRECT
NamedResource<Config> config = new("main"); // mutable — cache works
```
