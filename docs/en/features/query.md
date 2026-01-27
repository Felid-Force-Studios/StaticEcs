---
title: Queries
parent: Features
nav_order: 12
---

# Query
Queries are a mechanism for searching entities and their components in the world
- All queries require no caching, are stack-allocated, and can be used on-the-fly
- Support filtering by components, tags, entity status, and clusters
- Two iteration modes: `Strict` (default, faster) and `Flexible` (allows modifying filtered types on other entities)

___

## Filters

Types for describing filtering. Each occupies 1 byte and requires no initialization.

```csharp
// Given 5 entities in the world:
//             Components                 Tags           EntityType
// Entity 1:  Position, Velocity         Unit           Npc
// Entity 2:  Position, Name             Player         Npc
// Entity 3:  Position, Velocity, Name   Unit, Player   Npc
// Entity 4:  Velocity                   —              Bullet
// Entity 5:  Position■, Velocity        Unit           Bullet
//            (■ = disabled)
//
// Examples below show which entities pass each filter
```

### Components:
```csharp
// All — presence of ALL enabled components (from 1 to 8 types)
All<Position, Velocity, Direction> all = default;

// AllOnlyDisabled — presence of ALL disabled components
AllOnlyDisabled<Position> disabled = default;

// AllWithDisabled — presence of ALL components (any state)
AllWithDisabled<Position, Velocity> any = default;

// None — absence of enabled components (from 1 to 8 types)
None<Position, Name> none = default;

// NoneWithDisabled — absence of components (any state)
NoneWithDisabled<Position> noneAll = default;

// Any — presence of at least one enabled component (from 2 to 8 types)
Any<Position, Velocity> any = default;

// AnyOnlyDisabled — at least one disabled
AnyOnlyDisabled<Position, Velocity> anyDis = default;

// AnyWithDisabled — at least one (any state)
AnyWithDisabled<Position, Velocity> anyAll = default;

// Results for the entities above:
// All<Position, Velocity>              → 1, 3
// AllOnlyDisabled<Position>            → 5
// AllWithDisabled<Position, Velocity>  → 1, 3, 5
// None<Name>                           → 1, 4, 5
// NoneWithDisabled<Position>           → 4
// Any<Position, Name>                  → 1, 2, 3
// AnyOnlyDisabled<Position, Velocity>  → 5
// AnyWithDisabled<Position, Name>      → 1, 2, 3, 5
```

### Tags:

Tags use the same filters as components — `All<>`, `None<>`, `Any<>` and their variants. No separate tag filter types exist.

```csharp
// All — presence of ALL specified tags (from 1 to 8 types)
All<Unit, Player> tagAll = default;

// None — absence of specified tags (from 1 to 8 types)
None<Unit, Player> tagNone = default;

// Any — at least one of the specified tags (from 2 to 8 types)
Any<Unit, Player> tagAny = default;

// Results for the entities above:
// All<Unit, Player>  → 3
// None<Unit>         → 2, 4
// Any<Unit, Player>  → 1, 2, 3, 5
```

### Change tracking:
```csharp
// AllAdded — ALL specified components were added since last ClearTracking (from 1 to 5 types)
AllAdded<Position> added = default;
AllAdded<Position, Velocity> addedMulti = default;

// AnyAdded — AT LEAST ONE of the specified components was added (from 2 to 5 types)
AnyAdded<Position, Velocity> anyAdded = default;

// NoneAdded — NONE of the specified components were added (from 1 to 5 types)
NoneAdded<Position> noneAdded = default;

// AllDeleted — ALL specified components were deleted since last ClearTracking (from 1 to 5 types)
AllDeleted<Position> deleted = default;

// AnyDeleted — AT LEAST ONE was deleted (from 2 to 5 types)
AnyDeleted<Position, Velocity> anyDeleted = default;

// NoneDeleted — NONE were deleted (from 1 to 5 types)
NoneDeleted<Position> noneDeleted = default;

// AllChanged — ALL specified components were accessed via ref since last ClearChangedTracking (from 1 to 5 types)
// Requires ComponentTypeConfig.TrackChanged = true
AllChanged<Position> changed = default;

// AnyChanged — AT LEAST ONE was changed (from 2 to 5 types)
AnyChanged<Position, Velocity> anyChanged = default;

// NoneChanged — NONE were changed (from 1 to 5 types)
NoneChanged<Position> noneChanged = default;

// AllAdded / AnyAdded / NoneAdded / AllDeleted / AnyDeleted / NoneDeleted
// also work with tags — use the same filters for both components and tags

// Created — entity was created since last ClearCreatedTracking
// (requires WorldConfig.TrackCreated = true, no type parameters)
Created created = default;

// Combination with other filters
foreach (var entity in W.Query<AllAdded<Position>, All<Velocity, Unit>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
    // process newly added entities with Position
}
```

