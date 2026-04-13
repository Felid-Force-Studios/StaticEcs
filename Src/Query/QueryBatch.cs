#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
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

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public abstract partial class World<TWorld> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        public readonly ref partial struct WorldQuery<TFilter> where TFilter : struct, IQueryFilter {

            /// <summary>
            /// Counts all entities matching this query's filter. Builds the full query bitmask and sums
            /// the popcount of each matched block. This is a full scan, not O(1).
            /// </summary>
            /// <param name="entities">Which entity lifecycle state to count (enabled, disabled, or any).</param>
            /// <param name="clusters">Optional cluster filter. If provided, only entities in these clusters are counted.</param>
            /// <returns>Total number of matching entities.</returns>
            [MethodImpl(AggressiveInlining)]
            public int EntitiesCount(EntityStatusType entities = EntityStatusType.Enabled,
                                     ReadOnlySpan<ushort> clusters = default) {
                var count = 0;
                if (Prepare(Filter, clusters, entities, true, out var queryData, out var firstGlobalBlockIdx)) {
                    while (firstGlobalBlockIdx >= 0) {
                        ref var cache = ref queryData.Blocks[firstGlobalBlockIdx];
                        count += cache.EntitiesMask.PopCnt();
                        firstGlobalBlockIdx = cache.NextGlobalBlock;
                    }

                    Dispose(Filter, entities, true, queryData);
                }

                return count;
            }

            /// <summary>
            /// Counts all entities matching this query's filter. Builds the full query bitmask and sums
            /// the popcount of each matched block. This is a full scan, not O(1).
            /// </summary>
            /// <param name="entities">Which entity lifecycle state to count (enabled, disabled, or any).</param>
            /// <param name="chunks">Chunks filter.</param>
            /// <returns>Total number of matching entities.</returns>
            [MethodImpl(AggressiveInlining)]
            public int EntitiesCount(ReadOnlySpan<uint> chunks, EntityStatusType entities = EntityStatusType.Enabled) {
                var count = 0;
                if (Prepare(Filter, chunks, entities, true, out var queryData, out var firstGlobalBlockIdx)) {
                    while (firstGlobalBlockIdx >= 0) {
                        ref var cache = ref queryData.Blocks[firstGlobalBlockIdx];
                        count += cache.EntitiesMask.PopCnt();
                        firstGlobalBlockIdx = cache.NextGlobalBlock;
                    }

                    Dispose(Filter, entities, true, queryData);
                }

                return count;
            }

            /// <summary>
            /// Destroys all entities matching this query's filter in a single batch operation.
            /// More efficient than iterating and destroying one by one. All matched entities are
            /// removed from the world, their components' OnDelete hooks are called, and their
            /// storage is released.
            /// </summary>
            /// <param name="entities">Which entity lifecycle state to destroy (enabled, disabled, or any).</param>
            /// <param name="mode">Query mode: Strict (default) blocks hooks from modifying filtered types on non-current entities; Flexible allows free modification.</param>
            /// <param name="clusters">Optional cluster filter. If provided, only entities in these clusters are destroyed.</param>
            /// <param name="withDisabledClusters">If <c>true</c>, also includes entities in disabled (unloaded) clusters.</param>
            [MethodImpl(AggressiveInlining)]
            public void BatchDestroy(EntityStatusType entities = EntityStatusType.Enabled,
                                     QueryMode mode = QueryMode.Strict,
                                     ReadOnlySpan<ushort> clusters = default,
                                     bool withDisabledClusters = false) {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx, withDisabledClusters)) {
                    Data.Instance.DestroyEntitiesBatch(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void BatchDestroyInternal(HookReason reason,
                                               EntityStatusType entities = EntityStatusType.Enabled,
                                               QueryMode mode = QueryMode.Strict,
                                               ReadOnlySpan<ushort> clusters = default,
                                               bool withDisabledClusters = false) {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx, withDisabledClusters)) {
                    Data.Instance.DestroyEntitiesBatch(queryData, firstGlobalBlockIdx, reason);
                    Dispose(Filter, entities, strict, queryData);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal bool BatchDestroyInternal(HookReason reason,
                                               ReadOnlySpan<uint> chunks,
                                               EntityStatusType entities = EntityStatusType.Enabled,
                                               QueryMode mode = QueryMode.Strict) {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, chunks, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Data.Instance.DestroyEntitiesBatch(queryData, firstGlobalBlockIdx, reason);
                    Dispose(Filter, entities, strict, queryData);
                    return true;
                }

                return false;
            }

            /// <inheritdoc cref="BatchDestroy(EntityStatusType, QueryMode, ReadOnlySpan{ushort}, bool)"/>
            [MethodImpl(AggressiveInlining)]
            public void BatchDestroy(ReadOnlySpan<uint> chunks,
                                     EntityStatusType entities = EntityStatusType.Enabled,
                                     QueryMode mode = QueryMode.Strict) {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, chunks, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Data.Instance.DestroyEntitiesBatch(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }
            }

            /// <summary>
            /// Unloads all entities matching this query's filter in a single batch operation.
            /// More efficient than iterating and unloading one by one. All matched entities have
            /// their components and tags removed via OnDelete hooks, and their loaded state is cleared,
            /// but the entities remain alive (active mask, disabled mask, and versions are preserved).
            /// </summary>
            /// <param name="entities">Which entity lifecycle state to unload (enabled, disabled, or any).</param>
            /// <param name="mode">Query mode: Strict (default) blocks hooks from modifying filtered types on non-current entities; Flexible allows free modification.</param>
            /// <param name="clusters">Optional cluster filter. If provided, only entities in these clusters are unloaded.</param>
            /// <param name="withDisabledClusters">If <c>true</c>, also includes entities in disabled (unloaded) clusters.</param>
            [MethodImpl(AggressiveInlining)]
            public void BatchUnload(EntityStatusType entities = EntityStatusType.Enabled,
                                    QueryMode mode = QueryMode.Strict,
                                    ReadOnlySpan<ushort> clusters = default,
                                    bool withDisabledClusters = false) {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx, withDisabledClusters)) {
                    Data.Instance.UnloadEntitiesBatch(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }
            }

            /// <inheritdoc cref="BatchUnload(EntityStatusType, QueryMode, ReadOnlySpan{ushort}, bool)"/>
            [MethodImpl(AggressiveInlining)]
            public void BatchUnload(ReadOnlySpan<uint> chunks,
                                    EntityStatusType entities = EntityStatusType.Enabled,
                                    QueryMode mode = QueryMode.Strict) {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, chunks, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Data.Instance.UnloadEntitiesBatch(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }
            }

            /// <summary>
            /// Adds components to all entities matching this query's filter in a single batch.
            /// For entities that already have the component, the existing value is preserved (no overwrite, no hooks).
            /// For newly added components, OnAdd hooks are triggered. Returns <c>this</c> for method chaining.
            /// <para>Available with 1–5 type parameters.</para>
            /// </summary>
            /// <param name="entities">Which entity lifecycle state to target.</param>
            /// <param name="mode">Query mode: Strict (default) blocks hooks from modifying filtered types on non-current entities; Flexible allows free modification.</param>
            /// <param name="clusters">Optional cluster filter.</param>
            /// <returns>This query instance for chaining.</returns>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchAdd<T1>(EntityStatusType entities = EntityStatusType.Enabled,
                                                    QueryMode mode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default) where T1 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchAdd{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchAdd<T1>(ReadOnlySpan<uint> chunks,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    QueryMode mode = QueryMode.Strict) where T1 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, chunks, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchAdd{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchAdd<T1, T2>(EntityStatusType entities = EntityStatusType.Enabled,
                                                        QueryMode mode = QueryMode.Strict,
                                                        ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponent
                where T2 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchAdd{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchAdd<T1, T2, T3>(EntityStatusType entities = EntityStatusType.Enabled,
                                                            QueryMode mode = QueryMode.Strict,
                                                            ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Components<T3>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchAdd{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchAdd<T1, T2, T3, T4>(EntityStatusType entities = EntityStatusType.Enabled,
                                                                QueryMode mode = QueryMode.Strict,
                                                                ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Components<T3>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Components<T4>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchAdd{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchAdd<T1, T2, T3, T4, T5>(EntityStatusType entities = EntityStatusType.Enabled,
                                                                    QueryMode mode = QueryMode.Strict,
                                                                    ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Components<T3>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Components<T4>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Components<T5>.Instance.BatchAdd(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <summary>
            /// Adds components or tags to all entities matching this query's filter in a single batch.
            /// For components: If an entity already has the component, the old value's OnDelete is called, the value is overwritten,
            /// and OnAdd is called on the new value. For newly added components, only OnAdd is triggered.
            /// Returns <c>this</c> for method chaining.
            /// <para>Available with 1–5 type parameters.</para>
            /// </summary>
            /// <param name="entities">Which entity lifecycle state to target.</param>
            /// <param name="mode">Query mode: Strict (default) blocks hooks from modifying filtered types on non-current entities; Flexible allows free modification.</param>
            /// <param name="clusters">Optional cluster filter.</param>
            /// <returns>This query instance for chaining.</returns>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchSet<T1>(T1 value1 = default,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    QueryMode mode = QueryMode.Strict,
                                                    ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchSet(queryData, firstGlobalBlockIdx, value1);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchSet{T1}(T1, EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchSet<T1, T2>(T1 value1 = default, T2 value2 = default,
                                                        EntityStatusType entities = EntityStatusType.Enabled,
                                                        QueryMode mode = QueryMode.Strict,
                                                        ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchSet(queryData, firstGlobalBlockIdx, value1);
                    Components<T2>.Instance.BatchSet(queryData, firstGlobalBlockIdx, value2);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchSet{T1}(T1, EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchSet<T1, T2, T3>(T1 value1 = default, T2 value2 = default, T3 value3 = default,
                                                            EntityStatusType entities = EntityStatusType.Enabled,
                                                            QueryMode mode = QueryMode.Strict,
                                                            ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchSet(queryData, firstGlobalBlockIdx, value1);
                    Components<T2>.Instance.BatchSet(queryData, firstGlobalBlockIdx, value2);
                    Components<T3>.Instance.BatchSet(queryData, firstGlobalBlockIdx, value3);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchSet{T1}(T1, EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchSet<T1, T2, T3, T4>(T1 value1 = default, T2 value2 = default, T3 value3 = default, T4 value4 = default,
                                                                EntityStatusType entities = EntityStatusType.Enabled,
                                                                QueryMode mode = QueryMode.Strict,
                                                                ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag
                where T4 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchSet(queryData, firstGlobalBlockIdx, value1);
                    Components<T2>.Instance.BatchSet(queryData, firstGlobalBlockIdx, value2);
                    Components<T3>.Instance.BatchSet(queryData, firstGlobalBlockIdx, value3);
                    Components<T4>.Instance.BatchSet(queryData, firstGlobalBlockIdx, value4);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchSet{T1}(T1, EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchSet<T1, T2, T3, T4, T5>(T1 value1 = default, T2 value2 = default, T3 value3 = default, T4 value4 = default, T5 value5 = default,
                                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                                    QueryMode mode = QueryMode.Strict,
                                                                    ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag
                where T4 : struct, IComponentOrTag
                where T5 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchSet(queryData, firstGlobalBlockIdx, value1);
                    Components<T2>.Instance.BatchSet(queryData, firstGlobalBlockIdx, value2);
                    Components<T3>.Instance.BatchSet(queryData, firstGlobalBlockIdx, value3);
                    Components<T4>.Instance.BatchSet(queryData, firstGlobalBlockIdx, value4);
                    Components<T5>.Instance.BatchSet(queryData, firstGlobalBlockIdx, value5);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <summary>
            /// Removes components from all entities matching this query's filter in a single batch.
            /// OnDelete hooks are triggered for each entity that had the component.
            /// Returns <c>this</c> for method chaining.
            /// <para>Available with 1–5 type parameters.</para>
            /// </summary>
            /// <param name="entities">Which entity lifecycle state to target.</param>
            /// <param name="mode">Query mode: Strict (default) blocks hooks from modifying filtered types on non-current entities; Flexible allows free modification.</param>
            /// <param name="clusters">Optional cluster filter.</param>
            /// <returns>This query instance for chaining.</returns>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchDelete<T1>(EntityStatusType entities = EntityStatusType.Enabled,
                                                       QueryMode mode = QueryMode.Strict,

                                                       ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchDelete{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchDelete<T1>(EntityStatusType entities, ReadOnlySpan<uint> chunks,
                                                       QueryMode mode = QueryMode.Strict)
                where T1 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, chunks, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchDelete{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchDelete<T1, T2>(EntityStatusType entities = EntityStatusType.Enabled,
                                                           QueryMode mode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchDelete{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchDelete<T1, T2, T3>(EntityStatusType entities = EntityStatusType.Enabled,
                                                               QueryMode mode = QueryMode.Strict,
                                                               ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Components<T3>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchDelete{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchDelete<T1, T2, T3, T4>(EntityStatusType entities = EntityStatusType.Enabled,
                                                                   QueryMode mode = QueryMode.Strict,

                                                                   ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag
                where T4 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Components<T3>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Components<T4>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchDelete{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchDelete<T1, T2, T3, T4, T5>(EntityStatusType entities = EntityStatusType.Enabled,
                                                                       QueryMode mode = QueryMode.Strict,

                                                                       ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag
                where T4 : struct, IComponentOrTag
                where T5 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Components<T3>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Components<T4>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Components<T5>.Instance.BatchDelete(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <summary>
            /// Disables components on all entities matching this query's filter in a single batch.
            /// Returns <c>this</c> for method chaining.
            /// <para>Available with 1–5 type parameters.</para>
            /// </summary>
            /// <param name="entities">Which entity lifecycle state to target.</param>
            /// <param name="mode">Query mode: Strict (default) blocks hooks from modifying filtered types on non-current entities; Flexible allows free modification.</param>
            /// <param name="clusters">Optional cluster filter.</param>
            /// <returns>This query instance for chaining.</returns>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchDisable<T1>(EntityStatusType entities = EntityStatusType.Enabled,
                                                        QueryMode mode = QueryMode.Strict,
                                                        ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchDisable{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchDisable<T1>(EntityStatusType entities, ReadOnlySpan<uint> chunks,
                                                        QueryMode mode = QueryMode.Strict)
                where T1 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, chunks, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchDisable{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchDisable<T1, T2>(EntityStatusType entities = EntityStatusType.Enabled,
                                                            QueryMode mode = QueryMode.Strict,
                                                            ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponent
                where T2 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchDisable{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchDisable<T1, T2, T3>(EntityStatusType entities = EntityStatusType.Enabled,
                                                                QueryMode mode = QueryMode.Strict,
                                                                ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Components<T3>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchDisable{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchDisable<T1, T2, T3, T4>(EntityStatusType entities = EntityStatusType.Enabled,
                                                                    QueryMode mode = QueryMode.Strict,
                                                                    ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Components<T3>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Components<T4>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchDisable{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchDisable<T1, T2, T3, T4, T5>(EntityStatusType entities = EntityStatusType.Enabled,
                                                                        QueryMode mode = QueryMode.Strict,

                                                                        ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Components<T3>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Components<T4>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Components<T5>.Instance.BatchDisable(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <summary>
            /// Re-enables previously disabled components on all entities matching this query's filter in a single batch.
            /// After enabling, components become visible to standard query iteration again.
            /// Returns <c>this</c> for method chaining.
            /// <para>Available with 1–5 type parameters.</para>
            /// </summary>
            /// <param name="entities">Which entity lifecycle state to target.</param>
            /// <param name="mode">Query mode: Strict (default) blocks hooks from modifying filtered types on non-current entities; Flexible allows free modification.</param>
            /// <param name="clusters">Optional cluster filter.</param>
            /// <returns>This query instance for chaining.</returns>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchEnable<T1>(EntityStatusType entities = EntityStatusType.Enabled,
                                                       QueryMode mode = QueryMode.Strict,

                                                       ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchEnable{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchEnable<T1>(EntityStatusType entities, ReadOnlySpan<uint> chunks,
                                                       QueryMode mode = QueryMode.Strict)
                where T1 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, chunks, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchEnable{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchEnable<T1, T2>(EntityStatusType entities = EntityStatusType.Enabled,
                                                           QueryMode mode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponent
                where T2 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchEnable{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchEnable<T1, T2, T3>(EntityStatusType entities = EntityStatusType.Enabled,
                                                               QueryMode mode = QueryMode.Strict,
                                                               ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Components<T3>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchEnable{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchEnable<T1, T2, T3, T4>(EntityStatusType entities = EntityStatusType.Enabled,
                                                                   QueryMode mode = QueryMode.Strict,

                                                                   ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Components<T3>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Components<T4>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }

            /// <inheritdoc cref="BatchEnable{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchEnable<T1, T2, T3, T4, T5>(EntityStatusType entities = EntityStatusType.Enabled,
                                                                       QueryMode mode = QueryMode.Strict,

                                                                       ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Components<T3>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Components<T4>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Components<T5>.Instance.BatchEnable(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }

                return this;
            }


            #region BATCH TAG OPERATIONS
            /// <summary>
            /// Toggles tags on all entities matching this query's filter in a single batch:
            /// sets the tag if absent, removes it if present. Returns <c>this</c> for method chaining.
            /// <para>Available with 1–5 type parameters.</para>
            /// </summary>
            /// <param name="entities">Which entity lifecycle state to target.</param>
            /// <param name="mode">Query mode: Strict (default) blocks hooks from modifying filtered types on non-current entities; Flexible allows free modification.</param>
            /// <param name="clusters">Optional cluster filter.</param>
            /// <returns>This query instance for chaining.</returns>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchToggle<T1>(EntityStatusType entities = EntityStatusType.Enabled,
                                                       QueryMode mode = QueryMode.Strict,
                                                       ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }
                return this;
            }
            
            /// <inheritdoc cref="BatchToggle{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchToggle<T1>(EntityStatusType entities, ReadOnlySpan<uint> chunks,
                                                       QueryMode mode = QueryMode.Strict)
                where T1 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, chunks, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }
                return this;
            }

            /// <inheritdoc cref="BatchToggle{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchToggle<T1, T2>(EntityStatusType entities = EntityStatusType.Enabled,
                                                           QueryMode mode = QueryMode.Strict,
                                                           ReadOnlySpan<ushort> clusters = default) 
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }
                return this;
            }

            /// <inheritdoc cref="BatchToggle{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchToggle<T1, T2, T3>(EntityStatusType entities = EntityStatusType.Enabled,
                                                               QueryMode mode = QueryMode.Strict,
                                                               ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Components<T3>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }
                return this;
            }
            
            /// <inheritdoc cref="BatchToggle{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchToggle<T1, T2, T3, T4>(EntityStatusType entities = EntityStatusType.Enabled,
                                                                   QueryMode mode = QueryMode.Strict,
                                                                   ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag
                where T4 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Components<T3>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Components<T4>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }
                return this;
            }

            /// <inheritdoc cref="BatchToggle{T1}(EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchToggle<T1, T2, T3, T4, T5>(EntityStatusType entities = EntityStatusType.Enabled,
                                                                       QueryMode mode = QueryMode.Strict,
                                                                       ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag
                where T4 : struct, IComponentOrTag
                where T5 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Components<T2>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Components<T3>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Components<T4>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Components<T5>.Instance.BatchToggle(queryData, firstGlobalBlockIdx);
                    Dispose(Filter, entities, strict, queryData);
                }
                return this;
            }

            /// <summary>
            /// Conditionally sets or removes tags on all entities matching this query's filter based on boolean flags.
            /// Each tag type gets its own state parameter: <c>true</c> sets the tag, <c>false</c> removes it.
            /// Returns <c>this</c> for method chaining.
            /// <para>Available with 1–5 type parameters.</para>
            /// </summary>
            /// <param name="entities">Which entity lifecycle state to target.</param>
            /// <param name="mode">Query mode: Strict (default) blocks hooks from modifying filtered types on non-current entities; Flexible allows free modification.</param>
            /// <param name="clusters">Optional cluster filter.</param>
            /// <returns>This query instance for chaining.</returns>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchApply<T1>(bool state1,
                                                      EntityStatusType entities = EntityStatusType.Enabled,
                                                      QueryMode mode = QueryMode.Strict,
                                                      ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state1);
                    Dispose(Filter, entities, strict, queryData);
                }
                return this;
            }
            
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchApply<T1>(bool state1,
                                                      EntityStatusType entities, ReadOnlySpan<uint> chunks,
                                                      QueryMode mode = QueryMode.Strict)
                where T1 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, chunks, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state1);
                    Dispose(Filter, entities, strict, queryData);
                }
                return this;
            }

            /// <inheritdoc cref="BatchApply{T1}(bool, EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchApply<T1, T2>(bool state1, bool state2,
                                                          EntityStatusType entities = EntityStatusType.Enabled,
                                                          QueryMode mode = QueryMode.Strict,
                                                          ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag 
                where T2 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state1);
                    Components<T2>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state2);
                    Dispose(Filter, entities, strict, queryData);
                }
                return this;
            }

            /// <inheritdoc cref="BatchApply{T1}(bool, EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchApply<T1, T2, T3>(bool state1, bool state2, bool state3,
                                                              EntityStatusType entities = EntityStatusType.Enabled,
                                                              QueryMode mode = QueryMode.Strict,
                                                              ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state1);
                    Components<T2>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state2);
                    Components<T3>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state3);
                    Dispose(Filter, entities, strict, queryData);
                }
                return this;
            }

            /// <inheritdoc cref="BatchApply{T1}(bool, EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchApply<T1, T2, T3, T4>(bool state1, bool state2, bool state3, bool state4,
                                                                  EntityStatusType entities = EntityStatusType.Enabled,
                                                                  QueryMode mode = QueryMode.Strict,
                                                                  ReadOnlySpan<ushort> clusters = default) 
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag
                where T4 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state1);
                    Components<T2>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state2);
                    Components<T3>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state3);
                    Components<T4>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state4);
                    Dispose(Filter, entities, strict, queryData);
                }
                return this;
            }

            /// <inheritdoc cref="BatchApply{T1}(bool, EntityStatusType, QueryMode, ReadOnlySpan{ushort})"/>
            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Type metadata is preserved by the registration path.")]
            #endif
            [MethodImpl(AggressiveInlining)]
            public WorldQuery<TFilter> BatchApply<T1, T2, T3, T4, T5>(bool state1, bool state2, bool state3, bool state4, bool state5,
                                                                      EntityStatusType entities = EntityStatusType.Enabled,
                                                                      QueryMode mode = QueryMode.Strict,
                                                                      ReadOnlySpan<ushort> clusters = default)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag
                where T4 : struct, IComponentOrTag
                where T5 : struct, IComponentOrTag {
                var strict = mode == QueryMode.Strict;
                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    Components<T1>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state1);
                    Components<T2>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state2);
                    Components<T3>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state3);
                    Components<T4>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state4);
                    Components<T5>.Instance.BatchApply(queryData, firstGlobalBlockIdx, state5);
                    Dispose(Filter, entities, strict, queryData);
                }
                return this;
            }
            #endregion

            [MethodImpl(AggressiveInlining)]
            internal void WriteEntitySnapshotData(ref BinaryPackWriter writer,
                                                  CustomSnapshotEntityDataWriter<TWorld> snapshotDataEntityWriter,
                                                  SnapshotWriteParams snapshotParams,
                                                  ReadOnlySpan<uint> chunks,
                                                  EntityStatusType entities) {
                if (Prepare(Filter, chunks, entities, true, out var queryData, out var firstGlobalBlockIdx)) {
                    var entity = new Entity();
                    ref var eid = ref entity.IdWithOffset;

                    #if !NET6_0_OR_GREATER
                    var deBruijn = Utils.DeBruijn;
                    #endif

                    while (firstGlobalBlockIdx >= 0) {
                        ref var cache = ref queryData.Blocks[firstGlobalBlockIdx];
                        ref var entitiesMask = ref cache.EntitiesMask;
                        var mask = entitiesMask;
                        var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT) + Const.ENTITY_ID_OFFSET;
                        firstGlobalBlockIdx = cache.NextGlobalBlock;

                        var runStarts = mask & ~(mask << 1);
                        var runEnds = mask & ~(mask >> 1);
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
                            eid = chunkBlockEntityId + runStart;
                            var eidEnd = chunkBlockEntityId + runEnd;
                            while (eid <= eidEnd) {
                                snapshotDataEntityWriter(ref writer, entity, snapshotParams);
                                eid++;
                            }
                        } while (runStarts != 0);
                    }
                    Dispose(Filter, entities, true, queryData);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void ReadEntitySnapshotData(ref BinaryPackReader reader,
                                                 CustomSnapshotEntityDataReader<TWorld> snapshotDataEntityReader,
                                                 ushort version,
                                                 SnapshotReadParams snapshotParams,
                                                 ReadOnlySpan<uint> chunks,
                                                 EntityStatusType entities) {
                if (Prepare(Filter, chunks, entities, true, out var queryData, out var firstGlobalBlockIdx)) {
                    var entity = new Entity();
                    ref var eid = ref entity.IdWithOffset;

                    #if !NET6_0_OR_GREATER
                    var deBruijn = Utils.DeBruijn;
                    #endif

                    while (firstGlobalBlockIdx >= 0) {
                        ref var cache = ref queryData.Blocks[firstGlobalBlockIdx];
                        ref var entitiesMask = ref cache.EntitiesMask;
                        var mask = entitiesMask;
                        var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT) + Const.ENTITY_ID_OFFSET;
                        firstGlobalBlockIdx = cache.NextGlobalBlock;

                        var runStarts = mask & ~(mask << 1);
                        var runEnds = mask & ~(mask >> 1);
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
                            eid = chunkBlockEntityId + runStart;
                            var eidEnd = chunkBlockEntityId + runEnd;
                            while (eid <= eidEnd) {
                                snapshotDataEntityReader(ref reader, entity, version, snapshotParams);
                                eid++;
                            }
                        } while (runStarts != 0);
                    }
                    Dispose(Filter, entities, true, queryData);
                }
            }
        }
    }
}