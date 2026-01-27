#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public abstract partial class World<TWorld> {

        /// <summary>
        /// Creates a query with no filter (<see cref="Nothing"/>), matching all alive entities.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static WorldQuery<Nothing> Query() => new(default);

        /// <summary>
        /// Creates a query with a single filter.
        /// <para>Usage: <c>W.Query&lt;All&lt;Position, Velocity&gt;&gt;()</c></para>
        /// </summary>
        /// <typeparam name="TFilter0">
        /// A filter type implementing <see cref="IQueryFilter"/>. Common filters:
        /// <see cref="All{T0}"/>, <see cref="None{T0}"/>, <see cref="Any{T0,T1}"/>,
        /// <see cref="TagAll{T0}"/>, <see cref="TagNone{T0}"/>, <see cref="TagAny{T0,T1}"/>.
        /// </typeparam>
        [MethodImpl(AggressiveInlining)]
        public static WorldQuery<TFilter0> Query<TFilter0>(TFilter0 filter0 = default)
            where TFilter0 : struct, IQueryFilter => new(filter0);

        /// <inheritdoc cref="Query{TFilter0}"/>
        [MethodImpl(AggressiveInlining)]
        public static WorldQuery<And<TFilter0, TFilter1>> Query<TFilter0, TFilter1>(TFilter0 filter0 = default, TFilter1 filter1 = default)
            where TFilter0 : struct, IQueryFilter
            where TFilter1 : struct, IQueryFilter =>
            new (new And<TFilter0, TFilter1>(filter0, filter1));

        /// <inheritdoc cref="Query{TFilter0}"/>
        [MethodImpl(AggressiveInlining)]
        public static WorldQuery<And<TFilter0, TFilter1, TFilter2>> Query<TFilter0, TFilter1, TFilter2>(TFilter0 filter0 = default, TFilter1 filter1 = default, TFilter2 filter2 = default)
            where TFilter0 : struct, IQueryFilter
            where TFilter1 : struct, IQueryFilter
            where TFilter2 : struct, IQueryFilter => new (new And<TFilter0, TFilter1, TFilter2>(filter0, filter1, filter2));

        /// <inheritdoc cref="Query{TFilter0}"/>
        [MethodImpl(AggressiveInlining)]
        public static WorldQuery<And<TFilter0, TFilter1, TFilter2, TFilter3>> Query<TFilter0, TFilter1, TFilter2, TFilter3>(TFilter0 filter0 = default, TFilter1 filter1 = default, TFilter2 filter2 = default, TFilter3 filter3 = default)
            where TFilter0 : struct, IQueryFilter
            where TFilter1 : struct, IQueryFilter
            where TFilter2 : struct, IQueryFilter
            where TFilter3 : struct, IQueryFilter => new (new And<TFilter0, TFilter1, TFilter2, TFilter3>(filter0, filter1, filter2, filter3));

        /// <inheritdoc cref="Query{TFilter0}"/>
        [MethodImpl(AggressiveInlining)]
        public static WorldQuery<And<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4>> Query<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4>(
            TFilter0 filter0 = default, TFilter1 filter1 = default, TFilter2 filter2 = default, TFilter3 filter3 = default, TFilter4 filter4 = default
        )
            where TFilter0 : struct, IQueryFilter
            where TFilter1 : struct, IQueryFilter
            where TFilter2 : struct, IQueryFilter
            where TFilter3 : struct, IQueryFilter
            where TFilter4 : struct, IQueryFilter => new (new And<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4>(filter0, filter1, filter2, filter3, filter4));

        /// <inheritdoc cref="Query{TFilter0}"/>
        [MethodImpl(AggressiveInlining)]
        public static WorldQuery<And<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4, TFilter5>> Query<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4, TFilter5>(
            TFilter0 filter0 = default, TFilter1 filter1 = default, TFilter2 filter2 = default, TFilter3 filter3 = default, TFilter4 filter4 = default, TFilter5 filter5 = default
        )
            where TFilter0 : struct, IQueryFilter
            where TFilter1 : struct, IQueryFilter
            where TFilter2 : struct, IQueryFilter
            where TFilter3 : struct, IQueryFilter
            where TFilter4 : struct, IQueryFilter
            where TFilter5 : struct, IQueryFilter => new (new And<TFilter0, TFilter1, TFilter2, TFilter3, TFilter4, TFilter5>(filter0, filter1, filter2, filter3, filter4, filter5));

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        /// <summary>
        /// Stack-allocated query handle providing zero-allocation entity iteration and batch operations.
        /// Created via <c>World&lt;TWorld&gt;.Query&lt;TFilter&gt;()</c>.
        /// <para>
        /// Iteration modes:
        /// <list type="bullet">
        /// <item><see cref="Entities"/> — strict mode (default, faster). Forbids modifying filtered types on other entities.</item>
        /// <item><see cref="EntitiesFlexible"/> — flexible mode. Allows modifying filtered types on other entities during iteration.</item>
        /// </list>
        /// </para>
        /// <para>
        /// Also supports delegate-based iteration via <c>For()</c>, parallel iteration via <c>ForParallel()</c>,
        /// and batch operations via <c>BatchAdd()</c>, <c>BatchDelete()</c>, etc. (defined in partial Query files).
        /// </para>
        /// </summary>
        /// <typeparam name="TFilter">The filter type constraining which entities match. See <see cref="IQueryFilter"/>.</typeparam>
        public readonly ref partial struct WorldQuery<TFilter> where TFilter : struct, IQueryFilter {
            internal readonly TFilter Filter;

            [MethodImpl(AggressiveInlining)]
            public WorldQuery(TFilter filter) {
                Filter = filter;
            }

            /// <summary>
            /// Returns a strict-mode iterator over matching entities. This is the default and fastest iteration mode.
            /// <para>
            /// In strict mode, modifying filtered component/tag types on OTHER entities during iteration is forbidden
            /// (enforced in debug builds). Only the current entity's filtered components may be safely modified.
            /// </para>
            /// </summary>
            /// <param name="entities">Which entity lifecycle states to include (default: only enabled).</param>
            /// <param name="clusters">Optional cluster IDs to restrict iteration scope. Empty = all clusters.</param>
            /// <returns>A <see cref="QueryStrictIterator{TWorld, TFilter}"/> for foreach enumeration.</returns>
            [MethodImpl(AggressiveInlining)]
            public QueryStrictIterator<TWorld, TFilter> Entities(EntityStatusType entities = EntityStatusType.Enabled,
                                                                 ReadOnlySpan<ushort> clusters = default) {
                return new QueryStrictIterator<TWorld, TFilter>(clusters, Filter, entities);
            }

            /// <summary>
            /// Returns a flexible-mode iterator over matching entities. Slower than <see cref="Entities"/>
            /// but allows modifying filtered component/tag types on OTHER entities during iteration.
            /// <para>
            /// Use this when iteration logic needs to add/remove/enable/disable filtered components on non-current entities.
            /// The iterator re-checks bitmasks on each entity to handle concurrent structural changes.
            /// </para>
            /// </summary>
            /// <param name="entities">Which entity lifecycle states to include (default: only enabled).</param>
            /// <param name="clusters">Optional cluster IDs to restrict iteration scope. Empty = all clusters.</param>
            /// <returns>A <see cref="QueryFlexibleIterator{TWorld, TFilter}"/> for foreach enumeration.</returns>
            [MethodImpl(AggressiveInlining)]
            public QueryFlexibleIterator<TWorld, TFilter> EntitiesFlexible(EntityStatusType entities = EntityStatusType.Enabled,
                                                                           ReadOnlySpan<ushort> clusters = default) {
                return new QueryFlexibleIterator<TWorld, TFilter>(clusters, Filter, entities);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal QueryStrictIterator<TWorld, TFilter> Entities(ReadOnlySpan<uint> chunks, EntityStatusType entities = EntityStatusType.Enabled) {
                return new QueryStrictIterator<TWorld, TFilter>(chunks, Filter, entities);
            }

            [MethodImpl(AggressiveInlining)]
            internal QueryFlexibleIterator<TWorld, TFilter> EntitiesFlexible(ReadOnlySpan<uint> chunks, EntityStatusType entities = EntityStatusType.Enabled) {
                return new QueryFlexibleIterator<TWorld, TFilter>(chunks, Filter, entities);
            }

            /// <summary>
            /// Checks if at least one entity matches the query and returns it.
            /// Optimized single-entity lookup.
            /// </summary>
            /// <param name="entity">The first matching entity, or <c>default</c> if none.</param>
            /// <param name="entities">Which entity lifecycle states to include (default: only enabled).</param>
            /// <param name="clusters">Optional cluster IDs to restrict search scope. Empty = all clusters.</param>
            /// <returns><c>true</c> if a matching entity was found.</returns>
            [MethodImpl(AggressiveInlining)]
            public bool Any(out Entity entity, EntityStatusType entities = EntityStatusType.Enabled,
                            ReadOnlySpan<ushort> clusters = default) {
                return FindFirst(Filter, clusters, entities, out entity, false);
            }

            /// <summary>
            /// Checks if exactly zero or one entity matches the query and returns it.
            /// In debug builds, throws <see cref="StaticEcsException"/> if more than one entity matches.
            /// In release builds, behaves identically to <see cref="Any"/>.
            /// Optimized single-entity lookup.
            /// </summary>
            /// <param name="entity">The single matching entity, or <c>default</c> if none.</param>
            /// <param name="entities">Which entity lifecycle states to include (default: only enabled).</param>
            /// <param name="clusters">Optional cluster IDs to restrict search scope. Empty = all clusters.</param>
            /// <returns><c>true</c> if exactly one entity was found; <c>false</c> if zero.</returns>
            [MethodImpl(AggressiveInlining)]
            public bool One(out Entity entity, EntityStatusType entities = EntityStatusType.Enabled,
                            ReadOnlySpan<ushort> clusters = default) {
                return FindFirst(Filter, clusters, entities, out entity, true);
            }
        }
        
        /// <summary>
        /// Provides indexed access to a contiguous block of entities during low-level query iteration.
        /// Used internally by <c>ForBlock</c>-style iteration for maximum performance via unsafe pointers.
        /// </summary>
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        public ref struct EntityBlock {
            internal Entity Value;
            internal uint Offset;

            [MethodImpl(AggressiveInlining)]
            internal EntityBlock(Entity value, uint offset) {
                Value = value;
                Offset = offset;
            }

            /// <summary>Gets the entity at the specified index within this block.</summary>
            public Entity this[uint idx] {
                [MethodImpl(AggressiveInlining)]
                get {
                    Value.IdWithOffset = Offset + idx;
                    return Value;
                }
            }
        }
    }

    /// <summary>
    /// Interface for query filter types that determine which entities match a query.
    /// Filters operate at two levels: chunk-level (coarse, via heuristic bitmasks) and entity-level (fine, via presence bitmasks).
    /// <para>
    /// Built-in filter implementations for components:
    /// <see cref="All{T0}"/>, <see cref="None{T0}"/>, <see cref="Any{T0, T1}"/>,
    /// <see cref="AllOnlyDisabled{T0}"/>, <see cref="AllWithDisabled{T0}"/>,
    /// <see cref="NoneWithDisabled{T0}"/>, <see cref="AnyOnlyDisabled{T0, T1}"/>, <see cref="AnyWithDisabled{T0, T1}"/>.
    /// For tags: <c>TagAll</c>, <c>TagNone</c>, <c>TagAny</c>.
    /// Combine multiple filters as query type parameters for complex matching logic.
    /// </para>
    /// </summary>
    public interface IQueryFilter {
        /// <summary>
        /// Coarse-grained chunk-level filtering. Returns heuristic bitmask
        /// to quickly skip entire blocks (64 entities) that cannot match.
        /// The caller ANDs the result with the current chunk mask.
        /// </summary>
        public ulong FilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType;

        /// <summary>
        /// Fine-grained entity-level filtering. Returns actual presence bitmask
        /// to determine exactly which entities in this block match.
        /// The caller ANDs the result with the current entities mask.
        /// </summary>
        public ulong FilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType;

        /// <summary>
        /// Registers query data for live cache updates. Called when a query begins iteration
        /// in flexible mode, so that structural changes (Add/Delete/Enable/Disable) during iteration
        /// can update the query's bitmask to maintain stable iteration.
        /// </summary>
        public void PushQueryData<TWorld>(QueryData data) where TWorld : struct, IWorldType;

        /// <summary>
        /// Unregisters query data after iteration completes. Pairs with <see cref="PushQueryData{TWorld}"/>.
        /// </summary>
        public void PopQueryData<TWorld>() where TWorld : struct, IWorldType;

        #if FFS_ECS_DEBUG
        /// <summary>
        /// Debug-only: asserts that all component/tag types used by this filter are registered.
        /// </summary>
        public void Assert<TWorld>() where TWorld : struct, IWorldType;

        /// <summary>
        /// Debug-only: increments/decrements blocker counters on filtered component/tag types
        /// to detect forbidden structural changes during strict-mode iteration.
        /// </summary>
        public void Block<TWorld>(int val) where TWorld : struct, IWorldType;
        #endif

        #if FFS_ECS_BURST
        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterChunk<TWorld>(uint chunkIdx) where TWorld : struct, IWorldType;

        [MethodImpl(AggressiveInlining)]
        public ulong BurstFilterEntities<TWorld>(uint segmentIdx, byte segmentBlockIdx) where TWorld : struct, IWorldType;
        #endif
    }

    /// <summary>
    /// Controls which entities are iterated based on their lifecycle state (entity-level, not component-level).
    /// </summary>
    public enum EntityStatusType : byte {
        /// <summary>Only iterate entities that are enabled (default).</summary>
        Enabled = 0,
        /// <summary>Only iterate entities that are disabled.</summary>
        Disabled = 1,
        /// <summary>Iterate all entities regardless of enabled/disabled state.</summary>
        Any = 2,
    }

    /// <summary>
    /// Controls which component presence states are considered when filtering.
    /// Used by some query overloads to parameterize filter behavior at runtime.
    /// </summary>
    public enum ComponentStatus : byte {
        /// <summary>Only match enabled components.</summary>
        Enabled = 0,
        /// <summary>Match components in any state (enabled or disabled).</summary>
        Any = 1,
        /// <summary>Only match disabled components.</summary>
        Disabled = 2,
    }

    /// <summary>
    /// Controls the iteration mode for queries, affecting performance and safety guarantees.
    /// The default value (<see cref="Strict"/>) provides the fastest iteration.
    /// </summary>
    public enum QueryMode {
        /// <summary>
        /// Strict mode (default): forbids modifying filtered component/tag types on OTHER entities during iteration.
        /// Faster — uses fast-path for full blocks, skips re-checking bitmasks. Use when you only
        /// modify the currently iterated entity's filtered components, or don't modify them at all.
        /// </summary>
        Strict,
        /// <summary>
        /// Flexible mode: allows modifying filtered component/tag types on other entities during iteration.
        /// Slower — always re-checks bitmasks to handle concurrent structural changes. Use when
        /// iteration logic may add/remove/enable/disable filtered components on non-current entities.
        /// </summary>
        Flexible
    }

    /// <summary>
    /// Cached bitmask for a single block (64 entities) during flexible-mode query iteration.
    /// Allows structural changes to update the mask mid-iteration without invalidating the iterator.
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public struct BlockMaskCache {
        /// <summary>Bitmask of matching entities in this block. Bits cleared by structural changes during iteration.</summary>
        public ulong EntitiesMask;
        /// <summary>Index of the next non-empty global block in the iteration chain, or -1 if this is the last block.</summary>
        public int NextGlobalBlock;
    }
    

    /// <summary>
    /// Holds the cached block masks and update callback for a single active flexible-mode query.
    /// Registered via <see cref="IQueryFilter.PushQueryData{TWorld}"/> when flexible iteration begins,
    /// and unregistered via <see cref="IQueryFilter.PopQueryData{TWorld}"/> when it ends.
    /// Structural changes (Add/Delete/Enable/Disable) on filtered types invoke <see cref="Update"/>
    /// to clear affected bits so the iterator skips entities that no longer match.
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public struct QueryData {
        /// <summary>Array of cached block masks for the active query. Indexed by global block index.</summary>
        public BlockMaskCache[] Blocks;
        /// <summary>Optional callback for custom cache update logic. If null, the default bitwise AND is used.</summary>
        public OnCacheUpdate OnCacheUpdate;
        
        [MethodImpl(AggressiveInlining)]
        public void Update(ulong invertedBlockEntityMask, uint segmentIdx, byte segmentBlockIdx) {
            if (OnCacheUpdate == null) {
                Blocks[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask &= invertedBlockEntityMask;
            } else {
                OnCacheUpdate(Blocks, invertedBlockEntityMask, segmentIdx, segmentBlockIdx, false);
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void BatchUpdate(uint segmentIdx, byte segmentBlockIdx) {
            if (OnCacheUpdate == null) {
                Blocks[(segmentIdx << Const.BLOCKS_IN_SEGMENT_SHIFT) + segmentBlockIdx].EntitiesMask = 0;
            } else {
                OnCacheUpdate(Blocks, 0, segmentIdx, segmentBlockIdx, true);
            }
        }
    }

    /// <summary>
    /// Delegate for custom cache update logic during flexible-mode iteration.
    /// Called when structural changes affect entities in a block being iterated.
    /// </summary>
    public delegate void OnCacheUpdate(BlockMaskCache[] cache, ulong invertedEntitiesMask, uint segmentIdx, byte segmentBlockIdx, bool batch);
}