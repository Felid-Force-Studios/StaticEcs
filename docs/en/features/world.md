---
title: World
parent: Features
nav_order: 9
---

## WorldType
World identifier type-tag, used to isolate static data when creating different worlds in the same process
- Represented as a user structure without data with a marker interface `IWorldType`

#### Example:
```c#
public struct MainWorldType : IWorldType { }
public struct MiniGameWorldType : IWorldType { }
```
___


## World
Library entry point responsible for accessing, creating, initializing, operating, and destroying world data
- Represented as a static class `World<T>` parameterized by `IWorldType`

{: .important }
> Since the type-identifier `IWorldType` defines access to a specific world   
> There are three ways to work with the framework:

___

#### The first way is as is via full address (very inconvenient):
```c#
public struct WT : IWorldType { }

World<WT>.Create(WorldConfig.Default());
World<WT>.CalculateEntitiesCount();

var entity = World<WT>.Entity.New<Position>();
```

#### The second way is a little more convenient, use static imports or static aliases (you'll have to write in each file)
```c#
using static FFS.Libraries.StaticEcs.World<WT>;

public struct WT : IWorldType { }

Create(WorldConfig.Default());
CalculateEntitiesCount();

var entity = Entity.New<Position>();
```

#### The third way is the most convenient, use type-aliases in the root namespace (no need to write in every file)
This is the method that will be used everywhere in the examples
```c#
public struct WT : IWorldType { }

public abstract class W : World<WT> { }

W.Create(WorldConfig.Default());
W.CalculateEntitiesCount();

var entity = W.Entity.New<Position>();
```

___

#### Basic operations:
```c#
// Defining the world ID
public struct WT : IWorldType { }

// Register types - aliases
public abstract class World : World<WT> { }

// Creating a world with a default configuration
W.Create(WorldConfig.Default());
// Or a custom one
W.Create(new() {
            // Indicates whether the world is independent or dependent (see the "Chunk" section for more details).
            Independent = true   
            // Base size of all component types (number of component types)
            BaseComponentTypesCount = 64                        
            // Base size of all varieties of tag types (number of tag types)
            BaseTagTypesCount = 64,                             
            // Operation mode of multithreaded processing 
            // (Disabled - no threads are created, MaxThreadsCount - maximum available number of threads is created, CustomThreadsCount - specified number of threads)
            ParallelQueryType = ParallelQueryType.Disabled,
            // Number of threads at ParallelQueryType.CustomThreadsCount
            CustomThreadCount = 4,
            // Strict Query default mode of operation, optionally in the Queries section
            DefaultQueryModeStrict = true
        });

W.Entity.    // Entity access for MainWorldType (world ID)
W.Context.   // Access to context for MainWorldType (world ID)
W.Components.// Access to components for MainWorldType (world ID)
W.Tags.      // Access to tags for MainWorldType (world ID)
W.Events.    // Access to events

// Initialization of the world
W.Initialize(baseEntitiesCapacity = 4096);
// Initialize the world by loading previously saved identifiers
W.InitializeFromGIDStoreSnapshot(snapshot);
// Initializing the world from saved data
W.InitializeFromWorldSnapshot(snapshot);

// Destroying and deleting the world's data
W.Destroy();

// true if the world is initialized
bool initialized = W.IsInitialized();

// true if the world is independent
bool independent = W.IsIndependent();

// number of entities created in the world (active + unloaded)
int entitiesCount = W.CalculateEntitiesCount();

// number of entities loaded in the world
int loadedEntitiesCount = W.CalculateLoadedEntitiesCount();

// current capacity for entities
int entitiesCapacity = W.CalculateEntitiesCapacity();

// Destroys all entities in the world
W.DestroyAllEntities();
```

___

## Cluster:
A cluster is a set of entity chunks. Entities belonging to the same cluster are grouped together and stored in memory in a segmented manner.  
A cluster is represented by a ushort value between 0 and 65535. By default, when the world is initialized, a single cluster with the identifier 0 is created, and all entities are created in it by default.  

___

