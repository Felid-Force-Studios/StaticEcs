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
    internal readonly struct QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, P>
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent
        where WorldType : struct, IWorldType {
        internal static readonly QueryFunctionRunner<WorldType, C1, C2, C3, C4, C5, C6, C7, C8, P> Value = default;
        
        [MethodImpl(AggressiveInlining)]
        internal bool Search(ReadOnlySpan<ushort> clusters, SearchFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode, out World<WorldType>.Entity entity) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;
            ref var c6 = ref World<WorldType>.Components<C6>.Value;
            ref var c7 = ref World<WorldType>.Components<C7>.Value;
            ref var c8 = ref World<WorldType>.Components<C8>.Value;

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
                Span<C3> d3 = default;
                Span<C4> d4 = default;
                Span<C5> d5 = default;
                Span<C6> d6 = default;
                Span<C7> d7 = default;
                Span<C8> d8 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                C3[] d3 = null;
                C4[] d4 = null;
                C5[] d5 = null;
                C6[] d6 = null;
                C7[] d7 = null;
                C8[] d8 = null;
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
                        d3 = new(c3.data[dataIdx]);
                        d4 = new(c4.data[dataIdx]);
                        d5 = new(c5.data[dataIdx]);
                        d6 = new(c6.data[dataIdx]);
                        d7 = new(c7.data[dataIdx]);
                        d8 = new(c8.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
                        d4 = c4.data[dataIdx];
                        d5 = c5.data[dataIdx];
                        d6 = c6.data[dataIdx];
                        d7 = c7.data[dataIdx];
                        d8 = c8.data[dataIdx];
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
                                if (function(entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx], ref d8[dIdx])) {
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
                            if (function(entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx], ref d8[dIdx])) {
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
        internal void Run<R>(ReadOnlySpan<ushort> clusters, ref R function, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode) where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;
            ref var c6 = ref World<WorldType>.Components<C6>.Value;
            ref var c7 = ref World<WorldType>.Components<C7>.Value;
            ref var c8 = ref World<WorldType>.Components<C8>.Value;

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
                Span<C3> d3 = default;
                Span<C4> d4 = default;
                Span<C5> d5 = default;
                Span<C6> d6 = default;
                Span<C7> d7 = default;
                Span<C8> d8 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                C3[] d3 = null;
                C4[] d4 = null;
                C5[] d5 = null;
                C6[] d6 = null;
                C7[] d7 = null;
                C8[] d8 = null;
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
                        d3 = new(c3.data[dataIdx]);
                        d4 = new(c4.data[dataIdx]);
                        d5 = new(c5.data[dataIdx]);
                        d6 = new(c6.data[dataIdx]);
                        d7 = new(c7.data[dataIdx]);
                        d8 = new(c8.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
                        d4 = c4.data[dataIdx];
                        d5 = c5.data[dataIdx];
                        d6 = c6.data[dataIdx];
                        d7 = c7.data[dataIdx];
                        d8 = c8.data[dataIdx];
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
                                function.Invoke(entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx], ref d8[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            function.Invoke(entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx], ref d8[dIdx]);
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
        internal void Run(ReadOnlySpan<ushort> clusters, QueryFunction<C1, C2, C3, C4, C5, C6, C7, C8> function, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;
            ref var c6 = ref World<WorldType>.Components<C6>.Value;
            ref var c7 = ref World<WorldType>.Components<C7>.Value;
            ref var c8 = ref World<WorldType>.Components<C8>.Value;

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
                Span<C3> d3 = default;
                Span<C4> d4 = default;
                Span<C5> d5 = default;
                Span<C6> d6 = default;
                Span<C7> d7 = default;
                Span<C8> d8 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                C3[] d3 = null;
                C4[] d4 = null;
                C5[] d5 = null;
                C6[] d6 = null;
                C7[] d7 = null;
                C8[] d8 = null;
                #endif

                var dataIdx = uint.MaxValue;
                while (firstGlobalBlockIdx >= 0) {
                    if (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = (uint) (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT);
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        d3 = new(c3.data[dataIdx]);
                        d4 = new(c4.data[dataIdx]);
                        d5 = new(c5.data[dataIdx]);
                        d6 = new(c6.data[dataIdx]);
                        d7 = new(c7.data[dataIdx]);
                        d8 = new(c8.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
                        d4 = c4.data[dataIdx];
                        d5 = c5.data[dataIdx];
                        d6 = c6.data[dataIdx];
                        d7 = c7.data[dataIdx];
                        d8 = c8.data[dataIdx];
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
                                function(ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx], ref d8[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            function(ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx], ref d8[dIdx]);
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
        internal void Run<D>(ReadOnlySpan<ushort> clusters, ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8> function, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;
            ref var c6 = ref World<WorldType>.Components<C6>.Value;
            ref var c7 = ref World<WorldType>.Components<C7>.Value;
            ref var c8 = ref World<WorldType>.Components<C8>.Value;

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
                Span<C3> d3 = default;
                Span<C4> d4 = default;
                Span<C5> d5 = default;
                Span<C6> d6 = default;
                Span<C7> d7 = default;
                Span<C8> d8 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                C3[] d3 = null;
                C4[] d4 = null;
                C5[] d5 = null;
                C6[] d6 = null;
                C7[] d7 = null;
                C8[] d8 = null;
                #endif

                var dataIdx = uint.MaxValue;
                while (firstGlobalBlockIdx >= 0) {
                    if (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = (uint) (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT);
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        d3 = new(c3.data[dataIdx]);
                        d4 = new(c4.data[dataIdx]);
                        d5 = new(c5.data[dataIdx]);
                        d6 = new(c6.data[dataIdx]);
                        d7 = new(c7.data[dataIdx]);
                        d8 = new(c8.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
                        d4 = c4.data[dataIdx];
                        d5 = c5.data[dataIdx];
                        d6 = c6.data[dataIdx];
                        d7 = c7.data[dataIdx];
                        d8 = c8.data[dataIdx];
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
                                function(ref data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx], ref d8[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            function(ref data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx], ref d8[dIdx]);
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
        internal void Run(ReadOnlySpan<ushort> clusters, QueryFunctionWithEntity<WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;
            ref var c6 = ref World<WorldType>.Components<C6>.Value;
            ref var c7 = ref World<WorldType>.Components<C7>.Value;
            ref var c8 = ref World<WorldType>.Components<C8>.Value;

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
                Span<C3> d3 = default;
                Span<C4> d4 = default;
                Span<C5> d5 = default;
                Span<C6> d6 = default;
                Span<C7> d7 = default;
                Span<C8> d8 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                C3[] d3 = null;
                C4[] d4 = null;
                C5[] d5 = null;
                C6[] d6 = null;
                C7[] d7 = null;
                C8[] d8 = null;
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
                        d3 = new(c3.data[dataIdx]);
                        d4 = new(c4.data[dataIdx]);
                        d5 = new(c5.data[dataIdx]);
                        d6 = new(c6.data[dataIdx]);
                        d7 = new(c7.data[dataIdx]);
                        d8 = new(c8.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
                        d4 = c4.data[dataIdx];
                        d5 = c5.data[dataIdx];
                        d6 = c6.data[dataIdx];
                        d7 = c7.data[dataIdx];
                        d8 = c8.data[dataIdx];
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
                                function(entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx], ref d8[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            function(entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx], ref d8[dIdx]);
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
        internal void Run<D>(ReadOnlySpan<ushort> clusters, ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8> function, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;
            ref var c6 = ref World<WorldType>.Components<C6>.Value;
            ref var c7 = ref World<WorldType>.Components<C7>.Value;
            ref var c8 = ref World<WorldType>.Components<C8>.Value;

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
                Span<C3> d3 = default;
                Span<C4> d4 = default;
                Span<C5> d5 = default;
                Span<C6> d6 = default;
                Span<C7> d7 = default;
                Span<C8> d8 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                C3[] d3 = null;
                C4[] d4 = null;
                C5[] d5 = null;
                C6[] d6 = null;
                C7[] d7 = null;
                C8[] d8 = null;
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
                        d3 = new(c3.data[dataIdx]);
                        d4 = new(c4.data[dataIdx]);
                        d5 = new(c5.data[dataIdx]);
                        d6 = new(c6.data[dataIdx]);
                        d7 = new(c7.data[dataIdx]);
                        d8 = new(c8.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
                        d4 = c4.data[dataIdx];
                        d5 = c5.data[dataIdx];
                        d6 = c6.data[dataIdx];
                        d7 = c7.data[dataIdx];
                        d8 = c8.data[dataIdx];
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
                                function(ref data, entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx], ref d8[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            function(ref data, entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx], ref d8[dIdx]);
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
            World<WorldType>.AssertRegisteredComponent<C3>(World<WorldType>.Components<C3>.ComponentsTypeName);
            World<WorldType>.AssertRegisteredComponent<C4>(World<WorldType>.Components<C4>.ComponentsTypeName);
            World<WorldType>.AssertRegisteredComponent<C5>(World<WorldType>.Components<C5>.ComponentsTypeName);
            World<WorldType>.AssertRegisteredComponent<C6>(World<WorldType>.Components<C6>.ComponentsTypeName);
            World<WorldType>.AssertRegisteredComponent<C7>(World<WorldType>.Components<C7>.ComponentsTypeName);
            World<WorldType>.AssertRegisteredComponent<C8>(World<WorldType>.Components<C8>.ComponentsTypeName);
            with.Assert<WorldType>();
            #endif

            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;
            ref var c6 = ref World<WorldType>.Components<C6>.Value;
            ref var c7 = ref World<WorldType>.Components<C7>.Value;
            ref var c8 = ref World<WorldType>.Components<C8>.Value;

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
                    ref var ch3 = ref c3.chunks[chunkIdx];
                    ref var ch4 = ref c4.chunks[chunkIdx];
                    ref var ch5 = ref c5.chunks[chunkIdx];
                    ref var ch6 = ref c6.chunks[chunkIdx];
                    ref var ch7 = ref c7.chunks[chunkIdx];
                    ref var ch8 = ref c8.chunks[chunkIdx];

                    var chunkMask = chE.notEmptyBlocks
                                    & ch1.notEmptyBlocks
                                    & ch2.notEmptyBlocks
                                    & ch3.notEmptyBlocks
                                    & ch4.notEmptyBlocks
                                    & ch5.notEmptyBlocks
                                    & ch6.notEmptyBlocks
                                    & ch7.notEmptyBlocks
                                    & ch8.notEmptyBlocks;
                    with.CheckChunk<WorldType>(ref chunkMask, chunkIdx);

                    var lMask = chE.loadedEntities;
                    var aMask = chE.entities;
                    var aMask1 = ch1.entities;
                    var aMask2 = ch2.entities;
                    var aMask3 = ch3.entities;
                    var aMask4 = ch4.entities;
                    var aMask5 = ch5.entities;
                    var aMask6 = ch6.entities;
                    var aMask7 = ch7.entities;
                    var aMask8 = ch8.entities;

                    var dMask = chE.disabledEntities;
                    var dMask1 = ch1.disabledEntities;
                    var dMask2 = ch2.disabledEntities;
                    var dMask3 = ch3.disabledEntities;
                    var dMask4 = ch4.disabledEntities;
                    var dMask5 = ch5.disabledEntities;
                    var dMask6 = ch6.disabledEntities;
                    var dMask7 = ch7.disabledEntities;
                    var dMask8 = ch8.disabledEntities;

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
                            ComponentStatus.Enabled => aMask1[blockIdx] & ~dMask1[blockIdx] & aMask2[blockIdx] & ~dMask2[blockIdx] & aMask3[blockIdx] & ~dMask3[blockIdx] & aMask4[blockIdx] & ~dMask4[blockIdx] & aMask5[blockIdx] &
                                                       ~dMask5[blockIdx] & aMask6[blockIdx] & ~dMask6[blockIdx] & aMask7[blockIdx] & ~dMask7[blockIdx] & aMask8[blockIdx] & ~dMask8[blockIdx],
                            ComponentStatus.Disabled => dMask1[blockIdx] & dMask2[blockIdx] & dMask3[blockIdx] & dMask4[blockIdx] & dMask5[blockIdx] & dMask6[blockIdx] & dMask7[blockIdx] & dMask8[blockIdx],
                            _                        => aMask1[blockIdx] & aMask2[blockIdx] & aMask3[blockIdx] & aMask4[blockIdx] & aMask5[blockIdx] & aMask6[blockIdx] & aMask7[blockIdx] & aMask8[blockIdx],
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
                                        case ComponentStatus.Enabled: c1.IncQDeleteDisable(qd); c2.IncQDeleteDisable(qd); c3.IncQDeleteDisable(qd); c4.IncQDeleteDisable(qd); c5.IncQDeleteDisable(qd); c6.IncQDeleteDisable(qd); c7.IncQDeleteDisable(qd); c8.IncQDeleteDisable(qd);
                                            break;
                                        case ComponentStatus.Disabled: c1.IncQDeleteEnable(qd); c2.IncQDeleteEnable(qd); c3.IncQDeleteEnable(qd); c4.IncQDeleteEnable(qd); c5.IncQDeleteEnable(qd); c6.IncQDeleteEnable(qd); c7.IncQDeleteEnable(qd); c8.IncQDeleteEnable(qd);
                                            break;
                                        default: c1.IncQDelete(qd); c2.IncQDelete(qd); c3.IncQDelete(qd); c4.IncQDelete(qd); c5.IncQDelete(qd); c6.IncQDelete(qd); c7.IncQDelete(qd); c8.IncQDelete(qd);
                                            break;
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
                                        case ComponentStatus.Enabled: c1.BlockDeleteDisable(b); c2.BlockDeleteDisable(b); c3.BlockDeleteDisable(b); c4.BlockDeleteDisable(b); c5.BlockDeleteDisable(b); c6.BlockDeleteDisable(b); c7.BlockDeleteDisable(b); c8.BlockDeleteDisable(b);
                                            break;
                                        case ComponentStatus.Disabled: c1.BlockDeleteEnable(b); c2.BlockDeleteEnable(b); c3.BlockDeleteEnable(b); c4.BlockDeleteEnable(b); c5.BlockDeleteEnable(b); c6.BlockDeleteEnable(b); c7.BlockDeleteEnable(b); c8.BlockDeleteEnable(b);
                                            break;
                                        default: c1.BlockDelete(b); c2.BlockDelete(b); c3.BlockDelete(b); c4.BlockDelete(b); c5.BlockDelete(b); c6.BlockDelete(b); c7.BlockDelete(b); c8.BlockDelete(b);
                                            break;
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
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;
            ref var c6 = ref World<WorldType>.Components<C6>.Value;
            ref var c7 = ref World<WorldType>.Components<C7>.Value;
            ref var c8 = ref World<WorldType>.Components<C8>.Value;

            if (qd.Blocks != null) {
                if (!strict) {
                    with.DecQ<WorldType>();
                    switch (entities) {
                        case EntityStatusType.Enabled:  World<WorldType>.Entities.Value.DecQDisable(); break;
                        case EntityStatusType.Disabled: World<WorldType>.Entities.Value.DecQEnable(); break;
                    }
                    World<WorldType>.Entities.Value.DecQDestroy();
                    switch (components) {
                        case ComponentStatus.Enabled: c1.DecQDeleteDisable(); c2.DecQDeleteDisable(); c3.DecQDeleteDisable(); c4.DecQDeleteDisable(); c5.DecQDeleteDisable(); c6.DecQDeleteDisable(); c7.DecQDeleteDisable(); c8.DecQDeleteDisable(); break;
                        case ComponentStatus.Disabled: c1.DecQDeleteEnable(); c2.DecQDeleteEnable(); c3.DecQDeleteEnable(); c4.DecQDeleteEnable(); c5.DecQDeleteEnable(); c6.DecQDeleteEnable(); c7.DecQDeleteEnable(); c8.DecQDeleteEnable(); break;
                        default: c1.DecQDelete(); c2.DecQDelete(); c3.DecQDelete(); c4.DecQDelete(); c5.DecQDelete(); c6.DecQDelete(); c7.DecQDelete(); c8.DecQDelete(); break;
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
                            c1.BlockDeleteDisable(b); c2.BlockDeleteDisable(b); c3.BlockDeleteDisable(b); c4.BlockDeleteDisable(b); c5.BlockDeleteDisable(b); c6.BlockDeleteDisable(b); c7.BlockDeleteDisable(b); c8.BlockDeleteDisable(b);
                            break;
                        case ComponentStatus.Disabled:
                            c1.BlockDeleteEnable(b); c2.BlockDeleteEnable(b); c3.BlockDeleteEnable(b); c4.BlockDeleteEnable(b); c5.BlockDeleteEnable(b); c6.BlockDeleteEnable(b); c7.BlockDeleteEnable(b); c8.BlockDeleteEnable(b);
                            break;
                        default:
                            c1.BlockDelete(b); c2.BlockDelete(b); c3.BlockDelete(b); c4.BlockDelete(b); c5.BlockDelete(b); c6.BlockDelete(b); c7.BlockDelete(b); c8.BlockDelete(b);
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