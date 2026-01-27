# StaticEcs 2.0.0 — What's New

**StaticEcs 2.0.0** is not an incremental update but a complete restructuring of the framework. The data storage model has been redesigned, APIs unified, and fundamentally new capabilities added — from a change tracking system to block iteration with direct pointers. If 1.x was a fast and convenient ECS, then 2.0 is a mature framework ready for large-scale projects: networked games, open-world streaming, reactive UIs, and simulations with millions of entities.

Below is an overview of key changes, starting with the major new features and ending with API details.

---

## Major New Features

### Segment-Based Storage Model

In 1.x the world was divided into chunks (4096 entities) and blocks (64 entities). In 2.0 an intermediate level has been added — the **segment** (256 entities). The new hierarchy:

```
Chunk (4096) → Segment (256) → Block (64) → Entity
```

Segments became the unit of memory allocation for components. This enabled two-dimensional partitioning **EntityType × Cluster** — entities of the same type within a cluster are placed in adjacent segments, which radically improves cache locality during iteration.

---

### Entity Types (IEntityType)

One of the most significant architectural additions. In 1.x all entities were the same — they differed only in their component sets. In 2.0 each entity receives a **type** at creation — a logical category that determines its purpose and memory placement.

```csharp
public struct Bullet : IEntityType {
    public static readonly byte Id = 1;
}

public struct Enemy : IEntityType {
    public static readonly byte Id = 2;
}

// Creation — type is specified explicitly
var bullet = W.NewEntity<Bullet>();
var enemy = W.NewEntity<Enemy>();
```

Entity types provide:

- **Cache locality** — entities of the same type are stored in adjacent memory segments. When a query iterates over units, it traverses densely packed data without jumping over projectiles and effects.
- **Query filtering** — new filters `EntityIs<T>`, `EntityIsNot<T>`, `EntityIsAny<T>` allow restricting a query to a specific type:
  ```csharp
  foreach (var entity in W.Query<All<Position>, EntityIs<Bullet>>().Entities()) { ... }
  ```
- **Lifecycle hooks** — `OnCreate` and `OnDestroy` are defined directly in the type struct:
  ```csharp
  public struct Bullet : IEntityType {
      public static readonly byte Id = 1;

      public void OnCreate<TWorld>(World<TWorld>.Entity entity)
          where TWorld : struct, IWorldType {
          entity.Set(new Velocity { Speed = 100 });
          entity.SetTag<Active>();
      }
  }
  ```
- **Parameterized creation** — since `IEntityType` is a struct, the type can contain data passed at creation:
  ```csharp
  public struct Flora : IEntityType {
      public static readonly byte Id = 4;
      public enum Kind : byte { Grass, Bush, Tree }
      public Kind FloraKind;

      public void OnCreate<TWorld>(World<TWorld>.Entity entity)
          where TWorld : struct, IWorldType {
          entity.Set(new Health { Value = FloraKind == Kind.Tree ? 100 : 10 });
      }
  }

  var tree = W.NewEntity(new Flora { FloraKind = Flora.Kind.Tree });
  ```

The built-in type `Default` (Id = 0) is registered automatically and used as the default type.

---

### Change Tracking System

An entirely new subsystem absent from 1.x. Allows tracking addition, deletion, and modification of components and tags without manually maintaining dirty flags.

Four tracking types:

| Type | What it tracks | Scope |
|------|---------------|-------|
| **Added** | Component/tag addition | Components, tags |
| **Deleted** | Component/tag deletion | Components, tags |
| **Changed** | Data accessed via `Mut<T>()` | Components only |
| **Created** | Entity creation | Entire world |

Tracking is enabled during type registration:

```csharp
W.Types().Component<Health>(new ComponentTypeConfig<Health>(
    trackAdded: true,
    trackDeleted: true,
    trackChanged: true
));
```

Tracking is versioned per world tick via a ring buffer:

```csharp
// Buffer size defaults to 8 — configurable via WorldConfig
W.Create(new WorldConfig { TrackingBufferSize = 16 });
```

And used through new query filters:

