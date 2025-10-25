#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

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
    internal readonly struct QueryFunctionRunner<WorldType, P>
        where P : struct, IQueryMethod
        where WorldType : struct, IWorldType {
        internal static readonly QueryFunctionRunner<WorldType, P> Value = default;
        
        [MethodImpl(AggressiveInlining)]
        internal bool Search(ReadOnlySpan<ushort> clusters, SearchFunctionWithEntity<WorldType> function, P with, EntityStatusType entities, QueryMode queryMode, out World<WorldType>.Entity entity) {
            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.config.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(clusters, with, entities, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;
            var result = false;

            #if FFS_ECS_DEBUG
            try
            #endif
            {
                entity = new World<WorldType>.Entity();
                ref var eid = ref entity.id;
                while (firstGlobalBlockIdx >= 0) {
                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Const.ENTITY_ID_OFFSET;
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                #if FFS_ECS_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                if (function(entity)) {
                                    result = true;
                                    goto EXIT;
                                }
                            }
                        }
                    } else {
                        do {
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            if (function(entity)) {
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
                Dispose(with, entities, strict, qd);
            }

            return result;
        }

        [MethodImpl(AggressiveInlining)]
        internal void Run<R>(ReadOnlySpan<ushort> clusters, ref R function, P with, EntityStatusType entities, QueryMode queryMode) where R : struct, World<WorldType>.IQueryFunction {
            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.config.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(clusters, with, entities, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if FFS_ECS_DEBUG
            try
            #endif
            {
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity.id;
                while (firstGlobalBlockIdx >= 0) {
                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Const.ENTITY_ID_OFFSET;
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                #if FFS_ECS_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                function.Invoke(entity);
                            }
                        }
                    } else {
                        do {
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            function.Invoke(entity);
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
                Dispose(with, entities, strict, qd);
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal void Run(ReadOnlySpan<ushort> clusters, QueryFunctionWithEntity<WorldType> function, P with, EntityStatusType entities, QueryMode queryMode) {
            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.config.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(clusters, with, entities, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if FFS_ECS_DEBUG
            try
            #endif
            {
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity.id;
                while (firstGlobalBlockIdx >= 0) {
                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Const.ENTITY_ID_OFFSET;
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                #if FFS_ECS_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                function(entity);
                            }
                        }
                    } else {
                        do {
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            function(entity);
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
                Dispose(with, entities, strict, qd);
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal void Run(ReadOnlySpan<uint> chunks, QueryFunctionWithEntity<WorldType> function, P with, EntityStatusType entities, QueryMode queryMode) {
            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.config.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(chunks, with, entities, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if FFS_ECS_DEBUG
            try
            #endif
            {
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity.id;
                while (firstGlobalBlockIdx >= 0) {
                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Const.ENTITY_ID_OFFSET;
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                #if FFS_ECS_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                function(entity);
                            }
                        }
                    } else {
                        do {
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            function(entity);
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
                Dispose(with, entities, strict, qd);
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal void Run<D>(ReadOnlySpan<uint> chunks, ref D data, QueryFunctionWithRefDataEntity<D, WorldType> function, P with, EntityStatusType entities, QueryMode queryMode) {
            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.config.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(chunks, with, entities, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if FFS_ECS_DEBUG
            try
            #endif
            {
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity.id;
                while (firstGlobalBlockIdx >= 0) {
                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Const.ENTITY_ID_OFFSET;
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                #if FFS_ECS_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                function(ref data, entity);
                            }
                        }
                    } else {
                        do {
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            function(ref data, entity);
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
                Dispose(with, entities, strict, qd);
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal void Run<D>(ReadOnlySpan<ushort> clusters, ref D data, QueryFunctionWithRefDataEntity<D, WorldType> function, P with, EntityStatusType entities, QueryMode queryMode) {
            var strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.config.DefaultQueryModeStrict);
            
            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            
            Prepare(clusters, with, entities, strict, out var qd, out var firstGlobalBlockIdx);
            var filteredBlocks = qd.Blocks;

            #if FFS_ECS_DEBUG
            try
            #endif
            {
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity.id;
                while (firstGlobalBlockIdx >= 0) {
                    ref var entitiesMaskRef = ref filteredBlocks[firstGlobalBlockIdx].EntitiesMask;
                    var entitiesMask = entitiesMaskRef;
                    var blockEntity = (uint) (firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Const.ENTITY_ID_OFFSET;
                    firstGlobalBlockIdx = filteredBlocks[firstGlobalBlockIdx].NextGlobalBlock;
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMaskRef & (1UL << idx)) > 0) {
                                #if FFS_ECS_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                eid = blockEntity + idx;
                                function(ref data, entity);
                            }
                        }
                    } else {
                        do {
                            #if FFS_ECS_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            eid = blockEntity + idx;
                            function(ref data, entity);
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
                Dispose(with, entities, strict, qd);
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void Prepare(ReadOnlySpan<ushort> clusters, P with, EntityStatusType entities, bool strict, out QueryData qd, out int firstGlobalBlockIdx) {
            #if FFS_ECS_DEBUG
            World<WorldType>.AssertNotNestedParallelQuery(World<WorldType>.WorldTypeName);
            with.Assert<WorldType>();
            #endif

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

                    var chunkMask = chE.notEmptyBlocks;
                    with.CheckChunk<WorldType>(ref chunkMask, chunkIdx);

                    var lMask = chE.loadedEntities;
                    var aMask = chE.entities;

                    var dMask = chE.disabledEntities;

                    while (chunkMask > 0) {
                        var blockIdx = (uint) deBruijn[(int) (((chunkMask & (ulong) -(long) chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                        chunkMask &= chunkMask - 1;
                        var globalBlockIdx = blockIdx + (chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                        var entitiesMask = entities switch {
                            EntityStatusType.Enabled  => lMask[blockIdx] & aMask[blockIdx] & ~dMask[blockIdx],
                            EntityStatusType.Disabled => lMask[blockIdx] & dMask[blockIdx],
                            _                         => lMask[blockIdx] & aMask[blockIdx]
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
                                #if FFS_ECS_DEBUG
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
        }

        [MethodImpl(AggressiveInlining)]
        internal void Prepare(ReadOnlySpan<uint> chunks, P with, EntityStatusType entities, bool strict, out QueryData qd, out int firstGlobalBlockIdx) {
            #if FFS_ECS_DEBUG
            World<WorldType>.AssertNotNestedParallelQuery(World<WorldType>.WorldTypeName);
            with.Assert<WorldType>();
            #endif

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

            for (var chunkMapIdx = 0; chunkMapIdx < chunks.Length; chunkMapIdx++) {
                var chunkIdx = chunks[chunkMapIdx];

                ref var chE = ref entitiesChunks[chunkIdx];

                var chunkMask = chE.notEmptyBlocks;
                with.CheckChunk<WorldType>(ref chunkMask, chunkIdx);

                var lMask = chE.loadedEntities;
                var aMask = chE.entities;

                var dMask = chE.disabledEntities;

                while (chunkMask > 0) {
                    var blockIdx = (uint) deBruijn[(int) (((chunkMask & (ulong) -(long) chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                    chunkMask &= chunkMask - 1;
                    var globalBlockIdx = blockIdx + (chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                    var entitiesMask = entities switch {
                        EntityStatusType.Enabled  => lMask[blockIdx] & aMask[blockIdx] & ~dMask[blockIdx],
                        EntityStatusType.Disabled => lMask[blockIdx] & dMask[blockIdx],
                        _                         => lMask[blockIdx] & aMask[blockIdx]
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
                            #if FFS_ECS_DEBUG
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
        internal void Dispose(P with, EntityStatusType entities, bool strict, QueryData qd) {
            if (qd.Blocks != null) {
                if (!strict) {
                    with.DecQ<WorldType>();
                    switch (entities) {
                        case EntityStatusType.Enabled:  World<WorldType>.Entities.Value.DecQDisable(); break;
                        case EntityStatusType.Disabled: World<WorldType>.Entities.Value.DecQEnable(); break;
                    }
                    World<WorldType>.Entities.Value.DecQDestroy();
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