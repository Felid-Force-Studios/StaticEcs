#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    #region TYPES
    /// <summary>
    /// Indicates whether a chunk is owned by the local world instance (<see cref="Self"/>)
    /// or received from an external source such as a network peer (<see cref="Other"/>).
    /// </summary>
    public enum ChunkOwnerType : byte {
        /// <summary>Chunk is owned and managed by this world instance.</summary>
        Self,
        /// <summary>Chunk originates from an external source (e.g. network, another world).</summary>
        Other
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    /// <summary>
    /// Lightweight metadata wrapper for a chunk, providing the chunk index and derived
    /// entity range information. Returned by <c>TryFindNextSelfFreeChunk</c> /
    /// <c>FindNextSelfFreeChunk</c>.
    /// </summary>
    public readonly struct EntitiesChunkInfo {
        /// <summary>Zero-based chunk index within the world.</summary>
        public readonly uint ChunkIdx;

        /// <summary>Creates chunk info for the given chunk index.</summary>
        /// <param name="chunk">Zero-based chunk index.</param>
        [MethodImpl(AggressiveInlining)]
        public EntitiesChunkInfo(uint chunk) {
            ChunkIdx = chunk;
        }

        /// <summary>
        /// The first entity slot index belonging to this chunk.
        /// Equal to <c>ChunkIdx * <see cref="Const.ENTITIES_IN_CHUNK"/></c>.
        /// </summary>
        public uint EntitiesFrom {
            [MethodImpl(AggressiveInlining)] get => ChunkIdx << Const.ENTITIES_IN_CHUNK_SHIFT;
        }

        /// <summary>
        /// The total number of entity slots in a single chunk (always <see cref="Const.ENTITIES_IN_CHUNK"/>).
        /// </summary>
        public ushort EntitiesCapacity {
            [MethodImpl(AggressiveInlining)] get => Const.ENTITIES_IN_CHUNK;
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    internal struct EntitiesSegment {
        internal const ushort InvalidCluster = ushort.MaxValue;
        internal const int Invalid = -1;
        internal const ushort NoEntityType = ushort.MaxValue;
        
        internal ulong[] Masks;  // 4 ulong active entities + 4 ulong disabled entities + 4 loaded entities
        internal ushort[] Versions;
        internal ushort ClusterId;
        internal ushort EntityType; // 0-255 types and ushort.MaxValue not reserved
        internal bool SelfOwner;
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    internal struct EntitiesCluster {
        internal uint[] Chunks;
        internal uint[] LoadedChunks;
        internal int[] FreeSegmentByType;
        internal SpinLock Lock;
        internal uint ChunksCount;
        internal uint LoadedChunksCount;
        internal bool Registered;
        internal bool Disabled;
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public struct HeuristicChunk {
        internal AtomicMask NotEmptyBlocks;
        internal AtomicMask FullBlocks;
    }
    #endregion

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public abstract partial class World<TWorld> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        // ReSharper disable once UnusedTypeParameter
        internal static class EntityTypeInfo<T> where T : struct, IEntityType {
            internal static byte Id;
            internal static bool HasOnCreate;
            internal static bool Registered;
        }
        
        #region TYPE REGISTRATION
        [MethodImpl(AggressiveInlining)]
        internal static void RegisterComponentType<T>(ComponentTypeConfig<T> config = default)
            where T : struct, IComponent {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            AssertNotRegisteredComponent<T>(WorldTypeName);
            #else
            if (IsComponentTypeRegistered<T>()) return;
            #endif
            Data.Instance.RegisterComponentTypeInternal(config);
        }

        /// <summary>
        /// Registers a multi-component type with an optional element serialization strategy.
        /// Sets <see cref="Multi{TValue}.ElementStrategy"/> before component registration.
        /// </summary>
        /// <typeparam name="T">Multi-component element type implementing <see cref="IMultiComponent"/>.</typeparam>
        /// <param name="config">Component configuration for <see cref="Multi{T}"/>.</param>
        /// <param name="elementStrategy">Serialization strategy for elements. Null uses default <c>StructPackArrayStrategy</c>.</param>
        [MethodImpl(AggressiveInlining)]
        internal static void RegisterMultiComponentType<T>(ComponentTypeConfig<Multi<T>> config, IPackArrayStrategy<T> elementStrategy)
            where T : struct, IMultiComponent {
            Multi<T>.ElementStrategy = elementStrategy;
            RegisterComponentType(config);
        }

        [MethodImpl(AggressiveInlining)]
        internal static void RegisterTagType<T>(TagTypeConfig<T> config) where T : struct, ITag {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            AssertNotRegisteredComponent<T>(WorldTypeName);
            #else
            if (IsTagTypeRegistered<T>()) return;
            #endif
            Data.Instance.RegisterTagTypeInternal(config);
        }

        [MethodImpl(AggressiveInlining)]
        internal static void RegisterEventType<T>(EventTypeConfig<T> config = default) where T : struct, IEvent {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreatedOrInitialized(WorldTypeName);
            AssertNotRegisteredEvent<T>(WorldTypeName);
            #else
            if (IsEventTypeRegistered<T>()) return;
            #endif
            Data.Instance.RegisterEventTypeInternal(config);
        }

        [MethodImpl(AggressiveInlining)]
        internal static void RegisterEntityType<
            #if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
            #endif
            T>(byte id) where T : struct, IEntityType {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            Assert(WorldTypeName, !IsEntityTypeRegistered(id), $"EntityType with id {id} already registered");
            #else
            if (IsEntityTypeRegistered(id)) return;
            #endif
            Data.Instance.RegisterEntityTypeInternal<T>(id);
        }

        [MethodImpl(AggressiveInlining)]
        internal static bool IsComponentTypeRegistered<T>() where T : struct, IComponent => Components<T>.Instance.IsRegistered;

        [MethodImpl(AggressiveInlining)]
        internal static bool IsTagTypeRegistered<T>() where T : struct, ITag => Components<T>.Instance.IsRegistered;

        [MethodImpl(AggressiveInlining)]
        internal static bool IsEventTypeRegistered<T>() where T : struct, IEvent => Events<T>.Instance.Initialized;

        [MethodImpl(AggressiveInlining)]
        internal static bool IsEntityTypeRegistered(byte id) => Data.Instance.EntityTypes[id].Registered;

        [MethodImpl(AggressiveInlining)]
        internal static bool IsEntityTypeRegistered<T>() where T : struct, IEntityType => EntityTypeInfo<T>.Registered;
        #endregion

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        internal struct Data {
            public static Data Instance;
            public static WorldHandle Handle;

            #region FIELDS
            // TEMPLATES
            private readonly ushort[] _versionEntitiesTemplate;
            internal readonly ulong[] SegmentsMaskCache;
            
            #if FFS_ECS_BURST
            internal WorldLifecycleHandle LifecycleHandle;
            #endif

            // CHUNKS
            internal HeuristicChunk[] HeuristicChunks;
            internal AtomicMask[] HeuristicLoadedChunks;
            internal EntitiesSegment[] EntitiesSegments;
            private uint[] _selfFreeChunks;
            private int _selfFreeChunksCount;

            // CLUSTERS
            internal EntitiesCluster[] Clusters;
            internal ushort[] ActiveClusters;
            internal ushort ActiveClustersCount;

            // ENTITY TYPES
            internal EntityTypeData[] EntityTypes;
            internal Action[] EntityTypeResetters;
            internal int EntityTypeResettersCount;
            
            // COMPONENTS
            private readonly Dictionary<Guid, ComponentsHandle> _componentPoolByGuid;
            private readonly Dictionary<Guid, EcsComponentDeleteMigrationReader<TWorld>> _migratorByGuid;
            internal ulong[][] BitMaskComponents;
            private ComponentsHandle[] _poolsComponents;
            private Guid[] _guidsComponents;
            private ushort _poolsCountComponents;
            private ushort[] _trackingComponentIndices;
            private ushort _trackingComponentIndicesCount;
            internal ushort BitMaskComponentsLen;
            
            // EVENTS
            #if FFS_ECS_DEBUG
            internal IEventsDebugEventListener EventListener;
            #endif
            private readonly Dictionary<Guid, EventsHandle> _poolEventsByGuid;
            private readonly Dictionary<Guid, EcsEventDeleteMigrationReader> _deleteEventMigratorByGuid;
            private EventsHandle[] _poolsEvents;
            private ushort _poolsCountEvents;
            
            // MULTI COMPONENTS
            private Action<uint>[] _multiStorageResizers;
            private uint _multiStorageResizersCount;
            private Action[] _multiStorageResetters;
            private uint _multiStorageResettersCount;

            // QUERY
            private readonly QueryData[] _queriesToUpdateOnDisable;
            private readonly QueryData[] _queriesToUpdateOnEnable;
            private readonly QueryData[] _queriesToUpdateOnDestroy;
            private byte _queriesToUpdateOnDestroyCount;
            private byte _queriesToUpdateOnDisableCount;
            private byte _queriesToUpdateOnEnableCount;

            internal uint QueryDataCount;
            #if FFS_ECS_DEBUG
            private int _blockerDestroy;
            private int _blockerDisable;
            private int _blockerEnable;
            internal byte QueryMode; // 0 - None, 1 - Strict, 2 - Flexible
            private (uint, uint)[] _currentEntitiesRangeMainThread;
            [ThreadStatic] private static (uint, uint)[] _currentEntitiesRangeOtherThread;
            #endif

            // CONFIG
            internal readonly bool IndependentWorld;
            internal readonly bool ZeroThreadCount;
            internal readonly bool TrackCreated;
            internal readonly byte TrackingBufferSize;

            // CREATED TRACKING
            internal ulong[] CreatedTrackingChunks;
            internal ulong[][] CreatedTrackingSegments;
            private ulong[][] _createdTrackingSegmentsPool;
            private int _createdTrackingSegmentsPoolCount;
            internal ulong[][] CreatedTrackingHistoryChunks;
            internal ulong[][][] CreatedTrackingHistorySegments;

            // STATE
            internal ulong CurrentTick;
            internal ulong CurrentLastTick;
            internal WorldStatus WorldStatus;
            internal volatile bool MultiThreadActive;
            #endregion

            #region BASE
            public Data(WorldConfig worldConfig) {
                worldConfig = worldConfig.MergeWith(WorldConfig.Default());
                IndependentWorld = worldConfig.Independent.Value;
                ZeroThreadCount = worldConfig.ThreadCount.Value == 0;
                TrackCreated = worldConfig.TrackCreated.Value;
                TrackingBufferSize = worldConfig.TrackingBufferSize.Value;
                CurrentTick = 1;
                CurrentLastTick = 0;
                CreatedTrackingChunks = null;
                CreatedTrackingSegments = null;
                _createdTrackingSegmentsPool = null;
                _createdTrackingSegmentsPoolCount = 0;
                CreatedTrackingHistoryChunks = null;
                CreatedTrackingHistorySegments = null;

                // TEMPLATES
                _versionEntitiesTemplate = new ushort[Const.ENTITIES_IN_SEGMENT];
                for (var i = 0; i < Const.ENTITIES_IN_SEGMENT; i++) {
                    _versionEntitiesTemplate[i] = 1;
                }

                // ENTITY TYPES
                EntityTypes = new EntityTypeData[256];
                
                // CLUSTERS
                Clusters = new EntitiesCluster[worldConfig.BaseClustersCapacity.Value];
                ActiveClusters = new ushort[worldConfig.BaseClustersCapacity.Value];
                ActiveClustersCount = 0;
                for (ushort i = 0; i < worldConfig.BaseClustersCapacity; i++) {
                    ref var cluster = ref Clusters[i];
                    cluster.Chunks = new uint[32];
                    cluster.LoadedChunks = new uint[32];
                    cluster.Lock = new SpinLock(false);
                    cluster.ChunksCount = 0;
                    cluster.LoadedChunksCount = 0;
                    cluster.FreeSegmentByType = new int[256];
                    Array.Fill(cluster.FreeSegmentByType, EntitiesSegment.Invalid);
                }

                // QUERY
                _queriesToUpdateOnDisable = new QueryData[Const.MAX_QUERY_DATA_PER_TYPE];
                _queriesToUpdateOnEnable = new QueryData[Const.MAX_QUERY_DATA_PER_TYPE];
                _queriesToUpdateOnDestroy = new QueryData[Const.MAX_QUERY_DATA_PER_TYPE];
                _queriesToUpdateOnDestroyCount = 0;
                _queriesToUpdateOnDisableCount = 0;
                _queriesToUpdateOnEnableCount = 0;
                #if FFS_ECS_DEBUG
                _blockerDestroy = 0;
                _blockerDisable = 0;
                _blockerEnable = 0;
                #endif
                
                MultiThreadActive = false;
                WorldStatus = WorldStatus.Created;
                
                if (!BinaryPack.IsRegistered<EntityGID>()) {
                    BinaryPack.RegisterWithCollections<EntityGID, UnmanagedPackArrayStrategy<EntityGID>>(EntityGIDSerializer.WriteEntityGID, EntityGIDSerializer.ReadEntityGID);
                }

                if (!BinaryPack.IsRegistered<EntityGIDCompact>()) {
                    BinaryPack.RegisterWithCollections<EntityGIDCompact, UnmanagedPackArrayStrategy<EntityGIDCompact>>(EntityGIDSerializer.WriteEntityGIDCompact, EntityGIDSerializer.ReadEntityGIDCompact);
                }
                
                // COMPONENTS
                _poolsComponents = new ComponentsHandle[worldConfig.BaseComponentTypesCount.Value];
                _guidsComponents = new Guid[worldConfig.BaseComponentTypesCount.Value];
                _trackingComponentIndices = new ushort[worldConfig.BaseComponentTypesCount.Value];
                _trackingComponentIndicesCount = default;
                _componentPoolByGuid = new Dictionary<Guid, ComponentsHandle>();
                _migratorByGuid = new Dictionary<Guid, EcsComponentDeleteMigrationReader<TWorld>>();
                BitMaskComponents = null;
                _poolsCountComponents = default;
                BitMaskComponentsLen = default;

                ParallelRunner<TWorld>.Create(worldConfig.ThreadCount.Value, worldConfig.WorkerSpinCount.Value);
                
                QueryDataCount = 0;
                #if FFS_ECS_DEBUG
                QueryMode = 0;
                #endif
                
                // EVENTS
                _poolsEvents = new EventsHandle[64];
                _poolEventsByGuid = new Dictionary<Guid, EventsHandle>();
                _deleteEventMigratorByGuid = new Dictionary<Guid, EcsEventDeleteMigrationReader>();
                _poolsCountEvents = default;

                // CHUNKS
                SegmentsMaskCache = Const.DataMasks;
                HeuristicChunks = null;
                HeuristicLoadedChunks = null;
                EntitiesSegments = null;
                _selfFreeChunks = null;
                _selfFreeChunksCount = default;
                
                #if FFS_ECS_DEBUG
                _currentEntitiesRangeMainThread = null;
                EventListener = null;
                #endif
                
                // MULTI STORAGES
                _multiStorageResizers = null;
                _multiStorageResizersCount = 0;
                _multiStorageResetters = null;
                _multiStorageResettersCount = 0;

                // ENTITY TYPE RESETTERS
                EntityTypeResetters = null;
                EntityTypeResettersCount = 0;

                #if FFS_ECS_BURST
                LifecycleHandle = default;
                #endif
                
                RegisterEntityTypeInternal<Default>(0);
            }

            [MethodImpl(NoInlining)]
            internal void InitializeInternal(uint chunksCapacity) {
                var segmentsCapacity = chunksCapacity * Const.SEGMENTS_IN_CHUNK;
                
                // SEGMENTS
                EntitiesSegments = new EntitiesSegment[segmentsCapacity];
                HeuristicChunks = new HeuristicChunk[chunksCapacity];
                HeuristicLoadedChunks = new AtomicMask[chunksCapacity];
                for (var i = (int) segmentsCapacity - 1; i >= 0; i--) {
                    ref var segment = ref EntitiesSegments[i];
                    FillEntitiesVersions(ref segment.Versions);
                    segment.Masks = new ulong[Const.BLOCKS_IN_SEGMENT * 3];
                    segment.ClusterId = EntitiesSegment.InvalidCluster;
                    segment.SelfOwner = IndependentWorld;
                    segment.EntityType = EntitiesSegment.NoEntityType;
                }

                _selfFreeChunksCount = 0;
                _selfFreeChunks = new uint[chunksCapacity];
                if (IndependentWorld) {
                    for (var i = (int) chunksCapacity - 1; i >= 0; i--) {
                        _selfFreeChunks[_selfFreeChunksCount++] = (uint) i;
                    }
                }
                
                // COMPONENTS
                BitMaskComponentsLen = (ushort)(_poolsCountComponents.Normalize(Const.U64_BITS) >> Const.U64_SHIFT);
                BitMaskComponents = new ulong[chunksCapacity][];
                for (uint i = 0; i < chunksCapacity; i++) {
                    BitMaskComponents[i] = new ulong[BitMaskComponentsLen * Const.SEGMENTS_IN_CHUNK];
                }

                // ENTITY TYPES
                for (var i = 0; i < 256; i++) {
                    if (EntityTypes[i].Registered) {
                        EntityTypes[i].HeuristicChunks = new AtomicMask[chunksCapacity];
                    }
                }

                #if FFS_ECS_BURST
                LifecycleHandle.OnInitialize(segmentsCapacity);
                #endif

                // CREATED TRACKING
                if (TrackCreated) {
                    var totalSlots = TrackingBufferSize + 1;
                    var createdPoolSize = TrackingBufferSize > 0 ? segmentsCapacity * totalSlots : segmentsCapacity;
                    _createdTrackingSegmentsPool = new ulong[createdPoolSize][];
                    _createdTrackingSegmentsPoolCount = 0;
                    if (TrackingBufferSize > 0) {
                        CreatedTrackingHistoryChunks = new ulong[totalSlots][];
                        CreatedTrackingHistorySegments = new ulong[totalSlots][][];
                        for (var i = 0; i < totalSlots; i++) {
                            CreatedTrackingHistoryChunks[i] = new ulong[chunksCapacity];
                            CreatedTrackingHistorySegments[i] = new ulong[segmentsCapacity][];
                        }
                        var currentSlot = (int)((CurrentTick + 1) % (ulong)totalSlots);
                        CreatedTrackingChunks = CreatedTrackingHistoryChunks[currentSlot];
                        CreatedTrackingSegments = CreatedTrackingHistorySegments[currentSlot];
                    }
                    else {
                        CreatedTrackingChunks = new ulong[chunksCapacity];
                        CreatedTrackingSegments = new ulong[segmentsCapacity][];
                    }
                }

                for (uint i = 0; i < _poolsCountComponents; i++) {
                    _poolsComponents[i].Initialize(chunksCapacity, BitMaskComponents, BitMaskComponentsLen);
                }

                WorldStatus = WorldStatus.Initialized;
            }

            [MethodImpl(NoInlining)]
            private void ResizeWorld(uint chunksCapacity) {
                var segmentsCapacity = chunksCapacity * Const.SEGMENTS_IN_CHUNK;
                var oldChunksCapacity = HeuristicChunks.Length;
                var oldSegmentsCapacity = EntitiesSegments.Length;

                Array.Resize(ref EntitiesSegments, (int) segmentsCapacity);
                Array.Resize(ref HeuristicChunks, (int) chunksCapacity);
                Array.Resize(ref HeuristicLoadedChunks, (int) chunksCapacity);
                Array.Resize(ref _selfFreeChunks, (int) chunksCapacity);
                
                for (var i = (int) segmentsCapacity - 1; i >= oldSegmentsCapacity; i--) {
                    ref var segment = ref EntitiesSegments[i];
                    FillEntitiesVersions(ref segment.Versions);
                    segment.Masks = new ulong[Const.BLOCKS_IN_SEGMENT * 3];
                    segment.ClusterId = EntitiesSegment.InvalidCluster;
                    segment.SelfOwner = IndependentWorld;
                    segment.EntityType = EntitiesSegment.NoEntityType;
                }
                
                if (IndependentWorld) {
                    for (var i = (int) chunksCapacity - 1; i >= oldChunksCapacity; i--) {
                        _selfFreeChunks[_selfFreeChunksCount++] = (uint) i;
                    }
                }

                // COMPONENTS
                Array.Resize(ref BitMaskComponents, (int)chunksCapacity);
                for (var i = oldChunksCapacity; i < chunksCapacity; i++) {
                    BitMaskComponents[i] = new ulong[BitMaskComponentsLen * Const.SEGMENTS_IN_CHUNK];
                }

                // ENTITY TYPES
                for (var i = 0; i < 256; i++) {
                    if (EntityTypes[i].Registered) {
                        Array.Resize(ref EntityTypes[i].HeuristicChunks, (int)chunksCapacity);
                    }
                }

                // CREATED TRACKING
                if (TrackCreated) {
                    var totalSlots = TrackingBufferSize + 1;
                    var createdPoolSize = TrackingBufferSize > 0 ? segmentsCapacity * totalSlots : segmentsCapacity;
                    Array.Resize(ref _createdTrackingSegmentsPool, (int)createdPoolSize);
                    if (TrackingBufferSize > 0) {
                        for (var i = 0; i < totalSlots; i++) {
                            Array.Resize(ref CreatedTrackingHistoryChunks[i], (int)chunksCapacity);
                            Array.Resize(ref CreatedTrackingHistorySegments[i], (int)segmentsCapacity);
                        }
                        var currentSlot = (int)((CurrentTick + 1) % (ulong)totalSlots);
                        CreatedTrackingChunks = CreatedTrackingHistoryChunks[currentSlot];
                        CreatedTrackingSegments = CreatedTrackingHistorySegments[currentSlot];
                    }
                    else {
                        Array.Resize(ref CreatedTrackingChunks, (int)chunksCapacity);
                        Array.Resize(ref CreatedTrackingSegments, (int)segmentsCapacity);
                    }
                }

                // MULTI STORAGES
                for (uint i = 0; i < _multiStorageResizersCount; i++) {
                    _multiStorageResizers[i](segmentsCapacity);
                }

                #if FFS_ECS_BURST
                LifecycleHandle.OnResize(segmentsCapacity, oldSegmentsCapacity);
                #endif

                for (uint i = 0, iMax = _poolsCountComponents; i < iMax; i++) {
                    _poolsComponents[i].Resize(chunksCapacity, BitMaskComponents);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly void DestroyInternal() {
                for (var i = 0; i < _poolsCountComponents; i++) {
                    _poolsComponents[i].Destroy();
                }
                for (var i = 0; i < _poolsCountEvents; i++) {
                    _poolsEvents[i].Destroy();
                }

                for (var i = 0; i < EntityTypeResettersCount; i++) {
                    EntityTypeResetters[i]();
                }

                ParallelRunner<TWorld>.Destroy();

                #if FFS_ECS_BURST
                LifecycleHandle.OnDestroy();
                #endif

                Instance = default;
            }

            [MethodImpl(NoInlining)]
            internal void HardResetInternal() {
                for (var i = 0; i < _poolsCountComponents; i++) {
                    _poolsComponents[i].HardReset();
                }

                if (_multiStorageResetters != null) {
                    for (uint i = 0; i < _multiStorageResettersCount; i++) {
                        _multiStorageResetters[i]();
                    }
                }

                for (var i = 0; i < BitMaskComponents.Length; i++) {
                    Array.Clear(BitMaskComponents[i], 0, BitMaskComponents[i].Length);
                }

                for (var i = 0; i < EntitiesSegments.Length; i++) {
                    ref var segment = ref EntitiesSegments[i];
                    Array.Clear(segment.Masks, 0, Const.BLOCKS_IN_SEGMENT * 3);
                    var versions = segment.Versions;
                    if (versions != null) {
                        for (var v = 0; v < versions.Length; v++) {
                            var ver = (ushort)(versions[v] + 1);
                            versions[v] = ver == 0 ? (ushort)1 : ver;
                        }
                    }
                    segment.ClusterId = EntitiesSegment.InvalidCluster;
                    segment.EntityType = EntitiesSegment.NoEntityType;

                    if (TrackCreated) {
                        if (TrackingBufferSize > 0) {
                            var totalSlots = TrackingBufferSize + 1;
                            for (var slot = 0; slot < totalSlots; slot++) {
                                ref var createdSeg = ref CreatedTrackingHistorySegments[slot][i];
                                if (createdSeg != null) {
                                    _createdTrackingSegmentsPool[_createdTrackingSegmentsPoolCount++] = createdSeg;
                                    createdSeg = null;
                                }
                            }
                        }
                        else {
                            ref var createdSeg = ref CreatedTrackingSegments[i];
                            if (createdSeg != null) {
                                _createdTrackingSegmentsPool[_createdTrackingSegmentsPoolCount++] = createdSeg;
                                createdSeg = null;
                            }
                        }
                    }
                }

                Array.Clear(HeuristicChunks, 0, HeuristicChunks.Length);
                Array.Clear(HeuristicLoadedChunks, 0, HeuristicLoadedChunks.Length);

                if (TrackCreated) {
                    if (TrackingBufferSize > 0) {
                        var totalSlots = TrackingBufferSize + 1;
                        for (var slot = 0; slot < totalSlots; slot++) {
                            Array.Clear(CreatedTrackingHistoryChunks[slot], 0, CreatedTrackingHistoryChunks[slot].Length);
                        }
                        var writeSlot = (int)((CurrentTick + 1) % (ulong)totalSlots);
                        CreatedTrackingChunks = CreatedTrackingHistoryChunks[writeSlot];
                        CreatedTrackingSegments = CreatedTrackingHistorySegments[writeSlot];
                    }
                    else {
                        Array.Clear(CreatedTrackingChunks, 0, CreatedTrackingChunks.Length);
                    }
                }

                for (var i = 0; i < 256; i++) {
                    if (EntityTypes[i].Registered && EntityTypes[i].HeuristicChunks != null) {
                        Array.Clear(EntityTypes[i].HeuristicChunks, 0, EntityTypes[i].HeuristicChunks.Length);
                    }
                }

                for (var i = ActiveClustersCount - 1; i >= 0; i--) {
                    ref var cluster = ref Clusters[ActiveClusters[i]];
                    Array.Fill(cluster.FreeSegmentByType, EntitiesSegment.Invalid);
                    cluster.ChunksCount = 0;
                    cluster.LoadedChunksCount = 0;
                    cluster.Registered = false;
                    cluster.Disabled = false;
                    #if FFS_ECS_BURST
                    LifecycleHandle.OnClusterValuesChanged(ActiveClusters[i]);
                    #endif
                }
                ActiveClustersCount = 0;

                _selfFreeChunksCount = 0;
                if (IndependentWorld) {
                    for (var i = HeuristicChunks.Length - 1; i >= 0; i--) {
                        _selfFreeChunks[_selfFreeChunksCount++] = (uint)i;
                    }
                }
            }
            #endregion

            #region MULTI COMPONENTS
            [MethodImpl(AggressiveInlining)]
            internal void RegisterMultiStorageResizer(Action<uint> resizer) {
                _multiStorageResizers ??= new Action<uint>[16];
                if (_multiStorageResizersCount == _multiStorageResizers.Length) {
                    Array.Resize(ref _multiStorageResizers, (int)(_multiStorageResizersCount << 1));
                }
                _multiStorageResizers[_multiStorageResizersCount++] = resizer;
            }

            [MethodImpl(AggressiveInlining)]
            internal void RegisterMultiStorageResetter(Action resetter) {
                _multiStorageResetters ??= new Action[16];
                if (_multiStorageResettersCount == _multiStorageResetters.Length) {
                    Array.Resize(ref _multiStorageResetters, (int)(_multiStorageResettersCount << 1));
                }
                _multiStorageResetters[_multiStorageResettersCount++] = resetter;
            }
            #endregion

            #region TRACKING
            [MethodImpl(AggressiveInlining)]
            internal readonly void ClearAllComponentsTrackingInternal() {
                for (var i = 0; i < _poolsCountComponents; i++) {
                    _poolsComponents[i].ClearTracking();
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly void ClearAddedComponentTrackingInternal() {
                for (var i = 0; i < _poolsCountComponents; i++) {
                    _poolsComponents[i].ClearAddedTracking();
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly void ClearDeletedComponentTrackingInternal() {
                for (var i = 0; i < _poolsCountComponents; i++) {
                    _poolsComponents[i].ClearDeletedTracking();
                }
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            [MethodImpl(AggressiveInlining)]
            internal readonly void ClearChangedComponentTrackingInternal() {
                for (var i = 0; i < _poolsCountComponents; i++) {
                    _poolsComponents[i].ClearChangedTracking();
                }
            }
            #endif


            [MethodImpl(NoInlining)]
            internal void SetCreatedBit(uint segmentIdx, byte segmentBlockIdx, ulong blockEntityMask, uint chunkIdx, byte chunkBlockIdx) {
                ref var seg = ref CreatedTrackingSegments[segmentIdx];
                seg ??= AllocateCreatedTrackingSegment();
                seg[segmentBlockIdx] |= blockEntityMask;
                CreatedTrackingChunks[chunkIdx] |= 1UL << chunkBlockIdx;
            }

            [MethodImpl(NoInlining)]
            internal void SetCreatedBitBatch(ulong newEntitiesMask, uint segmentIdx, byte segmentBlockIdx, uint chunkIdx, byte chunkBlockIdx) {
                ref var seg = ref CreatedTrackingSegments[segmentIdx];
                seg ??= AllocateCreatedTrackingSegment();
                seg[segmentBlockIdx] |= newEntitiesMask;
                CreatedTrackingChunks[chunkIdx] |= 1UL << chunkBlockIdx;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly ulong CreatedMask(uint segmentIdx, int segmentBlockIdx) {
                var masks = CreatedTrackingSegments[segmentIdx];
                return masks != null ? masks[segmentBlockIdx] : 0UL;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly ulong CreatedHeuristicHistory(ulong fromTick, ulong toTick, uint chunkIdx) {
                #if FFS_ECS_DEBUG
                AssertTrackingBufferNotOverflow(WorldTypeName, fromTick, toTick, TrackingBufferSize);
                #endif
                var ticksToCheck = toTick - fromTick;
                var totalSlots = (ulong)TrackingBufferSize + 1;
                if (ticksToCheck == 0) return 0UL;
                if (ticksToCheck > TrackingBufferSize) ticksToCheck = TrackingBufferSize;
                if (ticksToCheck == 1) {
                    return CreatedTrackingHistoryChunks[(int)(toTick % totalSlots)][chunkIdx];
                }
                ulong result = 0;
                for (ulong i = 0; i < ticksToCheck; i++) {
                    result |= CreatedTrackingHistoryChunks[(int)((toTick - i) % totalSlots)][chunkIdx];
                }
                return result;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly ulong CreatedMaskHistory(ulong fromTick, ulong toTick, uint segmentIdx, int segmentBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertTrackingBufferNotOverflow(WorldTypeName, fromTick, toTick, TrackingBufferSize);
                #endif
                var ticksToCheck = toTick - fromTick;
                var totalSlots = (ulong)TrackingBufferSize + 1;
                if (ticksToCheck == 0) return 0UL;
                if (ticksToCheck > TrackingBufferSize) ticksToCheck = TrackingBufferSize;
                if (ticksToCheck == 1) {
                    var m = CreatedTrackingHistorySegments[(int)(toTick % totalSlots)][segmentIdx];
                    return m != null ? m[segmentBlockIdx] : 0UL;
                }
                ulong result = 0;
                for (ulong i = 0; i < ticksToCheck; i++) {
                    var m = CreatedTrackingHistorySegments[(int)((toTick - i) % totalSlots)][segmentIdx];
                    if (m != null) result |= m[segmentBlockIdx];
                }
                return result;
            }

            [MethodImpl(NoInlining)]
            internal void AdvanceTickInternal() {
                CurrentTick++;
                AdvanceCreatedTrackingSlot();
                for (var i = 0; i < _trackingComponentIndicesCount; i++) {
                    _poolsComponents[_trackingComponentIndices[i]].AdvanceTracking();
                }
            }

            [MethodImpl(NoInlining)]
            internal void AdvanceCreatedTrackingSlot() {
                if (!TrackCreated || TrackingBufferSize == 0) return;
                var newSlot = (int)((CurrentTick + 1) % (ulong)(TrackingBufferSize + 1));
                var chunks = CreatedTrackingHistoryChunks[newSlot];
                var segments = CreatedTrackingHistorySegments[newSlot];
                for (var chunkIdx = 0; chunkIdx < chunks.Length; chunkIdx++) {
                    var notEmpty = chunks[chunkIdx];
                    if (notEmpty == 0) continue;
                    chunks[chunkIdx] = 0;
                    var baseSegment = (uint)(chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT);
                    while (notEmpty != 0) {
                        #if NET6_0_OR_GREATER
                        var blockIdx = System.Numerics.BitOperations.TrailingZeroCount(notEmpty);
                        #else
                        var blockIdx = Utils.DeBruijn[(uint)(((notEmpty & (ulong)-(long)notEmpty) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        var segmentOffset = blockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                        var segmentIdx = (uint)(baseSegment + segmentOffset);
                        ref var segRef = ref segments[segmentIdx];
                        if (segRef != null) {
                            ReturnCreatedTrackingSegment(segRef);
                            segRef = null;
                        }
                        notEmpty &= ~(0xFUL << (segmentOffset << Const.BLOCKS_IN_SEGMENT_SHIFT));
                    }
                }
                CreatedTrackingChunks = chunks;
                CreatedTrackingSegments = segments;
            }

            [MethodImpl(AggressiveInlining)]
            internal void ClearCreatedTrackingInternal() {
                if (!TrackCreated) return;
                #if FFS_ECS_DEBUG
                AssertMultiThreadNotActive(WorldTypeName);
                #endif
                if (TrackingBufferSize > 0) {
                    var totalSlots = TrackingBufferSize + 1;
                    for (var slot = 0; slot < totalSlots; slot++) {
                        ClearCreatedTrackingSlot(CreatedTrackingHistoryChunks[slot], CreatedTrackingHistorySegments[slot]);
                    }
                }
                else {
                    ClearCreatedTrackingSlot(CreatedTrackingChunks, CreatedTrackingSegments);
                }
            }

            [MethodImpl(NoInlining)]
            private void ClearCreatedTrackingSlot(ulong[] chunks, ulong[][] segments) {
                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif
                for (var chunkIdx = 0; chunkIdx < chunks.Length; chunkIdx++) {
                    var notEmpty = chunks[chunkIdx];
                    if (notEmpty == 0) continue;
                    chunks[chunkIdx] = 0;
                    var baseSegment = (uint)(chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT);
                    while (notEmpty != 0) {
                        #if NET6_0_OR_GREATER
                        var blockIdx = System.Numerics.BitOperations.TrailingZeroCount(notEmpty);
                        #else
                        var blockIdx = deBruijn[(uint)(((notEmpty & (ulong)-(long)notEmpty) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        var segmentOffset = blockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                        var segmentIdx = (uint)(baseSegment + segmentOffset);
                        ref var segRef = ref segments[segmentIdx];
                        if (segRef != null) {
                            ReturnCreatedTrackingSegment(segRef);
                            segRef = null;
                        }
                        notEmpty &= ~(0xFUL << (segmentOffset << Const.BLOCKS_IN_SEGMENT_SHIFT));
                    }
                }
            }

            [MethodImpl(NoInlining)]
            private ulong[] AllocateCreatedTrackingSegment() {
                if (_createdTrackingSegmentsPoolCount > 0) {
                    var seg = _createdTrackingSegmentsPool[--_createdTrackingSegmentsPoolCount];
                    _createdTrackingSegmentsPool[_createdTrackingSegmentsPoolCount] = null;
                    Array.Clear(seg, 0, Const.BLOCKS_IN_SEGMENT);
                    return seg;
                }
                return new ulong[Const.BLOCKS_IN_SEGMENT];
            }

            [MethodImpl(AggressiveInlining)]
            private void ReturnCreatedTrackingSegment(ulong[] seg) {
                _createdTrackingSegmentsPool[_createdTrackingSegmentsPoolCount++] = seg;
            }
            #endregion

            #region COUNTS
            [MethodImpl(AggressiveInlining)]
            internal readonly uint CalculateEntitiesCountInternal() {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                var count = 0;

                for (var chunkIdx = 0; chunkIdx < HeuristicChunks.Length; chunkIdx++) {
                    ref var heuristic = ref HeuristicChunks[chunkIdx];
                    var fullBlocks = heuristic.FullBlocks.Value;
                    count += fullBlocks.PopCnt() * Const.ENTITIES_IN_BLOCK;

                    var partialMask = heuristic.NotEmptyBlocks.Value & ~fullBlocks;
                    while (partialMask != 0) {
                        var chunkBlockIdx = Utils.PopLsb(ref partialMask);
                        var segmentIdx = (uint) ((chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT) + (chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT));
                        var segmentBlockIdx = (byte) (chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);

                        count += EntitiesSegments[segmentIdx].Masks[segmentBlockIdx].PopCnt();
                    }
                }

                return (uint) count;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal readonly uint CalculateLoadedEntitiesCountInternal() {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                var count = 0;
                for (ushort i = 0; i < ActiveClustersCount; i++) {
                    ref var cluster = ref Clusters[ActiveClusters[i]];

                    var chunksCount = cluster.LoadedChunksCount;
                    for (var j = 0; j < chunksCount; j++) {
                        var loadedBlocks = HeuristicLoadedChunks[cluster.LoadedChunks[j]].Value;
                        while (loadedBlocks != 0) {
                            var chunkBlockIdx = Utils.PopLsb(ref loadedBlocks);
                            var segmentIdx = (cluster.LoadedChunks[j] << Const.SEGMENTS_IN_CHUNK_SHIFT) + (chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT);
                            var segmentBlockIdx = chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK;
                            count += EntitiesSegments[segmentIdx].Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT * 2].PopCnt();
                        }
                    }
                }

                return (uint) count;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly uint CalculateEntitiesCapacityInternal() {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                return (uint) (EntitiesSegments.Length << Const.ENTITIES_IN_SEGMENT_SHIFT);
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly uint CalculateEntitiesCountByTypeInternal(byte entityType) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                if (!EntityTypes[entityType].Registered) return 0;

                var count = 0;
                var typeHeuristic = EntityTypes[entityType].HeuristicChunks;

                for (var chunkIdx = 0; chunkIdx < typeHeuristic.Length; chunkIdx++) {
                    var typeMask = typeHeuristic[chunkIdx].Value;
                    if (typeMask == 0) continue;

                    var fullBlocks = HeuristicChunks[chunkIdx].FullBlocks.Value;
                    var fullOfType = typeMask & fullBlocks;
                    count += fullOfType.PopCnt() * Const.ENTITIES_IN_BLOCK;

                    var partialOfType = typeMask & ~fullBlocks;
                    while (partialOfType != 0) {
                        var chunkBlockIdx = Utils.PopLsb(ref partialOfType);
                        var segmentIdx = (uint) ((chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT) + (chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT));
                        var segmentBlockIdx = (byte) (chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);
                        count += EntitiesSegments[segmentIdx].Masks[segmentBlockIdx].PopCnt();
                    }
                }

                return (uint) count;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly uint CalculateEntitiesCapacityByTypeInternal(byte entityType) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                if (!EntityTypes[entityType].Registered) return 0;

                var segmentCount = 0;
                for (var i = 0; i < EntitiesSegments.Length; i++) {
                    if (EntitiesSegments[i].EntityType == entityType) {
                        segmentCount++;
                    }
                }

                return (uint) (segmentCount * Const.ENTITIES_IN_SEGMENT);
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly bool HasFreeEntities(ushort clusterId) {
                return CalculateFreeEntitiesCount(clusterId) > 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly uint CalculateFreeEntitiesCount(ushort clusterId) {
                uint count = 0;
                ref var cluster = ref Clusters[clusterId];
                for (var i = 0; i < cluster.ChunksCount; i++) {
                    var chunkIdx = cluster.Chunks[i];
                    var baseSegmentIdx = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;
                    if (EntitiesSegments[baseSegmentIdx].SelfOwner) {
                        var fullBlocksInv = ~HeuristicChunks[chunkIdx].FullBlocks.Value;
                        while (fullBlocksInv != 0) {
                            var chunkBlockIdx = Utils.PopLsb(ref fullBlocksInv);
                            var segmentIdx = baseSegmentIdx + (chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT);
                            var segmentBlockIdx = chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK;
                            var free = (~EntitiesSegments[segmentIdx].Masks[segmentBlockIdx]).PopCnt();
                            count += (uint) free;
                        }
                    }
                }

                return count;
            }
            #endregion

            #region ENTITY PROPERTIES
            [MethodImpl(AggressiveInlining)]
            internal readonly GIDStatus GIDStatus(EntityGID gid) {
                var entityId = gid.Id;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                if (segmentIdx >= (uint) EntitiesSegments.Length) {
                    return StaticEcs.GIDStatus.NotActual;
                }
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var segmentEntityIdx = entityId & Const.ENTITIES_IN_SEGMENT_MASK;
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);

                var segment = EntitiesSegments[segmentIdx];

                if (segment.ClusterId != gid.ClusterId
                    || gid.Version != segment.Versions[segmentEntityIdx]
                    || (segment.Masks[segmentBlockIdx] & blockEntityMask) == 0) {
                    return StaticEcs.GIDStatus.NotActual;
                }

                return (segment.Masks[segmentBlockIdx + (Const.BLOCKS_IN_SEGMENT << 1)] & blockEntityMask) == 0 
                    ? StaticEcs.GIDStatus.NotLoaded 
                    : StaticEcs.GIDStatus.Active;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal readonly bool EntityIsNotDestroyed(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                #endif
                
                if (entity.IdWithOffset > 0) {
                    var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                    var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                    var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                    var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);

                    return segmentIdx < EntitiesSegments.Length && (EntitiesSegments[segmentIdx].Masks[segmentBlockIdx] & blockEntityMask) != 0;
                }

                return false;
            }       
            
            [MethodImpl(AggressiveInlining)]
            internal readonly bool EntityIsLoaded(Entity entity) {
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);

                return (EntitiesSegments[segmentIdx].Masks[segmentBlockIdx + (Const.BLOCKS_IN_SEGMENT << 1)] & blockEntityMask) != 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly ushort EntityVersion(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif
                
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                return EntitiesSegments[entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT].Versions[entityId & Const.ENTITIES_IN_SEGMENT_MASK];
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly ushort EntityClusterId(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif

                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                return EntitiesSegments[entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT].ClusterId;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly byte EntityType(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif

                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                return (byte)EntitiesSegments[entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT].EntityType;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly bool EntityIsSelfOwned(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif
                
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                return EntitiesSegments[entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT].SelfOwner;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly EntityGID EntityGID(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif
                
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segment = EntitiesSegments[entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT];
                var version = segment.Versions[entityId & Const.ENTITIES_IN_SEGMENT_MASK];
                var clusterId = segment.ClusterId;
                return new EntityGID(entityId, version, clusterId);
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly EntityGIDCompact EntityGIDCompact(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif
                
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segment = EntitiesSegments[entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT];
                var version = segment.Versions[entityId & Const.ENTITIES_IN_SEGMENT_MASK];
                var clusterId = segment.ClusterId;
                return new EntityGIDCompact(entityId, version, clusterId);
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly bool IsEnabledEntity(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif
                
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);
                return (EntitiesSegments[segmentIdx].Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] & blockEntityMask) == 0;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal readonly string EntityToPrettyString(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif
                
                var builder = new StringBuilder(256);
                builder.Append("Entity ID: ");
                builder.Append(entity.ID);
                builder.Append(" Version: ");
                builder.Append(entity.Version);
                builder.Append(" Cluster ID: ");
                builder.Append(entity.ClusterId);
                builder.Append(" Type: ");
                builder.Append(entity.EntityType);
                builder.Append(" Owner: ");
                builder.Append(entity.IsSelfOwned ? "Self" : "Other");
                if (entity.IsDisabled) {
                    builder.Append(" [Disabled]");
                }
                builder.AppendLine();
                
                var maskLen = BitMaskComponentsLen;
                var eid = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = eid >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var masks = BitMaskComponents[segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT];
                var start = (segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK) * maskLen;

                builder.AppendLine("Components:");
                for (ushort i = 0; i < maskLen; i++) {
                    var mask = masks[start + i];
                    var offset = i << Const.U64_SHIFT;
                    while (mask > 0) {
                        var id = Utils.PopLsb(ref mask) + offset;
                        if (!_poolsComponents[id].IsTag) {
                            _poolsComponents[id].TryToStringComponent(builder, eid);
                        }
                    }
                }

                builder.AppendLine("Tags:");
                for (ushort i = 0; i < maskLen; i++) {
                    var mask = masks[start + i];
                    var offset = i << Const.U64_SHIFT;
                    while (mask > 0) {
                        var id = Utils.PopLsb(ref mask) + offset;
                        if (_poolsComponents[id].IsTag) {
                            _poolsComponents[id].TryToStringComponent(builder, eid);
                        }
                    }
                }

                return builder.ToString();
            }
            #endregion

            #region CREATE ENTITY FUNCTIONS
            [MethodImpl(NoInlining)]
            internal void CallOnCreate<TEntityType>(TEntityType entityType, Entity entity) where TEntityType : struct, IEntityType {
                entityType.OnCreate(entity);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void CreateEntityWithOnCreate(byte entityType, ushort clusterId, out Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                AssertEntityTypeIsRegistered(EntityTypeName, entityType);
                #endif
                if (!TryCreateEntity(entityType, clusterId, out entity)) {
                    throw new StaticEcsException($"World<{typeof(TWorld)}>, Method: CreateEntity, ran out of space in the attached chunks");
                }
                unsafe {
                    var onCreate = EntityTypes[entityType].OnCreateFn;
                    if (onCreate != null) onCreate(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void CreateEntity<TEntityType>(TEntityType entityType, ushort clusterId, out Entity entity) where TEntityType : struct, IEntityType {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                AssertEntityTypeIsRegistered(EntityTypeName, EntityTypeInfo<TEntityType>.Id);
                #endif
                var type = EntityTypeInfo<TEntityType>.Id;
                if (!TryCreateEntity(type, clusterId, out entity)) {
                    throw new StaticEcsException($"World<{typeof(TWorld)}>, Method: CreateEntity, ran out of space in the attached chunks");
                }
                if (EntityTypeInfo<TEntityType>.HasOnCreate) {
                    CallOnCreate(entityType, entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void CreateEntity(byte entityType, ushort clusterId, out Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                AssertEntityTypeIsRegistered(EntityTypeName, entityType);
                #endif
                if (!TryCreateEntity(entityType, clusterId, out entity)) {
                    throw new StaticEcsException($"World<{typeof(TWorld)}>, Method: CreateEntity, ran out of space in the attached chunks");
                }
            }
            
            [MethodImpl(NoInlining)]
            private bool TryCreateEntityCold(ref EntitiesCluster cluster, byte entityType, ushort clusterId, out uint segmentIdx, out uint chunkIdx) {
                TryFindNextFreeSegmentForType(ref cluster, entityType);
                var freeSegment = cluster.FreeSegmentByType[entityType];
                segmentIdx = (uint) freeSegment;
                chunkIdx = segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT;
                if (freeSegment == EntitiesSegment.Invalid) {
                    if (_selfFreeChunksCount == 0) {
                        if (IndependentWorld) {
                            ResizeWorld((uint) (HeuristicChunks.Length + 4));
                        } else {
                            return false;
                        }
                    }

                    chunkIdx = _selfFreeChunks[--_selfFreeChunksCount];
                    segmentIdx = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;
                    SetChunkSegmentsCluster(chunkIdx, clusterId, true);
                    EntitiesSegments[segmentIdx].EntityType = entityType;
                    #if FFS_ECS_BURST
                    LifecycleHandle.OnSegmentEntityTypeChanged(segmentIdx, entityType);
                    #endif
                    cluster.FreeSegmentByType[entityType] = (int) segmentIdx;
                    if (cluster.ChunksCount == cluster.Chunks.Length) {
                        Array.Resize(ref cluster.Chunks, (int) (cluster.ChunksCount << 1));
                        Array.Resize(ref cluster.LoadedChunks, (int) (cluster.ChunksCount << 1));
                        #if FFS_ECS_BURST
                        LifecycleHandle.OnClusterArraysResized(clusterId);
                        #endif
                    }

                    cluster.Chunks[cluster.ChunksCount++] = chunkIdx;
                    #if FFS_ECS_BURST
                    LifecycleHandle.OnClusterValuesChanged(clusterId);
                    #endif
                }
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryCreateEntity<TEntityType>(TEntityType entityType, ushort clusterId, out Entity entity) where TEntityType : struct, IEntityType {
                var type = EntityTypeInfo<TEntityType>.Id;
                if (TryCreateEntity(type, clusterId, out entity)) {
                    if (EntityTypeInfo<TEntityType>.HasOnCreate) {
                        CallOnCreate(entityType, entity);
                    }

                    return true;
                }
                return false;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryCreateEntity(byte entityType, ushort clusterId, out Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                AssertEntityTypeIsRegistered(EntityTypeName, entityType);
                #endif

                ref var cluster = ref Clusters[clusterId];

                uint chunkIdx;
                uint segmentIdx;
                var freeSegment = cluster.FreeSegmentByType[entityType];
                if (freeSegment != EntitiesSegment.Invalid) {
                    segmentIdx = (uint) freeSegment;
                    chunkIdx = segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT;
                } else if (!TryCreateEntityCold(ref cluster, entityType, clusterId, out segmentIdx, out chunkIdx)) {
                    entity = default;
                    return false;
                }

                ref var heuristic = ref HeuristicChunks[chunkIdx];
                ref var segment = ref EntitiesSegments[segmentIdx];
                var segmentInChunk = (int)(segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK);
                var segmentBlocksOffset = segmentInChunk << Const.BLOCKS_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = Utils.Lsb(~(heuristic.FullBlocks.Value >> segmentBlocksOffset) & ((1UL << Const.BLOCKS_IN_SEGMENT) - 1));
                ref var entitiesMask = ref segment.Masks[segmentBlockIdx];
                var blockEntityIdx = Utils.Lsb(~entitiesMask);

                if (entitiesMask == 0) {
                    var chunkBlockMask = 1UL << (segmentBlocksOffset + segmentBlockIdx);
                    heuristic.NotEmptyBlocks.Value |= chunkBlockMask;
                    ref var loaded = ref HeuristicLoadedChunks[chunkIdx];
                    if (loaded.Value == 0) {
                        cluster.LoadedChunks[cluster.LoadedChunksCount++] = chunkIdx;
                        #if FFS_ECS_BURST
                        LifecycleHandle.OnClusterValuesChanged(clusterId);
                        #endif
                    }
                    loaded.Value |= chunkBlockMask;
                    EntityTypes[entityType].HeuristicChunks[chunkIdx].Value |= chunkBlockMask;
                }

                entitiesMask |= 1UL << blockEntityIdx;
                segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT * 2] |= 1UL << blockEntityIdx; // Loaded entities mask

                if (TrackCreated) {
                    SetCreatedBit(segmentIdx, (byte)segmentBlockIdx, 1UL << blockEntityIdx, chunkIdx, (byte)(segmentBlocksOffset + segmentBlockIdx));
                }

                if (entitiesMask == ulong.MaxValue) {
                    var chunkBlockMask = 1UL << (segmentBlocksOffset + segmentBlockIdx);
                    heuristic.FullBlocks.Value |= chunkBlockMask;
                    var mask = SegmentsMaskCache[segmentInChunk];
                    if ((heuristic.FullBlocks.Value & mask) == mask) {
                        TryFindNextFreeSegmentForType(ref cluster, entityType);
                    }
                }

                entity.IdWithOffset = (uint) ((segmentIdx << Const.ENTITIES_IN_SEGMENT_SHIFT) + (segmentBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT) + blockEntityIdx + Const.ENTITY_ID_OFFSET);
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            internal void CreateEntity(byte entityType, uint chunkIdx, out Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertChunkIsRegistered(EntityTypeName, chunkIdx);
                AssertEntityTypeIsRegistered(EntityTypeName, entityType);
                #endif
                if (!TryCreateEntity(entityType, chunkIdx, out entity)) {
                    throw new StaticEcsException($"World<{typeof(TWorld)}>, Method: CreateEntity, ran out of space in chunk {chunkIdx}");
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void CreateEntity<TEntityType>(TEntityType entityType, uint chunkIdx, out Entity entity) where TEntityType : struct, IEntityType {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertChunkIsRegistered(EntityTypeName, chunkIdx);
                AssertEntityTypeIsRegistered(EntityTypeName, EntityTypeInfo<TEntityType>.Id);
                #endif
                var type = EntityTypeInfo<TEntityType>.Id;
                if (!TryCreateEntity(type, chunkIdx, out entity)) {
                    throw new StaticEcsException($"World<{typeof(TWorld)}>, Method: CreateEntity, ran out of space in chunk {chunkIdx}");
                }
                if (EntityTypeInfo<TEntityType>.HasOnCreate) {
                    CallOnCreate(entityType, entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void CreateEntityWithOnCreate(byte entityType, uint chunkIdx, out Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertChunkIsRegistered(EntityTypeName, chunkIdx);
                AssertEntityTypeIsRegistered(EntityTypeName, entityType);
                #endif
                if (!TryCreateEntity(entityType, chunkIdx, out entity)) {
                    throw new StaticEcsException($"World<{typeof(TWorld)}>, Method: CreateEntity, ran out of space in the attached chunks");
                }
                unsafe {
                    var onCreate = EntityTypes[entityType].OnCreateFn;
                    if (onCreate != null) onCreate(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryCreateEntity<TEntityType>(TEntityType entityType, uint chunkIdx, out Entity entity) where TEntityType : struct, IEntityType {
                var type = EntityTypeInfo<TEntityType>.Id;
                if (TryCreateEntity(type, chunkIdx, out entity)) {
                    if (EntityTypeInfo<TEntityType>.HasOnCreate) {
                        CallOnCreate(entityType, entity);
                    }

                    return true;
                }
                return false;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryCreateEntity(byte entityType, uint chunkIdx, out Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertChunkIsRegistered(EntityTypeName, chunkIdx);
                AssertEntityTypeIsRegistered(EntityTypeName, entityType);
                Assert(EntityTypeName, EntitiesSegments[chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT].SelfOwner, $"Chunk {chunkIdx} has the ownership type Other");
                #endif

                ref var heuristic = ref HeuristicChunks[chunkIdx];

                var baseSegmentIdx = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;
                ulong eligibleBlocks = 0;
                for (var s = 0; s < Const.SEGMENTS_IN_CHUNK; s++) {
                    var segMask = SegmentsMaskCache[s];
                    if (EntitiesSegments[baseSegmentIdx + s].EntityType == entityType || (heuristic.NotEmptyBlocks.Value & segMask) == 0) {
                        eligibleBlocks |= segMask;
                    }
                }

                var freeEligible = eligibleBlocks & ~heuristic.FullBlocks.Value;
                if (freeEligible == 0) {
                    entity.IdWithOffset = default;
                    return false;
                }

                var chunkBlockIdx = Utils.Lsb(freeEligible);
                var segmentIdx = (baseSegmentIdx) + (uint)(chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT);
                var segmentBlockIdx = chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK;
                ref var segment = ref EntitiesSegments[segmentIdx];
                ref var entitiesMask = ref segment.Masks[segmentBlockIdx];
                var blockEntityIdx = Utils.Lsb(~entitiesMask);

                if (entitiesMask == 0) {
                    var chunkBlockMask = 1UL << chunkBlockIdx;
                    heuristic.NotEmptyBlocks.Value |= chunkBlockMask;
                    segment.EntityType = entityType;
                    #if FFS_ECS_BURST
                    LifecycleHandle.OnSegmentEntityTypeChanged(segmentIdx, entityType);
                    #endif
                    ref var loaded = ref HeuristicLoadedChunks[chunkIdx];
                    if (loaded.Value == 0) {
                        Clusters[segment.ClusterId].LoadedChunks[Clusters[segment.ClusterId].LoadedChunksCount++] = chunkIdx;
                        #if FFS_ECS_BURST
                        LifecycleHandle.OnClusterValuesChanged(segment.ClusterId);
                        #endif
                    }
                    loaded.Value |= chunkBlockMask;
                    EntityTypes[entityType].HeuristicChunks[chunkIdx].Value |= chunkBlockMask;
                }

                entitiesMask |= 1UL << blockEntityIdx;
                segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT * 2] |= 1UL << blockEntityIdx; // Loaded entities mask

                if (TrackCreated) {
                    SetCreatedBit(segmentIdx, (byte)segmentBlockIdx, 1UL << blockEntityIdx, chunkIdx, (byte)chunkBlockIdx);
                }

                if (entitiesMask == ulong.MaxValue) {
                    var chunkBlockMask = 1UL << chunkBlockIdx;
                    heuristic.FullBlocks.Value |= chunkBlockMask;
                    ref var cluster = ref Clusters[segment.ClusterId];
                    var segmentInChunk = (int)(segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK);
                    var mask = SegmentsMaskCache[segmentInChunk];
                    if ((heuristic.FullBlocks.Value & mask) == mask && cluster.FreeSegmentByType[segment.EntityType] == (int) segmentIdx) {
                        TryFindNextFreeSegmentForType(ref cluster, (byte)segment.EntityType);
                    }
                }

                entity.IdWithOffset = (uint) ((segmentIdx << Const.ENTITIES_IN_SEGMENT_SHIFT) + (segmentBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT) + blockEntityIdx + Const.ENTITY_ID_OFFSET);
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            internal void CreateEntityWithOnCreate(byte entityType, EntityGID gid, out Entity entity) {
                CreateEntity(entityType, gid, out entity);
                unsafe {
                    var onCreate = EntityTypes[entityType].OnCreateFn;
                    if (onCreate != null) onCreate(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void CreateEntity<TEntityType>(TEntityType entityType, EntityGID gid, out Entity entity) where TEntityType : struct, IEntityType {
                var type = EntityTypeInfo<TEntityType>.Id;
                CreateEntity(type, gid, out entity);
                if (EntityTypeInfo<TEntityType>.HasOnCreate) {
                    CallOnCreate(entityType, entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void CreateEntity(byte entityType, EntityGID gid, out Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertEntityTypeIsRegistered(EntityTypeName, entityType);
                #endif

                var eid = gid.Id;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var segmentIdx = eid >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var chunkBlockIdx = (int)((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                var segmentBlockIdx = (int)((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityIdx = (int)(eid & Const.ENTITIES_IN_BLOCK_MASK);
                var segmentEntityIdx = (int)(eid & Const.ENTITIES_IN_SEGMENT_MASK);

                if (chunkIdx >= HeuristicChunks.Length) {
                    ResizeWorld((chunkIdx + 1).Normalize(4));
                }

                var baseSegmentIdx = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;

                if (!ClusterIsRegisteredInternal(gid.ClusterId)) {
                    RegisterClusterInternal(gid.ClusterId);
                }

                if (EntitiesSegments[baseSegmentIdx].ClusterId == EntitiesSegment.InvalidCluster) {
                    RegisterChunkInternal(chunkIdx, ChunkOwnerType.Other, gid.ClusterId);
                }

                #if FFS_ECS_DEBUG
                AssertGidIsNotActive(EntityTypeName, gid);
                Assert(EntityTypeName, !EntitiesSegments[baseSegmentIdx].SelfOwner, "It is possible to create entities with GID only for chunks with Other ownership");
                if (EntitiesSegments[baseSegmentIdx].ClusterId != gid.ClusterId) throw new StaticEcsException($"World<{typeof(TWorld)}>, Method: CreateEntity, Chunk cluster {EntitiesSegments[baseSegmentIdx].ClusterId} not equal GID cluster {gid.ClusterId}");
                #endif

                ref var segment = ref EntitiesSegments[segmentIdx];
                #if FFS_ECS_DEBUG
                if (segment.Masks[segmentBlockIdx] != 0 && segment.EntityType != entityType) {
                    throw new StaticEcsException($"World<{typeof(TWorld)}>, Method: CreateEntity, Segment {segmentIdx} already has entities of type {segment.EntityType}, cannot assign type {entityType}");
                }
                #endif
                segment.EntityType = entityType;
                #if FFS_ECS_BURST
                LifecycleHandle.OnSegmentEntityTypeChanged(segmentIdx, entityType);
                #endif
                ref var entitiesMask = ref segment.Masks[segmentBlockIdx];
                if (entitiesMask == 0) {
                    var chunkBlockMask = 1UL << chunkBlockIdx;
                    HeuristicChunks[chunkIdx].NotEmptyBlocks.Value |= chunkBlockMask;
                    ref var loaded = ref HeuristicLoadedChunks[chunkIdx];
                    if (loaded.Value == 0) {
                        ref var cluster = ref Clusters[gid.ClusterId];
                        cluster.LoadedChunks[cluster.LoadedChunksCount++] = chunkIdx;
                        #if FFS_ECS_BURST
                        LifecycleHandle.OnClusterValuesChanged(gid.ClusterId);
                        #endif
                    }
                    loaded.Value |= chunkBlockMask;
                    EntityTypes[entityType].HeuristicChunks[chunkIdx].Value |= chunkBlockMask;
                }

                entitiesMask |= 1UL << blockEntityIdx;
                segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT * 2] |= 1UL << blockEntityIdx; // Loaded entities mask
                segment.Versions[segmentEntityIdx] = gid.Version;

                if (TrackCreated) {
                    SetCreatedBit(segmentIdx, (byte)segmentBlockIdx, 1UL << blockEntityIdx, chunkIdx, (byte)chunkBlockIdx);
                }

                if (entitiesMask == ulong.MaxValue) {
                    var chunkBlockMask = 1UL << chunkBlockIdx;
                    HeuristicChunks[chunkIdx].FullBlocks.Value |= chunkBlockMask;
                    ref var cluster = ref Clusters[EntitiesSegments[baseSegmentIdx].ClusterId];
                    var segmentInChunk = (int)(segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK);
                    var mask = SegmentsMaskCache[segmentInChunk];
                    if ((HeuristicChunks[chunkIdx].FullBlocks.Value & mask) == mask && cluster.FreeSegmentByType[segment.EntityType] == (int) segmentIdx) {
                        TryFindNextFreeSegmentForType(ref cluster, (byte)segment.EntityType);
                    }
                }

                entity = new Entity(eid);
            }

            [MethodImpl(AggressiveInlining)]
            internal uint CreateEntitiesBatch(byte entityType, ushort clusterId, uint count, out ulong newEntitiesMask, out uint segmentIdx, out byte segmentBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                AssertEntityTypeIsRegistered(EntityTypeName, entityType);
                #endif

                ref var cluster = ref Clusters[clusterId];

                uint chunkIdx;
                var freeSegment = cluster.FreeSegmentByType[entityType];
                if (freeSegment != EntitiesSegment.Invalid) {
                    segmentIdx = (uint) freeSegment;
                    chunkIdx = segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT;
                } else if (!TryCreateEntityCold(ref cluster, entityType, clusterId, out segmentIdx, out chunkIdx)) {
                    throw new StaticEcsException($"World<{typeof(TWorld)}>, Method: CreateEntitiesBatch, ran out of space in the attached chunks");
                }

                ref var heuristic = ref HeuristicChunks[chunkIdx];
                ref var segment = ref EntitiesSegments[segmentIdx];
                var segmentInChunk = (int)(segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK);
                var segmentBlocksOffset = segmentInChunk << Const.BLOCKS_IN_SEGMENT_SHIFT;
                segmentBlockIdx = (byte) Utils.Lsb(~(heuristic.FullBlocks.Value >> segmentBlocksOffset) & ((1UL << Const.BLOCKS_IN_SEGMENT) - 1));
                ref var entitiesMask = ref segment.Masks[segmentBlockIdx];

                var freeMask = ~entitiesMask;
                var freeCount = (uint) freeMask.PopCnt();
                var toCreate = count < freeCount ? count : freeCount;

                newEntitiesMask = 0;
                var remaining = freeMask;
                for (uint i = 0; i < toCreate; i++) {
                    var bit = remaining & (ulong) -(long) remaining;
                    newEntitiesMask |= bit;
                    remaining &= remaining - 1;
                }

                if (entitiesMask == 0) {
                    var chunkBlockMask = 1UL << (segmentBlocksOffset + segmentBlockIdx);
                    heuristic.NotEmptyBlocks.Value |= chunkBlockMask;
                    ref var loaded = ref HeuristicLoadedChunks[chunkIdx];
                    if (loaded.Value == 0) {
                        cluster.LoadedChunks[cluster.LoadedChunksCount++] = chunkIdx;
                        #if FFS_ECS_BURST
                        LifecycleHandle.OnClusterValuesChanged(clusterId);
                        #endif
                    }
                    loaded.Value |= chunkBlockMask;
                    EntityTypes[entityType].HeuristicChunks[chunkIdx].Value |= chunkBlockMask;
                }

                entitiesMask |= newEntitiesMask;
                segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT * 2] |= newEntitiesMask; // Loaded entities mask

                if (TrackCreated) {
                    SetCreatedBitBatch(newEntitiesMask, segmentIdx, segmentBlockIdx, chunkIdx, (byte)(segmentBlocksOffset + segmentBlockIdx));
                }

                if (entitiesMask == ulong.MaxValue) {
                    var chunkBlockMask = 1UL << (segmentBlocksOffset + segmentBlockIdx);
                    heuristic.FullBlocks.Value |= chunkBlockMask;
                    var mask = SegmentsMaskCache[segmentInChunk];
                    if ((heuristic.FullBlocks.Value & mask) == mask) {
                        TryFindNextFreeSegmentForType(ref cluster, entityType);
                    }
                }

                return toCreate;
            }

            [MethodImpl(AggressiveInlining)]
            internal static void InvokeOnCreateBatch<TEntityType>(TEntityType entityType, QueryFunctionWithEntity<TWorld> onCreate, ulong mask, uint segmentIdx, byte segmentBlockIdx)
                where TEntityType : struct, IEntityType {

                var hasOnCreate = EntityTypeInfo<TEntityType>.HasOnCreate;
                
                if (!hasOnCreate && onCreate == null) return;

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                var baseGlobalBlockEntityIdx = (uint)((segmentIdx << Const.ENTITIES_IN_SEGMENT_SHIFT) + (segmentBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT));
                var entity = new Entity(baseGlobalBlockEntityIdx);
                ref var entityIdRef = ref entity.IdWithOffset;

                var starts = mask & ~(mask << 1);
                var ends = mask & ~(mask >> 1);
                do {
                    #if NET6_0_OR_GREATER
                    var start = (byte)System.Numerics.BitOperations.TrailingZeroCount(starts);
                    var endIncluded = (byte)System.Numerics.BitOperations.TrailingZeroCount(ends);
                    #else
                    var start = deBruijn[(uint)(((starts & (ulong)-(long)starts) * 0x37E84A99DAE458FUL) >> 58)];
                    var endIncluded = deBruijn[(uint)(((ends & (ulong)-(long)ends) * 0x37E84A99DAE458FUL) >> 58)];
                    #endif
                    starts &= starts - 1UL;
                    ends &= ends - 1UL;

                    entityIdRef = baseGlobalBlockEntityIdx + start + Const.ENTITY_ID_OFFSET;

                    while (start <= endIncluded) {
                        if (hasOnCreate) {
                            entityType.OnCreate(entity);
                        }
                        onCreate?.Invoke(entity);
                        entityIdRef++;
                        start++;
                    }
                } while (starts != 0);
            }
            #endregion
            
            #region ENTITY OPERATIONS
            [MethodImpl(AggressiveInlining)]
            internal readonly void UpEntityVersion(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                #endif
                
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                ref var segment = ref EntitiesSegments[entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT];
                ref var version = ref segment.Versions[entityId & Const.ENTITIES_IN_SEGMENT_MASK];
                version = (ushort)(version == ushort.MaxValue ? 1 : version + 1);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal readonly void CopyEntity(Entity srcEntity, Entity dstEntity) {
                var maskLen = BitMaskComponentsLen;
                var srcEid = srcEntity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = srcEid >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var masks = BitMaskComponents[segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT];
                var start = (segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK) * maskLen;
                for (ushort i = 0; i < maskLen; i++) {
                    var mask = masks[start + i];
                    var offset = i << Const.U64_SHIFT;
                    while (mask > 0) {
                        var id = Utils.PopLsb(ref mask) + offset;
                        _poolsComponents[id].Copy(srcEntity.ID, dstEntity.ID);
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly void DisableEntity(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                AssertNotBlockedByQuery(EntityTypeName, entity, _blockerDisable);
                AssertNotBlockedByParallelQuery(EntityTypeName, entity);
                #endif
                
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);
                EntitiesSegments[segmentIdx].Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] |= blockEntityMask;
                for (uint i = 0; i < _queriesToUpdateOnDisableCount; i++) {
                    _queriesToUpdateOnDisable[i].Update(~blockEntityMask, segmentIdx, segmentBlockIdx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly void EnableEntity(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                AssertNotBlockedByQuery(EntityTypeName, entity, _blockerEnable);
                AssertNotBlockedByParallelQuery(EntityTypeName, entity);
                #endif
                
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var invertedBlockEntityMask = ~(1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK));
                EntitiesSegments[segmentIdx].Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] &= invertedBlockEntityMask;
                for (uint i = 0; i < _queriesToUpdateOnEnableCount; i++) {
                    _queriesToUpdateOnEnable[i].Update(invertedBlockEntityMask, segmentIdx, segmentBlockIdx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void LoadEntity(EntityGID gid, out Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertGidIsNotLoaded(EntityTypeName, gid);
                #endif

                var eid = gid.Id;
                var chunkIdx = eid >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var segmentIdx = eid >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var chunkBlockIdx = (int)((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                var segmentBlockIdx = (int)((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityIdx = (int)(eid & Const.ENTITIES_IN_BLOCK_MASK);

                var baseSegmentIdx = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;
                ref var segment = ref EntitiesSegments[segmentIdx];
                #if FFS_ECS_DEBUG
                if (!Clusters[gid.ClusterId].Registered) throw new StaticEcsException($"World<{typeof(TWorld)}>, Method: LoadEntity, Cluster {EntitiesSegments[baseSegmentIdx].ClusterId} not registered");
                if (EntitiesSegments[baseSegmentIdx].ClusterId != gid.ClusterId) throw new StaticEcsException($"World<{typeof(TWorld)}>, Method: LoadEntity, Chunk cluster {EntitiesSegments[baseSegmentIdx].ClusterId} not equal GID cluster {gid.ClusterId}");
                var segmentEntityIdx = (int)(eid & Const.ENTITIES_IN_SEGMENT_MASK);
                if (segment.Versions[segmentEntityIdx] != gid.Version) throw new StaticEcsException($"World<{typeof(TWorld)}>, Method: LoadEntity, Entity version {segment.Versions[segmentEntityIdx]} not equal GID version {gid.Version}");
                #endif
                
                ref var loaded = ref HeuristicLoadedChunks[chunkIdx];
                if (loaded.Value == 0) {
                    ref var cluster = ref Clusters[gid.ClusterId];
                    cluster.LoadedChunks[cluster.LoadedChunksCount++] = chunkIdx;
                    #if FFS_ECS_BURST
                    LifecycleHandle.OnClusterValuesChanged(gid.ClusterId);
                    #endif
                }

                ref var masks = ref segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT * 2];  // Loaded entities mask
                if (masks == 0) {
                    loaded.SetBit((byte) chunkBlockIdx);
                }
                masks |= 1UL << blockEntityIdx;
                entity = new Entity(eid);
            }        
            
            [MethodImpl(AggressiveInlining)]
            internal void UnloadEntity(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, entity);
                AssertNotBlockedByParallelQuery(EntityTypeName, entity);
                #endif
                
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var chunkIdx = entityId >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                var segmentBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityInvMask = ~(1UL << (int)(entityId & Const.ENTITIES_IN_BLOCK_MASK));

                ref var segment = ref EntitiesSegments[segmentIdx];
                var entityTypeData = EntityTypes[segment.EntityType];
                unsafe {
                    if (entityTypeData.OnDestroyFn != null) {
                        entityTypeData.OnDestroyFn(entity, HookReason.UnloadEntity);
                    }
                }

                DestroyEntityComponentsAndTags(entity, HookReason.UnloadEntity);

                segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT * 2] &= blockEntityInvMask;

                if (segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT * 2] == 0) {
                    if (HeuristicLoadedChunks[chunkIdx].ClearBit(chunkBlockIdx) == 0) {
                        var clusterId = EntitiesSegments[chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT].ClusterId;
                        ref var cluster = ref Clusters[clusterId];

                        var taken = false;
                        cluster.Lock.Enter(ref taken);
                        #if FFS_ECS_DEBUG
                        if (!taken) throw new StaticEcsException($"Failed to acquire cluster lock for cluster {clusterId}");
                        #endif
                        RemoveLoadedChunkFromCluster(ref cluster, chunkIdx, clusterId);
                        cluster.Lock.Exit();
                    }
                }

                for (uint i = 0; i < _queriesToUpdateOnDestroyCount; i++) {
                    _queriesToUpdateOnDestroy[i].Update(blockEntityInvMask, segmentIdx, segmentBlockIdx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal bool DestroyEntity(Entity entity, HookReason reason = HookReason.Default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertNotBlockedByParallelQuery(EntityTypeName, entity);
                AssertNotBlockedByQuery(EntityTypeName, entity, _blockerDestroy);
                #endif

                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var chunkIdx = entityId >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                var segmentBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityInvMask = ~(1UL << (int)(entityId & Const.ENTITIES_IN_BLOCK_MASK));
                var segmentEntityIdx = (int)(entityId & Const.ENTITIES_IN_SEGMENT_MASK);
                
                ref var segment = ref EntitiesSegments[segmentIdx];
                ref var entitiesMask = ref segment.Masks[segmentBlockIdx];

                var blockEntityMask = ~blockEntityInvMask;
                if ((entitiesMask & blockEntityMask) == 0) {
                    return false;
                }

                var entityTypeData = EntityTypes[segment.EntityType];
                unsafe {
                    if (entityTypeData.OnDestroyFn != null) {
                        entityTypeData.OnDestroyFn(entity, reason);
                    }
                }

                DestroyEntityComponentsAndTags(entity, reason);

                if (entitiesMask == ulong.MaxValue) {
                    HeuristicChunks[chunkIdx].FullBlocks.ClearBit(chunkBlockIdx);
                }

                entitiesMask &= blockEntityInvMask;
                segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] &= blockEntityInvMask; // Disabled entities mask
                segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT * 2] &= blockEntityInvMask; // Loaded entities mask
                ref var version = ref segment.Versions[segmentEntityIdx];
                version = (ushort)(version == ushort.MaxValue ? 1 : version + 1);

                if (entitiesMask == 0) {
                    HeuristicChunks[chunkIdx].NotEmptyBlocks.ClearBit(chunkBlockIdx);
                    entityTypeData.HeuristicChunks[chunkIdx].ClearBit(chunkBlockIdx);
                }

                if (segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT * 2] == 0) {
                    if (HeuristicLoadedChunks[chunkIdx].ClearBit(chunkBlockIdx) == 0) {
                        var clusterId = EntitiesSegments[chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT].ClusterId;
                        ref var cluster = ref Clusters[clusterId];

                        var taken = false;
                        cluster.Lock.Enter(ref taken);
                        #if FFS_ECS_DEBUG
                        if (!taken) throw new StaticEcsException($"Failed to acquire cluster lock for cluster {clusterId}");
                        #endif
                        RemoveLoadedChunkFromCluster(ref cluster, chunkIdx, clusterId);
                        cluster.Lock.Exit();
                    }
                }

                for (uint i = 0; i < _queriesToUpdateOnDestroyCount; i++) {
                    _queriesToUpdateOnDestroy[i].Update(blockEntityInvMask, segmentIdx, segmentBlockIdx);
                }

                return true;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void DestroyEntitiesBatch(QueryData queryData, int nextGlobalBlockIdx, HookReason reason = HookReason.Default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                #endif

                while (nextGlobalBlockIdx >= 0) {
                    ref var cache = ref queryData.Blocks[nextGlobalBlockIdx];

                    var globalBlockIdx = (uint)nextGlobalBlockIdx;
                    nextGlobalBlockIdx = cache.NextGlobalBlock;

                    var segmentIdx = globalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                    var segmentBlockIdx = (byte)(globalBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);

                    DestroyEntitiesBatch(cache.EntitiesMask, segmentIdx, segmentBlockIdx, reason);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void DestroyEntitiesBatch(ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx, HookReason reason = HookReason.Default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                #endif

                if (entitiesMaskFilter == 0) {
                    return;
                }

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                var chunkIdx = segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT;
                var chunkBlockIdx = (byte)((segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK) * Const.BLOCKS_IN_SEGMENT + segmentBlockIdx);
                var baseSegmentEntityIdx = (ushort)(segmentBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT);

                ref var segment = ref EntitiesSegments[segmentIdx];
                ref var entitiesMask = ref segment.Masks[segmentBlockIdx];
                var entityTypeData = EntityTypes[segment.EntityType];
                unsafe {
                    if (entityTypeData.OnDestroyFn != null) {
                        var baseGlobalBlockEntityIdx = (uint)((segmentIdx << Const.ENTITIES_IN_SEGMENT_SHIFT) + (segmentBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT));
                        var runStarts = entitiesMaskFilter & ~(entitiesMaskFilter << 1);
                        var runEnds = entitiesMaskFilter & ~(entitiesMaskFilter >> 1);
                        do {
                            #if NET6_0_OR_GREATER
                            var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(runStarts);
                            var runEnd = (byte)System.Numerics.BitOperations.TrailingZeroCount(runEnds);
                            #else
                            var runStart = deBruijn[(uint)(((runStarts & (ulong)-(long)runStarts) * 0x37E84A99DAE458FUL) >> 58)];
                            var runEnd = deBruijn[(uint)(((runEnds & (ulong)-(long)runEnds) * 0x37E84A99DAE458FUL) >> 58)];
                            #endif
                            runStarts &= runStarts - 1UL;
                            runEnds &= runEnds - 1UL;

                            Entity entity = default;
                            ref var entityId = ref entity.IdWithOffset;
                            entityId = baseGlobalBlockEntityIdx + runStart + Const.ENTITY_ID_OFFSET;
                            while (runStart <= runEnd) {
                                entityTypeData.OnDestroyFn(entity, reason);
                                entityId++;
                                runStart++;
                            }
                        } while (runStarts != 0);
                    }
                }

                var maskLen = BitMaskComponentsLen;
                var masks = BitMaskComponents[chunkIdx];
                var start = (segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK) * maskLen;
                for (ushort mi = 0; mi < maskLen; mi++) {
                    var poolMask = masks[start + mi];
                    var offset = mi << Const.U64_SHIFT;
                    while (poolMask > 0) {
                        var id = Utils.PopLsb(ref poolMask) + offset;
                        _poolsComponents[id].BatchDelete(entitiesMaskFilter, segmentIdx, segmentBlockIdx, reason);
                    }
                }

                var invertedFilter = ~entitiesMaskFilter;
                if (entitiesMask == ulong.MaxValue) {
                    HeuristicChunks[chunkIdx].FullBlocks.ClearBit(chunkBlockIdx);
                }

                entitiesMask &= invertedFilter;
                segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] &= invertedFilter; // Disabled entities mask
                segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT * 2] &= invertedFilter; // Loaded entities mask

                if (entitiesMask == 0) {
                    HeuristicChunks[chunkIdx].NotEmptyBlocks.ClearBit(chunkBlockIdx);
                    entityTypeData.HeuristicChunks[chunkIdx].ClearBit(chunkBlockIdx);
                }

                var versionMask = entitiesMaskFilter;
                while (versionMask != 0) {
                    #if NET6_0_OR_GREATER
                    var bit = (ushort)System.Numerics.BitOperations.TrailingZeroCount(versionMask);
                    #else
                    var bit = (ushort)(deBruijn[(uint)(((versionMask & (ulong)-(long)versionMask) * 0x37E84A99DAE458FUL) >> 58)]);
                    #endif
                    versionMask &= versionMask - 1;
                    ref var version = ref segment.Versions[baseSegmentEntityIdx + bit];
                    version = (ushort)(version == ushort.MaxValue ? 1 : version + 1);
                }

                if (segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT * 2] == 0) {
                    if (HeuristicLoadedChunks[chunkIdx].ClearBit(chunkBlockIdx) == 0) {
                        var clId = EntitiesSegments[chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT].ClusterId;
                        ref var cluster = ref Clusters[clId];

                        var taken = false;
                        cluster.Lock.Enter(ref taken);
                        #if FFS_ECS_DEBUG
                        if (!taken) throw new StaticEcsException($"Failed to acquire cluster lock for cluster {clId}");
                        #endif
                        RemoveLoadedChunkFromCluster(ref cluster, chunkIdx, clId);
                        cluster.Lock.Exit();
                    }
                }

                for (uint i = 0; i < _queriesToUpdateOnDestroyCount; i++) {
                    _queriesToUpdateOnDestroy[i].Update(invertedFilter, segmentIdx, segmentBlockIdx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void UnloadEntitiesBatch(QueryData queryData, int nextGlobalBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                #endif

                while (nextGlobalBlockIdx >= 0) {
                    ref var cache = ref queryData.Blocks[nextGlobalBlockIdx];

                    var globalBlockIdx = (uint)nextGlobalBlockIdx;
                    nextGlobalBlockIdx = cache.NextGlobalBlock;

                    var segmentIdx = globalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                    var segmentBlockIdx = (byte)(globalBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);

                    UnloadEntitiesBatch(cache.EntitiesMask, segmentIdx, segmentBlockIdx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void UnloadEntitiesBatch(ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                #endif

                if (entitiesMaskFilter == 0) {
                    return;
                }

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                var chunkIdx = segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT;
                var chunkBlockIdx = (byte)((segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK) * Const.BLOCKS_IN_SEGMENT + segmentBlockIdx);

                ref var segment = ref EntitiesSegments[segmentIdx];
                var entityTypeData = EntityTypes[segment.EntityType];
                unsafe {
                    if (entityTypeData.OnDestroyFn != null) {
                        var baseGlobalBlockEntityIdx = (uint)((segmentIdx << Const.ENTITIES_IN_SEGMENT_SHIFT) + (segmentBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT));
                        var runStarts = entitiesMaskFilter & ~(entitiesMaskFilter << 1);
                        var runEnds = entitiesMaskFilter & ~(entitiesMaskFilter >> 1);
                        do {
                            #if NET6_0_OR_GREATER
                            var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(runStarts);
                            var runEnd = (byte)System.Numerics.BitOperations.TrailingZeroCount(runEnds);
                            #else
                            var runStart = deBruijn[(uint)(((runStarts & (ulong)-(long)runStarts) * 0x37E84A99DAE458FUL) >> 58)];
                            var runEnd = deBruijn[(uint)(((runEnds & (ulong)-(long)runEnds) * 0x37E84A99DAE458FUL) >> 58)];
                            #endif
                            runStarts &= runStarts - 1UL;
                            runEnds &= runEnds - 1UL;

                            Entity entity = default;
                            ref var entityId = ref entity.IdWithOffset;
                            entityId = baseGlobalBlockEntityIdx + runStart + Const.ENTITY_ID_OFFSET;
                            while (runStart <= runEnd) {
                                entityTypeData.OnDestroyFn(entity, HookReason.UnloadEntity);
                                entityId++;
                                runStart++;
                            }
                        } while (runStarts != 0);
                    }
                }

                var maskLen = BitMaskComponentsLen;
                var masks = BitMaskComponents[chunkIdx];
                var start = (segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK) * maskLen;
                for (ushort mi = 0; mi < maskLen; mi++) {
                    var poolMask = masks[start + mi];
                    var offset = mi << Const.U64_SHIFT;
                    while (poolMask > 0) {
                        var id = Utils.PopLsb(ref poolMask) + offset;
                        _poolsComponents[id].BatchDelete(entitiesMaskFilter, segmentIdx, segmentBlockIdx, HookReason.UnloadEntity);
                    }
                }

                var invertedFilter = ~entitiesMaskFilter;
                segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT * 2] &= invertedFilter; // Loaded entities mask

                if (segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT * 2] == 0) {
                    if (HeuristicLoadedChunks[chunkIdx].ClearBit(chunkBlockIdx) == 0) {
                        var clId = EntitiesSegments[chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT].ClusterId;
                        ref var cluster = ref Clusters[clId];

                        var taken = false;
                        cluster.Lock.Enter(ref taken);
                        #if FFS_ECS_DEBUG
                        if (!taken) throw new StaticEcsException($"Failed to acquire cluster lock for cluster {clId}");
                        #endif
                        RemoveLoadedChunkFromCluster(ref cluster, chunkIdx, clId);
                        cluster.Lock.Exit();
                    }
                }

                for (uint i = 0; i < _queriesToUpdateOnDestroyCount; i++) {
                    _queriesToUpdateOnDestroy[i].Update(invertedFilter, segmentIdx, segmentBlockIdx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            private void DestroyEntityComponentsAndTags(Entity entity, HookReason reason) {
                var maskLen = BitMaskComponentsLen;
                var eid = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = eid >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var masks = BitMaskComponents[segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT];
                var start = (segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK) * maskLen;
                for (ushort i = 0; i < maskLen; i++) {
                    var mask = masks[start + i];
                    var offset = i << Const.U64_SHIFT;
                    while (mask > 0) {
                        var id = Utils.PopLsb(ref mask) + offset;
                        _poolsComponents[id].Delete(entity.ID, reason);
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            private readonly void SetChunkSegmentsCluster(uint chunkIdx, ushort clusterId, bool selfOwner) {
                var baseSegIdx = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;
                for (var s = 0; s < Const.SEGMENTS_IN_CHUNK; s++) {
                    ref var seg = ref EntitiesSegments[baseSegIdx + s];
                    seg.ClusterId = clusterId;
                    seg.SelfOwner = selfOwner;
                    if (clusterId == EntitiesSegment.InvalidCluster) {
                        seg.EntityType = EntitiesSegment.NoEntityType;
                    }
                }
                #if FFS_ECS_BURST
                LifecycleHandle.OnSetChunkSegmentsCluster(chunkIdx, clusterId, selfOwner);
                #endif
            }
            #endregion

            #region CHUNKS_AND_CLUSTER
            [MethodImpl(AggressiveInlining)]
            internal readonly ReadOnlySpan<ushort> GetActiveClustersIfEmpty(ReadOnlySpan<ushort> clusters) {
                return clusters.Length == 0
                    ? new ReadOnlySpan<ushort>(ActiveClusters, 0, ActiveClustersCount)
                    : clusters;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void RegisterClusterInternal(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertClusterIsNotRegistered(WorldTypeName, clusterId);
                AssertMultiThreadNotActive(WorldTypeName);
                #endif
                
                if (clusterId >= Clusters.Length) {
                    ResizeClusters((int)Utils.RoundUpToPowerOf2((ushort)(clusterId + 1)));
                }

                Clusters[clusterId].Registered = true;
                ActiveClusters[ActiveClustersCount++] = clusterId;

                #if FFS_ECS_BURST
                LifecycleHandle.OnClusterRegistered(clusterId);
                LifecycleHandle.OnActiveClustersChanged();
                #endif
            }
            
            [MethodImpl(AggressiveInlining)]
            internal readonly bool ClusterIsRegisteredInternal(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                return clusterId < Clusters.Length && Clusters[clusterId].Registered;
            }

            [MethodImpl(AggressiveInlining)]
            private void ResizeClusters(int newClustersCapacity) {
                var oldClustersCapacity = Clusters.Length;

                Array.Resize(ref Clusters, newClustersCapacity);
                Array.Resize(ref ActiveClusters, newClustersCapacity);
                #if FFS_ECS_BURST
                LifecycleHandle.OnActiveClustersChanged();
                #endif

                for (var i = oldClustersCapacity; i < newClustersCapacity; i++) {
                    ref var cluster = ref Clusters[i];
                    cluster.Chunks = new uint[Math.Max((int) ActiveClustersCount, 32)];
                    cluster.LoadedChunks = new uint[Math.Max((int) ActiveClustersCount, 32)];
                    cluster.Lock = new SpinLock(false);
                    cluster.ChunksCount = 0;
                    cluster.LoadedChunksCount = 0;
                    cluster.FreeSegmentByType = new int[256];
                    Array.Fill(cluster.FreeSegmentByType, EntitiesSegment.Invalid);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void SetActiveClusterInternal(ushort clusterId, bool active) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertQueryNotActive(WorldTypeName);
                AssertClusterIsRegistered(WorldTypeName, clusterId);
                #endif

                Clusters[clusterId].Disabled = !active;
                #if FFS_ECS_BURST
                LifecycleHandle.OnClusterValuesChanged(clusterId);
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly bool ClusterIsActiveInternal(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertQueryNotActive(WorldTypeName);
                AssertClusterIsRegistered(WorldTypeName, clusterId);
                #endif

                return !Clusters[clusterId].Disabled;
            }

            [MethodImpl(AggressiveInlining)]
            internal void FreeClusterInternal(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertQueryNotActive(WorldTypeName);
                #endif

                if (!TryFreeClusterInternal(clusterId)) {
                    throw new StaticEcsException($"World<{typeof(TWorld)}>", "FreeCluster", $"Cluster {clusterId} not registered");
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryFreeClusterInternal(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertQueryNotActive(WorldTypeName);
                #endif

                if (clusterId >= Clusters.Length) {
                    return false;
                }
                
                ref var cluster = ref Clusters[clusterId];
                if (cluster.Registered) {
                    var chunksCount = cluster.ChunksCount;
                    for (var i = (int) chunksCount - 1; i >= 0; i--) {
                        FreeChunkInternal(cluster.Chunks[i]);
                    }
                    
                    for (var i = ActiveClustersCount - 1; i >= 0; i--) {
                        if (ActiveClusters[i] == clusterId) {
                            ActiveClusters[i] = ActiveClusters[--ActiveClustersCount];
                            break;
                        }
                    }
                    #if FFS_ECS_BURST
                    LifecycleHandle.OnActiveClustersChanged();
                    #endif

                    Array.Fill(cluster.FreeSegmentByType, EntitiesSegment.Invalid);
                    cluster.ChunksCount = 0;
                    cluster.LoadedChunksCount = 0;
                    cluster.Registered = false;
                    cluster.Disabled = false;
                    #if FFS_ECS_BURST
                    LifecycleHandle.OnClusterValuesChanged(clusterId);
                    #endif
                    return true;
                }

                return false;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal readonly ReadOnlySpan<uint> GetClusterChunksInternal(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertClusterIsRegistered(WorldTypeName, clusterId);
                #endif

                ref var cluster = ref Clusters[clusterId];
                return new ReadOnlySpan<uint>(cluster.Chunks, 0, (int) cluster.ChunksCount);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal readonly ReadOnlySpan<uint> GetClusterLoadedChunksInternal(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertClusterIsRegistered(WorldTypeName, clusterId);
                #endif

                ref var cluster = ref Clusters[clusterId];
                return new ReadOnlySpan<uint>(cluster.LoadedChunks, 0, (int) cluster.LoadedChunksCount);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void RegisterChunkInternal(uint chunkIdx, ChunkOwnerType owner, ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertClusterIsRegistered(WorldTypeName, clusterId);
                #endif
                
                if (!TryRegisterChunkInternal(chunkIdx, owner, clusterId)) {
                    throw new StaticEcsException($"Chunk {chunkIdx} already registered");
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryRegisterChunkInternal(uint chunkIdx, ChunkOwnerType owner, ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertClusterIsRegistered(WorldTypeName, clusterId);
                #endif
                
                if (chunkIdx >= HeuristicChunks.Length) {
                    ResizeWorld((chunkIdx + 1).Normalize(4));
                }

                var baseSegmentIdx = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;

                if (EntitiesSegments[baseSegmentIdx].ClusterId != EntitiesSegment.InvalidCluster) {
                    return false;
                }

                var selfOwner = owner == ChunkOwnerType.Self;
                SetChunkSegmentsCluster(chunkIdx, clusterId, selfOwner);

                ref var cluster = ref Clusters[clusterId];
                if (cluster.ChunksCount == cluster.Chunks.Length) {
                    Array.Resize(ref cluster.Chunks, (int) (cluster.ChunksCount << 1));
                    Array.Resize(ref cluster.LoadedChunks, (int) (cluster.ChunksCount << 1));
                    #if FFS_ECS_BURST
                    LifecycleHandle.OnClusterArraysResized(clusterId);
                    #endif
                }

                cluster.Chunks[cluster.ChunksCount++] = chunkIdx;
                #if FFS_ECS_BURST
                LifecycleHandle.OnClusterValuesChanged(clusterId);
                #endif
                if (selfOwner) {
                    RemoveFromSelfFreeChunks(chunkIdx);
                }
                return true;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal readonly bool ChunkIsRegisteredInternal(uint chunkIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                return chunkIdx < (EntitiesSegments.Length >> Const.SEGMENTS_IN_CHUNK_SHIFT) && EntitiesSegments[chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT].ClusterId != EntitiesSegment.InvalidCluster;
            }

            [MethodImpl(AggressiveInlining)]
            internal void FreeChunkInternal(uint chunkIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertQueryNotActive(WorldTypeName);
                #endif
                
                var baseSegmentIdx = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;
                ref var baseSegment = ref EntitiesSegments[baseSegmentIdx];
                var selfOwned = baseSegment.SelfOwner;

                if (baseSegment.ClusterId != EntitiesSegment.InvalidCluster) {
                    ReadOnlySpan<uint> chunks = stackalloc uint[] { chunkIdx };
                    Query().BatchDestroy(chunks, EntityStatusType.Any, StaticEcs.QueryMode.Flexible);
                    ref var cluster = ref Clusters[baseSegment.ClusterId];
                    RemoveChunkFromCluster(ref cluster, chunkIdx, baseSegment.ClusterId);
                    SetChunkSegmentsCluster(chunkIdx, EntitiesSegment.InvalidCluster, IndependentWorld);
                    if (selfOwned) {
                        #if FFS_ECS_DEBUG
                        for (var fi = 0; fi < _selfFreeChunksCount; fi++) {
                            Assert(WorldTypeName, _selfFreeChunks[fi] != chunkIdx, $"Chunk {chunkIdx} already in self free chunks");
                        }
                        #endif
                        _selfFreeChunks[_selfFreeChunksCount++] = chunkIdx;
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            public readonly ushort GetChunkClusterIdInternal(uint chunkIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertChunkIsRegistered(WorldTypeName, chunkIdx);
                #endif

                return EntitiesSegments[chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT].ClusterId;
            }

            [MethodImpl(AggressiveInlining)]
            internal void ChangeChunkClusterInternal(uint chunkIdx, ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertChunkIsRegistered(WorldTypeName, chunkIdx);
                AssertClusterIsRegistered(WorldTypeName, clusterId);
                #endif

                var baseSegmentIdx = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;
                var oldClusterId = EntitiesSegments[baseSegmentIdx].ClusterId;

                if (HeuristicLoadedChunks[chunkIdx].Value != 0) {
                    RemoveLoadedChunkFromCluster(ref Clusters[oldClusterId], chunkIdx, oldClusterId);
                }
                RemoveChunkFromCluster(ref Clusters[oldClusterId], chunkIdx, oldClusterId);

                var selfOwner = EntitiesSegments[baseSegmentIdx].SelfOwner;
                SetChunkSegmentsCluster(chunkIdx, clusterId, selfOwner);

                ref var cluster = ref Clusters[clusterId];
                if (cluster.ChunksCount == cluster.Chunks.Length) {
                    Array.Resize(ref cluster.Chunks, (int) (cluster.ChunksCount << 1));
                    Array.Resize(ref cluster.LoadedChunks, (int) (cluster.ChunksCount << 1));
                    #if FFS_ECS_BURST
                    LifecycleHandle.OnClusterArraysResized(clusterId);
                    #endif
                }
                cluster.Chunks[cluster.ChunksCount++] = chunkIdx;
                if (HeuristicLoadedChunks[chunkIdx].Value != 0) {
                    cluster.LoadedChunks[cluster.LoadedChunksCount++] = chunkIdx;
                }
                #if FFS_ECS_BURST
                LifecycleHandle.OnClusterValuesChanged(clusterId);
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal void LoadCluster(ushort clusterId) {
                ref var cluster = ref Clusters[clusterId];

                var chunksCount = cluster.ChunksCount;
                for (var i = (int) chunksCount - 1; i >= 0; i--) {
                    LoadChunk(cluster.Chunks[i]);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void LoadChunk(uint chunkIdx) {
                var baseSegmentIdx = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;
                ref var cluster = ref Clusters[EntitiesSegments[baseSegmentIdx].ClusterId];

                #if FFS_ECS_DEBUG
                Assert(WorldTypeName, HeuristicLoadedChunks[chunkIdx].Value == 0, $"Incorrect chunk index {chunkIdx}, chunk already has loaded entities");
                #endif

                var notEmptyBlocks = HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
                while (notEmptyBlocks > 0) {
                    var chunkBlockIdx = Utils.PopLsb(ref notEmptyBlocks);
                    var segmentIdx = baseSegmentIdx + (chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT);
                    var segmentBlockIdx = chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK;
                    ref var segment = ref EntitiesSegments[segmentIdx];
                    segment.Masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT * 2] = segment.Masks[segmentBlockIdx]; // Loaded = Active
                }
                HeuristicLoadedChunks[chunkIdx].Value = HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;

                cluster.LoadedChunks[cluster.LoadedChunksCount++] = chunkIdx;
                #if FFS_ECS_BURST
                LifecycleHandle.OnClusterValuesChanged(EntitiesSegments[baseSegmentIdx].ClusterId);
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal void ChangeChunkOwnerInternal(uint chunkIdx, ChunkOwnerType owner) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertChunkIsRegistered(WorldTypeName, chunkIdx);
                #endif
                var baseSegmentIdx = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;
                ref var baseSegment = ref EntitiesSegments[baseSegmentIdx];

                var selfOwner = owner == ChunkOwnerType.Self;

                if (baseSegment.SelfOwner != selfOwner) {
                    ref var cluster = ref Clusters[baseSegment.ClusterId];
                    if (!selfOwner) {
                        InvalidateFreeSegmentsForChunk(ref cluster, chunkIdx);
                    }
                    SetChunkSegmentsCluster(chunkIdx, baseSegment.ClusterId, selfOwner);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly ChunkOwnerType GetChunkOwnerType(uint chunkIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                AssertChunkIsRegistered(WorldTypeName, chunkIdx);
                #endif
                return EntitiesSegments[chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT].SelfOwner ? ChunkOwnerType.Self : ChunkOwnerType.Other;
            }

            [MethodImpl(AggressiveInlining)]
            internal EntitiesChunkInfo FindNextSelfFreeChunkInternal() {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                #endif
                
                if (!TryFindNextSelfFreeChunkInternal(out var chunkInfo)) {
                    throw new StaticEcsException($"World<{typeof(TWorld)}>, Method: FindNextSelfFreeChunk, ran out of space in the self owned chunks");
                }

                return chunkInfo;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryFindNextSelfFreeChunkInternal(out EntitiesChunkInfo chunkInfo) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                AssertMultiThreadNotActive(WorldTypeName);
                #endif
                
                if (_selfFreeChunksCount == 0) {
                    if (IndependentWorld) {
                        ResizeWorld((uint) (HeuristicChunks.Length + 4));
                    } else {
                        chunkInfo = default;
                        return false;
                    }
                }

                chunkInfo = new EntitiesChunkInfo(_selfFreeChunks[_selfFreeChunksCount - 1]);
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            // ReSharper disable once UnusedParameter.Local
            private void RemoveChunkFromCluster(ref EntitiesCluster cluster, uint chunkIdx, int clusterId) {
                for (var i = (int) cluster.ChunksCount - 1; i >= 0; i--) {
                    if (cluster.Chunks[i] == chunkIdx) {
                        cluster.Chunks[i] = cluster.Chunks[--cluster.ChunksCount];
                        InvalidateFreeSegmentsForChunk(ref cluster, chunkIdx);
                        #if FFS_ECS_BURST
                        LifecycleHandle.OnClusterValuesChanged(clusterId);
                        #endif
                        return;
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            // ReSharper disable once UnusedParameter.Local
            private readonly void RemoveLoadedChunkFromCluster(ref EntitiesCluster cluster, uint chunkIdx, int clusterId) {
                for (var i = (int) cluster.LoadedChunksCount - 1; i >= 0; i--) {
                    if (cluster.LoadedChunks[i] == chunkIdx) {
                        cluster.LoadedChunks[i] = cluster.LoadedChunks[--cluster.LoadedChunksCount];
                        #if FFS_ECS_BURST
                        LifecycleHandle.OnClusterValuesChanged(clusterId);
                        #endif
                        break;
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            private readonly void TryFindNextFreeSegmentForType(ref EntitiesCluster cluster, byte entityType) {
                cluster.FreeSegmentByType[entityType] = EntitiesSegment.Invalid;
                uint emptySegment = uint.MaxValue;
                var count = (int) cluster.ChunksCount;
                for (var i = 0; i < count; i++) {
                    var chunkIdx = cluster.Chunks[i];
                    var baseSegmentIdx = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;
                    ref var heuristic = ref HeuristicChunks[chunkIdx];
                    if (heuristic.FullBlocks.Value == ulong.MaxValue || !EntitiesSegments[baseSegmentIdx].SelfOwner) {
                        continue;
                    }

                    var nonFullBlocks = ~heuristic.FullBlocks.Value;
                    while (nonFullBlocks != 0) {
                        var segmentIdxInChunk = (uint)Utils.Lsb(nonFullBlocks) >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                        var segmentMask = SegmentsMaskCache[segmentIdxInChunk];
                        nonFullBlocks &= ~segmentMask;

                        var segIdx = baseSegmentIdx + segmentIdxInChunk;
                        var type = EntitiesSegments[segIdx].EntityType;
                        if ((heuristic.NotEmptyBlocks.Value & segmentMask) != 0) {
                            if (type == entityType) {
                                cluster.FreeSegmentByType[entityType] = (int) segIdx;
                                return;
                            }
                        } else if (emptySegment == uint.MaxValue && (type == EntitiesSegment.NoEntityType || type == entityType)) {
                            emptySegment = segIdx;
                        }
                    }
                }

                if (emptySegment != uint.MaxValue) {
                    EntitiesSegments[emptySegment].EntityType = entityType;
                    #if FFS_ECS_BURST
                    LifecycleHandle.OnSegmentEntityTypeChanged(emptySegment, entityType);
                    #endif
                    cluster.FreeSegmentByType[entityType] = (int) emptySegment;
                }
            }

            [MethodImpl(AggressiveInlining)]
            private readonly void InvalidateFreeSegmentsForChunk(ref EntitiesCluster cluster, uint chunkIdx) {
                var baseSegIdx = (int)(chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT);
                for (var s = 0; s < Const.SEGMENTS_IN_CHUNK; s++) {
                    var segIdx = baseSegIdx + s;
                    var segType = EntitiesSegments[segIdx].EntityType;
                    if (cluster.FreeSegmentByType[segType] == segIdx) {
                        cluster.FreeSegmentByType[segType] = EntitiesSegment.Invalid;
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            private readonly void RebuildFreeSegmentCacheForCluster(ref EntitiesCluster cluster) {
                Array.Fill(cluster.FreeSegmentByType, EntitiesSegment.Invalid);
                for (var i = 0; i < (int) cluster.ChunksCount; i++) {
                    var chunkIdx = cluster.Chunks[i];
                    var baseSegmentIdx = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;
                    ref var heuristic = ref HeuristicChunks[chunkIdx];
                    if (heuristic.FullBlocks.Value == ulong.MaxValue || !EntitiesSegments[baseSegmentIdx].SelfOwner) {
                        continue;
                    }

                    var nonFullBlocks = ~heuristic.FullBlocks.Value;
                    while (nonFullBlocks != 0) {
                        var segmentIdxInChunk = (uint)Utils.Lsb(nonFullBlocks) >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                        var segmentMask = SegmentsMaskCache[segmentIdxInChunk];
                        nonFullBlocks &= ~segmentMask;

                        var segIdx = baseSegmentIdx + segmentIdxInChunk;
                        var segType = EntitiesSegments[segIdx].EntityType;
                        if (segType != EntitiesSegment.NoEntityType &&
                            cluster.FreeSegmentByType[segType] == EntitiesSegment.Invalid) {
                            cluster.FreeSegmentByType[segType] = (int) segIdx;
                        }
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            private void RemoveFromSelfFreeChunks(uint chunkIdx) {
                var freeChunksCountTemp = _selfFreeChunksCount;
                #if FFS_ECS_DEBUG
                var found = false;
                #endif
                while (freeChunksCountTemp > 0) {
                    var i = --freeChunksCountTemp;
                    var index = _selfFreeChunks[i];
                    if (index == chunkIdx) {
                        _selfFreeChunks[i] = _selfFreeChunks[--_selfFreeChunksCount];
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
                ref var cluster = ref Clusters[clusterId];
                for (uint j = 0; j < cluster.ChunksCount; j++) {
                    var chunkIdx = cluster.Chunks[j];
                    var selfOwner = EntitiesSegments[chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT].SelfOwner;

                    if (strategy == ChunkWritingStrategy.All || (strategy == ChunkWritingStrategy.SelfOwner && selfOwner) || (strategy == ChunkWritingStrategy.OtherOwner && !selfOwner)) {
                        tempChunks.Add(chunkIdx);
                    }
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Write(ref BinaryPackWriter writer, ChunkWritingStrategy strategy, ReadOnlySpan<ushort> clustersToWrite, ref TempChunksData tempChunks, bool fullWorld) {
                clustersToWrite = GetActiveClustersIfEmpty(clustersToWrite);
                
                writer.WriteInt(HeuristicChunks.Length);
                writer.WriteInt(Clusters.Length);
                writer.WriteInt(clustersToWrite.Length);
                
                for (var i = 0; i < clustersToWrite.Length; i++) {
                    var clusterId = clustersToWrite[i];
                    ref var cluster = ref Clusters[clusterId];
                
                    writer.WriteUshort(clusterId);
                    writer.WriteBool(cluster.Disabled);
                    writer.WriteInt(cluster.Chunks.Length);

                    uint count = 0;
                    var offset = writer.MakePoint(sizeof(uint));
                    for (uint j = 0; j < cluster.ChunksCount; j++) {
                        var chunkIdx = cluster.Chunks[j];
                        var selfOwner = EntitiesSegments[chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT].SelfOwner;

                        if (strategy == ChunkWritingStrategy.All || (strategy == ChunkWritingStrategy.SelfOwner && selfOwner) || (strategy == ChunkWritingStrategy.OtherOwner && !selfOwner)) {
                            writer.WriteUint(chunkIdx);
                            WriteChunk(ref writer, chunkIdx, fullWorld);
                            tempChunks.Add(chunkIdx);
                            count++;
                        }
                    }
                    writer.WriteUintAt(offset, count);
                }

                writer.WriteUshort(_poolsCountComponents);
                writer.WriteUint(tempChunks.ChunksCount);
                for (var i = 0; i < tempChunks.ChunksCount; i++) {
                    var chunkIdx = tempChunks.Chunks[i];
                    writer.WriteUint(chunkIdx);
                    WriteDataChunk(ref writer, chunkIdx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void Read(ref BinaryPackReader reader, bool fullWorld, bool hardReset = false) {
                var chunksCapacity = reader.ReadInt();
                var clustersCapacity = reader.ReadInt();
                var clustersCount = reader.ReadInt();

                if (IsWorldInitialized) {
                    if (hardReset) {
                        HardResetInternal();
                    } else {
                        var oldClustersCount = ActiveClustersCount - 1;
                        for (var i = oldClustersCount; i >= 0; i--) {
                            FreeClusterInternal(ActiveClusters[i]);
                        }
                    }
                } else {
                    InitializeInternal((uint) chunksCapacity);
                }

                if (chunksCapacity > HeuristicChunks.Length) {
                    ResizeWorld((uint) chunksCapacity);
                }
                
                if (clustersCapacity > Clusters.Length) {
                    ResizeClusters(clustersCapacity);
                }
                
                for (var i = 0; i < clustersCount; i++) {
                    var clusterId = reader.ReadUshort();
                    var disabled = reader.ReadBool();
                    var chunksClusterCapacity = reader.ReadInt();
                    
                    RegisterClusterInternal(clusterId);
                    
                    ref var cluster = ref Clusters[clusterId];
                    if (chunksClusterCapacity > cluster.Chunks.Length) {
                        Array.Resize(ref cluster.Chunks, chunksClusterCapacity);
                        Array.Resize(ref cluster.LoadedChunks, chunksClusterCapacity);
                        #if FFS_ECS_BURST
                        LifecycleHandle.OnClusterArraysResized(clusterId);
                        #endif
                    }

                    cluster.Disabled = disabled;

                    var count = reader.ReadUint();
                    for (var j = 0; j < count; j++) {
                        var chunkIdx = reader.ReadUint();
                        ReadChunk(ref reader, chunkIdx, fullWorld);

                        cluster.Chunks[cluster.ChunksCount++] = chunkIdx;
                        if (HeuristicLoadedChunks[chunkIdx].Value != 0) {
                            cluster.LoadedChunks[cluster.LoadedChunksCount++] = chunkIdx;
                        }
                    }

                    #if FFS_ECS_BURST
                    LifecycleHandle.OnClusterValuesChanged(clusterId);
                    #endif
                    RebuildFreeSegmentCacheForCluster(ref cluster);
                }

                _selfFreeChunksCount = 0;
                for (uint chunkIdx = 0; chunkIdx < HeuristicChunks.Length; chunkIdx++) {
                    if (EntitiesSegments[chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT].ClusterId == EntitiesSegment.InvalidCluster) {
                        _selfFreeChunks[_selfFreeChunksCount++] = chunkIdx;
                    }
                }

                var componentsPoolsCount = reader.ReadUshort();
                var chunksCount = reader.ReadUint();
                for (var i = 0; i < chunksCount; i++) {
                    var chunkIdx = reader.ReadUint();
                    ReadDataChunk(ref reader, chunkIdx, componentsPoolsCount);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly void WriteChunk(ref BinaryPackWriter writer, uint chunkIdx, bool fullWorld) {
                var heuristic = HeuristicChunks[chunkIdx];
                var baseSegmentIdx = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;

                writer.WriteBool(EntitiesSegments[baseSegmentIdx].SelfOwner);
                writer.WriteUlong(heuristic.NotEmptyBlocks.Value);
                writer.WriteUlong(heuristic.FullBlocks.Value);
                writer.WriteUshort(EntitiesSegments[baseSegmentIdx].ClusterId);

                if (heuristic.NotEmptyBlocks.Value != 0) {
                    for (var s = 0; s < Const.SEGMENTS_IN_CHUNK; s++) {
                        writer.WriteArrayUnmanaged(EntitiesSegments[baseSegmentIdx + s].Masks, 0, Const.BLOCKS_IN_SEGMENT * 2);
                    }
                    if (fullWorld) {
                        writer.WriteUlong(HeuristicLoadedChunks[chunkIdx].Value);
                        for (var s = 0; s < Const.SEGMENTS_IN_CHUNK; s++) {
                            writer.WriteArrayUnmanaged(EntitiesSegments[baseSegmentIdx + s].Masks, Const.BLOCKS_IN_SEGMENT * 2, Const.BLOCKS_IN_SEGMENT);
                        }
                    }
                }

                for (var s = 0; s < Const.SEGMENTS_IN_CHUNK; s++) {
                    writer.WriteArrayUnmanaged(EntitiesSegments[baseSegmentIdx + s].Versions, 0, Const.ENTITIES_IN_SEGMENT);
                }

                for (var s = 0; s < Const.SEGMENTS_IN_CHUNK; s++) {
                    writer.WriteUshort(EntitiesSegments[baseSegmentIdx + s].EntityType);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void ReadChunk(ref BinaryPackReader reader, uint chunkIdx, bool fullWorld) {
                ref var heuristic = ref HeuristicChunks[chunkIdx];
                var baseSegmentIdx = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;

                var selfOwner = reader.ReadBool();
                heuristic.NotEmptyBlocks.Value = reader.ReadUlong();
                heuristic.FullBlocks.Value = reader.ReadUlong();
                var clusterId = reader.ReadUshort();
                SetChunkSegmentsCluster(chunkIdx, clusterId, selfOwner);
                HeuristicLoadedChunks[chunkIdx].Value = 0;

                if (heuristic.NotEmptyBlocks.Value != 0) {
                    for (var s = 0; s < Const.SEGMENTS_IN_CHUNK; s++) {
                        reader.ReadArrayUnmanaged(ref EntitiesSegments[baseSegmentIdx + s].Masks, 0);
                    }
                    if (fullWorld) {
                        HeuristicLoadedChunks[chunkIdx].Value = reader.ReadUlong();
                        for (var s = 0; s < Const.SEGMENTS_IN_CHUNK; s++) {
                            reader.ReadArrayUnmanaged(ref EntitiesSegments[baseSegmentIdx + s].Masks, Const.BLOCKS_IN_SEGMENT * 2);
                        }
                    }
                }

                for (var s = 0; s < Const.SEGMENTS_IN_CHUNK; s++) {
                    reader.ReadArrayUnmanaged(ref EntitiesSegments[baseSegmentIdx + s].Versions);
                }

                for (var s = 0; s < Const.SEGMENTS_IN_CHUNK; s++) {
                    var et = reader.ReadUshort();
                    EntitiesSegments[baseSegmentIdx + s].EntityType = et;
                    #if FFS_ECS_BURST
                    LifecycleHandle.OnSegmentEntityTypeChanged((uint)(baseSegmentIdx + s), et);
                    #endif
                }

                for (var s = 0; s < Const.SEGMENTS_IN_CHUNK; s++) {
                    var segMask = SegmentsMaskCache[s];
                    var segNotEmpty = heuristic.NotEmptyBlocks.Value & segMask;
                    if (segNotEmpty != 0) {
                        var entityType = EntitiesSegments[baseSegmentIdx + s].EntityType;
                        EntityTypes[entityType].HeuristicChunks[chunkIdx].Value |= segNotEmpty;
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly void WriteGuids(ref BinaryPackWriter writer) {
                writer.WriteArrayUnmanaged(_guidsComponents, 0, _poolsCountComponents);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void WriteDataChunk(ref BinaryPackWriter writer, uint chunkIdx) {
               var hasEntities = HasEntitiesInChunk(chunkIdx);
               writer.WriteBool(hasEntities);

               for (var j = 0; j < _poolsCountComponents; j++) {
                   var pool = _poolsComponents[j];
                   var guid = pool.Guid;

                   writer.WriteGuid(guid);
                   writer.WriteUshort(pool.DynamicId);

                   if (hasEntities) {
                       var sizePosition = writer.MakePoint(sizeof(uint));
                       pool.WriteChunk(ref writer, chunkIdx);
                       writer.WriteUintAt(sizePosition, writer.Position - (sizePosition + sizeof(uint)));
                   }
               }

            }

            [MethodImpl(AggressiveInlining)]
            internal void ReadDataChunk(ref BinaryPackReader reader, uint chunkIdx, uint componentsPoolsCount) {
                var tempDeletedPoolIds = ArrayPool<(Guid, uint)>.Shared.Rent((_poolsComponents.Length + _poolsEvents.Length) * 2);
                var tempDeletedPoolIdsCount = 0;
                var hasEntities = reader.ReadBool();

                for (var j = 0; j < componentsPoolsCount; j++) {
                    var guid = reader.ReadGuid();
                    reader.ReadUshort(); // id

                    if (hasEntities) {
                        var byteSize = reader.ReadUint();
                        if (_componentPoolByGuid.TryGetValue(guid, out var pool)) {
                            pool.ReadChunk(ref reader, chunkIdx);
                        }
                        else {
                            tempDeletedPoolIds[tempDeletedPoolIdsCount++] = (guid, reader.Position);
                            reader.SkipNext(byteSize);
                        }
                    }
                }

                if (_migratorByGuid.Count > 0) {
                    for (var i = 0; i < tempDeletedPoolIdsCount; i++) {
                        var (id, offset) = tempDeletedPoolIds[i];
                        if (_migratorByGuid.TryGetValue(id, out var migrator)) {
                            var pReader = reader.AsReader(offset);
                            pReader.DeleteAllComponentMigration(migrator, chunkIdx);
                        }
                    }
                }

                ArrayPool<(Guid, uint)>.Shared.Return(tempDeletedPoolIds);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void WriteEntity(ref BinaryPackWriter writer, Entity entity, bool unload) {
                writer.WriteUlong(entity.GID.Raw);
                writer.WriteByte(entity.EntityType);
                writer.WriteBool(entity.IsDisabled);

                ushort len = 0;
                var point = writer.MakePoint(sizeof(ushort));
                    
                var maskLen = BitMaskComponentsLen;
                var eid = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = eid >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var masks = BitMaskComponents[segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT];
                var start = (segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK) * maskLen;
                for (ushort i = 0; i < maskLen; i++) {
                    var mask = masks[start + i];
                    var offset = i << Const.U64_SHIFT;
                    while (mask > 0) {
                        var id = Utils.PopLsb(ref mask) + offset;
                        var pool = _poolsComponents[id];
                        var pos = writer.Position;
                        writer.WriteUshort((ushort)id);
                        if (pool.WriteEntity(ref writer, entity.ID, false)) {
                            len++;
                        }
                        else {
                            writer.Position = pos;
                        }
                    }
                }
                writer.WriteUshortAt(point, len);

                if (unload) {
                    UnloadEntity(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal readonly void ReadEntity(ref BinaryPackReader reader, Guid[] componentGuidById, ComponentsHandle[] dynamicComponentPoolMap, Entity entity) {
                var len = reader.ReadUshort();
                for (var i = 0; i < len; i++) {
                    var id = reader.ReadUshort();
                    ref readonly var pool = ref dynamicComponentPoolMap[id];
                    if (pool.Guid != default) {
                        pool.ReadEntity(ref reader, entity.ID);
                    }
                    else {
                        if (_migratorByGuid.TryGetValue(componentGuidById[id], out var migration)) {
                            reader.DeleteOneComponentMigration(entity, migration);
                        }
                        else {
                            reader.SkipOneComponent();
                        }
                    }
                }
            }
            #endregion

            #region QUERY
            #if FFS_ECS_DEBUG
            [MethodImpl(AggressiveInlining)]
            internal void SetCurrentQueryEntity(uint entity) {
                if (QueryDataCount > 0) {
                    if (MultiThreadActive) {
                        _currentEntitiesRangeOtherThread ??= new (uint, uint)[Environment.ProcessorCount * Const.MAX_NESTED_QUERY + 1];
                        _currentEntitiesRangeOtherThread[QueryDataCount - 1] = (entity, entity);
                    } else {
                        _currentEntitiesRangeMainThread ??= new (uint, uint)[Const.MAX_NESTED_QUERY + 1];
                        _currentEntitiesRangeMainThread[QueryDataCount - 1] = (entity, entity);
                    }
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void SetCurrentQueryEntity(uint entityStart, uint entityEndIncluded) {
                if (QueryDataCount > 0) {
                    if (MultiThreadActive) {
                        _currentEntitiesRangeOtherThread ??= new (uint, uint)[Environment.ProcessorCount * Const.MAX_NESTED_QUERY + 1];
                        _currentEntitiesRangeOtherThread[QueryDataCount - 1] = (entityStart, entityEndIncluded);
                    }
                    else {
                        _currentEntitiesRangeMainThread ??= new (uint, uint)[Const.MAX_NESTED_QUERY + 1];
                        _currentEntitiesRangeMainThread[QueryDataCount - 1] = (entityStart, entityEndIncluded);
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly bool IsNotCurrentQueryEntity(Entity entity) {
                for (var i = 0; i < QueryDataCount; i++) {
                    if (MultiThreadActive) {
                        if (entity.IdWithOffset < _currentEntitiesRangeOtherThread[i].Item1) return true;
                        if (entity.IdWithOffset > _currentEntitiesRangeOtherThread[i].Item2) return true;
                    } else {
                        if (entity.IdWithOffset < _currentEntitiesRangeMainThread[i].Item1) return true;
                        if (entity.IdWithOffset > _currentEntitiesRangeMainThread[i].Item2) return true;
                    }
                }

                return false;
            }
            #endif
            
            [MethodImpl(AggressiveInlining)]
            internal QueryData PushCurrentQuery() {
                #if FFS_ECS_DEBUG
                if (QueryDataCount == Const.MAX_NESTED_QUERY) throw new StaticEcsException($"The maximum number of nested Query is {Const.MAX_NESTED_QUERY - 1}");
                #endif

                QueryDataCount++;
                var data = new QueryData {
                    Blocks = ArrayPool<BlockMaskCache>.Shared.Rent(HeuristicChunks.Length << Const.BLOCKS_IN_CHUNK_SHIFT),
                };
                return data;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void PopCurrentQuery(QueryData data) {
                #if FFS_ECS_DEBUG
                if (QueryDataCount == 0) throw new StaticEcsException("Unexpected error");
                SetCurrentQueryEntity(default, default);
                #endif
                ArrayPool<BlockMaskCache>.Shared.Return(data.Blocks);
                QueryDataCount--;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void PushQueryDataForDestroy(QueryData qData) {
                _queriesToUpdateOnDestroy[_queriesToUpdateOnDestroyCount++] = qData;
            }

            [MethodImpl(AggressiveInlining)]
            internal void PopQueryDataForDestroy() {
                _queriesToUpdateOnDestroy[--_queriesToUpdateOnDestroyCount] = default;
            }

            [MethodImpl(AggressiveInlining)]
            internal void PushQueryDataForDisable(QueryData qData) {
                _queriesToUpdateOnDisable[_queriesToUpdateOnDisableCount++] = qData;
            }

            internal void PopQueryDataForDisable() {
                _queriesToUpdateOnDisable[--_queriesToUpdateOnDisableCount] = default;
            }

            [MethodImpl(AggressiveInlining)]
            internal void PushQueryDataForEnable(QueryData qData) {
                _queriesToUpdateOnEnable[_queriesToUpdateOnEnableCount++] = qData;
            }

            [MethodImpl(AggressiveInlining)]
            internal void PopQueryDataForEnable() {
                _queriesToUpdateOnEnable[--_queriesToUpdateOnEnableCount] = default;
            }

            #if FFS_ECS_DEBUG
            [MethodImpl(AggressiveInlining)]
            internal void BlockDestroy(int val) {
                _blockerDestroy += val;
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

            #region ENTITY TYPES
            [MethodImpl(AggressiveInlining)]
            internal void RegisterEntityTypeInternal<
                #if NET5_0_OR_GREATER
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
                #endif
                T>(byte id) where T : struct, IEntityType {
                EntityTypeInfo<T>.Id = id;
                EntityTypeInfo<T>.HasOnCreate = EntityTypeType<T>.HasOnCreate();
                EntityTypeInfo<T>.Registered = true;

                ref var data = ref EntityTypes[id];
                data.Registered = true;
                if (EntityTypeType<T>.HasOnDestroy()) {
                    unsafe { data.OnDestroyFn = &EntityTypeOnDestroy<T>; }
                }
                if (EntityTypeType<T>.HasOnCreate()) {
                    unsafe { data.OnCreateFn = &EntityTypeOnCreate<T>; }
                }

                if (EntityTypeResetters == null) {
                    EntityTypeResetters = new Action[8];
                } else if (EntityTypeResettersCount == EntityTypeResetters.Length) {
                    var newArr = new Action[EntityTypeResettersCount << 1];
                    for (var i = 0; i < EntityTypeResettersCount; i++) newArr[i] = EntityTypeResetters[i];
                    EntityTypeResetters = newArr;
                }
                EntityTypeResetters[EntityTypeResettersCount++] = ResetEntityTypeInfo<T>;
            }

            private static void ResetEntityTypeInfo<T>() where T : struct, IEntityType {
                EntityTypeInfo<T>.Id = default;
                EntityTypeInfo<T>.HasOnCreate = default;
                EntityTypeInfo<T>.Registered = default;
            }

            [MethodImpl(AggressiveInlining)]
            private static void EntityTypeOnDestroy<T>(Entity entity, HookReason reason) where T : struct, IEntityType {
                var entityType = default(T);
                entityType.OnDestroy(entity, reason);
            }

            [MethodImpl(AggressiveInlining)]
            private static void EntityTypeOnCreate<T>(Entity entity) where T : struct, IEntityType {
                var entityType = default(T);
                entityType.OnCreate(entity);
            }
            #endregion

            #region COMPONENTS
            [MethodImpl(AggressiveInlining)]
            internal void RegisterComponentTypeInternal<TComponent>(ComponentTypeConfig<TComponent> typeConfig) where TComponent : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertNotRegisteredComponent<TComponent>(WorldTypeName);
                #endif

                typeConfig = typeConfig.MergeWith(AutoRegistration.FindComponentConfig<TComponent>())
                                       .MergeWith(ComponentTypeConfig<TComponent>.DefaultComponent);

                #if FFS_ECS_DEBUG
                AssertGuidNotEmpty<TComponent>(WorldTypeName, typeConfig.Guid.Value);
                #endif

                if (_poolsCountComponents == _poolsComponents.Length) {
                    Array.Resize(ref _poolsComponents, _poolsCountComponents << 1);
                    Array.Resize(ref _guidsComponents, _poolsCountComponents << 1);
                }

                Components<TComponent>.Instance = new Components<TComponent>(_poolsCountComponents, typeConfig, false);
                Components<TComponent>.Handle = ComponentsHandle.Create<TWorld, TComponent>();
                _poolsComponents[_poolsCountComponents] = Components<TComponent>.Handle;
                _guidsComponents[_poolsCountComponents++] = typeConfig.Guid.Value;

                if (typeConfig.TrackAdded.Value || typeConfig.TrackDeleted.Value
                    #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                    || typeConfig.TrackChanged.Value
                    #endif
                    ) {
                    if (_trackingComponentIndicesCount == _trackingComponentIndices.Length) {
                        Array.Resize(ref _trackingComponentIndices, _trackingComponentIndicesCount << 1);
                    }
                    _trackingComponentIndices[_trackingComponentIndicesCount++] = (ushort)(_poolsCountComponents - 1);
                }

                var guid = typeConfig.Guid.Value;
                if (_componentPoolByGuid.ContainsKey(guid)) {
                    throw new StaticEcsException($"Component type {typeof(TComponent)} with guid {guid} already registered");
                }

                _componentPoolByGuid[guid] = Components<TComponent>.Handle;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly void SetMigrator(Guid id, EcsComponentDeleteMigrationReader<TWorld> migrator) {
                _migratorByGuid[id] = migrator;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal readonly bool TryGetComponentPool(in Guid guid, out ComponentsHandle pool) {
                return _componentPoolByGuid.TryGetValue(guid, out pool);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal readonly ReadOnlySpan<ComponentsHandle> GetAllComponentsHandles() {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                return new ReadOnlySpan<ComponentsHandle>(_poolsComponents, 0, _poolsCountComponents);
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly ref ComponentsHandle GetComponentsHandleRef(Type type) {
                for (var i = 0; i < _poolsCountComponents; i++) {
                    if (_poolsComponents[i].ComponentType == type) {
                        return ref _poolsComponents[i];
                    }
                }
                throw new StaticEcsException($"Components type {type} not registered");
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort ComponentsCount(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                ushort count = 0;
                
                var maskLen = BitMaskComponentsLen;
                var eid = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = eid >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var masks = BitMaskComponents[segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT];
                var start = (segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK) * maskLen;
                for (ushort i = 0; i < maskLen; i++) {
                    var mask = masks[start + i];
                    var offset = i << Const.U64_SHIFT;
                    while (mask > 0) {
                        var id = Utils.PopLsb(ref mask) + offset;
                        ref readonly var pool = ref _poolsComponents[id];
                        if (!pool.IsTag && pool.Has(entity.ID)) {
                            count++;
                        }
                    }
                }
                
                return count;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal readonly void GetAllComponents(Entity entity, List<IComponent> result) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                result.Clear();
                
                var maskLen = BitMaskComponentsLen;
                var eid = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = eid >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var masks = BitMaskComponents[segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT];
                var start = (segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK) * maskLen;
                for (ushort i = 0; i < maskLen; i++) {
                    var mask = masks[start + i];
                    var offset = i << Const.U64_SHIFT;
                    while (mask > 0) {
                        var id = Utils.PopLsb(ref mask) + offset;
                        ref readonly var pool = ref _poolsComponents[id];
                        if (!pool.IsTag && pool.TryGetRaw(entity.ID, out var component)) {
                            result.Add((IComponent)component);
                        }
                    }
                }
            }
            #endregion

            #region TAGS
            [MethodImpl(AggressiveInlining)]
            internal void RegisterTagTypeInternal<TTag>(TagTypeConfig<TTag> config) where TTag : struct, ITag {
                #if FFS_ECS_DEBUG
                AssertNotRegisteredComponent<TTag>(WorldTypeName);
                #endif

                var typeConfig = config.AsComponentConfig
                                       .MergeWith(AutoRegistration.FindTagConfig<TTag>().AsComponentConfig)
                                       .MergeWith(ComponentTypeConfig<TTag>.DefaultTag);

                #if FFS_ECS_DEBUG
                AssertGuidNotEmpty<TTag>(WorldTypeName, typeConfig.Guid.Value);
                #endif

                if (_poolsCountComponents == _poolsComponents.Length) {
                    Array.Resize(ref _poolsComponents, _poolsCountComponents << 1);
                    Array.Resize(ref _guidsComponents, _poolsCountComponents << 1);
                }

                Components<TTag>.Instance = new Components<TTag>(_poolsCountComponents, typeConfig, isTag: true);
                Components<TTag>.Handle = ComponentsHandle.Create<TWorld, TTag>();
                _poolsComponents[_poolsCountComponents] = Components<TTag>.Handle;
                _guidsComponents[_poolsCountComponents++] = typeConfig.Guid.Value;

                if (typeConfig.TrackAdded.Value || typeConfig.TrackDeleted.Value) {
                    if (_trackingComponentIndicesCount == _trackingComponentIndices.Length) {
                        Array.Resize(ref _trackingComponentIndices, _trackingComponentIndicesCount << 1);
                    }
                    _trackingComponentIndices[_trackingComponentIndicesCount++] = (ushort)(_poolsCountComponents - 1);
                }

                var guid = typeConfig.Guid.Value;
                if (_componentPoolByGuid.ContainsKey(guid)) {
                    throw new StaticEcsException($"Tag type {typeof(TTag)} with guid {guid} already registered");
                }

                _componentPoolByGuid[guid] = Components<TTag>.Handle;
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort TagsCount(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                ushort count = 0;

                var maskLen = BitMaskComponentsLen;
                var eid = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = eid >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var masks = BitMaskComponents[segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT];
                var start = (segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK) * maskLen;
                for (ushort i = 0; i < maskLen; i++) {
                    var mask = masks[start + i];
                    var offset = i << Const.U64_SHIFT;
                    while (mask > 0) {
                        var id = Utils.PopLsb(ref mask) + offset;
                        ref readonly var pool = ref _poolsComponents[id];
                        if (pool.IsTag && pool.Has(entity.ID)) {
                            count++;
                        }
                    }
                }

                return count;
            }

            [MethodImpl(AggressiveInlining)]
            internal void GetAllTags(Entity entity, List<ITag> result) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                result.Clear();

                var maskLen = BitMaskComponentsLen;
                var eid = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = eid >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var masks = BitMaskComponents[segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT];
                var start = (segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK) * maskLen;
                for (ushort i = 0; i < maskLen; i++) {
                    var mask = masks[start + i];
                    var offset = i << Const.U64_SHIFT;
                    while (mask > 0) {
                        var id = Utils.PopLsb(ref mask) + offset;
                        ref readonly var pool = ref _poolsComponents[id];
                        if (pool.IsTag && pool.TryGetRaw(entity.ID, out var component)) {
                            result.Add((ITag) component);
                        }
                    }
                }
            }
            #endregion

            #region EVENTS
            [MethodImpl(AggressiveInlining)]
            internal void RegisterEventTypeInternal<T>(EventTypeConfig<T> config) where T : struct, IEvent {
                if (Events<T>.Instance.Initialized) throw new StaticEcsException($"Event {typeof(T)} already registered");

                var typeConfig = config.MergeWith(AutoRegistration.FindEventConfig<T>())
                                       .MergeWith(EventTypeConfig<T>.Default);

                #if FFS_ECS_DEBUG
                AssertGuidNotEmpty<T>(WorldTypeName, typeConfig.Guid.Value);
                #endif

                Events<T>.Instance = new Events<T>(_poolsCountEvents, typeConfig);

                if (_poolsCountEvents == _poolsEvents.Length) {
                    Array.Resize(ref _poolsEvents, _poolsCountEvents << 1);
                }

                _poolsEvents[_poolsCountEvents] = EventsHandle.Create<TWorld, T>();

                var guid = typeConfig.Guid.Value;
                if (_poolEventsByGuid.ContainsKey(guid)) {
                    throw new StaticEcsException($"Event type {typeof(T)} with guid {guid} already registered");
                }

                _poolEventsByGuid[guid] = _poolsEvents[_poolsCountEvents];

                _poolsCountEvents++;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal readonly void SetDeleteEventsMigrator(Guid id, EcsEventDeleteMigrationReader migrator) {
                _deleteEventMigratorByGuid[id] = migrator;
            }

            [MethodImpl(AggressiveInlining)]
            internal void WriteEvents(ref BinaryPackWriter writer) {
                ushort len = 0;
                var point = writer.MakePoint(sizeof(ushort));
                for (var i = 0; i < _poolsCountEvents; i++) {
                    var pool = _poolsEvents[i];
                    var guid = pool.Guid;

                    if (_poolEventsByGuid.TryGetValue(guid, out var wrapper)) {
                        writer.WriteGuid(guid);
                        var offset = writer.MakePoint(sizeof(uint));
                        wrapper.WriteAll(ref writer);
                        writer.WriteUintAt(offset, writer.Position - (offset + sizeof(uint)));
                        len++;
                    }
                }

                writer.WriteUshortAt(point, len);
            }

            [MethodImpl(AggressiveInlining)]
            internal void ReadEvents(ref BinaryPackReader reader) {
                ClearEvents();
                var tempDeletedPoolIds = ArrayPool<(Guid, uint)>.Shared.Rent((_poolsComponents.Length + _poolsEvents.Length) * 2);
                var tempDeletedPoolIdsCount = 0;

                var poolsCount = reader.ReadUshort();
                for (var i = 0; i < poolsCount; i++) {
                    var guid = reader.ReadGuid();
                    var byteSize = reader.ReadUint();
                    if (_poolEventsByGuid.TryGetValue(guid, out var pool)) {
                        pool.ReadAll(ref reader);
                    }
                    else {
                        tempDeletedPoolIds[tempDeletedPoolIdsCount++] = (guid, reader.Position);
                        reader.SkipNext(byteSize);
                    }
                }

                if (_deleteEventMigratorByGuid.Count > 0) {
                    for (var i = 0; i < tempDeletedPoolIdsCount; i++) {
                        var (id, offset) = tempDeletedPoolIds[i];
                        if (_deleteEventMigratorByGuid.TryGetValue(id, out var migration)) {
                            var pReader = reader.AsReader(offset);
                            pReader.DeleteAllEventMigration<TWorld>(migration);
                        }
                    }
                }

                ArrayPool<(Guid, uint)>.Shared.Return(tempDeletedPoolIds);
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly ReadOnlySpan<EventsHandle> GetAllEventsHandles() => new(_poolsEvents, 0, _poolsCountEvents);

            [MethodImpl(AggressiveInlining)]
            internal readonly ref EventsHandle GetEventsHandleRef(Type type) {
                for (var i = 0; i < _poolsCountEvents; i++) {
                    if (_poolsEvents[i].EventType == type) {
                        return ref _poolsEvents[i];
                    }
                }
                throw new StaticEcsException($"Events type {type} not registered");
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly void ClearEvents() {
                for (var i = 0; i < _poolsCountEvents; i++) {
                    _poolsEvents[i].Reset();
                }
            }
            #endregion

            [MethodImpl(AggressiveInlining)]
            private readonly void FillEntitiesVersions(ref ushort[] dst) {
                dst ??= new ushort[Const.ENTITIES_IN_SEGMENT];

                unsafe {
                    const int size = Const.ENTITIES_IN_SEGMENT * sizeof(ushort);
                    fixed (void* dstPtr = &dst[0]) {
                        fixed (void* srcPtr = &_versionEntitiesTemplate[0]) {
                            Buffer.MemoryCopy(srcPtr, dstPtr, size, size);
                        }
                    }
                }
            }
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
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
                    Chunks = ArrayPool<uint>.Shared.Rent(Data.Instance.HeuristicChunks.Length),
                    ChunksCount = 0
                };
            }

            [MethodImpl(AggressiveInlining)]
            internal void Dispose() {
                ArrayPool<uint>.Shared.Return(Chunks);
            }
        }
        
        #region HANDLE
        #region BASE
        [MethodImpl(AggressiveInlining)]
        internal static GIDStatus _GidStatus(EntityGID gid) => Data.Instance.GIDStatus(gid);

        [MethodImpl(AggressiveInlining)]
        internal static EntityGID _NewEntity(byte entityType, ushort clusterId) {
            Data.Instance.CreateEntityWithOnCreate(entityType, clusterId, out var entity);
            return entity;
        }

        [MethodImpl(AggressiveInlining)]
        internal static uint _CalculateEntitiesCount() => CalculateEntitiesCount();

        [MethodImpl(AggressiveInlining)]
        internal static uint _CalculateEntitiesCapacity() => CalculateEntitiesCapacity();

        [MethodImpl(AggressiveInlining)]
        internal static bool _DestroyEntity(EntityGID gid) {
            if (gid.TryUnpack<TWorld>(out var entity)) {
                return Data.Instance.DestroyEntity(entity);
            }
            return false;
        }

        [MethodImpl(AggressiveInlining)]
        internal static void _DestroyAllEntities() => DestroyAllLoadedEntities();

        [MethodImpl(AggressiveInlining)]
        internal static void _DestroyAllEntitiesInCluster(ushort clusterId) => DestroyAllEntitiesInCluster(clusterId);

        [MethodImpl(AggressiveInlining)]
        internal static void _DestroyAllEntitiesInChunk(uint chunkId) => DestroyAllEntitiesInChunk(chunkId);

        [MethodImpl(AggressiveInlining)]
        internal static WorldStatus _WorldStatus() => Data.Instance.WorldStatus;

        [MethodImpl(AggressiveInlining)]
        internal static bool _IsEntityTypeRegistered(byte id) => IsEntityTypeRegistered(id);

        [MethodImpl(AggressiveInlining)]
        internal static uint _CalculateEntitiesCountByType(byte entityType) => Data.Instance.CalculateEntitiesCountByTypeInternal(entityType);

        [MethodImpl(AggressiveInlining)]
        internal static uint _CalculateEntitiesCapacityByType(byte entityType) => Data.Instance.CalculateEntitiesCapacityByTypeInternal(entityType);
        #endregion

        #region COMPONENTS
        [MethodImpl(AggressiveInlining)]
        internal static bool _TryGetComponentsHandle(Type type, out ComponentsHandle pool) {
            var pools = Data.Instance.GetAllComponentsHandles();
            for (var i = 0; i < pools.Length; i++) {
                ref readonly var p = ref pools[i];
                if (p.ComponentType == type) {
                    pool = p;
                    return true;
                }
            }

            pool = default;
            return false;
        }

        [MethodImpl(AggressiveInlining)]
        internal static ref ComponentsHandle _GetComponentsHandle(Type type) {
            return ref Data.Instance.GetComponentsHandleRef(type);
        }

        [MethodImpl(AggressiveInlining)]
        internal static ReadOnlySpan<ComponentsHandle> _GetAllComponentsHandles() => Data.Instance.GetAllComponentsHandles();
        #endregion


        #region EVENTS
        [MethodImpl(AggressiveInlining)]
        internal static bool _TryGetEventsHandle(Type type, out EventsHandle pool) {
            var pools = Data.Instance.GetAllEventsHandles();
            for (var i = 0; i < pools.Length; i++) {
                ref readonly var p = ref pools[i];
                if (p.EventType == type) {
                    pool = p;
                    return true;
                }
            }

            pool = default;
            return false;
        }

        [MethodImpl(AggressiveInlining)]
        internal static ref EventsHandle _GetEventsHandle(Type type) {
            return ref Data.Instance.GetEventsHandleRef(type);
        }

        [MethodImpl(AggressiveInlining)]
        internal static ReadOnlySpan<EventsHandle> _GetAllEventsHandles() => Data.Instance.GetAllEventsHandles();
        #endregion

        #region RESOURCES
        [MethodImpl(AggressiveInlining)]
        internal static bool _HasResource(Type type) => ResourcesData.Instance.HasRaw(type);

        [MethodImpl(AggressiveInlining)]
        internal static bool _HasResourceByKey(string key) => NamedResources.Has(key);

        [MethodImpl(AggressiveInlining)]
        internal static object _GetResource(Type type) => ResourcesData.Instance.GetRaw(type);

        [MethodImpl(AggressiveInlining)]
        internal static object _GetResourceByKey(string key) => NamedResources.Get<object>(key);

        [MethodImpl(AggressiveInlining)]
        internal static void _RemoveResource(Type type) => ResourcesData.Instance.RemoveRaw(type);

        [MethodImpl(AggressiveInlining)]
        internal static void _RemoveResourceByKey(string key) => NamedResources.Remove(key);

        [MethodImpl(AggressiveInlining)]
        internal static void _SetResource(Type type, object value, bool clearOnDestroy) => ResourcesData.Instance.GetAllGetSetRemoveValuesMethods()[type].Item2(value, clearOnDestroy);

        [MethodImpl(AggressiveInlining)]
        internal static void _SetResourceByKey(string key, object value, bool clearOnDestroy) => NamedResources.Set(key, value, clearOnDestroy);

        [MethodImpl(AggressiveInlining)]
        internal static IReadOnlyCollection<string> _GetAllResourcesKeys() => NamedResources.Values.Keys;

        [MethodImpl(AggressiveInlining)]
        internal static IReadOnlyCollection<Type> _GetAllResourcesTypes() => ResourcesData.Instance.GetAllGetSetRemoveValuesMethods().Keys;
        #endregion
        #endregion

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        internal struct EntityTypeData {
            internal AtomicMask[] HeuristicChunks;
            internal unsafe delegate*<Entity, HookReason, void> OnDestroyFn;
            internal unsafe delegate*<Entity, void> OnCreateFn;
            internal bool Registered;
        }
    }

    /// <summary>
    /// Controls which chunks are included when writing a snapshot.
    /// </summary>
    public enum ChunkWritingStrategy : byte {
        /// <summary>Write all registered chunks regardless of ownership.</summary>
        All,
        /// <summary>Write only chunks owned by this world instance (<see cref="ChunkOwnerType.Self"/>).</summary>
        SelfOwner,
        /// <summary>Write only externally-owned chunks (<see cref="ChunkOwnerType.Other"/>).</summary>
        OtherOwner
    }

    public enum HookReason : byte {
        Default = 0,
        UnloadEntity = 1,
        WorldDestroy = 2
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    internal static class EntityTypeType<
        #if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
        #endif
        T> where T : struct, IEntityType {
        internal static bool HasOnCreate() {
            return HasMethod(typeof(T), nameof(IEntityType.OnCreate));
        }

        internal static bool HasOnDestroy() {
            return HasMethod(typeof(T), nameof(IEntityType.OnDestroy));
        }

        private static bool HasMethod(
            #if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
            #endif
            Type structType, string methodName) {
            var methods = structType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var methodInfo in methods) {
                if (methodInfo.Name == methodName && methodInfo.IsGenericMethodDefinition) {
                    return true;
                }
            }
            return false;
        }
    }

    public unsafe struct WorldLifecycleHandle {
        // ReSharper disable InconsistentNaming
        public delegate*<uint, void> _OnInitialize;                            // segmentsCapacity
        public delegate*<uint, int, void> _OnResize;                           // newSegmentsCapacity, oldSegmentsCapacity
        public delegate*<void> _OnDestroy;
        public delegate*<int, void> _OnClusterRegistered;                      // clusterId
        public delegate*<uint, int, bool, void> _OnSetChunkSegmentsCluster;    // chunkIdx, clusterId, selfOwner
        public delegate*<uint, byte, void> _OnSegmentEntityTypeChanged;        // segmentIdx, entityType
        public delegate*<int, void> _OnClusterValuesChanged;                   // clusterId
        public delegate*<int, void> _OnClusterArraysResized;                   // clusterId
        public delegate*<void> _OnActiveClustersChanged;
        // ReSharper restore InconsistentNaming

        [MethodImpl(AggressiveInlining)]
        public readonly void OnInitialize(uint segmentsCapacity) {
            if (_OnInitialize != null) _OnInitialize(segmentsCapacity);
        }

        [MethodImpl(AggressiveInlining)]
        public readonly void OnResize(uint newSegmentsCapacity, int oldSegmentsCapacity) {
            if (_OnResize != null) _OnResize(newSegmentsCapacity, oldSegmentsCapacity);
        }

        [MethodImpl(AggressiveInlining)]
        public readonly void OnDestroy() {
            if (_OnDestroy != null) _OnDestroy();
        }

        [MethodImpl(AggressiveInlining)]
        public readonly void OnClusterRegistered(int clusterId) {
            if (_OnClusterRegistered != null) _OnClusterRegistered(clusterId);
        }

        [MethodImpl(AggressiveInlining)]
        public readonly void OnSetChunkSegmentsCluster(uint chunkIdx, int clusterId, bool selfOwner) {
            if (_OnSetChunkSegmentsCluster != null) _OnSetChunkSegmentsCluster(chunkIdx, clusterId, selfOwner);
        }

        [MethodImpl(AggressiveInlining)]
        public readonly void OnSegmentEntityTypeChanged(uint segmentIdx, byte entityType) {
            if (_OnSegmentEntityTypeChanged != null) _OnSegmentEntityTypeChanged(segmentIdx, entityType);
        }

        [MethodImpl(AggressiveInlining)]
        public readonly void OnClusterValuesChanged(int clusterId) {
            if (_OnClusterValuesChanged != null) _OnClusterValuesChanged(clusterId);
        }

        [MethodImpl(AggressiveInlining)]
        public readonly void OnClusterArraysResized(int clusterId) {
            if (_OnClusterArraysResized != null) _OnClusterArraysResized(clusterId);
        }

        [MethodImpl(AggressiveInlining)]
        public readonly void OnActiveClustersChanged() {
            if (_OnActiveClustersChanged != null) _OnActiveClustersChanged();
        }
    }
}