```csharp
// Entities that had Position added in the previous frame
foreach (var entity in W.Query<All<Position>, AllAdded<Position>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
}

// Process only changed positions (for network synchronization)
foreach (var entity in W.Query<All<Position>, AllChanged<Position>>().Entities()) {
    ref readonly var pos = ref entity.Read<Position>();
    SendPositionUpdate(entity, pos);
}

// Entities created in the previous frame
foreach (var entity in W.Query<Created, EntityIs<Bullet>, All<Position>>().Entities()) {
    // initialize visuals
}
```

**Tracking architecture:**
- Bitmap storage: one `ulong` per 64 entities — same format as component masks
- Tick-based ring buffer: `W.Tick()` advances world tick, each system in `W.Systems<T>.Update()` automatically sees changes since its last execution
- Changed tracking: `Mut<T>()` marks Changed, `Ref<T>()` does NOT (fast access without tracking)
- In delegate queries `ref` marks Changed, `in` does not
- Filters and `HasAdded/HasDeleted/HasChanged` methods accept optional `fromTick` parameter for custom tick range
- `ClearTracking()` clears all buffer slots — normally not needed, tracking managed automatically
- Zero overhead for types with tracking disabled
- `FFS_ECS_DISABLE_CHANGED_TRACKING` removes all Changed tracking code paths at compile time

**Game loop:**
```csharp
W.Systems<Update>.Update();    // each system sees changes since its LastTick
W.Tick();                      // one tick per frame
```

Full set of **16 tracking filters**: `AllAdded`, `AnyAdded`, `NoneAdded`, `AllDeleted`, `AnyDeleted`, `NoneDeleted`, `AllChanged`, `AnyChanged`, `NoneChanged`, `TagAllAdded`, `TagAnyAdded`, `TagNoneAdded`, `TagAllDeleted`, `TagAnyDeleted`, `TagNoneDeleted`, `Created`.

---

### Block Iteration (ForBlock)

A new fastest iteration method for `unmanaged` components. Instead of getting a `ref` to each component of each entity, ForBlock provides wrappers `Block<T>` and `BlockR<T>` — direct pointers to data arrays of a block of up to 64 entities at a time.

```csharp
readonly struct MoveBlock : W.IQueryBlock.Write<Position>.Read<Velocity> {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(uint count, EntityBlock entities,
                       Block<Position> positions, BlockR<Velocity> velocities) {
        for (uint i = 0; i < count; i++) {
            positions[i].Value += velocities[i].Value;
        }
    }
}

W.Query().WriteBlock<Position>().Read<Velocity>().For<MoveBlock>();
```

Supports parallel processing:
```csharp
W.Query().WriteBlock<Position, Velocity>().ForParallel<MoveBlock>(minEntitiesPerThread: 50000);
```

This is the most performant way to process data — minimal indirection, direct pointers, optimal use of SIMD instructions.

---

### Fluent Builder API for Queries

Struct functions (`IQuery`/`IQueryBlock`) received a powerful fluent builder API with explicit separation of writable and readonly components:

```csharp
// Writable + readonly via chaining
W.Query().Write<Position>().Read<Velocity>().For<ApplyVelocity>();

// All writable
W.Query().Write<Position, Velocity>().For<MoveFunction>();

// All readonly
W.Query().Read<Position, Velocity>().For<PrintPositions>();

// Block versions
W.Query().WriteBlock<Position>().Read<Velocity>().For<MoveBlock>();
```

Interface types are nested for type-level access control:
- `IQuery.Write<T0>.Read<T1>` — first parameters are writable (`ref`), the rest are read-only (`in`)
- `IQueryBlock.Write<T0>.Read<T1>` — first parameters as `Block<T>`, the rest as `BlockR<T>`

The ISystem can now simultaneously implement IQuery:
```csharp
public struct MoveSystem : ISystem, W.IQuery.Write<Position>.Read<Velocity> {
    private float _speed;

    public void Update() {
        _speed = W.GetResource<GameConfig>().Speed;
        W.Query<TagAll<Unit>>().Write<Position>().Read<Velocity>().For(ref this);
    }

    public void Invoke(W.Entity entity, ref Position pos, in Velocity vel) {
        pos.Value += vel.Value * _speed;
    }
}
```