{: .note }
In delegate-based iteration (`For`), `ref` parameters mark the component as Changed, while `in` parameters do not. Use `in` for read-only access to avoid unnecessary Changed marks. See [Changed Tracking](tracking#changed-tracking) for details.

### Entity types:
```csharp
// EntityIs — exactly this entity type (1 type parameter)
EntityIs<Bullet> entityIs = default;

// EntityIsNot — exclude entity types (from 1 to 5 types)
EntityIsNot<Effect> entityIsNot = default;

// EntityIsAny — match any of the specified entity types (from 2 to 5 types)
EntityIsAny<Bullet, Rocket> entityIsAny = default;

// Results for the entities above:
// EntityIs<Npc>              → 1, 2, 3
// EntityIsNot<Bullet>        → 1, 2, 3
// EntityIsAny<Npc, Bullet>   → 1, 2, 3, 4, 5
```

### And / Or — composite filters:

`And` and `Or` allow grouping multiple filters into a single type. This is useful for:
- **Passing a complex filter as one generic parameter** — store in a field, pass to a method, or use as a type argument
- **Building filters that basic types cannot express** — e.g., "entities with component set A **or** component set B"

#### And — all conditions must match (from 2 to 6 filters):
```csharp
And<All<Position, Velocity>, None<Name>, Any<Unit, Player>> filter = default;

// Via factory method (type inference)
var filter = And.By(
    default(All<Position, Velocity>),
    default(None<Name>),
    default(Any<Unit, Player>)
);

// Use case: pass a composite filter to a helper method
void ProcessMovable(And<All<Position, Velocity>, None<Frozen>> filter) {
    foreach (var entity in W.Query(filter).Entities()) {
        entity.Ref<Position>().Value += entity.Read<Velocity>().Value;
    }
}
```

#### Or — at least one condition must match (from 2 to 6 filters):

`Or` enables combinationally complex filtering that basic filter types cannot express.

```csharp
// Melee fighters OR ranged fighters — completely different component sets,
// cannot be expressed with a single All/Any/None combination
Or<All<MeleeWeapon, Damage>, All<RangedWeapon, Ammo>> fighters = default;

// Rebuild spatial index when Position was added, removed, or modified
Or<AllAdded<Position>, AllDeleted<Position>, AllChanged<Position>> spatialChanged = default;

// Process both UI buttons (ClickArea + Label) and world interactables (Collider + Interaction)
Or<All<ClickArea, Label>, All<Collider, Interaction>> clickable = default;

// Via factory method
var filter = Or.By(
    default(All<MeleeWeapon, Damage>),
    default(All<RangedWeapon, Ammo>)
);

// Results for the entities above:
// Or<All<Position, Velocity>, All<Position, Name>>
// Entity 1: Pos✓ Vel✓         → ✓ (passes first)
// Entity 2: Pos✓ Name✓        → ✓ (passes second)
// Entity 3: Pos✓ Vel✓ Name✓   → ✓ (passes both)
// Entity 4: Pos✗              → ✗
// → Result: 1, 2, 3, 5
```

#### Nesting:
```csharp
// And and Or can be nested for arbitrarily complex logic
// (A and B and C) or (A and B and D):
Or<All<A, B, C>, All<A, B, D>> complex = default;

// All visible entities that are either alive units or active effects:
And<All<Visible>, Or<All<Unit, Alive>, All<Effect, Active>>> visibleAlive = default;
```

___

## Entity iteration

```csharp
// Iterate over all entities without filtering
foreach (var entity in W.Query().Entities()) {
    Console.WriteLine(entity.PrettyString);
}

// With filter via generic (from 1 to 8 filters)
foreach (var entity in W.Query<All<Position, Velocity>>().Entities()) {
    entity.Ref<Position>().Value += entity.Read<Velocity>().Value;
}

// With multiple filters
foreach (var entity in W.Query<All<Position, Velocity>, None<Name>>().Entities()) {
    entity.Ref<Position>().Value += entity.Read<Velocity>().Value;
}

// Via filter value
var all = default(All<Position, Velocity>);
foreach (var entity in W.Query(all).Entities()) {
    entity.Ref<Position>().Value += entity.Read<Velocity>().Value;
}

// Via And/Or — group filters into a single type for passing to methods or storing in fields
var filter = default(And<All<Position, Velocity>, None<Name>>);
foreach (var entity in W.Query(filter).Entities()) {
    entity.Ref<Position>().Value += entity.Read<Velocity>().Value;
}

// Flexible mode — allows modifying filtered types on other entities
foreach (var entity in W.Query<All<Position>>().EntitiesFlexible()) {
    // ...
}

// Find the first matching entity
if (W.Query<All<Position>>().Any(out var found)) {
    // found — first entity with Position
}

// Get the only entity (error in debug if more than one found)
if (W.Query<All<Position>>().One(out var single)) {
    // single — the only entity with Position
}

// Count matching entities (full scan)
int count = W.Query<All<Position>>().EntitiesCount();
```

___

## Delegate-based iteration (For)

Optimized iteration via delegates — unrolls loops under the hood.

```csharp
// Over all entities
W.Query().For(entity => {
    Console.WriteLine(entity.PrettyString);
});

// By components (from 1 to 6 types)
// Components in the delegate automatically act as an All filter
W.Query().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});

// With entity in delegate
W.Query().For(static (W.Entity entity, ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});

// With user data (to avoid delegate allocations)
W.Query().For(deltaTime, static (ref float dt, ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value * dt;
});

// With ref data (for accumulating results)
int count = 0;
W.Query().For(ref count, static (ref int counter, W.Entity entity, ref Position pos) => {
    counter++;
});

// With tuple of multiple parameters
W.Query().For((deltaTime, gravity), static (ref (float dt, float g) data, ref Position pos, ref Velocity vel) => {
    vel.Value += data.g * data.dt;
    pos.Value += vel.Value * data.dt;
});
```

### Readonly components (Read):

When a component is only read and not modified, use `in` instead of `ref` in delegates. This tells the change tracking system not to mark the component as changed.

```csharp
// Last N components as readonly via `in`
W.Query().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;  // Position — writable (ref), Velocity — readonly (in)
});

// All components readonly
W.Query().For(static (in Position pos, in Velocity vel) => {
    Console.WriteLine(pos.Value + vel.Value);
});

// With entity
W.Query().For(static (W.Entity entity, ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});

// With user data
W.Query().For(ref result, static (ref float res, in Position pos, in Velocity vel) => {
    res += pos.Value.Length;
});
```

{: .note }
Read variants are available when changed tracking is enabled (default). Can be disabled via `FFS_ECS_DISABLE_CHANGED_TRACKING` define.

### With additional filtering:
```csharp
// Components in the delegate act as an All filter,
// additional filters are specified directly on Query and don't require specifying delegate components
W.Query<Any<Unit, Player>>().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});

// With multiple filters
W.Query<None<Name>, Any<Unit, Player>>().For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});

// Via value
var filter = default(Any<Unit, Player>);
W.Query(filter).For(static (ref Position pos, in Velocity vel) => {
    pos.Value += vel.Value;
});
```

### Entity and component status:
```csharp
W.Query().For(
    static (ref Position pos, ref Velocity vel) => {
        // ...
    },
    entities: EntityStatusType.Disabled,    // Enabled (default), Disabled, Any
    components: ComponentStatus.Disabled    // Enabled (default), Disabled, Any
);
```

___

## Single entity search (Search)

Iteration with early exit on first match. All components in search delegates are readonly (`in`).

```csharp
if (W.Query().Search(out W.Entity found,
    (W.Entity entity, in Position pos, in Health health) => {
        return pos.Value.x > 100 && health.Current < 50;
    })) {
    // found — first entity matching the condition
}
```

___

## Function structs (IQuery / IQueryBlock)

Function structs instead of delegates — for optimization, state passing, or extracting logic.
Function structs use a **fluent builder API** on `WorldQuery` — unlike delegates, component types are not specified via generic parameters on `For`, but via the builder chain.

### IQuery — per-entity callback:

The interface hierarchy uses nested types for write/read access control (from 1 to 6 components total):
- `IQuery.Write<T0, T1>` — all components writable (`ref`)
- `IQuery.Read<T0, T1>` — all components readonly (`in`)
- `IQuery.Write<T0>.Read<T1>` — first writable, rest readonly

```csharp
// All writable — IQuery.Write
readonly struct MoveFunction : W.IQuery.Write<Position, Velocity> {
    public void Invoke(W.Entity entity, ref Position pos, ref Velocity vel) {
        pos.Value += vel.Value;
    }
}

// Fluent API: Write<...>() specifies writable components, then For<TFunction>() executes
W.Query().Write<Position, Velocity>().For<MoveFunction>();

// With a value
W.Query().Write<Position, Velocity>().For(new MoveFunction());

// Via ref (to preserve state after iteration)
var func = new MoveFunction();
W.Query().Write<Position, Velocity>().For(ref func);

// Mixed write/read — IQuery.Write<>.Read<>
readonly struct ApplyVelocity : W.IQuery.Write<Position>.Read<Velocity> {
    public void Invoke(W.Entity entity, ref Position pos, in Velocity vel) {
        pos.Value += vel.Value;
    }
}

// Chain: Write<writable>().Read<readonly>().For<TFunction>()
W.Query().Write<Position>().Read<Velocity>().For<ApplyVelocity>();

// All readonly — IQuery.Read
readonly struct PrintPositions : W.IQuery.Read<Position, Velocity> {
    public void Invoke(W.Entity entity, in Position pos, in Velocity vel) {
        Console.WriteLine(pos.Value + vel.Value);
    }
}

W.Query().Read<Position, Velocity>().For<PrintPositions>();

// With additional filtering
W.Query<None<Name>, Any<Unit, Player>>()
    .Write<Position, Velocity>().For<MoveFunction>();

// Combining system and IQuery
public struct MoveSystem : ISystem, W.IQuery.Write<Position>.Read<Velocity> {
    private float _speed;

    public void Update() {
        _speed = W.GetResource<GameConfig>().Speed;
        W.Query<All<Unit>>()
            .Write<Position>().Read<Velocity>().For(ref this);
    }

    public void Invoke(W.Entity entity, ref Position pos, in Velocity vel) {
        pos.Value += vel.Value * _speed;
    }
}
```

### WorldQuery methods

#### Delegates — component types are inferred from the lambda:

| Method | Components |
|--------|------------|
| `For(delegate)` | 1–6, `ref` or `in` per component |
| `ForParallel(delegate)` | 1–6, `ref` or `in` per component |
| `Search(out entity, delegate)` | 1–6, all `in` |

#### Function structs — component access via builder:

| Method | Components | Access |
|--------|------------|--------|
| `Write<1‑6>()` | 1–6 | all `ref` |
| `Write<1‑5>().Read<1‑5>()` | 2–6 total | first `ref`, rest `in` |
| `Read<1‑6>()` | 1–6 | all `in` |

#### Block function structs — same pattern, `unmanaged` only:

| Method | Components | Access |
|--------|------------|--------|
| `WriteBlock<1‑6>()` | 1–6 | all `Block<T>` |
| `WriteBlock<1‑5>().Read<1‑5>()` | 2–6 total | `Block<T>` + `BlockR<T>` |
| `ReadBlock<1‑6>()` | 1–6 | all `BlockR<T>` |

Each builder provides `For<F>()` and `ForParallel<F>()`.
`Read` / `ReadBlock` require change tracking (enabled by default, disable via `FFS_ECS_DISABLE_CHANGED_TRACKING`).

___

## Parallel processing

{: .warning }
Parallel processing requires enabling at world creation: `ParallelQueryType.MaxThreadsCount` or `ParallelQueryType.CustomThreadsCount` with `CustomThreadCount` in `WorldConfig`.
Inside parallel iteration, only the **current** iterated entity may be modified or destroyed. Forbidden: creating entities, modifying other entities, reading events. Sending events (`SendEvent`) is thread-safe (when there is no concurrent reading of the same type, see [Events](events#multithreading) for details). Always uses `QueryMode.Strict`.

```csharp
// Delegate — the first parameter, minEntitiesPerThread is named (default 256)
W.Query().ForParallel(
    static (W.Entity entity, ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value;
    },
    minEntitiesPerThread: 50000
);

// Without entity — components only
W.Query().ForParallel(
    static (ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value;
    },
    minEntitiesPerThread: 50000
);

// With user data
W.Query().ForParallel(deltaTime,
    static (ref float dt, ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value * dt;
    },
    minEntitiesPerThread: 50000
);

// With filtering
W.Query<None<Name>, Any<Unit, Player>>().ForParallel(
    static (W.Entity entity) => {
        entity.Add<Name>();
    },
    minEntitiesPerThread: 50000
);

// Via function struct
W.Query().Write<Position>().Read<Velocity>().ForParallel<ApplyVelocity>(
    minEntitiesPerThread: 50000
);

// workersLimit — limit the number of threads (0 = use all available)
W.Query().ForParallel(
    static (ref Position pos) => { /* ... */ },
    minEntitiesPerThread: 10000,
    workersLimit: 4
);
```

___

## Block iteration (ForBlock)

Low-level iteration via function structs — for `unmanaged` components, provides `Block<T>` (writable) and `BlockR<T>` (readonly) wrappers with direct pointers to data arrays.

The interface hierarchy mirrors `IQuery` (from 1 to 6 unmanaged components total):
- `IQueryBlock.Write<T0, T1>` — all components writable (`Block<T>`)
- `IQueryBlock.Read<T0, T1>` — all components readonly (`BlockR<T>`)
- `IQueryBlock.Write<T0>.Read<T1>` — first writable, rest readonly

```csharp
// All writable — IQueryBlock.Write
readonly struct MoveBlock : W.IQueryBlock.Write<Position, Velocity> {
    public void Invoke(uint count, EntityBlock entitiesBlock,
                       Block<Position> positions, Block<Velocity> velocities) {
        for (uint i = 0; i < count; i++) {
            positions[i].Value += velocities[i].Value;
        }
    }
}

// Fluent API: WriteBlock<...>().For<TFunction>()
W.Query().WriteBlock<Position, Velocity>().For<MoveBlock>();

// Mixed write/read — WriteBlock<>.Read<>
readonly struct ApplyVelocityBlock : W.IQueryBlock.Write<Position>.Read<Velocity> {
    public void Invoke(uint count, EntityBlock entitiesBlock,
                       Block<Position> positions, BlockR<Velocity> velocities) {
        for (uint i = 0; i < count; i++) {
            positions[i].Value += velocities[i].Value;
        }
    }
}

W.Query().WriteBlock<Position>().Read<Velocity>().For<ApplyVelocityBlock>();

// All readonly — ReadBlock<>
readonly struct SumPositionsBlock : W.IQueryBlock.Read<Position> {
    public void Invoke(uint count, EntityBlock entitiesBlock, BlockR<Position> positions) {
        for (uint i = 0; i < count; i++) {
            // read-only access
        }
    }
}

W.Query().ReadBlock<Position>().For<SumPositionsBlock>();

// Via ref (to preserve state)
var func = new MoveBlock();
W.Query().WriteBlock<Position, Velocity>().For(ref func);

// Parallel version
W.Query().WriteBlock<Position, Velocity>().ForParallel<MoveBlock>(minEntitiesPerThread: 50000);
```

___

## Batch operations

Bulk operations on all entities matching a filter — without writing a loop.
Can be orders of magnitude faster than manual iteration via `For`: instead of per-entity processing, batch operations work with bitmasks — in the best case, adding or removing a component/tag for 64 entities is a single bitwise operation.
Support call chaining — multiple operations can be performed in a single pass.

```csharp
// Add component to all entities (from 1 to 5 types)
W.Query<All<Position>>().BatchSet(new Velocity { Value = 1f });

// Delete component from all
W.Query<All<Position, Velocity>>().BatchDelete<Velocity>();

// Disable/enable component on all
W.Query<All<Position>>().BatchDisable<Position>();
W.Query<AllOnlyDisabled<Position>>().BatchEnable<Position>();

// Tags: set, delete, toggle, apply by condition (from 1 to 5 types)
W.Query<All<Position>>().BatchSet<Unit>();
W.Query<All<Unit>>().BatchDelete<Unit>();
W.Query<All<Position>>().BatchToggle<Unit>();
W.Query<All<Position>>().BatchApply<Unit>(true);

// Chaining
W.Query<All<Position>>()
    .BatchSet(new Velocity { Value = 1f })
    .BatchSet<Unit>()
    .BatchDisable<Position>();
```

___

## Destroying and unloading entities

```csharp
// Destroy all entities matching a filter
W.Query<All<Position>>().BatchDestroy();

// With parameters
W.Query<All<Unit>>().BatchDestroy(
    entities: EntityStatusType.Any,
    mode: QueryMode.Flexible
);

// Unload all entities matching a filter
// (marks as unloaded, removes components/tags, but preserves entity IDs and versions)
W.Query<All<Position>>().BatchUnload();

// With parameters
W.Query<All<Unit>>().BatchUnload(
    entities: EntityStatusType.Any,
    mode: QueryMode.Flexible
);
```

___

## Clusters

{: .important }
All query methods (`Entities`, `For`, `ForParallel`, `Search`, `Batch*`, `BatchDestroy`, `BatchUnload`) accept a `clusters` parameter:

```csharp
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { 2, 5, 12 };

foreach (var entity in W.Query<All<Position>>().Entities(clusters: clusters)) {
    // iteration only over entities from clusters 2, 5, 12
}

W.Query().For(static (W.Entity entity, ref Position pos) => {
    // ...
}, clusters: clusters);
```

___

## QueryMode

For `For`, `Search`, `Entities` methods:

- **`QueryMode.Strict`** (default) — forbids modifying filtered component/tag types on **other** entities during iteration. Faster.
- **`QueryMode.Flexible`** — allows modifying filtered types on other entities, correctly controls current iteration.

```csharp
var anotherEntity = W.NewEntity<Default>();
anotherEntity.Add<Position>();

// Strict: modifying Position on another entity — error in DEBUG
foreach (var entity in W.Query<All<Position>>().Entities()) {
    anotherEntity.Delete<Position>(); // ERROR in DEBUG
}

// Flexible: modification allowed, iteration is stable
foreach (var entity in W.Query<All<Position>>().EntitiesFlexible()) {
    anotherEntity.Delete<Position>(); // OK — anotherEntity correctly excluded
}

// For For/Search via parameter
W.Query().For(static (ref Position pos) => {
    // ...
}, queryMode: QueryMode.Flexible);
```

{: .note }
`Flexible` is useful for hierarchies or caches when modifying entities from components of other entities. In other cases, prefer `Strict` for performance.
