#if !FFS_ECS_DISABLE_TAGS
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

        [MethodImpl(AggressiveInlining)]
        public void SetNotEmptyBit(byte index) {
            var mask = 1UL << index;
            #if NET6_0_OR_GREATER
            Interlocked.Or(ref notEmptyBlocksSigned, (long) mask);
            #else
            long orig, newVal;
            do {
                orig = notEmptyBlocksSigned;
                newVal = orig | (long) mask;
            } while (Interlocked.CompareExchange(ref notEmptyBlocksSigned, newVal, orig) != orig);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public ulong ClearNotEmptyBit(byte index) {
            var mask = ~(1UL << index);
            #if NET6_0_OR_GREATER
            return (ulong) Interlocked.And(ref notEmptyBlocksSigned, (long) mask);
            #else
            long orig, newVal;
            do {
                orig = notEmptyBlocksSigned;
                newVal = orig & (long) mask;
            } while (Interlocked.CompareExchange(ref notEmptyBlocksSigned, newVal, orig) != orig);

            return (ulong) newVal;
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public void SetFullBit(byte index) {
            var mask = 1UL << index;
            #if NET6_0_OR_GREATER
            Interlocked.Or(ref fullBlocksSigned, (long) mask);
            #else
            long orig, newVal;
            do {
                orig = fullBlocksSigned;
                newVal = orig | (long) mask;
            } while (Interlocked.CompareExchange(ref fullBlocksSigned, newVal, orig) != orig);
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public void ClearFullBit(byte index) {
            var mask = ~(1UL << index);
            #if NET6_0_OR_GREATER
            Interlocked.And(ref fullBlocksSigned, (long) mask);
            #else
            long orig, newVal;
            do {
                orig = fullBlocksSigned;
                newVal = orig & (long) mask;
            } while (Interlocked.CompareExchange(ref fullBlocksSigned, newVal, orig) != orig);
            #endif
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
            
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<ITagDebugEventListener> debugEventListeners;
            #endif
            
            internal TagsChunk[] chunks;
            private QueryData[] _queriesToUpdateOnDelete;
            private QueryData[] _queriesToUpdateOnAdd;
            
            internal ushort id;
            internal ushort maskLen;
            internal ulong idMask;
            internal ulong idMaskInv;
            internal ushort idDiv;
            private bool _registered;
            internal byte _queriesToUpdateOnDeleteCount;
            internal byte _queriesToUpdateOnAddCount;
            
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            private int _blockerDelete;
            private int _blockerAdd;
            #endif

            #region PUBLIC
            [MethodImpl(AggressiveInlining)]
            public bool Set(Entity entity) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Add, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Add, Tag type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Add, cannot access ID - {id} from deleted entity");
                if (_blockerAdd > 0 && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method Add, pool is blocked, use QueryMode.Flexible");
                if (MultiThreadActive && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Add, pool is blocked, it is forbidden to modify a non-current entity in a parallel query");
                #endif
                
                var eid = entity._id;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityMask = 1UL << (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                
                ref var chunk = ref chunks[chunkIdx];
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

                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnTagAdd<T>(entity);
                }
                #endif
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            public bool Has(Entity entity) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Has, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Has, Tag type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Has, cannot access ID - {id} from deleted entity");
                #endif
                var eid = entity._id;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                return (chunks[chunkIdx].entities[blockIdx] & (1UL << blockEntityIdx)) != 0;
            }

            [MethodImpl(AggressiveInlining)]
            public bool Delete(Entity entity, bool deleteMask = true) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Delete, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Delete, Tag type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Delete, cannot access ID - {id} from deleted entity");
                if (_blockerDelete > 0 && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method Delete, pool is blocked, use QueryMode.Flexible");
                if (MultiThreadActive && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Delete, pool is blocked, it is forbidden to modify a non-current entity in a parallel query");
                #endif
                var eid = entity._id;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityMask = 1UL << (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                var blockEntityInvMask = ~blockEntityMask;
                
                ref var chunk = ref chunks[chunkIdx];
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
                    chunk.ClearNotEmptyBit(blockIdx);
                }
                
                for (uint i = 0; i < _queriesToUpdateOnDeleteCount; i++) {
                    _queriesToUpdateOnDelete[i].Update(blockEntityInvMask, eid);
                }
 
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnTagDelete<T>(entity);
                }
                #endif
                
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            public void Copy(Entity src, Entity dst) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Copy, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Copy, Tag type not registered");
                if (!src.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
                if (!dst.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
                #endif

                if (Has(src)) {
                    Set(dst);
                }
            }
                        
            [MethodImpl(AggressiveInlining)]
            public void Move(Entity src, Entity dst) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Move, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Move, Tag type not registered");
                if (!src.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Move, cannot access ID - {id} from deleted entity");
                if (!dst.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Move, cannot access ID - {id} from deleted entity");
                #endif

                if (Has(src)) {
                    Set(dst);
                    Delete(src);
                }
            }
                        
            [MethodImpl(AggressiveInlining)]
            public bool Toggle(Entity entity) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Toggle, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Toggle, Tag type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Toggle, cannot access ID - {id} from deleted entity");
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
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Apply, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Apply, Tag type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Apply, cannot access ID - {id} from deleted entity");
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
            [MethodImpl(AggressiveInlining)]
            internal void UpdateBitMask(BitMask bitMask) {
                for (var i = 0; i < bitMask.chunks.Length; i++) {
                    chunks[i].bitmask = bitMask.chunks[i];
                }

                maskLen = bitMask.maskLen;
            }
            
            internal void Create(Guid guid, ushort tagId, uint entitiesCapacity) {
                SetDynamicId(tagId);
                var chunkCount = entitiesCapacity >> Const.ENTITIES_IN_CHUNK_SHIFT;
                chunks = new TagsChunk[chunkCount];
                for (var i = 0; i < chunkCount; i++) {
                    chunks[i].entities = new ulong[Const.BLOCK_IN_CHUNK];
                }
                Serializer.Value.Create(guid);
                _queriesToUpdateOnDelete = new QueryData[Const.MAX_NESTED_QUERY];
                _queriesToUpdateOnAdd = new QueryData[Const.MAX_NESTED_QUERY];
                _queriesToUpdateOnDeleteCount = 0;
                _queriesToUpdateOnAddCount = 0;
                _registered = true;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                _blockerDelete = 0;
                _blockerAdd = 0;
                #endif
            }
            
            
            [MethodImpl(AggressiveInlining)]
            public ushort DynamicId() {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (Status < WorldStatus.Created) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: DynamicId, World not created");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: DynamicId, Tag type not registered");
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
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (Status < WorldStatus.Created) throw new StaticEcsException($"World<{typeof(WorldType)}>.Tags<{typeof(T)}> Method: Guid, World not created");
                if (!_registered) throw new Exception($"World<{typeof(StaticEcsException)}>.Masks<{typeof(T)}>, Method: Guid, Mask type not registered");
                #endif
                return Serializer.Value.guid;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Resize(uint entitiesCapacity) {
                var chunkCount = entitiesCapacity >> Const.ENTITIES_IN_CHUNK_SHIFT;
                
                if (chunks.Length < chunkCount) {
                    var old = chunks.Length;
                    Array.Resize(ref chunks, (int) chunkCount);
                    for (var i = old; i < chunkCount; i++) {
                        chunks[i].entities = new ulong[Const.BLOCK_IN_CHUNK];
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                chunks = null;
                id = 0;
                maskLen = 0;
                idMask = 0;
                idMaskInv = 0;
                idDiv = 0;
                Serializer.Value.Destroy();
                _registered = false;
                _queriesToUpdateOnDelete = null;
                _queriesToUpdateOnAdd = null;
                _queriesToUpdateOnDeleteCount = 0;
                _queriesToUpdateOnAddCount = 0;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                debugEventListeners = null;
                _blockerDelete = 0;
                _blockerAdd = 0;
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                for (var i = 0; i < chunks.Length; i++) {
                    ref var chunk = ref chunks[i];
                    Array.Clear(chunk.entities,  0, chunk.entities.Length);
                    chunk.notEmptyBlocks = 0;
                    chunk.fullBlocks = 0;
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal ref TagsChunk Chunk(uint chunkIdx) {
                return ref chunks[chunkIdx];
            }
            
            [MethodImpl(AggressiveInlining)]
            internal ulong EMask(uint chunkIdx, int blockIdx) {
                return chunks[chunkIdx].entities[blockIdx];
            }

            [MethodImpl(AggressiveInlining)]
            internal void IncQDelete(QueryData data) => _queriesToUpdateOnDelete[_queriesToUpdateOnDeleteCount++] = data;

            [MethodImpl(AggressiveInlining)]
            internal void DecQDelete() => _queriesToUpdateOnDelete[--_queriesToUpdateOnDeleteCount] = default;
            
            [MethodImpl(AggressiveInlining)]
            internal void IncQAdd(QueryData data) => _queriesToUpdateOnAdd[_queriesToUpdateOnAddCount++] = data;
            
            [MethodImpl(AggressiveInlining)]
            internal void DecQAdd() => _queriesToUpdateOnAdd[--_queriesToUpdateOnAddCount] = default;
            
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
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
#endif