---

### Or-Filters in Queries

In 1.x filters supported only And-combinations: all conditions had to match. In 2.0 `Or<>` has been added, enabling queries that were previously impossible:

```csharp
// Melee OR ranged fighters — different component sets
Or<All<MeleeWeapon, Damage>, All<RangedWeapon, Ammo>> fighters = default;

// Rebuild index on any position change
Or<AllAdded<Position>, AllDeleted<Position>, AllChanged<Position>> spatialChanged = default;

// Nesting — arbitrarily complex logic
And<All<Visible>, Or<TagAll<Unit, Alive>, TagAll<Effect, Active>>> visibleAlive = default;
```

---

## API Unification and Simplification

### Unified ISystem Instead of Separate Interfaces

In 1.x a system could implement combinations of `IInitSystem`, `IUpdateSystem`, `IDestroySystem`, `ISystemCondition`, `ISystemState`. In 2.0 everything is combined into a single `ISystem` interface with four optional methods:

```csharp
// Before (v1.2.x):
struct MoveSystem : IUpdateSystem {
    public void Update() { }
}
struct InitSystem : IInitSystem, IDestroySystem {
    public void Init() { }
    public void Destroy() { }
}
Systems.AddUpdate(new MoveSystem());
Systems.AddCallOnce(new InitSystem());

// After (v2.0.0):
struct MoveSystem : ISystem {
    public void Update() { }
}
struct InitSystem : ISystem {
    public void Init() { }
    public void Destroy() { }
}
GameSys.Add(new MoveSystem(), order: 0);
GameSys.Add(new InitSystem(), order: -10);
```

Unimplemented methods are detected via reflection and never called — zero overhead. The `UpdateIsActive()` method replaces `ISystemCondition`:

```csharp
public struct PausableSystem : ISystem {
    public void Update() { /* ... */ }
    public bool UpdateIsActive() => !W.GetResource<GameState>().IsPaused;
}
```

System registration now uses a fluent API with explicit ordering:
```csharp
GameSys.Add(new InputSystem(), order: -10)
    .Add(new MoveSystem(), order: 0)
    .Add(new RenderSystem(), order: 10);
```

---

### Unified Query Instead of Split API

In 1.x entity and component iteration was split between `Query.Entities<>()` and `Query.For()`:

```csharp
// Before:
foreach (var entity in W.Query.Entities<All<Position>>()) { }
W.Query.For((ref Position pos) => { pos.X += 1; });

// After:
foreach (var entity in W.Query<All<Position>>().Entities()) { }
W.Query().For(static (ref Position pos) => { pos.X += 1; });
```

A single entry point `W.Query<>()` — cleaner, simpler, no duplication.

---

### Hooks in IComponent via Default Interface Methods

In 1.x every component with hooks required a separate Config class — a cumbersome pair of structs:

```csharp
// Before (v1.2.x):
struct Position : IComponent { public float X, Y; }

class PositionConfig : IComponentConfig<Position, WT> {
    public OnComponentHandler<Position> OnAdd() => (ref Position c, Entity e) => { };
    public OnComponentHandler<Position> OnDelete() => ...;
    public Guid Id() => ...;
    public BinaryWriter<Position> Writer() => ...;
    public BinaryReader<Position> Reader() => ...;
    // ...5+ more methods
}
W.RegisterComponentType<Position>(new PositionConfig());
```

In 2.0 hooks are declared directly in the component struct:

```csharp
// After (v2.0.0):
struct Position : IComponent {
    public float X, Y;

    public void OnAdd<TWorld>(World<TWorld>.Entity self)
        where TWorld : struct, IWorldType { }

    public void OnDelete<TWorld>(World<TWorld>.Entity self, HookReason reason)
        where TWorld : struct, IWorldType { }

    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType {
        writer.WriteFloat(X); writer.WriteFloat(Y);
    }

    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self,
                             byte version, bool disabled)
        where TWorld : struct, IWorldType {
        X = reader.ReadFloat(); Y = reader.ReadFloat();
    }
}

W.Types().Component<Position>(new ComponentTypeConfig<Position>(
    guid: new Guid("...")
));
```

