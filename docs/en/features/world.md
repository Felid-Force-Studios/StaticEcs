---
title: World
parent: Features
nav_order: 9
---

## WorldType
World identifier type-tag, used to isolate static data when creating different worlds in the same process
- Represented as a user struct with no data and the `IWorldType` marker interface
- Each unique `IWorldType` gets its own fully isolated static storage

#### Example:
```csharp
public struct MainWorldType : IWorldType { }
public struct MiniGameWorldType : IWorldType { }
```

___

## World
Library entry point responsible for accessing, creating, initializing, operating, and destroying world data
- Represented as a static class `World<T>` parameterized by `IWorldType`

{: .important }
> Since the `IWorldType` type-identifier defines access to a specific world,
> there are three ways to work with the framework:

___

#### First way — full qualification:
```csharp
public struct WT : IWorldType { }

World<WT>.Create(WorldConfig.Default());
World<WT>.CalculateEntitiesCount();

var entity = World<WT>.NewEntity<Default>();
```

#### Second way — static imports:
```csharp
using static FFS.Libraries.StaticEcs.World<WT>;

public struct WT : IWorldType { }

Create(WorldConfig.Default());
CalculateEntitiesCount();

var entity = NewEntity<Default>();
```

#### Third way — type alias in the root namespace:
This is the method used in all examples
```csharp
public struct WT : IWorldType { }

public abstract class W : World<WT> { }

W.Create(WorldConfig.Default());
W.CalculateEntitiesCount();

var entity = W.NewEntity<Default>();
```

___

## Lifecycle

```
Create() → Type registration → Initialize() → Work → Destroy()
```

#### WorldStatus:
- `NotCreated` — world not created or destroyed
- `Created` — structures allocated, type registration available
- `Initialized` — world fully operational, entity operations available

___

#### Creating the world:
```csharp
// Define the world identifier
public struct WT : IWorldType { }
public abstract class W : World<WT> { }

// Create with default configuration
W.Create(WorldConfig.Default());

// Or with custom configuration
W.Create(new WorldConfig {
    // Independent world (manages chunks automatically) or dependent (requires manual chunk management)
    Independent = true,
    // Initial capacity for component types (default — 64)
    BaseComponentTypesCount = 64,
    // Initial capacity for clusters (minimum 16, default — 16)
    BaseClustersCapacity = 16,
    // Multithreading mode
    // Disabled — no threads created
    // MaxThreadsCount — maximum available threads
    // CustomThreadsCount — specified number of threads
    ParallelQueryType = ParallelQueryType.Disabled,
    // Thread count when using CustomThreadsCount
    CustomThreadCount = 4,
    // Worker spin iterations before blocking (default — 256)
    WorkerSpinCount = 256,
    // Enable entity creation tracking for the Created query filter (default — false)
    TrackCreated = true,
});
```

{: .note }
`WorldConfig` provides factory methods:
- `WorldConfig.Default()` — standard settings (single-threaded, independent)
- `WorldConfig.MaxThreads()` — all available CPU threads
Both accept `bool independent = true`.

___

#### Type registration:
```csharp
W.Create(WorldConfig.Default());

// Register components, tags, and events — only between Create() and Initialize()
W.Types()
    .EntityType<Bullet>(Bullet.Id)
    .Component<Position>()
    .Component<Velocity>()
    .Tag<IsPlayer>()
    .Event<OnDamage>();

// Initialize the world
W.Initialize();
```

{: .important }
Type registration (`Types().Component<T>()`, `Types().Tag<T>()`, `Types().EntityType<T>(id)`) is only available in the `Created` state — after `Create()` and before `Initialize()`. Event registration (`Types().Event<T>()`) is also available after initialization.

___

#### Auto-registration of types:
Instead of manually registering each type, you can use automatic assembly scanning.
`RegisterAll()` discovers all structs implementing ECS interfaces and registers them automatically:

```csharp
W.Create(WorldConfig.Default());

// Auto-register all types from the calling assembly
W.Types().RegisterAll();

// Or specify particular assemblies
W.Types().RegisterAll(typeof(MyGame).Assembly, typeof(MyPlugin).Assembly);

// Can be combined with manual registration (e.g. to set serialization GUID)
W.Types()
    .RegisterAll()
    .Component<SpecialComponent>(new ComponentTypeConfig<SpecialComponent> { Guid = myGuid });

W.Initialize();
```

Detected interfaces:

| Interface | Registration |
|-----------|-------------|
| `IComponent` | `Types().Component<T>()` |
| `ITag` | `Types().Tag<T>()` |
| `IEvent` | `Types().Event<T>()` |
| `ILinkType` | Wrapped in `Link<T>` and registered as a component |
| `ILinksType` | Wrapped in `Links<T>` and registered as a component |
| `IMultiComponent` | Wrapped in `Multi<T>` and registered as a component |
| `IEntityType` | `Types().EntityType<T>(T.Id)` |

{: .note }
- If no assemblies are specified, only the calling assembly is scanned (not all loaded assemblies)
- The StaticEcs framework assembly itself is always excluded from scanning
- `RegisterAll()` searches for a static field or property of the matching config type inside each struct and uses it if found. Otherwise, default configuration is used. Lookup rules:
  - `IComponent` — looks for `ComponentTypeConfig<T>` (prefers name `Config`)
  - `IEvent` — looks for `EventTypeConfig<T>` (prefers name `Config`)
  - `ITag` — looks for `TagTypeConfig<T>` (prefers name `Config`)
  - `IEntityType` — looks for `byte` (prefers name `Id`)
- Both fields and properties are supported
- A struct implementing multiple interfaces (e.g. both `IComponent` and `IMultiComponent`) will be registered for each one

___

#### Initialization:
```csharp
// Standard initialization (baseEntitiesCapacity — initial entity capacity)
W.Initialize(baseEntitiesCapacity: 4096);

// Initialize with restored entity identifiers (EntityGID versions)
W.InitializeFromGIDStoreSnapshot(snapshot);

// Initialize with full world restoration from a snapshot
W.InitializeFromWorldSnapshot(snapshot);
```

{: .note }
`InitializeFromGIDStoreSnapshot` restores only entity identifier metadata (GID versions). `InitializeFromWorldSnapshot` restores the full world state, including all entities and their data.

___

#### Destruction:
```csharp
// Destroy the world and release all resources
W.Destroy();
```

___

## Basic operations

```csharp
// Current world status
WorldStatus status = W.Status;

// true if the world is initialized
bool initialized = W.IsWorldInitialized;

// true if the world is independent
bool independent = W.IsIndependent;

// Entity count in the world (active + unloaded)
uint entitiesCount = W.CalculateEntitiesCount();

// Loaded entity count
uint loadedCount = W.CalculateLoadedEntitiesCount();

// Current entity capacity
uint capacity = W.CalculateEntitiesCapacity();

// Destroy all entities in the world (world remains initialized)
W.DestroyAllLoadedEntities();
```

___

For details on creating entities and entity operations — see [Entity](entity).

For details on world resources — see [Resources](resources).

___

## Cluster

A cluster is a group of entity chunks for spatial segmentation of the world. Entities in the same cluster are grouped together and stored in memory in a segmented manner.
- Represented as a `ushort` value (0–65535)
- By default, cluster 0 is created on world initialization
- All entities are created in cluster 0 by default
- A cluster can be disabled — entities from disabled clusters are excluded from iteration

{: .note }
Clusters are designed for **spatial grouping**: levels, map zones, game rooms. For **logical** grouping (units, bullets, effects) use `entityType`.

___

#### Basic operations:
```csharp
// Register clusters (can be called after Create() or after Initialize())
const ushort LEVEL_1_CLUSTER = 1;
const ushort LEVEL_2_CLUSTER = 2;
W.RegisterCluster(LEVEL_1_CLUSTER);
W.RegisterCluster(LEVEL_2_CLUSTER);

// Check if a cluster is registered
bool registered = W.ClusterIsRegistered(LEVEL_1_CLUSTER);

// Enable or disable a cluster — entities from disabled clusters are excluded from iteration
W.SetActiveCluster(LEVEL_2_CLUSTER, false);

// Check if a cluster is active
bool active = W.ClusterIsActive(LEVEL_2_CLUSTER);

// Destroy all entities in a cluster
W.DestroyAllEntitiesInCluster(LEVEL_1_CLUSTER);

// Free a cluster — all entities are deleted, chunks and the identifier are released
W.FreeCluster(LEVEL_2_CLUSTER);

// Safe free — returns false if the cluster is not registered
bool freed = W.TryFreeCluster(LEVEL_2_CLUSTER);
```

