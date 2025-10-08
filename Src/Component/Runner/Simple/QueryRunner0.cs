using System;
using System.Buffers;
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
    public readonly struct QueryFunctionRunner<WorldType, P>
        where P : struct, IQueryMethod
        where WorldType : struct, IWorldType {
        internal static readonly QueryFunctionRunner<WorldType, P> Value = default;
        
        [MethodImpl(AggressiveInlining)]
        public bool Search(SearchFunctionWithEntity<WorldType> runner, P with, EntityStatusType entities, QueryMode queryMode, out World<WorldType>.Entity entity) {
            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.cfg.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(with, entities, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;
            var result = false;

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                entity = new World<WorldType>.Entity();
                ref var eid = ref entity._id;
                while (firstGlobalBlockIdx >= 0) {
                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                if (runner(entity)) {
                                    result = true;
                                    goto EXIT;
                                }
                            }
                        }
                    } else {
                        do {
                            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            if (runner(entity)) {
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
                Dispose(with, entities, strict, qd);
            }

            return result;
        }

        [MethodImpl(AggressiveInlining)]
        public void Run<R>(ref R runner, P with, EntityStatusType entities, QueryMode queryMode) where R : struct, World<WorldType>.IQueryFunction {
            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.cfg.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(with, entities, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity._id;
                while (firstGlobalBlockIdx >= 0) {
                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                runner.Run(entity);
                            }
                        }
                    } else {
                        do {
                            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            runner.Run(entity);
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
                Dispose(with, entities, strict, qd);
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run(QueryFunctionWithEntity<WorldType> runner, P with, EntityStatusType entities, QueryMode queryMode) {
            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.cfg.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(with, entities, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity._id;
                while (firstGlobalBlockIdx >= 0) {
                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                runner(entity);
                            }
                        }
                    } else {
                        do {
                            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            runner(entity);
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
                Dispose(with, entities, strict, qd);
            }
        }

        [MethodImpl(AggressiveInlining)]
        public void Run<D>(ref D data, QueryFunctionWithRefDataEntity<D, WorldType> runner, P with, EntityStatusType entities, QueryMode queryMode) {
            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.cfg.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(with, entities, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity._id;
                while (firstGlobalBlockIdx >= 0) {
                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                runner(ref data, entity);
                            }
                        }
                    } else {
                        do {
                            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            runner(ref data, entity);
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
                Dispose(with, entities, strict, qd);
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Prepare(P with, EntityStatusType entities, bool strict, out QueryData qd, out int firstGlobalBlockIdx) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (World<WorldType>.MultiThreadActive) throw new StaticEcsException("Nested query are not available with parallel query");
            #endif

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

                var chunkMask = chE.notEmptyBlocks;
                with.CheckChunk<WorldType>(ref chunkMask, chunkIdx);

                var aMask = chE.entities;

                var dMask = chE.disabledEntities;

                while (chunkMask > 0) {
                    var blockIdx = (uint) deBruijn[(int) (((chunkMask & (ulong) -(long) chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                    chunkMask &= chunkMask - 1;
                    var globalBlockIdx = blockIdx + (chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                    var entitiesMask = entities switch {
                        EntityStatusType.Enabled  => aMask[blockIdx] & ~dMask[blockIdx],
                        EntityStatusType.Disabled => dMask[blockIdx],
                        _                         => aMask[blockIdx]
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
                            }
                            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                            else {
                                const int b = 1;
                                with.BlockQ<WorldType>(b);
                                switch (entities) {
                                    case EntityStatusType.Enabled:  World<WorldType>.Entities.Value.BlockDisable(b); break;
                                    case EntityStatusType.Disabled: World<WorldType>.Entities.Value.BlockEnable(b); break;
                                }
                                World<WorldType>.Entities.Value.BlockDestroy(b);
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
        public void Dispose(P with, EntityStatusType entities, bool strict, QueryData qd) {
            if (qd.Blocks != null) {
                if (!strict) {
                    with.DecQ<WorldType>();
                    switch (entities) {
                        case EntityStatusType.Enabled:  World<WorldType>.Entities.Value.DecQDisable(); break;
                        case EntityStatusType.Disabled: World<WorldType>.Entities.Value.DecQEnable(); break;
                    }
                    World<WorldType>.Entities.Value.DecQDestroy();
                }
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                else {
                    const int b = -1;
                    with.BlockQ<WorldType>(b);
                    switch (entities) {
                        case EntityStatusType.Enabled:  World<WorldType>.Entities.Value.BlockDisable(b); break;
                        case EntityStatusType.Disabled: World<WorldType>.Entities.Value.BlockEnable(b); break;
                    }
                    World<WorldType>.Entities.Value.BlockDestroy(b);
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