#### Main operations:
```c#
// Cluster registration can be called after creation or after world initialization.
const ushort NPC_CLUSTER = 1;
const ushort ENVIRONMENT_CLUSTER = 2;
W.RegisterCluster(NPC_CLUSTER);
W.RegisterCluster(ENVIRONMENT_CLUSTER);

// Check if the cluster is registered
bool clusterIsRegistered = W.ClusterIsRegistered(NPC_CLUSTER);

// Enable or disable a cluster; entities from disabled clusters are not included in the iteration.
W.SetActiveCluster(ENVIRONMENT_CLUSTER, false);

// Check if the cluster is enabled
bool active = W.ClusterIsActive(ENVIRONMENT_CLUSTER);

// Release the cluster; all entities in the cluster will be deleted, all chunks and the cluster ID will be released (an error will occur if the cluster is not registered).
W.FreeCluster(ENVIRONMENT_CLUSTER);

// Release the cluster if it is registered
bool free = W.TryFreeCluster(ENVIRONMENT_CLUSTER);

// Destroy all entities in the cluster
W.DestroyAllEntitiesInCluster(NPC_CLUSTER);

// Take a snapshot of the cluster that stores all entity data in this cluster.
// There are method overloads for writing to disk, compression, etc.
// More examples in the "serialization" section.
byte[] clusterSnapshot = W.Serializer.CreateClusterSnapshot(NPC_CLUSTER);

// Unload the cluster from memory, all component and tag chunks will be deleted,
// entities will be marked as unloaded, and only information about identifiers will be saved; entities will not be received in queries.
W.UnloadCluster(NPC_CLUSTER);

// Load from the snapshot of the entity cluster into the world
W.Serializer.LoadClusterSnapshot(clusterSnapshot);

// Get all chunks in the cluster (including empty chunks where there are no loaded entities)
ReadOnlySpan<uint> chunks = W.GetClusterChunks(NPC_CLUSTER);

// Get all chunks in the cluster that have at least one entity loaded
ReadOnlySpan<uint> loadedChunks = W.GetClusterLoadedChunks(NPC_CLUSTER);

// When creating an entity, you can pass the cluster ID (by default, the entity is created in the default cluster W.DEFAULT_CLUSTER = 0).
var npc = W.Entity.New(clusterId: W.DEFAULT_CLUSTER);

// Attempt to create an entity in the cluster; if the world is dependent and there are no free entity identifiers left in it, false will be returned.
var created = W.Entity.TryNew(out var ent, clusterId: ENVIRONMENT_CLUSTER);

// An optional cluster identifier parameter has been added for all overloads.
W.Entity.New(
    new Position(),
    new Name(),
    clusterId: NPC_CLUSTER
);

// Get entity cluster
ushort entityClusterId = npc.ClusterId();

// Get the entity cluster at EntityGID
ushort gidClusterId = npc.Gid().ClusterId;
```

___

## Chunk:
A chunk is a group of entities with a size of 4096; the entire world consists of chunks. A chunk always belongs to a cluster.  
A world can be dependent or independent; this parameter is set in the world configuration when creating `W.Create(new() { Independent = true })`.  
By default, an independent world manages entity IDs and all chunks automatically, creating new chunks when entities are created with `W.Entity.New()` when needed.   
A dependent world does not have entity and chunk IDs available for creating entities via `W.Entity.New()` when created; the world must be told which chunks are available.  
Next, we will look at some examples.  


___