___

#### Cluster snapshots and unloading:
```csharp
// Create a cluster snapshot (stores all entity data)
// Overloads available for writing to disk, compression, etc.
byte[] snapshot = W.Serializer.CreateClusterSnapshot(LEVEL_1_CLUSTER);

// Unload a cluster from memory
// Component and tag data is removed, entities are marked as unloaded
// Only identifier information is preserved, entities are excluded from queries
ReadOnlySpan<ushort> clusters = stackalloc ushort[] { LEVEL_1_CLUSTER };
W.Query().BatchUnload(EntityStatusType.Any, clusters: clusters);

// Load a cluster from a snapshot
W.Serializer.LoadClusterSnapshot(snapshot);
```

___

#### Cluster chunks:
```csharp
// Get all chunks in a cluster (including empty ones)
ReadOnlySpan<uint> chunks = W.GetClusterChunks(LEVEL_1_CLUSTER);

// Get chunks that have at least one loaded entity
ReadOnlySpan<uint> loadedChunks = W.GetClusterLoadedChunks(LEVEL_1_CLUSTER);
```

___

#### Creating entities in a cluster:
```csharp
// Specify a cluster when creating an entity (default — cluster 0)
struct UnitType : IEntityType { }
var entity = W.NewEntity<UnitType>(clusterId: LEVEL_1_CLUSTER);

// The clusterId parameter is available in all overloads
W.NewEntity<UnitType>(
    new UnitType(),  // entity type instance (can carry config data for OnCreate)
    clusterId: LEVEL_1_CLUSTER
);

// Get the entity's cluster
ushort entityClusterId = entity.ClusterId;

// Get the cluster from EntityGID
ushort gidClusterId = entity.GID.ClusterId;
```

___

## Chunk

A chunk is a block of 4096 entities. The entire world consists of chunks. Each chunk belongs to a cluster.

- **Independent world** (`Independent = true`) — manages chunks automatically, creates new ones as needed
- **Dependent world** (`Independent = false`) — has no chunks available for entity creation via `NewEntity()`, chunks must be explicitly assigned

___

#### Basic operations:
```csharp
// Find a free chunk not belonging to any cluster
// Independent world: if none available — creates a new one
// Dependent world: if none available — error
EntitiesChunkInfo chunkInfo = W.FindNextSelfFreeChunk();
uint chunkIdx = chunkInfo.ChunkIdx;
// chunkInfo.EntitiesFrom — first entity identifier in the chunk
// chunkInfo.EntitiesCapacity — chunk size (always 4096)

// Safe variant (returns false if no free chunks)
bool found = W.TryFindNextSelfFreeChunk(out EntitiesChunkInfo info);

// Register a chunk in a cluster
W.RegisterChunk(chunkIdx, clusterId: LEVEL_1_CLUSTER);

// Register a chunk with a specific ownership type
W.RegisterChunk(chunkIdx, owner: ChunkOwnerType.Self, clusterId: LEVEL_1_CLUSTER);

// Safe registration (returns false if the chunk is already registered)
bool registered = W.TryRegisterChunk(chunkIdx, clusterId: LEVEL_1_CLUSTER);

// Check if a chunk is registered
bool isRegistered = W.ChunkIsRegistered(chunkIdx);

// Get the cluster a chunk belongs to
ushort clusterId = W.GetChunkClusterId(chunkIdx);

// Move a chunk to another cluster
W.ChangeChunkCluster(chunkIdx, LEVEL_2_CLUSTER);

// Check for entities in a chunk
bool hasEntities = W.HasEntitiesInChunk(chunkIdx);           // active + unloaded
bool hasLoaded = W.HasLoadedEntitiesInChunk(chunkIdx);       // loaded only

// Destroy all entities in a chunk
W.DestroyAllEntitiesInChunk(chunkIdx);

// Free a chunk — all entities are deleted, the identifier is released
W.FreeChunk(chunkIdx);
```

