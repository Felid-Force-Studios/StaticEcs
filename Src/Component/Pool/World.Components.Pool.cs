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
    public struct ComponentsChunk {
        [FieldOffset(0)] internal ulong notEmptyBlocks;
        [FieldOffset(0)] private long notEmptyBlocksSigned;
        [FieldOffset(8)] internal ulong fullBlocks;
        [FieldOffset(8)] private long fullBlocksSigned;
        [FieldOffset(72)] internal ulong[] entities;
        [FieldOffset(80)] internal ulong[] disabledEntities;
        [FieldOffset(88)] internal ulong[] bitmask;
        [FieldOffset(96)] internal SpinLock chunkLock;
        
        [MethodImpl(AggressiveInlining)]
        internal ComponentsChunkData MoveToPoolData() {
            var result = new ComponentsChunkData(entities, disabledEntities);
            entities = null;
            disabledEntities = null;
            return result;
        }
        
        [MethodImpl(AggressiveInlining)]
        internal void InitFromPoolData(ref ComponentsChunkData data) {
            entities = data.entities;
            disabledEntities = data.disabledEntities;
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
    public struct ComponentsChunkData {
        public readonly ulong[] entities;
        public readonly ulong[] disabledEntities;
        
        [MethodImpl(AggressiveInlining)]
        public ComponentsChunkData(ulong[] entities, ulong[] disabledEntities) {
            this.entities = entities;
            this.disabledEntities = disabledEntities;
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
        public partial struct Components<T> where T : struct, IComponent {
            public static Components<T> Value;

            #if FFS_ECS_EVENTS
            internal List<IComponentsDebugEventListener> debugEventListeners;
            #endif

            internal OnComponentHandler<T> onAddHandler;
            internal OnComponentHandler<T> onPutHandler;
            internal OnComponentHandler<T> onDeleteHandler;
            internal OnCopyHandler<T> onCopyHandler;
            internal ulong[] dataMaskCache;
            internal T[][] data;
            internal T[][] dataPool;
            internal ComponentsChunk[] chunks;
            internal ComponentsChunkData[] chunksDataPool;
            private QueryData[] _queriesToUpdateOnDelete;
            private QueryData[] _queriesToUpdateOnAdd;
            private QueryData[] _queriesToUpdateOnDisable;
            private QueryData[] _queriesToUpdateOnEnable;
            private BitMask _bitMask;
            private FreeChunkCommandBuffer _freeChunkCommandBuffer;

            internal long dataPoolCount;
            internal int chunksDataPoolCount;
            internal ushort id;
            internal ushort maskLen;
            internal ulong idMask;
            internal ulong idMaskInv;
            internal ushort idDiv;
            private bool _copyable;
            private bool _clearable;
            private byte _queriesToUpdateOnDeleteCount;
            private byte _queriesToUpdateOnAddCount;
            private byte _queriesToUpdateOnDisableCount;
            private byte _queriesToUpdateOnEnableCount;

            #if FFS_ECS_DEBUG
            internal static string ComponentsTypeName = $"{WorldTypeName}.Components<{typeof(T).GenericName()}>"; 
            
            internal string addWithoutValueError;
            private int _blockerDelete;
            private int _blockerAdd;
            private int _blockerDisable;
            private int _blockerEnable;
            #endif

            private bool _registered;

            #region PUBLIC
            [MethodImpl(AggressiveInlining)]
            public ref T Ref(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertEntityHasComponent<T>(ComponentsTypeName, entity);
                #endif

                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                #if FFS_ECS_EVENTS
                ref var val = ref data[eid >> Const.DATA_SHIFT][eid & Const.DATA_ENTITY_MASK];
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentRef(entity, ref val);
                }

                return ref val;
                #else
                return ref data[eid >> Const.DATA_SHIFT][eid & Const.DATA_ENTITY_MASK];
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public ref T Add(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertEntityNotHasComponent<T>(ComponentsTypeName, entity);
                Assert(ComponentsTypeName, addWithoutValueError == null, addWithoutValueError);
                AssertNotBlockedByQuery(ComponentsTypeName, entity, _blockerAdd);
                AssertNotBlockedByParallelQuery(ComponentsTypeName, entity);
                #endif

                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var dataIdx = (ushort) (eid >> Const.DATA_SHIFT);
                var dataEIdx = (ushort) (eid & Const.DATA_ENTITY_MASK);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);

                ref var chunk = ref chunks[chunkIdx];
                if (chunk.entities == null) {
                    InitChunk(ref chunk, chunkIdx);
                }
                ref var entitiesMask = ref chunk.entities[blockIdx];

                if (entitiesMask == 0) {
                    chunk.SetNotEmptyBit(blockIdx);
                    if (data[dataIdx] == null) {
                        var count = Interlocked.Decrement(ref dataPoolCount);
                        if (count >= 0) {
                            data[dataIdx] = dataPool[count];
                        } else {
                            Interlocked.Increment(ref dataPoolCount);
                            data[dataIdx] = new T[Const.DATA_BLOCK_SIZE];
                        }
                    }
                }

                entitiesMask |= 1UL << blockEntityIdx;
                if (entitiesMask == ulong.MaxValue) {
                    chunk.SetFullBit(blockIdx);
                }

                chunk.bitmask[(eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + idDiv] |= idMask;
                onAddHandler?.Invoke(entity, ref data[dataIdx][dataEIdx]);

                for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                    _queriesToUpdateOnAdd[i].Update(~(1UL << blockEntityIdx), eid);
                }

                #if FFS_ECS_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentAdd(entity, ref data[dataIdx][dataEIdx]);
                }
                #endif


                return ref data[dataIdx][dataEIdx];
            }

            [MethodImpl(AggressiveInlining)]
            public ref T Add(Entity entity, T component) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertEntityNotHasComponent<T>(ComponentsTypeName, entity);
                AssertNotBlockedByQuery(ComponentsTypeName, entity, _blockerAdd);
                AssertNotBlockedByParallelQuery(ComponentsTypeName, entity);
                #endif

                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var dataIdx = (ushort) (eid >> Const.DATA_SHIFT);
                var dataEIdx = (ushort) (eid & Const.DATA_ENTITY_MASK);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);

                ref var chunk = ref chunks[chunkIdx];
                if (chunk.entities == null) {
                    InitChunk(ref chunk, chunkIdx);
                }
                ref var entitiesMask = ref chunk.entities[blockIdx];

                if (entitiesMask == 0) {
                    chunk.SetNotEmptyBit(blockIdx);
                    if (data[dataIdx] == null) {
                        var count = Interlocked.Decrement(ref dataPoolCount);
                        if (count >= 0) {
                            data[dataIdx] = dataPool[count];
                        } else {
                            Interlocked.Increment(ref dataPoolCount);
                            data[dataIdx] = new T[Const.DATA_BLOCK_SIZE];
                        }
                    }
                }

                entitiesMask |= 1UL << blockEntityIdx;
                if (entitiesMask == ulong.MaxValue) {
                    chunk.SetFullBit(blockIdx);
                }

                ref var val = ref data[dataIdx][dataEIdx];
                val = component;
                chunk.bitmask[(eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + idDiv] |= idMask;
                onAddHandler?.Invoke(entity, ref val);

                for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                    _queriesToUpdateOnAdd[i].Update(~(1UL << blockEntityIdx), eid);
                }

                #if FFS_ECS_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentAdd(entity, ref val);
                }
                #endif

                return ref val;
            }

            [MethodImpl(AggressiveInlining)]
            internal void AddInternal(Entity entity, T component) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertEntityNotHasComponent<T>(ComponentsTypeName, entity);
                AssertNotBlockedByQuery(ComponentsTypeName, entity, _blockerAdd);
                AssertNotBlockedByParallelQuery(ComponentsTypeName, entity);
                #endif

                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var dataIdx = (ushort) (eid >> Const.DATA_SHIFT);
                var dataEIdx = (ushort) (eid & Const.DATA_ENTITY_MASK);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);

                ref var chunk = ref chunks[chunkIdx];
                if (chunk.entities == null) {
                    InitChunk(ref chunk, chunkIdx);
                }
                ref var entitiesMask = ref chunk.entities[blockIdx];

                if (entitiesMask == 0) {
                    chunk.SetNotEmptyBit(blockIdx);
                    if (data[dataIdx] == null) {
                        var count = Interlocked.Decrement(ref dataPoolCount);
                        if (count >= 0) {
                            data[dataIdx] = dataPool[count];
                        } else {
                            Interlocked.Increment(ref dataPoolCount);
                            data[dataIdx] = new T[Const.DATA_BLOCK_SIZE];
                        }
                    }
                }

                entitiesMask |= 1UL << blockEntityIdx;
                if (entitiesMask == ulong.MaxValue) {
                    chunk.SetFullBit(blockIdx);
                }

                ref var val = ref data[dataIdx][dataEIdx];
                val = component;
                chunk.bitmask[(eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + idDiv] |= idMask;

                for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                    _queriesToUpdateOnAdd[i].Update(~(1UL << blockEntityIdx), eid);
                }

                #if FFS_ECS_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentAdd(entity, ref val);
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public void Put(Entity entity, T component) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertNotBlockedByQuery(ComponentsTypeName, entity, _blockerAdd);
                AssertNotBlockedByParallelQuery(ComponentsTypeName, entity);
                #endif

                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var dataIdx = (ushort) (eid >> Const.DATA_SHIFT);
                var dataEIdx = (ushort) (eid & Const.DATA_ENTITY_MASK);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);

                ref var chunk = ref chunks[chunkIdx];
                if (chunk.entities == null) {
                    InitChunk(ref chunk, chunkIdx);
                }
                
                ref var entitiesMask = ref chunk.entities[blockIdx];

                if ((entitiesMask & (1UL << blockEntityIdx)) != 0) {
                    ref var val = ref data[dataIdx][dataEIdx];
                    val = component;
                    onPutHandler?.Invoke(entity, ref val);
                    #if FFS_ECS_EVENTS
                    foreach (var listener in debugEventListeners) {
                        listener.OnComponentPut(entity, ref val);
                    }
                    #endif
                    return;
                }

                if (entitiesMask == 0) {
                    chunk.SetNotEmptyBit(blockIdx);
                    if (data[dataIdx] == null) {
                        var count = Interlocked.Decrement(ref dataPoolCount);
                        if (count >= 0) {
                            data[dataIdx] = dataPool[count];
                        } else {
                            Interlocked.Increment(ref dataPoolCount);
                            data[dataIdx] = new T[Const.DATA_BLOCK_SIZE];
                        }
                    }
                }

                entitiesMask |= 1UL << blockEntityIdx;
                if (entitiesMask == ulong.MaxValue) {
                    chunk.SetFullBit(blockIdx);
                }

                ref var newVal = ref data[dataIdx][dataEIdx];
                newVal = component;
                chunk.bitmask[(eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + idDiv] |= idMask;
                onPutHandler?.Invoke(entity, ref newVal);

                for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                    _queriesToUpdateOnAdd[i].Update(~(1UL << blockEntityIdx), eid);
                }

                #if FFS_ECS_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentPut(entity, ref newVal);
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public ref T TryAdd(Entity entity) {
                return ref Has(entity)
                    ? ref Ref(entity)
                    : ref Add(entity);
            }

            [MethodImpl(AggressiveInlining)]
            public ref T TryAdd(Entity entity, out bool added) {
                added = !Has(entity);
                return ref added
                    ? ref Add(entity)
                    : ref Ref(entity);
            }

            [MethodImpl(AggressiveInlining)]
            public bool Has(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                #endif

                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                var entities = chunks[chunkIdx].entities;
                
                return entities != null && (entities[blockIdx] & (1UL << blockEntityIdx)) != 0;
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasDisabled(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                var disabledEntities = chunks[chunkIdx].disabledEntities;
                
                return disabledEntities != null && (disabledEntities[blockIdx] & (1UL << blockEntityIdx)) != 0;
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasEnabled(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                var entities = chunks[chunkIdx].entities;
                
                return entities != null && (entities[blockIdx] & ~chunks[chunkIdx].disabledEntities[blockIdx] & (1UL << blockEntityIdx)) != 0;
            }

            [MethodImpl(AggressiveInlining)]
            public void Disable(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertEntityHasComponent<T>(ComponentsTypeName, entity);
                AssertNotBlockedByQuery(ComponentsTypeName, entity, _blockerDisable);
                AssertNotBlockedByParallelQuery(ComponentsTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                chunks[chunkIdx].disabledEntities[blockIdx] |= 1UL << blockEntityIdx;
                for (uint i = 0; i < _queriesToUpdateOnDisableCount; i++) {
                    _queriesToUpdateOnDisable[i].Update(~(1UL << blockEntityIdx), eid);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public void Enable(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertEntityHasComponent<T>(ComponentsTypeName, entity);
                AssertNotBlockedByQuery(ComponentsTypeName, entity, _blockerEnable);
                AssertNotBlockedByParallelQuery(ComponentsTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                var invMask = ~(1UL << blockEntityIdx);
                chunks[chunkIdx].disabledEntities[blockIdx] &= invMask;
                for (uint i = 0; i < _queriesToUpdateOnEnableCount; i++) {
                    _queriesToUpdateOnEnable[i].Update(~(1UL << blockEntityIdx), eid);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public void Delete(Entity entity, bool deleteMask = true) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertEntityHasComponent<T>(ComponentsTypeName, entity);
                AssertNotBlockedByQuery(ComponentsTypeName, entity, _blockerDelete);
                AssertNotBlockedByParallelQuery(ComponentsTypeName, entity);
                #endif
                
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var dataIdx = (ushort) (eid >> Const.DATA_SHIFT);
                var dataEIdx = (ushort) (eid & Const.DATA_ENTITY_MASK);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                var blockEntityInvMask = ~(1UL << blockEntityIdx);

                ref var chunk = ref chunks[chunkIdx];

                if (deleteMask) {
                    chunk.bitmask[(eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + idDiv] &= idMaskInv;
                }

                ref var entitiesMask = ref chunk.entities[blockIdx];

                if (entitiesMask == ulong.MaxValue) {
                    chunk.ClearFullBit(blockIdx);
                }

                entitiesMask &= blockEntityInvMask;
                chunk.disabledEntities[blockIdx] &= blockEntityInvMask;

                var components = data[dataIdx];

                if (entitiesMask == 0) {
                    var notEmptyBlocks = chunk.ClearNotEmptyBit(blockIdx);
                    if ((notEmptyBlocks & dataMaskCache[dataIdx & Const.DATA_BLOCK_MASK]) == 0UL) {
                        dataPool[Interlocked.Increment(ref dataPoolCount) - 1] = data[dataIdx];
                        data[dataIdx] = null;
                    }
                    if (notEmptyBlocks == 0) {
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
                    listener.OnComponentDelete(entity, ref components[dataEIdx]);
                }
                #endif

                if (onDeleteHandler != null) {
                    onDeleteHandler(entity, ref components[dataEIdx]);
                } else if (_clearable) {
                    components[dataEIdx] = default;
                }
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryDelete(Entity entity) {
                if (Has(entity)) {
                    Delete(entity);
                    return true;
                }

                return false;
            }

            [MethodImpl(AggressiveInlining)]
            public void Copy(Entity src, Entity dst) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, src);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, dst);
                #endif

                if (_copyable) {
                    if (onCopyHandler == null) {
                        TryAdd(dst) = Ref(src);
                    } else {
                        onCopyHandler(src, dst, ref Ref(src), ref TryAdd(dst));
                    }

                    if (HasDisabled(src)) {
                        Disable(dst);
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryCopy(Entity src, Entity dst) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, src);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, dst);
                #endif

                if (Has(src)) {
                    Copy(src, dst);
                    return true;
                }

                return false;
            }

            [MethodImpl(AggressiveInlining)]
            public void Move(Entity src, Entity dst) {
                Copy(src, dst);
                Delete(src);
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryMove(Entity src, Entity dst) {
                TryCopy(src, dst);
                return TryDelete(src);
            }

            [MethodImpl(AggressiveInlining)]
            public bool IsRegistered() {
                return _registered;
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

            [MethodImpl(AggressiveInlining)]
            public int CalculateCapacity() {
                var count = 0;
                for (var i = 0; i < data.Length; i++) {
                    if (data[i] != null) {
                        count++;
                    }
                }

                count += (int) dataPoolCount;

                return count * Const.DATA_BLOCK_SIZE;
            }
            #endregion

            #region INTERNAL
            [MethodImpl(NoInlining)]
            private void InitChunk(ref ComponentsChunk chunk, uint chunkIdx) {
                var taken = false;
                chunk.chunkLock.Enter(ref taken);
                #if FFS_ECS_DEBUG
                if (!taken) throw new StaticEcsException($"Failed to acquire components lock for chunk {chunkIdx}");
                #endif
                if (chunk.entities == null) {
                    var count = Interlocked.Decrement(ref chunksDataPoolCount);
                    if (count >= 0) {
                        chunk.InitFromPoolData(ref chunksDataPool[count]);
                    } else {
                        Interlocked.Increment(ref chunksDataPoolCount);
                        chunk.entities = new ulong[Const.BLOCK_IN_CHUNK];
                        chunk.disabledEntities = new ulong[Const.BLOCK_IN_CHUNK];
                    }
                    chunk.bitmask = _bitMask.chunks[chunkIdx];
                }
                chunk.chunkLock.Exit();
            }
            
            [MethodImpl(AggressiveInlining)]
            private void InitChunkSimple(ref ComponentsChunk chunk, uint chunkIdx) {
                if (chunksDataPoolCount > 0) {
                    chunk.InitFromPoolData(ref chunksDataPool[--chunksDataPoolCount]);
                } else {
                    chunk.entities = new ulong[Const.BLOCK_IN_CHUNK];
                    chunk.disabledEntities = new ulong[Const.BLOCK_IN_CHUNK];
                    chunk.bitmask = _bitMask.chunks[chunkIdx];
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void InitializeData(ref T[] components) {
                #if FFS_ECS_DEBUG
                if (components != null) throw new StaticEcsException("Invariant failed components != null");
                #endif
                components = dataPoolCount > 0 
                    ? dataPool[--dataPoolCount] 
                    : new T[Const.DATA_BLOCK_SIZE];
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

            [MethodImpl(AggressiveInlining)]
            internal ref T RefInternal(Entity entity) {
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                return ref data[eid >> Const.DATA_SHIFT][eid & Const.DATA_ENTITY_MASK];
            }

            [MethodImpl(AggressiveInlining)]
            internal void Create(ushort componentId, IComponentConfig<T, WorldType> config, FreeChunkCommandBuffer freeChunkCommandBuffer, BitMask bitMask) {
                SetDynamicId(componentId);
                _freeChunkCommandBuffer = freeChunkCommandBuffer;
                _bitMask = bitMask;
                dataMaskCache = Const.DataMasks;
                dataPoolCount = 0;
                onAddHandler = config.OnAdd();
                onPutHandler = config.OnPut();
                onDeleteHandler = config.OnDelete();
                onCopyHandler = config.OnCopy();
                _copyable = config.IsCopyable();
                _clearable = config.IsClearable();
                Serializer.Value.Create(config);
                chunksDataPoolCount = 0;
                _queriesToUpdateOnDeleteCount = 0;
                _queriesToUpdateOnAddCount = 0;
                _queriesToUpdateOnDisableCount = 0;
                _queriesToUpdateOnEnableCount = 0;
                _queriesToUpdateOnDelete = new QueryData[Const.MAX_NESTED_QUERY];
                _queriesToUpdateOnAdd = new QueryData[Const.MAX_NESTED_QUERY];
                _queriesToUpdateOnDisable = new QueryData[Const.MAX_NESTED_QUERY];
                _queriesToUpdateOnEnable = new QueryData[Const.MAX_NESTED_QUERY];
                _registered = true;
                #if FFS_ECS_DEBUG
                _blockerDelete = 0;
                _blockerAdd = 0;
                _blockerDisable = 0;
                _blockerEnable = 0;
                #endif
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Initialize(uint chunksCapacity) {
                chunksDataPool = new ComponentsChunkData[chunksCapacity];
                chunks = new ComponentsChunk[chunksCapacity];
                for (var i = 0; i < chunksCapacity; i++) {
                    chunks[i].chunkLock = new SpinLock(false);
                }
                data = new T[chunksCapacity * Const.DATA_BLOCKS_IN_CHUNK][];
                dataPool = new T[data.Length][];
                maskLen = _bitMask.maskLen;
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort DynamicId() {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
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
                AssertWorldIsCreatedOrInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                #endif
                
                return Serializer.Value.guid;
            }

            [MethodImpl(AggressiveInlining)]
            internal void Resize(uint chunksCapacity) {
                var dataCapacity = chunksCapacity * Const.DATA_BLOCKS_IN_CHUNK;
                var oldChunksCapacity = chunks.Length;
                Array.Resize(ref chunksDataPool, (int) chunksCapacity);
                Array.Resize(ref chunks, (int) chunksCapacity);
                for (var i = oldChunksCapacity; i < chunks.Length; i++) {
                    chunks[i].chunkLock = new SpinLock(false);
                }
                Array.Resize(ref data, (int) dataCapacity);
                Array.Resize(ref dataPool, (int) dataCapacity);
            }

            [MethodImpl(AggressiveInlining)]
            internal void ToStringComponent(StringBuilder builder, Entity entity) {
                builder.Append(" - [");
                builder.Append(id);
                builder.Append("] ");
                if (HasDisabled(entity)) {
                    builder.Append("[Disabled] ");
                }

                builder.Append(typeof(T).Name);
                builder.Append(" ( ");
                builder.Append(RefInternal(entity));
                builder.AppendLine(" )");
            }

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                _freeChunkCommandBuffer = null;
                onAddHandler = null;
                onPutHandler = null;
                onDeleteHandler = null;
                onCopyHandler = null;
                chunks = null;
                dataMaskCache = null;
                data = null;
                dataPool = null;
                chunksDataPool = null;
                chunksDataPoolCount = 0;
                dataPoolCount = 0;
                id = 0;
                maskLen = 0;
                idMask = 0;
                idMaskInv = 0;
                idDiv = 0;
                Serializer.Value.Destroy();
                _copyable = false;
                _clearable = false;
                _queriesToUpdateOnDeleteCount = 0;
                _queriesToUpdateOnAddCount = 0;
                _queriesToUpdateOnDisableCount = 0;
                _queriesToUpdateOnEnableCount = 0;
                _queriesToUpdateOnDelete = null;
                _queriesToUpdateOnAdd = null;
                _queriesToUpdateOnDisable = null;
                _queriesToUpdateOnEnable = null;
                _bitMask = null;
                #if FFS_ECS_EVENTS
                debugEventListeners = null;
                #endif
                #if FFS_ECS_DEBUG
                addWithoutValueError = null;
                _blockerDelete = 0;
                _blockerAdd = 0;
                _blockerDisable = 0;
                _blockerEnable = 0;
                #endif
                _registered = false;
            }

            [MethodImpl(AggressiveInlining)]
            internal void ClearChunk(uint chunkIdx) {
                ref var chunk = ref chunks[chunkIdx];

                if (chunk.notEmptyBlocks != 0) {
                    var dataIdxStart = (ushort) ((chunkIdx << Const.ENTITIES_IN_CHUNK_SHIFT) >> Const.DATA_SHIFT);
                    var dataIdxEnd = dataIdxStart + Const.DATA_BLOCKS_IN_CHUNK;

                    var entity = new Entity();
                    for (var i = dataIdxStart; i < dataIdxEnd; i++) {
                        ref var components = ref data[i];
                        if (components != null) {
                            if (onDeleteHandler != null) {
                                ref var notEmptyBlocks = ref chunk.notEmptyBlocks;
                                while (notEmptyBlocks > 0) {
                                    var blockIdx = Utils.PopLsb(ref notEmptyBlocks);
                                    ref var entitiesMask = ref chunk.entities[blockIdx];
                                    while (entitiesMask > 0) {
                                        var blockEntityIdx = Utils.PopLsb(ref entitiesMask);
                                        var eid = (uint) ((chunkIdx << Const.ENTITIES_IN_CHUNK_SHIFT) + (blockIdx << Const.ENTITIES_IN_BLOCK_SHIFT) + blockEntityIdx);
                                        entity.id = eid + Const.ENTITY_ID_OFFSET;
                                        var dataIdx = (ushort) (eid >> Const.DATA_SHIFT);
                                        var dataEIdx = (ushort) (eid & Const.DATA_ENTITY_MASK);
                                        onDeleteHandler(entity, ref data[dataIdx][dataEIdx]);
                                    }
                                }
                            } else if (_clearable) {
                                Array.Clear(components, 0, components.Length);
                            }
                            dataPool[dataPoolCount++] = components;
                            components = null;
                        }
                    }
                }


                if (chunk.entities != null) {
                    if (chunk.notEmptyBlocks != 0) {
                        Array.Clear(chunk.entities, 0, Const.BLOCK_IN_CHUNK);
                        Array.Clear(chunk.disabledEntities, 0, Const.BLOCK_IN_CHUNK);
                    }
                    chunksDataPool[chunksDataPoolCount++] = chunk.MoveToPoolData();
                }
                chunk.bitmask = null;
                chunk.notEmptyBlocks = 0;
                chunk.fullBlocks = 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal ref ComponentsChunk Chunk(uint chunkIdx) {
                return ref chunks[chunkIdx];
            }

            [MethodImpl(AggressiveInlining)]
            internal ulong EMask(uint chunkIdx, int blockIdx) {
                var entities = chunks[chunkIdx].entities;
                return entities != null ? entities[blockIdx] & ~chunks[chunkIdx].disabledEntities[blockIdx] : 0UL;
            }

            [MethodImpl(AggressiveInlining)]
            internal ulong DMask(uint chunkIdx, int blockIdx) {
                var disabledEntities = chunks[chunkIdx].disabledEntities;
                return disabledEntities != null ? disabledEntities[blockIdx] : 0UL;
            }

            [MethodImpl(AggressiveInlining)]
            internal ulong AMask(uint chunkIdx, int blockIdx) {
                var entities = chunks[chunkIdx].entities;
                return entities != null ? entities[blockIdx] : 0UL;
            }

            [MethodImpl(AggressiveInlining)]
            internal void IncQDelete(QueryData qData) {
                _queriesToUpdateOnDelete[_queriesToUpdateOnDeleteCount++] = qData;
            }

            internal void DecQDelete() {
                _queriesToUpdateOnDelete[--_queriesToUpdateOnDeleteCount] = default;
            }

            [MethodImpl(AggressiveInlining)]
            internal void IncQDeleteDisable(QueryData qData) {
                _queriesToUpdateOnDelete[_queriesToUpdateOnDeleteCount++] = qData;
                _queriesToUpdateOnDisable[_queriesToUpdateOnDisableCount++] = qData;
            }

            [MethodImpl(AggressiveInlining)]
            internal void DecQDeleteDisable() {
                _queriesToUpdateOnDelete[--_queriesToUpdateOnDeleteCount] = default;
                _queriesToUpdateOnDisable[--_queriesToUpdateOnDisableCount] = default;
            }

            [MethodImpl(AggressiveInlining)]
            internal void IncQDeleteEnable(QueryData qData) {
                _queriesToUpdateOnDelete[_queriesToUpdateOnDeleteCount++] = qData;
                _queriesToUpdateOnEnable[_queriesToUpdateOnEnableCount++] = qData;
            }

            [MethodImpl(AggressiveInlining)]
            internal void DecQDeleteEnable() {
                _queriesToUpdateOnDelete[--_queriesToUpdateOnDeleteCount] = default;
                _queriesToUpdateOnEnable[--_queriesToUpdateOnEnableCount] = default;
            }

            [MethodImpl(AggressiveInlining)]
            internal void IncQAdd(QueryData qData) {
                _queriesToUpdateOnAdd[_queriesToUpdateOnAddCount++] = qData;
            }

            [MethodImpl(AggressiveInlining)]
            internal void DecQAdd() {
                _queriesToUpdateOnAdd[--_queriesToUpdateOnAddCount] = default;
            }

            [MethodImpl(AggressiveInlining)]
            internal void IncQAddEnable(QueryData qData) {
                _queriesToUpdateOnAdd[_queriesToUpdateOnAddCount++] = qData;
                _queriesToUpdateOnEnable[_queriesToUpdateOnEnableCount++] = qData;
            }

            [MethodImpl(AggressiveInlining)]
            internal void DecQAddEnable() {
                _queriesToUpdateOnAdd[--_queriesToUpdateOnAddCount] = default;
                _queriesToUpdateOnEnable[--_queriesToUpdateOnEnableCount] = default;
            }

            #if FFS_ECS_DEBUG
            [MethodImpl(AggressiveInlining)]
            internal void BlockDelete(int val) {
                _blockerDelete += val;
            }

            [MethodImpl(AggressiveInlining)]
            internal void BlockDeleteDisable(int val) {
                _blockerDelete += val;
                _blockerDisable += val;
            }

            [MethodImpl(AggressiveInlining)]
            internal void BlockDeleteEnable(int val) {
                _blockerDelete += val;
                _blockerEnable += val;
            }

            [MethodImpl(AggressiveInlining)]
            internal void BlockAdd(int val) {
                _blockerAdd += val;
            }

            [MethodImpl(AggressiveInlining)]
            internal void BlockAddEnable(int val) {
                _blockerAdd += val;
                _blockerEnable += val;
            }
            #endif
            #endregion
        }

        public delegate void OnComponentHandler<T>(Entity entity, ref T component) where T : struct;

        public delegate void OnCopyHandler<T>(Entity srcEntity, Entity dstEntity, ref T src, ref T dst) where T : struct;
    }
}