#### Basic operations:
```c#
// Find a free chunk that does not belong to any cluster.
// For an independent world, if there is no free chunk, a new one will be created.
// For a dependent world, if there is no free chunk, an error will occur.
EntitiesChunkInfo chunkInfo = W.FindNextSelfFreeChunk();
uint chunkIdx = chunkInfo.ChunkIdx; // Chunk index
// chunkInfo.EntitiesFrom - first entity identifier in the cluster
// chunkInfo.EntitiesCapacity - chunk size (always 4096)

// Try to find a free chunk that does not belong to any cluster.
// For an independent world, if there is no free chunk, a new one will be created (the result is always true).
// For a dependent world, if there is no free chunk, the result will be false.
bool hasFreeChunk = W.TryFindNextSelfFreeChunk(out EntitiesChunkInfo info);

// Register a free chunk in the cluster (if the chunk is already registered, an error will occur)
W.RegisterChunk(chunkIdx, clusterId: NPC_CLUSTER);
// Register a free chunk in the cluster and assign an ownership type (details below) (If the chunk is already registered, an error will occur)
W.RegisterChunk(chunkIdx, owner: ChunkOwnerType.Self, clusterId: NPC_CLUSTER);

// Attempt to register a free chunk in the cluster (if the chunk is already registered, false will be returned)
bool chunkRegistered = W.TryRegisterChunk(chunkIdx, NPC_CLUSTER);

// Check if the chunk is registered
bool registered = W.ChunkIsRegistered(chunkIdx);

// Get the ID of the cluster to which the chunk belongs
ushort chunkClusterId = W.GetChunkClusterId(chunkIdx);

// Change the chunk cluster; all entities within the chunk will belong to another cluster.
W.ChangeChunkCluster(chunkIdx, ENVIRONMENT_CLUSTER);

// Check if there are entities in the chunk (active + unloaded)
bool hasEntitiesInChunk = W.HasEntitiesInChunk(chunkIdx);

// Check if there are any loaded entities in the chunk
bool hasLoadedEntitiesInChunk = W.HasLoadedEntitiesInChunk(chunkIdx);

// Free up the chunk; all entities in the chunk will be deleted, and the chunk identifier will be freed up.
W.FreeChunk(chunkIdx);

// Destroy all entities in the chunk
W.DestroyAllEntitiesInChunk(chunkIdx);

// Take a snapshot of the chunk that stores all entity data in this chunk.
// There are method overloads for writing to disk, compression, etc.
// More examples in the "serialization" section.
byte[] chunkSnapshot = W.Serializer.CreateChunkSnapshot(chunkIdx);

// Unload the chunk from memory, all components and tags will be removed,
// entities will be marked as unloaded, and only information about identifiers will be saved; entities will not be received in queries.
W.UnloadChunk(chunkIdx);

// Load from the snapshot of the essence into the world
W.Serializer.LoadChunkSnapshot(chunkSnapshot);

// When creating an entity, you can pass the chunk index (without specifying it; the chunk selection is determined by the world).
var entity = W.Entity.New(chunkIdx: chunkIdx);

// Попытаться создать сущность в чанке, если чанк полон вернется false
var created = W.Entity.TryNew(out var ent, chunkIdx: chunkIdx);


// Check the chunk owner
// ChunkOwnerType.Self means that the chunk is managed by this world; only chunks with Self ownership are used to create entities via Entity.New()
//    - The independent world by default has all chunks with Self ownership.
// ChunkOwnerType.Other - means that the chunk is not managed by this world, entities created via Entity.New() will never be created in these chunks.
//    - The dependent world by default has all chunks with Other ownership.
ChunkOwnerType owner = W.GetChunkOwner(chunkIdx);

// Change the type of ownership of a chunk
W.ChangeChunkOwner(chunkIdx, ChunkOwnerType.Other);
 
// Creating entities via Entity.New(gid) is only available for chunks with `Other` ownership type.
// Creating entities via Entity.New(chunkIdx) is only available for chunks with the Self ownership type.
```

___

## Examples of cluster and chunk usage:

Clusters can be used for any user logic, for example:
- Different clusters can define different types of entities, such as a unit cluster, a game environment cluster, an item cluster, or an effect cluster.
    - This reduces memory consumption and fragmentation, speeds up iteration, and helps with world serialization and game logic.
    - For example, with a large game map that is loaded and unloaded as the player moves, different clusters greatly save memory.
- Another example is using clusters for different game levels and loading/unloading clusters when changing levels.
- The cluster ID can also define the game session. Combined with parallel iteration, it is possible to create a multi-world emulation within a single world.


Chunk management can be used, for example, for:
- World streaming, allowing chunks to be loaded and unloaded during gameplay
- Custom entity ID management
- Quick selection and clearing of large numbers of entities, as an arena memory for temporary entities

Chunk ownership management can be used for client-server interactions, for example:

```c#
// On the server side in the Independent world
// Find a free chunk and register it
EntitiesChunkInfo chunkInfo = WServer.FindNextSelfFreeChunk();
// Set the ownership type of the chunk to `Other`, so that the server will never create entities in this range of identifiers.
WServer.RegisterChunk(chunkInfo.ChunkIdx, ChunkOwnerType.Other);

// Send the chunk ID to the client

// On the client side in the Dependent world
// We receive the chunk ID from the server
// Register a chunk with the `Self` ownership type
WClient.RegisterChunk(ChunkIdxFromServer, ChunkOwnerType.Self);

// Now there are 4096 free identifiers available for entities on the client.
// Client entities can be created using W.Entity.New().
// For example, for UI or VFX.

// Similarly, it can be used for p2p network formats
// where there is one Independent host and N Dependent clients
```