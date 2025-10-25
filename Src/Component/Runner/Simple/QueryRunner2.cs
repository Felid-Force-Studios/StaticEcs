#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
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
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    internal readonly struct QueryFunctionRunner<WorldType, C1, C2, P>
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where WorldType : struct, IWorldType {
        internal static readonly QueryFunctionRunner<WorldType, C1, C2, P> Value = default;
        
        [MethodImpl(AggressiveInlining)]
        internal bool Search(ReadOnlySpan<ushort> clusters, SearchFunctionWithEntity<WorldType, C1, C2> function, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode, out World<WorldType>.Entity entity) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;

            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.config.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(clusters, with, entities, components, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;
            var result = false;

            #if FFS_ECS_DEBUG
            try
            #endif
            {
                #if NET6_0_OR_GREATER
                Span<C1> d1 = default;
                Span<C2> d2 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                #endif

                var dataIdx = uint.MaxValue;
                entity = new World<WorldType>.Entity();
                ref var eid = ref entity.id;
                while (firstGlobalBlockIdx >= 0) {
                    if (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = (uint) (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT);
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        #endif
                    }

                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                    #if NET6_0_OR_GREATER
                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                    #else
                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                    #endif
                    blockEntity += Const.ENTITY_ID_OFFSET;
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if FFS_ECS_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                if (function(entity, ref d1[dIdx], ref d2[dIdx])) {
                                    result = true;
                                    goto EXIT;
                                }
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            if (function(entity, ref d1[dIdx], ref d2[dIdx])) {
                                result = true;
                                goto EXIT;
                            }
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMaskRef;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }

                EXIT: ;
            }
            #if FFS_ECS_DEBUG
            finally
            #endif
            {
                Dispose(with, entities, components, strict, qd);
            }
            return result;
        }

        [MethodImpl(AggressiveInlining)]
        internal void Run<R>(ReadOnlySpan<ushort> clusters, ref R function, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode) where R : struct, World<WorldType>.IQueryFunction<C1, C2> {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;

            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.config.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(clusters, with, entities, components, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if FFS_ECS_DEBUG
            try
            #endif
            {
                #if NET6_0_OR_GREATER
                Span<C1> d1 = default;
                Span<C2> d2 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                #endif

                var dataIdx = uint.MaxValue;
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity.id;
                while (firstGlobalBlockIdx >= 0) {
                    if (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = (uint) (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT);
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        #endif
                    }

                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                    #if NET6_0_OR_GREATER
                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                    #else
                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                    #endif
                    blockEntity += Const.ENTITY_ID_OFFSET;
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if FFS_ECS_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                function.Invoke(entity, ref d1[dIdx], ref d2[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            function.Invoke(entity, ref d1[dIdx], ref d2[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMaskRef;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }

            #if FFS_ECS_DEBUG
            finally
            #endif
            {
                Dispose(with, entities, components, strict, qd);
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal void Run(ReadOnlySpan<ushort> clusters, QueryFunction<C1, C2> function, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;

            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.config.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(clusters, with, entities, components, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if FFS_ECS_DEBUG
            try
            #endif
            {
                #if NET6_0_OR_GREATER
                Span<C1> d1 = default;
                Span<C2> d2 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                #endif

                var dataIdx = uint.MaxValue;
                while (firstGlobalBlockIdx >= 0) {
                    if (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = (uint) (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT);
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        #endif
                    }

                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                    #if NET6_0_OR_GREATER
                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                    #else
                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                    #endif
                    blockEntity += Const.ENTITY_ID_OFFSET;
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if FFS_ECS_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                function(ref d1[dIdx], ref d2[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            function(ref d1[dIdx], ref d2[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMaskRef;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }

            #if FFS_ECS_DEBUG
            finally
            #endif
            {
                Dispose(with, entities, components, strict, qd);
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal void Run<D>(ReadOnlySpan<ushort> clusters, ref D data, QueryFunctionWithRefData<D, C1, C2> function, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;

            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.config.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(clusters, with, entities, components, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if FFS_ECS_DEBUG
            try
            #endif
            {
                #if NET6_0_OR_GREATER
                Span<C1> d1 = default;
                Span<C2> d2 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                #endif

                var dataIdx = uint.MaxValue;
                while (firstGlobalBlockIdx >= 0) {
                    if (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = (uint) (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT);
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        #endif
                    }

                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                    #if NET6_0_OR_GREATER
                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                    #else
                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                    #endif
                    blockEntity += Const.ENTITY_ID_OFFSET;
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if FFS_ECS_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                function(ref data, ref d1[dIdx], ref d2[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            function(ref data, ref d1[dIdx], ref d2[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMaskRef;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }

            #if FFS_ECS_DEBUG
            finally
            #endif
            {
                Dispose(with, entities, components, strict, qd);
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal void Run(ReadOnlySpan<ushort> clusters, QueryFunctionWithEntity<WorldType, C1, C2> function, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;

            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.config.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(clusters, with, entities, components, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if FFS_ECS_DEBUG
            try
            #endif
            {
                #if NET6_0_OR_GREATER
                Span<C1> d1 = default;
                Span<C2> d2 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                #endif

                var dataIdx = uint.MaxValue;
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity.id;
                while (firstGlobalBlockIdx >= 0) {
                    if (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = (uint) (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT);
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        #endif
                    }

                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                    #if NET6_0_OR_GREATER
                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                    #else
                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                    #endif
                    blockEntity += Const.ENTITY_ID_OFFSET;
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if FFS_ECS_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                function(entity, ref d1[dIdx], ref d2[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            function(entity, ref d1[dIdx], ref d2[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMaskRef;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }

            #if FFS_ECS_DEBUG
            finally
            #endif
            {
                Dispose(with, entities, components, strict, qd);
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal void Run<D>(ReadOnlySpan<ushort> clusters, ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2> function, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;

            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.config.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(clusters, with, entities, components, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if FFS_ECS_DEBUG
            try
            #endif
            {
                #if NET6_0_OR_GREATER
                Span<C1> d1 = default;
                Span<C2> d2 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                #endif

                var dataIdx = uint.MaxValue;
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity.id;
                while (firstGlobalBlockIdx >= 0) {
                    if (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = (uint) (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT);
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        #endif
                    }
                    
                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                    #if NET6_0_OR_GREATER
                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                    #else
                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                    #endif
                    blockEntity += Const.ENTITY_ID_OFFSET;
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if FFS_ECS_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                function(ref data, entity, ref d1[dIdx], ref d2[dIdx]);
                                
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            function(ref data, entity, ref d1[dIdx], ref d2[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMaskRef;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }

            #if FFS_ECS_DEBUG
            finally
            #endif
            {
                Dispose(with, entities, components, strict, qd);
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void Prepare(ReadOnlySpan<ushort> clusters, P with, EntityStatusType entities, ComponentStatus components, bool strict, out QueryData qd, out int firstGlobalBlockIdx) {
            #if FFS_ECS_DEBUG
            World<WorldType>.AssertNotNestedParallelQuery(World<WorldType>.WorldTypeName);
            World<WorldType>.AssertRegisteredComponent<C1>(World<WorldType>.Components<C1>.ComponentsTypeName);
            World<WorldType>.AssertRegisteredComponent<C2>(World<WorldType>.Components<C2>.ComponentsTypeName);
            with.Assert<WorldType>();
            #endif

            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            
            qd = default;
            
            #if FFS_ECS_DEBUG
            var modeVal = strict ? (byte) 1 : (byte) 2;
            World<WorldType>.AssertSameQueryMode(World<WorldType>.WorldTypeName, modeVal);
            World<WorldType>.CurrentQuery.QueryMode = modeVal;
            #endif

            var entitiesChunks = World<WorldType>.Entities.Value.chunks;
            BlockMaskCache[] filteredBlocks = null;

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif

            var previousGlobalBlockIdx = -1;
            firstGlobalBlockIdx = -1;

            var entitiesClusters = World<WorldType>.Entities.Value.clusters;

            for (var i = 0; i < clusters.Length; i++) {
                var clusterIdx = clusters[i];
                ref var cluster = ref entitiesClusters[clusterIdx];
                if (cluster.disabled) {
                    continue;
                }
                for (uint chunkMapIdx = 0; chunkMapIdx < cluster.loadedChunksCount; chunkMapIdx++) {
                    var chunkIdx = cluster.loadedChunks[chunkMapIdx];

                    ref var chE = ref entitiesChunks[chunkIdx];
                    ref var ch1 = ref c1.chunks[chunkIdx];
                    ref var ch2 = ref c2.chunks[chunkIdx];

                    var chunkMask = chE.notEmptyBlocks
                                    & ch1.notEmptyBlocks
                                    & ch2.notEmptyBlocks;
                    with.CheckChunk<WorldType>(ref chunkMask, chunkIdx);

                    var lMask = chE.loadedEntities;
                    var aMask = chE.entities;
                    var aMask1 = ch1.entities;
                    var aMask2 = ch2.entities;

                    var dMask = chE.disabledEntities;
                    var dMask1 = ch1.disabledEntities;
                    var dMask2 = ch2.disabledEntities;

                    while (chunkMask > 0) {
                        var blockIdx = (uint) deBruijn[(int) (((chunkMask & (ulong) -(long) chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                        chunkMask &= chunkMask - 1;
                        var globalBlockIdx = blockIdx + (chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                        var entitiesMask = entities switch {
                            EntityStatusType.Enabled  => lMask[blockIdx] & aMask[blockIdx] & ~dMask[blockIdx],
                            EntityStatusType.Disabled => lMask[blockIdx] & dMask[blockIdx],
                            _                         => lMask[blockIdx] & aMask[blockIdx]
                        };
                        entitiesMask &= components switch {
                            ComponentStatus.Enabled  => aMask1[blockIdx] & ~dMask1[blockIdx] & aMask2[blockIdx] & ~dMask2[blockIdx],
                            ComponentStatus.Disabled => dMask1[blockIdx] & dMask2[blockIdx],
                            _                        => aMask1[blockIdx] & aMask2[blockIdx],
                        };
                        with.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, (int) blockIdx);

                        if (entitiesMask > 0) {
                            if (previousGlobalBlockIdx >= 0) {
                                filteredBlocks[previousGlobalBlockIdx].NextGlobalBlock = (int) globalBlockIdx;
                            } else {
                                qd = World<WorldType>.CurrentQuery.RegisterQuery();

                                if (!strict) {
                                    with.IncQ<WorldType>(qd);
                                    switch (entities) {
                                        case EntityStatusType.Enabled:  World<WorldType>.Entities.Value.IncQDisable(qd); break;
                                        case EntityStatusType.Disabled: World<WorldType>.Entities.Value.IncQEnable(qd); break;
                                    }
                                    World<WorldType>.Entities.Value.IncQDestroy(qd);
                                    switch (components) {
                                        case ComponentStatus.Enabled:  c1.IncQDeleteDisable(qd); c2.IncQDeleteDisable(qd); break;
                                        case ComponentStatus.Disabled: c1.IncQDeleteEnable(qd); c2.IncQDeleteEnable(qd); break;
                                        default:                       c1.IncQDelete(qd); c2.IncQDelete(qd); break;
                                    }
                                }
                                #if FFS_ECS_DEBUG
                                else {
                                    const int b = 1;
                                    with.BlockQ<WorldType>(b);
                                    switch (entities) {
                                        case EntityStatusType.Enabled:  World<WorldType>.Entities.Value.BlockDisable(b); break;
                                        case EntityStatusType.Disabled: World<WorldType>.Entities.Value.BlockEnable(b); break;
                                    }
                                    World<WorldType>.Entities.Value.BlockDestroy(b);
                                    switch (components) {
                                        case ComponentStatus.Enabled:  c1.BlockDeleteDisable(b); c2.BlockDeleteDisable(b); break;
                                        case ComponentStatus.Disabled: c1.BlockDeleteEnable(b); c2.BlockDeleteEnable(b); break;
                                        default:                       c1.BlockDelete(b); c2.BlockDelete(b); break;
                                    }
                                }
                                #endif

                                filteredBlocks = qd.Blocks;
                                firstGlobalBlockIdx = (int) globalBlockIdx;
                            }

                            filteredBlocks[globalBlockIdx].EntitiesMask = entitiesMask;
                            filteredBlocks[globalBlockIdx].NextGlobalBlock = -1;
                            previousGlobalBlockIdx = (int) globalBlockIdx;
                        }
                    }
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void Dispose(P with, EntityStatusType entities, ComponentStatus components, bool strict, QueryData qd) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;

            if (qd.Blocks != null) {
                if (!strict) {
                    with.DecQ<WorldType>();
                    switch (entities) {
                        case EntityStatusType.Enabled:  World<WorldType>.Entities.Value.DecQDisable(); break;
                        case EntityStatusType.Disabled: World<WorldType>.Entities.Value.DecQEnable(); break;
                    }
                    World<WorldType>.Entities.Value.DecQDestroy();
                    switch (components) {
                        case ComponentStatus.Enabled: c1.DecQDeleteDisable(); c2.DecQDeleteDisable(); break;
                        case ComponentStatus.Disabled: c1.DecQDeleteEnable(); c2.DecQDeleteEnable(); break;
                        default: c1.DecQDelete(); c2.DecQDelete(); break;
                    }
                }
                #if FFS_ECS_DEBUG
                else {
                    const int b = -1;
                    with.BlockQ<WorldType>(b);
                    switch (entities) {
                        case EntityStatusType.Enabled:  World<WorldType>.Entities.Value.BlockDisable(b); break;
                        case EntityStatusType.Disabled: World<WorldType>.Entities.Value.BlockEnable(b); break;
                    }
                    World<WorldType>.Entities.Value.BlockDestroy(b);
                    switch (components) {
                        case ComponentStatus.Enabled:
                            c1.BlockDeleteDisable(b); c2.BlockDeleteDisable(b);
                            break;
                        case ComponentStatus.Disabled:
                            c1.BlockDeleteEnable(b); c2.BlockDeleteEnable(b);
                            break;
                        default:
                            c1.BlockDelete(b); c2.BlockDelete(b);
                            break;
                    }
                }
                World<WorldType>.CurrentQuery.SetCurrentEntity(0);
                #endif
                World<WorldType>.CurrentQuery.UnregisterQuery(qd);
            }
            #if FFS_ECS_DEBUG
            if (World<WorldType>.CurrentQuery.QueryDataCount == 0) {
                World<WorldType>.CurrentQuery.QueryMode = 0;
            }
            #endif
        }
    }
}