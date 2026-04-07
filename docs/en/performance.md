---
title: Performance
parent: EN
nav_order: 3
---

# Performance

## Architectural features

StaticEcs is designed for maximum performance and massive worlds:

- **Entity never moves in memory** on Add/Remove — operations are bitwise O(1). In archetype-based ECS, adding or removing a component moves the entity between archetypes, copying all data. In sparse set ECS, removing a component swap-backs the last element into the removed slot

- **SoA storage** (Structure of Arrays) — components of the same type are contiguous in memory, ensuring optimal CPU cache utilization during iteration. Archetype-based ECS also use SoA within archetypes, but data is fragmented across separate arrays of different archetypes, the number of which grows combinatorially. In StaticEcs, all components of the same type are stored in a single segment array — fragmentation is possible when using many entityTypes and clusters, but remains controllable. Sparse set ECS store components in dense arrays, but accessing multiple components of the same entity requires indexing through different arrays with potentially different element order

- **Static generics** — data access via `Components<T>` is a direct static field access resolved at compile time. In other ECS, finding a component pool requires hash lookup by type ID or access through lookups with safety checks

- **No archetype explosion problem** — in archetype-based ECS, each unique component combination creates a new archetype. With 30+ component types, the number of archetypes can reach thousands, causing memory fragmentation and iteration degradation. StaticEcs is free from this problem — the number of component types doesn't affect storage structure

- **Zero allocations** on the hot path — all data structures are pre-allocated, queries return ref struct iterators. In other ECS, creating a view/filter may require allocations on first use or is managed through wrappers with safety check overhead

- **Two-dimensional partitioning** (Cluster × EntityType) — built-in spatial and logical grouping at the memory level, allowing control over entity placement without changing the component set. In other ECS, grouping is only possible via query filters (tags, shared components), without direct control over memory layout

- **Built-in streaming** — loading/unloading clusters and chunks without rebuilding internal structures. In archetype-based ECS, mass creation or deletion of entities causes chunk rebalancing. In sparse set ECS, mass deletion fragments dense arrays

- **Predictable performance** — Add/Remove/Has operation time doesn't depend on the number of components on an entity or the total number of types in the world. In archetype-based ECS, the cost of structural changes grows with component count (all entity data is copied). In sparse set ECS, Has/Ref cost is constant, but iterating over multiple components requires set intersection

___

## Iteration methods (fastest to most convenient)

#### 1. ForBlock — block pointers (fastest for unmanaged):
```csharp
readonly struct MoveBlock : W.IQueryBlock.Write<Position>.Read<Velocity> {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(uint count, W.EntityBlock entities,
                       Block<Position> positions, BlockR<Velocity> velocities) {
        for (uint i = 0; i < count; i++) {
            positions[i].Value += velocities[i].Value;
        }
    }
}

W.Query().WriteBlock<Position>().Read<Velocity>().For<MoveBlock>();
```

#### 2. For with function struct (zero-allocation, stateful):
```csharp
struct MoveFunction : W.IQuery.Write<Position>.Read<Velocity> {
    public float DeltaTime;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(W.Entity entity, ref Position pos, in Velocity vel) {
        pos.Value += vel.Value * DeltaTime;
    }
}

W.Query().Write<Position>().Read<Velocity>().For(new MoveFunction { DeltaTime = 0.016f });
```

#### 3. For with delegate (zero-allocation with static lambdas):
```csharp
// Without data
W.Query().For(
    static (ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value;
    }
);

// With user data (no captures)
W.Query().For(deltaTime,
    static (ref float dt, ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value * dt;
    }
);
```

#### 4. Foreach iteration (most flexible):
```csharp
foreach (var entity in W.Query<All<Position, Velocity>>().Entities()) {
    ref var pos = ref entity.Ref<Position>();
    ref readonly var vel = ref entity.Read<Velocity>();
    pos.Value += vel.Value;
}
```

___

## Extension methods for IL2CPP

When using IL2CPP in Unity, standard generic Entity methods (`entity.Ref<T>()`, `entity.Has<T>()`) can be 10–25% slower due to AOT compilation specifics. It is recommended to create typed extension methods:

```csharp
public static class ComponentExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref Position RefPosition(this W.Entity entity) {
        return ref W.Components<Position>.Instance.Ref(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasPosition(this W.Entity entity) {
        return W.Components<Position>.Instance.Has(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasTagPlayer(this W.Entity entity) {
        return W.Tags<IsPlayer>.Instance.Has(entity);
    }
}
```

```csharp
// Usage — convenient and fast
ref var pos = ref entity.RefPosition();
bool has = entity.HasPosition();
bool isPlayer = entity.HasTagPlayer();
```

{: .note }
In Mono/CoreCLR the difference is minimal due to aggressive JIT inlining. This optimization is specifically relevant for IL2CPP.

___

## Parallel execution

To enable multithreaded queries, specify the thread count in world configuration:

```csharp
W.Create(new WorldConfig {
    ThreadCount = WorldConfig.MaxThreadCount, // all available CPU threads
    // or
    // ThreadCount = 8, // specific number of threads
});
```

```csharp
// Parallel iteration
W.Query().ForParallel(
    static (ref Position pos, in Velocity vel) => {
        pos.Value += vel.Value;
    },
    minEntitiesPerThread: 50000  // minimum entities per thread
);
```