___

#### Chunk snapshots and unloading:
```csharp
// Create a chunk snapshot
byte[] snapshot = W.Serializer.CreateChunkSnapshot(chunkIdx);

// Unload a chunk from memory (data removed, entities marked as unloaded)
ReadOnlySpan<uint> chunks = stackalloc uint[] { chunkIdx };
W.Query().BatchUnload(EntityStatusType.Any, chunks);

// Load a chunk from a snapshot
W.Serializer.LoadChunkSnapshot(snapshot);
```

___

#### Creating entities in a specific chunk:
```csharp
// Create an entity in a specific chunk
struct UnitType : IEntityType { }
var entity = W.NewEntityInChunk<UnitType>(chunkIdx: chunkIdx);

// Safe variant (returns false if the chunk is full)
bool created = W.TryNewEntityInChunk<UnitType>(out var entity, chunkIdx: chunkIdx);

// Non-generic variant (entity type known at runtime as byte)
byte entityTypeId = EntityTypeInfo<UnitType>.Id;
var entity = W.NewEntityInChunk(entityTypeId, chunkIdx: chunkIdx);
```

___

## Chunk ownership (ChunkOwnerType)

The ownership type determines how the world uses a chunk for entity creation:

- **`ChunkOwnerType.Self`** — chunk is managed by this world. Entities created via `NewEntity()` are placed in these chunks
  - Independent worlds have all chunks with `Self` ownership by default
- **`ChunkOwnerType.Other`** — chunk is not managed by this world. `NewEntity()` will never place entities in these chunks
  - Dependent worlds have all chunks with `Other` ownership by default

```csharp
// Get the chunk's ownership type
ChunkOwnerType owner = W.GetChunkOwner(chunkIdx);

// Change ownership
// Self → Other: chunk becomes unavailable for NewEntity()
// Other → Self: chunk becomes available for NewEntity()
W.ChangeChunkOwner(chunkIdx, ChunkOwnerType.Other);
```

{: .important }
Entity creation via `NewEntityByGID<TEntityType>(gid)` is only available for chunks with `Other` ownership.
Entity creation via `NewEntityInChunk<TEntityType>(chunkIdx)` is only available for chunks with `Self` ownership.

___

#### Client-server example:

```csharp
// === Server side (Independent world) ===
// Find a free chunk and register with Other ownership
// The server will not create its own entities in this identifier range
EntitiesChunkInfo chunkInfo = WServer.FindNextSelfFreeChunk();
WServer.RegisterChunk(chunkInfo.ChunkIdx, ChunkOwnerType.Other);
// Send the chunk identifier to the client

// === Client side (Dependent world) ===
// Receive the chunk identifier from the server
// Register with Self ownership — now 4096 entity slots are available
WClient.RegisterChunk(chunkIdxFromServer, ChunkOwnerType.Self);

// The client can create entities via NewEntity()
// For example, for UI or VFX
var vfx = WClient.NewEntity<VfxType>();

// Similarly works for P2P:
// one Independent host + N Dependent clients
```

___

## Cluster and chunk usage examples

#### Clusters:
- **Levels and map zones** — different clusters for different parts of the game world. As the player moves, clusters can be loaded and unloaded to save memory
- **Game levels** — load/unload clusters when changing levels
- **Game sessions** — cluster identifier defines a session. Combined with parallel iteration, multi-world emulation within a single world is possible

#### Chunks:
- **World streaming** — loading and unloading chunks during gameplay
- **Custom identifier management** — control over EntityGID distribution
- **Arena memory** — fast allocation and cleanup of large numbers of temporary entities

#### Chunk ownership:
- **Client-server interaction** — server allocates identifier ranges to clients
- **P2P network formats** — one Independent host and N Dependent clients
