#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

[assembly: InternalsVisibleTo("Test")]
[assembly: InternalsVisibleTo("FFS.StaticEcs.Unity")]
[assembly: InternalsVisibleTo("FFS.StaticEcs.Unity.Editor")]


namespace FFS.Libraries.StaticEcs {
    
    internal class StaticEcsException : InvalidOperationException {
        internal StaticEcsException() { }

        internal StaticEcsException(string message) : base(message) { }

        internal StaticEcsException(string message, Exception inner) : base(message, inner) { }
        
        internal StaticEcsException(string source, string method, string message) : base($"{source}, method `{method}`: {message}") { }
        
        internal StaticEcsException(string source, string message) : base($"{source}: {message}") { }
        
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public unsafe struct Block<T> where T : unmanaged {
        #if FFS_ECS_BURST
        [Unity.Collections.LowLevel.Unsafe.NativeDisableUnsafePtrRestriction]
        #endif
        public T* Ptr;
        #if FFS_ECS_DEBUG
        public uint Count;
        #endif
    
        [MethodImpl(AggressiveInlining)]
        public Block(T* ptr) {
            Ptr = ptr;
            #if FFS_ECS_DEBUG
            Count = 0;
            #endif
        }
    
        public readonly ref T this[uint idx] {
            [MethodImpl(AggressiveInlining)]
            get {
                #if FFS_ECS_DEBUG
                if (idx >= Count) throw new IndexOutOfRangeException();
                #endif
                return ref Ptr[idx];
            }
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public unsafe struct BlockR<T> where T : unmanaged {
        #if FFS_ECS_BURST
        [Unity.Collections.LowLevel.Unsafe.NativeDisableUnsafePtrRestriction]
        #endif
        public readonly T* Ptr;
        #if FFS_ECS_DEBUG
        public uint Count;
        #endif

        [MethodImpl(AggressiveInlining)]
        public BlockR(T* ptr) {
            Ptr = ptr;
            #if FFS_ECS_DEBUG
            Count = 0;
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public BlockR(Block<T> block) {
            Ptr = block.Ptr;
            #if FFS_ECS_DEBUG
            Count = block.Count;
            #endif
        }

        public ref readonly T this[uint idx] {
            [MethodImpl(AggressiveInlining)]
            get {
                #if FFS_ECS_DEBUG
                if (idx >= Count) throw new IndexOutOfRangeException();
                #endif
                return ref Ptr[idx];
            }
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    [StructLayout(LayoutKind.Explicit)]
    public struct AtomicMask {
        [FieldOffset(0)] internal ulong Value;
        [FieldOffset(0)] private long _value;

        [MethodImpl(AggressiveInlining)]
        internal void SetBit(byte index) {
            var mask = (long) (1UL << index);
            #if NET6_0_OR_GREATER
            Interlocked.Or(ref _value, mask);
            #else
            long orig, newVal;
            do {
                orig = _value;
                newVal = orig | mask;
            } while (Interlocked.CompareExchange(ref _value, newVal, orig) != orig);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        internal ulong ClearBit(byte index) {
            var mask = (long) ~(1UL << index);
            #if NET6_0_OR_GREATER
            return (ulong) (Interlocked.And(ref _value, mask) & mask);
            #else
            long orig, newVal;
            do {
                orig = _value;
                newVal = orig & mask;
            } while (Interlocked.CompareExchange(ref _value, newVal, orig) != orig);
            return (ulong) newVal;
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        internal void ClearBits(ulong invertedMask) {
            #if NET6_0_OR_GREATER
            Interlocked.And(ref _value, (long) invertedMask);
            #else
            long orig, newVal;
            do {
                orig = _value;
                newVal = orig & (long) invertedMask;
            } while (Interlocked.CompareExchange(ref _value, newVal, orig) != orig);
            #endif
        }
    }

    /// <summary>
    /// Compile-time constants defining the entity storage hierarchy layout.
    /// <para>
    /// The storage model is a three-level hierarchy: <b>Chunk</b> (4096 entities) →
    /// <b>Segment</b> (256 entities) → <b>Block</b> (64 entities, one <c>ulong</c> bitmask).
    /// All sizes are powers of two, enabling branchless index arithmetic via masks and shifts.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    [Il2CppEagerStaticClassConstruction]
    #endif
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Const {
        #if FFS_ECS_DEBUG
        static Const() {
            if ((1 << ENTITIES_IN_CHUNK_SHIFT) != ENTITIES_IN_CHUNK) throw new StaticEcsException("Const: ENTITIES_IN_CHUNK_SHIFT mismatch");
            if ((1 << ENTITIES_IN_SEGMENT_SHIFT) != ENTITIES_IN_SEGMENT) throw new StaticEcsException("Const: ENTITIES_IN_SEGMENT_SHIFT mismatch");
            if ((1 << ENTITIES_IN_BLOCK_SHIFT) != ENTITIES_IN_BLOCK) throw new StaticEcsException("Const: ENTITIES_IN_BLOCK_SHIFT mismatch");
            if ((1 << SEGMENTS_IN_CHUNK_SHIFT) != SEGMENTS_IN_CHUNK) throw new StaticEcsException("Const: SEGMENTS_IN_CHUNK_SHIFT mismatch");
            if ((1 << BLOCKS_IN_CHUNK_SHIFT) != BLOCKS_IN_CHUNK) throw new StaticEcsException("Const: BLOCKS_IN_CHUNK_SHIFT mismatch");
            if ((1 << BLOCKS_IN_SEGMENT_SHIFT) != BLOCKS_IN_SEGMENT) throw new StaticEcsException("Const: BLOCKS_IN_SEGMENT_SHIFT mismatch");
        }
        #endif
        #if ENABLE_IL2CPP
        #if ENABLE_IL2CPP_CHECKS || FFS_ECS_DEBUG
        /// <summary>Whether IL2CPP null checks are enabled. <c>true</c> when <c>ENABLE_IL2CPP_CHECKS</c> or <c>FFS_ECS_DEBUG</c> is defined.</summary>
        public const bool IL2CPPNullChecks = true;
        /// <summary>Whether IL2CPP array bounds checks are enabled. <c>true</c> when <c>ENABLE_IL2CPP_CHECKS</c> or <c>FFS_ECS_DEBUG</c> is defined.</summary>
        public const bool IL2CPPArrayBoundsChecks = true;
        #else
        /// <summary>Whether IL2CPP null checks are enabled. <c>false</c> in release IL2CPP builds for maximum performance.</summary>
        public const bool IL2CPPNullChecks = false;
        /// <summary>Whether IL2CPP array bounds checks are enabled. <c>false</c> in release IL2CPP builds for maximum performance.</summary>
        public const bool IL2CPPArrayBoundsChecks = false;
        #endif
        #endif

        /// <summary>
        /// Offset added to raw entity slot indices so that a default-initialized (zeroed)
        /// <see cref="World{TWorld}.Entity"/> is never a valid handle. Value: 1.
        /// </summary>
        public const int ENTITY_ID_OFFSET = 1;

        /// <summary>Number of blocks batched into a single parallel query job unit.</summary>
        public const int JOB_SIZE = 4;
        /// <summary>Maximum depth of nested query iterations allowed simultaneously.</summary>
        public const int MAX_NESTED_QUERY = 4;
        /// <summary>Maximum number of QueryData entries per component/tag type (accounts for multiple Any filters per query).</summary>
        public const int MAX_QUERY_DATA_PER_TYPE = MAX_NESTED_QUERY * 8;

        public const int U64_BITS = 64;
        public const int U64_SHIFT = 6;
        public const int U64_MASK = U64_BITS - 1;

        /// <summary>Number of entity slots per chunk (4096). Top level of the storage hierarchy.</summary>
        public const int ENTITIES_IN_CHUNK = 4096;
        /// <summary>Number of entity slots per segment (256). Middle level of the storage hierarchy.</summary>
        public const int ENTITIES_IN_SEGMENT = 256;
        /// <summary>Number of entity slots per block (64). Bottom level — one <c>ulong</c> bitmask.</summary>
        public const int ENTITIES_IN_BLOCK = U64_BITS;
        /// <summary>Number of segments per chunk (16).</summary>
        public const int SEGMENTS_IN_CHUNK = ENTITIES_IN_CHUNK / ENTITIES_IN_SEGMENT;
        /// <summary>Number of blocks per chunk (64).</summary>
        public const int BLOCKS_IN_CHUNK = ENTITIES_IN_CHUNK / ENTITIES_IN_BLOCK;
        /// <summary>Number of blocks per segment (4).</summary>
        public const int BLOCKS_IN_SEGMENT = ENTITIES_IN_SEGMENT / ENTITIES_IN_BLOCK;

        /// <summary>Bitmask for fast modulo by <see cref="ENTITIES_IN_CHUNK"/> (4095).</summary>
        public const int ENTITIES_IN_CHUNK_MASK = ENTITIES_IN_CHUNK - 1;
        /// <summary>Bitmask for fast modulo by <see cref="ENTITIES_IN_SEGMENT"/> (255).</summary>
        public const int ENTITIES_IN_SEGMENT_MASK = ENTITIES_IN_SEGMENT - 1;
        /// <summary>Bitmask for fast modulo by <see cref="ENTITIES_IN_BLOCK"/> (63).</summary>
        public const int ENTITIES_IN_BLOCK_MASK = ENTITIES_IN_BLOCK - 1;
        /// <summary>Bitmask for fast modulo by <see cref="SEGMENTS_IN_CHUNK"/> (15).</summary>
        public const int SEGMENTS_IN_CHUNK_MASK = SEGMENTS_IN_CHUNK - 1;
        /// <summary>Bitmask for fast modulo by <see cref="BLOCKS_IN_CHUNK"/> (63).</summary>
        public const int BLOCKS_IN_CHUNK_MASK = BLOCKS_IN_CHUNK - 1;
        /// <summary>Bitmask for fast modulo by <see cref="BLOCKS_IN_SEGMENT"/> (3).</summary>
        public const int BLOCKS_IN_SEGMENT_MASK = BLOCKS_IN_SEGMENT - 1;

        /// <summary>Log₂ of <see cref="ENTITIES_IN_CHUNK"/> (12). Bit-shift for chunk index extraction.</summary>
        public const int ENTITIES_IN_CHUNK_SHIFT = 12;
        /// <summary>Log₂ of <see cref="ENTITIES_IN_SEGMENT"/> (8). Bit-shift for segment index extraction.</summary>
        public const int ENTITIES_IN_SEGMENT_SHIFT = 8;
        /// <summary>Log₂ of <see cref="ENTITIES_IN_BLOCK"/> (6). Bit-shift for block index extraction.</summary>
        public const int ENTITIES_IN_BLOCK_SHIFT = 6;
        /// <summary>Log₂ of <see cref="SEGMENTS_IN_CHUNK"/> (4). Bit-shift for segment-in-chunk index.</summary>
        public const int SEGMENTS_IN_CHUNK_SHIFT = 4;
        /// <summary>Log₂ of <see cref="BLOCKS_IN_CHUNK"/> (6). Bit-shift for block-in-chunk index.</summary>
        public const int BLOCKS_IN_CHUNK_SHIFT = 6;
        /// <summary>Log₂ of <see cref="BLOCKS_IN_SEGMENT"/> (2). Bit-shift for block-in-segment index.</summary>
        public const int BLOCKS_IN_SEGMENT_SHIFT = 2;
        
        /*
         
         Layout:
         ================================================================================================================================================================
         Chunk   | 4096 | [][][][] [][][][] [][][][] [][][][] [][][][] [][][][] [][][][] [][][][] [][][][] [][][][] [][][][] [][][][] [][][][] [][][][] [][][][] [][][][]
         Segment | 256  | [][][][]
         Block   | 64   | []
        
         Mapping:
         ================================================================================================================================================================
         FIELD                    |   LOGIC                                                  |  FORMULA
         ================================================================================================================================================================
         chunkIdx                 |   Entity ID -> Chunk Index                               |  entityId >> Const.ENTITIES_IN_CHUNK_SHIFT
         globalBlockIdx           |   Entity ID -> Global Block Index                        |  entityId >> Const.ENTITIES_IN_BLOCK_SHIFT
         chunkBlockIdx            |   Entity ID -> Chunk Block Index                         |  (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK)
         segmentIdx               |   Entity ID -> Segment Index                             |  entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT
         segmentBlockIdx          |   Entity ID -> Segment Block Index                       |  (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK)
         segmentEntityIdx         |   Entity ID -> Segment Entity Index                      |  (byte) (entityId & Const.ENTITIES_IN_SEGMENT_MASK)
         blockEntityIdx           |   Entity ID -> Block Entity Index                        |  entityId & Const.ENTITIES_IN_BLOCK_MASK
         blockEntityMask          |   Entity ID -> Block Mask Entity                         |  1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK)
                                  |               
         baseChunkEntityIdx       |   Chunk Index -> Base Chunk Entity Index                 |  chunkIdx << Const.ENTITIES_IN_CHUNK_SHIFT
         baseGlobalBlockIdx       |   Chunk Index -> Base Global Block Index                 |  chunkIdx << Const.BLOCKS_IN_CHUNK_SHIFT
         baseSegmentIdx           |   Chunk Index -> Base Segment Index                      |  chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT
                                  |               
         baseSegmentEntityIdx     |   Segment Index -> Base Segment Entity Index             |  segmentIdx << Const.ENTITIES_IN_SEGMENT_SHIFT
         baseGlobalBlockIdx       |   Segment Index -> Base Chunk Global Block Index         |  segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT             
         chunkIdx                 |   Segment Index -> Chunk Index                           |  segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT
                                  |   
         baseGlobalBlockEntityIdx |   Global Block Index -> Base Global Entity Index         |  globalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT
         baseSegmentEntityIdx     |   Segment Block Index -> Base Segment Entity Index       |  segmentBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT
         chunkBlockIdx            |   Global Block Index -> Chunk Block Index                |  globalBlockIdx & Const.BLOCKS_IN_CHUNK_MASK
         chunkIdx                 |   Global Block Index -> Chunk Index                      |  globalBlockIdx >> Const.BLOCKS_IN_CHUNK_SHIFT
         segmentIdx               |   Global Block Index -> Segment Index                    |  globalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT
         segmentBlockIdx          |   Global Block Index -> Segment Block Index              |  (byte) (globalBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK)
         
        */
        
        
        internal static readonly ulong[] DataMasks = CreateDataMasks();

        internal static ulong[] CreateDataMasks() {
            var masks = new ulong[ENTITIES_IN_CHUNK / ENTITIES_IN_SEGMENT];
            const int range = BLOCKS_IN_SEGMENT;
            const ulong baseMask = (1UL << range) - 1;
            for (var i = 0; i < masks.Length; i++) {
                masks[i] = baseMask << (i * range);
            }

            return masks;
        }

    }
    
    #if FFS_ECS_DEBUG
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public abstract partial class World<TWorld> {

        internal static readonly string WorldTypeName = $"World<{typeof(TWorld)}>"; 
        internal static readonly string EntityTypeName = $"World<{typeof(TWorld)}>.Entity"; 

        internal static void Assert(string type, bool state, string message, [CallerMemberName] string method = "") {
            if (!state) {
                throw new StaticEcsException(type, method, message);
            }
        }
        
        internal static void AssertRegisteredEvent<T>(string type, [CallerMemberName] string method = "") where T : struct, IEvent {
            if (!Events<T>.Instance.Initialized) {
                throw new StaticEcsException(type, method, $"Event {typeof(T).GenericName()} is not registered.");
            }
        }
        
        internal static void AssertWorldIsInitialized(string type, [CallerMemberName] string method = "") {
            if (Data.Instance.WorldStatus != WorldStatus.Initialized) {
                throw new StaticEcsException(type, method, "World not initialized.");
            }
        }
        
        internal static void AssertWorldIsIndependent(string type, [CallerMemberName] string method = "") {
            if (!IsIndependent) {
                throw new StaticEcsException(type, method, "World not Independent.");
            }
        }
        
        internal static void AssertWorldIsDependent(string type, [CallerMemberName] string method = "") {
            if (IsIndependent) {
                throw new StaticEcsException(type, method, "World not Dependent.");
            }
        }
        
        internal static void AssertWorldIsCreatedOrInitialized(string type, [CallerMemberName] string method = "") {
            if (Data.Instance.WorldStatus != WorldStatus.Created && Data.Instance.WorldStatus != WorldStatus.Initialized) {
                throw new StaticEcsException(type, method, "World not created or initialized.");
            }
        }
        
        internal static void AssertWorldIsCreated(string type, [CallerMemberName] string method = "") {
            if (Data.Instance.WorldStatus != WorldStatus.Created) {
                throw new StaticEcsException(type, method, "World not created.");
            }
        }
        
        internal static void AssertWorldIsNotCreated(string type, [CallerMemberName] string method = "") {
            if (Data.Instance.WorldStatus != WorldStatus.NotCreated) {
                throw new StaticEcsException(type, method, "World already created.");
            }
        }
        
        internal static void AssertNotRegisteredComponent<T>(string type, [CallerMemberName] string method = "") where T : struct, IComponentOrTag {
            if (Components<T>.Instance.IsRegistered) {
                throw new StaticEcsException(type, method, $"Component {typeof(T).GenericName()} already registered.");
            }
        }
        
        internal static void AssertIsTag<T>(string type, [CallerMemberName] string method = "") where T : struct, IComponentOrTag {
            if (!Components<T>.Instance.IsTag) {
                throw new StaticEcsException(type, method, $"Type {typeof(T).GenericName()} is not a tag.");
            }
        }

        internal static void AssertIsNotTag<T>(string type, [CallerMemberName] string method = "") where T : struct, IComponentOrTag {
            if (Components<T>.Instance.IsTag) {
                throw new StaticEcsException(type, method, $"Type {typeof(T).GenericName()} is a tag, not a component.");
            }
        }

        internal static void AssertRegisteredComponent<T>(string type, [CallerMemberName] string method = "") where T : struct, IComponentOrTag {
            if (!Components<T>.Instance.IsRegistered) {
                throw new StaticEcsException(type, method, $"Component {typeof(T).GenericName()} is not registered.");
            }
        }

        internal static void AssertComponentTrackAdded<T>(string type, [CallerMemberName] string method = "") where T : struct, IComponentOrTag {
            if (!Components<T>.Instance.TrackAdded) {
                throw new StaticEcsException(type, method, $"Component {typeof(T).GenericName()} does not have TrackAdded enabled.");
            }
        }

        internal static void AssertComponentTrackDeleted<T>(string type, [CallerMemberName] string method = "") where T : struct, IComponentOrTag {
            if (!Components<T>.Instance.TrackDeleted) {
                throw new StaticEcsException(type, method, $"Component {typeof(T).GenericName()} does not have TrackDeleted enabled.");
            }
        }

        #if !FFS_ECS_DISABLE_CHANGED_TRACKING
        internal static void AssertComponentTrackChanged<T>(string type, [CallerMemberName] string method = "") where T : struct, IComponentOrTag {
            if (!Components<T>.Instance.TrackChanged) {
                throw new StaticEcsException(type, method, $"Component {typeof(T).GenericName()} does not have TrackChanged enabled.");
            }
        }
        #endif

        internal static void AssertTrackCreated([CallerMemberName] string method = "") {
            if (!Data.Instance.TrackCreated) {
                throw new StaticEcsException(WorldTypeName, method, "WorldConfig.TrackCreated is not enabled.");
            }
        }

        internal static void AssertNotRegisteredEvent<T>(string type, [CallerMemberName] string method = "") where T : struct, IEvent {
            if (Events<T>.Instance.Initialized) {
                throw new StaticEcsException(type, method, $"Event {typeof(T).GenericName()} already registered.");
            }
        }

        internal static void AssertEntityIsNotDestroyedAndLoaded(string type, Entity entity, [CallerMemberName] string method = "") {
            if (entity.IsDestroyed) {
                throw new StaticEcsException(type, method, $"Cannot access destroyed {entity}.");
            }
            if (!Data.Instance.EntityIsLoaded(entity)) {
                throw new StaticEcsException(type, method, $"Cannot access not loaded {entity}.");
            }
        }
        
        internal static void AssertEntityIsLoaded(string type, Entity entity, [CallerMemberName] string method = "") {
            if (!Data.Instance.EntityIsLoaded(entity)) {
                throw new StaticEcsException(type, method, $"Cannot access not loaded {entity}.");
            }
        }
        
        internal static void AssertEntityIsNotLoaded(string type, Entity entity, [CallerMemberName] string method = "") {
            if (Data.Instance.EntityIsLoaded(entity)) {
                throw new StaticEcsException(type, method, $"Cannot access loaded {entity}.");
            }
        }
        
        internal static void AssertEntityHasComponent<T>(string type, Entity entity, [CallerMemberName] string method = "") where T : struct, IComponentOrTag {
            if (!Components<T>.Instance.Has(entity)) {
                throw new StaticEcsException(type, method, $"Component `{typeof(T).GenericName()}` is missing on {entity}.");
            }
        }

        internal static void AssertEntityNotHasComponent<T>(string type, Entity entity, [CallerMemberName] string method = "") where T : struct, IComponentOrTag {
            if (Components<T>.Instance.Has(entity)) {
                throw new StaticEcsException(type, method, $"Component `{typeof(T).GenericName()}` already exists on {entity}.");
            }
        }
        
        internal static void AssertNotBlockedByQuery(string type, Entity entity, int blocker, [CallerMemberName] string method = "") {
            if (blocker > 0 && Data.Instance.IsNotCurrentQueryEntity(entity)) {
                throw new StaticEcsException(type, method, $"Operation is blocked, it is forbidden to modify a non-current {entity} in a {nameof(QueryMode)}{QueryMode.Strict} query, use {nameof(QueryMode)}{QueryMode.Flexible}.");
            }
        }
        
        internal static void AssertNotBlockedByQuery(string type, int blocker, [CallerMemberName] string method = "") {
            if (blocker > 0) {
                throw new StaticEcsException(type, method, $"Operation is blocked, it is forbidden to modify a non-current entity in a {nameof(QueryMode)}{QueryMode.Strict} query, use {nameof(QueryMode)}{QueryMode.Flexible}.");
            }
        }
        
        internal static void AssertNotBlockedByParallelQuery(string type, Entity entity, [CallerMemberName] string method = "") {
            if (Data.Instance.MultiThreadActive && Data.Instance.IsNotCurrentQueryEntity(entity)) {
                throw new StaticEcsException(type, method, $"Operation is blocked, it is forbidden to modify a non-current {entity} in a parallel query.");
            }
        }
        
        internal static void AssertNotNestedParallelQuery(string type, [CallerMemberName] string method = "") {
            if (Data.Instance.MultiThreadActive) {
                throw new StaticEcsException(type, method, "Nested query are not available with parallel query");
            }
        }
        
        internal static void AssertNotMoreThanOneParallelQuery(string type, [CallerMemberName] string method = "") {
            if (Data.Instance.QueryDataCount != 0) {
                throw new StaticEcsException(type, method, "Nested query are not available with parallel query");
            }
        }
        
        internal static void AssertParallelAvailable(string type, [CallerMemberName] string method = "") {
            if (Data.Instance.ZeroThreadCount) {
                throw new StaticEcsException(type, method, "ThreadCount = 0, change World config");
            }
        }
        
        internal static void AssertMultiThreadNotActive(string type, [CallerMemberName] string method = "") {
            if (Data.Instance.MultiThreadActive) {
                throw new StaticEcsException(type, method, "Forbidden in a parallel query.");
            }
        }

        internal static void AssertTrackingBufferNotOverflow(string type, ulong fromTick, ulong toTick, byte bufferSize, [CallerMemberName] string method = "") {
            var ticksToCheck = toTick - fromTick;
            if (ticksToCheck > bufferSize) {
                throw new StaticEcsException(type, method, $"Tracking buffer overflow: system missed {ticksToCheck - bufferSize} ticks (buffer={bufferSize}, from={fromTick}, to={toTick}). Increase WorldConfig.TrackingBufferSize or call Tick() less frequently.");
            }
        }

        internal static void AssertSameQueryMode(string type, byte mode, [CallerMemberName] string method = "") {
            if (Data.Instance.QueryMode != 0 && mode != Data.Instance.QueryMode) {
                throw new StaticEcsException(type, method, "Nested iterators must have the same QueryMode as the outer iterator");
            }
        }
        
        internal static void AssertQueryNotActive(string type, [CallerMemberName] string method = "") {
            if (Data.Instance.QueryDataCount != 0) {
                throw new StaticEcsException(type, method, "Not available within the query");
            }
        }
        
        internal static void AssertGidIsActive(string type, EntityGID gid, [CallerMemberName] string method = "") {
            if (gid.Status<TWorld>() != GIDStatus.Active) {
                throw new StaticEcsException(type, method, $"EntityGID ID: {gid.Id}, Version {gid.Version}, ClusterId {gid.ClusterId}. Not actual or not loaded.");
            }
        }
        
        internal static void AssertGidIsNotActive(string type, EntityGID gid, [CallerMemberName] string method = "") {
            if (gid.Status<TWorld>() == GIDStatus.Active) {
                throw new StaticEcsException(type, method, $"EntityGID ID: {gid.Id}, Version {gid.Version}, ClusterId {gid.ClusterId}. Already registered.");
            }
        }
        
        internal static void AssertGidIsNotLoaded(string type, EntityGID gid, [CallerMemberName] string method = "") {
            if (gid.Status<TWorld>() != GIDStatus.NotLoaded) {
                throw new StaticEcsException(type, method, $"EntityGID ID: {gid.Id}, Version {gid.Version}, ClusterId {gid.ClusterId}. Not actual or already loaded.");
            }
        }
        
        internal static void AssertClusterIsRegistered(string type, ushort clusterId, [CallerMemberName] string method = "") {
            if (!Data.Instance.ClusterIsRegisteredInternal(clusterId)) {
                throw new StaticEcsException(type, method, $"ClusterId {clusterId} not registered.");
            }
        }

        internal static void AssertEntityTypeIsRegistered(string type, byte entityType, [CallerMemberName] string method = "") {
            if (!Data.Instance.EntityTypes[entityType].Registered) {
                throw new StaticEcsException(type, method, $"EntityType {entityType} not registered.");
            }
        }
        
        internal static void AssertClusterIsNotRegistered(string type, ushort clusterId, [CallerMemberName] string method = "") {
            if (Data.Instance.ClusterIsRegisteredInternal(clusterId)) {
                throw new StaticEcsException(type, method, $"ClusterId {clusterId} already registered.");
            }
        }
        
        internal static void AssertChunkIsRegistered(string type, uint chunkIdx, [CallerMemberName] string method = "") {
            if (!Data.Instance.ChunkIsRegisteredInternal(chunkIdx)) {
                throw new StaticEcsException(type, method, $"Chunk {chunkIdx} not registered.");
            }
        }
        
        internal static void AssertChunkIsNotRegistered(string type, uint chunkIdx, [CallerMemberName] string method = "") {
            if (Data.Instance.ChunkIsRegisteredInternal(chunkIdx)) {
                throw new StaticEcsException(type, method, $"Chunk {chunkIdx} already registered.");
            }
        }

        internal static void AssertGuidNotEmpty<T>(string type, Guid guid, [CallerMemberName] string method = "") {
            if (guid.Equals(Guid.Empty)) {
                throw new StaticEcsException(type, method, $"`{typeof(T).GenericName()}` GUID can't be empty. Set valid value or leave it uninitialized.");
            }
        }
    }
    #endif

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    [Il2CppEagerStaticClassConstruction]
    #endif
    internal static class Utils {
        #if FFS_ECS_DEBUG
        internal static Func<EntityGID, string> EntityGidToString = gid => $"GID {gid.Id}, Version {gid.Version}, ClusterId {gid.ClusterId}";
        #endif

        internal static readonly byte[] DeBruijn = {
            0, 1, 17, 2, 18, 50, 3, 57, 47, 19, 22, 51, 29, 4, 33, 58,
            15, 48, 20, 27, 25, 23, 52, 41, 54, 30, 38, 5, 43, 34, 59, 8,
            63, 16, 49, 56, 46, 21, 28, 32, 14, 26, 24, 40, 53, 37, 42, 7,
            62, 55, 45, 31, 13, 39, 36, 6, 61, 44, 12, 35, 60, 11, 10, 9,
        };

        internal static readonly byte[] DeBruijnMsb = {
            0, 1, 48, 2, 57, 49, 28, 3, 61, 58, 50, 42, 38, 29, 17, 4,
            62, 55, 59, 36, 53, 51, 43, 22, 45, 39, 33, 30, 24, 18, 12,
            5, 63, 47, 56, 27, 60, 41, 37, 16, 54, 35, 52, 21, 44, 32, 23,
            11, 46, 26, 40, 15, 34, 20, 31, 10, 25, 14, 19, 9, 13, 8, 7, 6
        };
        
        [MethodImpl(AggressiveInlining)]
        internal static int Msb(ulong value) {
            #if FFS_ECS_DEBUG
            if (value == 0) throw new StaticEcsException("MSB check");
            #endif
            
            #if NET6_0_OR_GREATER
            return 63 - System.Numerics.BitOperations.LeadingZeroCount(value); 
            #else

            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value |= value >> 32;

            return DeBruijnMsb[(uint) ((((value >> 1) + 1UL) * 0x03F79D71B4CB0A89UL) >> 58)];
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        internal static int PopCnt(this ulong x) {
            #if NET6_0_OR_GREATER
            return System.Numerics.BitOperations.PopCount(x);
            #else
            x -= (x >> 1) & 0x5555555555555555UL;
            x = (x & 0x3333333333333333UL) + ((x >> 2) & 0x3333333333333333UL);
            x = (x + (x >> 4)) & 0x0F0F0F0F0F0F0F0FUL;
            return (int)((x * 0x0101010101010101UL) >> 56);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        internal static int PopLsb(ref ulong v) {
            #if FFS_ECS_DEBUG
            if (v == 0) throw new StaticEcsException("PopLsb: value must not be zero");
            #endif
            #if NET6_0_OR_GREATER
            var val = System.Numerics.BitOperations.TrailingZeroCount(v);
            #else
            var val = DeBruijn[(uint) (((v & (ulong) -(long) v) * 0x37E84A99DAE458FUL) >> 58)];
            #endif
            v &= v - 1;
            return val;
        }

        [MethodImpl(AggressiveInlining)]
        internal static int Lsb(ulong v) {
            #if FFS_ECS_DEBUG
            if (v == 0) throw new StaticEcsException("Lsb: value must not be zero");
            #endif
            #if NET6_0_OR_GREATER
            return System.Numerics.BitOperations.TrailingZeroCount(v);
            #else
            return DeBruijn[(uint) (((v & (ulong) -(long) v) * 0x37E84A99DAE458FUL) >> 58)];
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        internal static void SetBitAtomic(this ref long mask, int bitIndex) {
            var bit = 1UL << bitIndex;
            long oldValue, newValue;
            do {
                oldValue = Volatile.Read(ref mask);
                newValue = (long)((ulong)oldValue | bit);
            }
            while (Interlocked.CompareExchange(ref mask, newValue, oldValue) != oldValue);
        }

        [MethodImpl(AggressiveInlining)]
        internal static void ClearBitAtomic(this ref long mask, int bitIndex) {
            var bitMask = ~(1UL << bitIndex);
            long oldValue, newValue;
            do {
                oldValue = Volatile.Read(ref mask);
                newValue = (long)((ulong)oldValue & bitMask);
            }
            while (Interlocked.CompareExchange(ref mask, newValue, oldValue) != oldValue);
        }

        [MethodImpl(AggressiveInlining)]
        internal static uint RoundUpToPowerOf2(ushort value) {
            if (value == 0) {
                return 0;
            }

            #if NET6_0_OR_GREATER
            return System.Numerics.BitOperations.RoundUpToPowerOf2(value);
            #else
            int u = value;
            u--;
            u |= u >> 1;
            u |= u >> 2;
            u |= u >> 4;
            u |= u >> 8;
            u |= u >> 16;
            u++;

            return (uint)u;
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        internal static uint Normalize(this uint value, uint min) {
            #if FFS_ECS_DEBUG
            if (min == 0 || (min & (min - 1)) != 0) throw new StaticEcsException($"Normalize: min must be non-zero power of 2, got {min}");
            #endif
            var minMinusOne = min - 1;
            return (Math.Max(value, min) + minMinusOne) & ~minMinusOne;
        }

        [MethodImpl(AggressiveInlining)]
        internal static ushort Normalize(this ushort value, ushort min) {
            #if FFS_ECS_DEBUG
            if (min == 0 || (min & (min - 1)) != 0) throw new StaticEcsException($"Normalize: min must be non-zero power of 2, got {min}");
            #endif
            var minMinusOne = min - 1;
            return (ushort) ((Math.Max(value, min) + minMinusOne) & ~minMinusOne);
        }
        
        [MethodImpl(AggressiveInlining)]
        internal static void LoopFallbackCopy<T>(T[] src, uint srcIdx, T[] dst, uint dstIdx, uint len) {
            if (len > 4) {
                Array.Copy(src, srcIdx, dst, dstIdx, len);
                return;
            }
 
            for (var i = 0; i < len; i++) {
                dst[dstIdx + i] = src[srcIdx + i];
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        internal static void LoopFallbackCopyReverse<T>(T[] src, uint srcIdx, T[] dst, uint dstIdx, uint len) {
            if (len > 4) {
                Array.Copy(src, srcIdx, dst, dstIdx, len);
                return;
            }
            
            for (var i = (int) (len - 1); i >= 0; i--) {
                dst[dstIdx + i] = src[srcIdx + i];
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        internal static void LoopFallbackClear<T>(T[] array, int idx, int len) {
            if (len > 4) {
                Array.Clear(array, idx, len);
                return;
            }
            
            for (uint i = 0; i < len; i++) {
                array[idx + i] = default;
            }
        }

        internal static string GenericName(this Type type) {
            if (!type.IsGenericType) {
                return type.Name;
            }

            var genericArguments = type.GetGenericArguments();
            var fullName = type.FullName ?? type.Name;
            var typeName = fullName.Substring(0, fullName.IndexOf('`'));
            var genericArgs = string.Join(", ", Array.ConvertAll(genericArguments, GenericName));

            return $"{typeName}<{genericArgs}>".Replace("+", ".");
        }

        internal static Guid GuidFromAQN(this Type type) {
            var simplifiedAQN = $"{type.FullName}, {type.Assembly.GetName().Name}".ToLowerInvariant();

            using var md5 = MD5.Create();
            var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(simplifiedAQN));
            return new Guid(hashBytes);
        }
    }

    /// <summary>
    /// Reflection-based auto-discovery and registration of all ECS types in specified assemblies.
    /// Scans for value types (structs) implementing ECS marker interfaces and registers them
    /// with the target <c>World&lt;TWorld&gt;</c> using default configuration.
    /// <para>
    /// <b>Detected interfaces and registration behavior:</b>
    /// <list type="bullet">
    /// <item><see cref="IComponent"/> — registered via <c>RegisterComponentType&lt;T&gt;(default)</c>.
    /// Internal framework components (implementing <c>IComponentInternal</c>) are excluded.</item>
    /// <item><see cref="ITag"/> — registered via <c>RegisterTagType&lt;T&gt;(default)</c>.</item>
    /// <item><see cref="IEvent"/> — registered via <c>RegisterEventType&lt;T&gt;(default)</c>.</item>
    /// <item><see cref="ILinkType"/> — wrapped as <c>Link&lt;T&gt;</c> and registered as a component.</item>
    /// <item><see cref="ILinksType"/> — wrapped as <c>Links&lt;T&gt;</c> and registered as a component.</item>
    /// <item><see cref="IMultiComponent"/> — wrapped as <c>Multi&lt;T&gt;</c> and registered as a component.</item>
    /// </list>
    /// </para>
    /// <para>
    /// <b>Important notes:</b>
    /// <list type="bullet">
    /// <item>Must be called during the <c>WorldStatus.Created</c> phase (after <c>Create</c>, before <c>Initialize</c>).</item>
    /// <item>The StaticEcs framework assembly is always excluded from scanning.</item>
    /// <item>Abstract types and open generic definitions are skipped.</item>
    /// <item>All types are registered with default configuration (no custom GUID, no custom serialization).</item>
    /// <item>A single type implementing multiple interfaces will be registered for each applicable one.</item>
    /// <item>If no assemblies are specified, only the calling assembly is scanned (not all loaded assemblies).</item>
    /// </list>
    /// </para>
    /// <para>
    /// Prefer the fluent API: <c>World&lt;W&gt;.Types().RegisterAll()</c>.
    /// </para>
    /// </summary>
    public static class AutoRegistration {
        /// <summary>
        /// Scans the specified assemblies for all ECS types and registers them with <c>World&lt;TWorld&gt;</c>.
        /// See <see cref="AutoRegistration"/> class documentation for the full list of detected interfaces
        /// and registration behavior.
        /// </summary>
        /// <typeparam name="TWorld">The world type to register types into.</typeparam>
        /// <param name="assemblies">
        /// Assemblies to scan. If <c>null</c> or empty, scans the calling assembly only.
        /// The StaticEcs framework assembly is always excluded.
        /// </param>
        #if NET5_0_OR_GREATER
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods, typeof(World<>))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(World<>))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicNestedTypes, typeof(World<>))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor, typeof(ComponentTypeConfig<>))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor, typeof(TagTypeConfig<>))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor, typeof(EventTypeConfig<>))]
        [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Types are preserved by static usage in consuming code.")]
        [UnconditionalSuppressMessage("Trimming", "IL2055", Justification = "Config types are preserved by DynamicDependency above.")]
        [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "Registration methods are preserved by DynamicDependency above.")]
        [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Config types have public parameterless constructors preserved by DynamicDependency above.")]
        [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Generic instantiations are pre-compiled because component/tag/event types are statically referenced in consuming code.")]
        #endif
        [MethodImpl(NoInlining)]
        public static void RegisterAll<TWorld>(params Assembly[] assemblies) where TWorld : struct, IWorldType {
            var ecsAssembly = typeof(IComponent).Assembly;
            var worldType = typeof(World<TWorld>);
            var tWorld = worldType.GetGenericArguments()[0];

            var registerComponent = worldType.GetMethod("RegisterComponentType", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new StaticEcsException($"AutoRegistration: method RegisterComponentType not found on {worldType.Name}");
            var registerTag = worldType.GetMethod("RegisterTagType", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new StaticEcsException($"AutoRegistration: method RegisterTagType not found on {worldType.Name}");
            var registerEvent = worldType.GetMethod("RegisterEventType", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new StaticEcsException($"AutoRegistration: method RegisterEventType not found on {worldType.Name}");
            var registerMultiComponent = worldType.GetMethod("RegisterMultiComponentType", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new StaticEcsException($"AutoRegistration: method RegisterMultiComponentType not found on {worldType.Name}");
            var registerEntityType = worldType.GetMethod("RegisterEntityType", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new StaticEcsException($"AutoRegistration: method RegisterEntityType not found on {worldType.Name}");

            var isComponentRegistered = worldType.GetMethod("IsComponentTypeRegistered", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new StaticEcsException($"AutoRegistration: method IsComponentTypeRegistered not found on {worldType.Name}");
            var isTagRegistered = worldType.GetMethod("IsTagTypeRegistered", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new StaticEcsException($"AutoRegistration: method IsTagTypeRegistered not found on {worldType.Name}");
            var isEventRegistered = worldType.GetMethod("IsEventTypeRegistered", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new StaticEcsException($"AutoRegistration: method IsEventTypeRegistered not found on {worldType.Name}");
            var isEntityTypeRegisteredById = worldType.GetMethod("IsEntityTypeRegistered", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(byte) }, null)
                ?? throw new StaticEcsException($"AutoRegistration: method IsEntityTypeRegistered(byte) not found on {worldType.Name}");
            MethodInfo isEntityTypeRegisteredByType = null;
            foreach (var m in worldType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)) {
                if (m.Name == "IsEntityTypeRegistered" && m.IsGenericMethodDefinition) {
                    isEntityTypeRegisteredByType = m;
                    break;
                }
            }
            if (isEntityTypeRegisteredByType == null) throw new StaticEcsException($"AutoRegistration: method IsEntityTypeRegistered<T> not found on {worldType.Name}");

            var linkOpenType = worldType.GetNestedType("Link`1", BindingFlags.Public)
                ?? throw new StaticEcsException($"AutoRegistration: nested type Link`1 not found on {worldType.Name}");
            var linksOpenType = worldType.GetNestedType("Links`1", BindingFlags.Public)
                ?? throw new StaticEcsException($"AutoRegistration: nested type Links`1 not found on {worldType.Name}");
            var multiOpenType = worldType.GetNestedType("Multi`1", BindingFlags.Public)
                ?? throw new StaticEcsException($"AutoRegistration: nested type Multi`1 not found on {worldType.Name}");

            foreach (var assembly in assemblies) {
                if (assembly == ecsAssembly) continue;

                foreach (var type in assembly.GetTypes()) {
                    if (!type.IsValueType || type.IsAbstract || type.IsGenericTypeDefinition) continue;

                    if (typeof(IComponent).IsAssignableFrom(type) && !typeof(IComponentInternal).IsAssignableFrom(type)) {
                        if (!IsRegistered(isComponentRegistered, type))
                            InvokeRegisterComponent(registerComponent, type);
                    }

                    if (typeof(ITag).IsAssignableFrom(type)) {
                        if (!IsRegistered(isTagRegistered, type))
                            InvokeRegisterTag(registerTag, type);
                    }

                    if (typeof(IEvent).IsAssignableFrom(type)) {
                        if (!IsRegistered(isEventRegistered, type))
                            InvokeRegisterEvent(registerEvent, type);
                    }

                    if (typeof(IMultiComponent).IsAssignableFrom(type)) {
                        var closedMulti = multiOpenType.MakeGenericType(tWorld, type);
                        if (!IsRegistered(isComponentRegistered, closedMulti))
                            InvokeRegisterMultiComponent(registerMultiComponent, closedMulti, type);
                    }

                    if (typeof(ILinksType).IsAssignableFrom(type)) {
                        var closedLinks = linksOpenType.MakeGenericType(tWorld, type);
                        if (!IsRegistered(isComponentRegistered, closedLinks))
                            InvokeRegisterComponent(registerComponent, closedLinks);
                    }
                    else if (typeof(ILinkType).IsAssignableFrom(type)) {
                        var closedLink = linkOpenType.MakeGenericType(tWorld, type);
                        if (!IsRegistered(isComponentRegistered, closedLink))
                            InvokeRegisterComponent(registerComponent, closedLink);
                    }

                    if (typeof(IEntityType).IsAssignableFrom(type) && type != typeof(Default)) {
                        var id = FindStaticConfig(type, typeof(byte), "Id");
                        if (id != null) {
                            if (!(bool) isEntityTypeRegisteredById.Invoke(null, new[] { id }))
                                registerEntityType.MakeGenericMethod(type).Invoke(null, new[] { id });
                        } else if (!IsRegistered(isEntityTypeRegisteredByType, type)) {
                            throw new StaticEcsException($"AutoRegistration: IEntityType {type.Name} must have a static byte Id field or be registered manually");
                        }
                    }
                }
            }
        }

        internal static ComponentTypeConfig<T> FindComponentConfig<T>() where T : struct, IComponentOrTag {
            return (ComponentTypeConfig<T>)FindComponentConfig(typeof(T));
        }

        internal static EventTypeConfig<T> FindEventConfig<T>() where T : struct, IEvent {
            return (EventTypeConfig<T>)FindEventConfig(typeof(T));
        }

        internal static TagTypeConfig<T> FindTagConfig<T>() where T : struct, ITag {
            return (TagTypeConfig<T>)FindTagConfig(typeof(T));
        }

        #if NET5_0_OR_GREATER
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor, typeof(UnmanagedPackArrayStrategy<>))]
        #endif
        internal static IPackArrayStrategy<T> TryCreateUnmanagedPackArrayStrategy<T>() where T : struct {
            try {
                var unmanagedPackStrategyType = typeof(UnmanagedPackArrayStrategy<>).MakeGenericType(typeof(T));
                return (IPackArrayStrategy<T>)Activator.CreateInstance(unmanagedPackStrategyType);
            }
            catch (Exception) {
                return null;
            }
        }

        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("Trimming", "IL2055", Justification = "Config types are preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "Registration methods are preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Config types have public parameterless constructors preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Generic instantiations are pre-compiled because types are statically referenced in consuming code.")]
        #endif
        private static void InvokeRegisterComponent(MethodInfo openMethod, Type componentType) {
            var config = FindComponentConfig(componentType);
            openMethod.MakeGenericMethod(componentType).Invoke(null, new[] { config });
        }

        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("Trimming", "IL2055", Justification = "Config types are preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "Registration methods are preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Config types have public parameterless constructors preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Generic instantiations are pre-compiled because types are statically referenced in consuming code.")]
        #endif
        private static void InvokeRegisterMultiComponent(MethodInfo openMethod, Type multiType, Type elementType) {
            var configType = typeof(ComponentTypeConfig<>).MakeGenericType(multiType);
            var config = FindStaticConfig(multiType, configType, "Config")
                         ?? Activator.CreateInstance(configType);
            var strategyType = typeof(IPackArrayStrategy<>).MakeGenericType(elementType);
            var strategy = FindStaticConfig(elementType, strategyType, "PackStrategy");
            openMethod.MakeGenericMethod(elementType).Invoke(null, new[] { config, strategy });
        }

        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("Trimming", "IL2055", Justification = "Config types are preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "Registration methods are preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Config types have public parameterless constructors preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Generic instantiations are pre-compiled because types are statically referenced in consuming code.")]
        #endif
        private static void InvokeRegisterEvent(MethodInfo openMethod, Type eventType) {
            var config = FindEventConfig(eventType);
            openMethod.MakeGenericMethod(eventType).Invoke(null, new[] { config });
        }

        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("Trimming", "IL2055", Justification = "Config types are preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "Registration methods are preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Config types have public parameterless constructors preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Generic instantiations are pre-compiled because types are statically referenced in consuming code.")]
        #endif
        private static void InvokeRegisterTag(MethodInfo openMethod, Type tagType) {
            var config = FindTagConfig(tagType);
            openMethod.MakeGenericMethod(tagType).Invoke(null, new[] { config });
        }

        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("Trimming", "IL2055", Justification = "Config types are preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "Registration methods are preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Config types have public parameterless constructors preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Generic instantiations are pre-compiled because types are statically referenced in consuming code.")]
        #endif
        private static object FindComponentConfig(Type componentType) {
            var configType = typeof(ComponentTypeConfig<>).MakeGenericType(componentType);
            return FindStaticConfig(componentType, configType, "Config") ?? Activator.CreateInstance(configType);
        }

        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("Trimming", "IL2055", Justification = "Config types are preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "Registration methods are preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Config types have public parameterless constructors preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Generic instantiations are pre-compiled because types are statically referenced in consuming code.")]
        #endif
        private static object FindEventConfig(Type eventType) {
            var configType = typeof(EventTypeConfig<>).MakeGenericType(eventType);
            return FindStaticConfig(eventType, configType, "Config") ?? Activator.CreateInstance(configType);
        }

        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("Trimming", "IL2055", Justification = "Config types are preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "Registration methods are preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Config types have public parameterless constructors preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Generic instantiations are pre-compiled because types are statically referenced in consuming code.")]
        #endif
        private static object FindTagConfig(Type tagType) {
            var configType = typeof(TagTypeConfig<>).MakeGenericType(tagType);
            return FindStaticConfig(tagType, configType, "Config") ?? Activator.CreateInstance(configType);
        }

        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("Trimming", "IL2070", Justification = "Component/event/tag types with Config fields are statically referenced in consuming code.")]
        #endif
        #if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "Registration check methods are preserved by DynamicDependency on RegisterAll.")]
        [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Generic instantiations are pre-compiled because types are statically referenced in consuming code.")]
        #endif
        private static bool IsRegistered(MethodInfo openMethod, Type type)
            => (bool) openMethod.MakeGenericMethod(type).Invoke(null, null);

        private static object FindStaticConfig(Type type, Type configType, string preferredName) {
            object result = null;
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields) {
                if (field.FieldType == configType) {
                    if (field.Name == preferredName) return field.GetValue(null);
                    result ??= field.GetValue(null);
                }
            }
            if (result != null) return result;

            var properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            foreach (var property in properties) {
                if (property.PropertyType == configType) {
                    if (property.Name == preferredName) return property.GetValue(null);
                    result ??= property.GetValue(null);
                }
            }
            return result;
        }
    }
}

#if ENABLE_IL2CPP
namespace Unity.IL2CPP.CompilerServices {
    using System;

    internal enum Option {
        NullChecks = 1,
        ArrayBoundsChecks = 2,
        DivideByZeroChecks = 3
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    internal class Il2CppSetOptionAttribute : Attribute {
        public Option Option { get; }
        public object Value { get; }

        public Il2CppSetOptionAttribute(Option option, object value) {
            Option = option;
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    internal class Il2CppEagerStaticClassConstructionAttribute : Attribute { }
}
#endif