Component configuration is significantly simplified — `ComponentTypeConfig<T>` contains only metadata (guid, version, strategy, defaultValue, tracking flags), all behavioral logic is in the component itself.

Bonus — the config can be declared as a static field directly in the struct, and `RegisterAll()` will pick it up automatically:
```csharp
public struct Health : IComponent {
    public float Value;
    public static readonly ComponentTypeConfig<Health> Config = new(
        defaultValue: new Health { Value = 100f }
    );
}
```

**Removed:** `IComponentConfig<T,W>`, `DefaultComponentConfig<T,W>`, `ValueComponentConfig<T,W>`, `OnComponentHandler<T>`, `OnCopyHandler<T>`, `OnPut` hook.

---

### Context Renamed to Resources

The global data storage mechanism received a clearer name and an extended API:

```csharp
// Before:
W.Context.Set<GameTime>(new GameTime());
ref var time = ref W.Context.Get<GameTime>();

// After:
W.SetResource(new GameTime());
ref var time = ref W.GetResource<GameTime>();
bool has = W.HasResource<GameTime>();
W.RemoveResource<GameTime>();
```

New feature — **named resources** for storing multiple instances of the same type:
```csharp
W.SetResource("player_config", new GameConfig { Gravity = 9.81f });
W.SetResource("moon_config", new GameConfig { Gravity = 1.62f });
ref var cfg = ref W.GetResource<GameConfig>("moon_config");
```

`NamedResource<T>` — a struct handle with caching:
```csharp
var moonConfig = new W.NamedResource<GameConfig>("moon_config");
ref var cfg = ref moonConfig.Value; // first call does a dictionary lookup, subsequent calls — O(1)
```

---

### Redesigned Relations System

The entity relationship system has become simpler and more uniform:

```csharp
// Before (v1.2.x):
struct Parent : IEntityLinkComponent<Parent> {
    public EntityGID Link;
    ref EntityGID IRefProvider<Parent, EntityGID>.RefValue(ref Parent c) => ref c.Link;
}
W.RegisterToOneRelationType<Parent>(config);
entity.SetLink<Parent>(targetGID);

// After (v2.0.0):
struct ParentLink : ILinkType {
    public void OnAdd<TW>(World<TW>.Entity self, EntityGID link) where TW : struct, IWorldType { }
    public void OnDelete<TW>(World<TW>.Entity self, EntityGID link, HookReason reason) where TW : struct, IWorldType { }
}
W.Types().Link<ParentLink>();       // one-to-one relation
W.Types().Links<ParentLink>();      // one-to-many relation

entity.Set(new W.Link<ParentLink>(parentEntity));
ref var links = ref entity.Ref<W.Links<ChildLink>>();
```

The wrapper types `Link<T>` and `Links<T>` are standard components. Relations work as regular components in queries, with `OnAdd`/`OnDelete` hooks directly on `ILinkType`.

---

### Tags and Components Unification

Tags and components are now unified into a single storage system. Tags are stored in `Components<T>` with an `IsTag` flag — no separate infrastructure. This simplifies the API and reduces the codebase by ~7,500 lines.

**What changed:**
- Filters `TagAll<>`, `TagNone<>`, `TagAny<>` removed — use `All<>`, `None<>`, `Any<>` for tags
- Entity methods: `SetTag` → `Set`, `HasTag` → `Has`, `DeleteTag` → `Delete`, `ToggleTag` → `Toggle`, `ApplyTag` → `Apply`
- Copy/Move: `CopyTagsTo` → `CopyTo`, `MoveTagsTo` → `MoveTo`, `CopyComponentsTo` → `CopyTo`, `MoveComponentsTo` → `MoveTo`
- Tracking filters: `TagAllAdded` → `AllAdded`, `TagAnyAdded` → `AnyAdded`, etc. — same filters for components and tags
- Entity checks: `HasAddedTag` → `HasAdded`, `HasDeletedTag` → `HasDeleted`
- Clearing: `ClearTagTracking` → `ClearTracking`, `ClearAllTagsTracking` → `ClearAllTracking`
- Batch: `BatchSetTag` → `BatchSet`, `BatchDeleteTag` → `BatchDelete`, `BatchToggleTag` → `BatchToggle`, `BatchApplyTag` → `BatchApply`
- `TagsHandle` removed → `ComponentsHandle` (with `IsTag` field)
- `WorldConfig.BaseTagTypesCount` removed — tags count towards `BaseComponentTypesCount`

