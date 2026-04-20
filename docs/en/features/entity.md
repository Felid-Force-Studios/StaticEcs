---
title: Entity
parent: Features
nav_order: 1
---

## Entity
Entity is a structure for identifying an object in the world and accessing its components and tags
- 4-byte struct (`uint` wrapper over a slot index)
- Does not contain a generation counter — for persistent references use [EntityGID](gid.md)
- All component and tag operations are available as methods on the entity itself

___

## Entity type (IEntityType)

Entity type is a logical category assigned at creation. It determines the entity's purpose (units, bullets, effects) and controls memory placement — entities of the same type within a cluster are stored in the same segments.

### Defining entity types

Entity types are defined as structs implementing `IEntityType` with a stable `byte Id()` method:

```csharp
public struct Bullet : IEntityType {
    public byte Id() => 1;
}

public struct Enemy : IEntityType {
    public byte Id() => 2;
}

public struct Effect : IEntityType {
    public byte Id() => 3;
}
```

The built-in `Default` type (Id = 0) is registered automatically when the world is created.

### Registration

Entity types are registered during the `Created` phase.

**Manual registration:**
```csharp
W.Types()
    .EntityType<Bullet>()
    .EntityType<Enemy>()
    .EntityType<Effect>();
```

**Auto-registration:**
```csharp
W.Types().RegisterAll();
```

`RegisterAll()` discovers all types implementing `IEntityType` in the specified assemblies (defaults to `typeof(TWorld).Assembly` — no stack walking, safe on Unity IL2CPP, Unity WebGL and NativeAOT) and registers them automatically. The identifier is obtained from the `Id()` method.

### Lifecycle hooks (OnCreate / OnDestroy)

Entity types can define `OnCreate` and `OnDestroy` hooks. If a method is not defined in the struct, it is not called. There is no need to leave empty implementations.

```csharp
public struct Bullet : IEntityType {
    public byte Id() => 1;

    public void OnCreate<TWorld>(World<TWorld>.Entity entity) where TWorld : struct, IWorldType {
        entity.Set(new Velocity { Speed = 100 });
        entity.Set<Active>();
    }

    public void OnDestroy<TWorld>(World<TWorld>.Entity entity, HookReason reason) where TWorld : struct, IWorldType {
        // Cleanup logic, event sending, etc.
        // All components and tags are still accessible here.
    }
}
```

### Entity types with data

Since `IEntityType` is a struct, it can carry fields. `OnCreate` is an instance method — it has access to fields via `this`. This allows parameterized creation without extra arguments or allocations:

```csharp
public struct Flora : IEntityType {
    public byte Id() => 4;

    public enum Kind : byte { Grass, Bush, Tree }
    public Kind FloraKind;

    public void OnCreate<TWorld>(World<TWorld>.Entity entity) where TWorld : struct, IWorldType {
        entity.Set(new Health { Value = FloraKind == Kind.Tree ? 100 : 10 });
    }
}

// Usage:
var tree = W.NewEntity(new Flora { FloraKind = Flora.Kind.Tree });
var grass = W.NewEntity(new Flora { FloraKind = Flora.Kind.Grass });
```

### Why it matters

**Data locality during iteration.** Components are stored in SoA arrays indexed by entity position. When entities of the same type (e.g., all bullets) occupy adjacent slots in the same segment, their components are contiguous in memory — the CPU efficiently utilizes cache lines.

**Reduced fragmentation.** Without typing, entities of different kinds would be created interleaved. With typing, holes from destroyed bullets are filled by new bullets — the segment remains homogeneous.

**Query filtering.** Entity type filters (`EntityIs<T>`, `EntityIsNot<T>`, `EntityIsAny<T0,T1>`) work at the segment level with zero per-entity cost — `FilterEntities` is a no-op. This is the cheapest filter type in the system.

### entityType and clusterId

These two parameters complement each other:

- **`entityType`** — **logical** grouping: defines *what* the entity is (unit, bullet, effect). Affects memory placement — entities of the same type are stored together for optimal iteration.
- **`clusterId`** — **spatial** grouping: defines *where* the entity is (level, map zone, room). Allows restricting queries to specific areas of the world and managing streaming — loading and unloading entire clusters.

Segmentation works at the intersection of these parameters: within each cluster, separate segments are allocated for each type.

___

## Creation

```csharp
// Create an entity with default type (Id = 0)
W.Entity entity = W.NewEntity<Default>();

// With specific entity type — OnCreate hook is called automatically
W.Entity entity = W.NewEntity<Bullet>();
W.Entity entity = W.NewEntity<Enemy>(clusterId: LEVEL_1_CLUSTER);

// With data in the entity type struct
W.Entity entity = W.NewEntity(new Flora { FloraKind = Flora.Kind.Tree });

// With components — Set returns Entity, enabling chaining
W.Entity entity = W.NewEntity<Bullet>().Set(new Position { Value = Vector3.One });
W.Entity entity = W.NewEntity<Bullet>().Set(
    new Position { Value = Vector3.One },
    new Velocity { Value = 10f },
    new Damage { Value = 5 }
);

// Create in a specific chunk
W.Entity entity = W.NewEntityInChunk<Bullet>(chunkIdx: chunkIdx);

// Create by GID (for deserialization and network synchronization)
W.Entity entity = W.NewEntityByGID<Default>(gid);

// Non-generic overloads (entity type known only at runtime, e.g. during deserialization)
byte entityTypeId = EntityTypeInfo<Bullet>.Id;
W.Entity entity = W.NewEntity(entityTypeId, clusterId: LEVEL_1_CLUSTER);
W.Entity entity = W.NewEntityInChunk(entityTypeId, chunkIdx: chunkIdx);
W.Entity entity = W.NewEntityByGID(entityTypeId, gid);
```

