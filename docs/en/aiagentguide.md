---
title: AI Agent Guide
parent: EN
nav_order: 5
---

# AI Agent Guide

If you use AI coding assistants (Claude Code, Cursor, Copilot, etc.) with StaticEcs, you can provide your agent with context about the library. This page contains ready-to-use snippets.

___

## llms.txt

Point your agent at the library's AI-readable documentation:
- **Concise**: `https://felid-force-studios.github.io/StaticEcs/llms.txt`
- **Full**: `https://felid-force-studios.github.io/StaticEcs/llms-full.txt`

___

## CLAUDE.md Snippet

If you use [Claude Code](https://claude.ai/code), copy the following block into your project's `CLAUDE.md` file. This gives Claude the essential context to use StaticEcs correctly.

For other agents, paste this into their respective instruction files (`.cursorrules`, `.github/copilot-instructions.md`, etc.).

````markdown
## StaticEcs ECS Framework

This project uses [StaticEcs](https://github.com/Felid-Force-Studios/StaticEcs) — a static generic ECS framework for C#. Namespace: `FFS.Libraries.StaticEcs`.

### Setup Pattern
```csharp
public struct WT : IWorldType { }
public abstract class W : World<WT> { }           // type alias for world access
public struct GameSystems : ISystemsType { }
public abstract class GameSys : W.Systems<GameSystems> { }
```

### World Lifecycle (strict order)
1. `W.Create(WorldConfig.Default())` — creates the world
2. `W.Types().RegisterAll()` or manual registration `.Component<T>().Tag<T>().Event<T>()` — register ALL types (required!). `RegisterAll()` without arguments scans `typeof(TWorld).Assembly` (safe on IL2CPP/WebGL/NativeAOT). For types split across assemblies use `RegisterAll(typeof(TWorld).Assembly, typeof(Other).Assembly)`.
3. `W.Initialize()` — after this, entity operations are available
4. Work: create entities, run systems, iterate queries
5. `W.Destroy()` — cleanup

### Critical Rules
- ALWAYS register component/tag/event/link types between Create() and Initialize(). Use `W.Types().RegisterAll()` to auto-register all types from the assembly that declares your `TWorld` marker (works on Unity IL2CPP / WebGL / NativeAOT because it uses `typeof(TWorld).Assembly`, not `GetCallingAssembly`), or register manually. For multi-assembly projects pass each assembly explicitly: `W.Types().RegisterAll(typeof(TWorld).Assembly, typeof(OtherAssemblyMarker).Assembly)`. Unregistered types cause runtime errors.
- Entity is a 4-byte uint handle — NOT a persistent reference. NEVER store Entity in fields/collections across frames. Use EntityGID for persistent references.
- `Add<T>()` without value is idempotent (if exists → returns ref, no hooks). `Set(value)` ALWAYS overwrites with OnDelete→OnAdd hook cycle.
- `Ref<T>()` returns a ref to the component. Assumes component exists — check with `Has<T>()` first if uncertain.
- For read-only components use `Read<T>()` (returns `ref readonly`) instead of `Ref<T>()`, and `in` instead of `ref` in query delegates.
- Query filter types: `All<>` (require), `None<>` (exclude), `Any<>` (at least one). These filters work with both components and tags. Combine with `And<Filter1, Filter2>` (all must match) or `Or<Filter1, Filter2>` (any must match).
- Default query mode is Strict — do NOT modify filtered component/tag types on OTHER entities during iteration. Use `EntitiesFlexible()` if needed.
- During `ForParallel`, only modify the current entity. No structural changes.
- Systems: `ISystem` with `Init()`, `Update()`, `UpdateIsActive()`, `Destroy()`. All methods have default empty implementations.

### Common Patterns
```csharp
// Create entity with components
var entity = W.NewEntity<Default>().Set(new Position { Value = v }, new Velocity { Value = 1f });

// Query iteration (foreach)
foreach (var e in W.Query<All<Position, Velocity>>().Entities()) {
    ref var pos = ref e.Ref<Position>();
    ref readonly var vel = ref e.Read<Velocity>();
    pos.Value += vel.Value;
}

// Query iteration (delegate — faster, zero-allocation)
W.Query().For(static (ref Position p, in Velocity v) => {
    p.Value += v.Value;
});

// Persistent reference
EntityGID gid = entity.GID;
if (gid.TryUnpack<WT>(out var resolved)) { /* resolved is alive */ }

// Tags
entity.Set<IsPlayer>();
if (entity.Has<IsPlayer>()) { ... }

// Multi-components (list of same-type values on an entity)
ref var items = ref entity.Add<W.Multi<Item>>();
items.Add(new Item { Id = 1 });
items.Add(new Item { Id = 2 });
foreach (ref var item in items) { item.Weight *= 2f; }

// Relations (entity links)
entity.Set(new W.Link<Parent>(parentEntity));           // single link
ref var children = ref entity.Add<W.Links<Children>>(); // multi link
children.TryAdd(childEntity.AsLink<Children>());

// Systems
public struct MoveSystem : ISystem {
    public void Init() { /* called once on Initialize */ }
    public void Update() {
        W.Query().For(static (ref Position p, in Velocity v) => {
            p.Value += v.Value;
        });
    }
    public void Destroy() { /* called on Destroy */ }
}
GameSys.Create();
GameSys.Add(new MoveSystem(), order: 0);
GameSys.Initialize();
// In game loop: GameSys.Update();

// Resources
W.SetResource(new GameConfig { ... });
ref var config = ref W.GetResource<GameConfig>();
```

### Full documentation
- Concise AI reference: https://felid-force-studios.github.io/StaticEcs/llms.txt
- Full documentation: https://felid-force-studios.github.io/StaticEcs/en/features.html
````
