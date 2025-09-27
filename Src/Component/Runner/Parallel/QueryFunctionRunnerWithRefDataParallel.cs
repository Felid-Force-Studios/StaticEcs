using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    #region QUERY_FUNCTION_RUNNER

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public sealed unsafe class QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, P> : AbstractParallelTask
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private QueryFunctionWithRefData<D, C1> _runner;
        private Job[] _jobs;
        private uint[] _jobIndexes;
        private D _data;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefData<D, C1> runner, P with, EntityStatusType entities, ComponentStatus components, uint minEntitiesPerThread, uint workersLimit) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (World<WorldType>.MultiThreadActive) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.CurrentQuery.QueryDataCount != 0) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.cfg.ParallelQueryType == ParallelQueryType.Disabled) throw new StaticEcsException("ParallelQueryType = Disabled, change World config");
            #endif
            World<WorldType>.CurrentQuery.QueryDataCount++;

            _runner = runner;
            _data = data;
            
            Prepare(with, entities, components, out var count);

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                ParallelRunner<WorldType>.Run(this, count, Math.Max(minEntitiesPerThread / Const.DATA_BLOCK_SIZE_FOR_THREADS, 1), workersLimit);;
                data = _data;
            }
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            finally
            #endif
            {
                if (count > 0) {
                    ArrayPool<Job>.Shared.Return(_jobs);
                    ArrayPool<uint>.Shared.Return(_jobIndexes);
                    _jobs = null;
                    _jobIndexes = null;
                }
                World<WorldType>.CurrentQuery.QueryDataCount--;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (World<WorldType>.CurrentQuery.QueryDataCount == 0) {
                    World<WorldType>.CurrentQuery.QueryMode = 0;
                }
                #endif
                _runner = null;
                _data = default;
            }
        }

        public override void Run(uint from, uint to) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            
            #if NET6_0_OR_GREATER
            Span<C1> d1 = default;
            #else
            C1[] d1 = null;
            #endif

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            var jobs = _jobs;
            var jobIndexes = _jobIndexes;

            var dataIdx = uint.MaxValue;
            while (from < to) {
                ref var job = ref jobs[jobIndexes[from++]];
                var count = job.Count;
                job.Count = 0;

                for (uint i = 0; i < count; i++) {
                    var entitiesMask = job.Masks[i];
                    var globalBlockIdx = job.GlobalBlockIdx[i];

                    if (globalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT;
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        #endif
                    }

                    var blockEntity = globalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT;
                    #if NET6_0_OR_GREATER
                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                    #else
                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                    #endif
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMask & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if DEBUG || EFS_ECS_ENABLE_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                _runner(ref _data, ref d1[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            _runner(ref _data, ref d1[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMask;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }
            #if DEBUG || EFS_ECS_ENABLE_DEBUG
            World<WorldType>.CurrentQuery.SetCurrentEntity(0);
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Prepare(P with, EntityStatusType entities, ComponentStatus components, out uint jobsCount) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;

            var chunksCount = World<WorldType>.Entities.Value.nextActiveChunkIdx;
            var entitiesChunks = World<WorldType>.Entities.Value.chunks;

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif

            jobsCount = 0;
            for (uint chunkIdx = 0; chunkIdx < chunksCount; chunkIdx++) {
                ref var chE = ref entitiesChunks[chunkIdx];
                ref var ch1 = ref c1.chunks[chunkIdx];

                var chunkMask = chE.notEmptyBlocks
                                & ch1.notEmptyBlocks;
                with.CheckChunk<WorldType>(ref chunkMask, chunkIdx);

                var aMask = chE.entities;
                var aMask1 = ch1.entities;

                var dMask = chE.disabledEntities;
                var dMask1 = ch1.disabledEntities;

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
                        ComponentStatus.Enabled => aMask1[blockIdx] & ~dMask1[blockIdx],
                        ComponentStatus.Disabled => dMask1[blockIdx],
                        _                        => aMask1[blockIdx],
                    };
                    with.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, (int) blockIdx);

                    if (entitiesMask > 0) {
                        if (jobsCount == 0) {
                            var size = (int) (chunksCount * Const.DATA_BLOCK_IN_CHUNK_FOR_THREADS);
                            _jobs = ArrayPool<Job>.Shared.Rent(size);
                            _jobIndexes = ArrayPool<uint>.Shared.Rent(size);
                        }

                        var jobIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT_FOR_THREADS;
                        ref var job = ref _jobs[jobIdx];
                        if (job.Count == 0) {
                            _jobIndexes[jobsCount++] = jobIdx;
                        }
                        job.Masks[job.Count] = entitiesMask;
                        job.GlobalBlockIdx[job.Count++] = globalBlockIdx;
                    }
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public sealed unsafe class QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, P> : AbstractParallelTask
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private QueryFunctionWithRefData<D, C1, C2> _runner;
        private Job[] _jobs;
        private uint[] _jobIndexes;
        private D _data;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefData<D, C1, C2> runner, P with, EntityStatusType entities, ComponentStatus components, uint minEntitiesPerThread, uint workersLimit) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (World<WorldType>.MultiThreadActive) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.CurrentQuery.QueryDataCount != 0) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.cfg.ParallelQueryType == ParallelQueryType.Disabled) throw new StaticEcsException("ParallelQueryType = Disabled, change World config");
            #endif
            World<WorldType>.CurrentQuery.QueryDataCount++;

            _runner = runner;
            _data = data;
            
            Prepare(with, entities, components, out var count);

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                ParallelRunner<WorldType>.Run(this, count, Math.Max(minEntitiesPerThread / Const.DATA_BLOCK_SIZE_FOR_THREADS, 1), workersLimit);;
                data = _data;
            }
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            finally
            #endif
            {
                if (count > 0) {
                    ArrayPool<Job>.Shared.Return(_jobs);
                    ArrayPool<uint>.Shared.Return(_jobIndexes);
                    _jobs = null;
                    _jobIndexes = null;
                }
                World<WorldType>.CurrentQuery.QueryDataCount--;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (World<WorldType>.CurrentQuery.QueryDataCount == 0) {
                    World<WorldType>.CurrentQuery.QueryMode = 0;
                }
                #endif
                _runner = null;
                _data = default;
            }
        }

        public override void Run(uint from, uint to) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            
            #if NET6_0_OR_GREATER
            Span<C1> d1 = default;
            Span<C2> d2 = default;
            #else
            C1[] d1 = null;
            C2[] d2 = null;
            #endif

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            var jobs = _jobs;
            var jobIndexes = _jobIndexes;

            var dataIdx = uint.MaxValue;
            while (from < to) {
                ref var job = ref jobs[jobIndexes[from++]];
                var count = job.Count;
                job.Count = 0;

                for (uint i = 0; i < count; i++) {
                    var entitiesMask = job.Masks[i];
                    var globalBlockIdx = job.GlobalBlockIdx[i];

                    if (globalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT;
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        #endif
                    }

                    var blockEntity = globalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT;
                    #if NET6_0_OR_GREATER
                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                    #else
                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                    #endif
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMask & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if DEBUG || EFS_ECS_ENABLE_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                _runner(ref _data, ref d1[dIdx], ref d2[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            _runner(ref _data, ref d1[dIdx], ref d2[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMask;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }
            #if DEBUG || EFS_ECS_ENABLE_DEBUG
            World<WorldType>.CurrentQuery.SetCurrentEntity(0);
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Prepare(P with, EntityStatusType entities, ComponentStatus components, out uint jobsCount) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;

            var chunksCount = World<WorldType>.Entities.Value.nextActiveChunkIdx;
            var entitiesChunks = World<WorldType>.Entities.Value.chunks;

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif

            jobsCount = 0;
            for (uint chunkIdx = 0; chunkIdx < chunksCount; chunkIdx++) {
                ref var chE = ref entitiesChunks[chunkIdx];
                ref var ch1 = ref c1.chunks[chunkIdx];
                ref var ch2 = ref c2.chunks[chunkIdx];

                var chunkMask = chE.notEmptyBlocks
                                & ch1.notEmptyBlocks
                                & ch2.notEmptyBlocks;
                with.CheckChunk<WorldType>(ref chunkMask, chunkIdx);

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
                        EntityStatusType.Enabled  => aMask[blockIdx] & ~dMask[blockIdx],
                        EntityStatusType.Disabled => dMask[blockIdx],
                        _                         => aMask[blockIdx]
                    };
                    entitiesMask &= components switch {
                        ComponentStatus.Enabled => aMask1[blockIdx] & ~dMask1[blockIdx] & aMask2[blockIdx] & ~dMask2[blockIdx],
                        ComponentStatus.Disabled => dMask1[blockIdx] & dMask2[blockIdx],
                        _                        => aMask1[blockIdx] & aMask2[blockIdx],
                    };
                    with.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, (int) blockIdx);

                    if (entitiesMask > 0) {
                        if (jobsCount == 0) {
                            var size = (int) (chunksCount * Const.DATA_BLOCK_IN_CHUNK_FOR_THREADS);
                            _jobs = ArrayPool<Job>.Shared.Rent(size);
                            _jobIndexes = ArrayPool<uint>.Shared.Rent(size);
                        }

                        var jobIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT_FOR_THREADS;
                        ref var job = ref _jobs[jobIdx];
                        if (job.Count == 0) {
                            _jobIndexes[jobsCount++] = jobIdx;
                        }
                        job.Masks[job.Count] = entitiesMask;
                        job.GlobalBlockIdx[job.Count++] = globalBlockIdx;
                    }
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public sealed unsafe class QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, P> : AbstractParallelTask
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private QueryFunctionWithRefData<D, C1, C2, C3> _runner;
        private Job[] _jobs;
        private uint[] _jobIndexes;
        private D _data;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefData<D, C1, C2, C3> runner, P with, EntityStatusType entities, ComponentStatus components, uint minEntitiesPerThread, uint workersLimit) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (World<WorldType>.MultiThreadActive) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.CurrentQuery.QueryDataCount != 0) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.cfg.ParallelQueryType == ParallelQueryType.Disabled) throw new StaticEcsException("ParallelQueryType = Disabled, change World config");
            #endif
            World<WorldType>.CurrentQuery.QueryDataCount++;

            _runner = runner;
            _data = data;
            
            Prepare(with, entities, components, out var count);

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                ParallelRunner<WorldType>.Run(this, count, Math.Max(minEntitiesPerThread / Const.DATA_BLOCK_SIZE_FOR_THREADS, 1), workersLimit);;
                data = _data;
            }
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            finally
            #endif
            {
                if (count > 0) {
                    ArrayPool<Job>.Shared.Return(_jobs);
                    ArrayPool<uint>.Shared.Return(_jobIndexes);
                    _jobs = null;
                    _jobIndexes = null;
                }
                World<WorldType>.CurrentQuery.QueryDataCount--;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (World<WorldType>.CurrentQuery.QueryDataCount == 0) {
                    World<WorldType>.CurrentQuery.QueryMode = 0;
                }
                #endif
                _runner = null;
                _data = default;
            }
        }

        public override void Run(uint from, uint to) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            
            #if NET6_0_OR_GREATER
            Span<C1> d1 = default;
            Span<C2> d2 = default;
            Span<C3> d3 = default;
            #else
            C1[] d1 = null;
            C2[] d2 = null;
            C3[] d3 = null;
            #endif

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            var jobs = _jobs;
            var jobIndexes = _jobIndexes;

            var dataIdx = uint.MaxValue;
            while (from < to) {
                ref var job = ref jobs[jobIndexes[from++]];
                var count = job.Count;
                job.Count = 0;

                for (uint i = 0; i < count; i++) {
                    var entitiesMask = job.Masks[i];
                    var globalBlockIdx = job.GlobalBlockIdx[i];

                    if (globalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT;
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

                    var blockEntity = globalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT;
                    #if NET6_0_OR_GREATER
                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                    #else
                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                    #endif
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMask & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if DEBUG || EFS_ECS_ENABLE_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                _runner(ref _data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            _runner(ref _data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMask;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }
            #if DEBUG || EFS_ECS_ENABLE_DEBUG
            World<WorldType>.CurrentQuery.SetCurrentEntity(0);
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Prepare(P with, EntityStatusType entities, ComponentStatus components, out uint jobsCount) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;

            var chunksCount = World<WorldType>.Entities.Value.nextActiveChunkIdx;
            var entitiesChunks = World<WorldType>.Entities.Value.chunks;

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif

            jobsCount = 0;
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
                        if (jobsCount == 0) {
                            var size = (int) (chunksCount * Const.DATA_BLOCK_IN_CHUNK_FOR_THREADS);
                            _jobs = ArrayPool<Job>.Shared.Rent(size);
                            _jobIndexes = ArrayPool<uint>.Shared.Rent(size);
                        }

                        var jobIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT_FOR_THREADS;
                        ref var job = ref _jobs[jobIdx];
                        if (job.Count == 0) {
                            _jobIndexes[jobsCount++] = jobIdx;
                        }
                        job.Masks[job.Count] = entitiesMask;
                        job.GlobalBlockIdx[job.Count++] = globalBlockIdx;
                    }
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public sealed unsafe class QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, P> : AbstractParallelTask
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private QueryFunctionWithRefData<D, C1, C2, C3, C4> _runner;
        private Job[] _jobs;
        private uint[] _jobIndexes;
        private D _data;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4> runner, P with, EntityStatusType entities, ComponentStatus components, uint minEntitiesPerThread, uint workersLimit) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (World<WorldType>.MultiThreadActive) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.CurrentQuery.QueryDataCount != 0) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.cfg.ParallelQueryType == ParallelQueryType.Disabled) throw new StaticEcsException("ParallelQueryType = Disabled, change World config");
            #endif
            World<WorldType>.CurrentQuery.QueryDataCount++;

            _runner = runner;
            _data = data;
            
            Prepare(with, entities, components, out var count);

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                ParallelRunner<WorldType>.Run(this, count, Math.Max(minEntitiesPerThread / Const.DATA_BLOCK_SIZE_FOR_THREADS, 1), workersLimit);;
                data = _data;
            }
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            finally
            #endif
            {
                if (count > 0) {
                    ArrayPool<Job>.Shared.Return(_jobs);
                    ArrayPool<uint>.Shared.Return(_jobIndexes);
                    _jobs = null;
                    _jobIndexes = null;
                }
                World<WorldType>.CurrentQuery.QueryDataCount--;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (World<WorldType>.CurrentQuery.QueryDataCount == 0) {
                    World<WorldType>.CurrentQuery.QueryMode = 0;
                }
                #endif
                _runner = null;
                _data = default;
            }
        }

        public override void Run(uint from, uint to) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            
            #if NET6_0_OR_GREATER
            Span<C1> d1 = default;
            Span<C2> d2 = default;
            Span<C3> d3 = default;
            Span<C4> d4 = default;
            #else
            C1[] d1 = null;
            C2[] d2 = null;
            C3[] d3 = null;
            C4[] d4 = null;
            #endif

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            var jobs = _jobs;
            var jobIndexes = _jobIndexes;

            var dataIdx = uint.MaxValue;
            while (from < to) {
                ref var job = ref jobs[jobIndexes[from++]];
                var count = job.Count;
                job.Count = 0;

                for (uint i = 0; i < count; i++) {
                    var entitiesMask = job.Masks[i];
                    var globalBlockIdx = job.GlobalBlockIdx[i];

                    if (globalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT;
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        d3 = new(c3.data[dataIdx]);
                        d4 = new(c4.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
                        d4 = c4.data[dataIdx];
                        #endif
                    }

                    var blockEntity = globalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT;
                    #if NET6_0_OR_GREATER
                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                    #else
                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                    #endif
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMask & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if DEBUG || EFS_ECS_ENABLE_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                _runner(ref _data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            _runner(ref _data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMask;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }
            #if DEBUG || EFS_ECS_ENABLE_DEBUG
            World<WorldType>.CurrentQuery.SetCurrentEntity(0);
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Prepare(P with, EntityStatusType entities, ComponentStatus components, out uint jobsCount) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;

            var chunksCount = World<WorldType>.Entities.Value.nextActiveChunkIdx;
            var entitiesChunks = World<WorldType>.Entities.Value.chunks;

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif

            jobsCount = 0;
            for (uint chunkIdx = 0; chunkIdx < chunksCount; chunkIdx++) {
                ref var chE = ref entitiesChunks[chunkIdx];
                ref var ch1 = ref c1.chunks[chunkIdx];
                ref var ch2 = ref c2.chunks[chunkIdx];
                ref var ch3 = ref c3.chunks[chunkIdx];
                ref var ch4 = ref c4.chunks[chunkIdx];

                var chunkMask = chE.notEmptyBlocks
                                & ch1.notEmptyBlocks
                                & ch2.notEmptyBlocks
                                & ch3.notEmptyBlocks
                                & ch4.notEmptyBlocks;
                with.CheckChunk<WorldType>(ref chunkMask, chunkIdx);

                var aMask = chE.entities;
                var aMask1 = ch1.entities;
                var aMask2 = ch2.entities;
                var aMask3 = ch3.entities;
                var aMask4 = ch4.entities;

                var dMask = chE.disabledEntities;
                var dMask1 = ch1.disabledEntities;
                var dMask2 = ch2.disabledEntities;
                var dMask3 = ch3.disabledEntities;
                var dMask4 = ch4.disabledEntities;

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
                        ComponentStatus.Enabled => aMask1[blockIdx] & ~dMask1[blockIdx] & aMask2[blockIdx] & ~dMask2[blockIdx] & aMask3[blockIdx] & ~dMask3[blockIdx] & aMask4[blockIdx] & ~dMask4[blockIdx],
                        ComponentStatus.Disabled => dMask1[blockIdx] & dMask2[blockIdx] & dMask3[blockIdx] & dMask4[blockIdx],
                        _                        => aMask1[blockIdx] & aMask2[blockIdx] & aMask3[blockIdx] & aMask4[blockIdx],
                    };
                    with.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, (int) blockIdx);

                    if (entitiesMask > 0) {
                        if (jobsCount == 0) {
                            var size = (int) (chunksCount * Const.DATA_BLOCK_IN_CHUNK_FOR_THREADS);
                            _jobs = ArrayPool<Job>.Shared.Rent(size);
                            _jobIndexes = ArrayPool<uint>.Shared.Rent(size);
                        }

                        var jobIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT_FOR_THREADS;
                        ref var job = ref _jobs[jobIdx];
                        if (job.Count == 0) {
                            _jobIndexes[jobsCount++] = jobIdx;
                        }
                        job.Masks[job.Count] = entitiesMask;
                        job.GlobalBlockIdx[job.Count++] = globalBlockIdx;
                    }
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public sealed unsafe class QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, P> : AbstractParallelTask
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> _runner;
        private Job[] _jobs;
        private uint[] _jobIndexes;
        private D _data;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5> runner, P with, EntityStatusType entities, ComponentStatus components, uint minEntitiesPerThread, uint workersLimit) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (World<WorldType>.MultiThreadActive) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.CurrentQuery.QueryDataCount != 0) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.cfg.ParallelQueryType == ParallelQueryType.Disabled) throw new StaticEcsException("ParallelQueryType = Disabled, change World config");
            #endif
            World<WorldType>.CurrentQuery.QueryDataCount++;

            _runner = runner;
            _data = data;
            
            Prepare(with, entities, components, out var count);

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                ParallelRunner<WorldType>.Run(this, count, Math.Max(minEntitiesPerThread / Const.DATA_BLOCK_SIZE_FOR_THREADS, 1), workersLimit);;
                data = _data;
            }
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            finally
            #endif
            {
                if (count > 0) {
                    ArrayPool<Job>.Shared.Return(_jobs);
                    ArrayPool<uint>.Shared.Return(_jobIndexes);
                    _jobs = null;
                    _jobIndexes = null;
                }
                World<WorldType>.CurrentQuery.QueryDataCount--;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (World<WorldType>.CurrentQuery.QueryDataCount == 0) {
                    World<WorldType>.CurrentQuery.QueryMode = 0;
                }
                #endif
                _runner = null;
                _data = default;
            }
        }

        public override void Run(uint from, uint to) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;
            
            #if NET6_0_OR_GREATER
            Span<C1> d1 = default;
            Span<C2> d2 = default;
            Span<C3> d3 = default;
            Span<C4> d4 = default;
            Span<C5> d5 = default;
            #else
            C1[] d1 = null;
            C2[] d2 = null;
            C3[] d3 = null;
            C4[] d4 = null;
            C5[] d5 = null;
            #endif

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            var jobs = _jobs;
            var jobIndexes = _jobIndexes;

            var dataIdx = uint.MaxValue;
            while (from < to) {
                ref var job = ref jobs[jobIndexes[from++]];
                var count = job.Count;
                job.Count = 0;

                for (uint i = 0; i < count; i++) {
                    var entitiesMask = job.Masks[i];
                    var globalBlockIdx = job.GlobalBlockIdx[i];

                    if (globalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT;
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        d3 = new(c3.data[dataIdx]);
                        d4 = new(c4.data[dataIdx]);
                        d5 = new(c5.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
                        d4 = c4.data[dataIdx];
                        d5 = c5.data[dataIdx];
                        #endif
                    }

                    var blockEntity = globalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT;
                    #if NET6_0_OR_GREATER
                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                    #else
                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                    #endif
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMask & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if DEBUG || EFS_ECS_ENABLE_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                _runner(ref _data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            _runner(ref _data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMask;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }
            #if DEBUG || EFS_ECS_ENABLE_DEBUG
            World<WorldType>.CurrentQuery.SetCurrentEntity(0);
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Prepare(P with, EntityStatusType entities, ComponentStatus components, out uint jobsCount) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;

            var chunksCount = World<WorldType>.Entities.Value.nextActiveChunkIdx;
            var entitiesChunks = World<WorldType>.Entities.Value.chunks;

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif

            jobsCount = 0;
            for (uint chunkIdx = 0; chunkIdx < chunksCount; chunkIdx++) {
                ref var chE = ref entitiesChunks[chunkIdx];
                ref var ch1 = ref c1.chunks[chunkIdx];
                ref var ch2 = ref c2.chunks[chunkIdx];
                ref var ch3 = ref c3.chunks[chunkIdx];
                ref var ch4 = ref c4.chunks[chunkIdx];
                ref var ch5 = ref c5.chunks[chunkIdx];

                var chunkMask = chE.notEmptyBlocks
                                & ch1.notEmptyBlocks
                                & ch2.notEmptyBlocks
                                & ch3.notEmptyBlocks
                                & ch4.notEmptyBlocks
                                & ch5.notEmptyBlocks;
                with.CheckChunk<WorldType>(ref chunkMask, chunkIdx);

                var aMask = chE.entities;
                var aMask1 = ch1.entities;
                var aMask2 = ch2.entities;
                var aMask3 = ch3.entities;
                var aMask4 = ch4.entities;
                var aMask5 = ch5.entities;

                var dMask = chE.disabledEntities;
                var dMask1 = ch1.disabledEntities;
                var dMask2 = ch2.disabledEntities;
                var dMask3 = ch3.disabledEntities;
                var dMask4 = ch4.disabledEntities;
                var dMask5 = ch5.disabledEntities;

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
                        ComponentStatus.Enabled => aMask1[blockIdx] & ~dMask1[blockIdx] & aMask2[blockIdx] & ~dMask2[blockIdx] & aMask3[blockIdx] & ~dMask3[blockIdx] & aMask4[blockIdx] & ~dMask4[blockIdx] & aMask5[blockIdx] &
                                                   ~dMask5[blockIdx],
                        ComponentStatus.Disabled => dMask1[blockIdx] & dMask2[blockIdx] & dMask3[blockIdx] & dMask4[blockIdx] & dMask5[blockIdx],
                        _                        => aMask1[blockIdx] & aMask2[blockIdx] & aMask3[blockIdx] & aMask4[blockIdx] & aMask5[blockIdx],
                    };
                    with.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, (int) blockIdx);

                    if (entitiesMask > 0) {
                        if (jobsCount == 0) {
                            var size = (int) (chunksCount * Const.DATA_BLOCK_IN_CHUNK_FOR_THREADS);
                            _jobs = ArrayPool<Job>.Shared.Rent(size);
                            _jobIndexes = ArrayPool<uint>.Shared.Rent(size);
                        }

                        var jobIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT_FOR_THREADS;
                        ref var job = ref _jobs[jobIdx];
                        if (job.Count == 0) {
                            _jobIndexes[jobsCount++] = jobIdx;
                        }
                        job.Masks[job.Count] = entitiesMask;
                        job.GlobalBlockIdx[job.Count++] = globalBlockIdx;
                    }
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public sealed unsafe class QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, P> : AbstractParallelTask
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> _runner;
        private Job[] _jobs;
        private uint[] _jobIndexes;
        private D _data;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6> runner, P with, EntityStatusType entities, ComponentStatus components, uint minEntitiesPerThread, uint workersLimit) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (World<WorldType>.MultiThreadActive) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.CurrentQuery.QueryDataCount != 0) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.cfg.ParallelQueryType == ParallelQueryType.Disabled) throw new StaticEcsException("ParallelQueryType = Disabled, change World config");
            #endif
            World<WorldType>.CurrentQuery.QueryDataCount++;

            _runner = runner;
            _data = data;
            
            Prepare(with, entities, components, out var count);

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                ParallelRunner<WorldType>.Run(this, count, Math.Max(minEntitiesPerThread / Const.DATA_BLOCK_SIZE_FOR_THREADS, 1), workersLimit);;
                data = _data;
            }
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            finally
            #endif
            {
                if (count > 0) {
                    ArrayPool<Job>.Shared.Return(_jobs);
                    ArrayPool<uint>.Shared.Return(_jobIndexes);
                    _jobs = null;
                    _jobIndexes = null;
                }
                World<WorldType>.CurrentQuery.QueryDataCount--;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (World<WorldType>.CurrentQuery.QueryDataCount == 0) {
                    World<WorldType>.CurrentQuery.QueryMode = 0;
                }
                #endif
                _runner = null;
                _data = default;
            }
        }

        public override void Run(uint from, uint to) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;
            ref var c6 = ref World<WorldType>.Components<C6>.Value;
            
            #if NET6_0_OR_GREATER
            Span<C1> d1 = default;
            Span<C2> d2 = default;
            Span<C3> d3 = default;
            Span<C4> d4 = default;
            Span<C5> d5 = default;
            Span<C6> d6 = default;
            #else
            C1[] d1 = null;
            C2[] d2 = null;
            C3[] d3 = null;
            C4[] d4 = null;
            C5[] d5 = null;
            C6[] d6 = null;
            #endif

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            var jobs = _jobs;
            var jobIndexes = _jobIndexes;

            var dataIdx = uint.MaxValue;
            while (from < to) {
                ref var job = ref jobs[jobIndexes[from++]];
                var count = job.Count;
                job.Count = 0;

                for (uint i = 0; i < count; i++) {
                    var entitiesMask = job.Masks[i];
                    var globalBlockIdx = job.GlobalBlockIdx[i];

                    if (globalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT;
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        d3 = new(c3.data[dataIdx]);
                        d4 = new(c4.data[dataIdx]);
                        d5 = new(c5.data[dataIdx]);
                        d6 = new(c6.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
                        d4 = c4.data[dataIdx];
                        d5 = c5.data[dataIdx];
                        d6 = c6.data[dataIdx];
                        #endif
                    }

                    var blockEntity = globalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT;
                    #if NET6_0_OR_GREATER
                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                    #else
                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                    #endif
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMask & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if DEBUG || EFS_ECS_ENABLE_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                _runner(ref _data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            _runner(ref _data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMask;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }
            #if DEBUG || EFS_ECS_ENABLE_DEBUG
            World<WorldType>.CurrentQuery.SetCurrentEntity(0);
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Prepare(P with, EntityStatusType entities, ComponentStatus components, out uint jobsCount) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;
            ref var c6 = ref World<WorldType>.Components<C6>.Value;

            var chunksCount = World<WorldType>.Entities.Value.nextActiveChunkIdx;
            var entitiesChunks = World<WorldType>.Entities.Value.chunks;

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif

            jobsCount = 0;
            for (uint chunkIdx = 0; chunkIdx < chunksCount; chunkIdx++) {
                ref var chE = ref entitiesChunks[chunkIdx];
                ref var ch1 = ref c1.chunks[chunkIdx];
                ref var ch2 = ref c2.chunks[chunkIdx];
                ref var ch3 = ref c3.chunks[chunkIdx];
                ref var ch4 = ref c4.chunks[chunkIdx];
                ref var ch5 = ref c5.chunks[chunkIdx];
                ref var ch6 = ref c6.chunks[chunkIdx];

                var chunkMask = chE.notEmptyBlocks
                                & ch1.notEmptyBlocks
                                & ch2.notEmptyBlocks
                                & ch3.notEmptyBlocks
                                & ch4.notEmptyBlocks
                                & ch5.notEmptyBlocks
                                & ch6.notEmptyBlocks;
                with.CheckChunk<WorldType>(ref chunkMask, chunkIdx);

                var aMask = chE.entities;
                var aMask1 = ch1.entities;
                var aMask2 = ch2.entities;
                var aMask3 = ch3.entities;
                var aMask4 = ch4.entities;
                var aMask5 = ch5.entities;
                var aMask6 = ch6.entities;

                var dMask = chE.disabledEntities;
                var dMask1 = ch1.disabledEntities;
                var dMask2 = ch2.disabledEntities;
                var dMask3 = ch3.disabledEntities;
                var dMask4 = ch4.disabledEntities;
                var dMask5 = ch5.disabledEntities;
                var dMask6 = ch6.disabledEntities;

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
                        ComponentStatus.Enabled => aMask1[blockIdx] & ~dMask1[blockIdx] & aMask2[blockIdx] & ~dMask2[blockIdx] & aMask3[blockIdx] & ~dMask3[blockIdx] & aMask4[blockIdx] & ~dMask4[blockIdx] & aMask5[blockIdx] &
                                                   ~dMask5[blockIdx] & aMask6[blockIdx] & ~dMask6[blockIdx],
                        ComponentStatus.Disabled => dMask1[blockIdx] & dMask2[blockIdx] & dMask3[blockIdx] & dMask4[blockIdx] & dMask5[blockIdx] & dMask6[blockIdx],
                        _                        => aMask1[blockIdx] & aMask2[blockIdx] & aMask3[blockIdx] & aMask4[blockIdx] & aMask5[blockIdx] & aMask6[blockIdx],
                    };
                    with.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, (int) blockIdx);

                    if (entitiesMask > 0) {
                        if (jobsCount == 0) {
                            var size = (int) (chunksCount * Const.DATA_BLOCK_IN_CHUNK_FOR_THREADS);
                            _jobs = ArrayPool<Job>.Shared.Rent(size);
                            _jobIndexes = ArrayPool<uint>.Shared.Rent(size);
                        }

                        var jobIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT_FOR_THREADS;
                        ref var job = ref _jobs[jobIdx];
                        if (job.Count == 0) {
                            _jobIndexes[jobsCount++] = jobIdx;
                        }
                        job.Masks[job.Count] = entitiesMask;
                        job.GlobalBlockIdx[job.Count++] = globalBlockIdx;
                    }
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public sealed unsafe class QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, P> : AbstractParallelTask
        where P : struct, IQueryMethod
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where WorldType : struct, IWorldType {
        
        private QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7> _runner;
        private Job[] _jobs;
        private uint[] _jobIndexes;
        private D _data;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7> runner, P with, EntityStatusType entities, ComponentStatus components, uint minEntitiesPerThread, uint workersLimit) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (World<WorldType>.MultiThreadActive) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.CurrentQuery.QueryDataCount != 0) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.cfg.ParallelQueryType == ParallelQueryType.Disabled) throw new StaticEcsException("ParallelQueryType = Disabled, change World config");
            #endif
            World<WorldType>.CurrentQuery.QueryDataCount++;

            _runner = runner;
            _data = data;
            
            Prepare(with, entities, components, out var count);

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                ParallelRunner<WorldType>.Run(this, count, Math.Max(minEntitiesPerThread / Const.DATA_BLOCK_SIZE_FOR_THREADS, 1), workersLimit);;
                data = _data;
            }
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            finally
            #endif
            {
                if (count > 0) {
                    ArrayPool<Job>.Shared.Return(_jobs);
                    ArrayPool<uint>.Shared.Return(_jobIndexes);
                    _jobs = null;
                    _jobIndexes = null;
                }
                World<WorldType>.CurrentQuery.QueryDataCount--;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (World<WorldType>.CurrentQuery.QueryDataCount == 0) {
                    World<WorldType>.CurrentQuery.QueryMode = 0;
                }
                #endif
                _runner = null;
                _data = default;
            }
        }

        public override void Run(uint from, uint to) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;
            ref var c6 = ref World<WorldType>.Components<C6>.Value;
            ref var c7 = ref World<WorldType>.Components<C7>.Value;
            
            #if NET6_0_OR_GREATER
            Span<C1> d1 = default;
            Span<C2> d2 = default;
            Span<C3> d3 = default;
            Span<C4> d4 = default;
            Span<C5> d5 = default;
            Span<C6> d6 = default;
            Span<C7> d7 = default;
            #else
            C1[] d1 = null;
            C2[] d2 = null;
            C3[] d3 = null;
            C4[] d4 = null;
            C5[] d5 = null;
            C6[] d6 = null;
            C7[] d7 = null;
            #endif

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            var jobs = _jobs;
            var jobIndexes = _jobIndexes;

            var dataIdx = uint.MaxValue;
            while (from < to) {
                ref var job = ref jobs[jobIndexes[from++]];
                var count = job.Count;
                job.Count = 0;

                for (uint i = 0; i < count; i++) {
                    var entitiesMask = job.Masks[i];
                    var globalBlockIdx = job.GlobalBlockIdx[i];

                    if (globalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT;
                        #if NET6_0_OR_GREATER
                        d1 = new(c1.data[dataIdx]);
                        d2 = new(c2.data[dataIdx]);
                        d3 = new(c3.data[dataIdx]);
                        d4 = new(c4.data[dataIdx]);
                        d5 = new(c5.data[dataIdx]);
                        d6 = new(c6.data[dataIdx]);
                        d7 = new(c7.data[dataIdx]);
                        #else
                        d1 = c1.data[dataIdx];
                        d2 = c2.data[dataIdx];
                        d3 = c3.data[dataIdx];
                        d4 = c4.data[dataIdx];
                        d5 = c5.data[dataIdx];
                        d6 = c6.data[dataIdx];
                        d7 = c7.data[dataIdx];
                        #endif
                    }

                    var blockEntity = globalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT;
                    #if NET6_0_OR_GREATER
                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                    #else
                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                    #endif
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMask & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if DEBUG || EFS_ECS_ENABLE_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                _runner(ref _data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            _runner(ref _data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMask;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }
            #if DEBUG || EFS_ECS_ENABLE_DEBUG
            World<WorldType>.CurrentQuery.SetCurrentEntity(0);
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Prepare(P with, EntityStatusType entities, ComponentStatus components, out uint jobsCount) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;
            ref var c6 = ref World<WorldType>.Components<C6>.Value;
            ref var c7 = ref World<WorldType>.Components<C7>.Value;

            var chunksCount = World<WorldType>.Entities.Value.nextActiveChunkIdx;
            var entitiesChunks = World<WorldType>.Entities.Value.chunks;

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif

            jobsCount = 0;
            for (uint chunkIdx = 0; chunkIdx < chunksCount; chunkIdx++) {
                ref var chE = ref entitiesChunks[chunkIdx];
                ref var ch1 = ref c1.chunks[chunkIdx];
                ref var ch2 = ref c2.chunks[chunkIdx];
                ref var ch3 = ref c3.chunks[chunkIdx];
                ref var ch4 = ref c4.chunks[chunkIdx];
                ref var ch5 = ref c5.chunks[chunkIdx];
                ref var ch6 = ref c6.chunks[chunkIdx];
                ref var ch7 = ref c7.chunks[chunkIdx];

                var chunkMask = chE.notEmptyBlocks
                                & ch1.notEmptyBlocks
                                & ch2.notEmptyBlocks
                                & ch3.notEmptyBlocks
                                & ch4.notEmptyBlocks
                                & ch5.notEmptyBlocks
                                & ch6.notEmptyBlocks
                                & ch7.notEmptyBlocks;
                with.CheckChunk<WorldType>(ref chunkMask, chunkIdx);

                var aMask = chE.entities;
                var aMask1 = ch1.entities;
                var aMask2 = ch2.entities;
                var aMask3 = ch3.entities;
                var aMask4 = ch4.entities;
                var aMask5 = ch5.entities;
                var aMask6 = ch6.entities;
                var aMask7 = ch7.entities;

                var dMask = chE.disabledEntities;
                var dMask1 = ch1.disabledEntities;
                var dMask2 = ch2.disabledEntities;
                var dMask3 = ch3.disabledEntities;
                var dMask4 = ch4.disabledEntities;
                var dMask5 = ch5.disabledEntities;
                var dMask6 = ch6.disabledEntities;
                var dMask7 = ch7.disabledEntities;

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
                        ComponentStatus.Enabled => aMask1[blockIdx] & ~dMask1[blockIdx] & aMask2[blockIdx] & ~dMask2[blockIdx] & aMask3[blockIdx] & ~dMask3[blockIdx] & aMask4[blockIdx] & ~dMask4[blockIdx] & aMask5[blockIdx] &
                                                   ~dMask5[blockIdx] & aMask6[blockIdx] & ~dMask6[blockIdx] & aMask7[blockIdx] & ~dMask7[blockIdx],
                        ComponentStatus.Disabled => dMask1[blockIdx] & dMask2[blockIdx] & dMask3[blockIdx] & dMask4[blockIdx] & dMask5[blockIdx] & dMask6[blockIdx] & dMask7[blockIdx],
                        _                        => aMask1[blockIdx] & aMask2[blockIdx] & aMask3[blockIdx] & aMask4[blockIdx] & aMask5[blockIdx] & aMask6[blockIdx] & aMask7[blockIdx],
                    };
                    with.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, (int) blockIdx);

                    if (entitiesMask > 0) {
                        if (jobsCount == 0) {
                            var size = (int) (chunksCount * Const.DATA_BLOCK_IN_CHUNK_FOR_THREADS);
                            _jobs = ArrayPool<Job>.Shared.Rent(size);
                            _jobIndexes = ArrayPool<uint>.Shared.Rent(size);
                        }

                        var jobIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT_FOR_THREADS;
                        ref var job = ref _jobs[jobIdx];
                        if (job.Count == 0) {
                            _jobIndexes[jobsCount++] = jobIdx;
                        }
                        job.Masks[job.Count] = entitiesMask;
                        job.GlobalBlockIdx[job.Count++] = globalBlockIdx;
                    }
                }
            }
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public sealed unsafe class QueryFunctionRunnerWithRefDataParallel<D, WorldType, C1, C2, C3, C4, C5, C6, C7, C8, P> : AbstractParallelTask
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
        
        private QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8> _runner;
        private Job[] _jobs;
        private uint[] _jobIndexes;
        private D _data;

        [MethodImpl(AggressiveInlining)]
        public void Run(ref D data, QueryFunctionWithRefData<D, C1, C2, C3, C4, C5, C6, C7, C8> runner, P with, EntityStatusType entities, ComponentStatus components, uint minEntitiesPerThread, uint workersLimit) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (World<WorldType>.MultiThreadActive) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.CurrentQuery.QueryDataCount != 0) throw new StaticEcsException("Nested query are not available with parallel query");
            if (World<WorldType>.cfg.ParallelQueryType == ParallelQueryType.Disabled) throw new StaticEcsException("ParallelQueryType = Disabled, change World config");
            #endif
            World<WorldType>.CurrentQuery.QueryDataCount++;

            _runner = runner;
            _data = data;
            
            Prepare(with, entities, components, out var count);

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            try
            #endif
            {
                ParallelRunner<WorldType>.Run(this, count, Math.Max(minEntitiesPerThread / Const.DATA_BLOCK_SIZE_FOR_THREADS, 1), workersLimit);;
                data = _data;
            }
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            finally
            #endif
            {
                if (count > 0) {
                    ArrayPool<Job>.Shared.Return(_jobs);
                    ArrayPool<uint>.Shared.Return(_jobIndexes);
                    _jobs = null;
                    _jobIndexes = null;
                }
                World<WorldType>.CurrentQuery.QueryDataCount--;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (World<WorldType>.CurrentQuery.QueryDataCount == 0) {
                    World<WorldType>.CurrentQuery.QueryMode = 0;
                }
                #endif
                _runner = null;
                _data = default;
            }
        }

        public override void Run(uint from, uint to) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;
            ref var c6 = ref World<WorldType>.Components<C6>.Value;
            ref var c7 = ref World<WorldType>.Components<C7>.Value;
            ref var c8 = ref World<WorldType>.Components<C8>.Value;

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

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            var jobs = _jobs;
            var jobIndexes = _jobIndexes;

            var dataIdx = uint.MaxValue;
            while (from < to) {
                ref var job = ref jobs[jobIndexes[from++]];
                var count = job.Count;
                job.Count = 0;

                for (uint i = 0; i < count; i++) {
                    var entitiesMask = job.Masks[i];
                    var globalBlockIdx = job.GlobalBlockIdx[i];

                    if (globalBlockIdx >> Const.DATA_QUERY_SHIFT != dataIdx) {
                        dataIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT;
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

                    var blockEntity = globalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT;
                    #if NET6_0_OR_GREATER
                    var dOffset = (int) (blockEntity & Const.DATA_ENTITY_MASK);
                    #else
                    var dOffset = blockEntity & Const.DATA_ENTITY_MASK;
                    #endif
                    var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                    var end = Utils.ApproximateMSB(entitiesMask);
                    var total = Utils.PopCnt(entitiesMask);
                    if (total >= (end - idx) >> 1) {
                        for (; idx < end; idx++) {
                            if ((entitiesMask & (1UL << idx)) > 0) {
                                var dIdx = idx + dOffset;
                                #if DEBUG || EFS_ECS_ENABLE_DEBUG
                                World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                                #endif
                                _runner(ref _data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx], ref d8[dIdx]);
                            }
                        }
                    } else {
                        do {
                            var dIdx = idx + dOffset;
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(blockEntity + idx);
                            #endif
                            _runner(ref _data, ref d1[dIdx], ref d2[dIdx], ref d3[dIdx], ref d4[dIdx], ref d5[dIdx], ref d6[dIdx], ref d7[dIdx], ref d8[dIdx]);
                            entitiesMask &= (entitiesMask - 1UL) & entitiesMask;
                            idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        } while (entitiesMask > 0);
                    }
                }
            }
            #if DEBUG || EFS_ECS_ENABLE_DEBUG
            World<WorldType>.CurrentQuery.SetCurrentEntity(0);
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Prepare(P with, EntityStatusType entities, ComponentStatus components, out uint jobsCount) {
            ref var c1 = ref World<WorldType>.Components<C1>.Value;
            ref var c2 = ref World<WorldType>.Components<C2>.Value;
            ref var c3 = ref World<WorldType>.Components<C3>.Value;
            ref var c4 = ref World<WorldType>.Components<C4>.Value;
            ref var c5 = ref World<WorldType>.Components<C5>.Value;
            ref var c6 = ref World<WorldType>.Components<C6>.Value;
            ref var c7 = ref World<WorldType>.Components<C7>.Value;
            ref var c8 = ref World<WorldType>.Components<C8>.Value;

            var chunksCount = World<WorldType>.Entities.Value.nextActiveChunkIdx;
            var entitiesChunks = World<WorldType>.Entities.Value.chunks;

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif

            jobsCount = 0;
            for (uint chunkIdx = 0; chunkIdx < chunksCount; chunkIdx++) {
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
                        EntityStatusType.Enabled  => aMask[blockIdx] & ~dMask[blockIdx],
                        EntityStatusType.Disabled => dMask[blockIdx],
                        _                         => aMask[blockIdx]
                    };
                    entitiesMask &= components switch {
                        ComponentStatus.Enabled => aMask1[blockIdx] & ~dMask1[blockIdx] & aMask2[blockIdx] & ~dMask2[blockIdx] & aMask3[blockIdx] & ~dMask3[blockIdx] & aMask4[blockIdx] & ~dMask4[blockIdx] & aMask5[blockIdx] &
                                                   ~dMask5[blockIdx] & aMask6[blockIdx] & ~dMask6[blockIdx] & aMask7[blockIdx] & ~dMask7[blockIdx] & aMask8[blockIdx] & ~dMask8[blockIdx],
                        ComponentStatus.Disabled => dMask1[blockIdx] & dMask2[blockIdx] & dMask3[blockIdx] & dMask4[blockIdx] & dMask5[blockIdx] & dMask6[blockIdx] & dMask7[blockIdx] & dMask8[blockIdx],
                        _                        => aMask1[blockIdx] & aMask2[blockIdx] & aMask3[blockIdx] & aMask4[blockIdx] & aMask5[blockIdx] & aMask6[blockIdx] & aMask7[blockIdx] & aMask8[blockIdx],
                    };
                    with.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, (int) blockIdx);

                    if (entitiesMask > 0) {
                        if (jobsCount == 0) {
                            var size = (int) (chunksCount * Const.DATA_BLOCK_IN_CHUNK_FOR_THREADS);
                            _jobs = ArrayPool<Job>.Shared.Rent(size);
                            _jobIndexes = ArrayPool<uint>.Shared.Rent(size);
                        }

                        var jobIdx = globalBlockIdx >> Const.DATA_QUERY_SHIFT_FOR_THREADS;
                        ref var job = ref _jobs[jobIdx];
                        if (job.Count == 0) {
                            _jobIndexes[jobsCount++] = jobIdx;
                        }
                        job.Masks[job.Count] = entitiesMask;
                        job.GlobalBlockIdx[job.Count++] = globalBlockIdx;
                    }
                }
            }
        }
    }
    #endregion
}