---

## Full-Fledged Batch Operations

In 1.x methods like `AddForAll`, `DeleteForAll`, `SetTagForAll`, `DestroyAllEntities` existed more as syntactic sugar — internally they operated per-entity, processing each entity individually. In 2.0 this is a **fundamentally different mechanism**: batch operations work at the block level, directly with bitmasks. In the best case a single bitwise operation modifies 64 entities at once — this is orders of magnitude faster than per-entity processing and the fastest way to mass-modify entities in the world.

The set of operations has grown to a full spectrum:

| Method | Description |
|--------|-------------|
| `BatchAdd<T>()` | Add components (default, 1-5 types) |
| `BatchSet<T>(value)` | Add components with values (1-5 types) |
| `BatchDelete<T>()` | Delete components (1-5 types) |
| `BatchEnable<T>()` | Enable components (1-5 types) |
| `BatchDisable<T>()` | Disable components (1-5 types) |
| `BatchSetTag<T>()` | Set tags (1-5 types) |
| `BatchDeleteTag<T>()` | Delete tags (1-5 types) |
| `BatchToggleTag<T>()` | **New:** Toggle tags (1-5 types) |
| `BatchApplyTag<T>(bool)` | **New:** Conditional set/unset (1-5 types) |
| `BatchDestroy()` | Destroy entities |
| `BatchUnload()` | **New:** Unload entities |
| `EntitiesCount()` | Count quantity |

Chaining support:
```csharp
W.Query<All<Position>>()
    .BatchSet(new Velocity { Value = Vector3.One })
    .BatchSetTag<IsMovable>()
    .BatchDisable<Position>();
```

`UnloadCluster()`/`UnloadChunk()` have been removed — replaced by flexible `BatchUnload()` with filtering:
```csharp
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { clusterId };
W.Query().BatchUnload(EntityStatusType.Any, clusters: clusters);
```

---

## Semantic and API Changes

### Add/Set — New Clear Semantics

In 1.x there were three methods with non-obvious differences: `Add` (assert), `TryAdd` (idempotent), `Put` (overwrite). In 2.0 two remain with clearly separated semantics:

| Method | Behavior |
|--------|----------|
| `Add<T>()` | **Idempotent** (formerly `TryAdd`). If the component exists — returns ref, hooks are NOT called. If not — default-initializes + `OnAdd`. |
| `Set(value)` | **Always overwrites** (formerly `Put`, but now with hooks). If the component exists — `OnDelete(old)` → replace → `OnAdd(new)`. If not — set → `OnAdd`. |

```csharp
entity.Set(new Position { Value = Vector3.Zero }); // sets
entity.Add<Position>();                             // does nothing — returns ref
entity.Set(new Position { Value = Vector3.One });   // OnDelete(old) → replace → OnAdd(new)
```

### Delete/Disable/Enable — Predictable Returns

```csharp
// Before:
entity.Delete<C>();              // void, asserts if missing
bool ok = entity.TryDelete<C>(); // bool

// After:
bool deleted = entity.Delete<C>();           // bool (formerly TryDelete)
ToggleResult = entity.Disable<C>();          // MissingComponent, Unchanged, Changed
ToggleResult = entity.Enable<C>();           // MissingComponent, Unchanged, Changed
```

### Methods → Properties

All parameterless getter methods became properties:

```csharp
// Entity:
entity.GID           // was: entity.Gid()
entity.IsDestroyed   // was: entity.IsDestroyed()
entity.IsDisabled    // was: entity.IsDisabled()
entity.Version       // was: entity.Version()
entity.ClusterId     // was: entity.ClusterId()
entity.ChunkID       // was: entity.Chunk()
entity.EntityType    // NEW: byte — entity type ID

// World:
W.IsWorldInitialized // was: W.IsInitialized()
W.IsIndependent      // was: W.IsIndependent()
W.Status             // NEW: WorldStatus enum
```

