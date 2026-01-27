#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Diagnostics.CodeAnalysis;
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
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public readonly ref partial struct WorldQuery<TFilter> where TFilter : struct, IQueryFilter {

            #region DELEGATE SEARCH
            /// <summary>
            /// Searches for the first entity matching <typeparamref name="TFilter"/>,
            /// invoking <paramref name="function"/> for each one until it returns <c>true</c>.
            /// No component data is accessed — only entity identity is provided to the predicate.
            /// </summary>
            /// <param name="entity">When the method returns <c>true</c>, contains the entity for which the predicate matched; otherwise, <c>default</c>.</param>
            /// <param name="function">Predicate delegate receiving the entity. Return <c>true</c> to stop and select the entity.</param>
            /// <param name="entities">Filter by entity status.</param>
            /// <param name="queryMode"><see cref="QueryMode.Strict"/> (default) for fastest iteration; <see cref="QueryMode.Flexible"/> allows modifying filtered types on other entities.</param>
            /// <param name="clusters">Optional cluster filter. When empty, uses the world's active clusters.</param>
            /// <returns><c>true</c> if an entity was found (written to <paramref name="entity"/>); <c>false</c> if no entity matched.</returns>
            [MethodImpl(AggressiveInlining)]
            public bool Search(out Entity entity,
                                   SearchFunctionWithEntity<TWorld> function,
                                   EntityStatusType entities = EntityStatusType.Enabled,
                                   QueryMode queryMode = QueryMode.Strict,
                                   ReadOnlySpan<ushort> clusters = default) {
                ref var world = ref Data.Instance;
                var strict = queryMode == QueryMode.Strict;

                var result = false;
                entity = new Entity();
                ref var entityId = ref entity.IdWithOffset;

                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {

                        #if !NET6_0_OR_GREATER
                        var deBruijn = Utils.DeBruijn;
                        #endif

                        var blocks = queryData.Blocks;

                        do {
                            var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT);
                            ref var block = ref blocks[firstGlobalBlockIdx];
                            ref var entitiesMaskRef = ref block.EntitiesMask;
                            firstGlobalBlockIdx = block.NextGlobalBlock;
                            var entitiesMask = entitiesMaskRef;
                            #if NET6_0_OR_GREATER
                            var componentOffset = (int)(chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK);
                            #else
                            var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                            #endif
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            if (strict) {
                                if (entitiesMask == ulong.MaxValue) {
                                    var componentEnd = componentOffset + Const.U64_BITS;
                                    entityId = chunkBlockEntityId;
                                    while (componentOffset < componentEnd) {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        if (function.Invoke(entity)) {
                                            result = true;
                                            goto EXIT;
                                        }

                                        componentOffset++;
                                        entityId++;
                                    }
                                }
                                else {
                                    var runStarts = entitiesMask & ~(entitiesMask << 1);
                                    var runEnds = entitiesMask & ~(entitiesMask >> 1);
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
                                        var componentIdx = runStart + componentOffset;
                                        var componentEnd = runEnd + componentOffset;
                                        entityId = chunkBlockEntityId + runStart;
                                        while (componentIdx <= componentEnd) {
                                            #if FFS_ECS_DEBUG
                                            world.SetCurrentQueryEntity(entityId);
                                            #endif
                                            if (function.Invoke(entity)) {
                                                result = true;
                                                goto EXIT;
                                            }

                                            componentIdx++;
                                            entityId++;
                                        }
                                    } while (runStarts != 0);
                                }
                            }
                            else {
                                do {
                                    var isolatedBit = entitiesMask & (ulong)-(long)entitiesMask;
                                    #if NET6_0_OR_GREATER
                                    var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                                    #else
                                    var runStart = deBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                                    #endif

                                    entityId = chunkBlockEntityId + runStart;

                                    do {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        if (function.Invoke(entity)) {
                                            result = true;
                                            goto EXIT;
                                        }

                                        isolatedBit <<= 1;
                                        entityId++;
                                    } while ((entitiesMaskRef & isolatedBit) != 0);

                                    entitiesMask = entitiesMaskRef & ~(isolatedBit - 1);
                                } while (entitiesMask != 0);
                            }
                        } while (firstGlobalBlockIdx >= 0);

                        EXIT: ;
                    }

                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        Dispose(Filter, entities, strict, queryData);
                    }
                }

                return result;
            }
            #endregion

            #region DELEGATE WITH DATA AND ENTITY
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TData>(TData userData,
                                   QueryFunctionWithRefDataEntity<TData, TWorld> function,
                                   EntityStatusType entities = EntityStatusType.Enabled,
                                   QueryMode queryMode = QueryMode.Strict,
                                   ReadOnlySpan<ushort> clusters = default)
                where TData : struct {
                For(ref userData, function, entities, queryMode, clusters);
            }

            /// <summary>
            /// Iterates over all entities matching <typeparamref name="TFilter"/>,
            /// invoking <paramref name="function"/> with a ref to user data and the entity.
            /// No component data is accessed — only entity identity is provided.
            /// </summary>
            /// <typeparam name="TData">Type of user data passed by ref to each invocation.</typeparam>
            /// <param name="userData">User data passed by ref. Modifications persist after the query completes.</param>
            /// <param name="function">Delegate receiving <c>(ref TData, Entity)</c> for each matching entity.</param>
            /// <param name="entities">Filter by entity status.</param>
            /// <param name="queryMode"><see cref="QueryMode.Strict"/> (default) for fastest iteration; <see cref="QueryMode.Flexible"/> allows modifying filtered types on other entities.</param>
            /// <param name="clusters">Optional cluster filter. When empty, uses the world's active clusters.</param>
            [MethodImpl(AggressiveInlining)]
            public void For<TData>(ref TData userData,
                                   QueryFunctionWithRefDataEntity<TData, TWorld> function,
                                   EntityStatusType entities = EntityStatusType.Enabled,
                                   QueryMode queryMode = QueryMode.Strict,
                                   ReadOnlySpan<ushort> clusters = default) {
                ref var world = ref Data.Instance;
                var strict = queryMode == QueryMode.Strict;

                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {

                        #if !NET6_0_OR_GREATER
                        var deBruijn = Utils.DeBruijn;
                        #endif

                        var blocks = queryData.Blocks;
                        var segmentIdx = uint.MaxValue;
                        var entity = new Entity();
                        ref var entityId = ref entity.IdWithOffset;

                        do {
                            var curSegmentIdx = firstGlobalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                            if (curSegmentIdx != segmentIdx) {
                                segmentIdx = (uint)curSegmentIdx;
                            }

                            var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT);
                            ref var block = ref blocks[firstGlobalBlockIdx];
                            ref var entitiesMaskRef = ref block.EntitiesMask;
                            firstGlobalBlockIdx = block.NextGlobalBlock;
                            var entitiesMask = entitiesMaskRef;
                            #if NET6_0_OR_GREATER
                            var componentOffset = (int)(chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK);
                            #else
                            var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                            #endif
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            if (strict) {
                                if (entitiesMask == ulong.MaxValue) {
                                    var componentEnd = componentOffset + Const.U64_BITS;
                                    entityId = chunkBlockEntityId;
                                    while (componentOffset < componentEnd) {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        function.Invoke(
                                            ref userData,
                                            entity
                                        );
                                        componentOffset++;
                                        entityId++;
                                    }
                                }
                                else {
                                    var runStarts = entitiesMask & ~(entitiesMask << 1);
                                    var runEnds = entitiesMask & ~(entitiesMask >> 1);
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
                                        var componentIdx = runStart + componentOffset;
                                        var componentEnd = runEnd + componentOffset;
                                        entityId = chunkBlockEntityId + runStart;
                                        while (componentIdx <= componentEnd) {
                                            #if FFS_ECS_DEBUG
                                            world.SetCurrentQueryEntity(entityId);
                                            #endif
                                            function.Invoke(
                                                ref userData,
                                                entity
                                            );
                                            componentIdx++;
                                            entityId++;
                                        }
                                    } while (runStarts != 0);
                                }
                            }
                            else {
                                do {
                                    var isolatedBit = entitiesMask & (ulong)-(long)entitiesMask;
                                    #if NET6_0_OR_GREATER
                                    var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                                    #else
                                    var runStart = deBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                                    #endif

                                    entityId = chunkBlockEntityId + runStart;

                                    do {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        function.Invoke(
                                            ref userData,
                                            entity
                                        );

                                        isolatedBit <<= 1;
                                        entityId++;
                                    } while ((entitiesMaskRef & isolatedBit) != 0);

                                    entitiesMask = entitiesMaskRef & ~(isolatedBit - 1);
                                } while (entitiesMask != 0);
                            }
                        } while (firstGlobalBlockIdx >= 0);
                    }

                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        Dispose(Filter, entities, strict, queryData);
                    }
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void For<TData>(TData userData,
                                     ReadOnlySpan<uint> chunks,
                                     QueryFunctionWithRefDataEntity<TData, TWorld> function,
                                     EntityStatusType entities = EntityStatusType.Enabled,
                                     QueryMode queryMode = QueryMode.Strict)
                where TData : struct {
                For(ref userData, chunks, function, entities, queryMode);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void For<TData>(ref TData userData,
                                   ReadOnlySpan<uint> chunks,
                                   QueryFunctionWithRefDataEntity<TData, TWorld> function,
                                   EntityStatusType entities = EntityStatusType.Enabled,
                                   QueryMode queryMode = QueryMode.Strict) {
                ref var world = ref Data.Instance;
                var strict = queryMode == QueryMode.Strict;

                if (Prepare(Filter, chunks, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {

                        #if !NET6_0_OR_GREATER
                        var deBruijn = Utils.DeBruijn;
                        #endif

                        var blocks = queryData.Blocks;
                        var segmentIdx = uint.MaxValue;
                        var entity = new Entity();
                        ref var entityId = ref entity.IdWithOffset;

                        do {
                            var curSegmentIdx = firstGlobalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                            if (curSegmentIdx != segmentIdx) {
                                segmentIdx = (uint)curSegmentIdx;
                            }

                            var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT);
                            ref var block = ref blocks[firstGlobalBlockIdx];
                            ref var entitiesMaskRef = ref block.EntitiesMask;
                            firstGlobalBlockIdx = block.NextGlobalBlock;
                            var entitiesMask = entitiesMaskRef;
                            #if NET6_0_OR_GREATER
                            var componentOffset = (int)(chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK);
                            #else
                            var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                            #endif
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            if (strict) {
                                if (entitiesMask == ulong.MaxValue) {
                                    var componentEnd = componentOffset + Const.U64_BITS;
                                    entityId = chunkBlockEntityId;
                                    while (componentOffset < componentEnd) {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        function.Invoke(
                                            ref userData,
                                            entity
                                        );
                                        componentOffset++;
                                        entityId++;
                                    }
                                }
                                else {
                                    var runStarts = entitiesMask & ~(entitiesMask << 1);
                                    var runEnds = entitiesMask & ~(entitiesMask >> 1);
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
                                        var componentIdx = runStart + componentOffset;
                                        var componentEnd = runEnd + componentOffset;
                                        entityId = chunkBlockEntityId + runStart;
                                        while (componentIdx <= componentEnd) {
                                            #if FFS_ECS_DEBUG
                                            world.SetCurrentQueryEntity(entityId);
                                            #endif
                                            function.Invoke(
                                                ref userData,
                                                entity
                                            );
                                            componentIdx++;
                                            entityId++;
                                        }
                                    } while (runStarts != 0);
                                }
                            }
                            else {
                                do {
                                    var isolatedBit = entitiesMask & (ulong)-(long)entitiesMask;
                                    #if NET6_0_OR_GREATER
                                    var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                                    #else
                                    var runStart = deBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                                    #endif

                                    entityId = chunkBlockEntityId + runStart;

                                    do {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        function.Invoke(
                                            ref userData,
                                            entity
                                        );

                                        isolatedBit <<= 1;
                                        entityId++;
                                    } while ((entitiesMaskRef & isolatedBit) != 0);

                                    entitiesMask = entitiesMaskRef & ~(isolatedBit - 1);
                                } while (entitiesMask != 0);
                            }
                        } while (firstGlobalBlockIdx >= 0);
                    }

                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        Dispose(Filter, entities, strict, queryData);
                    }
                }
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="userData"/> by value.
            /// <para>See the <c>ref TData</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData>(TData userData,
                                           QueryFunctionWithRefDataEntity<TData, TWorld> function,
                                           EntityStatusType entities = EntityStatusType.Enabled,
                                           ReadOnlySpan<ushort> clusters = default,
                                           uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                           uint workersLimit = 0)
                where TData : struct {
                ForParallel(ref userData, function, entities, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <summary>
            /// Parallel version of the <c>For</c> overload accepting <c>(ref TData, QueryFunctionWithRefDataEntity)</c>.
            /// Distributes matching entities across worker threads via <see cref="ParallelRunner{TWorld}"/>.
            /// Always uses <see cref="QueryMode.Strict"/> semantics.
            /// </summary>
            /// <typeparam name="TData">Type of user data passed by ref to each invocation.</typeparam>
            /// <param name="userData">User data shared across workers. Updated after all workers complete.</param>
            /// <param name="function">Delegate receiving <c>(ref TData, Entity)</c> for each matching entity.</param>
            /// <param name="entities">Filter by entity status.</param>
            /// <param name="clusters">Optional cluster filter. When empty, uses the world's active clusters.</param>
            /// <param name="minEntitiesPerThread">Minimum entities per worker thread. Defaults to <see cref="Const.ENTITIES_IN_SEGMENT"/> (256).</param>
            /// <param name="workersLimit">Maximum number of worker threads. 0 = no limit.</param>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TData>(ref TData userData,
                                           QueryFunctionWithRefDataEntity<TData, TWorld> function,
                                           EntityStatusType entities = EntityStatusType.Enabled,
                                           ReadOnlySpan<ushort> clusters = default,
                                           uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                           uint workersLimit = 0) {
                if (PrepareParallel(Filter, clusters, entities, out var count, out var jobs, out var jobIndexes)) {
                    Resources<ParallelData<QueryFunctionWithRefDataEntity<TData, TWorld>, TData>>.Value = new(function, userData);
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ParallelFunctionWithRefDataEntity<TData>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        userData = Resources<ParallelData<QueryFunctionWithRefDataEntity<TData, TWorld>, TData>>.Value.Value2;
                        Resources<ParallelData<QueryFunctionWithRefDataEntity<TData, TWorld>, TData>>.Value = default;
                    }
                }
            }
            
            internal static unsafe void ParallelFunctionWithRefDataEntity<TData>(Job[] jobs, uint[] jobIndexes, uint from, uint to, int worker) {
                ref var world = ref Data.Instance;

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                var function = Resources<ParallelData<QueryFunctionWithRefDataEntity<TData, TWorld>, TData>>.Value.Value1;
                ref var data = ref Resources<ParallelData<QueryFunctionWithRefDataEntity<TData, TWorld>, TData>>.Value.Value2;
                var entity = new Entity();
                ref var entityId = ref entity.IdWithOffset;

                while (from < to) {
                    ref var job = ref jobs[jobIndexes[from++]];
                    var count = job.Count;
                    job.Count = 0;

                    for (uint i = 0; i < count; i++) {
                        var chunkBlockEntityId = job.GlobalBlockIdx[i] << Const.ENTITIES_IN_BLOCK_SHIFT;
                        var entitiesMask = job.Masks[i];
                        #if NET6_0_OR_GREATER
                        var componentOffset = (int)(chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK);
                        #else
                        var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                        #endif
                        chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                        if (entitiesMask == ulong.MaxValue) {
                            var componentEnd = componentOffset + Const.U64_BITS;
                            entityId = chunkBlockEntityId;
                            while (componentOffset < componentEnd) {
                                #if FFS_ECS_DEBUG
                                world.SetCurrentQueryEntity(entityId);
                                #endif
                                function.Invoke(
                                    ref data,
                                    entity
                                );
                                componentOffset++;
                                entityId++;
                            }
                        }
                        else {
                            var runStarts = entitiesMask & ~(entitiesMask << 1);
                            var runEnds = entitiesMask & ~(entitiesMask >> 1);
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
                                var componentIdx = runStart + componentOffset;
                                var componentEnd = runEnd + componentOffset;
                                entityId = chunkBlockEntityId + runStart;
                                while (componentIdx <= componentEnd) {
                                    #if FFS_ECS_DEBUG
                                    world.SetCurrentQueryEntity(entityId);
                                    #endif
                                    function.Invoke(
                                        ref data,
                                        entity
                                    );
                                    componentIdx++;
                                    entityId++;
                                }
                            } while (runStarts != 0);
                        }
                    }
                }
            }
            #endregion

            #region DELEGATE WITH ENTITY
            /// <summary>
            /// Iterates over all entities matching <typeparamref name="TFilter"/>,
            /// invoking <paramref name="function"/> with the entity handle.
            /// No component data is accessed — only entity identity is provided.
            /// </summary>
            /// <param name="function">Delegate receiving <c>(Entity)</c> for each matching entity.</param>
            /// <param name="entities">Filter by entity status.</param>
            /// <param name="components">Filter by component status.</param>
            /// <param name="queryMode"><see cref="QueryMode.Strict"/> (default) for fastest iteration; <see cref="QueryMode.Flexible"/> allows modifying filtered types on other entities.</param>
            /// <param name="clusters">Optional cluster filter. When empty, uses the world's active clusters.</param>
            [MethodImpl(AggressiveInlining)]
            public void For(QueryFunctionWithEntity<TWorld> function,
                            EntityStatusType entities = EntityStatusType.Enabled,
                            ComponentStatus components = ComponentStatus.Enabled,
                            QueryMode queryMode = QueryMode.Strict,
                            ReadOnlySpan<ushort> clusters = default) {
                ref var world = ref Data.Instance;
                var strict = queryMode == QueryMode.Strict;

                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {

                        #if !NET6_0_OR_GREATER
                        var deBruijn = Utils.DeBruijn;
                        #endif

                        var blocks = queryData.Blocks;
                        var segmentIdx = uint.MaxValue;
                        var entity = new Entity();
                        ref var entityId = ref entity.IdWithOffset;

                        do {
                            var curSegmentIdx = firstGlobalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                            if (curSegmentIdx != segmentIdx) {
                                segmentIdx = (uint)curSegmentIdx;
                            }

                            var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT);
                            ref var block = ref blocks[firstGlobalBlockIdx];
                            ref var entitiesMaskRef = ref block.EntitiesMask;
                            firstGlobalBlockIdx = block.NextGlobalBlock;
                            var entitiesMask = entitiesMaskRef;
                            #if NET6_0_OR_GREATER
                            var componentOffset = (int)(chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK);
                            #else
                            var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                            #endif
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            if (strict) {
                                if (entitiesMask == ulong.MaxValue) {
                                    var componentEnd = componentOffset + Const.U64_BITS;
                                    entityId = chunkBlockEntityId;
                                    while (componentOffset < componentEnd) {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        function.Invoke(
                                            entity
                                        );
                                        componentOffset++;
                                        entityId++;
                                    }
                                }
                                else {
                                    var runStarts = entitiesMask & ~(entitiesMask << 1);
                                    var runEnds = entitiesMask & ~(entitiesMask >> 1);
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
                                        var componentIdx = runStart + componentOffset;
                                        var componentEnd = runEnd + componentOffset;
                                        entityId = chunkBlockEntityId + runStart;
                                        while (componentIdx <= componentEnd) {
                                            #if FFS_ECS_DEBUG
                                            world.SetCurrentQueryEntity(entityId);
                                            #endif
                                            function.Invoke(
                                                entity
                                            );
                                            componentIdx++;
                                            entityId++;
                                        }
                                    } while (runStarts != 0);
                                }
                            }
                            else {
                                do {
                                    var isolatedBit = entitiesMask & (ulong)-(long)entitiesMask;
                                    #if NET6_0_OR_GREATER
                                    var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                                    #else
                                    var runStart = deBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                                    #endif

                                    entityId = chunkBlockEntityId + runStart;

                                    do {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        function.Invoke(
                                            entity
                                        );

                                        isolatedBit <<= 1;
                                        entityId++;
                                    } while ((entitiesMaskRef & isolatedBit) != 0);

                                    entitiesMask = entitiesMaskRef & ~(isolatedBit - 1);
                                } while (entitiesMask != 0);
                            }
                        } while (firstGlobalBlockIdx >= 0);
                    }

                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        Dispose(Filter, entities, strict, queryData);
                    }
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public void For(ReadOnlySpan<uint> chunks,
                            QueryFunctionWithEntity<TWorld> function,
                            EntityStatusType entities = EntityStatusType.Enabled,
                            QueryMode queryMode = QueryMode.Strict) {
                ref var world = ref Data.Instance;
                var strict = queryMode == QueryMode.Strict;

                if (Prepare(Filter, chunks, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {

                        #if !NET6_0_OR_GREATER
                        var deBruijn = Utils.DeBruijn;
                        #endif

                        var blocks = queryData.Blocks;
                        var segmentIdx = uint.MaxValue;
                        var entity = new Entity();
                        ref var entityId = ref entity.IdWithOffset;

                        do {
                            var curSegmentIdx = firstGlobalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                            if (curSegmentIdx != segmentIdx) {
                                segmentIdx = (uint)curSegmentIdx;
                            }

                            var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT);
                            ref var block = ref blocks[firstGlobalBlockIdx];
                            ref var entitiesMaskRef = ref block.EntitiesMask;
                            firstGlobalBlockIdx = block.NextGlobalBlock;
                            var entitiesMask = entitiesMaskRef;
                            #if NET6_0_OR_GREATER
                            var componentOffset = (int)(chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK);
                            #else
                            var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                            #endif
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            if (strict) {
                                if (entitiesMask == ulong.MaxValue) {
                                    var componentEnd = componentOffset + Const.U64_BITS;
                                    entityId = chunkBlockEntityId;
                                    while (componentOffset < componentEnd) {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        function.Invoke(
                                            entity
                                        );
                                        componentOffset++;
                                        entityId++;
                                    }
                                }
                                else {
                                    var runStarts = entitiesMask & ~(entitiesMask << 1);
                                    var runEnds = entitiesMask & ~(entitiesMask >> 1);
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
                                        var componentIdx = runStart + componentOffset;
                                        var componentEnd = runEnd + componentOffset;
                                        entityId = chunkBlockEntityId + runStart;
                                        while (componentIdx <= componentEnd) {
                                            #if FFS_ECS_DEBUG
                                            world.SetCurrentQueryEntity(entityId);
                                            #endif
                                            function.Invoke(
                                                entity
                                            );
                                            componentIdx++;
                                            entityId++;
                                        }
                                    } while (runStarts != 0);
                                }
                            }
                            else {
                                do {
                                    var isolatedBit = entitiesMask & (ulong)-(long)entitiesMask;
                                    #if NET6_0_OR_GREATER
                                    var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                                    #else
                                    var runStart = deBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                                    #endif

                                    entityId = chunkBlockEntityId + runStart;

                                    do {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        function.Invoke(
                                            entity
                                        );

                                        isolatedBit <<= 1;
                                        entityId++;
                                    } while ((entitiesMaskRef & isolatedBit) != 0);

                                    entitiesMask = entitiesMaskRef & ~(isolatedBit - 1);
                                } while (entitiesMask != 0);
                            }
                        } while (firstGlobalBlockIdx >= 0);
                    }

                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        Dispose(Filter, entities, strict, queryData);
                    }
                }
            }
            
            /// <summary>
            /// Parallel version of the <c>For</c> overload accepting <c>QueryFunctionWithEntity</c>.
            /// Distributes matching entities across worker threads via <see cref="ParallelRunner{TWorld}"/>.
            /// Always uses <see cref="QueryMode.Strict"/> semantics.
            /// </summary>
            /// <param name="function">Delegate receiving <c>(Entity)</c> for each matching entity.</param>
            /// <param name="entities">Filter by entity status.</param>
            /// <param name="components">Filter by component status.</param>
            /// <param name="clusters">Optional cluster filter. When empty, uses the world's active clusters.</param>
            /// <param name="minEntitiesPerThread">Minimum entities per worker thread. Defaults to <see cref="Const.ENTITIES_IN_SEGMENT"/> (256).</param>
            /// <param name="workersLimit">Maximum number of worker threads. 0 = no limit.</param>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel(QueryFunctionWithEntity<TWorld> function,
                                    EntityStatusType entities = EntityStatusType.Enabled,
                                    ComponentStatus components = ComponentStatus.Enabled,
                                    ReadOnlySpan<ushort> clusters = default,
                                    uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                    uint workersLimit = 0) {
                if (PrepareParallel(Filter, clusters, entities, out var count, out var jobs, out var jobIndexes)) {
                    Resources<ParallelData<QueryFunctionWithEntity<TWorld>>>.Value = new(function);
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ParallelFunctionWithEntity,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        Resources<ParallelData<QueryFunctionWithEntity<TWorld>>>.Value = default;
                    }
                }
            }
            
            internal static unsafe void ParallelFunctionWithEntity(Job[] jobs, uint[] jobIndexes, uint from, uint to, int worker) {
                ref var world = ref Data.Instance;

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                var function = Resources<ParallelData<QueryFunctionWithEntity<TWorld>>>.Value.Value;
                var entity = new Entity();
                ref var entityId = ref entity.IdWithOffset;

                while (from < to) {
                    ref var job = ref jobs[jobIndexes[from++]];
                    var count = job.Count;
                    job.Count = 0;

                    for (uint i = 0; i < count; i++) {
                        var chunkBlockEntityId = job.GlobalBlockIdx[i] << Const.ENTITIES_IN_BLOCK_SHIFT;
                        var entitiesMask = job.Masks[i];
                        #if NET6_0_OR_GREATER
                        var componentOffset = (int)(chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK);
                        #else
                        var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                        #endif
                        chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                        if (entitiesMask == ulong.MaxValue) {
                            var componentEnd = componentOffset + Const.U64_BITS;
                            entityId = chunkBlockEntityId;
                            while (componentOffset < componentEnd) {
                                #if FFS_ECS_DEBUG
                                world.SetCurrentQueryEntity(entityId);
                                #endif
                                function.Invoke(
                                    entity
                                );
                                componentOffset++;
                                entityId++;
                            }
                        }
                        else {
                            var runStarts = entitiesMask & ~(entitiesMask << 1);
                            var runEnds = entitiesMask & ~(entitiesMask >> 1);
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
                                var componentIdx = runStart + componentOffset;
                                var componentEnd = runEnd + componentOffset;
                                entityId = chunkBlockEntityId + runStart;
                                while (componentIdx <= componentEnd) {
                                    #if FFS_ECS_DEBUG
                                    world.SetCurrentQueryEntity(entityId);
                                    #endif
                                    function.Invoke(
                                        entity
                                    );
                                    componentIdx++;
                                    entityId++;
                                }
                            } while (runStarts != 0);
                        }
                    }
                }
            }
            #endregion
            
            #region UNSAFE DELEGATE WITH ENTITY
            /// <summary>
            /// Iterates over all entities matching <typeparamref name="TFilter"/>,
            /// invoking an unmanaged function pointer with the entity handle.
            /// This is the fastest iteration path for entity-only queries — avoids delegate indirection entirely.
            /// </summary>
            /// <param name="function">Unmanaged function pointer receiving <c>(Entity)</c> for each matching entity.</param>
            /// <param name="entities">Filter by entity status.</param>
            /// <param name="queryMode"><see cref="QueryMode.Strict"/> (default) for fastest iteration; <see cref="QueryMode.Flexible"/> allows modifying filtered types on other entities.</param>
            /// <param name="clusters">Optional cluster filter. When empty, uses the world's active clusters.</param>
            [MethodImpl(AggressiveInlining)]
            public unsafe void For(delegate*<Entity, void> function,
                                   EntityStatusType entities = EntityStatusType.Enabled,
                                   QueryMode queryMode = QueryMode.Strict,
                                   ReadOnlySpan<ushort> clusters = default) {
                ref var world = ref Data.Instance;
                var strict = queryMode == QueryMode.Strict;

                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {

                        #if !NET6_0_OR_GREATER
                        var deBruijn = Utils.DeBruijn;
                        #endif

                        var blocks = queryData.Blocks;
                        var segmentIdx = uint.MaxValue;
                        var entity = new Entity();
                        ref var entityId = ref entity.IdWithOffset;

                        do {
                            var curSegmentIdx = firstGlobalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                            if (curSegmentIdx != segmentIdx) {
                                segmentIdx = (uint)curSegmentIdx;
                            }

                            var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT);
                            ref var block = ref blocks[firstGlobalBlockIdx];
                            ref var entitiesMaskRef = ref block.EntitiesMask;
                            firstGlobalBlockIdx = block.NextGlobalBlock;
                            var entitiesMask = entitiesMaskRef;
                            #if NET6_0_OR_GREATER
                            var componentOffset = (int)(chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK);
                            #else
                            var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                            #endif
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            if (strict) {
                                if (entitiesMask == ulong.MaxValue) {
                                    var componentEnd = componentOffset + Const.U64_BITS;
                                    entityId = chunkBlockEntityId;
                                    while (componentOffset < componentEnd) {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        function(
                                            entity
                                        );
                                        componentOffset++;
                                        entityId++;
                                    }
                                }
                                else {
                                    var runStarts = entitiesMask & ~(entitiesMask << 1);
                                    var runEnds = entitiesMask & ~(entitiesMask >> 1);
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
                                        var componentIdx = runStart + componentOffset;
                                        var componentEnd = runEnd + componentOffset;
                                        entityId = chunkBlockEntityId + runStart;
                                        while (componentIdx <= componentEnd) {
                                            #if FFS_ECS_DEBUG
                                            world.SetCurrentQueryEntity(entityId);
                                            #endif
                                            function(
                                                entity
                                            );
                                            componentIdx++;
                                            entityId++;
                                        }
                                    } while (runStarts != 0);
                                }
                            }
                            else {
                                do {
                                    var isolatedBit = entitiesMask & (ulong)-(long)entitiesMask;
                                    #if NET6_0_OR_GREATER
                                    var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                                    #else
                                    var runStart = deBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                                    #endif

                                    entityId = chunkBlockEntityId + runStart;

                                    do {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        function(
                                            entity
                                        );

                                        isolatedBit <<= 1;
                                        entityId++;
                                    } while ((entitiesMaskRef & isolatedBit) != 0);

                                    entitiesMask = entitiesMaskRef & ~(isolatedBit - 1);
                                } while (entitiesMask != 0);
                            }
                        } while (firstGlobalBlockIdx >= 0);
                    }

                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        Dispose(Filter, entities, strict, queryData);
                    }
                }
            }
            #endregion
            
            #region STRUCT FUNCTION BLOCKS
            /// <summary>
            /// Convenience overload that accepts <paramref name="function"/> by value.
            /// <para>See the <c>ref TFunction</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForBlock<TFunction>(TFunction function = default,
                                            EntityStatusType entities = EntityStatusType.Enabled,
                                            ReadOnlySpan<ushort> clusters = default)
                where TFunction : struct, IQueryBlock {
                ForBlock(ref function, entities, clusters);
            }

            /// <summary>
            /// Block-based iteration over all entities matching <typeparamref name="TFilter"/>.
            /// The function receives contiguous runs of entities as <see cref="EntityBlock"/>, enabling batch processing.
            /// No component data is accessed. Always uses <see cref="QueryMode.Strict"/> semantics.
            /// </summary>
            /// <typeparam name="TFunction">Struct implementing <see cref="IQueryBlock"/>.</typeparam>
            /// <param name="function">The block function struct, passed by ref to preserve state across invocations.</param>
            /// <param name="entities">Filter by entity status.</param>
            /// <param name="clusters">Optional cluster filter. When empty, uses the world's active clusters.</param>
            [MethodImpl(AggressiveInlining)]
            public void ForBlock<TFunction>(ref TFunction function,
                                            EntityStatusType entities = EntityStatusType.Enabled,
                                            ReadOnlySpan<ushort> clusters = default)
                where TFunction : struct, IQueryBlock {
                
                if (Prepare(Filter, clusters, entities, true, out var queryData, out var firstGlobalBlockIdx)) {
                    ref var world = ref Data.Instance;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {

                        #if !NET6_0_OR_GREATER
                        var deBruijn = Utils.DeBruijn;
                        #endif

                        EntityBlock entityBlock = default;
                        ref var entityBlockOffset = ref entityBlock.Offset;
                        var blocks = queryData.Blocks;
                        do {
                            var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT);
                            ref var block = ref blocks[firstGlobalBlockIdx];
                            ref var entitiesMaskRef = ref block.EntitiesMask;
                            firstGlobalBlockIdx = block.NextGlobalBlock;
                            var entitiesMask = entitiesMaskRef;
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            if (entitiesMask == ulong.MaxValue) {
                                #if FFS_ECS_DEBUG
                                world.SetCurrentQueryEntity(chunkBlockEntityId, chunkBlockEntityId + Const.U64_BITS - 1);
                                #endif
                                entityBlockOffset = chunkBlockEntityId;
                                function.Invoke(
                                    Const.U64_BITS,
                                    entityBlock
                                );
                            }
                            else {
                                var runStarts = entitiesMask & ~(entitiesMask << 1);
                                var runEnds = entitiesMask & ~(entitiesMask >> 1);
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
                                    #if FFS_ECS_DEBUG
                                    world.SetCurrentQueryEntity(chunkBlockEntityId + runStart, chunkBlockEntityId + runEnd);
                                    #endif
                                    entityBlockOffset = chunkBlockEntityId + runStart;
                                    function.Invoke(
                                        (uint)(runEnd - runStart + 1),
                                        entityBlock
                                    );
                                } while (runStarts != 0);
                            }
                        } while (firstGlobalBlockIdx >= 0);
                    }

                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        Dispose(Filter, entities, true, queryData);
                    }
                }
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="function"/> by value.
            /// <para>See the <c>ref TFunction</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForBlockParallel<TFunction>(TFunction function = default,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ReadOnlySpan<ushort> clusters = default,
                                                    uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                    uint workersLimit = 0)
                where TFunction : struct, IQueryBlock {
                ForBlockParallel(ref function, entities, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <summary>
            /// Parallel version of <c>ForBlock</c>.
            /// Distributes block-based iteration across worker threads via <see cref="ParallelRunner{TWorld}"/>.
            /// Always uses <see cref="QueryMode.Strict"/> semantics.
            /// </summary>
            /// <typeparam name="TFunction">Struct implementing <see cref="IQueryBlock"/>.</typeparam>
            /// <param name="function">The block function struct, passed by ref to preserve state across invocations.</param>
            /// <param name="entities">Filter by entity status.</param>
            /// <param name="clusters">Optional cluster filter. When empty, uses the world's active clusters.</param>
            /// <param name="minEntitiesPerThread">Minimum entities per worker thread. Defaults to <see cref="Const.ENTITIES_IN_SEGMENT"/> (256).</param>
            /// <param name="workersLimit">Maximum number of worker threads. 0 = no limit.</param>
            [MethodImpl(AggressiveInlining)]
            public void ForBlockParallel<TFunction>(ref TFunction function,
                                                    EntityStatusType entities = EntityStatusType.Enabled,
                                                    ReadOnlySpan<ushort> clusters = default,
                                                    uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                                    uint workersLimit = 0)
                where TFunction : struct, IQueryBlock {
                if (PrepareParallel(Filter, clusters, entities, out var count, out var jobs, out var jobIndexes)) {
                    Resources<ParallelData<TFunction>>.Value = new(function);
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ParallelFunctionBlock<TFunction>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        function = Resources<ParallelData<TFunction>>.Value.Value;
                        Resources<ParallelData<TFunction>>.Value = default;
                    }
                }
            }
            
            internal static unsafe void ParallelFunctionBlock<TFunction>(Job[] jobs, uint[] jobIndexes, uint from, uint to, int worker)
                where TFunction : struct, IQueryBlock {
                ref var world = ref Data.Instance;

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                ref var function = ref Resources<ParallelData<TFunction>>.Value.Value;
                EntityBlock entityBlock = default;
                ref var entityBlockOffset = ref entityBlock.Offset;

                while (from < to) {
                    ref var job = ref jobs[jobIndexes[from++]];
                    var count = job.Count;
                    job.Count = 0;

                    for (uint i = 0; i < count; i++) {
                        var chunkBlockEntityId = job.GlobalBlockIdx[i] << Const.ENTITIES_IN_BLOCK_SHIFT;
                        var entitiesMask = job.Masks[i];
                        chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                        if (entitiesMask == ulong.MaxValue) {
                            #if FFS_ECS_DEBUG
                            world.SetCurrentQueryEntity(chunkBlockEntityId, chunkBlockEntityId + Const.U64_BITS - 1);
                            #endif
                            entityBlockOffset = chunkBlockEntityId;
                            function.Invoke(
                                Const.U64_BITS,
                                entityBlock
                            );
                        }
                        else {
                            var runStarts = entitiesMask & ~(entitiesMask << 1);
                            var runEnds = entitiesMask & ~(entitiesMask >> 1);
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
                                #if FFS_ECS_DEBUG
                                world.SetCurrentQueryEntity(chunkBlockEntityId + runStart, chunkBlockEntityId + runEnd);
                                #endif
                                entityBlockOffset = chunkBlockEntityId + runStart;
                                function.Invoke(
                                    (uint)(runEnd - runStart + 1),
                                    entityBlock
                                );
                            } while (runStarts != 0);
                        }
                    }
                }
            }
            #endregion

            #region UNSAFE DELEGATE BLOCKS
            /// <summary>
            /// Block-based iteration using an unmanaged function pointer.
            /// The function receives contiguous runs of entities as <see cref="EntityBlock"/>, enabling batch processing.
            /// No component data is accessed. Always uses <see cref="QueryMode.Strict"/> semantics.
            /// </summary>
            /// <param name="function">Unmanaged function pointer receiving <c>(byte count, EntityBlock entityBlock)</c> for each contiguous run.</param>
            /// <param name="entities">Filter by entity status.</param>
            /// <param name="clusters">Optional cluster filter. When empty, uses the world's active clusters.</param>
            [MethodImpl(AggressiveInlining)]
            public unsafe void ForBlock(delegate*<uint, EntityBlock, void> function,
                                        EntityStatusType entities = EntityStatusType.Enabled,
                                        ReadOnlySpan<ushort> clusters = default) {

                if (Prepare(Filter, clusters, entities, true, out var queryData, out var firstGlobalBlockIdx)) {
                    ref var world = ref Data.Instance;
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {

                        #if !NET6_0_OR_GREATER
                        var deBruijn = Utils.DeBruijn;
                        #endif

                        EntityBlock entityBlock = default;
                        ref var entityBlockOffset = ref entityBlock.Offset;
                        var blocks = queryData.Blocks;
                        do {
                            var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT);
                            ref var block = ref blocks[firstGlobalBlockIdx];
                            ref var entitiesMaskRef = ref block.EntitiesMask;
                            firstGlobalBlockIdx = block.NextGlobalBlock;
                            var entitiesMask = entitiesMaskRef;
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            if (entitiesMask == ulong.MaxValue) {
                                #if FFS_ECS_DEBUG
                                world.SetCurrentQueryEntity(chunkBlockEntityId, chunkBlockEntityId + Const.U64_BITS - 1);
                                #endif
                                entityBlockOffset = chunkBlockEntityId;
                                function(
                                    Const.U64_BITS,
                                    entityBlock
                                );
                            }
                            else {
                                var runStarts = entitiesMask & ~(entitiesMask << 1);
                                var runEnds = entitiesMask & ~(entitiesMask >> 1);
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
                                    #if FFS_ECS_DEBUG
                                    world.SetCurrentQueryEntity(chunkBlockEntityId + runStart, chunkBlockEntityId + runEnd);
                                    #endif
                                    entityBlockOffset = chunkBlockEntityId + runStart;
                                    function(
                                        (uint)(runEnd - runStart + 1),
                                        entityBlock
                                    );
                                } while (runStarts != 0);
                            }
                        } while (firstGlobalBlockIdx >= 0);
                    }

                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        Dispose(Filter, entities, true, queryData);
                    }
                }
            }
            #endregion

            #region STRUCT FUNCTION
            /// <summary>
            /// Convenience overload that accepts <paramref name="function"/> by value.
            /// <para>See the <c>ref TFunction</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void For<TFunction>(TFunction function = default,
                                       EntityStatusType entities = EntityStatusType.Enabled,
                                       QueryMode queryMode = QueryMode.Strict,
                                       ReadOnlySpan<ushort> clusters = default)
                where TFunction : struct, IQuery {
                For(ref function, entities, queryMode, clusters);
            }

            /// <summary>
            /// Iterates over all entities matching <typeparamref name="TFilter"/>,
            /// invoking a struct function implementing <see cref="IQuery"/> with <c>(Entity)</c>.
            /// No component data is accessed. Struct functions enable JIT devirtualization,
            /// producing the same code quality as hand-written loops.
            /// </summary>
            /// <typeparam name="TFunction">Struct implementing <see cref="IQuery"/>.</typeparam>
            /// <param name="function">The function struct, passed by ref to preserve state across invocations.</param>
            /// <param name="entities">Filter by entity status.</param>
            /// <param name="queryMode"><see cref="QueryMode.Strict"/> (default) for fastest iteration; <see cref="QueryMode.Flexible"/> allows modifying filtered types on other entities.</param>
            /// <param name="clusters">Optional cluster filter. When empty, uses the world's active clusters.</param>
            [MethodImpl(AggressiveInlining)]
            public void For<TFunction>(ref TFunction function,
                                       EntityStatusType entities = EntityStatusType.Enabled,
                                       QueryMode queryMode = QueryMode.Strict,
                                       ReadOnlySpan<ushort> clusters = default)
                where TFunction : struct, IQuery {
                ref var world = ref Data.Instance;
                var strict = queryMode == QueryMode.Strict;

                if (Prepare(Filter, clusters, entities, strict, out var queryData, out var firstGlobalBlockIdx)) {
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {

                        #if !NET6_0_OR_GREATER
                        var deBruijn = Utils.DeBruijn;
                        #endif

                        var blocks = queryData.Blocks;
                        var segmentIdx = uint.MaxValue;
                        var entity = new Entity();
                        ref var entityId = ref entity.IdWithOffset;

                        do {
                            var curSegmentIdx = firstGlobalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                            if (curSegmentIdx != segmentIdx) {
                                segmentIdx = (uint)curSegmentIdx;
                            }

                            var chunkBlockEntityId = (uint)(firstGlobalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT);
                            ref var block = ref blocks[firstGlobalBlockIdx];
                            ref var entitiesMaskRef = ref block.EntitiesMask;
                            firstGlobalBlockIdx = block.NextGlobalBlock;
                            var entitiesMask = entitiesMaskRef;
                            #if NET6_0_OR_GREATER
                            var componentOffset = (int)(chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK);
                            #else
                            var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                            #endif
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            if (strict) {
                                if (entitiesMask == ulong.MaxValue) {
                                    var componentEnd = componentOffset + Const.U64_BITS;
                                    entityId = chunkBlockEntityId;
                                    while (componentOffset < componentEnd) {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        function.Invoke(
                                            entity
                                        );
                                        componentOffset++;
                                        entityId++;
                                    }
                                }
                                else {
                                    var runStarts = entitiesMask & ~(entitiesMask << 1);
                                    var runEnds = entitiesMask & ~(entitiesMask >> 1);
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
                                        var componentIdx = runStart + componentOffset;
                                        var componentEnd = runEnd + componentOffset;
                                        entityId = chunkBlockEntityId + runStart;
                                        while (componentIdx <= componentEnd) {
                                            #if FFS_ECS_DEBUG
                                            world.SetCurrentQueryEntity(entityId);
                                            #endif
                                            function.Invoke(
                                                entity
                                            );
                                            componentIdx++;
                                            entityId++;
                                        }
                                    } while (runStarts != 0);
                                }
                            }
                            else {
                                do {
                                    var isolatedBit = entitiesMask & (ulong)-(long)entitiesMask;
                                    #if NET6_0_OR_GREATER
                                    var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                                    #else
                                    var runStart = deBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                                    #endif

                                    entityId = chunkBlockEntityId + runStart;

                                    do {
                                        #if FFS_ECS_DEBUG
                                        world.SetCurrentQueryEntity(entityId);
                                        #endif
                                        function.Invoke(
                                            entity
                                        );

                                        isolatedBit <<= 1;
                                        entityId++;
                                    } while ((entitiesMaskRef & isolatedBit) != 0);

                                    entitiesMask = entitiesMaskRef & ~(isolatedBit - 1);
                                } while (entitiesMask != 0);
                            }
                        } while (firstGlobalBlockIdx >= 0);
                    }

                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        Dispose(Filter, entities, strict, queryData);
                    }
                }
            }
            
            /// <summary>
            /// Convenience overload that accepts <paramref name="function"/> by value.
            /// <para>See the <c>ref TFunction</c> overload for full documentation.</para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TFunction>(TFunction function = default,
                                               EntityStatusType entities = EntityStatusType.Enabled,
                                               ReadOnlySpan<ushort> clusters = default,
                                               uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                               uint workersLimit = 0)
                where TFunction : struct, IQuery {
                ForParallel(ref function, entities, clusters, minEntitiesPerThread, workersLimit);
            }

            /// <summary>
            /// Parallel version of the struct-based <c>For</c> overload accepting <see cref="IQuery"/>.
            /// Distributes matching entities across worker threads via <see cref="ParallelRunner{TWorld}"/>.
            /// Always uses <see cref="QueryMode.Strict"/> semantics.
            /// </summary>
            /// <typeparam name="TFunction">Struct implementing <see cref="IQuery"/>.</typeparam>
            /// <param name="function">The function struct, passed by ref. Updated after all workers complete.</param>
            /// <param name="entities">Filter by entity status.</param>
            /// <param name="clusters">Optional cluster filter. When empty, uses the world's active clusters.</param>
            /// <param name="minEntitiesPerThread">Minimum entities per worker thread. Defaults to <see cref="Const.ENTITIES_IN_SEGMENT"/> (256).</param>
            /// <param name="workersLimit">Maximum number of worker threads. 0 = no limit.</param>
            [MethodImpl(AggressiveInlining)]
            public void ForParallel<TFunction>(ref TFunction function,
                                               EntityStatusType entities = EntityStatusType.Enabled,
                                               ReadOnlySpan<ushort> clusters = default,
                                               uint minEntitiesPerThread = Const.ENTITIES_IN_SEGMENT,
                                               uint workersLimit = 0)
                where TFunction : struct, IQuery {
                if (PrepareParallel(Filter, clusters, entities, out var count, out var jobs, out var jobIndexes)) {
                    Resources<ParallelData<TFunction>>.Value = new(function);
                    #if FFS_ECS_DEBUG
                    try
                    #endif
                    {
                        unsafe {
                            ParallelRunner<TWorld>.Run(&ParallelStructFunctionWithEntity<TFunction>,
                                jobs, jobIndexes, count, Math.Max(minEntitiesPerThread / Const.ENTITIES_IN_SEGMENT, 1), workersLimit
                            );
                        }
                    }
                    #if FFS_ECS_DEBUG
                    finally
                    #endif
                    {
                        ref var world = ref Data.Instance;
                        #if FFS_ECS_DEBUG
                        world.SetCurrentQueryEntity(default);
                        #endif
                        world.QueryDataCount--;
                        #if FFS_ECS_DEBUG
                        if (world.QueryDataCount == 0) {
                            world.QueryMode = 0;
                        }
                        #endif
                        function = Resources<ParallelData<TFunction>>.Value.Value;
                        Resources<ParallelData<TFunction>>.Value = default;
                    }
                }
            }
            
            internal static unsafe void ParallelStructFunctionWithEntity<TFunction>(Job[] jobs, uint[] jobIndexes, uint from, uint to, int worker)            
                where TFunction : struct, IQuery {
                ref var world = ref Data.Instance;

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                ref var function = ref Resources<ParallelData<TFunction>>.Value.Value;
                var entity = new Entity();
                ref var entityId = ref entity.IdWithOffset;

                while (from < to) {
                    ref var job = ref jobs[jobIndexes[from++]];
                    var count = job.Count;
                    job.Count = 0;

                    for (uint i = 0; i < count; i++) {
                        var chunkBlockEntityId = job.GlobalBlockIdx[i] << Const.ENTITIES_IN_BLOCK_SHIFT;
                        var entitiesMask = job.Masks[i];
                        #if NET6_0_OR_GREATER
                        var componentOffset = (int)(chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK);
                        #else
                        var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                        #endif
                        chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                        if (entitiesMask == ulong.MaxValue) {
                            var componentEnd = componentOffset + Const.U64_BITS;
                            entityId = chunkBlockEntityId;
                            while (componentOffset < componentEnd) {
                                #if FFS_ECS_DEBUG
                                world.SetCurrentQueryEntity(entityId);
                                #endif
                                function.Invoke(
                                    entity
                                );
                                componentOffset++;
                                entityId++;
                            }
                        }
                        else {
                            var runStarts = entitiesMask & ~(entitiesMask << 1);
                            var runEnds = entitiesMask & ~(entitiesMask >> 1);
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
                                var componentIdx = runStart + componentOffset;
                                var componentEnd = runEnd + componentOffset;
                                entityId = chunkBlockEntityId + runStart;
                                while (componentIdx <= componentEnd) {
                                    #if FFS_ECS_DEBUG
                                    world.SetCurrentQueryEntity(entityId);
                                    #endif
                                    function.Invoke(
                                        entity
                                    );
                                    componentIdx++;
                                    entityId++;
                                }
                            } while (runStarts != 0);
                        }
                    }
                }
            }
            #endregion

            #region PREPARE AND DISPOSE
            [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
            [MethodImpl(AggressiveInlining)]
            internal bool Prepare(TFilter filter, ReadOnlySpan<ushort> clusters, EntityStatusType entities,
                                  bool strict, out QueryData queryData, out int firstGlobalBlockIdx, bool withDisabledClusters = false) {
                #if FFS_ECS_DEBUG
                AssertNotNestedParallelQuery(WorldTypeName);
                filter.Assert<TWorld>();
                #endif


                ref var world = ref Data.Instance;

                clusters = world.GetActiveClustersIfEmpty(clusters);
                queryData = default;
                BlockMaskCache[] filteredBlocks = null;

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                var previousGlobalBlockIdx = -1;
                firstGlobalBlockIdx = -1;

                for (var i = 0; i < clusters.Length; i++) {
                    var clusterIdx = clusters[i];
                    ref var cluster = ref world.Clusters[clusterIdx];
                    if (!withDisabledClusters && cluster.Disabled) {
                        continue;
                    }

                    for (uint chunkMapIdx = 0; chunkMapIdx < cluster.LoadedChunksCount; chunkMapIdx++) {
                        var chunkIdx = cluster.LoadedChunks[chunkMapIdx];
                        var chunkMask = world.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
                        chunkMask &= filter.FilterChunk<TWorld>(chunkIdx);

                        if (chunkMask != 0) {
                            var segmentIdx = uint.MaxValue;

                            ulong[] worldMasks = null;

                            do {
                                #if NET6_0_OR_GREATER
                                var chunkBlockIdx = (uint)System.Numerics.BitOperations.TrailingZeroCount(chunkMask);
                                #else
                                var chunkBlockIdx = (uint)deBruijn[(uint)(((chunkMask & (ulong)-(long)chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                                #endif
                                chunkMask &= chunkMask - 1;
                                var globalBlockIdx = chunkBlockIdx + (chunkIdx << Const.BLOCKS_IN_CHUNK_SHIFT);

                                var curSegmentIdx = (chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT) + (chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT);
                                if (curSegmentIdx != segmentIdx) {
                                    segmentIdx = curSegmentIdx;
                                    worldMasks = world.EntitiesSegments[segmentIdx].Masks;
                                }

                                var blockIdx = (byte)(chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);
                                var disabledBlockIdx = blockIdx + Const.BLOCKS_IN_SEGMENT;
                                var loadedBlockIdx = disabledBlockIdx + Const.BLOCKS_IN_SEGMENT;

                                var entitiesMask = entities switch {
                                    EntityStatusType.Enabled => worldMasks[loadedBlockIdx] & worldMasks[blockIdx] & ~worldMasks[disabledBlockIdx],
                                    EntityStatusType.Disabled => worldMasks[loadedBlockIdx] & worldMasks[disabledBlockIdx],
                                    _ => worldMasks[loadedBlockIdx] & worldMasks[blockIdx]
                                };
                                entitiesMask &= filter.FilterEntities<TWorld>(segmentIdx, blockIdx);

                                if (entitiesMask != 0) {
                                    if (previousGlobalBlockIdx >= 0) {
                                        filteredBlocks[previousGlobalBlockIdx].NextGlobalBlock = (int)globalBlockIdx;
                                    }
                                    else {
                                        #if FFS_ECS_DEBUG
                                        var queryMode = strict ? (byte)1 : (byte)2;
                                        AssertSameQueryMode(WorldTypeName, queryMode);
                                        world.QueryMode = queryMode;
                                        #endif

                                        queryData = world.PushCurrentQuery();

                                        if (!strict) {
                                            filter.PushQueryData<TWorld>(queryData);
                                            world.PushQueryDataForDestroy(queryData);

                                            switch (entities) {
                                                case EntityStatusType.Enabled: world.PushQueryDataForDisable(queryData); break;
                                                case EntityStatusType.Disabled: world.PushQueryDataForEnable(queryData); break;
                                            }
                                        }
                                        #if FFS_ECS_DEBUG
                                        else {
                                            const int block = 1;
                                            filter.Block<TWorld>(block);
                                            world.BlockDestroy(block);

                                            switch (entities) {
                                                case EntityStatusType.Enabled: world.BlockDisable(block); break;
                                                case EntityStatusType.Disabled: world.BlockEnable(block); break;
                                            }
                                        }
                                        #endif

                                        filteredBlocks = queryData.Blocks;
                                        firstGlobalBlockIdx = (int)globalBlockIdx;
                                    }

                                    filteredBlocks[globalBlockIdx].EntitiesMask = entitiesMask;
                                    filteredBlocks[globalBlockIdx].NextGlobalBlock = -1;
                                    previousGlobalBlockIdx = (int)globalBlockIdx;
                                }
                            } while (chunkMask != 0);
                        }
                    }
                }

                return filteredBlocks != null;
            }
            [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
            [MethodImpl(AggressiveInlining)]
            internal bool Prepare(TFilter filter, ReadOnlySpan<uint> chunks, EntityStatusType entities,
                                  bool strict, out QueryData queryData, out int firstGlobalBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertNotNestedParallelQuery(WorldTypeName);
                filter.Assert<TWorld>();
                #endif


                ref var world = ref Data.Instance;

                queryData = default;
                BlockMaskCache[] filteredBlocks = null;

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                var previousGlobalBlockIdx = -1;
                firstGlobalBlockIdx = -1;

                for (uint chunkMapIdx = 0; chunkMapIdx < chunks.Length; chunkMapIdx++) {
                    var chunkIdx = chunks[(int)chunkMapIdx];
                    var chunkMask = world.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
                    chunkMask &= filter.FilterChunk<TWorld>(chunkIdx);

                    if (chunkMask != 0) {
                        var segmentIdx = uint.MaxValue;

                        ulong[] worldMasks = null;

                        do {
                            #if NET6_0_OR_GREATER
                            var chunkBlockIdx = (uint)System.Numerics.BitOperations.TrailingZeroCount(chunkMask);
                            #else
                            var chunkBlockIdx = (uint)deBruijn[(uint)(((chunkMask & (ulong)-(long)chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                            #endif
                            chunkMask &= chunkMask - 1;
                            var globalBlockIdx = chunkBlockIdx + (chunkIdx << Const.BLOCKS_IN_CHUNK_SHIFT);

                            var curSegmentIdx = (chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT) + (chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT);
                            if (curSegmentIdx != segmentIdx) {
                                segmentIdx = curSegmentIdx;
                                worldMasks = world.EntitiesSegments[segmentIdx].Masks;
                            }

                            var blockIdx = (byte)(chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);
                            var disabledBlockIdx = blockIdx + Const.BLOCKS_IN_SEGMENT;
                            var loadedBlockIdx = disabledBlockIdx + Const.BLOCKS_IN_SEGMENT;

                            var entitiesMask = entities switch {
                                EntityStatusType.Enabled => worldMasks[loadedBlockIdx] & worldMasks[blockIdx] & ~worldMasks[disabledBlockIdx],
                                EntityStatusType.Disabled => worldMasks[loadedBlockIdx] & worldMasks[disabledBlockIdx],
                                _ => worldMasks[loadedBlockIdx] & worldMasks[blockIdx]
                            };
                            entitiesMask &= filter.FilterEntities<TWorld>(segmentIdx, blockIdx);

                            if (entitiesMask != 0) {
                                if (previousGlobalBlockIdx >= 0) {
                                    filteredBlocks[previousGlobalBlockIdx].NextGlobalBlock = (int)globalBlockIdx;
                                }
                                else {
                                    #if FFS_ECS_DEBUG
                                    var queryMode = strict ? (byte)1 : (byte)2;
                                    AssertSameQueryMode(WorldTypeName, queryMode);
                                    world.QueryMode = queryMode;
                                    #endif

                                    queryData = world.PushCurrentQuery();

                                    if (!strict) {
                                        filter.PushQueryData<TWorld>(queryData);
                                        world.PushQueryDataForDestroy(queryData);

                                        switch (entities) {
                                            case EntityStatusType.Enabled: world.PushQueryDataForDisable(queryData); break;
                                            case EntityStatusType.Disabled: world.PushQueryDataForEnable(queryData); break;
                                        }
                                    }
                                    #if FFS_ECS_DEBUG
                                    else {
                                        const int block = 1;
                                        filter.Block<TWorld>(block);
                                        world.BlockDestroy(block);

                                        switch (entities) {
                                            case EntityStatusType.Enabled: world.BlockDisable(block); break;
                                            case EntityStatusType.Disabled: world.BlockEnable(block); break;
                                        }
                                    }
                                    #endif

                                    filteredBlocks = queryData.Blocks;
                                    firstGlobalBlockIdx = (int)globalBlockIdx;
                                }

                                filteredBlocks[globalBlockIdx].EntitiesMask = entitiesMask;
                                filteredBlocks[globalBlockIdx].NextGlobalBlock = -1;
                                previousGlobalBlockIdx = (int)globalBlockIdx;
                            }
                        } while (chunkMask != 0);
                    }
                }

                return filteredBlocks != null;
            }
            
            [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
            [MethodImpl(AggressiveInlining)]
            internal unsafe bool PrepareParallel(TFilter filter, ReadOnlySpan<ushort> clusters,
                                                 EntityStatusType entities,
                                                 out uint jobsCount, out Job[] jobs, out uint[] jobIndexes) {
                #if FFS_ECS_DEBUG
                AssertNotNestedParallelQuery(WorldTypeName);
                AssertNotMoreThanOneParallelQuery(WorldTypeName);
                AssertParallelAvailable(WorldTypeName);
                filter.Assert<TWorld>();
                #endif

                ref var world = ref Data.Instance;

                clusters = world.GetActiveClustersIfEmpty(clusters);
                jobsCount = 0;
                jobs = default;
                jobIndexes = default;

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                for (var i = 0; i < clusters.Length; i++) {
                    var clusterIdx = clusters[i];
                    ref var cluster = ref world.Clusters[clusterIdx];
                    if (cluster.Disabled) {
                        continue;
                    }

                    for (uint chunkMapIdx = 0; chunkMapIdx < cluster.LoadedChunksCount; chunkMapIdx++) {
                        var chunkIdx = cluster.LoadedChunks[chunkMapIdx];
                        var chunkMask = world.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
                        chunkMask &= filter.FilterChunk<TWorld>(chunkIdx);

                        if (chunkMask != 0) {
                            var segmentIdx = uint.MaxValue;

                            ulong[] worldMasks = null;

                            do {
                                #if NET6_0_OR_GREATER
                                var chunkBlockIdx = (uint)System.Numerics.BitOperations.TrailingZeroCount(chunkMask);
                                #else
                                var chunkBlockIdx = (uint)deBruijn[(uint)(((chunkMask & (ulong)-(long)chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                                #endif
                                chunkMask &= chunkMask - 1;
                                var globalBlockIdx = chunkBlockIdx + (chunkIdx << Const.BLOCKS_IN_CHUNK_SHIFT);

                                var curSegmentIdx = (chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT) + (chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT);
                                if (curSegmentIdx != segmentIdx) {
                                    segmentIdx = curSegmentIdx;
                                    worldMasks = world.EntitiesSegments[segmentIdx].Masks;
                                }

                                var blockIdx = (byte)(chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);
                                var disabledBlockIdx = blockIdx + Const.BLOCKS_IN_SEGMENT;
                                var loadedBlockIdx = disabledBlockIdx + Const.BLOCKS_IN_SEGMENT;

                                var entitiesMask = entities switch {
                                    EntityStatusType.Enabled => worldMasks[loadedBlockIdx] & worldMasks[blockIdx] & ~worldMasks[disabledBlockIdx],
                                    EntityStatusType.Disabled => worldMasks[loadedBlockIdx] & worldMasks[disabledBlockIdx],
                                    _ => worldMasks[loadedBlockIdx] & worldMasks[blockIdx]
                                };
                                entitiesMask &= filter.FilterEntities<TWorld>(segmentIdx, blockIdx);

                                if (entitiesMask != 0) {
                                    if (jobsCount == 0) {
                                        var size = world.EntitiesSegments.Length;
                                        if (ParallelRunner<TWorld>.CachedSize < size) {
                                            ParallelRunner<TWorld>.CachedJobs = new Job[size];
                                            ParallelRunner<TWorld>.CachedJobIndexes = new uint[size];
                                            ParallelRunner<TWorld>.CachedSize = size;
                                        }
                                        jobs = ParallelRunner<TWorld>.CachedJobs;
                                        jobIndexes = ParallelRunner<TWorld>.CachedJobIndexes;
                                        world.QueryDataCount++;
                                    }

                                    ref var job = ref jobs[segmentIdx];
                                    if (job.Count == 0) {
                                        jobIndexes[jobsCount++] = segmentIdx;
                                    }

                                    job.Masks[job.Count] = entitiesMask;
                                    job.GlobalBlockIdx[job.Count++] = globalBlockIdx;
                                }
                            } while (chunkMask != 0);
                        }
                    }
                }

                return jobsCount != 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal void Dispose(TFilter filter, EntityStatusType entities, bool strict, QueryData queryData) {
                ref var world = ref Data.Instance;

                world.PopCurrentQuery(queryData);

                if (!strict) {
                    filter.PopQueryData<TWorld>();
                    world.PopQueryDataForDestroy();

                    switch (entities) {
                        case EntityStatusType.Enabled: world.PopQueryDataForDisable(); break;
                        case EntityStatusType.Disabled: world.PopQueryDataForEnable(); break;
                    }
                }
                #if FFS_ECS_DEBUG
                else {
                    const int unblock = -1;
                    filter.Block<TWorld>(unblock);
                    world.BlockDestroy(unblock);

                    switch (entities) {
                        case EntityStatusType.Enabled: world.BlockDisable(unblock); break;
                        case EntityStatusType.Disabled: world.BlockEnable(unblock); break;
                    }
                }

                if (world.QueryDataCount == 0) {
                    world.QueryMode = 0;
                }
                #endif
            }
            #endregion

            #region FIND FIRST
            [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
            [MethodImpl(AggressiveInlining)]
            internal bool FindFirst(TFilter filter, ReadOnlySpan<ushort> clusters, EntityStatusType entities, out Entity entity, bool assertSingle) {
                #if FFS_ECS_DEBUG
                filter.Assert<TWorld>();
                #endif

                ref var world = ref Data.Instance;
                clusters = world.GetActiveClustersIfEmpty(clusters);
                entity = default;

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                #if FFS_ECS_DEBUG
                var found = false;
                #endif

                for (var i = 0; i < clusters.Length; i++) {
                    var clusterIdx = clusters[i];
                    ref var cluster = ref world.Clusters[clusterIdx];
                    if (cluster.Disabled) continue;

                    for (uint chunkMapIdx = 0; chunkMapIdx < cluster.LoadedChunksCount; chunkMapIdx++) {
                        var chunkIdx = cluster.LoadedChunks[chunkMapIdx];
                        var chunkMask = world.HeuristicChunks[chunkIdx].NotEmptyBlocks.Value;
                        chunkMask &= filter.FilterChunk<TWorld>(chunkIdx);

                        if (chunkMask != 0) {
                            var segmentIdx = uint.MaxValue;
                            ulong[] worldMasks = null;

                            do {
                                #if NET6_0_OR_GREATER
                                var chunkBlockIdx = (uint)System.Numerics.BitOperations.TrailingZeroCount(chunkMask);
                                #else
                                var chunkBlockIdx = (uint)deBruijn[(uint)(((chunkMask & (ulong)-(long)chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                                #endif
                                chunkMask &= chunkMask - 1;
                                var globalBlockIdx = chunkBlockIdx + (chunkIdx << Const.BLOCKS_IN_CHUNK_SHIFT);

                                var curSegmentIdx = (chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT) + (chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT);
                                if (curSegmentIdx != segmentIdx) {
                                    segmentIdx = curSegmentIdx;
                                    worldMasks = world.EntitiesSegments[segmentIdx].Masks;
                                }

                                var blockIdx = (byte)(chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);
                                var disabledBlockIdx = blockIdx + Const.BLOCKS_IN_SEGMENT;
                                var loadedBlockIdx = disabledBlockIdx + Const.BLOCKS_IN_SEGMENT;

                                var entitiesMask = entities switch {
                                    EntityStatusType.Enabled => worldMasks[loadedBlockIdx] & worldMasks[blockIdx] & ~worldMasks[disabledBlockIdx],
                                    EntityStatusType.Disabled => worldMasks[loadedBlockIdx] & worldMasks[disabledBlockIdx],
                                    _ => worldMasks[loadedBlockIdx] & worldMasks[blockIdx]
                                };
                                entitiesMask &= filter.FilterEntities<TWorld>(segmentIdx, blockIdx);

                                if (entitiesMask != 0) {
                                    #if FFS_ECS_DEBUG
                                    if (found) {
                                        throw new StaticEcsException($"WorldQuery<{typeof(TWorld)}, {typeof(TFilter)}>.One() found more than one entity");
                                    }
                                    #endif

                                    #if NET6_0_OR_GREATER
                                    var bitIdx = (uint)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                                    #else
                                    var bitIdx = (uint)deBruijn[(uint)(((entitiesMask & (ulong)-(long)entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                                    #endif
                                    entity.IdWithOffset = (globalBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT) + Const.ENTITY_ID_OFFSET + bitIdx;

                                    #if FFS_ECS_DEBUG
                                    if (assertSingle) {
                                        if ((entitiesMask & (entitiesMask - 1)) != 0) {
                                            throw new StaticEcsException($"WorldQuery<{typeof(TWorld)}, {typeof(TFilter)}>.One() found more than one entity");
                                        }
                                        found = true;
                                        continue;
                                    }
                                    #endif
                                    return true;
                                }
                            } while (chunkMask != 0);
                        }
                    }
                }

                #if FFS_ECS_DEBUG
                return found;
                #else
                return false;
                #endif
            }
            #endregion

        }
    }
}