#### Creation in a dependent world (Try):

{: .note }
A dependent world (`Independent = false`) shares slot space with other worlds. If its allocated slots are exhausted, entity creation is not possible.

```csharp
// Returns false if the dependent world has run out of allocated slots
if (W.TryNewEntity<Bullet>(out var entity, clusterId: LEVEL_1_CLUSTER)) {
    entity.Set(new Position { Value = Vector3.Zero });
}
```

___

## Batch creation

```csharp
uint count = 1000;

// Without components
W.NewEntities<Default>(count);

// With components by type (from 1 to 5 types)
W.NewEntities<Default, Position>(count);
W.NewEntities<Default, Position, Velocity>(count);

// With components by value (from 1 to 8 components)
W.NewEntities<Default>(count, new Position { Value = Vector3.Zero });
W.NewEntities<Default>(count,
    new Position { Value = Vector3.Zero },
    new Velocity { Value = 1f }
);

// With initialization delegate for each entity
W.NewEntities<Default, Position>(count, onCreate: static entity => {
    entity.Set<Unit>();
});

// With cluster
W.NewEntities<Default, Position>(count, clusterId: LEVEL_1_CLUSTER);

// Full overload: values + cluster + delegate
W.NewEntities<Default>(count,
    new Position { Value = Vector3.Zero },
    clusterId: LEVEL_1_CLUSTER,
    onCreate: static entity => {
        entity.Set<Unit>();
    }
);
```

___

## Properties

```csharp
W.Entity entity = W.NewEntity<Bullet>();

uint id = entity.ID;                         // Internal slot index
EntityGID gid = entity.GID;                  // Global identifier (8 bytes)
EntityGIDCompact gidC = entity.GIDCompact;   // Compact identifier (4 bytes)
ushort version = entity.Version;             // Slot generation counter
ushort clusterId = entity.ClusterId;         // Cluster identifier
byte entityType = entity.EntityType;         // Entity type (0–255)
uint chunkId = entity.ChunkID;              // Chunk index

bool alive = entity.IsNotDestroyed;          // Not destroyed
bool destroyed = entity.IsDestroyed;         // Destroyed
bool enabled = entity.IsEnabled;             // Enabled (participates in queries)
bool disabled = entity.IsDisabled;           // Disabled
bool selfOwned = entity.IsSelfOwned;         // Segment belongs to this world (not received from another world)

// Entity type checks
bool isBullet = entity.Is<Bullet>();                    // Exact type match
bool isProjectile = entity.IsAny<Bullet, Rocket>();     // Any of the types
bool isNotEffect = entity.IsNot<Effect>();              // Not this type
bool isNotVfx = entity.IsNot<Effect, Particle>();       // Not any of these

string info = entity.PrettyString;           // Debug string (ID, version, components, tags)
```

___

## Lifecycle

```csharp
// Disable entity — excluded from standard queries but retains all data
entity.Disable();

// Re-enable
entity.Enable();

// Destroy — removes all components (triggering OnDelete hooks), tags, frees slot
// Returns bool: true if the entity was alive and destroyed, false if already destroyed
entity.Destroy();

// Unload from memory — entity becomes invisible but its ID is preserved
// Used for streaming (temporary unload with subsequent reload via serialization)
entity.Unload();

// Increment version without destroying — all previously obtained GIDs become invalid
entity.UpVersion();
```

___

## Cloning and transfer

```csharp
// Clone entity — creates a new entity with a copy of all components and tags
W.Entity clone = entity.Clone();

// Clone into a different cluster
W.Entity clone = entity.Clone(clusterId: OTHER_CLUSTER);

// Copy all components and tags to an existing entity
// If the target already has matching components — they are overwritten
entity.CopyTo(targetEntity);

// Move all data to an existing entity and destroy the source
entity.MoveTo(targetEntity);

// Move to a different cluster — creates a new entity, copies data, destroys the source
W.Entity moved = entity.MoveTo(clusterId: OTHER_CLUSTER);
```

___

## Components

Component API is described in the [Components](component.md) section.

___

## Tags

Tag API is described in the [Tags](tag.md) section.

___

## Multi-components

Multi-component API is described in the [Multi-components](multicomponent.md) section.

___

## Relations

Entity relations API is described in the [Relations](relations.md) section.

___

## Debugging

```csharp
// Debug string with full information
string info = entity.PrettyString;

// Component count (enabled + disabled)
int compCount = entity.ComponentsCount();

// Tag count
int tagCount = entity.TagsCount();

// Get all components (list is cleared before filling)
var components = new List<IComponent>();
entity.GetAllComponents(components);

// Get all tags (list is cleared before filling)
var tags = new List<ITag>();
entity.GetAllTags(tags);
```

___

## Query filters by entity type

Entity type filters are described in the [Queries — Entity types](query.md#entity-types) section.

___

## Operators and conversions

```csharp
W.Entity a = W.NewEntity<Default>();
W.Entity b = W.NewEntity<Default>();

// Comparison by slot index (no version check)
bool eq = a == b;
bool neq = a != b;

// Implicit conversion Entity → EntityGID (8 bytes)
EntityGID gid = entity;

// Explicit conversion Entity → EntityGIDCompact (4 bytes)
// Throws in DEBUG if Chunk >= 4 or ClusterId >= 4
EntityGIDCompact compact = (EntityGIDCompact)entity;

// Convert to typed link (for the relations system)
Link<ChildOf> link = entity.AsLink<ChildOf>();
```
