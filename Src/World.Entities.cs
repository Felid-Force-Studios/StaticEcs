#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    #region TYPES

    public enum ChunkOwnerType : byte {
        Self,
        Other
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [StructLayout(LayoutKind.Explicit)]
    internal struct EntitiesChunk {
        [FieldOffset(0)] internal ulong notEmptyBlocks;
        [FieldOffset(0)] private long notEmptyBlocksSigned;
        [FieldOffset(8)] internal ulong fullBlocks;
        [FieldOffset(8)] private long fullBlocksSigned;
        [FieldOffset(72)] internal ulong[] entities;
        [FieldOffset(80)] internal ulong[] disabledEntities;
        [FieldOffset(88)] internal ulong[] loadedEntities;
        [FieldOffset(96)] internal ushort[] versions;
        [FieldOffset(104)] internal int clusterId;
        [FieldOffset(108)] internal int loadedEntitiesCount;
        [FieldOffset(112)] internal bool selfOwner;

        [MethodImpl(AggressiveInlining)]
        internal void SetNotEmptyBit(byte index) {
            var mask = 1UL << index;
            long orig, newVal;
            do {
                orig = notEmptyBlocksSigned;
                newVal = orig | (long) mask;
            } while (Interlocked.CompareExchange(ref notEmptyBlocksSigned, newVal, orig) != orig);
        }

        [MethodImpl(AggressiveInlining)]
        internal ulong ClearNotEmptyBit(byte index) {
            var mask = ~(1UL << index);
            long orig, newVal;
            do {
                orig = notEmptyBlocksSigned;
                newVal = orig & (long) mask;
            } while (Interlocked.CompareExchange(ref notEmptyBlocksSigned, newVal, orig) != orig);

            return (ulong) newVal;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetFullBit(byte index) {
            var mask = 1UL << index;
            long orig, newVal;
            do {
                orig = fullBlocksSigned;
                newVal = orig | (long) mask;
            } while (Interlocked.CompareExchange(ref fullBlocksSigned, newVal, orig) != orig);
        }

        [MethodImpl(AggressiveInlining)]
        public void ClearFullBit(byte index) {
            var mask = ~(1UL << index);
            long orig, newVal;
            do {
                orig = fullBlocksSigned;
                newVal = orig & (long) mask;
            } while (Interlocked.CompareExchange(ref fullBlocksSigned, newVal, orig) != orig);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct EntitiesCluster {
        internal uint[] chunks;
        internal uint[] loadedChunks;
        internal SpinLock clusterLock;
        internal uint chunksCount;
        internal uint loadedChunksCount;
        internal int freeChunkIndex;
        internal bool registered;
        internal bool disabled;
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct EntitiesChunkInfo {
        public readonly uint ChunkIdx;

        [MethodImpl(AggressiveInlining)]
        public EntitiesChunkInfo(uint chunk) {
            ChunkIdx = chunk;
        }

        public uint EntitiesFrom {
            [MethodImpl(AggressiveInlining)] get => ChunkIdx << Const.ENTITIES_IN_CHUNK_SHIFT;
        }

        public ushort EntitiesCapacity {
            [MethodImpl(AggressiveInlining)] get => Const.ENTITIES_IN_CHUNK;
        }
    }
    #endregion

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        
        public const ushort DEFAULT_CLUSTER = 0;

        #region API
        [MethodImpl(AggressiveInlining)]
        public static void OnCreateEntity(QueryFunctionWithEntity<WorldType> function) => Entities.Value.OnCreateEntity(function);

        [MethodImpl(AggressiveInlining)]
        public static uint CalculateEntitiesCount() => Entities.Value.CalculateEntitiesCount();

        [MethodImpl(AggressiveInlining)]
        public static uint CalculateLoadedEntitiesCount() => Entities.Value.CalculateLoadedEntitiesCount();

        [MethodImpl(AggressiveInlining)]
        public static uint CalculateEntitiesCapacity() => Entities.Value.CalculateEntitiesCapacity();

        [MethodImpl(AggressiveInlining)]
        public static void DestroyAllEntities() => Entities.Value.DestroyAllEntities();


        [MethodImpl(AggressiveInlining)]
        public static void RegisterCluster(ushort clusterId) => Entities.Value.RegisterCluster(clusterId);

        [MethodImpl(AggressiveInlining)]
        public static bool ClusterIsRegistered(ushort clusterId) => Entities.Value.ClusterIsRegistered(clusterId);

        [MethodImpl(AggressiveInlining)]
        public static bool TryFreeCluster(ushort clusterId) => Entities.Value.TryFreeCluster(clusterId);

        [MethodImpl(AggressiveInlining)]
        public static void FreeCluster(ushort clusterId) => Entities.Value.FreeCluster(clusterId);

        [MethodImpl(AggressiveInlining)]
        public static void SetActiveCluster(ushort clusterId, bool active) => Entities.Value.SetActiveCluster(clusterId, active);

        [MethodImpl(AggressiveInlining)]
        public static bool ClusterIsActive(ushort clusterId) => Entities.Value.ClusterIsActive(clusterId);

        [MethodImpl(AggressiveInlining)]
        public static void DestroyAllEntitiesInCluster(ushort clusterId) => Entities.Value.DestroyAllEntitiesInCluster(clusterId);

        [MethodImpl(AggressiveInlining)]
        public static void UnloadCluster(ushort clusterId) => Entities.Value.UnloadCluster(clusterId);

        [MethodImpl(AggressiveInlining)]
        public static ReadOnlySpan<uint> GetClusterChunks(ushort clusterId) => Entities.Value.GetClusterChunks(clusterId);
        
        [MethodImpl(AggressiveInlining)]
        public static ReadOnlySpan<uint> GetClusterLoadedChunks(ushort clusterId) => Entities.Value.GetClusterLoadedChunks(clusterId);


        [MethodImpl(AggressiveInlining)]
        public static bool TryFindNextSelfFreeChunk(out EntitiesChunkInfo chunkInfo) => Entities.Value.TryFindNextSelfFreeChunk(out chunkInfo);

        [MethodImpl(AggressiveInlining)]
        public static EntitiesChunkInfo FindNextSelfFreeChunk() => Entities.Value.FindNextSelfFreeChunk();

        [MethodImpl(AggressiveInlining)]
        public static bool TryRegisterChunk(uint chunkIdx, ChunkOwnerType owner = ChunkOwnerType.Self, ushort clusterId = DEFAULT_CLUSTER) => Entities.Value.TryRegisterChunk(chunkIdx, owner, clusterId);

        [MethodImpl(AggressiveInlining)]
        public static void RegisterChunk(uint chunkIdx, ChunkOwnerType owner = ChunkOwnerType.Self, ushort clusterId = DEFAULT_CLUSTER) => Entities.Value.RegisterChunk(chunkIdx, owner, clusterId);

        [MethodImpl(AggressiveInlining)]
        public static bool ChunkIsRegistered(uint chunkIdx) => Entities.Value.ChunkIsRegistered(chunkIdx);

        [MethodImpl(AggressiveInlining)]
        public static void FreeChunk(uint chunkIdx) => Entities.Value.FreeChunk(chunkIdx);

        [MethodImpl(AggressiveInlining)]
        public static void DestroyAllEntitiesInChunk(uint chunkIdx) => Entities.Value.DestroyAllEntitiesInChunk(chunkIdx);

        [MethodImpl(AggressiveInlining)]
        public static void UnloadChunk(uint chunkIdx) => Entities.Value.UnloadChunk(chunkIdx);

        [MethodImpl(AggressiveInlining)]
        public static ushort GetChunkClusterId(uint chunkIdx) => Entities.Value.GetChunkClusterId(chunkIdx);

        [MethodImpl(AggressiveInlining)]
        public static void ChangeChunkCluster(uint chunkIdx, ushort clusterId) => Entities.Value.ChangeChunkCluster(chunkIdx, clusterId);

        [MethodImpl(AggressiveInlining)]
        public static bool HasEntitiesInChunk(uint chunkIdx) {
            #if FFS_ECS_DEBUG
            AssertWorldIsInitialized(WorldTypeName);
            AssertMultiThreadNotActive(WorldTypeName);
            AssertChunkIsRegistered(WorldTypeName, chunkIdx);
            #endif

            return Entities.Value.chunks[chunkIdx].notEmptyBlocks != 0;
        }
        
        [MethodImpl(AggressiveInlining)]
        public static bool HasLoadedEntitiesInChunk(uint chunkIdx) {
            #if FFS_ECS_DEBUG
            AssertWorldIsInitialized(WorldTypeName);
            AssertMultiThreadNotActive(WorldTypeName);
            AssertChunkIsRegistered(WorldTypeName, chunkIdx);
            #endif

            return Entities.Value.chunks[chunkIdx].loadedEntitiesCount > 0;
        }

        [MethodImpl(AggressiveInlining)]
        public static void ChangeChunkOwner(uint chunkIdx, ChunkOwnerType owner) => Entities.Value.ChangeChunkOwner(chunkIdx, owner);

        [MethodImpl(AggressiveInlining)]
        public static ChunkOwnerType GetChunkOwner(uint chunkIdx) => Entities.Value.GetChunkOwnerType(chunkIdx);
        #endregion

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        internal struct Entities {
            internal static Entities Value;

            #region FIELDS
            // TEMPLATES
            internal ushort[] versionEntitiesTemplate;

            // CHUNKS
            internal EntitiesChunk[] chunks;
            internal uint[] selfFreeChunks;
            internal int selfFreeChunksCount;

            // CLUSTERS AND CALLBACKS
            internal EntitiesCluster[] clusters;
            internal ushort[] activeClusters;
            internal QueryFunctionWithEntity<WorldType>[] onCreate;
            internal uint onCreateCount;
            internal ushort activeClustersCount;

            // QUERY
            internal QueryData[] queriesToUpdateOnDisable;
            internal QueryData[] queriesToUpdateOnEnable;
            internal QueryData[] queriesToUpdateOnDestroy;
            #if FFS_ECS_DEBUG
            internal int _blockerDestroy;
            internal int _blockerDisable;
            internal int _blockerEnable;
            #endif
            internal byte queriesToUpdateOnDestroyCount;
            internal byte queriesToUpdateOnDisableCount;
            internal byte queriesToUpdateOnEnableCount;

            // CONFIG
            internal bool independentWorld;
            #endregion

            #region BASE
            [MethodImpl(AggressiveInlining)]
            internal void Create(bool independent, ushort baseClustersCapacity) {
                independentWorld = independent;

                // TEMPLATES
                versionEntitiesTemplate = new ushort[Const.ENTITIES_IN_CHUNK];
                for (var i = 0; i < Const.ENTITIES_IN_CHUNK; i++) {
                    versionEntitiesTemplate[i] = 1;
                }

                // CALLBACKS
                onCreate = new QueryFunctionWithEntity<WorldType>[16];
                onCreateCount = 0;
                
                // CLUSTERS
                clusters = new EntitiesCluster[baseClustersCapacity];
                activeClusters = new ushort[baseClustersCapacity];
                for (ushort i = 0; i < baseClustersCapacity; i++) {
                    ref var cluster = ref clusters[i];
                    cluster.chunks = new uint[Math.Max((int) activeClustersCount, 32)];
                    cluster.loadedChunks = new uint[Math.Max((int) activeClustersCount, 32)];
                    cluster.clusterLock = new SpinLock(false);
                    cluster.chunksCount = 0;
                    cluster.loadedChunksCount = 0;
                    cluster.freeChunkIndex = -1;
                }

                // QUERY
                queriesToUpdateOnDisable = new QueryData[Const.MAX_NESTED_QUERY];
                queriesToUpdateOnEnable = new QueryData[Const.MAX_NESTED_QUERY];
                queriesToUpdateOnDestroy = new QueryData[Const.MAX_NESTED_QUERY];
                queriesToUpdateOnDestroyCount = 0;
                queriesToUpdateOnDisableCount = 0;
                queriesToUpdateOnEnableCount = 0;
                #if FFS_ECS_DEBUG
                _blockerDestroy = 0;
                _blockerDisable = 0;
                _blockerEnable = 0;
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal void Initialize(uint chunksCapacity) {
                // CHUNKS
                chunks = new EntitiesChunk[chunksCapacity];
                for (var i = (int) chunksCapacity - 1; i >= 0; i--) {
                    ref var chunk = ref chunks[i];
                    FillEntitiesVersions(ref chunk.versions);
                    chunk.entities = new ulong[Const.BLOCK_IN_CHUNK];
                    chunk.disabledEntities = new ulong[Const.BLOCK_IN_CHUNK];
                    chunk.loadedEntities = new ulong[Const.BLOCK_IN_CHUNK];
                    chunk.clusterId = -1;
                    chunk.selfOwner = independentWorld;
                }

                selfFreeChunksCount = 0;
                selfFreeChunks = new uint[chunksCapacity];
                if (independentWorld) {
                    for (var i = (int) chunksCapacity - 1; i >= 0; i--) {
                        selfFreeChunks[selfFreeChunksCount++] = (uint) i;
                    }
                }
                
                ModuleComponents.Value.Initialize(chunksCapacity);
                ModuleTags.Value.Initialize(chunksCapacity);
            }

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                independentWorld = default;

                // TEMPLATES
                versionEntitiesTemplate = null;

                // CHUNKS
                chunks = null;
                selfFreeChunks = null;
                selfFreeChunksCount = 0;

                // CLUSTERS
                activeClustersCount = 0;
                clusters = null;
                activeClusters = null;

                // CALLBACKS
                onCreate = null;
                onCreateCount = 0;

                // QUERY
                queriesToUpdateOnDisable = null;
                queriesToUpdateOnEnable = null;
                queriesToUpdateOnDestroy = null;
                queriesToUpdateOnDestroyCount = 0;
                queriesToUpdateOnDisableCount = 0;
                queriesToUpdateOnEnableCount = 0;
                #if FFS_ECS_DEBUG
                _blockerDestroy = 0;
                _blockerDisable = 0;
                _blockerEnable = 0;
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            private void Resize(uint chunksCapacity) {
                var oldChunksCapacity = chunks.Length;

                Array.Resize(ref chunks, (int) chunksCapacity);
                Array.Resize(ref selfFreeChunks, (int) chunksCapacity);
                
                for (var i = (int) chunksCapacity - 1; i >= oldChunksCapacity; i--) {
                    ref var chunk = ref chunks[i];
                    FillEntitiesVersions(ref chunk.versions);
                    chunk.entities = new ulong[Const.BLOCK_IN_CHUNK];
                    chunk.disabledEntities = new ulong[Const.BLOCK_IN_CHUNK];
                    chunk.loadedEntities = new ulong[Const.BLOCK_IN_CHUNK];
                    chunk.clusterId = -1;
                    chunk.selfOwner = independentWorld;
                }
                
                if (independentWorld) {
                    for (var i = (int) chunksCapacity - 1; i >= oldChunksCapacity; i--) {
                        selfFreeChunks[selfFreeChunksCount++] = (uint) i;
                    }
                }

                ModuleComponents.Value.Resize(chunksCapacity);
                ModuleTags.Value.Resize(chunksCapacity);

                #if FFS_ECS_EVENTS
                OnWorldResized(chunksCapacity);
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal void DestroyAllEntities() {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                #endif
                
                for (var i = activeClustersCount - 1; i >= 0; i--) {
                    DestroyAllEntitiesInCluster(activeClusters[i]);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                var clustersCount = activeClustersCount - 1;
                for (var i = clustersCount; i >= 0; i--) {
                    FreeCluster(activeClusters[i]);
                }
            }
            #endregion

            #region ENTITIES
            [MethodImpl(AggressiveInlining)]
            internal void OnCreateEntity(QueryFunctionWithEntity<WorldType> function) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                if (onCreateCount == onCreate.Length) {
                    Array.Resize(ref onCreate, (int) (onCreateCount << 1));
                }

                onCreate[onCreateCount++] = function;
            }

            [MethodImpl(AggressiveInlining)]
            internal uint CalculateEntitiesCount() {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                var count = 0;
                for (ushort i = 0; i < activeClustersCount; i++) {
                    ref var cluster = ref clusters[activeClusters[i]];

                    var chunksCount = cluster.chunksCount;
                    for (var j = 0; j < chunksCount; j++) {
                        ref var chunk = ref chunks[cluster.chunks[j]];
                        var notEmptyBlocks = chunk.notEmptyBlocks;
                        while (notEmptyBlocks > 0) {
                            var blockIdx = Utils.PopLsb(ref notEmptyBlocks);
                            count += chunk.entities[blockIdx].PopCnt();
                        }
                    }
                }

                return (uint) count;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal uint CalculateLoadedEntitiesCount() {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                var count = 0;
                for (ushort i = 0; i < activeClustersCount; i++) {
                    ref var cluster = ref clusters[activeClusters[i]];

                    var chunksCount = cluster.loadedChunksCount;
                    for (var j = 0; j < chunksCount; j++) {
                        count += chunks[cluster.loadedChunks[j]].loadedEntitiesCount;
                    }
                }

                return (uint) count;
            }

            [MethodImpl(AggressiveInlining)]
            internal uint CalculateEntitiesCapacity() {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                return (uint) (chunks.Length << Const.ENTITIES_IN_CHUNK_SHIFT);
            }

            [MethodImpl(AggressiveInlining)]
            internal bool HasFreeEntities(ushort clusterId) {
                return CalculateFreeEntitiesCount(clusterId) > 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal uint CalculateFreeEntitiesCount(ushort clusterId) {
                uint count = 0;
                ref var cluster = ref clusters[clusterId];
                for (var i = 0; i < cluster.chunksCount; i++) {
                    ref var chunk = ref chunks[cluster.chunks[i]];
                    if (chunk.selfOwner) {
                        var fullBlocksInv = ~chunk.fullBlocks;
                        while (fullBlocksInv != 0) {
                            var blockIdx = Utils.PopLsb(ref fullBlocksInv);
                            var free = Utils.PopCnt(~chunk.entities[blockIdx]);
                            count += (uint) free;
                        
                        }
                    }
                }

                return count;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool EntityIsNotDestroyed(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                #endif
                
                if (entity.id > 0) {
                    var eid = entity.id - Const.ENTITY_ID_OFFSET;
                    var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                    var chunkEntityIdx = (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                    var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                    var blockEntityIdx = chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK;

                    return chunkIdx < chunks.Length && (chunks[chunkIdx].entities[blockIdx] & (1UL << blockEntityIdx)) != 0;
                }

                return false;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool GidIsActual(EntityGID gid) {
                var eid = gid.Id;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityIdx = chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK;

                if (chunkIdx < chunks.Length) {
                    ref var chunk = ref chunks[chunkIdx];
                    return chunk.clusterId == gid.ClusterId
                           && gid.Version == chunk.versions[chunkEntityIdx]
                           && (chunk.entities[blockIdx] & (1UL << blockEntityIdx)) != 0;
                }

                return false;
            }        
            
            [MethodImpl(AggressiveInlining)]
            internal bool EntityIsLoaded(Entity entity) {
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityIdx = chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK;

                return (chunks[chunkIdx].loadedEntities[blockIdx] & (1UL << blockEntityIdx)) != 0;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool GidIsLoaded(EntityGID gid) {
                var eid = gid.Id;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityIdx = chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK;

                return (chunks[chunkIdx].loadedEntities[blockIdx] & (1UL << blockEntityIdx)) != 0;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool GidIsLoadedAndActual(EntityGID gid) {
                var eid = gid.Id;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityMask = 1UL << (chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);

                ref var chunk = ref chunks[chunkIdx];
                var loadedEntities = chunk.loadedEntities;

                return (loadedEntities[blockIdx] & blockEntityMask) != 0
                       && chunk.clusterId == gid.ClusterId
                       && gid.Version == chunk.versions[chunkEntityIdx]
                       && (chunk.entities[blockIdx] & blockEntityMask) != 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort EntityVersion(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                return chunks[eid >> Const.ENTITIES_IN_CHUNK_SHIFT].versions[(ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK)];
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort EntityClusterId(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                return (ushort) chunks[eid >> Const.ENTITIES_IN_CHUNK_SHIFT].clusterId;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool EntityIsSelfOwned(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                return chunks[eid >> Const.ENTITIES_IN_CHUNK_SHIFT].selfOwner;
            }

            [MethodImpl(AggressiveInlining)]
            internal EntityGID EntityGID(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var version = chunks[chunkIdx].versions[(ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK)];
                var clusterId = (ushort) chunks[chunkIdx].clusterId;
                return new EntityGID(eid, version, clusterId);
            }

            [MethodImpl(AggressiveInlining)]
            internal EntityGIDCompact EntityGIDCompact(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var version = chunks[chunkIdx].versions[(ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK)];
                var clusterId = (ushort) chunks[chunkIdx].clusterId;
                return new EntityGIDCompact(eid, version, clusterId);
            }

            [MethodImpl(AggressiveInlining)]
            internal void UpEntityVersion(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                ref var version = ref chunks[eid >> Const.ENTITIES_IN_CHUNK_SHIFT].versions[(ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK)];
                version = version == ushort.MaxValue ? Const.US1 : (ushort) (version + 1);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool IsDisabledEntity(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityIdx = chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK;
                return (chunks[chunkIdx].disabledEntities[blockIdx] & (1UL << blockEntityIdx)) != 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool IsEnabledEntity(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityIdx = chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK;
                return (chunks[chunkIdx].disabledEntities[blockIdx] & (1UL << blockEntityIdx)) == 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal void DisableEntity(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                AssertNotBlockedByQuery(EntityTypeName, entity, _blockerDisable);
                AssertNotBlockedByParallelQuery(EntityTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityIdx = chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK;
                chunks[chunkIdx].disabledEntities[blockIdx] |= 1UL << blockEntityIdx;
                for (uint i = 0; i < queriesToUpdateOnDisableCount; i++) {
                    queriesToUpdateOnDisable[i].Update(~(1UL << blockEntityIdx), eid);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void EnableEntity(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                AssertNotBlockedByQuery(EntityTypeName, entity, _blockerEnable);
                AssertNotBlockedByParallelQuery(EntityTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityIdx = chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK;
                chunks[chunkIdx].disabledEntities[blockIdx] &= ~(1UL << blockEntityIdx);
                for (uint i = 0; i < queriesToUpdateOnEnableCount; i++) {
                    queriesToUpdateOnEnable[i].Update(~(1UL << blockEntityIdx), eid);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void CreateEntity(ushort clusterId, out Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                if (!TryCreateEntity(clusterId, out entity)) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CreateEntity, ran out of space in the attached chunks");
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryCreateEntity(ushort clusterId, out Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                
                ref var cluster = ref clusters[clusterId];

                uint chunkIdx;
                if (cluster.freeChunkIndex >= 0) {
                    chunkIdx = (uint) cluster.freeChunkIndex;
                } else {
                    if (selfFreeChunksCount == 0) {
                        if (independentWorld) {
                            Resize((uint) (chunks.Length + 4));
                        } else {
                            entity.id = default;
                            return false;
                        }
                    }

                    chunkIdx = selfFreeChunks[--selfFreeChunksCount];
                    chunks[chunkIdx].clusterId = clusterId;
                    cluster.freeChunkIndex = (int) chunkIdx;
                    if (cluster.chunksCount == cluster.chunks.Length) {
                        Array.Resize(ref cluster.chunks, (int) (cluster.chunksCount << 1));
                        Array.Resize(ref cluster.loadedChunks, (int) (cluster.chunksCount << 1));
                    }

                    cluster.chunks[cluster.chunksCount++] = chunkIdx;
                }

                ref var chunk = ref chunks[chunkIdx];
                var blockIdx = Utils.Lsb(~chunk.fullBlocks);
                ref var entitiesMask = ref chunk.entities[blockIdx];
                var blockEntityIdx = Utils.Lsb(~entitiesMask);

                if (chunk.loadedEntitiesCount == 0) {
                    cluster.loadedChunks[cluster.loadedChunksCount++] = chunkIdx;
                    ModuleComponents.Value.bitMask.InitChunk(chunkIdx);
                    ModuleTags.Value.bitMask.InitChunk(chunkIdx);
                }

                chunk.loadedEntitiesCount++;

                if (entitiesMask == 0) {
                    chunk.notEmptyBlocks |= 1UL << blockIdx;
                }

                entitiesMask |= 1UL << blockEntityIdx;
                chunk.loadedEntities[blockIdx] |= 1UL << blockEntityIdx;
                
                if (entitiesMask == ulong.MaxValue) {
                    chunk.fullBlocks |= 1UL << blockIdx;
                    if (chunk.fullBlocks == ulong.MaxValue) {
                        TryFindNextFreeChunkInCluster(ref cluster);
                    }
                }

                entity.id = (uint) ((chunkIdx << Const.ENTITIES_IN_CHUNK_SHIFT) + (blockIdx << Const.ENTITIES_IN_BLOCK_SHIFT) + blockEntityIdx + Const.ENTITY_ID_OFFSET);
                for (var i = 0; i < onCreateCount; i++) {
                    onCreate[i](entity);
                }

                #if FFS_ECS_EVENTS
                OnEntityCreated(entity);
                #endif
                return true;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void CreateEntity(uint chunkIdx, out Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertChunkIsRegistered(EntityTypeName, chunkIdx);
                #endif
                if (!TryCreateEntity(chunkIdx, out entity)) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CreateEntity, ran out of space in chunk {chunkIdx}");
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryCreateEntity(uint chunkIdx, out Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertChunkIsRegistered(EntityTypeName, chunkIdx);
                Assert(EntityTypeName, chunks[chunkIdx].selfOwner, $"Chunk {chunkIdx} has the ownership type Other");
                #endif
                
                ref var chunk = ref chunks[chunkIdx];
                
                if (chunk.fullBlocks == ulong.MaxValue) {
                    entity.id = default;
                    return false;
                }
                
                var blockIdx = Utils.Lsb(~chunk.fullBlocks);
                ref var entitiesMask = ref chunk.entities[blockIdx];
                var blockEntityIdx = Utils.Lsb(~entitiesMask);

                if (chunk.loadedEntitiesCount == 0) {
                    ref var cluster = ref clusters[chunk.clusterId];
                    cluster.loadedChunks[cluster.loadedChunksCount++] = chunkIdx;
                    ModuleComponents.Value.bitMask.InitChunk(chunkIdx);
                    ModuleTags.Value.bitMask.InitChunk(chunkIdx);
                }

                chunk.loadedEntitiesCount++;

                if (entitiesMask == 0) {
                    chunk.notEmptyBlocks |= 1UL << blockIdx;
                }

                entitiesMask |= 1UL << blockEntityIdx;
                chunk.loadedEntities[blockIdx] |= 1UL << blockEntityIdx;
                
                if (entitiesMask == ulong.MaxValue) {
                    chunk.fullBlocks |= 1UL << blockIdx;
                    ref var cluster = ref clusters[chunk.clusterId];
                    if (chunk.fullBlocks == ulong.MaxValue && cluster.freeChunkIndex == chunkIdx) {
                        TryFindNextFreeChunkInCluster(ref cluster);
                    }
                }

                entity.id = (uint) ((chunkIdx << Const.ENTITIES_IN_CHUNK_SHIFT) + (blockIdx << Const.ENTITIES_IN_BLOCK_SHIFT) + blockEntityIdx + Const.ENTITY_ID_OFFSET);
                for (var i = 0; i < onCreateCount; i++) {
                    onCreate[i](entity);
                }

                #if FFS_ECS_EVENTS
                OnEntityCreated(entity);
                #endif
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            internal void CreateEntity(EntityGID gid, out Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                #endif

                var eid = gid.Id;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityIdx = chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK;

                if (chunkIdx >= chunks.Length) {
                    Resize(chunkIdx.Normalize(4));
                }

                ref var chunk = ref chunks[chunkIdx];

                if (!ClusterIsRegistered(gid.ClusterId)) {
                    RegisterCluster(gid.ClusterId);
                }

                if (chunk.clusterId == -1) {
                    RegisterChunk(chunkIdx, ChunkOwnerType.Other, gid.ClusterId);
                }
                
                #if FFS_ECS_DEBUG
                AssertGidIsNotLoaded(EntityTypeName, gid);
                Assert(EntityTypeName, !chunk.selfOwner, "It is possible to create entities with GID only for chunks with Other ownership");
                if (chunk.clusterId != gid.ClusterId) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CreateEntity, Chunk cluster {chunk.clusterId} not equal GID cluster {gid.ClusterId}");
                #endif

                chunk.clusterId = gid.ClusterId;
                
                if (chunk.loadedEntitiesCount == 0) {
                    ref var cluster = ref clusters[chunk.clusterId];
                    cluster.loadedChunks[cluster.loadedChunksCount++] = chunkIdx;
                    ModuleComponents.Value.bitMask.InitChunk(chunkIdx);
                    ModuleTags.Value.bitMask.InitChunk(chunkIdx);
                }

                chunk.loadedEntitiesCount++;

                ref var entitiesMask = ref chunk.entities[blockIdx];
                if (entitiesMask == 0) {
                    chunk.notEmptyBlocks |= 1UL << blockIdx;
                }

                entitiesMask |= 1UL << blockEntityIdx;
                chunk.loadedEntities[blockIdx] |= 1UL << blockEntityIdx;
                chunk.versions[chunkEntityIdx] = gid.Version;
                                
                if (entitiesMask == ulong.MaxValue) {
                    chunk.fullBlocks |= 1UL << blockIdx;
                    ref var cluster = ref clusters[chunk.clusterId];
                    if (chunk.fullBlocks == ulong.MaxValue && cluster.freeChunkIndex == chunkIdx) {
                        TryFindNextFreeChunkInCluster(ref cluster);
                    }
                }

                entity = new Entity(eid);
                for (var i = 0; i < onCreateCount; i++) {
                    onCreate[i](entity);
                }

                #if FFS_ECS_EVENTS
                OnEntityCreated(entity);
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal void LoadEntity(EntityGID gid, out Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertGidIsActual(EntityTypeName, gid);
                AssertGidIsNotLoaded(EntityTypeName, gid);
                #endif

                var eid = gid.Id;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityIdx = chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK;

                ref var chunk = ref chunks[chunkIdx];
                #if FFS_ECS_DEBUG
                if (!clusters[gid.ClusterId].registered) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: LoadEntity, Cluster {chunk.clusterId} not registered");
                if (chunk.clusterId != gid.ClusterId) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: LoadEntity, Chunk cluster {chunk.clusterId} not equal GID cluster {gid.ClusterId}");
                if (chunk.versions[chunkEntityIdx] != gid.Version) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: LoadEntity, Entity version {chunk.versions[chunkEntityIdx]} not equal GID version {gid.Version}");
                #endif
                
                if (chunk.loadedEntitiesCount == 0) {
                    ref var cluster = ref clusters[chunk.clusterId];
                    cluster.loadedChunks[cluster.loadedChunksCount++] = chunkIdx;
                    ModuleComponents.Value.bitMask.InitChunk(chunkIdx);
                    ModuleTags.Value.bitMask.InitChunk(chunkIdx);
                }

                chunk.loadedEntitiesCount++;
                chunk.loadedEntities[blockIdx] |= 1UL << blockEntityIdx;

                entity = new Entity(eid);
                for (var i = 0; i < onCreateCount; i++) {
                    onCreate[i](entity);
                }

                #if FFS_ECS_EVENTS
                OnEntityCreated(entity);
                #endif
            }        
            
            [MethodImpl(AggressiveInlining)]
            internal void UnloadEntity(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                AssertNotBlockedByParallelQuery(EntityTypeName, entity);
                #endif
                
                ModuleTags.Value.DestroyEntity(entity);
                ModuleComponents.Value.DestroyEntity(entity);
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityInvMask = ~(1UL << (chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK));

                ref var chunk = ref chunks[chunkIdx];
                
                chunk.loadedEntities[blockIdx] &= blockEntityInvMask;
                chunk.loadedEntitiesCount--;
                if (chunk.loadedEntitiesCount == 0) {
                    RemoveLoadedChunkFromCluster(ref clusters[chunk.clusterId], chunkIdx);
                    ModuleComponents.Value.bitMask.FreeChunk(chunkIdx);
                    ModuleTags.Value.bitMask.FreeChunk(chunkIdx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void DestroyEntity(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                AssertNotBlockedByParallelQuery(EntityTypeName, entity);
                AssertNotBlockedByQuery(EntityTypeName, entity, _blockerDestroy);
                #endif
                
                ModuleTags.Value.DestroyEntity(entity);
                ModuleComponents.Value.DestroyEntity(entity);

                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = (byte) (chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT);
                var blockEntityInvMask = ~(1UL << (chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK));

                ref var chunk = ref chunks[chunkIdx];
                ref var entitiesMask = ref chunk.entities[blockIdx];
                
                if (entitiesMask == ulong.MaxValue) {
                    chunk.ClearFullBit(blockIdx);
                }

                entitiesMask &= blockEntityInvMask;
                chunk.disabledEntities[blockIdx] &= blockEntityInvMask;
                chunk.loadedEntities[blockIdx] &= blockEntityInvMask;
                ref var version = ref chunk.versions[chunkEntityIdx];
                version = version == ushort.MaxValue ? Const.US1 : (ushort) (version + 1);
                
                if (entitiesMask == 0) {
                    chunk.ClearNotEmptyBit(blockIdx);
                }

                if (Interlocked.Decrement(ref chunk.loadedEntitiesCount) == 0) {
                    ref var cluster = ref clusters[chunk.clusterId];

                    var taken = false;
                    cluster.clusterLock.Enter(ref taken);
                    #if FFS_ECS_DEBUG
                    if (!taken) throw new StaticEcsException($"Failed to acquire cluster lock for cluster {chunk.clusterId}");
                    #endif
                    ModuleComponents.Value.bitMask.FreeChunk(chunkIdx);
                    ModuleTags.Value.bitMask.FreeChunk(chunkIdx);
                    RemoveLoadedChunkFromCluster(ref cluster, chunkIdx);
                    cluster.clusterLock.Exit();
                }

                for (uint i = 0; i < queriesToUpdateOnDestroyCount; i++) {
                    queriesToUpdateOnDestroy[i].Update(blockEntityInvMask, eid);
                }

                #if FFS_ECS_EVENTS
                OnEntityDestroyed(entity);
                #endif
            }
            #endregion

            #region CHUNKS_AND_CLUSTER
            [MethodImpl(AggressiveInlining)]
            internal void RegisterCluster(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertClusterIsNotRegistered(WorldTypeName, clusterId);
                AssertMultiThreadNotActive(WorldTypeName);
                #endif
                
                if (clusterId >= clusters.Length) {
                    ResizeClusters((int)Utils.CalculateSize(clusterId));
                }

                clusters[clusterId].registered = true;
                activeClusters[activeClustersCount++] = clusterId;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool ClusterIsRegistered(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                return clusterId < clusters.Length && clusters[clusterId].registered;
            }

            [MethodImpl(AggressiveInlining)]
            private void ResizeClusters(int newClustersCapacity) {
                var oldClustersCapacity = clusters.Length;

                Array.Resize(ref clusters, newClustersCapacity);
                Array.Resize(ref activeClusters, newClustersCapacity);

                for (var i = oldClustersCapacity; i < newClustersCapacity; i++) {
                    ref var cluster = ref clusters[i];
                    cluster.chunks = new uint[Math.Max((int) activeClustersCount, 32)];
                    cluster.loadedChunks = new uint[Math.Max((int) activeClustersCount, 32)];
                    cluster.clusterLock = new SpinLock(false);
                    cluster.chunksCount = 0;
                    cluster.loadedChunksCount = 0;
                    cluster.freeChunkIndex = -1;
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void SetActiveCluster(ushort clusterId, bool active) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertQueryNotActive(WorldTypeName);
                AssertClusterIsRegistered(WorldTypeName, clusterId);
                #endif

                clusters[clusterId].disabled = !active;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool ClusterIsActive(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertQueryNotActive(WorldTypeName);
                AssertClusterIsRegistered(WorldTypeName, clusterId);
                #endif

                return !clusters[clusterId].disabled;
            }

            [MethodImpl(AggressiveInlining)]
            internal void FreeCluster(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertQueryNotActive(WorldTypeName);
                #endif

                if (!TryFreeCluster(clusterId)) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>", "FreeCluster", $"Cluster {clusterId} not registered");
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryFreeCluster(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertQueryNotActive(WorldTypeName);
                #endif

                if (clusterId >= clusters.Length) {
                    return false;
                }
                
                ref var cluster = ref clusters[clusterId];
                if (cluster.registered) {
                    var chunksCount = cluster.chunksCount;
                    for (var i = (int) chunksCount - 1; i >= 0; i--) {
                        var chunkIdx = cluster.chunks[i];
                        ref var chunk = ref chunks[chunkIdx];

                        if (chunk.notEmptyBlocks != 0) {
                            ModuleComponents.Value.ClearChunk(chunkIdx);
                            ModuleTags.Value.ClearChunk(chunkIdx);
    
                            chunk.fullBlocks = 0;
                            Array.Clear(chunk.entities, 0, Const.BLOCK_IN_CHUNK);
                            Array.Clear(chunk.disabledEntities, 0, Const.BLOCK_IN_CHUNK);
                            Array.Clear(chunk.loadedEntities, 0, Const.BLOCK_IN_CHUNK);
                            if (chunk.loadedEntitiesCount > 0) {
                                ModuleComponents.Value.bitMask.FreeChunk(chunkIdx);
                                ModuleTags.Value.bitMask.FreeChunk(chunkIdx);
                            }
                            chunk.loadedEntitiesCount = 0;
                        }

                        chunk.notEmptyBlocks = 0;
                        chunk.clusterId = -1;
                        chunk.selfOwner = independentWorld;
                    }
                    
                    for (var i = activeClustersCount - 1; i >= 0; i--) {
                        if (activeClusters[i] == clusterId) {
                            activeClusters[i] = activeClusters[--activeClustersCount];
                            break;
                        }
                    }

                    cluster.freeChunkIndex = -1;
                    cluster.chunksCount = 0;
                    cluster.loadedChunksCount = 0;
                    cluster.registered = false;
                    cluster.disabled = false;
                    return true;
                }

                return false;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void DestroyAllEntitiesInCluster(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertQueryNotActive(WorldTypeName);
                #endif
                
                ref var cluster = ref clusters[clusterId];

                var chunksCount = cluster.chunksCount;
                for (var i = (int) chunksCount - 1; i >= 0; i--) {
                    DestroyAllEntitiesInChunk(cluster.chunks[i]);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void UnloadCluster(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertQueryNotActive(WorldTypeName);
                AssertClusterIsRegistered(WorldTypeName, clusterId);
                #endif
                
                ref var cluster = ref clusters[clusterId];

                var chunksCount = cluster.chunksCount;
                for (var i = (int) chunksCount - 1; i >= 0; i--) {
                    UnloadChunk(cluster.chunks[i]);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public ReadOnlySpan<uint> GetClusterChunks(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertClusterIsRegistered(WorldTypeName, clusterId);
                #endif

                ref var cluster = ref clusters[clusterId];
                return new ReadOnlySpan<uint>(cluster.chunks, 0, (int) cluster.chunksCount);
            }
            
            [MethodImpl(AggressiveInlining)]
            public ReadOnlySpan<uint> GetClusterLoadedChunks(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertClusterIsRegistered(WorldTypeName, clusterId);
                #endif

                ref var cluster = ref clusters[clusterId];
                return new ReadOnlySpan<uint>(cluster.loadedChunks, 0, (int) cluster.loadedChunksCount);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void RegisterChunk(uint chunkIdx, ChunkOwnerType owner, ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertClusterIsRegistered(WorldTypeName, clusterId);
                #endif
                
                if (!TryRegisterChunk(chunkIdx, owner, clusterId)) {
                    throw new StaticEcsException($"Chunk {chunkIdx} already registered");
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryRegisterChunk(uint chunkIdx, ChunkOwnerType owner, ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertClusterIsRegistered(WorldTypeName, clusterId);
                #endif
                
                if (chunkIdx >= chunks.Length) {
                    Resize(chunkIdx.Normalize(4));
                }
                
                ref var chunk = ref chunks[chunkIdx];
                
                if (chunk.clusterId != -1) {
                    return false;
                }

                chunk.clusterId = clusterId;
                chunk.selfOwner = owner == ChunkOwnerType.Self;
                
                ref var cluster = ref clusters[clusterId];
                if (cluster.chunksCount == cluster.chunks.Length) {
                    Array.Resize(ref cluster.chunks, (int) (cluster.chunksCount << 1));
                    Array.Resize(ref cluster.loadedChunks, (int) (cluster.chunksCount << 1));
                }

                cluster.chunks[cluster.chunksCount++] = chunkIdx;
                if (chunk.selfOwner) {
                    selfFreeChunks[selfFreeChunksCount++] = chunkIdx;
                } else {
                    RemoveFromSelfFreeChunks(chunkIdx);
                }
                return true;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool ChunkIsRegistered(uint chunkIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                return chunkIdx < chunks.Length && chunks[chunkIdx].clusterId != -1;
            }

            [MethodImpl(AggressiveInlining)]
            internal void FreeChunk(uint chunkIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertQueryNotActive(WorldTypeName);
                #endif
                
                if (DestroyAllEntitiesInChunk(chunkIdx)) {
                    ref var chunk = ref chunks[chunkIdx];
                    ref var cluster = ref clusters[chunk.clusterId];
                    RemoveChunkFromCluster(ref cluster, chunkIdx);
                    chunk.clusterId = -1;
                    chunk.selfOwner = independentWorld;
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal bool DestroyAllEntitiesInChunk(uint chunkIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertQueryNotActive(WorldTypeName);
                #endif
                ref var chunk = ref chunks[chunkIdx];

                if (chunk.clusterId >= 0) {
                    ref var cluster = ref clusters[chunk.clusterId];
                    
                    if (chunk.notEmptyBlocks != 0) {
                        ModuleComponents.Value.ClearChunk(chunkIdx);
                        ModuleTags.Value.ClearChunk(chunkIdx);
    
                        chunk.fullBlocks = 0;
                        Array.Clear(chunk.entities, 0, Const.BLOCK_IN_CHUNK);
                        Array.Clear(chunk.disabledEntities, 0, Const.BLOCK_IN_CHUNK);
                        Array.Clear(chunk.loadedEntities, 0, Const.BLOCK_IN_CHUNK);
                        if (chunk.loadedEntitiesCount > 0) {
                            RemoveLoadedChunkFromCluster(ref cluster, chunkIdx);
                            ModuleComponents.Value.bitMask.FreeChunk(chunkIdx);
                            ModuleTags.Value.bitMask.FreeChunk(chunkIdx);
                        }
                        chunk.loadedEntitiesCount = 0;
                    }

                    chunk.notEmptyBlocks = 0;
                    
                    if (cluster.freeChunkIndex == -1 && chunk.selfOwner) {
                        cluster.freeChunkIndex = (int) chunkIdx;
                    }

                    return true;
                }

                return false;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool UnloadChunk(uint chunkIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertQueryNotActive(WorldTypeName);
                #endif
                
                ref var chunk = ref chunks[chunkIdx];

                if (chunk.clusterId >= 0) {
                    ref var cluster = ref clusters[chunk.clusterId];
                    
                    if (chunk.notEmptyBlocks != 0) {
                        ModuleComponents.Value.ClearChunk(chunkIdx);
                        ModuleTags.Value.ClearChunk(chunkIdx);
                        Array.Clear(chunk.loadedEntities, 0, Const.BLOCK_IN_CHUNK);
                        if (chunk.loadedEntitiesCount > 0) {
                            RemoveLoadedChunkFromCluster(ref cluster, chunkIdx);
                            ModuleComponents.Value.bitMask.FreeChunk(chunkIdx);
                            ModuleTags.Value.bitMask.FreeChunk(chunkIdx);
                        }
                        chunk.loadedEntitiesCount = 0;
                    }
                    
                    if (cluster.freeChunkIndex == chunkIdx) {
                        TryFindNextFreeChunkInCluster(ref cluster);
                    }

                    return true;
                }

                return false;
            }

            [MethodImpl(AggressiveInlining)]
            public ushort GetChunkClusterId(uint chunkIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertChunkIsRegistered(WorldTypeName, chunkIdx);
                #endif

                return (ushort) chunks[chunkIdx].clusterId;
            }

            [MethodImpl(AggressiveInlining)]
            internal void ChangeChunkCluster(uint chunkIdx, ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertChunkIsRegistered(WorldTypeName, chunkIdx);
                #endif
                
                ref var chunk = ref chunks[chunkIdx];
                
                if (chunk.loadedEntitiesCount > 0) {
                    RemoveLoadedChunkFromCluster(ref clusters[chunk.clusterId], chunkIdx);
                }
                RemoveChunkFromCluster(ref clusters[chunk.clusterId], chunkIdx);
                
                chunk.clusterId = clusterId;
                
                ref var cluster = ref clusters[clusterId];
                if (cluster.chunksCount == cluster.chunks.Length) {
                    Array.Resize(ref cluster.chunks, (int) (cluster.chunksCount << 1));
                    Array.Resize(ref cluster.loadedChunks, (int) (cluster.chunksCount << 1));
                }
                cluster.chunks[cluster.chunksCount++] = chunkIdx;
                if (chunk.loadedEntitiesCount > 0) {
                    cluster.loadedChunks[cluster.loadedChunksCount++] = chunkIdx;
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void LoadCluster(ushort clusterId) {
                ref var cluster = ref clusters[clusterId];

                var chunksCount = cluster.chunksCount;
                for (var i = (int) chunksCount - 1; i >= 0; i--) {
                    LoadChunk(cluster.chunks[i]);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void LoadChunk(uint chunkIdx) {
                ref var chunk = ref chunks[chunkIdx];
                ref var cluster = ref clusters[chunk.clusterId];
                
                #if FFS_ECS_DEBUG
                Assert(WorldTypeName, chunk.loadedEntitiesCount == 0, $"Incorrect chunk index {chunkIdx}, chunk already has loaded entities");
                #endif
                
                var notEmptyBlocks = chunk.notEmptyBlocks;
                while (notEmptyBlocks > 0) {
                    var blockIdx = Utils.PopLsb(ref notEmptyBlocks);
                    var mask = chunk.entities[blockIdx];
                    chunk.loadedEntities[blockIdx] = mask;
                    chunk.loadedEntitiesCount += mask.PopCnt();
                }
                
                cluster.loadedChunks[cluster.loadedChunksCount++] = chunkIdx;
            }

            [MethodImpl(AggressiveInlining)]
            internal void ChangeChunkOwner(uint chunkIdx, ChunkOwnerType owner) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertChunkIsRegistered(WorldTypeName, chunkIdx);
                #endif
                ref var chunk = ref chunks[chunkIdx];

                var selfOwner = owner == ChunkOwnerType.Self;

                if (chunk.selfOwner != selfOwner) {
                    ref var cluster = ref clusters[chunk.clusterId];
                    if (selfOwner) {
                        if (cluster.freeChunkIndex == -1) {
                            cluster.freeChunkIndex = (int) chunkIdx;
                        }
                    } else {
                        if (cluster.freeChunkIndex == chunkIdx) {
                            TryFindNextFreeChunkInCluster(ref cluster);
                        }
                    }
                    chunk.selfOwner = selfOwner;
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal ChunkOwnerType GetChunkOwnerType(uint chunkIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertChunkIsRegistered(WorldTypeName, chunkIdx);
                #endif
                return chunks[chunkIdx].selfOwner ? ChunkOwnerType.Self : ChunkOwnerType.Other;
            }

            [MethodImpl(AggressiveInlining)]
            internal EntitiesChunkInfo FindNextSelfFreeChunk() {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                #endif
                
                if (!TryFindNextSelfFreeChunk(out var chunkInfo)) {
                    throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: FindNextSelfFreeChunk, ran out of space in the self owned chunks");
                }

                return chunkInfo;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryFindNextSelfFreeChunk(out EntitiesChunkInfo chunkInfo) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                #endif
                
                if (selfFreeChunksCount == 0) {
                    if (independentWorld) {
                        Resize((uint) (chunks.Length + 4));
                    } else {
                        chunkInfo = default;
                        return false;
                    }
                }

                chunkInfo = new EntitiesChunkInfo(selfFreeChunks[selfFreeChunksCount - 1]);
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            private void RemoveChunkFromCluster(ref EntitiesCluster cluster, uint chunkIdx) {
                for (var i = (int) cluster.chunksCount - 1; i >= 0; i--) {
                    if (cluster.chunks[i] == chunkIdx) {
                        cluster.chunks[i] = cluster.chunks[--cluster.chunksCount];
                        if (cluster.freeChunkIndex == chunkIdx) {
                            TryFindNextFreeChunkInCluster(ref cluster);
                        }

                        return;
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            private void RemoveLoadedChunkFromCluster(ref EntitiesCluster cluster, uint chunkIdx) {
                for (var i = (int) cluster.loadedChunksCount - 1; i >= 0; i--) {
                    if (cluster.loadedChunks[i] == chunkIdx) {
                        cluster.loadedChunks[i] = cluster.loadedChunks[--cluster.loadedChunksCount];
                        break;
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            private void TryFindNextFreeChunkInCluster(ref EntitiesCluster cluster) {
                cluster.freeChunkIndex = -1;
                var count = (int) cluster.chunksCount - 1;
                for (var i = count; i >= 0; i--) {
                    var index = cluster.chunks[i];
                    ref var chunk = ref chunks[index];
                    if (chunk.selfOwner && chunk.fullBlocks != ulong.MaxValue) {
                        cluster.freeChunkIndex = (int) index;
                        return;
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            private void RemoveFromSelfFreeChunks(uint chunkIdx) {
                var freeChunksCountTemp = selfFreeChunksCount;
                #if FFS_ECS_DEBUG
                var found = false;
                #endif
                while (freeChunksCountTemp > 0) {
                    var i = --freeChunksCountTemp;
                    var index = selfFreeChunks[i];
                    if (index == chunkIdx) {
                        selfFreeChunks[i] = selfFreeChunks[--selfFreeChunksCount];
                        #if FFS_ECS_DEBUG
                        found = true;
                        #endif
                        break;
                    }
                }
                
                #if FFS_ECS_DEBUG
                Assert(WorldTypeName, found, $"Incorrect chunk index {chunkIdx}, chunk not free");
                #endif
            }
            #endregion

            #region SERIALIZATION
            
            [MethodImpl(AggressiveInlining)]
            internal void FillClusterChunks(ChunkWritingStrategy strategy, ushort clusterId, ref TempChunksData tempChunks) {
                ref var cluster = ref clusters[clusterId];
                for (uint j = 0; j < cluster.chunksCount; j++) {
                    var chunkIdx = cluster.chunks[j];
                    ref var chunk = ref chunks[chunkIdx];
                        
                    if (strategy == ChunkWritingStrategy.All || (strategy == ChunkWritingStrategy.SelfOwner && chunk.selfOwner) || (strategy == ChunkWritingStrategy.OtherOwner && !chunk.selfOwner)) {
                        tempChunks.Add(chunkIdx);
                    }
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Write(ref BinaryPackWriter writer, ChunkWritingStrategy strategy, ReadOnlySpan<ushort> clustersToWrite, ref TempChunksData tempChunks, bool fullWorld) {
                writer.WriteInt(chunks.Length);
                writer.WriteInt(clusters.Length);
                writer.WriteInt(clustersToWrite.Length);
                
                for (var i = 0; i < clustersToWrite.Length; i++) {
                    var clusterId = clustersToWrite[i];
                    ref var cluster = ref clusters[clusterId];
                
                    writer.WriteUshort(clusterId);
                    writer.WriteBool(cluster.disabled);
                    writer.WriteInt(cluster.chunks.Length);

                    uint count = 0;
                    var offset = writer.MakePoint(sizeof(uint));
                    for (uint j = 0; j < cluster.chunksCount; j++) {
                        var chunkIdx = cluster.chunks[j];
                        ref var chunk = ref chunks[chunkIdx];
                        
                        if (strategy == ChunkWritingStrategy.All || (strategy == ChunkWritingStrategy.SelfOwner && chunk.selfOwner) || (strategy == ChunkWritingStrategy.OtherOwner && !chunk.selfOwner)) {
                            writer.WriteUint(chunkIdx);
                            WriteChunk(ref writer, ref chunk, fullWorld);
                            tempChunks.Add(chunkIdx);
                            count++;
                        }
                    }
                    writer.WriteUintAt(offset, count);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void Read(ref BinaryPackReader reader, bool fullWorld) {
                var chunksCapacity = reader.ReadInt();
                var clustersCapacity = reader.ReadInt();
                var clustersCount = reader.ReadInt();

                if (IsWorldInitialized()) {
                    Clear();
                } else {
                    Initialize((uint) chunksCapacity);
                }

                if (chunksCapacity > chunks.Length) {
                    Resize((uint) chunksCapacity);
                }
                
                if (clustersCapacity > clusters.Length) {
                    ResizeClusters(clustersCapacity);
                }
                
                for (var i = 0; i < clustersCount; i++) {
                    var clusterId = reader.ReadUshort();
                    var disabled = reader.ReadBool();
                    var chunksClusterCapacity = reader.ReadInt();
                    
                    RegisterCluster(clusterId);
                    
                    ref var cluster = ref clusters[clusterId];
                    if (chunksClusterCapacity > cluster.chunks.Length) {
                        Array.Resize(ref cluster.chunks, chunksClusterCapacity);
                        Array.Resize(ref cluster.loadedChunks, chunksClusterCapacity);
                    }
                    
                    cluster.disabled = disabled;
                    
                    var count = reader.ReadUint();
                    for (var j = 0; j < count; j++) {
                        var chunkIdx = reader.ReadUint();
                        ref var chunk = ref chunks[chunkIdx];
                        ReadChunk(ref reader, ref chunk, fullWorld);
                        
                        cluster.chunks[cluster.chunksCount++] = chunkIdx;
                        if (chunk.loadedEntitiesCount > 0) {
                            cluster.loadedChunks[cluster.loadedChunksCount++] = chunkIdx;
                        }
                    }
                    
                    TryFindNextFreeChunkInCluster(ref cluster);
                }

                selfFreeChunksCount = 0;
                for (uint chunkIdx = 0; chunkIdx < chunks.Length; chunkIdx++) {
                    if (chunks[chunkIdx].clusterId == -1) {
                        selfFreeChunks[selfFreeChunksCount++] = chunkIdx;
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void WriteChunk(ref BinaryPackWriter writer, ref EntitiesChunk chunk, bool fullWorld) {
                writer.WriteBool(chunk.selfOwner);
                writer.WriteUlong(chunk.notEmptyBlocks);
                writer.WriteUlong(chunk.fullBlocks);
                writer.WriteInt(chunk.clusterId);

                if (chunk.notEmptyBlocks != 0) {
                    writer.WriteArrayUnmanaged(chunk.entities, 0, Const.BLOCK_IN_CHUNK);
                    writer.WriteArrayUnmanaged(chunk.disabledEntities, 0, Const.BLOCK_IN_CHUNK);
                    if (fullWorld) {
                        writer.WriteInt(chunk.loadedEntitiesCount);
                        writer.WriteArrayUnmanaged(chunk.loadedEntities, 0, Const.BLOCK_IN_CHUNK);
                    }
                }

                writer.WriteArrayUnmanaged(chunk.versions, 0, Const.ENTITIES_IN_CHUNK);
            }

            [MethodImpl(AggressiveInlining)]
            internal void ReadChunk(ref BinaryPackReader reader, ref EntitiesChunk chunk, bool fullWorld) {
                chunk.selfOwner = reader.ReadBool();
                chunk.notEmptyBlocks = reader.ReadUlong();
                chunk.fullBlocks = reader.ReadUlong();
                chunk.clusterId = reader.ReadInt();
                chunk.loadedEntitiesCount = 0;

                if (chunk.notEmptyBlocks != 0) {
                    reader.ReadArrayUnmanaged(ref chunk.entities);
                    reader.ReadArrayUnmanaged(ref chunk.disabledEntities);
                    if (fullWorld) {
                        chunk.loadedEntitiesCount = reader.ReadInt();
                        reader.ReadArrayUnmanaged(ref chunk.loadedEntities);
                    }
                }

                reader.ReadArrayUnmanaged(ref chunk.versions);
            }
            #endregion

            #region QUERY
            [MethodImpl(AggressiveInlining)]
            internal void IncQDestroy(QueryData qData) {
                queriesToUpdateOnDestroy[queriesToUpdateOnDestroyCount++] = qData;
            }

            [MethodImpl(AggressiveInlining)]
            internal void DecQDestroy() {
                queriesToUpdateOnDestroy[--queriesToUpdateOnDestroyCount] = default;
            }

            [MethodImpl(AggressiveInlining)]
            internal void IncQDisable(QueryData qData) {
                queriesToUpdateOnDisable[queriesToUpdateOnDisableCount++] = qData;
            }

            internal void DecQDisable() {
                queriesToUpdateOnDisable[--queriesToUpdateOnDisableCount] = default;
            }

            [MethodImpl(AggressiveInlining)]
            internal void IncQEnable(QueryData qData) {
                queriesToUpdateOnEnable[queriesToUpdateOnEnableCount++] = qData;
            }

            [MethodImpl(AggressiveInlining)]
            internal void DecQEnable() {
                queriesToUpdateOnEnable[--queriesToUpdateOnEnableCount] = default;
            }

            #if FFS_ECS_DEBUG
            [MethodImpl(AggressiveInlining)]
            internal void BlockDestroy(int val) {
                _blockerDisable += val;
            }

            [MethodImpl(AggressiveInlining)]
            internal void BlockDisable(int val) {
                _blockerDisable += val;
            }

            [MethodImpl(AggressiveInlining)]
            internal void BlockEnable(int val) {
                _blockerEnable += val;
            }
            #endif
            #endregion

            [MethodImpl(AggressiveInlining)]
            private void FillEntitiesVersions(ref ushort[] dst) {
                dst ??= new ushort[Const.ENTITIES_IN_CHUNK];

                unsafe {
                    const int size = Const.ENTITIES_IN_CHUNK * sizeof(ushort);
                    fixed (void* dstPtr = &dst[0]) {
                        fixed (void* srcPtr = &versionEntitiesTemplate[0]) {
                            Buffer.MemoryCopy(srcPtr, dstPtr, size, size);
                        }
                    }
                }
            }
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        internal struct TempChunksData {
            internal uint[] Chunks;
            internal uint ChunksCount;

            [MethodImpl(AggressiveInlining)]
            internal void Add(uint chunkIdx) {
                Chunks[ChunksCount++] = chunkIdx;
            }

            [MethodImpl(AggressiveInlining)]
            internal static TempChunksData Create() {
                return new TempChunksData {
                    Chunks = ArrayPool<uint>.Shared.Rent(Entities.Value.chunks.Length),
                    ChunksCount = 0
                };
            }

            [MethodImpl(AggressiveInlining)]
            internal void Dispose() {
                ArrayPool<uint>.Shared.Return(Chunks);
            }
        }
    }

    public enum ChunkWritingStrategy : byte {
        All,
        SelfOwner,
        OtherOwner
    }
}