{: .important }
Parallel iteration constraints: only the current entity may be modified/destroyed. No entity creation, no modification of other entities. `SendEvent` is thread-safe (when there is no concurrent reading of the same type).

___

## Entity type (entityType)

`entityType` groups logically similar entities in adjacent memory segments, improving cache locality:

```csharp
struct UnitType : IEntityType { }
struct BulletType : IEntityType { }
struct EffectType : IEntityType { }

// Units are co-located in memory
var unit = W.NewEntity<UnitType>();
unit.Add<Position>(); unit.Add<Health>();

// Bullets — in their own segments
var bullet = W.NewEntity<BulletType>();
bullet.Add<Position>(); bullet.Add<Velocity>();
```

Queries automatically iterate over contiguous memory blocks — the more homogeneous the data, the more efficient the CPU cache.

___

## Cluster-scoped queries

Limiting queries to specific clusters skips unrelated chunks:

```csharp
const ushort ACTIVE_ZONE = 1;
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { ACTIVE_ZONE };

// Iterate only over specified clusters
W.Query().For(
    static (ref Position pos) => { pos.Value.Y -= 9.8f * 0.016f; },
    clusters: clusters
);
```

___

## Batch operations

Batch operations work at the bitmask level — a single bitwise operation affects up to 64 entities at once. This is orders of magnitude faster than per-entity iteration.

#### Available operations:

| Method | Description |
|--------|-------------|
| `BatchAdd<T>()` | Add components (default values, 1–5 types) |
| `BatchSet<T>(value)` | Add components with values (1–5 types) |
| `BatchDelete<T>()` | Remove components (1–5 types) |
| `BatchEnable<T>()` | Enable components (1–5 types) |
| `BatchDisable<T>()` | Disable components (1–5 types) |
| `BatchSet<T>()` | Set tags (1–5 types) |
| `BatchDelete<T>()` | Remove tags (1–5 types) |
| `BatchToggle<T>()` | Toggle tags (1–5 types) |
| `BatchApply<T>(bool)` | Set or unset tag by condition (1–5 types) |
| `BatchDestroy()` | Destroy all matching entities |
| `BatchUnload()` | Unload all matching entities |
| `EntitiesCount()` | Count matching entities |

#### Examples:
```csharp
// Chain operations — add component, set tag, disable component
W.Query<All<Position>>()
    .BatchSet(new Velocity { Value = Vector3.One })
    .BatchSet<IsMovable>()
    .BatchDisable<Position>();

// Destroy all entities with the IsDead tag
W.Query<All<Health, IsDead>>().BatchDestroy();

// Count entities
int count = W.Query<All<Position, Velocity>>().EntitiesCount();

// Filter by clusters and entity status
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { 1, 2 };
W.Query<All<Position>>().BatchDelete<Velocity>(
    entities: EntityStatusType.Any,
    clusters: clusters
);

// Toggle tag — entities that had it will lose it; those without will get it
W.Query<All<Position>>().BatchToggle<IsVisible>();
```

{: .note }
All batch operations support filtering by `EntityStatusType` (Enabled/Disabled/Any) and `clusters`. Methods return `WorldQuery` for chaining.

___

## QueryMode

The default is `QueryMode.Strict` — the fastest mode. Use `QueryMode.Flexible` only when you need to modify filtered components/tags on **other** entities during iteration:

```csharp
// Strict (default) — fast path for full blocks
W.Query().For(
    static (ref Position pos) => { /* ... */ }
);

// Flexible — re-checks bitmasks on each iteration
W.Query().For(
    static (W.Entity entity, ref Position pos) => {
        // Can modify Position on other entities
    },
    queryMode: QueryMode.Flexible
);
```

___

## Stripping (reducing build size)

StaticEcs extensively uses generic overloads: Query0–Query6 × delegate variants × Read variants × Parallel — this produces a large number of generic specializations, most of which are unused in any given project. To remove unused code from the build and significantly reduce its size, use managed code stripping.

#### Unity:
Set **Player Settings → Other Settings → Managed Stripping Level** to **Medium** or **High**. This removes unreferenced generic instantiations generated by the library.

#### .NET (publish trimming):
```xml
<PropertyGroup>
    <PublishTrimmed>true</PublishTrimmed>
    <TrimMode>link</TrimMode>
</PropertyGroup>
```

{: .important }
After enabling stripping, test your build thoroughly — aggressive stripping may remove code that is only accessed via reflection. If you use `RegisterAll` for auto-discovery, ensure the relevant types are preserved (e.g., via `[Preserve]` attribute in Unity or TrimmerRootAssembly in .NET).

___

## Recommendations

| Practice | Reason |
|----------|--------|
| Use `ForBlock` for critical loops | Direct pointers, minimal overhead |
| Use `static` lambdas in `For` | Zero allocations, JIT inlining |
| Use `in` for read-only components | Correct change tracking semantics |
| Group entities by `entityType` | Cache locality |
| Scope queries to clusters | Skip unrelated chunks |
| `QueryMode.Strict` by default | 10–40% faster than Flexible |
| Batch operations for bulk changes | Single operation per 64 entities |
| Medium/High stripping in Unity | Removes unused generic overloads |
| `UnmanagedPackArrayStrategy<T>` for serialization | Bulk memory copy |
| Typed extension methods for IL2CPP | 10–25% faster than generic Entity wrappers |
