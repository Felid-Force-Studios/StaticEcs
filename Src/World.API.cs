#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    /// <summary>
    /// Marker interface that identifies a world type. Implement this on an empty struct to define
    /// a unique world identity. Each distinct <c>TWorld</c> type creates a completely isolated
    /// static ECS world with its own entities, components, tags, events, and resources.
    /// <para>
    /// Example: <c>public struct MyGameWorld : IWorldType { }</c> then use <c>World&lt;MyGameWorld&gt;</c>
    /// to access all ECS operations for that world.
    /// </para>
    /// </summary>
    public interface IWorldType { }

    /// <summary>
    /// The central static ECS world. All entity, component, tag, event, and resource data for a given
    /// <typeparamref name="TWorld"/> is stored in static fields, providing zero-indirection access
    /// with no per-instance overhead.
    /// <para>
    /// <b>Lifecycle:</b> A world must go through three states in order:
    /// <list type="number">
    /// <item><see cref="Create"/> — allocates internal structures, registers component/tag/event types.</item>
    /// <item><see cref="Initialize"/> (or <c>InitializeFrom*Snapshot</c>) — allocates entity storage, transitions to <see cref="WorldStatus.Initialized"/>.</item>
    /// <item><see cref="Destroy"/> — destroys all entities, frees all data, returns to <see cref="WorldStatus.NotCreated"/>.</item>
    /// </list>
    /// </para>
    /// <para>
    /// <b>Thread safety:</b> Most mutating operations assert that multi-threading is not active
    /// (in debug builds). Use the query system's parallel runner for safe concurrent reads.
    /// </para>
    /// <para>
    /// This is an abstract partial class — the remaining parts (Entity, Components, Tags, Events,
    /// Queries, Serializer, etc.) are defined in other <c>World.*.cs</c> files.
    /// </para>
    /// </summary>
    /// <typeparam name="TWorld">
    /// A struct implementing <see cref="IWorldType"/> that uniquely identifies this world.
    /// Different TWorld types produce completely independent static worlds.
    /// </typeparam>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    [Il2CppEagerStaticClassConstruction]
    #endif
    public abstract partial class World<TWorld> where TWorld : struct, IWorldType {

        #region BASE
        /// <summary>
        /// Whether the world has been fully initialized and is ready for entity creation,
        /// queries, and gameplay logic. Returns <c>true</c> only in <see cref="WorldStatus.Initialized"/> state.
        /// <para>
        /// A world that is <see cref="WorldStatus.Created"/> but not yet initialized cannot create entities —
        /// it is only ready for type registration (via <c>Types().Component&lt;T&gt;()</c>, etc.).
        /// </para>
        /// </summary>
        public static bool IsWorldInitialized {
            [MethodImpl(AggressiveInlining)]
            get => Data.Instance.WorldStatus == WorldStatus.Initialized;
        }

        /// <summary>
        /// Whether this world was configured as independent (see <see cref="WorldConfig.Independent"/>).
        /// Independent worlds manage their own chunk allocation and do not share entity ID space
        /// with other worlds. Non-independent worlds can share chunks across multiple world instances
        /// for advanced streaming or multi-world scenarios.
        /// </summary>
        public static bool IsIndependent {
            [MethodImpl(AggressiveInlining)]
            get => Data.Instance.IndependentWorld;
        }

        /// <summary>
        /// Current lifecycle state of the world: <see cref="WorldStatus.NotCreated"/>,
        /// <see cref="WorldStatus.Created"/>, or <see cref="WorldStatus.Initialized"/>.
        /// </summary>
        public static WorldStatus Status {
            [MethodImpl(AggressiveInlining)]
            get => Data.Instance.WorldStatus;
        }

        /// <summary>
        /// First step of world lifecycle: allocates internal data structures according to
        /// <paramref name="worldConfig"/>. After this call, the world is in <see cref="WorldStatus.Created"/>
        /// state and you can register component, tag, and event types. You must call
        /// <see cref="Initialize"/> (or an <c>InitializeFrom*Snapshot</c> overload) before creating entities.
        /// <para>
        /// Call this exactly once. In debug builds, throws if the world is already created.
        /// </para>
        /// </summary>
        /// <param name="worldConfig">
        /// Configuration controlling initial capacities, parallel query threading,
        /// cluster count, and independence mode.
        /// All parameters are optional - any unset value falls back to <see cref="WorldConfig.Default"/>.
        /// </param>
        public static void Create(WorldConfig worldConfig = default) {
            #if FFS_ECS_DEBUG
            AssertWorldIsNotCreated(WorldTypeName);
            #endif

            Data.Instance = new Data(worldConfig);
            Data.Handle = WorldHandle.Create<TWorld>();
            Serializer.Create();
        }

        /// <summary>
        /// Second step of world lifecycle: allocates entity storage and transitions the world to
        /// <see cref="WorldStatus.Initialized"/>. After this call, you can create entities,
        /// run queries, and perform all ECS operations.
        /// <para>
        /// Must be called after <see cref="Create"/> and after all type registrations are complete.
        /// </para>
        /// </summary>
        /// <param name="baseEntitiesCapacity">
        /// Initial entity capacity, rounded up to the nearest chunk boundary (multiples of 4096).
        /// Default is 4 chunks = 16,384 entities. The world grows automatically beyond this,
        /// but setting an appropriate initial size avoids early reallocations.
        /// </param>
        public static void Initialize(uint baseEntitiesCapacity = Const.ENTITIES_IN_CHUNK * 4) {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif
            baseEntitiesCapacity = baseEntitiesCapacity.Normalize(Const.ENTITIES_IN_CHUNK);

            var chunksCapacity = baseEntitiesCapacity >> Const.ENTITIES_IN_CHUNK_SHIFT;
            Data.Instance.InitializeInternal(chunksCapacity);
            Data.Instance.RegisterClusterInternal(default);
        }

        /// <summary>
        /// Initializes the world by restoring GID (Global ID) metadata from a binary snapshot.
        /// This restores the entity slot versions and cluster assignments so that previously
        /// serialized <see cref="EntityGID"/> references can be resolved correctly.
        /// The world transitions to <see cref="WorldStatus.Initialized"/> upon completion.
        /// <para>
        /// Use this when you need to reconstruct the entity ID mapping table without loading
        /// full entity/component data.
        /// </para>
        /// </summary>
        /// <param name="reader">A <see cref="BinaryPackReader"/> positioned at the start of the GID store snapshot data.</param>
        public static void InitializeFromGIDStoreSnapshot(BinaryPackReader reader) {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            Serializer.RestoreFromGIDStoreSnapshot(reader);
            Data.Instance.WorldStatus = WorldStatus.Initialized;
        }

        /// <summary>
        /// Initializes the world by restoring GID metadata from a byte array snapshot.
        /// See <see cref="InitializeFromGIDStoreSnapshot(BinaryPackReader)"/> for semantics.
        /// </summary>
        /// <param name="snapshot">Byte array containing the serialized GID store data.</param>
        /// <param name="gzip">If <c>true</c>, the data is gzip-compressed and will be decompressed before reading.</param>
        public static void InitializeFromGIDStoreSnapshot(byte[] snapshot, bool gzip = false) {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            Serializer.RestoreFromGIDStoreSnapshot(snapshot, gzip);
            Data.Instance.WorldStatus = WorldStatus.Initialized;
        }

        /// <summary>
        /// Initializes the world by restoring GID metadata from a file on disk.
        /// See <see cref="InitializeFromGIDStoreSnapshot(BinaryPackReader)"/> for semantics.
        /// </summary>
        /// <param name="worldSnapshotFilePath">Path to the snapshot file.</param>
        /// <param name="gzip">If <c>true</c>, the file is gzip-compressed.</param>
        /// <param name="byteSizeHint">Optional hint for the initial read buffer size (0 = auto).</param>
        public static void InitializeFromGIDStoreSnapshot(string worldSnapshotFilePath, bool gzip = false, uint byteSizeHint = 0) {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            Serializer.RestoreFromGIDStoreSnapshot(worldSnapshotFilePath, gzip, byteSizeHint);
            Data.Instance.WorldStatus = WorldStatus.Initialized;
        }

        /// <summary>
        /// Initializes the world by loading a full world snapshot (entities, components, tags,
        /// and all associated data) from a binary reader. This is a complete restore — after this
        /// call the world is in the exact state it was when the snapshot was taken.
        /// The world transitions to <see cref="WorldStatus.Initialized"/> upon completion.
        /// <para>
        /// Use this for save/load functionality where you need to restore the entire game state.
        /// </para>
        /// </summary>
        /// <param name="reader">A <see cref="BinaryPackReader"/> positioned at the start of the world snapshot data.</param>
        public static void InitializeFromWorldSnapshot(BinaryPackReader reader) {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            Serializer.LoadWorldSnapshot(reader);
            Data.Instance.WorldStatus = WorldStatus.Initialized;
        }

        /// <summary>
        /// Initializes the world by loading a full world snapshot from a byte array.
        /// See <see cref="InitializeFromWorldSnapshot(BinaryPackReader)"/> for semantics.
        /// </summary>
        /// <param name="snapshot">Byte array containing the serialized world data.</param>
        /// <param name="gzip">If <c>true</c>, the data is gzip-compressed and will be decompressed before reading.</param>
        public static void InitializeFromWorldSnapshot(byte[] snapshot, bool gzip = false) {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            Serializer.LoadWorldSnapshot(snapshot, gzip);
            Data.Instance.WorldStatus = WorldStatus.Initialized;
        }

        /// <summary>
        /// Initializes the world by loading a full world snapshot from a file on disk.
        /// See <see cref="InitializeFromWorldSnapshot(BinaryPackReader)"/> for semantics.
        /// </summary>
        /// <param name="worldSnapshotFilePath">Path to the snapshot file.</param>
        /// <param name="gzip">If <c>true</c>, the file is gzip-compressed.</param>
        /// <param name="byteSizeHint">Optional hint for the initial read buffer size (0 = auto).</param>
        public static void InitializeFromWorldSnapshot(string worldSnapshotFilePath, bool gzip = false, uint byteSizeHint = 0) {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            Serializer.LoadWorldSnapshot(worldSnapshotFilePath, gzip, byteSizeHint);
            Data.Instance.WorldStatus = WorldStatus.Initialized;
        }

        /// <summary>
        /// Final step of world lifecycle: frees all internal storage, clears resources, and resets the world to
        /// <see cref="WorldStatus.NotCreated"/>. After this call, the world can be re-created
        /// with a new <see cref="Create"/> call.
        /// <para>
        /// Can be called in both <see cref="WorldStatus.Created"/> and <see cref="WorldStatus.Initialized"/> states.
        /// In <see cref="WorldStatus.Created"/> state, skips entity destruction (no entities exist yet)
        /// and only cleans up type registrations and resources.
        /// </para>
        /// <para>
        /// This is a destructive operation — all entity handles, GIDs, and component references
        /// become invalid. Ensure all systems have stopped before calling.
        /// </para>
        /// </summary>
        /// <param name="withHooks">
        /// When <c>true</c>, gracefully destroys all entities first, calling OnDelete hooks on every
        /// component and OnDestroy on every entity type — use when user cleanup logic must run.
        /// When <c>false</c>, all storage is zeroed and freed directly without any hook invocations.
        /// </param>
        public static void Destroy(bool withHooks = true) {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreatedOrInitialized(WorldTypeName);
            #endif

            if (withHooks && Data.Instance.WorldStatus == WorldStatus.Initialized) {
                Query().BatchDestroyInternal(HookReason.WorldDestroy, EntityStatusType.Any, QueryMode.Flexible, withDisabledClusters: true);
            }

            Data.Instance.DestroyInternal();
            ResourcesData.Instance.Clear();
            NamedResources.Clear();
            Serializer.DestroySerializer();

            Data.Instance.WorldStatus = WorldStatus.NotCreated;
        }

        /// <summary>
        /// Hard-resets the world to its post-<see cref="Initialize"/> state without deallocating arrays.
        /// All entity data, component segments, tag segments, multi-component storage, and events are
        /// zeroed and returned to their respective pools. No hooks are called (OnDelete, OnDestroy, etc.).
        /// <para>
        /// The world remains in <see cref="WorldStatus.Initialized"/> state — type registrations,
        /// allocated capacity, and resources are preserved. New entities can be created immediately after.
        /// </para>
        /// <para>
        /// Use this for scenarios where maximum reset performance is required and hook execution is
        /// not needed (e.g., benchmark resets, level restarts where cleanup is handled externally).
        /// For a reset that respects component lifecycle hooks, use <see cref="DestroyAllLoadedEntities"/> instead.
        /// </para>
        /// </summary>
        public static void HardReset() {
            #if FFS_ECS_DEBUG
            AssertWorldIsInitialized(WorldTypeName);
            #endif
            Data.Instance.HardResetInternal();
            Data.Instance.ClearEvents();
            Data.Instance.RegisterClusterInternal(default);
        }

        /// <summary>
        /// Calculates the total number of entities currently alive in the world (both loaded and unloaded chunks).
        /// This is a computed value that scans the bitmap — not a cached counter.
        /// </summary>
        /// <returns>Total entity count across all chunks.</returns>
        [MethodImpl(AggressiveInlining)]
        public static uint CalculateEntitiesCount() => Data.Instance.CalculateEntitiesCountInternal();

        /// <summary>
        /// Calculates the number of entities in currently loaded chunks only.
        /// Entities in unloaded chunks are excluded from this count.
        /// Useful for knowing how many entities are actively in play during streaming scenarios.
        /// </summary>
        /// <returns>Entity count in loaded chunks.</returns>
        [MethodImpl(AggressiveInlining)]
        public static uint CalculateLoadedEntitiesCount() => Data.Instance.CalculateLoadedEntitiesCountInternal();

        /// <summary>
        /// Calculates the current total entity capacity (number of entity slots allocated across all chunks).
        /// This grows as new chunks are registered.
        /// </summary>
        /// <returns>Total allocated entity slot count.</returns>
        [MethodImpl(AggressiveInlining)]
        public static uint CalculateEntitiesCapacity() => Data.Instance.CalculateEntitiesCapacityInternal();

        /// <summary>
        /// Destroys every entity in the world, triggering all OnDelete component hooks.
        /// The world remains in <see cref="WorldStatus.Initialized"/> state — chunks and type
        /// registrations are preserved, so new entities can be created immediately after.
        /// <para>
        /// Use this for "soft reset" scenarios (e.g. restarting a game level) without the overhead
        /// of fully destroying and re-creating the world.
        /// </para>
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static void DestroyAllLoadedEntities() {
            Query().BatchDestroy(EntityStatusType.Any, QueryMode.Flexible, withDisabledClusters: true);
        }
        #endregion

        #region CLUSTERS AND CHUNKS
        /// <summary>
        /// Registers a cluster with the given ID. Clusters are logical groupings of chunks
        /// (e.g. entity types, spatial zones, gameplay regions, network authority domains). A cluster must be
        /// registered before entities can be created in it.
        /// <para>
        /// A default cluster (ID 0) is always registered automatically. Register additional
        /// clusters for entity types, streaming, spatial partitioning, or multi-authority scenarios.
        /// </para>
        /// </summary>
        /// <param name="clusterId">Unique cluster identifier.</param>
        [MethodImpl(AggressiveInlining)]
        public static void RegisterCluster(ushort clusterId) => Data.Instance.RegisterClusterInternal(clusterId);

        /// <summary>
        /// Checks whether a cluster with the given ID has been registered.
        /// </summary>
        /// <param name="clusterId">Cluster identifier to check.</param>
        /// <returns><c>true</c> if the cluster is registered.</returns>
        [MethodImpl(AggressiveInlining)]
        public static bool ClusterIsRegistered(ushort clusterId) => Data.Instance.ClusterIsRegisteredInternal(clusterId);

        /// <summary>
        /// Attempts to free a cluster and all its associated chunks. Returns <c>false</c> if the
        /// cluster not registered.
        /// On success, the cluster ID can be reused.
        /// </summary>
        /// <param name="clusterId">Cluster identifier to free.</param>
        /// <returns><c>true</c> if the cluster was freed; <c>false</c> if it still contains entities.</returns>
        [MethodImpl(AggressiveInlining)]
        public static bool TryFreeCluster(ushort clusterId) => Data.Instance.TryFreeClusterInternal(clusterId);

        /// <summary>
        /// Frees a cluster unconditionally. Unlike <see cref="TryFreeCluster"/>, this will throw
        /// in debug builds if the cluster not registered.
        /// </summary>
        /// <param name="clusterId">Cluster identifier to free.</param>
        [MethodImpl(AggressiveInlining)]
        public static void FreeCluster(ushort clusterId) => Data.Instance.FreeClusterInternal(clusterId);

        /// <summary>
        /// Activates or deactivates a cluster. Deactivated clusters are skipped during query
        /// iteration, effectively "pausing" all entities in that cluster without destroying them.
        /// </summary>
        /// <param name="clusterId">Cluster identifier.</param>
        /// <param name="active"><c>true</c> to activate; <c>false</c> to deactivate.</param>
        [MethodImpl(AggressiveInlining)]
        public static void SetActiveCluster(ushort clusterId, bool active) => Data.Instance.SetActiveClusterInternal(clusterId, active);

        /// <summary>
        /// Checks whether a cluster is currently active (participating in queries).
        /// </summary>
        /// <param name="clusterId">Cluster identifier.</param>
        /// <returns><c>true</c> if the cluster is active.</returns>
        [MethodImpl(AggressiveInlining)]
        public static bool ClusterIsActive(ushort clusterId) => Data.Instance.ClusterIsActiveInternal(clusterId);

        /// <summary>
        /// Destroys all entities in the specified cluster, triggering OnDelete hooks.
        /// The cluster itself and its chunks remain registered.
        /// </summary>
        /// <param name="clusterId">Cluster identifier.</param>
        [MethodImpl(AggressiveInlining)]
        public static void DestroyAllEntitiesInCluster(ushort clusterId) {
            ReadOnlySpan<ushort> clusters = stackalloc ushort[] { clusterId };
            Query().BatchDestroy(EntityStatusType.Any, QueryMode.Flexible, clusters);
        }

        /// <summary>
        /// Returns the chunk indices associated with a cluster (both loaded and unloaded).
        /// </summary>
        /// <param name="clusterId">Cluster identifier.</param>
        /// <returns>Read-only span of chunk indices belonging to this cluster.</returns>
        [MethodImpl(AggressiveInlining)]
        public static ReadOnlySpan<uint> GetClusterChunks(ushort clusterId) => Data.Instance.GetClusterChunksInternal(clusterId);

        /// <summary>
        /// Returns only the currently loaded chunk indices for a cluster.
        /// </summary>
        /// <param name="clusterId">Cluster identifier.</param>
        /// <returns>Read-only span of loaded chunk indices.</returns>
        [MethodImpl(AggressiveInlining)]
        public static ReadOnlySpan<uint> GetClusterLoadedChunks(ushort clusterId) => Data.Instance.GetClusterLoadedChunksInternal(clusterId);

        /// <summary>
        /// Searches for a free (unregistered) chunk that can be owned by this world (Self-owned).
        /// Returns <c>true</c> if a free chunk was found, along with its info.
        /// <para>
        /// Used internally when a cluster's existing chunks are full and a new chunk is needed
        /// for entity allocation. Can also be called manually for advanced chunk management.
        /// </para>
        /// </summary>
        /// <param name="chunkInfo">Information about the found free chunk (index, capacity, etc.).</param>
        /// <returns><c>true</c> if a free chunk is available.</returns>
        [MethodImpl(AggressiveInlining)]
        public static bool TryFindNextSelfFreeChunk(out EntitiesChunkInfo chunkInfo) => Data.Instance.TryFindNextSelfFreeChunkInternal(out chunkInfo);

        /// <summary>
        /// Finds the next free self-owned chunk. Throws if none are available.
        /// See <see cref="TryFindNextSelfFreeChunk"/> for a non-throwing variant.
        /// </summary>
        /// <returns>Information about the found free chunk.</returns>
        [MethodImpl(AggressiveInlining)]
        public static EntitiesChunkInfo FindNextSelfFreeChunk() => Data.Instance.FindNextSelfFreeChunkInternal();

        /// <summary>
        /// Attempts to register a specific chunk index for use. Returns <c>false</c> if the chunk
        /// is already registered. Chunks are the fundamental storage unit (4096 entity slots each).
        /// <para>
        /// In most cases, chunks are registered automatically when entities are created in a cluster.
        /// Use this for manual chunk management in advanced streaming scenarios.
        /// </para>
        /// </summary>
        /// <param name="chunkIdx">Zero-based chunk index to register.</param>
        /// <param name="owner">
        /// Ownership type: <see cref="ChunkOwnerType.Self"/> for locally-owned chunks,
        /// or another value for externally-managed chunks (e.g. from network).
        /// </param>
        /// <param name="clusterId">Cluster to assign the chunk to (default = cluster 0).</param>
        /// <returns><c>true</c> if the chunk was registered; <c>false</c> if already in use.</returns>
        [MethodImpl(AggressiveInlining)]
        public static bool TryRegisterChunk(uint chunkIdx, ChunkOwnerType owner = ChunkOwnerType.Self, ushort clusterId = default) => Data.Instance.TryRegisterChunkInternal(chunkIdx, owner, clusterId);

        /// <summary>
        /// Registers a chunk index unconditionally. Throws in debug builds if already registered.
        /// See <see cref="TryRegisterChunk"/> for a non-throwing variant.
        /// </summary>
        /// <param name="chunkIdx">Zero-based chunk index.</param>
        /// <param name="owner">Chunk ownership type.</param>
        /// <param name="clusterId">Cluster to assign the chunk to.</param>
        [MethodImpl(AggressiveInlining)]
        public static void RegisterChunk(uint chunkIdx, ChunkOwnerType owner = ChunkOwnerType.Self, ushort clusterId = default) => Data.Instance.RegisterChunkInternal(chunkIdx, owner, clusterId);

        /// <summary>
        /// Checks whether a chunk with the given index is currently registered.
        /// </summary>
        /// <param name="chunkIdx">Chunk index to check.</param>
        /// <returns><c>true</c> if the chunk is registered.</returns>
        [MethodImpl(AggressiveInlining)]
        public static bool ChunkIsRegistered(uint chunkIdx) => Data.Instance.ChunkIsRegisteredInternal(chunkIdx);

        /// <summary>
        /// Frees a chunk, making its index available for reuse. Destroys all entities within a specific chunk, triggering OnDelete hooks.
        /// All component/tag storage for this chunk is released.
        /// </summary>
        /// <param name="chunkIdx">Chunk index to free.</param>
        [MethodImpl(AggressiveInlining)]
        public static void FreeChunk(uint chunkIdx) => Data.Instance.FreeChunkInternal(chunkIdx);

        /// <summary>
        /// Destroys all entities within a specific chunk, triggering OnDelete hooks.
        /// The chunk itself remains registered and can accept new entities.
        /// </summary>
        /// <param name="chunkIdx">Chunk index.</param>
        [MethodImpl(AggressiveInlining)]
        public static void DestroyAllEntitiesInChunk(uint chunkIdx) {
            ReadOnlySpan<uint> chunks = stackalloc uint[] { chunkIdx };
            Query().BatchDestroy(chunks, EntityStatusType.Any, QueryMode.Flexible);
        }

        /// <summary>
        /// Returns the cluster ID that a chunk belongs to.
        /// </summary>
        /// <param name="chunkIdx">Chunk index.</param>
        /// <returns>Cluster identifier that owns this chunk.</returns>
        [MethodImpl(AggressiveInlining)]
        public static ushort GetChunkClusterId(uint chunkIdx) => Data.Instance.GetChunkClusterIdInternal(chunkIdx);

        /// <summary>
        /// Moves a chunk from its current cluster to a different cluster.
        /// The chunk's entities are not modified — only the cluster assignment changes.
        /// </summary>
        /// <param name="chunkIdx">Chunk index to reassign.</param>
        /// <param name="clusterId">Target cluster identifier.</param>
        [MethodImpl(AggressiveInlining)]
        public static void ChangeChunkCluster(uint chunkIdx, ushort clusterId) => Data.Instance.ChangeChunkClusterInternal(chunkIdx, clusterId);

        /// <summary>
        /// Checks whether a chunk contains any entities (loaded or unloaded) by examining
        /// the <c>NotEmptyBlocks</c> bitmap. Fast O(1) check.
        /// </summary>
        /// <param name="chunkIdx">Chunk index.</param>
        /// <returns><c>true</c> if the chunk has at least one entity.</returns>
        [MethodImpl(AggressiveInlining)]
        public static bool HasEntitiesInChunk(uint chunkIdx) {
            #if FFS_ECS_DEBUG
            AssertWorldIsInitialized(WorldTypeName);
            AssertMultiThreadNotActive(WorldTypeName);
            AssertChunkIsRegistered(WorldTypeName, chunkIdx);
            #endif

            return Data.Instance.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value != 0;
        }

        /// <summary>
        /// Checks whether a chunk has any loaded entities.
        /// </summary>
        /// <param name="chunkIdx">Chunk index.</param>
        /// <returns><c>true</c> if the chunk has loaded entities.</returns>
        [MethodImpl(AggressiveInlining)]
        public static bool HasLoadedEntitiesInChunk(uint chunkIdx) {
            #if FFS_ECS_DEBUG
            AssertWorldIsInitialized(WorldTypeName);
            AssertMultiThreadNotActive(WorldTypeName);
            AssertChunkIsRegistered(WorldTypeName, chunkIdx);
            #endif

            return Data.Instance.HeuristicLoadedChunks[chunkIdx].Value != 0;
        }

        /// <summary>
        /// Changes the ownership type of chunk (e.g. from Self to an external owner or vice versa).
        /// </summary>
        /// <param name="chunkIdx">Chunk index.</param>
        /// <param name="owner">New ownership type.</param>
        [MethodImpl(AggressiveInlining)]
        public static void ChangeChunkOwner(uint chunkIdx, ChunkOwnerType owner) => Data.Instance.ChangeChunkOwnerInternal(chunkIdx, owner);

        /// <summary>
        /// Returns the current ownership type of chunk.
        /// </summary>
        /// <param name="chunkIdx">Chunk index.</param>
        /// <returns>The chunk's <see cref="ChunkOwnerType"/>.</returns>
        [MethodImpl(AggressiveInlining)]
        public static ChunkOwnerType GetChunkOwner(uint chunkIdx) => Data.Instance.GetChunkOwnerType(chunkIdx);
        #endregion

        #region TICK
        /// <summary>
        /// Advances the world tick counter and rotates the tracking ring buffer.
        /// Call once per frame after all system groups have run.
        /// After calling, the current tracking slot is cleared and ready for new changes.
        /// Systems automatically track their last tick to see changes since their previous execution.
        /// <para>Requires <see cref="WorldConfig.TrackingBufferSize"/> &gt; 0.</para>
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static void Tick() {
            #if FFS_ECS_DEBUG
            AssertWorldIsInitialized(WorldTypeName);
            if (Data.Instance.TrackingBufferSize == 0) {
                throw new StaticEcsException(WorldTypeName, "Tick", "TrackingBufferSize is 0. Tick-based tracking is disabled.");
            }
            if (Data.Instance.MultiThreadActive) {
                throw new StaticEcsException(WorldTypeName, "Tick", "Cannot advance tick while parallel queries are active.");
            }
            #endif
            Data.Instance.AdvanceTickInternal();
        }

        /// <summary>
        /// The current world tick. Monotonically increasing, starts at 1.
        /// Incremented by <see cref="Tick"/>.
        /// </summary>
        public static ulong CurrentTick {
            [MethodImpl(AggressiveInlining)]
            get => Data.Instance.CurrentTick;
        }

        /// <summary>
        /// The last tick of the currently executing system (set by <see cref="Systems{TSystemsType}.Update"/>).
        /// Used by tracking filters to determine the range of ticks to check.
        /// </summary>
        public static ulong CurrentLastTick {
            [MethodImpl(AggressiveInlining)]
            get => Data.Instance.CurrentLastTick;
        }
        #endregion

        #region TRACKING
        /// <summary>
        /// Clears tracking state for component type <typeparamref name="T"/>.
        /// After calling, <c>Added&lt;T&gt;</c> and <c>Deleted&lt;T&gt;</c> query filters will match no entities
        /// until new structural changes occur.
        /// </summary>
        /// <remarks>
        /// Normally not needed — tracking is managed automatically by `W.Tick()`. Clears ALL ring buffer slots.
        /// Only affects the specified component type — other tracked types retain their state.
        /// </remarks>
        /// <typeparam name="T">Component type with tracking enabled.</typeparam>
        /// <example><code>
        /// // At the start of each frame, reset tracking for Position:
        /// MyWorld.ClearTracking&lt;Position&gt;();
        /// </code></example>
        [MethodImpl(AggressiveInlining)]
        public static void ClearTracking<T>() where T : struct, IComponentOrTag {
            Components<T>.Instance.ClearTrackingInternal();
        }

        /// <summary>
        /// Clears tracking state for all registered component types.
        /// After calling, <c>Added&lt;T&gt;</c> and <c>Deleted&lt;T&gt;</c> query filters will match no entities
        /// for any component type until new structural changes occur.
        /// </summary>
        /// <remarks>
        /// Iterates over every registered component type and clears its tracking bitmasks.
        /// Prefer <see cref="ClearTracking{T}"/> when only a single type needs to be reset.
        /// </remarks>
        [MethodImpl(AggressiveInlining)]
        public static void ClearAllComponentsTracking() {
            Data.Instance.ClearAllComponentsTrackingInternal();
        }

        /// <summary>
        /// Clears tracking state for all registered component and tag types.
        /// After calling, all <c>AllAdded</c>, <c>AllDeleted</c>, and <c>Created</c> query filters
        /// will match no entities until new structural changes occur.
        /// </summary>
        /// <remarks>
        /// Normally not needed — tracking is managed automatically by `W.Tick()`. Clears ALL ring buffer slots.
        /// </remarks>
        /// <example><code>
        /// // Typical game loop usage:
        /// MyWorld.ClearTracking();
        /// systems.Update();
        /// </code></example>
        [MethodImpl(AggressiveInlining)]
        public static void ClearTracking() {
            ClearAllComponentsTracking();
            ClearCreatedTracking();
        }

        /// <summary>
        /// Clears entity creation tracking state.
        /// After calling, the <c>Created</c> query filter will match no entities
        /// until new entities are created.
        /// </summary>
        /// <remarks>
        /// Requires <see cref="WorldConfig.TrackCreated"/> to be <c>true</c>.
        /// Normally not needed — tracking is managed automatically by `W.Tick()`. Clears ALL ring buffer slots.
        /// </remarks>
        [MethodImpl(AggressiveInlining)]
        public static void ClearCreatedTracking() {
            Data.Instance.ClearCreatedTrackingInternal();
        }

        /// <summary>
        /// Clears only the Added tracking state for component type <typeparamref name="T"/>.
        /// Deleted tracking state is preserved. After calling, <c>Added&lt;T&gt;</c> query filters will match
        /// no entities until new Add operations occur.
        /// </summary>
        /// <typeparam name="T">Component type with tracking enabled.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void ClearAddedTracking<T>() where T : struct, IComponentOrTag {
            Components<T>.Instance.ClearAddedTracking();
        }

        /// <summary>
        /// Clears only the Deleted tracking state for component type <typeparamref name="T"/>.
        /// Added tracking state is preserved. After calling, <c>Deleted&lt;T&gt;</c> query filters will match
        /// no entities until new Delete operations occur.
        /// </summary>
        /// <typeparam name="T">Component type with tracking enabled.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void ClearDeletedTracking<T>() where T : struct, IComponentOrTag {
            Components<T>.Instance.ClearDeletedTracking();
        }

        #if !FFS_ECS_DISABLE_CHANGED_TRACKING
        /// <summary>
        /// Clears only the Changed tracking state for component type <typeparamref name="T"/>.
        /// Added and Deleted tracking state is preserved. After calling, <c>Changed&lt;T&gt;</c> query filters will match
        /// no entities until new Ref/Add operations occur.
        /// </summary>
        /// <typeparam name="T">Component type with tracking enabled.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void ClearChangedTracking<T>() where T : struct, IComponent {
            Components<T>.Instance.ClearChangedTracking();
        }

        /// <summary>
        /// Clears Changed tracking state for all registered component types.
        /// After calling, <c>Changed&lt;T&gt;</c> query filters will match no entities until new Ref/Add operations occur.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static void ClearAllChangedTracking() {
            Data.Instance.ClearChangedComponentTrackingInternal();
        }
        #endif

        /// <summary>
        /// Clears only the Added tracking state for all registered component types.
        /// Deleted tracking state is preserved for all types.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static void ClearAllAddedTracking() {
            Data.Instance.ClearAddedComponentTrackingInternal();
        }

        /// <summary>
        /// Clears only the Deleted tracking state for all registered component types.
        /// Added tracking state is preserved for all types.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static void ClearAllDeletedTracking() {
            Data.Instance.ClearDeletedComponentTrackingInternal();
        }
        #endregion

        #region EVENTS
        /// <summary>
        /// Sends an event to all registered receivers of this event type.
        /// Events are buffered and delivered to receivers on their next read.
        /// </summary>
        /// <remarks>
        /// Thread-safe for concurrent sends of the same event type, but only when there is no concurrent reading
        /// of the same type. Reading of one event type must occur in a single thread. Different event types
        /// can be read from different threads simultaneously. Receiver operations are main-thread only.
        /// </remarks>
        /// <typeparam name="TEvent">The event type to send.</typeparam>
        /// <param name="value">Event data. Default value sends an empty/default-initialized event.</param>
        /// <returns><c>true</c> if the event was successfully added to the buffer; <c>false</c> if there are no registered receivers.</returns>
        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Event metadata is preserved by the registration path.")]
        #endif
        [MethodImpl(AggressiveInlining)]
        public static bool SendEvent<TEvent>(TEvent value = default) where TEvent : struct, IEvent {
            #if FFS_ECS_DEBUG
            AssertWorldIsInitialized(WorldTypeName);
            AssertRegisteredEvent<TEvent>(WorldTypeName);
            #endif
            return Events<TEvent>.Instance.Add(value);
        }

        /// <summary>
        /// Creates and returns a new event receiver for the specified event type.
        /// The receiver can be used to read events sent via <see cref="SendEvent{TEvent}"/>.
        /// Each receiver maintains its own read cursor — multiple receivers can independently
        /// consume the same event stream.
        /// </summary>
        /// <typeparam name="TEvent">The event type to receive.</typeparam>
        /// <returns>A new <see cref="EventReceiver{TWorld, TEvent}"/> that can read events of this type.</returns>
        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Event metadata is preserved by the registration path.")]
        #endif
        [MethodImpl(AggressiveInlining)]
        public static EventReceiver<TWorld, TEvent> RegisterEventReceiver<TEvent>() where TEvent : struct, IEvent {
            #if FFS_ECS_DEBUG
            AssertWorldIsInitialized(WorldTypeName);
            AssertRegisteredEvent<TEvent>(WorldTypeName);
            #endif
            return Events<TEvent>.Instance.CreateReceiver();
        }

        /// <summary>
        /// Deletes an event receiver, freeing its internal resources. The receiver becomes invalid
        /// after this call and must not be used.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="receiver">Reference to the receiver to delete. Will be invalidated.</param>
        [MethodImpl(AggressiveInlining)]
        public static void DeleteEventReceiver<TEvent>(ref EventReceiver<TWorld, TEvent> receiver) where TEvent : struct, IEvent {
            #if FFS_ECS_DEBUG
            AssertWorldIsInitialized(WorldTypeName);
            AssertRegisteredEvent<TEvent>(WorldTypeName);
            #endif
            Events<TEvent>.Instance.DeleteReceiver(ref receiver);
        }
        #endregion

        #region RESOURCES
        /// <summary>
        /// Checks whether a singleton resource of the given type exists in this world.
        /// Resources are world-scoped singletons — one value per type per world.
        /// <para>
        /// Usable in both <see cref="WorldStatus.Created"/> and <see cref="WorldStatus.Initialized"/> phases,
        /// so resources can be set up before initialization and queried at any time.
        /// </para>
        /// </summary>
        /// <typeparam name="TResource">The resource type to check for. Can be any type (no interface constraint).</typeparam>
        /// <returns><c>true</c> if a resource of this type has been set.</returns>
        [MethodImpl(AggressiveInlining)]
        public static bool HasResource<TResource>() {
            return Resources<TResource>.Has();
        }

        /// <summary>
        /// Checks whether a keyed resource exists. Keyed resources allow multiple instances of the
        /// same type to be stored under different string keys.
        /// </summary>
        /// <typeparam name="TResource">The resource type (used for type safety at call site).</typeparam>
        /// <param name="key">String key identifying the resource.</param>
        /// <returns><c>true</c> if a resource with this key exists.</returns>
        [MethodImpl(AggressiveInlining)]
        public static bool HasResource<TResource>(string key) {
            return NamedResources.Has(key);
        }

        /// <summary>
        /// Returns a reference to the singleton resource of the given type. The resource must have
        /// been previously set via <see cref="SetResource{TResource}(TResource, bool)"/>.
        /// <para>
        /// The returned reference is valid as long as the resource exists and the world is alive.
        /// You can read and write through this reference directly.
        /// </para>
        /// </summary>
        /// <typeparam name="TResource">The resource type.</typeparam>
        /// <returns>A mutable reference to the stored resource value.</returns>
        [MethodImpl(AggressiveInlining)]
        public static ref TResource GetResource<TResource>() {
            return ref Resources<TResource>.Value;
        }

        /// <summary>
        /// Returns a reference to a keyed resource.
        /// </summary>
        /// <typeparam name="TResource">The resource type.</typeparam>
        /// <param name="key">String key identifying the resource.</param>
        /// <returns>A mutable reference to the stored resource value.</returns>
        [MethodImpl(AggressiveInlining)]
        public static ref TResource GetResource<TResource>(string key) {
            return ref NamedResources.Get<TResource>(key);
        }

        /// <summary>
        /// Sets (or replaces) the singleton resource of the given type.
        /// Resources are world-scoped singletons ideal for shared state like configuration,
        /// time data, input state, or service references.
        /// </summary>
        /// <typeparam name="TResource">The resource type. Can be any type — no interface constraint required.</typeparam>
        /// <param name="value">The resource value to store.</param>
        /// <param name="clearOnDestroy">
        /// If <c>true</c> (default), the resource is automatically cleared when the world is destroyed.
        /// Set to <c>false</c> for resources that should persist across world re-creation cycles.
        /// Only applied on the first registration — replacing an existing resource preserves
        /// the original <paramref name="clearOnDestroy"/> setting.
        /// </param>
        [MethodImpl(AggressiveInlining)]
        public static void SetResource<TResource>(TResource value, bool clearOnDestroy = true) {
            Resources<TResource>.Set(value, clearOnDestroy);
        }

        /// <summary>
        /// Sets (or replaces) a keyed resource, allowing multiple resources of the same type
        /// under different string keys.
        /// </summary>
        /// <typeparam name="TResource">The resource type.</typeparam>
        /// <param name="key">String key for this resource instance.</param>
        /// <param name="value">The resource value to store.</param>
        /// <param name="clearOnDestroy">If <c>true</c>, cleared on world destroy.</param>
        [MethodImpl(AggressiveInlining)]
        public static void SetResource<TResource>(string key, TResource value, bool clearOnDestroy = true) {
            NamedResources.Set(key, value, clearOnDestroy);
        }

        /// <summary>
        /// Removes the singleton resource of the given type.
        /// </summary>
        /// <typeparam name="TResource">The resource type to remove.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void RemoveResource<TResource>() {
            Resources<TResource>.Remove();
        }

        /// <summary>
        /// Removes a keyed resource.
        /// </summary>
        /// <param name="key">String key of the resource to remove.</param>
        [MethodImpl(AggressiveInlining)]
        public static void RemoveResource(string key) {
            NamedResources.Remove(key);
        }
        #endregion

        #region ENTITIES
        /// <summary>
        /// Creates a new entity of the specified type. If <typeparamref name="TEntityType"/> defines
        /// <see cref="IEntityType.OnCreate{TWorld}"/>, it is called after entity creation (inlined by JIT).
        /// </summary>
        /// <typeparam name="TEntityType">Entity type struct implementing <see cref="IEntityType"/>. Determines segment placement and query filter matching.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static Entity NewEntity<TEntityType>(ushort clusterId = default)
            where TEntityType : struct, IEntityType {
            Data.Instance.CreateEntity<TEntityType>(default, clusterId, out var entity);
            return entity;
        }

        /// <summary>
        /// Creates a new entity of the specified type with configuration data in the struct.
        /// The <paramref name="entityType"/> struct fields are accessible in OnCreate via <c>this</c>.
        /// </summary>
        /// <typeparam name="TEntityType">Entity type struct implementing <see cref="IEntityType"/>. Determines segment placement and query filter matching.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static Entity NewEntity<TEntityType>(TEntityType entityType, ushort clusterId = default)
            where TEntityType : struct, IEntityType {
            Data.Instance.CreateEntity(entityType, clusterId, out var entity);
            return entity;
        }

        /// <summary>
        /// Creates a new entity of the specified type in a specific chunk.
        /// </summary>
        /// <typeparam name="TEntityType">Entity type struct implementing <see cref="IEntityType"/>. Determines segment placement and query filter matching.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static Entity NewEntityInChunk<TEntityType>(uint chunkIdx)
            where TEntityType : struct, IEntityType {
            Data.Instance.CreateEntity<TEntityType>(default, chunkIdx, out var entity);
            return entity;
        }

        /// <summary>
        /// Creates a new entity of the specified type in a specific chunk.
        /// </summary>
        /// <typeparam name="TEntityType">Entity type struct implementing <see cref="IEntityType"/>. Determines segment placement and query filter matching.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static Entity NewEntityInChunk<TEntityType>(TEntityType entityType, uint chunkIdx)
            where TEntityType : struct, IEntityType {
            Data.Instance.CreateEntity(entityType, chunkIdx, out var entity);
            return entity;
        }

        /// <summary>
        /// Attempts to create a new entity of the specified type.
        /// Returns false if creation failed (no available chunks in dependent world).
        /// </summary>
        /// <typeparam name="TEntityType">Entity type struct implementing <see cref="IEntityType"/>. Determines segment placement and query filter matching.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static bool TryNewEntity<TEntityType>(out Entity entity, ushort clusterId = default)
            where TEntityType : struct, IEntityType {
            return Data.Instance.TryCreateEntity<TEntityType>(default, clusterId, out entity);
        }

        /// <summary>
        /// Attempts to create a new entity of the specified type.
        /// Returns false if creation failed (no available chunks in dependent world).
        /// </summary>
        /// <typeparam name="TEntityType">Entity type struct implementing <see cref="IEntityType"/>. Determines segment placement and query filter matching.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static bool TryNewEntity<TEntityType>(TEntityType entityType, out Entity entity, ushort clusterId = default)
            where TEntityType : struct, IEntityType {
            return Data.Instance.TryCreateEntity(entityType, clusterId, out entity);
        }

        /// <summary>
        /// Attempts to create a new entity of the specified type.
        /// Returns false if creation failed (no available chunks in dependent world).
        /// </summary>
        /// <typeparam name="TEntityType">Entity type struct implementing <see cref="IEntityType"/>. Determines segment placement and query filter matching.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static bool TryNewEntityInChunk<TEntityType>(out Entity entity, uint chunkIdx)
            where TEntityType : struct, IEntityType {
            return Data.Instance.TryCreateEntity<TEntityType>(default, chunkIdx, out entity);
        }

        /// <summary>
        /// Attempts to create a new entity of the specified type.
        /// Returns false if creation failed (no available chunks in dependent world).
        /// </summary>
        /// <typeparam name="TEntityType">Entity type struct implementing <see cref="IEntityType"/>. Determines segment placement and query filter matching.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static bool TryNewEntityInChunk<TEntityType>(TEntityType entityType, out Entity entity, uint chunkIdx)
            where TEntityType : struct, IEntityType {
            return Data.Instance.TryCreateEntity(entityType, chunkIdx, out entity);
        }

        /// <summary>
        /// Creates an entity at a specific slot dictated by a previously serialized <see cref="EntityGID"/>.
        /// The entity is placed in the exact same Id/Chunk/Cluster position as the original,
        /// preserving GID validity for deserialization and cross-world reference resolution.
        /// The segment's entity type is set to <typeparamref name="TEntityType"/>.
        /// <para>
        /// Primarily used by the serializer during world snapshot loading. In normal gameplay,
        /// prefer <see cref="NewEntity{TEntityType}(ushort)"/>.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntityType">Entity type struct implementing <see cref="IEntityType"/>.</typeparam>
        /// <param name="gid">The EntityGID specifying the exact slot, version, and cluster to restore.</param>
        /// <returns>A live entity handle at the specified slot.</returns>
        [MethodImpl(AggressiveInlining)]
        public static Entity NewEntityByGID<TEntityType>(EntityGID gid)
            where TEntityType : struct, IEntityType {
            Data.Instance.CreateEntity<TEntityType>(default, gid, out var entity);
            return entity;
        }

        /// <summary>
        /// Creates an entity at a specific slot dictated by a previously serialized <see cref="EntityGID"/>.
        /// The entity is placed in the exact same Id/Chunk/Cluster position as the original,
        /// preserving GID validity for deserialization and cross-world reference resolution.
        /// The segment's entity type is set to <typeparamref name="TEntityType"/>.
        /// <para>
        /// Primarily used by the serializer during world snapshot loading. In normal gameplay,
        /// prefer <see cref="NewEntity{TEntityType}(ushort)"/>.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntityType">Entity type struct implementing <see cref="IEntityType"/>.</typeparam>
        /// <param name="entityType">Entity type instance; its fields are accessible in <see cref="IEntityType.OnCreate{TWorld}"/> via <c>this</c>.</param>
        /// <param name="gid">The EntityGID specifying the exact slot, version, and cluster to restore.</param>
        /// <returns>A live entity handle at the specified slot.</returns>
        [MethodImpl(AggressiveInlining)]
        public static Entity NewEntityByGID<TEntityType>(TEntityType entityType, EntityGID gid)
            where TEntityType : struct, IEntityType {
            Data.Instance.CreateEntity(entityType, gid, out var entity);
            return entity;
        }

        /// <summary>
        /// Creates a new entity using a runtime entity type ID.
        /// If the entity type defines <see cref="IEntityType.OnCreate{TWorld}"/>, it is called via type-erased dispatch.
        /// <para>
        /// Primarily used by the serializer when the entity type is only known at runtime.
        /// In normal gameplay, prefer <see cref="NewEntity{TEntityType}(ushort)"/>.
        /// </para>
        /// </summary>
        /// <param name="entityType">Runtime entity type ID (obtained from <c>EntityTypeInfo&lt;T&gt;.Id</c>).</param>
        /// <param name="clusterId">Optional cluster to place the entity in.</param>
        /// <returns>A live entity handle.</returns>
        [MethodImpl(AggressiveInlining)]
        public static Entity NewEntity(byte entityType, ushort clusterId = default) {
            Data.Instance.CreateEntityWithOnCreate(entityType, clusterId, out var entity);
            return entity;
        }

        /// <summary>
        /// Creates a new entity in a specific chunk using a runtime entity type ID.
        /// If the entity type defines <see cref="IEntityType.OnCreate{TWorld}"/>, it is called via type-erased dispatch.
        /// <para>
        /// Primarily used by the serializer when the entity type is only known at runtime.
        /// In normal gameplay, prefer <see cref="NewEntityInChunk{TEntityType}(uint)"/>.
        /// </para>
        /// </summary>
        /// <param name="entityType">Runtime entity type ID (obtained from <c>EntityTypeInfo&lt;T&gt;.Id</c>).</param>
        /// <param name="chunkIdx">Index of the chunk to place the entity in.</param>
        /// <returns>A live entity handle.</returns>
        [MethodImpl(AggressiveInlining)]
        public static Entity NewEntityInChunk(byte entityType, uint chunkIdx) {
            Data.Instance.CreateEntityWithOnCreate(entityType, chunkIdx, out var entity);
            return entity;
        }

        /// <summary>
        /// Creates an entity at a specific slot dictated by a previously serialized <see cref="EntityGID"/>
        /// using a runtime entity type ID. The entity is placed in the exact same Id/Chunk/Cluster position
        /// as the original, preserving GID validity for deserialization and cross-world reference resolution.
        /// <para>
        /// Primarily used by the serializer when the entity type is only known at runtime.
        /// In normal gameplay, prefer <see cref="NewEntityByGID{TEntityType}(EntityGID)"/>.
        /// </para>
        /// </summary>
        /// <param name="entityType">Runtime entity type ID (obtained from <c>EntityTypeInfo&lt;T&gt;.Id</c>).</param>
        /// <param name="gid">The EntityGID specifying the exact slot, version, and cluster to restore.</param>
        /// <returns>A live entity handle at the specified slot.</returns>
        [MethodImpl(AggressiveInlining)]
        public static Entity NewEntityByGID(byte entityType, EntityGID gid) {
            Data.Instance.CreateEntityWithOnCreate(entityType, gid, out var entity);
            return entity;
        }

        /// <summary>
        /// Batch entity creation: creates <paramref name="count"/> entities at once with the specified
        /// entity type. Significantly more efficient than calling <see cref="NewEntity{TEntityType}(ushort)"/> in a loop
        /// because entities are allocated in contiguous bitmap blocks, minimizing allocation overhead.
        /// <para>
        /// The <paramref name="onCreate"/> callback is invoked for each created entity, allowing
        /// per-entity initialization. If generic overloads are used, the specified component types
        /// are batch-added to all created entities before the callback fires.
        /// </para>
        /// <para>
        /// The batch may span multiple internal allocation rounds if <paramref name="count"/> exceeds
        /// the remaining capacity of the current chunk segment.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntityType">Entity type struct implementing <see cref="IEntityType"/>.</typeparam>
        /// <param name="entityType">Entity type instance; its fields are accessible in <see cref="IEntityType.OnCreate{TWorld}"/> via <c>this</c>.</param>
        /// <param name="count">Number of entities to create.</param>
        /// <param name="clusterId">Cluster to create entities in (default: 0).</param>
        /// <param name="onCreate">
        /// Callback invoked for each created entity, providing the entity handle for per-entity setup.
        /// Can be <c>null</c> for the generic overloads (components are added automatically).
        /// </param>
        #region NEW_BY_TYPE_BATCH
        [MethodImpl(AggressiveInlining)]
        public static void NewEntities<TEntityType>(uint count, TEntityType entityType = default, ushort clusterId = default, QueryFunctionWithEntity<TWorld> onCreate = null) 
            where TEntityType : struct, IEntityType {
            ref var entities = ref Data.Instance;

            while (count > 0) {
                var created = entities.CreateEntitiesBatch(EntityTypeInfo<TEntityType>.Instance.Id, clusterId, count, out var mask, out var segmentIdx, out var segmentBlockIdx);
                count -= created;
                Data.InvokeOnCreateBatch(entityType, onCreate, mask, segmentIdx, segmentBlockIdx);
            }
        }

        /// <inheritdoc cref="NewEntities{TEntityType}(uint, TEntityType, ushort, QueryFunctionWithEntity{TWorld})"/>
        /// <typeparam name="T1">Component type to add to all created entities (default-initialized).</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void NewEntities<TEntityType, T1>(uint count, TEntityType entityType = default, ushort clusterId = default, QueryFunctionWithEntity<TWorld> onCreate = null) 
            where TEntityType : struct, IEntityType
            where T1 : struct, IComponent {
            ref var entities = ref Data.Instance;

            while (count > 0) {
                var created = entities.CreateEntitiesBatch(EntityTypeInfo<TEntityType>.Instance.Id, clusterId, count, out var mask, out var segmentIdx, out var segmentBlockIdx);
                count -= created;
                Components<T1>.Instance.BatchAdd(mask, segmentIdx, segmentBlockIdx);
                Data.InvokeOnCreateBatch(entityType, onCreate, mask, segmentIdx, segmentBlockIdx);
            }
        }

        /// <inheritdoc cref="NewEntities{TEntityType}(uint, TEntityType, ushort, QueryFunctionWithEntity{TWorld})"/>
        /// <typeparam name="T1">First component type (default-initialized).</typeparam>
        /// <typeparam name="T2">Second component type (default-initialized).</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void NewEntities<TEntityType, T1, T2>(uint count, TEntityType entityType = default, ushort clusterId = default, QueryFunctionWithEntity<TWorld> onCreate = null) 
            where TEntityType : struct, IEntityType
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            ref var entities = ref Data.Instance;

            while (count > 0) {
                var created = entities.CreateEntitiesBatch(EntityTypeInfo<TEntityType>.Instance.Id, clusterId, count, out var mask, out var segmentIdx, out var segmentBlockIdx);
                count -= created;
                Components<T1>.Instance.BatchAdd(mask, segmentIdx, segmentBlockIdx);
                Components<T2>.Instance.BatchAdd(mask, segmentIdx, segmentBlockIdx);
                Data.InvokeOnCreateBatch(entityType, onCreate, mask, segmentIdx, segmentBlockIdx);
            }
        }

        /// <inheritdoc cref="NewEntities{TEntityType}(uint, TEntityType, ushort, QueryFunctionWithEntity{TWorld})"/>
        /// <typeparam name="T1">First component type (default-initialized).</typeparam>
        /// <typeparam name="T2">Second component type (default-initialized).</typeparam>
        /// <typeparam name="T3">Third component type (default-initialized).</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void NewEntities<TEntityType, T1, T2, T3>(uint count, TEntityType entityType = default, ushort clusterId = default, QueryFunctionWithEntity<TWorld> onCreate = null) 
            where TEntityType : struct, IEntityType
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            ref var entities = ref Data.Instance;

            while (count > 0) {
                var created = entities.CreateEntitiesBatch(EntityTypeInfo<TEntityType>.Instance.Id, clusterId, count, out var mask, out var segmentIdx, out var segmentBlockIdx);
                count -= created;
                Components<T1>.Instance.BatchAdd(mask, segmentIdx, segmentBlockIdx);
                Components<T2>.Instance.BatchAdd(mask, segmentIdx, segmentBlockIdx);
                Components<T3>.Instance.BatchAdd(mask, segmentIdx, segmentBlockIdx);
                Data.InvokeOnCreateBatch(entityType, onCreate, mask, segmentIdx, segmentBlockIdx);
            }
        }

        /// <inheritdoc cref="NewEntities{TEntityType}(uint, TEntityType, ushort, QueryFunctionWithEntity{TWorld})"/>
        /// <typeparam name="T1">First component type (default-initialized).</typeparam>
        /// <typeparam name="T2">Second component type (default-initialized).</typeparam>
        /// <typeparam name="T3">Third component type (default-initialized).</typeparam>
        /// <typeparam name="T4">Fourth component type (default-initialized).</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void NewEntities<TEntityType, T1, T2, T3, T4>(uint count, TEntityType entityType = default, ushort clusterId = default, QueryFunctionWithEntity<TWorld> onCreate = null) 
            where TEntityType : struct, IEntityType
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            ref var entities = ref Data.Instance;

            while (count > 0) {
                var created = entities.CreateEntitiesBatch(EntityTypeInfo<TEntityType>.Instance.Id, clusterId, count, out var mask, out var segmentIdx, out var segmentBlockIdx);
                count -= created;
                Components<T1>.Instance.BatchAdd(mask, segmentIdx, segmentBlockIdx);
                Components<T2>.Instance.BatchAdd(mask, segmentIdx, segmentBlockIdx);
                Components<T3>.Instance.BatchAdd(mask, segmentIdx, segmentBlockIdx);
                Components<T4>.Instance.BatchAdd(mask, segmentIdx, segmentBlockIdx);
                Data.InvokeOnCreateBatch(entityType, onCreate, mask, segmentIdx, segmentBlockIdx);
            }
        }

        /// <inheritdoc cref="NewEntities{TEntityType}(uint, TEntityType, ushort, QueryFunctionWithEntity{TWorld})"/>
        /// <typeparam name="T1">First component type (default-initialized).</typeparam>
        /// <typeparam name="T2">Second component type (default-initialized).</typeparam>
        /// <typeparam name="T3">Third component type (default-initialized).</typeparam>
        /// <typeparam name="T4">Fourth component type (default-initialized).</typeparam>
        /// <typeparam name="T5">Fifth component type (default-initialized).</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void NewEntities<TEntityType, T1, T2, T3, T4, T5>(uint count, TEntityType entityType = default, ushort clusterId = default, QueryFunctionWithEntity<TWorld> onCreate = null) 
            where TEntityType : struct, IEntityType
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            ref var entities = ref Data.Instance;

            while (count > 0) {
                var created = entities.CreateEntitiesBatch(EntityTypeInfo<TEntityType>.Instance.Id, clusterId, count, out var mask, out var segmentIdx, out var segmentBlockIdx);
                count -= created;
                Components<T1>.Instance.BatchAdd(mask, segmentIdx, segmentBlockIdx);
                Components<T2>.Instance.BatchAdd(mask, segmentIdx, segmentBlockIdx);
                Components<T3>.Instance.BatchAdd(mask, segmentIdx, segmentBlockIdx);
                Components<T4>.Instance.BatchAdd(mask, segmentIdx, segmentBlockIdx);
                Components<T5>.Instance.BatchAdd(mask, segmentIdx, segmentBlockIdx);
                Data.InvokeOnCreateBatch(entityType, onCreate, mask, segmentIdx, segmentBlockIdx);
            }
        }

        /// <summary>
        /// Batch entity creation with initial component values: creates <paramref name="count"/>
        /// entities and assigns the same component value(s) to all of them. Supports 1–8 components.
        /// More efficient than per-entity creation for homogeneous spawning (e.g. spawning 1000 bullets
        /// with identical initial stats).
        /// </summary>
        /// <typeparam name="TEntityType">Entity type struct implementing <see cref="IEntityType"/>.</typeparam>
        /// <typeparam name="T1">First component type.</typeparam>
        /// <param name="entityType">Entity type instance; its fields are accessible in <see cref="IEntityType.OnCreate{TWorld}"/> via <c>this</c>.</param>
        /// <param name="count">Number of entities to create.</param>
        /// <param name="c1">Value to assign to the first component on all created entities.</param>
        /// <param name="clusterId">Cluster to create entities in (default: 0).</param>
        /// <param name="onCreate">Optional per-entity callback for additional initialization.</param>
        [MethodImpl(AggressiveInlining)]
        public static void NewEntities<TEntityType, T1>(uint count, T1 c1, TEntityType entityType = default, ushort clusterId = default, QueryFunctionWithEntity<TWorld> onCreate = null) 
            where TEntityType : struct, IEntityType
            where T1 : struct, IComponent {
            ref var entities = ref Data.Instance;

            while (count > 0) {
                var created = entities.CreateEntitiesBatch(EntityTypeInfo<TEntityType>.Instance.Id, clusterId, count, out var mask, out var segmentIdx, out var segmentBlockIdx);
                count -= created;
                Components<T1>.Instance.BatchSet(c1, mask, segmentIdx, segmentBlockIdx);
                Data.InvokeOnCreateBatch(entityType, onCreate, mask, segmentIdx, segmentBlockIdx);
            }
        }

        /// <inheritdoc cref="NewEntities{TEntityType, T1}(uint, T1, TEntityType, ushort, QueryFunctionWithEntity{TWorld})"/>
        /// <typeparam name="T2">Second component type.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void NewEntities<TEntityType, T1, T2>(uint count, T1 c1, T2 c2, TEntityType entityType = default, ushort clusterId = default, QueryFunctionWithEntity<TWorld> onCreate = null) 
            where TEntityType : struct, IEntityType
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            ref var entities = ref Data.Instance;

            while (count > 0) {
                var created = entities.CreateEntitiesBatch(EntityTypeInfo<TEntityType>.Instance.Id, clusterId, count, out var mask, out var segmentIdx, out var segmentBlockIdx);
                count -= created;
                Components<T1>.Instance.BatchSet(c1, mask, segmentIdx, segmentBlockIdx);
                Components<T2>.Instance.BatchSet(c2, mask, segmentIdx, segmentBlockIdx);
                Data.InvokeOnCreateBatch(entityType, onCreate, mask, segmentIdx, segmentBlockIdx);
            }
        }

        /// <inheritdoc cref="NewEntities{TEntityType, T1}(uint, T1, TEntityType, ushort, QueryFunctionWithEntity{TWorld})"/>
        /// <typeparam name="T2">Second component type.</typeparam>
        /// <typeparam name="T3">Third component type.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void NewEntities<TEntityType, T1, T2, T3>(uint count, T1 c1, T2 c2, T3 c3, TEntityType entityType = default, ushort clusterId = default, QueryFunctionWithEntity<TWorld> onCreate = null) 
            where TEntityType : struct, IEntityType
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            ref var entities = ref Data.Instance;

            while (count > 0) {
                var created = entities.CreateEntitiesBatch(EntityTypeInfo<TEntityType>.Instance.Id, clusterId, count, out var mask, out var segmentIdx, out var segmentBlockIdx);
                count -= created;
                Components<T1>.Instance.BatchSet(c1, mask, segmentIdx, segmentBlockIdx);
                Components<T2>.Instance.BatchSet(c2, mask, segmentIdx, segmentBlockIdx);
                Components<T3>.Instance.BatchSet(c3, mask, segmentIdx, segmentBlockIdx);
                Data.InvokeOnCreateBatch(entityType, onCreate, mask, segmentIdx, segmentBlockIdx);
            }
        }

        /// <inheritdoc cref="NewEntities{TEntityType, T1}(uint, T1, TEntityType, ushort, QueryFunctionWithEntity{TWorld})"/>
        /// <typeparam name="T2">Second component type.</typeparam>
        /// <typeparam name="T3">Third component type.</typeparam>
        /// <typeparam name="T4">Fourth component type.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void NewEntities<TEntityType, T1, T2, T3, T4>(uint count, T1 c1, T2 c2, T3 c3, T4 c4, TEntityType entityType = default,
                                                                    ushort clusterId = default, QueryFunctionWithEntity<TWorld> onCreate = null) 
            where TEntityType : struct, IEntityType
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            ref var entities = ref Data.Instance;

            while (count > 0) {
                var created = entities.CreateEntitiesBatch(EntityTypeInfo<TEntityType>.Instance.Id, clusterId, count, out var mask, out var segmentIdx, out var segmentBlockIdx);
                count -= created;
                Components<T1>.Instance.BatchSet(c1, mask, segmentIdx, segmentBlockIdx);
                Components<T2>.Instance.BatchSet(c2, mask, segmentIdx, segmentBlockIdx);
                Components<T3>.Instance.BatchSet(c3, mask, segmentIdx, segmentBlockIdx);
                Components<T4>.Instance.BatchSet(c4, mask, segmentIdx, segmentBlockIdx);
                Data.InvokeOnCreateBatch(entityType, onCreate, mask, segmentIdx, segmentBlockIdx);
            }
        }

        /// <inheritdoc cref="NewEntities{TEntityType, T1}(uint, T1, TEntityType, ushort, QueryFunctionWithEntity{TWorld})"/>
        /// <typeparam name="T2">Second component type.</typeparam>
        /// <typeparam name="T3">Third component type.</typeparam>
        /// <typeparam name="T4">Fourth component type.</typeparam>
        /// <typeparam name="T5">Fifth component type.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void NewEntities<TEntityType, T1, T2, T3, T4, T5>(uint count, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, TEntityType entityType = default,
                                                                        ushort clusterId = default, QueryFunctionWithEntity<TWorld> onCreate = null) 
            where TEntityType : struct, IEntityType
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            ref var entities = ref Data.Instance;

            while (count > 0) {
                var created = entities.CreateEntitiesBatch(EntityTypeInfo<TEntityType>.Instance.Id, clusterId, count, out var mask, out var segmentIdx, out var segmentBlockIdx);
                count -= created;
                Components<T1>.Instance.BatchSet(c1, mask, segmentIdx, segmentBlockIdx);
                Components<T2>.Instance.BatchSet(c2, mask, segmentIdx, segmentBlockIdx);
                Components<T3>.Instance.BatchSet(c3, mask, segmentIdx, segmentBlockIdx);
                Components<T4>.Instance.BatchSet(c4, mask, segmentIdx, segmentBlockIdx);
                Components<T5>.Instance.BatchSet(c5, mask, segmentIdx, segmentBlockIdx);
                Data.InvokeOnCreateBatch(entityType, onCreate, mask, segmentIdx, segmentBlockIdx);
            }
        }

        /// <inheritdoc cref="NewEntities{TEntityType, T1}(uint, T1, TEntityType, ushort, QueryFunctionWithEntity{TWorld})"/>
        /// <typeparam name="T2">Second component type.</typeparam>
        /// <typeparam name="T3">Third component type.</typeparam>
        /// <typeparam name="T4">Fourth component type.</typeparam>
        /// <typeparam name="T5">Fifth component type.</typeparam>
        /// <typeparam name="T6">Sixth component type.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void NewEntities<TEntityType, T1, T2, T3, T4, T5, T6>(uint count, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, TEntityType entityType = default,
                                                                            ushort clusterId = default, QueryFunctionWithEntity<TWorld> onCreate = null) 
            where TEntityType : struct, IEntityType
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent
            where T6 : struct, IComponent {
            ref var entities = ref Data.Instance;

            while (count > 0) {
                var created = entities.CreateEntitiesBatch(EntityTypeInfo<TEntityType>.Instance.Id, clusterId, count, out var mask, out var segmentIdx, out var segmentBlockIdx);
                count -= created;
                Components<T1>.Instance.BatchSet(c1, mask, segmentIdx, segmentBlockIdx);
                Components<T2>.Instance.BatchSet(c2, mask, segmentIdx, segmentBlockIdx);
                Components<T3>.Instance.BatchSet(c3, mask, segmentIdx, segmentBlockIdx);
                Components<T4>.Instance.BatchSet(c4, mask, segmentIdx, segmentBlockIdx);
                Components<T5>.Instance.BatchSet(c5, mask, segmentIdx, segmentBlockIdx);
                Components<T6>.Instance.BatchSet(c6, mask, segmentIdx, segmentBlockIdx);
                Data.InvokeOnCreateBatch(entityType, onCreate, mask, segmentIdx, segmentBlockIdx);
            }
        }

        /// <inheritdoc cref="NewEntities{TEntityType, T1}(uint, T1, TEntityType, ushort, QueryFunctionWithEntity{TWorld})"/>
        /// <typeparam name="T2">Second component type.</typeparam>
        /// <typeparam name="T3">Third component type.</typeparam>
        /// <typeparam name="T4">Fourth component type.</typeparam>
        /// <typeparam name="T5">Fifth component type.</typeparam>
        /// <typeparam name="T6">Sixth component type.</typeparam>
        /// <typeparam name="T7">Seventh component type.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void NewEntities<TEntityType, T1, T2, T3, T4, T5, T6, T7>(uint count, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7, TEntityType entityType = default,
                                                                                ushort clusterId = default, QueryFunctionWithEntity<TWorld> onCreate = null) 
            where TEntityType : struct, IEntityType
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent
            where T6 : struct, IComponent
            where T7 : struct, IComponent {
            ref var entities = ref Data.Instance;

            while (count > 0) {
                var created = entities.CreateEntitiesBatch(EntityTypeInfo<TEntityType>.Instance.Id, clusterId, count, out var mask, out var segmentIdx, out var segmentBlockIdx);
                count -= created;
                Components<T1>.Instance.BatchSet(c1, mask, segmentIdx, segmentBlockIdx);
                Components<T2>.Instance.BatchSet(c2, mask, segmentIdx, segmentBlockIdx);
                Components<T3>.Instance.BatchSet(c3, mask, segmentIdx, segmentBlockIdx);
                Components<T4>.Instance.BatchSet(c4, mask, segmentIdx, segmentBlockIdx);
                Components<T5>.Instance.BatchSet(c5, mask, segmentIdx, segmentBlockIdx);
                Components<T6>.Instance.BatchSet(c6, mask, segmentIdx, segmentBlockIdx);
                Components<T7>.Instance.BatchSet(c7, mask, segmentIdx, segmentBlockIdx);
                Data.InvokeOnCreateBatch(entityType, onCreate, mask, segmentIdx, segmentBlockIdx);
            }
        }

        /// <inheritdoc cref="NewEntities{TEntityType, T1}(uint, T1, TEntityType, ushort, QueryFunctionWithEntity{TWorld})"/>
        /// <typeparam name="T2">Second component type.</typeparam>
        /// <typeparam name="T3">Third component type.</typeparam>
        /// <typeparam name="T4">Fourth component type.</typeparam>
        /// <typeparam name="T5">Fifth component type.</typeparam>
        /// <typeparam name="T6">Sixth component type.</typeparam>
        /// <typeparam name="T7">Seventh component type.</typeparam>
        /// <typeparam name="T8">Eighth component type.</typeparam>
        [MethodImpl(AggressiveInlining)]
        public static void NewEntities<TEntityType, T1, T2, T3, T4, T5, T6, T7, T8>(uint count, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7, T8 c8, TEntityType entityType = default,
                                                                                    ushort clusterId = default, QueryFunctionWithEntity<TWorld> onCreate = null) 
            where TEntityType : struct, IEntityType
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent
            where T6 : struct, IComponent
            where T7 : struct, IComponent
            where T8 : struct, IComponent {
            ref var entities = ref Data.Instance;

            while (count > 0) {
                var created = entities.CreateEntitiesBatch(EntityTypeInfo<TEntityType>.Instance.Id, clusterId, count, out var mask, out var segmentIdx, out var segmentBlockIdx);
                count -= created;
                Components<T1>.Instance.BatchSet(c1, mask, segmentIdx, segmentBlockIdx);
                Components<T2>.Instance.BatchSet(c2, mask, segmentIdx, segmentBlockIdx);
                Components<T3>.Instance.BatchSet(c3, mask, segmentIdx, segmentBlockIdx);
                Components<T4>.Instance.BatchSet(c4, mask, segmentIdx, segmentBlockIdx);
                Components<T5>.Instance.BatchSet(c5, mask, segmentIdx, segmentBlockIdx);
                Components<T6>.Instance.BatchSet(c6, mask, segmentIdx, segmentBlockIdx);
                Components<T7>.Instance.BatchSet(c7, mask, segmentIdx, segmentBlockIdx);
                Components<T8>.Instance.BatchSet(c8, mask, segmentIdx, segmentBlockIdx);
                Data.InvokeOnCreateBatch(entityType, onCreate, mask, segmentIdx, segmentBlockIdx);
            }
        }
        #endregion
        #endregion

        /// <summary>
        /// Returns a fluent type registrar for chaining type registration calls.
        /// Use during the <see cref="WorldStatus.Created"/> phase to register all component, tag,
        /// event, link, and multi-component types in a single expression.
        /// <para>
        /// Example: <c>World&lt;W&gt;.Types().Component&lt;Position&gt;().Component&lt;Velocity&gt;().Tag&lt;IsAlive&gt;();</c>
        /// </para>
        /// </summary>
        /// <returns>A <see cref="TypeRegistrar"/> for fluent chaining.</returns>
        [MethodImpl(AggressiveInlining)]
        public static TypeRegistrar Types() => default;

        /// <summary>
        /// Fluent builder for registering ECS types (components, tags, events, links, multi-components).
        /// Obtained via <see cref="Types"/>. Each method returns <c>this</c> for chaining.
        /// </summary>
        public readonly struct TypeRegistrar {

            /// <summary>
            /// Auto-discovers and registers all ECS types found in the specified assemblies via reflection.
            /// Scans for structs implementing the following marker interfaces and registers each one:
            /// <list type="bullet">
            /// <item><see cref="IComponent"/> — registered as component (excludes internal framework components).</item>
            /// <item><see cref="ITag"/> — registered as tag.</item>
            /// <item><see cref="IEvent"/> — registered as event.</item>
            /// <item><see cref="ILinkType"/> — wrapped in <c>Link&lt;T&gt;</c> and registered as a component.</item>
            /// <item><see cref="ILinksType"/> — wrapped in <c>Links&lt;T&gt;</c> and registered as a component.</item>
            /// <item><see cref="IMultiComponent"/> — wrapped in <c>Multi&lt;T&gt;</c> and registered as a component.</item>
            /// <item><see cref="IEntityType"/> — registered as entity type with Id from a static <c>byte Id</c> field. <see cref="Default"/> is skipped (already registered).</item>
            /// </list>
            /// <para>
            /// If no assemblies are specified, scans the calling assembly only.
            /// The StaticEcs framework assembly itself is always excluded from scanning.
            /// Abstract types and open generic type definitions are skipped.
            /// All types are registered with default configuration (default GUID, default serialization settings).
            /// </para>
            /// <para>
            /// Must be called during the <see cref="WorldStatus.Created"/> phase
            /// (after <see cref="World{TWorld}.Create"/>, before <see cref="World{TWorld}.Initialize"/>).
            /// A type that implements multiple interfaces (e.g., both <see cref="IComponent"/> and <see cref="IMultiComponent"/>)
            /// will be registered for each applicable interface.
            /// </para>
            /// </summary>
            /// <param name="assemblies">Assemblies to scan for ECS type implementations. If empty, scans the calling assembly.</param>
            /// <returns>This registrar for chaining.</returns>
            [MethodImpl(NoInlining)]
            public void RegisterAll(params Assembly[] assemblies) {
                if (assemblies == null || assemblies.Length == 0) {
                    assemblies = new[] { Assembly.GetCallingAssembly() };
                }
                AutoRegistration.RegisterAll<TWorld>(assemblies);
            }

            /// <summary>
            /// Registers an entity type for use in this world.
            /// </summary>
            /// <typeparam name="T">Entity type struct implementing <see cref="IEntityType"/>.</typeparam>
            /// <returns>This registrar for chaining.</returns>
            [MethodImpl(AggressiveInlining)]
            public TypeRegistrar EntityType<
                #if NET5_0_OR_GREATER
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
                #endif
                T>() where T : struct, IEntityType {
                RegisterEntityType<T>();
                return this;
            }

            /// <summary>
            /// Registers a component type for use in this world.
            /// Configuration is provided by implementing <see cref="IComponentConfig{T}"/> on the component type.
            /// </summary>
            /// <typeparam name="T">Component type — must be a struct implementing <see cref="IComponent"/>.</typeparam>
            /// <returns>This registrar for chaining.</returns>
            [MethodImpl(AggressiveInlining)]
            public TypeRegistrar Component<T>() where T : struct, IComponent {
                ComponentTypeConfig<T> config = default;
                if (default(T) is IComponentConfig<T> cfg) {
                    config = cfg.Config();
                }
                RegisterComponentType(config, typeof(T).Name);
                return this;
            }

            /// <summary>
            /// Registers a tag type for use in this world. Tags are zero-size marker components.
            /// Configuration is provided by implementing <see cref="ITagConfig{T}"/> on the tag type.
            /// </summary>
            /// <typeparam name="T">Tag type — must be a struct implementing <see cref="ITag"/>.</typeparam>
            /// <returns>This registrar for chaining.</returns>
            [MethodImpl(AggressiveInlining)]
            public TypeRegistrar Tag<T>() where T : struct, ITag {
                TagTypeConfig<T> config = default;
                if (default(T) is ITagConfig<T> cfg) {
                    config = cfg.Config();
                }
                RegisterTagType(config);
                return this;
            }

            /// <summary>
            /// Registers an event type for use in this world.
            /// Configuration is provided by implementing <see cref="IEventConfig{T}"/> on the event type.
            /// </summary>
            /// <typeparam name="T">Event type — must be a struct implementing <see cref="IEvent"/>.</typeparam>
            /// <returns>This registrar for chaining.</returns>
            [MethodImpl(AggressiveInlining)]
            public TypeRegistrar Event<T>() where T : struct, IEvent {
                EventTypeConfig<T> config = default;
                if (default(T) is IEventConfig<T> cfg) {
                    config = cfg.Config();
                }
                RegisterEventType(config);
                return this;
            }

            /// <summary>
            /// Registers a single-link relation type. Equivalent to registering <see cref="Link{T}"/> as a component.
            /// Configuration is provided by implementing <see cref="ILinkConfig{T}"/> on the link type.
            /// </summary>
            /// <typeparam name="T">Link type implementing <see cref="ILinkType"/>.</typeparam>
            /// <returns>This registrar for chaining.</returns>
            [MethodImpl(AggressiveInlining)]
            public TypeRegistrar Link<T>() where T : unmanaged, ILinkType {
                ComponentTypeConfig<Link<T>> config = default;
                if (default(T) is ILinkConfig<T> cfg) {
                    config = cfg.Config<TWorld>();
                }
                RegisterComponentType(config, $"Link<{typeof(T).Name}>");
                return this;
            }

            /// <summary>
            /// Registers a multi-link relation type. Equivalent to registering <see cref="Links{T}"/> as a component.
            /// Configuration is provided by implementing <see cref="ILinksConfig{T}"/> on the links type.
            /// </summary>
            /// <typeparam name="T">Links type implementing <see cref="ILinksType"/>.</typeparam>
            /// <returns>This registrar for chaining.</returns>
            [MethodImpl(AggressiveInlining)]
            public TypeRegistrar Links<T>() where T : unmanaged, ILinksType {
                ComponentTypeConfig<Links<T>> config = default;
                if (default(T) is ILinksConfig<T> cfg) {
                    config = cfg.Config<TWorld>();
                }
                RegisterComponentType(config, $"Links<{typeof(T).Name}>");
                return this;
            }

            /// <summary>
            /// Registers a multi-component type. Equivalent to registering <see cref="Multi{T}"/> as a component.
            /// Configuration is provided by implementing <see cref="IMultiComponentConfig{T}"/> on the multi-component type.
            /// </summary>
            /// <typeparam name="T">Multi-component value type implementing <see cref="IMultiComponent"/>.</typeparam>
            /// <returns>This registrar for chaining.</returns>
            [MethodImpl(AggressiveInlining)]
            public TypeRegistrar Multi<T>() where T : struct, IMultiComponent {
                ComponentTypeConfig<Multi<T>> config = default;
                IPackArrayStrategy<T> elementStrategy = null;
                if (default(T) is IMultiComponentConfig<T> cfg) {
                    config = cfg.Config<TWorld>();
                    elementStrategy = cfg.ElementPackStrategy();
                }
                RegisterMultiComponentType(config, elementStrategy, $"Multi<{typeof(T).Name}>");
                return this;
            }
        }

    }

    /// <summary>
    /// Lifecycle state of a <see cref="World{TWorld}"/>.
    /// The world progresses through these states in order: NotCreated → Created → Initialized.
    /// <see cref="World{TWorld}.Destroy"/> resets back to NotCreated.
    /// </summary>
    public enum WorldStatus {
        /// <summary>
        /// The world has not been created yet, or has been destroyed.
        /// No operations are valid in this state except <see cref="World{TWorld}.Create"/>.
        /// </summary>
        NotCreated,

        /// <summary>
        /// The world has been created (internal structures allocated) and is ready for type
        /// registration (via <c>World&lt;TWorld&gt;.Types().Component&lt;T&gt;()</c>,
        /// <c>Types().Tag&lt;T&gt;()</c>, etc.).
        /// Entity creation is not yet available — call <see cref="World{TWorld}.Initialize"/>
        /// to transition to <see cref="Initialized"/>.
        /// </summary>
        Created,

        /// <summary>
        /// The world is fully operational. Entities can be created, queries can run,
        /// and all ECS operations are available.
        /// </summary>
        Initialized
    }

    /// <summary>
    /// Configuration for creating a <see cref="World{TWorld}"/>.
    /// Controls initial capacities, threading model, and behavioral settings.
    /// Pass to <see cref="World{TWorld}.Create"/>.
    /// All parameters are optional - any unset value falls back to <see cref="WorldConfig.Default"/>.
    /// </summary>
    public struct WorldConfig {
        /// <summary>
        /// Initial capacity for the component type registry. Does not limit the total number of
        /// component types — the registry grows automatically. Setting this to a reasonable estimate
        /// avoids early reallocations. Default: 64.
        /// </summary>
        public uint? BaseComponentTypesCount;

        /// <summary>
        /// Number of threads to use for parallel query execution.
        /// 0 = single-threaded only (default).
        /// <see cref="MaxThreadCount"/> = use all available CPU threads (<see cref="MaxThreads"/> configuration).
        /// </summary>
        public uint? ThreadCount;

        /// <summary>
        /// Number of spin-wait iterations worker threads perform before blocking on a kernel event.
        /// Higher values reduce wake-up latency when multiple parallel queries run per frame
        /// (the worker stays in a CPU spin loop and catches the next query without a kernel transition).
        /// Lower values save CPU when queries are infrequent.
        /// <para>
        /// Default 256 (4096 in <see cref="MaxThreads"/> configuration).
        /// </para>
        /// </summary>
        public uint? WorkerSpinCount;

        /// <summary>
        /// Initial capacity for the cluster registry. Minimum 16. Grows automatically if more
        /// clusters are registered.
        /// </summary>
        public ushort? BaseClustersCapacity;

        /// <summary>
        /// If <c>true</c> (default), this world independently manages its own chunk allocation.
        /// If <c>false</c>, the world can share chunk address space with other worlds,
        /// enabling advanced multi-world streaming where the same entity slots are visible
        /// from multiple worlds. Most applications should use <c>true</c>.
        /// </summary>
        public bool? Independent;

        /// <summary>
        /// If <c>true</c>, the world tracks entity creation events in per-block bitmasks.
        /// This enables the <c>Created</c> query filter to match entities that were created
        /// since the last <see cref="World{TWorld}.ClearCreatedTracking"/> call.
        /// Default: <c>false</c> (opt-in).
        /// </summary>
        public bool? TrackCreated;

        /// <summary>
        /// Size of the tracking ring buffer (number of ticks of history to retain).
        /// When non-zero, change tracking (Added/Deleted/Changed) is versioned per tick.
        /// Each <see cref="World{TWorld}.Tick"/> call advances the world tick and rotates
        /// the ring buffer, preserving up to this many ticks of tracking history.
        /// Systems automatically see changes since their last execution via per-system tick tracking.
        /// <para>Default: 8. Minimum: 2.</para>
        /// </summary>
        public byte? TrackingBufferSize;

        /// <summary>
        /// Returns a default configuration suitable for most applications:
        /// 64 component types, 64 tag types, 16 clusters, single-threaded, independent world.
        /// </summary>
        /// <param name="independent">Whether the world should be independent (default: true).</param>
        /// <returns>A <see cref="WorldConfig"/> with sensible defaults.</returns>
        public static WorldConfig Default(bool independent = true) => new() {
            BaseComponentTypesCount = 64,
            ThreadCount = 0,
            WorkerSpinCount = 256,
            BaseClustersCapacity = 16,
            Independent = independent,
            TrackCreated = false,
            TrackingBufferSize = 8,
        };

        /// <summary>
        /// Returns a configuration that uses all available CPU threads for parallel queries.
        /// Otherwise, identical to <see cref="Default"/>.
        /// </summary>
        /// <param name="independent">Whether the world should be independent (default: true).</param>
        /// <returns>A <see cref="WorldConfig"/> with max-threads parallelism enabled.</returns>
        public static WorldConfig MaxThreads(bool? independent = null) => new() {
            ThreadCount = MaxThreadCount,
            WorkerSpinCount = 4096,
            Independent = independent,
        };

        /// <summary>
        /// All available CPU threads (typically <c>Environment.ProcessorCount</c>).
        /// </summary>
        public static uint MaxThreadCount {
            get {
                #if UNITY_WEBGL
                return 1;
                #else
                return (uint)Environment.ProcessorCount;
                #endif
            }
        }

        internal WorldConfig MergeWith(WorldConfig other) => new() {
            BaseComponentTypesCount = BaseComponentTypesCount ?? other.BaseComponentTypesCount,
            ThreadCount = ThreadCount ?? other.ThreadCount,
            WorkerSpinCount = WorkerSpinCount ?? other.WorkerSpinCount,
            BaseClustersCapacity = BaseClustersCapacity ?? other.BaseClustersCapacity,
            Independent = Independent ?? other.Independent,
            TrackCreated = TrackCreated ?? other.TrackCreated,
            TrackingBufferSize = TrackingBufferSize ?? other.TrackingBufferSize
        };
    }
}