### Simplified Presence Checks

```csharp
// Components:
entity.Has<C>()          // was: entity.HasAllOf<C>()
entity.Has<C1, C2>()     // was: entity.HasAllOf<C1, C2>()
entity.HasAny<C1, C2>()  // was: entity.HasAnyOf<C1, C2>()
entity.HasDisabled<C>()  // was: entity.HasDisabledAllOf<C>()

// Tags:
entity.HasTag<T>()          // was: entity.HasAllOfTags<T>()
entity.HasAnyTags<T1, T2>() // was: entity.HasAnyOfTags<T1, T2>()
```

### Pool Access

```csharp
Components<T>.Instance  // was: Components<T>.Value
Tags<T>.Instance        // was: Tags<T>.Value
```

---

## Type Registration

### Unified Registrar W.Types()

In 1.x each type was registered with its own method:
```csharp
W.RegisterComponentType<Position>(new PositionConfig());
W.RegisterToOneRelationType<Parent>(config);
W.RegisterMultiComponentType<Items, int>(4);
```

In 2.0 everything goes through fluent `W.Types()`:
```csharp
W.Types()
    .Component<Position>()
    .Component<Health>(new ComponentTypeConfig<Health>(defaultValue: new Health { Value = 100 }))
    .Tag<Unit>()
    .Tag<Poisoned>(new TagTypeConfig<Poisoned>(trackAdded: true))
    .Event<DamageEvent>(new EventTypeConfig<DamageEvent>(guid: new Guid("...")))
    .Link<ParentLink>()
    .Links<ChildrenLinks>()
    .Multi<Item>()
    .EntityType<Bullet>(Bullet.Id)
    .EntityType<Enemy>(Enemy.Id);
```

`RegisterAll()` is still available for automatic discovery of all types in an assembly.

---

## Entity Creation

The creation API has been moved from `Entity.New()` to the world level and gained type parameterization:

```csharp
// Before:
var entity = W.Entity.New();
var entity = W.Entity.New<Position>(new Position());
W.Entity.NewOnes(count, onCreate);
bool ok = W.Entity.TryNew(out entity);

// After:
var entity = W.NewEntity<Default>();
var entity = W.NewEntity<Bullet>(clusterId: 5);
W.NewEntities<Default>(count: 100, onCreate: null);
bool ok = W.TryNewEntity<Default>(out entity);
var entity = W.NewEntity<Default>().Set(
    new Position { Value = Vector3.One },
    new Velocity { Value = 1f }
);
```

---

## Updated Performance and Documentation

### New Performance Page

The performance documentation has been completely rewritten. It now includes:

1. **Architectural analysis** — detailed comparison with archetype ECS (Unity DOTS, Flecs, Bevy) and sparse-set ECS (EnTT, DefaultEcs) across every aspect
2. **Iteration method hierarchy** — from fastest to most convenient:
   - `ForBlock` — pointers to blocks (fastest for unmanaged)
   - `For` with IQuery struct (zero allocations, with state)
   - `For` with delegate (zero allocations with static lambdas)
   - `foreach` (most flexible)
3. **Stripping/trimming** — recommendations for reducing assembly size
4. **Two-dimensional partitioning** — explanation of EntityType × Cluster for cache locality

### New "Common Pitfalls" Page (pitfalls.md)

An entirely new documentation section systematizing typical mistakes:

- **Lifecycle errors** — forgotten type registration, operations before Initialize
- **Entity errors** — usage after Destroy, storing across frames
- **Component errors** — Add vs Set semantics, empty hooks, HasOnDelete vs DataLifecycle
- **Query errors** — violating Strict mode, unnecessary Flexible
- **Registration errors** — MultiComponent without wrapper, missing serialization
- **Resource errors** — caching NamedResource in readonly fields

### AI Agent Guide (aiagentguide.md)

An entirely new section — a snippet for CLAUDE.md and other AI assistants featuring:
- World and systems setup patterns
- Strict lifecycle ordering
- Critical rules for working with Entity
- Standard code patterns
- Links to llms.txt

