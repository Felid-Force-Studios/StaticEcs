#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [StructLayout(LayoutKind.Explicit)]
    public struct TagsChunk {
        [FieldOffset(0)] internal ulong notEmptyBlocks;
        [FieldOffset(0)] private long notEmptyBlocksSigned;
        [FieldOffset(8)] internal ulong fullBlocks;
        [FieldOffset(8)] private long fullBlocksSigned;
        [FieldOffset(72)] internal ulong[] entities;
        [FieldOffset(80)] internal ulong[] bitmask;
        [FieldOffset(88)] internal SpinLock chunkLock;
        
        [MethodImpl(AggressiveInlining)]
        internal TagsChunkData MoveToPoolData() {
            var result = new TagsChunkData(entities);
            entities = null;
            return result;
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void InitFromPoolData(ref TagsChunkData data) {
            entities = data.entities;
            data = default;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetNotEmptyBit(byte index) {
            var mask = 1UL << index;
            long orig, newVal;
            do {
                orig = notEmptyBlocksSigned;
                newVal = orig | (long) mask;
            } while (Interlocked.CompareExchange(ref notEmptyBlocksSigned, newVal, orig) != orig);
        }

        [MethodImpl(AggressiveInlining)]
        public ulong ClearNotEmptyBit(byte index) {
            var mask = ~(1UL << index);
            long orig, newVal;
            do {
                orig = notEmptyBlocksSigned;
                newVal = orig & (long) mask;
            } while (Interlocked.CompareExchange(ref notEmptyBlocksSigned, newVal, orig) != orig);

            return (ulong) newVal;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetFullBit(byte index) {
            var mask = 1UL << index;
            long orig, newVal;
            do {
                orig = fullBlocksSigned;
                newVal = orig | (long) mask;
            } while (Interlocked.CompareExchange(ref fullBlocksSigned, newVal, orig) != orig);
        }

        [MethodImpl(AggressiveInlining)]
        public void ClearFullBit(byte index) {
            var mask = ~(1UL << index);
            long orig, newVal;
            do {
                orig = fullBlocksSigned;
                newVal = orig & (long) mask;
            } while (Interlocked.CompareExchange(ref fullBlocksSigned, newVal, orig) != orig);
        }
    }
    
        
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct TagsChunkData {
        public readonly ulong[] entities;
        
        [MethodImpl(AggressiveInlining)]
        public TagsChunkData(ulong[] entities) {
            this.entities = entities;
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        public partial struct Tags<T> where T : struct, ITag {
            public static Tags<T> Value;
            
            #if FFS_ECS_EVENTS
            internal List<ITagDebugEventListener> debugEventListeners;
            #endif
            
            internal TagsChunk[] chunks;
            internal TagsChunkData[] chunksDataPool;
            private QueryData[] _queriesToUpdateOnDelete;
            private QueryData[] _queriesToUpdateOnAdd;
            private BitMask _bitMask;
            private FreeChunkCommandBuffer _freeChunkCommandBuffer;
            
            internal int chunksDataPoolCount;
            internal ushort id;
            internal ushort maskLen;
            internal ulong idMask;
            internal ulong idMaskInv;
            internal ushort idDiv;
            private bool _registered;
            internal byte _queriesToUpdateOnDeleteCount;
            internal byte _queriesToUpdateOnAddCount;
            
            #if FFS_ECS_DEBUG
            
            internal static string TagsTypeName = $"{WorldTypeName}.Tags<{typeof(T).GenericName()}>"; 
            
            private int _blockerDelete;
            private int _blockerAdd;
            #endif

            #region PUBLIC
            [MethodImpl(AggressiveInlining)]
            public bool Set(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(TagsTypeName);
                AssertRegisteredTag<T>(TagsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(TagsTypeName, entity);
                AssertNotBlockedByQuery(TagsTypeName, entity, _blockerAdd);
                AssertNotBlockedByParallelQuery(TagsTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityMask = 1UL << (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                
                ref var chunk = ref chunks[chunkIdx];
                if (chunk.entities == null) {
                    InitChunk(ref chunk, chunkIdx);
                }
                ref var entitiesMask = ref chunk.entities[blockIdx];

                if ((entitiesMask & blockEntityMask) != 0) {
                    return false;
                }
                
                if (entitiesMask == 0) {
                    chunk.SetNotEmptyBit(blockIdx);
                }
                entitiesMask |= blockEntityMask;
                if (entitiesMask == ulong.MaxValue) {
                    chunk.SetFullBit(blockIdx);
                }
                
                chunk.bitmask[(eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + idDiv] |= idMask;
                
                for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                    _queriesToUpdateOnAdd[i].Update(~blockEntityMask, eid);
                }

                #if FFS_ECS_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnTagAdd<T>(entity);
                }
                #endif
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            public bool Has(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(TagsTypeName);
                AssertRegisteredTag<T>(TagsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(TagsTypeName, entity);
                #endif
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                var entities = chunks[chunkIdx].entities;

                return entities != null && (entities[blockIdx] & (1UL << blockEntityIdx)) != 0;
            }

            [MethodImpl(AggressiveInlining)]
            public bool Delete(Entity entity, bool deleteMask = true) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(TagsTypeName);
                AssertRegisteredTag<T>(TagsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(TagsTypeName, entity);
                AssertNotBlockedByQuery(TagsTypeName, entity, _blockerDelete);
                AssertNotBlockedByParallelQuery(TagsTypeName, entity);
                #endif
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityMask = 1UL << (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                var blockEntityInvMask = ~blockEntityMask;
                
                ref var chunk = ref chunks[chunkIdx];
                if (chunk.entities == null) {
                    return false;
                }
                
                ref var entitiesMask = ref chunk.entities[blockIdx];
                
                if ((entitiesMask & blockEntityMask) == 0) {
                    return false;
                }

                if (deleteMask) {
                    chunk.bitmask[(eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + idDiv] &= idMaskInv;
                }
                
                if (entitiesMask == ulong.MaxValue) {
                    chunk.ClearFullBit(blockIdx);
                }
                entitiesMask &= blockEntityInvMask;
                
                if (entitiesMask == 0) {
                    if (chunk.ClearNotEmptyBit(blockIdx) == 0) {
                        if (!MultiThreadActive) {
                            chunksDataPool[chunksDataPoolCount++] = chunk.MoveToPoolData();
                        } else {
                            _freeChunkCommandBuffer.Add(chunkIdx, id);
                        }
                    }
                }
                
                for (uint i = 0; i < _queriesToUpdateOnDeleteCount; i++) {
                    _queriesToUpdateOnDelete[i].Update(blockEntityInvMask, eid);
                }
 
                #if FFS_ECS_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnTagDelete<T>(entity);
                }
                #endif
                
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            public void Copy(Entity src, Entity dst) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(TagsTypeName);
                AssertRegisteredTag<T>(TagsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(TagsTypeName, src);
                AssertEntityIsNotDestroyedAndLoaded(TagsTypeName, dst);
                #endif

                if (Has(src)) {
                    Set(dst);
                }
            }
                        
            [MethodImpl(AggressiveInlining)]
            public void Move(Entity src, Entity dst) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(TagsTypeName);
                AssertRegisteredTag<T>(TagsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(TagsTypeName, src);
                AssertEntityIsNotDestroyedAndLoaded(TagsTypeName, dst);
                #endif

                if (Has(src)) {
                    Set(dst);
                    Delete(src);
                }
            }
                        
            [MethodImpl(AggressiveInlining)]
            public bool Toggle(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(TagsTypeName);
                AssertRegisteredTag<T>(TagsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(TagsTypeName, entity);
                #endif

                if (Has(entity)) {
                    Delete(entity);
                    return false;
                }

                Set(entity);
                return true;
            }
                        
            [MethodImpl(AggressiveInlining)]
            public void Apply(Entity entity, bool state) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(TagsTypeName);
                AssertRegisteredTag<T>(TagsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(TagsTypeName, entity);
                #endif
                
                if (state) {
                    Set(entity);
                } else {
                    Delete(entity);
                }
            }
                       
            [MethodImpl(AggressiveInlining)]
            public bool IsRegistered() {
                return _registered;
            }

            [MethodImpl(AggressiveInlining)]
            public void ToStringComponent(StringBuilder builder, Entity entity) {
                builder.Append(" - [");
                builder.Append(id);
                builder.Append("] ");
                builder.AppendLine(typeof(T).Name);
            }
            
            [MethodImpl(AggressiveInlining)]
            public uint CalculateCount() {
                var count = 0;
                for (var i = 0; i < chunks.Length; i++) {
                    ref var chunk = ref chunks[i];
                    var notEmptyBlocks = chunk.notEmptyBlocks;
                    while (notEmptyBlocks > 0) {
                        var blockIdx = Utils.PopLsb(ref notEmptyBlocks);
                        count += chunk.entities[blockIdx].PopCnt();
                    }
                }

                return (uint) count;
            }
            #endregion

            #region INTERNAL
            [MethodImpl(NoInlining)]
            private void InitChunk(ref TagsChunk chunk, uint chunkIdx) {
                var taken = false;
                chunk.chunkLock.Enter(ref taken);
                #if FFS_ECS_DEBUG
                if (!taken) throw new StaticEcsException($"Failed to acquire tags lock for chunk {chunkIdx}");
                #endif
                if (chunk.entities == null) {
                    var count = Interlocked.Decrement(ref chunksDataPoolCount);
                    if (count >= 0) {
                        chunk.InitFromPoolData(ref chunksDataPool[count]);
                    } else {
                        Interlocked.Increment(ref chunksDataPoolCount);
                        chunk.entities = new ulong[Const.BLOCK_IN_CHUNK];
                    }
                    chunk.bitmask = _bitMask.chunks[chunkIdx];
                }
                chunk.chunkLock.Exit();
            }
            
            [MethodImpl(AggressiveInlining)]
            private void InitChunkSimple(ref TagsChunk chunk, uint chunkIdx) {
                if (chunksDataPoolCount > 0) {
                    chunk.InitFromPoolData(ref chunksDataPool[--chunksDataPoolCount]);
                } else {
                    chunk.entities = new ulong[Const.BLOCK_IN_CHUNK];
                    chunk.bitmask = _bitMask.chunks[chunkIdx];
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void TryMoveChunkToPool(uint chunkIdx) {
                ref var chunk = ref chunks[chunkIdx];
                if (chunk.notEmptyBlocks == 0) {
                    chunksDataPool[chunksDataPoolCount++] = chunk.MoveToPoolData();
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void UpdateBitMask(uint chunkIdx) {
                chunks[chunkIdx].bitmask = _bitMask.chunks[chunkIdx];
                maskLen = _bitMask.maskLen;
            }
            
            internal void Create(Guid guid, ushort tagId, FreeChunkCommandBuffer freeChunkCommandBuffer, BitMask bitMask) {
                SetDynamicId(tagId);
                _freeChunkCommandBuffer = freeChunkCommandBuffer;
                _bitMask = bitMask;
                Serializer.Value.Create(guid);
                _queriesToUpdateOnDelete = new QueryData[Const.MAX_NESTED_QUERY];
                _queriesToUpdateOnAdd = new QueryData[Const.MAX_NESTED_QUERY];
                _queriesToUpdateOnDeleteCount = 0;
                _queriesToUpdateOnAddCount = 0;
                chunksDataPoolCount = 0;
                _registered = true;
                #if FFS_ECS_EVENTS
                _blockerDelete = 0;
                _blockerAdd = 0;
                #endif
            }
            
            internal void Initialize(uint chunksCapacity) {
                chunksDataPool = new TagsChunkData[chunksCapacity];
                chunks = new TagsChunk[chunksCapacity];
                for (var i = 0; i < chunks.Length; i++) {
                    chunks[i].chunkLock = new SpinLock(false);
                }
                maskLen = _bitMask.maskLen;
            }
            
            [MethodImpl(AggressiveInlining)]
            public ushort DynamicId() {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(TagsTypeName);
                AssertRegisteredTag<T>(TagsTypeName);
                #endif
                return id;
            }

            [MethodImpl(AggressiveInlining)]
            internal void SetDynamicId(ushort val) {
                id = val;
                idDiv = (ushort) (id >> Const.LONG_SHIFT);
                idMask = 1UL << (id & Const.LONG_OFFSET_MASK);
                idMaskInv = ~idMask;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal Guid Guid() {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(TagsTypeName);
                AssertRegisteredTag<T>(TagsTypeName);
                #endif
                return Serializer.Value.guid;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Resize(uint chunksCapacity) {
                var oldChunksCapacity = chunks.Length;
                Array.Resize(ref chunksDataPool, (int) chunksCapacity);
                Array.Resize(ref chunks, (int) chunksCapacity);
                for (var i = oldChunksCapacity; i < chunks.Length; i++) {
                    chunks[i].chunkLock = new SpinLock(false);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                _freeChunkCommandBuffer = null;
                chunks = null;
                chunksDataPool = null;
                chunksDataPoolCount = 0;
                id = 0;
                maskLen = 0;
                idMask = 0;
                idMaskInv = 0;
                idDiv = 0;
                Serializer.Value.Destroy();
                _registered = false;
                _queriesToUpdateOnDelete = null;
                _queriesToUpdateOnAdd = null;
                _bitMask = null;
                _queriesToUpdateOnDeleteCount = 0;
                _queriesToUpdateOnAddCount = 0;
                #if FFS_ECS_EVENTS
                debugEventListeners = null;
                #endif
                #if FFS_ECS_DEBUG
                _blockerDelete = 0;
                _blockerAdd = 0;
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal void ClearChunk(uint chunkIdx) {
                ref var chunk = ref chunks[chunkIdx];
                if (chunk.entities != null) {
                    if (chunk.notEmptyBlocks != 0) {
                        Array.Clear(chunk.entities,  0, Const.BLOCK_IN_CHUNK);
                    }
                    chunksDataPool[chunksDataPoolCount++] = chunk.MoveToPoolData();
                }
                chunk.bitmask = null;
                chunk.notEmptyBlocks = 0;
                chunk.fullBlocks = 0;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal ref TagsChunk Chunk(uint chunkIdx) {
                return ref chunks[chunkIdx];
            }
            
            [MethodImpl(AggressiveInlining)]
            internal ulong EMask(uint chunkIdx, int blockIdx) {
                var entities = chunks[chunkIdx].entities;
                return entities != null ? entities[blockIdx] : 0UL;
            }

            [MethodImpl(AggressiveInlining)]
            internal void IncQDelete(QueryData data) => _queriesToUpdateOnDelete[_queriesToUpdateOnDeleteCount++] = data;

            [MethodImpl(AggressiveInlining)]
            internal void DecQDelete() => _queriesToUpdateOnDelete[--_queriesToUpdateOnDeleteCount] = default;
            
            [MethodImpl(AggressiveInlining)]
            internal void IncQAdd(QueryData data) => _queriesToUpdateOnAdd[_queriesToUpdateOnAddCount++] = data;
            
            [MethodImpl(AggressiveInlining)]
            internal void DecQAdd() => _queriesToUpdateOnAdd[--_queriesToUpdateOnAddCount] = default;
            
            #if FFS_ECS_DEBUG
            [MethodImpl(AggressiveInlining)]
            internal void BlockDelete(int val) {
                _blockerDelete += val;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void BlockAdd(int val) {
                _blockerAdd += val;
            }
            #endif
            #endregion
        }
    }
}
