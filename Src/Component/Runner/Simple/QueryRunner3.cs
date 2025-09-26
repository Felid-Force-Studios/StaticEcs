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
    public readonly struct QueryFunctionRunner<WorldType, C1, C2, C3, P>
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where WorldType : struct, IWorldType {
        internal static readonly QueryFunctionRunner<WorldType, C1, C2, C3, P> Value = default;
        
        [MethodImpl(AggressiveInlining)]
        public bool Search(SearchFunctionWithEntity<WorldType, C1, C2, C3> runner, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode, out World<WorldType>.Entity entity) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;

            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.cfg.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(with, entities, components, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;
            var result = false;

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                #if NET6_0_OR_GREATER
                Span<C1> d1 = default;
                Span<C2> d2 = default;
                Span<C3> d3 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                C3[] d3 = null;
                #endif

                var dataIdx = uint.MaxValue;
                entity = new World<WorldType>.Entity();
                ref var eid = ref entity._id;
                while (firstGlobalBlockIdx >= 0) {
                    if (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = (uint) (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT);
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        d3 = new(c3.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
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
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if DEBUG || EFS_ECS_ENABLE_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                if (runner(entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx])) {
                                    result = true;
                                    goto EXIT;
                                }
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            if (runner(entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx])) {
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
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            finally
            #endif
            {
                Dispose(with, entities, components, strict, qd);
            }
            return result;
        }

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode) where R : struct, World<WorldType>.IQueryFunction<C1, C2, C3> {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;

            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.cfg.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(with, entities, components, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                #if NET6_0_OR_GREATER
                Span<C1> d1 = default;
                Span<C2> d2 = default;
                Span<C3> d3 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                C3[] d3 = null;
                #endif

                var dataIdx = uint.MaxValue;
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity._id;
                while (firstGlobalBlockIdx >= 0) {
                    if (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = (uint) (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT);
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        d3 = new(c3.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
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
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if DEBUG || EFS_ECS_ENABLE_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                runner.Run(entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            runner.Run(entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMaskRef;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            finally
            #endif
            {
                Dispose(with, entities, components, strict, qd);
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunction<C1, C2, C3> runner, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;

            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.cfg.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(with, entities, components, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                #if NET6_0_OR_GREATER
                Span<C1> d1 = default;
                Span<C2> d2 = default;
                Span<C3> d3 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                C3[] d3 = null;
                #endif

                var dataIdx = uint.MaxValue;
                while (firstGlobalBlockIdx >= 0) {
                    if (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = (uint) (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT);
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        d3 = new(c3.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
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
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if DEBUG || EFS_ECS_ENABLE_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                runner(ref d1[dIdx], ref d2[dIdx], ref d3[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            runner(ref d1[dIdx], ref d2[dIdx], ref d3[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMaskRef;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            finally
            #endif
            {
                Dispose(with, entities, components, strict, qd);
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefData<D, C1, C2, C3> runner, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;

            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.cfg.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(with, entities, components, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                #if NET6_0_OR_GREATER
                Span<C1> d1 = default;
                Span<C2> d2 = default;
                Span<C3> d3 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                C3[] d3 = null;
                #endif

                var dataIdx = uint.MaxValue;
                while (firstGlobalBlockIdx >= 0) {
                    if (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = (uint) (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT);
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        d3 = new(c3.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
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
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if DEBUG || EFS_ECS_ENABLE_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                runner(ref data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            runner(ref data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMaskRef;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            finally
            #endif
            {
                Dispose(with, entities, components, strict, qd);
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunctionWithEntity<WorldType, C1, C2, C3> runner, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;

            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.cfg.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(with, entities, components, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                #if NET6_0_OR_GREATER
                Span<C1> d1 = default;
                Span<C2> d2 = default;
                Span<C3> d3 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                C3[] d3 = null;
                #endif

                var dataIdx = uint.MaxValue;
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity._id;
                while (firstGlobalBlockIdx >= 0) {
                    if (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = (uint) (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT);
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        d3 = new(c3.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
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
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if DEBUG || EFS_ECS_ENABLE_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                runner(entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            runner(entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMaskRef;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            finally
            #endif
            {
                Dispose(with, entities, components, strict, qd);
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType, C1, C2, C3> runner, P with, EntityStatusType entities, ComponentStatus components, QueryMode queryMode) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;

            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.cfg.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(with, entities, components, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                #if NET6_0_OR_GREATER
                Span<C1> d1 = default;
                Span<C2> d2 = default;
                Span<C3> d3 = default;
                #else
                C1[] d1 = null;
                C2[] d2 = null;
                C3[] d3 = null;
                #endif

                var dataIdx = uint.MaxValue;
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity._id;
                while (firstGlobalBlockIdx >= 0) {
                    if (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = (uint) (firstGlobalBlockIdx >> Const.DATA_QUERY_SHIFT);
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        d3 = new(c3.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
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
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if DEBUG || EFS_ECS_ENABLE_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                runner(ref data, entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx]);
                                
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            runner(ref data, entity, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMaskRef;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            finally
            #endif
            {
                Dispose(with, entities, components, strict, qd);
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Prepare(P with, EntityStatusType entities, ComponentStatus components, bool strict, out QueryData qd, out int firstGlobalBlockIdx) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (World<WorldType>.MultiThreadActive) throw new StaticEcsException("Nested query are not available with parallel query");
            #endif

            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            
            qd = default;
            
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            var modeVal = strict ? (byte) 1 : (byte) 2;
            if (World<WorldType>.CurrentQuery.QueryMode != 0 && modeVal != World<WorldType>.CurrentQuery.QueryMode) {
                throw new StaticEcsException("Nested iterators must have the same QueryMode as the outer iterator");
            }

            World<WorldType>.CurrentQuery.QueryMode = modeVal;
            #endif

            var chunksCount = World<WorldType>.Entities.Value.nextActiveChunkIdx;
            var entitiesChunks = World<WorldType>.Entities.Value.chunks;
            BlockMaskCache[] filteredBlocks = null;

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif

            var previousGlobalBlockIdx = -1;
            firstGlobalBlockIdx = -1;
            for (uint chunkIdx = 0; chunkIdx < chunksCount; chunkIdx++) {
                ref var chE = ref entitiesChunks[chunkIdx];
                ref var ch1 = ref c1.chunks[chunkIdx];
                ref var ch2 = ref c2.chunks[chunkIdx];
                ref var ch3 = ref c3.chunks[chunkIdx];

                var chunkMask = chE.notEmptyBlocks
                                & ch1.notEmptyBlocks
                                & ch2.notEmptyBlocks
                                & ch3.notEmptyBlocks;
                with.CheckChunk<WorldType>(ref chunkMask, chunkIdx);

                var aMask = chE.entities;
                var aMask1 = ch1.entities;
                var aMask2 = ch2.entities;
                var aMask3 = ch3.entities;

                var dMask = chE.disabledEntities;
                var dMask1 = ch1.disabledEntities;
                var dMask2 = ch2.disabledEntities;
                var dMask3 = ch3.disabledEntities;

                while (chunkMask > 0) {
                    var blockIdx = (uint) deBruijn[(int) (((chunkMask & (ulong) -(long) chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                    chunkMask &= chunkMask - 1;
                    var globalBlockIdx = blockIdx + (chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                    var entitiesMask = entities switch {
                        EntityStatusType.Enabled  => aMask[blockIdx] & ~dMask[blockIdx],
                        EntityStatusType.Disabled => dMask[blockIdx],
                        _                         => aMask[blockIdx]
                    };
                    entitiesMask &= components switch {
                        ComponentStatus.Enabled => aMask1[blockIdx] & ~dMask1[blockIdx] & aMask2[blockIdx] & ~dMask2[blockIdx] & aMask3[blockIdx] & ~dMask3[blockIdx],
                        ComponentStatus.Disabled => dMask1[blockIdx] & dMask2[blockIdx] & dMask3[blockIdx],
                        _                        => aMask1[blockIdx] & aMask2[blockIdx] & aMask3[blockIdx],
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
                                    case ComponentStatus.Enabled: c1.IncQDeleteDisable(qd); c2.IncQDeleteDisable(qd); c3.IncQDeleteDisable(qd); break;
                                    case ComponentStatus.Disabled: c1.IncQDeleteEnable(qd); c2.IncQDeleteEnable(qd); c3.IncQDeleteEnable(qd); break;
                                    default: c1.IncQDelete(qd); c2.IncQDelete(qd); c3.IncQDelete(qd); break;
                                }
                            }
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            else {
                                const int b = 1;
                                with.BlockQ<WorldType>(b);
                                switch (entities) {
                                    case EntityStatusType.Enabled:  World<WorldType>.Entities.Value.BlockDisable(b); break;
                                    case EntityStatusType.Disabled: World<WorldType>.Entities.Value.BlockEnable(b); break;
                                }
                                World<WorldType>.Entities.Value.BlockDestroy(b);
                                switch (components) {
                                    case ComponentStatus.Enabled: c1.BlockDeleteDisable(b); c2.BlockDeleteDisable(b); c3.BlockDeleteDisable(b); break;
                                    case ComponentStatus.Disabled: c1.BlockDeleteEnable(b); c2.BlockDeleteEnable(b); c3.BlockDeleteEnable(b); break;
                                    default: c1.BlockDelete(b); c2.BlockDelete(b); c3.BlockDelete(b); break;
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
        
        [MethodImpl(AggressiveInlining)]
        public void Dispose(P with, EntityStatusType entities, ComponentStatus components, bool strict, QueryData qd) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;

            if (qd.Blocks != null) {
                if (!strict) {
                    with.DecQ<WorldType>();
                    switch (entities) {
                        case EntityStatusType.Enabled:  World<WorldType>.Entities.Value.DecQDisable(); break;
                        case EntityStatusType.Disabled: World<WorldType>.Entities.Value.DecQEnable(); break;
                    }
                    World<WorldType>.Entities.Value.DecQDestroy();
                    switch (components) {
                        case ComponentStatus.Enabled: c1.DecQDeleteDisable(); c2.DecQDeleteDisable(); c3.DecQDeleteDisable(); break;
                        case ComponentStatus.Disabled: c1.DecQDeleteEnable(); c2.DecQDeleteEnable(); c3.DecQDeleteEnable(); break;
                        default: c1.DecQDelete(); c2.DecQDelete(); c3.DecQDelete(); break;
                    }
                }
                #if DEBUG || EFS_ECS_ENABLE_DEBUG
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
                            c1.BlockDeleteDisable(b); c2.BlockDeleteDisable(b); c3.BlockDeleteDisable(b);
                            break;
                        case ComponentStatus.Disabled:
                            c1.BlockDeleteEnable(b); c2.BlockDeleteEnable(b); c3.BlockDeleteEnable(b);
                            break;
                        default:
                            c1.BlockDelete(b); c2.BlockDelete(b); c3.BlockDelete(b);
                            break;
                    }
                }
                World<WorldType>.CurrentQuery.SetCurrentEntity(0);
                #endif
                World<WorldType>.CurrentQuery.UnregisterQuery(qd);
            }
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (World<WorldType>.CurrentQuery.QueryDataCount == 0) {
                World<WorldType>.CurrentQuery.QueryMode = 0;
            }
            #endif
        }
    }
}