---

## Events

The event system has been moved from the `W.Events` level to the world level and gained hooks in IEvent:

```csharp
// Before:
W.Events.Send(new DamageEvent { Amount = 10 });
var receiver = W.Events.RegisterEventReceiver<DamageEvent>();
W.Events.DeleteEventReceiver(ref receiver);

// After:
W.SendEvent(new DamageEvent { Amount = 10 });
var receiver = W.RegisterEventReceiver<DamageEvent>();
W.DeleteEventReceiver(ref receiver);
```

`IEventConfig<T,W>` has been removed — configuration via `EventTypeConfig<T>`, Write/Read hooks directly in IEvent:
```csharp
struct DamageEvent : IEvent {
    public int Amount;
    public void Write(ref BinaryPackWriter writer) { writer.WriteInt(Amount); }
    public void Read(ref BinaryPackReader reader, byte version) { Amount = reader.ReadInt(); }
}
```

---

## Multi-Components

The interface has been simplified — `IMultiComponent<T, V>` became the marker `IMultiComponent`:

```csharp
// Before:
struct Items : IMultiComponent<Items, int> {
    public Multi<int> Values;
    public ref Multi<int> RefValue(ref Items c) => ref c.Values;
}
W.RegisterMultiComponentType<Items, int>(4);

// After:
struct Items : IMultiComponent {
    public Multi<int> Values;
}
W.Types().Multi<Item>();
```

API changes: `Count` → `Length`, methods `IsEmpty()`/`IsFull()` → properties.

---

## Serialization

Write/Read hooks have been moved from Config classes directly into IComponent/IEvent. This is the main architectural change — all behavioral logic of a component is now in one place.

```csharp
// Before — separate Config class:
class PositionConfig : DefaultComponentConfig<Position, WT> {
    public override BinaryWriter<Position> Writer() => ...;
    public override BinaryReader<Position> Reader() => ...;
}

// After — hooks in the struct:
struct Position : IComponent {
    public float X, Y;
    public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self)
        where TWorld : struct, IWorldType { writer.WriteFloat(X); writer.WriteFloat(Y); }
    public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self,
                             byte version, bool disabled)
        where TWorld : struct, IWorldType { X = reader.ReadFloat(); Y = reader.ReadFloat(); }
}
```

Event snapshots have been moved:
```csharp
// Before:
W.Events.CreateSnapshot();
W.Events.LoadSnapshot(snapshot);

// After:
W.Serializer.CreateEventsSnapshot();
W.Serializer.LoadEventsSnapshot(snapshot);
```

---

## Readonly Component Access

In 1.x all components in queries were `ref`. In 2.0 there is a clear distinction:

```csharp
// Delegates — ref (write) vs in (read)
W.Query().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;  // Position is writable, Velocity is read-only
});

// Outside queries
ref var pos = ref entity.Ref<Position>();           // mutable, does NOT mark Changed (fast path)
ref var tracked = ref entity.Mut<Position>();       // mutable, marks Changed
ref readonly var vel = ref entity.Read<Velocity>(); // readonly, does NOT mark Changed
```

This is integrated with the change tracking system — `Ref`/`Read` do not trigger false Changed positives, `Mut` explicitly marks the component as changed.

---

## WorldConfig — New Parameters

```csharp
new WorldConfig {
    // Existing:
    BaseComponentTypesCount = 64,
    BaseTagTypesCount = 64,
    ParallelQueryType = ParallelQueryType.Disabled,
    Independent = true,

    // New in 2.0:
    WorkerSpinCount = 256,        // spin-wait iterations before thread blocking
    BaseClustersCapacity = 16,    // initial capacity of the clusters array
    TrackCreated = false,         // global entity creation tracking
}

// Factory methods:
WorldConfig.Default()     // standard settings
WorldConfig.MaxThreads()  // all available threads
```

---

## New Compiler Directive

`FFS_ECS_DISABLE_CHANGED_TRACKING` — removes at compile time all Changed tracking code paths, including `AllChanged`, `NoneChanged`, `AnyChanged` filters and the `Mut<T>()` method.

---

## Removed APIs

