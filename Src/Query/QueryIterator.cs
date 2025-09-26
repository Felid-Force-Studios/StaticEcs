using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct BlockMaskCache {
        public ulong EntitiesMask;
        public int NextGlobalBlock;
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption (Option.NullChecks, false)]
    [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
    #endif
    public ref struct QueryEntitiesIterator<WorldType, QM> where QM : struct, IQueryMethod where WorldType : struct, IWorldType {
        private QueryData qd;                    //8
        private World<WorldType>.Entity current; //4
        private int idx;                         //4
        private int end;                         //4
        private int firstGlobalBlockIdx;         //4
        private QM _with;                        //???
        private bool loopMode;                   //1
        private bool strict;                     //1
        private EntityStatusType _entities;
        #if DEBUG || EFS_ECS_ENABLE_DEBUG
        private bool disposed; //1
        #endif

        [MethodImpl(AggressiveInlining)]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public QueryEntitiesIterator(QM with, EntityStatusType entities, QueryMode queryMode) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (World<WorldType>.MultiThreadActive) throw new StaticEcsException("Nested query are not available with parallel query");
            #endif

            _entities = entities;
            _with = with;
            qd = default;
            firstGlobalBlockIdx = -1;
            loopMode = false;
            #if DEBUG || EFS_ECS_ENABLE_DEBUG
            disposed = false;
            #endif
            strict = queryMode == QueryMode.Strict || (queryMode == QueryMode.Default && World<WorldType>.cfg.DefaultQueryModeStrict);
            idx = 0;
            end = 0;
            current = default;
            
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
            for (uint chunkIdx = 0; chunkIdx < chunksCount; chunkIdx++) {
                ref var chE = ref entitiesChunks[chunkIdx];
                var chunkMask = chE.notEmptyBlocks;
                _with.CheckChunk<WorldType>(ref chunkMask, chunkIdx);

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
                    _with.CheckEntities<WorldType>(ref entitiesMask, chunkIdx, (int) blockIdx);

                    if (entitiesMask > 0) {
                        if (previousGlobalBlockIdx >= 0) {
                            filteredBlocks[previousGlobalBlockIdx].NextGlobalBlock = (int) globalBlockIdx;
                        } else {
                            qd = World<WorldType>.CurrentQuery.RegisterQuery();

                            if (!strict) {
                                _with.IncQ<WorldType>(qd);
                                switch (_entities) {
                                    case EntityStatusType.Enabled:  World<WorldType>.Entities.Value.IncQDisable(qd); break;
                                    case EntityStatusType.Disabled: World<WorldType>.Entities.Value.IncQEnable(qd); break;
                                }
                                World<WorldType>.Entities.Value.IncQDestroy(qd);
                            }
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            else {
                                const int b = 1;
                                _with.BlockQ<WorldType>(b);
                                switch (_entities) {
                                    case EntityStatusType.Enabled:  World<WorldType>.Entities.Value.BlockDisable(b); break;
                                    case EntityStatusType.Disabled: World<WorldType>.Entities.Value.BlockEnable(b); break;
                                }
                                World<WorldType>.Entities.Value.BlockDestroy(b);
                            }
                            #endif

                            filteredBlocks = qd.Blocks;
                            firstGlobalBlockIdx = (int) globalBlockIdx;
                            loopMode = entitiesMask.CheckBitDensity(out idx, out end);
                        }

                        filteredBlocks[globalBlockIdx].EntitiesMask = entitiesMask;
                        filteredBlocks[globalBlockIdx].NextGlobalBlock = -1;
                        previousGlobalBlockIdx = (int) globalBlockIdx;
                    }
                }
            }
        }

        public readonly World<WorldType>.Entity Current {
            [MethodImpl(AggressiveInlining)] get => current;
        }

        [MethodImpl(AggressiveInlining)]
        public bool MoveNext() {
            while (firstGlobalBlockIdx >= 0) {
                ref var entitiesMask = ref qd.Blocks[firstGlobalBlockIdx].EntitiesMask;
        
                if (loopMode) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            current._id = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            #if DEBUG || EFS_ECS_ENABLE_DEBUG
                            World<WorldType>.CurrentQuery.SetCurrentEntity(current._id);
                            #endif
                            return true;
                        }
                    }
                } else if (entitiesMask > 0) {
                    idx = Utils.PopLsb(ref entitiesMask);
                    current._id = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx);
                    #if DEBUG || EFS_ECS_ENABLE_DEBUG
                    World<WorldType>.CurrentQuery.SetCurrentEntity(current._id);
                    #endif
                    return true;
                }
                
                firstGlobalBlockIdx = qd.Blocks[firstGlobalBlockIdx].NextGlobalBlock;
                if (firstGlobalBlockIdx >= 0) {
                    loopMode = qd.Blocks[firstGlobalBlockIdx].EntitiesMask.CheckBitDensity(out idx, out end);
                }
            }
        
            return false;
        }

        [MethodImpl(AggressiveInlining)]
        public readonly QueryEntitiesIterator<WorldType, QM> GetEnumerator() => this;

        [MethodImpl(AggressiveInlining)]
        public void Dispose() {
            DisposePart1();
            DisposePart2();
        }

        [MethodImpl(AggressiveInlining)]
        private void DisposePart1() {
            if (qd.Blocks != null) {
                if (!strict) {
                    _with.DecQ<WorldType>();
                    switch (_entities) {
                        case EntityStatusType.Enabled:  World<WorldType>.Entities.Value.DecQDisable(); break;
                        case EntityStatusType.Disabled: World<WorldType>.Entities.Value.DecQEnable(); break;
                    }
                    World<WorldType>.Entities.Value.DecQDestroy();
                }
                #if DEBUG || EFS_ECS_ENABLE_DEBUG
                else {
                    const int b = -1;
                    _with.BlockQ<WorldType>(b);
                    switch (_entities) {
                        case EntityStatusType.Enabled:  World<WorldType>.Entities.Value.BlockDisable(b); break;
                        case EntityStatusType.Disabled: World<WorldType>.Entities.Value.BlockEnable(b); break;
                    }
                    World<WorldType>.Entities.Value.BlockDestroy(b);
                }
                World<WorldType>.CurrentQuery.SetCurrentEntity(0);
                #endif
            }
        }

        [MethodImpl(AggressiveInlining)]
        private void DisposePart2() {
            if (qd.Blocks != null) {
                World<WorldType>.CurrentQuery.UnregisterQuery(qd);
                qd = default;
            }
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            disposed = true;
            if (World<WorldType>.CurrentQuery.QueryDataCount == 0) {
                World<WorldType>.CurrentQuery.QueryMode = 0;
            }
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public void DestroyAllEntities() {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            new World<WorldType>.Entity((uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1)).Destroy();
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        new World<WorldType>.Entity((uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask))).Destroy();
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }

            DisposePart2();
        }

        [MethodImpl(AggressiveInlining)]
        public bool First(out World<WorldType>.Entity entity) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            var moveNext = MoveNext();
            entity = Current;
            Dispose();
            return moveNext;
        }

        [MethodImpl(AggressiveInlining)]
        public bool FirstStrict(out World<WorldType>.Entity entity) {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            var moveNext = MoveNext();
            entity = Current;
            if (MoveNext()) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> found more than one entity");
            Dispose();
            return moveNext;
        }
        
        [MethodImpl(AggressiveInlining)]
        public int EntitiesCount() {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            var count = 0;
            
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                count += cache.EntitiesMask.PopCnt();
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            Dispose();
            return count;
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void WriteEntitySnapshotData(ref BinaryPackWriter writer, CustomSnapshotEntityDataWriter<WorldType> snapshotDataEntityWriter) {
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            snapshotDataEntityWriter(ref writer, entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        snapshotDataEntityWriter(ref writer, entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }

            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void ReadEntitySnapshotData(ref BinaryPackReader reader, CustomSnapshotEntityDataReader<WorldType> snapshotDataEntityReader, ushort version) {
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            snapshotDataEntityReader(ref reader, entity, version);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        snapshotDataEntityReader(ref reader, entity, version);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }

            DisposePart2();
        }

        #region COMPONENTS
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1>() where T1 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var components = ref World<WorldType>.Components<T1>.Value;
            
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            components.Add(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        components.Add(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent{
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Add(entity);
                            container2.Add(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Add(entity);
                        container2.Add(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Add(entity);
                            container2.Add(entity);
                            container3.Add(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Add(entity);
                        container2.Add(entity);
                        container3.Add(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3, T4>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Add(entity);
                            container2.Add(entity);
                            container3.Add(entity);
                            container4.Add(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Add(entity);
                        container2.Add(entity);
                        container3.Add(entity);
                        container4.Add(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            ref var container5 = ref World<WorldType>.Components<T5>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Add(entity);
                            container2.Add(entity);
                            container3.Add(entity);
                            container4.Add(entity);
                            container5.Add(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Add(entity);
                        container2.Add(entity);
                        container3.Add(entity);
                        container4.Add(entity);
                        container5.Add(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1>() where T1 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var components = ref World<WorldType>.Components<T1>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            components.TryAdd(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        components.TryAdd(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent{
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.TryAdd(entity);
                            container2.TryAdd(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.TryAdd(entity);
                        container2.TryAdd(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2, T3>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.TryAdd(entity);
                            container2.TryAdd(entity);
                            container3.TryAdd(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.TryAdd(entity);
                        container2.TryAdd(entity);
                        container3.TryAdd(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2, T3, T4>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.TryAdd(entity);
                            container2.TryAdd(entity);
                            container3.TryAdd(entity);
                            container4.TryAdd(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.TryAdd(entity);
                        container2.TryAdd(entity);
                        container3.TryAdd(entity);
                        container4.TryAdd(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            ref var container5 = ref World<WorldType>.Components<T5>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.TryAdd(entity);
                            container2.TryAdd(entity);
                            container3.TryAdd(entity);
                            container4.TryAdd(entity);
                            container5.TryAdd(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.TryAdd(entity);
                        container2.TryAdd(entity);
                        container3.TryAdd(entity);
                        container4.TryAdd(entity);
                        container5.TryAdd(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1>(T1 t1) 
            where T1 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Add(entity, t1);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Add(entity, t1);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2>(T1 t1, T2 t2) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Add(entity, t1);
                            container2.Add(entity, t2);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Add(entity, t1);
                        container2.Add(entity, t2);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3>(T1 t1, T2 t2, T3 t3) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Add(entity, t1);
                            container2.Add(entity, t2);
                            container3.Add(entity, t3);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Add(entity, t1);
                        container2.Add(entity, t2);
                        container3.Add(entity, t3);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Add(entity, t1);
                            container2.Add(entity, t2);
                            container3.Add(entity, t3);
                            container4.Add(entity, t4);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Add(entity, t1);
                        container2.Add(entity, t2);
                        container3.Add(entity, t3);
                        container4.Add(entity, t4);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void AddForAll<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            ref var container5 = ref World<WorldType>.Components<T5>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Add(entity, t1);
                            container2.Add(entity, t2);
                            container3.Add(entity, t3);
                            container4.Add(entity, t4);
                            container5.Add(entity, t5);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Add(entity, t1);
                        container2.Add(entity, t2);
                        container3.Add(entity, t3);
                        container4.Add(entity, t4);
                        container5.Add(entity, t5);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1>(T1 t1) 
            where T1 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.TryAdd(entity) = t1;
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.TryAdd(entity) = t1;
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2>(T1 t1, T2 t2) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.TryAdd(entity) = t1;
                            container2.TryAdd(entity) = t2;
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.TryAdd(entity) = t1;
                        container2.TryAdd(entity) = t2;
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2, T3>(T1 t1, T2 t2, T3 t3) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.TryAdd(entity) = t1;
                            container2.TryAdd(entity) = t2;
                            container3.TryAdd(entity) = t3;
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.TryAdd(entity) = t1;
                        container2.TryAdd(entity) = t2;
                        container3.TryAdd(entity) = t3;
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.TryAdd(entity) = t1;
                            container2.TryAdd(entity) = t2;
                            container3.TryAdd(entity) = t3;
                            container4.TryAdd(entity) = t4;
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.TryAdd(entity) = t1;
                        container2.TryAdd(entity) = t2;
                        container3.TryAdd(entity) = t3;
                        container4.TryAdd(entity) = t4;
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryAddForAll<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            ref var container5 = ref World<WorldType>.Components<T5>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.TryAdd(entity) = t1;
                            container2.TryAdd(entity) = t2;
                            container3.TryAdd(entity) = t3;
                            container4.TryAdd(entity) = t4;
                            container5.TryAdd(entity) = t5;
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.TryAdd(entity) = t1;
                        container2.TryAdd(entity) = t2;
                        container3.TryAdd(entity) = t3;
                        container4.TryAdd(entity) = t4;
                        container5.TryAdd(entity) = t5;
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void PutForAll<T1>(T1 t1) 
            where T1 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Put(entity, t1);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Put(entity, t1);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void PutForAll<T1, T2>(T1 t1, T2 t2) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Put(entity, t1);
                            container2.Put(entity, t2);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Put(entity, t1);
                        container2.Put(entity, t2);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void PutForAll<T1, T2, T3>(T1 t1, T2 t2, T3 t3) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Put(entity, t1);
                            container2.Put(entity, t2);
                            container3.Put(entity, t3);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Put(entity, t1);
                        container2.Put(entity, t2);
                        container3.Put(entity, t3);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void PutForAll<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Put(entity, t1);
                            container2.Put(entity, t2);
                            container3.Put(entity, t3);
                            container4.Put(entity, t4);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Put(entity, t1);
                        container2.Put(entity, t2);
                        container3.Put(entity, t3);
                        container4.Put(entity, t4);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void PutForAll<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            ref var container5 = ref World<WorldType>.Components<T5>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Put(entity, t1);
                            container2.Put(entity, t2);
                            container3.Put(entity, t3);
                            container4.Put(entity, t4);
                            container5.Put(entity, t5);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Put(entity, t1);
                        container2.Put(entity, t2);
                        container3.Put(entity, t3);
                        container4.Put(entity, t4);
                        container5.Put(entity, t5);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteForAll<T1>() 
            where T1 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Delete(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Delete(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteForAll<T1, T2>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Delete(entity);
                            container2.Delete(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Delete(entity);
                        container2.Delete(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteForAll<T1, T2, T3>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Delete(entity);
                            container2.Delete(entity);
                            container3.Delete(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Delete(entity);
                        container2.Delete(entity);
                        container3.Delete(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteForAll<T1, T2, T3, T4>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Delete(entity);
                            container2.Delete(entity);
                            container3.Delete(entity);
                            container4.Delete(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Delete(entity);
                        container2.Delete(entity);
                        container3.Delete(entity);
                        container4.Delete(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            ref var container5 = ref World<WorldType>.Components<T5>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Delete(entity);
                            container2.Delete(entity);
                            container3.Delete(entity);
                            container4.Delete(entity);
                            container5.Delete(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Delete(entity);
                        container2.Delete(entity);
                        container3.Delete(entity);
                        container4.Delete(entity);
                        container5.Delete(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteForAll<T1>() 
            where T1 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.TryDelete(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.TryDelete(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteForAll<T1, T2>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.TryDelete(entity);
                            container2.TryDelete(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.TryDelete(entity);
                        container2.TryDelete(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteForAll<T1, T2, T3>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.TryDelete(entity);
                            container2.TryDelete(entity);
                            container3.TryDelete(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.TryDelete(entity);
                        container2.TryDelete(entity);
                        container3.TryDelete(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteForAll<T1, T2, T3, T4>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.TryDelete(entity);
                            container2.TryDelete(entity);
                            container3.TryDelete(entity);
                            container4.TryDelete(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.TryDelete(entity);
                        container2.TryDelete(entity);
                        container3.TryDelete(entity);
                        container4.TryDelete(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void TryDeleteForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, IComponent
            where T2 : struct, IComponent
            where T3 : struct, IComponent
            where T4 : struct, IComponent
            where T5 : struct, IComponent {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Components<T1>.Value;
            ref var container2 = ref World<WorldType>.Components<T2>.Value;
            ref var container3 = ref World<WorldType>.Components<T3>.Value;
            ref var container4 = ref World<WorldType>.Components<T4>.Value;
            ref var container5 = ref World<WorldType>.Components<T5>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.TryDelete(entity);
                            container2.TryDelete(entity);
                            container3.TryDelete(entity);
                            container4.TryDelete(entity);
                            container5.TryDelete(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.TryDelete(entity);
                        container2.TryDelete(entity);
                        container3.TryDelete(entity);
                        container4.TryDelete(entity);
                        container5.TryDelete(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        #endregion
        
        #region TAGS
        #if !FFS_ECS_DISABLE_TAGS
        [MethodImpl(AggressiveInlining)]
        public void SetTagForAll<T1>() where T1 : struct, ITag {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container = ref World<WorldType>.Tags<T1>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container.Set(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container.Set(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetTagForAll<T1, T2>() 
            where T1 : struct, ITag
            where T2 : struct, ITag {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Set(entity);
                            container2.Set(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Set(entity);
                        container2.Set(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetTagForAll<T1, T2, T3>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            ref var container3 = ref World<WorldType>.Tags<T3>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Set(entity);
                            container2.Set(entity);
                            container3.Set(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Set(entity);
                        container2.Set(entity);
                        container3.Set(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetTagForAll<T1, T2, T3, T4>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag
            where T4 : struct, ITag {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            ref var container3 = ref World<WorldType>.Tags<T3>.Value;
            ref var container4 = ref World<WorldType>.Tags<T4>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Set(entity);
                            container2.Set(entity);
                            container3.Set(entity);
                            container4.Set(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Set(entity);
                        container2.Set(entity);
                        container3.Set(entity);
                        container4.Set(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetTagForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag
            where T4 : struct, ITag
            where T5 : struct, ITag {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            ref var container3 = ref World<WorldType>.Tags<T3>.Value;
            ref var container4 = ref World<WorldType>.Tags<T4>.Value;
            ref var container5 = ref World<WorldType>.Tags<T5>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Set(entity);
                            container2.Set(entity);
                            container3.Set(entity);
                            container4.Set(entity);
                            container5.Set(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Set(entity);
                        container2.Set(entity);
                        container3.Set(entity);
                        container4.Set(entity);
                        container5.Set(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteTagForAll<T1>() 
            where T1 : struct, ITag {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Delete(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Delete(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteTagForAll<T1, T2>() 
            where T1 : struct, ITag
            where T2 : struct, ITag {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Delete(entity);
                            container2.Delete(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Delete(entity);
                        container2.Delete(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteTagForAll<T1, T2, T3>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            ref var container3 = ref World<WorldType>.Tags<T3>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Delete(entity);
                            container2.Delete(entity);
                            container3.Delete(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Delete(entity);
                        container2.Delete(entity);
                        container3.Delete(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteTagForAll<T1, T2, T3, T4>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag
            where T4 : struct, ITag {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            ref var container3 = ref World<WorldType>.Tags<T3>.Value;
            ref var container4 = ref World<WorldType>.Tags<T4>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Delete(entity);
                            container2.Delete(entity);
                            container3.Delete(entity);
                            container4.Delete(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Delete(entity);
                        container2.Delete(entity);
                        container3.Delete(entity);
                        container4.Delete(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        
        [MethodImpl(AggressiveInlining)]
        public void DeleteTagForAll<T1, T2, T3, T4, T5>() 
            where T1 : struct, ITag
            where T2 : struct, ITag
            where T3 : struct, ITag
            where T4 : struct, ITag
            where T5 : struct, ITag {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (disposed) throw new StaticEcsException($"QueryEntitiesIterator<{typeof(WorldType)}, {typeof(QM)}> already disposed");
            #endif
            DisposePart1();
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            ref var container1 = ref World<WorldType>.Tags<T1>.Value;
            ref var container2 = ref World<WorldType>.Tags<T2>.Value;
            ref var container3 = ref World<WorldType>.Tags<T3>.Value;
            ref var container4 = ref World<WorldType>.Tags<T4>.Value;
            ref var container5 = ref World<WorldType>.Tags<T5>.Value;
            while (firstGlobalBlockIdx >= 0) {
                ref var cache = ref qd.Blocks[firstGlobalBlockIdx];
                ref var entitiesMask = ref cache.EntitiesMask;
        
                if (entitiesMask.CheckBitDensity(out idx, out end)) {
                    while (idx < end) {
                        if ((entitiesMask & (1UL << idx++)) > 0) {
                            eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + idx - 1);
                            container1.Delete(entity);
                            container2.Delete(entity);
                            container3.Delete(entity);
                            container4.Delete(entity);
                            container5.Delete(entity);
                        }
                    }
                } else {
                    while (entitiesMask > 0) {
                        eid = (uint) ((firstGlobalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Utils.PopLsb(ref entitiesMask));
                        container1.Delete(entity);
                        container2.Delete(entity);
                        container3.Delete(entity);
                        container4.Delete(entity);
                        container5.Delete(entity);
                    }
                }
                
                firstGlobalBlockIdx = cache.NextGlobalBlock;
            }
            DisposePart2();
        }
        #endif
        #endregion
    }
}