| Removed | Replacement |
|---------|-------------|
| `IWorld` interface | `WorldHandle` |
| `WorldWrapper<W>` | `WorldHandle` |
| `Worlds` static class | — |
| `BoxedEntity<W>` / `IEntity` / `entity.Box()` | — |
| `entity.TryAdd<C>()` | `entity.Add<C>()` |
| `entity.Put<C>(val)` | `entity.Set<C>(val)` |
| `entity.TryDelete<C>()` | `entity.Delete<C>()` |
| All `Raw` Entity methods | — |
| `Entity.New(...)` | `W.NewEntity<TEntityType>(...)` |
| `IInitSystem` / `IUpdateSystem` / `IDestroySystem` | `ISystem` |
| `ISystemCondition` / `ISystemState` | `ISystem.UpdateIsActive()` |
| `Systems.AddUpdate()` / `AddCallOnce()` | `Sys.Add(system, order)` |
| `IComponentConfig<T,W>` | `ComponentTypeConfig<T>` + hooks in IComponent |
| `IEventConfig<T,W>` | `EventTypeConfig<T>` + hooks in IEvent |
| `IEntityLinkComponent<T>` | `ILinkType` + `Link<T>` |
| `IEntityLinksComponent<T>` | `ILinksType` + `Links<T>` |
| `IMultiComponent<T,V>` | `IMultiComponent` (marker) |
| `DeleteTagsSystem<W, T>` | `Query.BatchDeleteTag<T>()` |
| `OnComponentHandler<T>` / `OnCopyHandler<T>` | Hooks in IComponent |
| `W.UnloadCluster()` / `W.UnloadChunk()` | `Query().BatchUnload()` |
| `W.Events.XXX` | `W.XXX` |
| `W.Context.Set/Get/Has` | `W.SetResource/GetResource/HasResource` |

---

## Quick Rename Reference

| Before (v1.2.x) | After (v2.0.0) |
|---|---|
| `W.Entity.New(...)` | `W.NewEntity<TEntityType>(...)` |
| `W.Entity.NewOnes(...)` | `W.NewEntities<TEntityType>(count)` |
| `W.IsInitialized()` | `W.IsWorldInitialized` |
| `entity.Gid()` | `entity.GID` |
| `entity.HasAllOf<C>()` | `entity.Has<C>()` |
| `entity.HasAnyOf<C1,C2>()` | `entity.HasAny<C1,C2>()` |
| `entity.HasAllOfTags<T>()` | `entity.HasTag<T>()` |
| `Components<T>.Value` | `Components<T>.Instance` |
| `Tags<T>.Value` | `Tags<T>.Instance` |
| `W.Query.Entities<F>()` | `W.Query<F>()` |
| `W.Query.For(...)` | `W.Query().For(...)` |
| `AddForAll<C>()` | `BatchAdd<C>()` |
| `DeleteForAll<C>()` | `BatchDelete<C>()` |
| `SetTagForAll<T>()` | `BatchSetTag<T>()` |
| `DestroyAllEntities()` | `BatchDestroy()` |
| `Multi<T>.Count` | `Multi<T>.Length` |

---

## Summary

StaticEcs 2.0 is a transition from a fast but somewhat fragmented API to a cohesive, fundamentally expanded framework. Key achievements:

- **Change tracking** — 16 filters, bitmap storage, zero overhead when disabled. Network synchronization, reactive UI, triggers — without manual dirty flags.
- **Entity types** — logical and physical grouping. Cache locality out of the box, lifecycle hooks, parameterized creation.
- **Block iteration** — direct pointers to data arrays for maximum unmanaged code performance.
- **Unified ISystem** — one interface instead of five, with automatic detection of implemented methods.
- **Hooks in IComponent** — component behavior in one place, without Config classes.
- **Or-filters** — queries that were previously impossible.
- **Extended batch operations** — full spectrum with chaining.
- **Clean, uniform API** — properties instead of methods, short names, fluent registration.

Migration will require changes in virtually all user code, but every change is a step toward a simpler, faster, and more expressive API. A detailed migration guide with correspondence tables is available in [migrationguide](docs/en/migrationguide.md).
