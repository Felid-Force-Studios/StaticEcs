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
        public partial struct Components<T> where T : struct, IComponent {
            public static Components<T> Value;
            
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<IComponentsDebugEventListener> debugEventListeners;
            #endif
            
            internal string addWithoutValueError;
            internal OnComponentHandler<T> onAddHandler;
            internal OnComponentHandler<T> onPutHandler;
            internal OnComponentHandler<T> onDeleteHandler;
            internal OnCopyHandler<T> onCopyHandler;
            internal ulong[] dataMaskCache;
            internal T[][] data;
            internal T[][] dataPool;
            internal ComponentsChunk[] chunks;
            private QueryData[] _queriesToUpdateOnDelete;
            private QueryData[] _queriesToUpdateOnAdd;
            private QueryData[] _queriesToUpdateOnDisable;
            private QueryData[] _queriesToUpdateOnEnable;
            
            internal long dataPoolCount;
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

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            private int _blockerDelete;
            private int _blockerAdd;
            private int _blockerDisable;
            private int _blockerEnable;
            #endif
            
            private bool _registered;

            #region PUBLIC
            [MethodImpl(AggressiveInlining)]
            public ref T Ref(Entity entity) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Ref, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, cannot access Entity ID - {id} from deleted entity");
                if (!Has(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, ID - {entity._id} is missing on an entity");
                #endif
                var eid = entity._id;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
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
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Add, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, cannot access Entity ID - {id} from deleted entity");
                if (Has(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, ID - {entity._id} is already on an entity");
                if (addWithoutValueError != null) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, {addWithoutValueError}");
                if (_blockerAdd > 0 && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, pool is blocked, use QueryMode.Flexible");
                if (MultiThreadActive && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, pool is blocked, it is forbidden to modify a non-current entity in a parallel query");
                #endif

                var eid = entity._id;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var dataIdx = (ushort) (eid >> Const.DATA_SHIFT);
                var dataEIdx = (ushort) (eid & Const.DATA_ENTITY_MASK);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                
                ref var chunk = ref chunks[chunkIdx];
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
                
                onAddHandler?.Invoke(entity, ref data[dataIdx][dataEIdx]);
                chunk.bitmask[(eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + idDiv] |= idMask;
                
                for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                    _queriesToUpdateOnAdd[i].Update(~(1UL << blockEntityIdx), eid);
                }

                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentAdd(entity, ref data[dataIdx][dataEIdx]);
                }
                #endif
                
                
                return ref data[dataIdx][dataEIdx];
            }

            [MethodImpl(AggressiveInlining)]
            public ref T Add(Entity entity, T component) {
             #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Add, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, cannot access Entity ID - {id} from deleted entity");
                if (Has(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, ID - {entity._id} is already on an entity");
                if (_blockerAdd > 0 && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, pool is blocked, use QueryMode.Flexible");
                if (MultiThreadActive && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, pool is blocked, it is forbidden to modify a non-current entity in a parallel query");
                #endif

                var eid = entity._id;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var dataIdx = (ushort) (eid >> Const.DATA_SHIFT);
                var dataEIdx = (ushort) (eid & Const.DATA_ENTITY_MASK);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                
                ref var chunk = ref chunks[chunkIdx];
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
                onAddHandler?.Invoke(entity, ref val);
                chunk.bitmask[(eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + idDiv] |= idMask;
                
                for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                    _queriesToUpdateOnAdd[i].Update(~(1UL << blockEntityIdx), eid);
                }

                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentAdd(entity, ref val);
                }
                #endif
                
                return ref val;
            }

            [MethodImpl(AggressiveInlining)]
            internal void AddInternal(Entity entity, T component) {
             #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Add, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, cannot access Entity ID - {id} from deleted entity");
                if (Has(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, ID - {entity._id} is already on an entity");
                if (_blockerAdd > 0 && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, pool is blocked, use QueryMode.Flexible");
                if (MultiThreadActive && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, pool is blocked, it is forbidden to modify a non-current entity in a parallel query");
                #endif

                var eid = entity._id;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var dataIdx = (ushort) (eid >> Const.DATA_SHIFT);
                var dataEIdx = (ushort) (eid & Const.DATA_ENTITY_MASK);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                
                ref var chunk = ref chunks[chunkIdx];
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

                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentAdd(entity, ref val);
                }
                #endif
                
            }

            [MethodImpl(AggressiveInlining)]
            public void Put(Entity entity, T component) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Put, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, cannot access ID - {id} from deleted entity");
                if (_blockerAdd > 0 && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, pool is blocked, use QueryMode.Flexible");
                if (MultiThreadActive && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, pool is blocked, it is forbidden to modify a non-current entity in a parallel query");
                #endif
                
                var eid = entity._id;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var dataIdx = (ushort) (eid >> Const.DATA_SHIFT);
                var dataEIdx = (ushort) (eid & Const.DATA_ENTITY_MASK);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                
                ref var chunk = ref chunks[chunkIdx];
                ref var entitiesMask = ref chunk.entities[blockIdx];
                
                if ((entitiesMask & (1UL << blockEntityIdx)) != 0) {
                    ref var val = ref data[dataIdx][dataEIdx];
                    val = component;
                    onPutHandler?.Invoke(entity, ref val);
                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
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
                onPutHandler?.Invoke(entity, ref newVal);
                chunk.bitmask[(eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen + idDiv] |= idMask;
                
                for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                    _queriesToUpdateOnAdd[i].Update(~(1UL << blockEntityIdx), eid);
                }
                
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
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
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Has, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Has, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Has, cannot access ID - {id} from deleted entity");
                #endif
                var eid = entity._id;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                return (chunks[chunkIdx].entities[blockIdx] & (1UL << blockEntityIdx)) != 0;
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasDisabled(Entity entity) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: HasDisabled, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: HasDisabled, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: HasDisabled, cannot access ID - {id} from deleted entity");
                #endif
                var eid = entity._id;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                return (chunks[chunkIdx].disabledEntities[blockIdx] & (1UL << blockEntityIdx)) != 0;
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasEnabled(Entity entity) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: HasEnabled, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: HasEnabled, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: HasEnabled, cannot access ID - {id} from deleted entity");
                #endif
                var eid = entity._id;
                var chunkIdx = (ushort) (eid >> Const.ENTITIES_IN_CHUNK_SHIFT);
                var blockIdx = (byte) ((eid >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCK_IN_CHUNK_OFFSET_MASK);
                var blockEntityIdx = (byte) (eid & Const.ENTITIES_IN_BLOCK_OFFSET_MASK);
                return (chunks[chunkIdx].entities[blockIdx] & ~chunks[chunkIdx].disabledEntities[blockIdx] & (1UL << blockEntityIdx)) != 0;
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Disable(Entity entity) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Disable, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Disable, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Disable, cannot access ID - {id} from deleted entity");
                if (!Has(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Disable, ID - {entity._id} is missing on an entity");
                if (_blockerDisable > 0 && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Disable, pool is blocked, use QueryMode.Flexible");
                if (MultiThreadActive && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Disable, pool is blocked, it is forbidden to modify a non-current entity in a parallel query");
                #endif
                var eid = entity._id;
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
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Enable, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Enable, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Enable, cannot access ID - {id} from deleted entity");
                if (!Has(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Enable, ID - {entity._id} is missing on an entity");
                if (_blockerEnable > 0 && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Enable, pool is blocked, use QueryMode.Flexible");
                if (MultiThreadActive && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Enable, pool is blocked, it is forbidden to modify a non-current entity in a parallel query");
                #endif
                var eid = entity._id;
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
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Delete, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete, cannot access ID - {id} from deleted entity");
                if (!Has(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete, cannot access ID - {id} component not added");
                if (_blockerDelete > 0 && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete, pool is blocked, use QueryMode.Flexible");
                if (MultiThreadActive && CurrentQuery.IsNotCurrentEntity(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete, pool is blocked, it is forbidden to modify a non-current entity in a parallel query");
                #endif
                var eid = entity._id;
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

                if (onDeleteHandler != null) {
                    onDeleteHandler(entity, ref data[dataIdx][dataEIdx]);
                } else if (_clearable) {
                    data[dataIdx][dataEIdx] = default;
                }
                
                if (entitiesMask == 0) {
                    if ((chunk.ClearNotEmptyBit(blockIdx) & dataMaskCache[dataIdx & Const.DATA_BLOCK_MASK]) == 0UL) {
                        dataPool[Interlocked.Increment(ref dataPoolCount) - 1] = data[dataIdx];
                        data[dataIdx] = null;
                    }
                }
                
                for (uint i = 0; i < _queriesToUpdateOnDeleteCount; i++) {
                    _queriesToUpdateOnDelete[i].Update(blockEntityInvMask, eid);
                }
 
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentDelete(entity, ref data[dataIdx][dataEIdx]);
                }
                #endif
                
                #if FFS_ECS_LIFECYCLE_ENTITY
                if (_bitMask.IsEmpty(entity._id)) {
                    World.DestroyEntity(entity);
                }
                #endif
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
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Copy, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, Component type not registered");
                if (!src.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
                if (!dst.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
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
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: TryCopy, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: TryCopy, Component type not registered");
                if (!src.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: TryCopy, cannot access ID - {id} from deleted entity");
                if (!dst.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: TryCopy, cannot access ID - {id} from deleted entity");
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
            [MethodImpl(AggressiveInlining)]
            internal void UpdateBitMask(BitMask bitMask) {
                for (var i = 0; i < bitMask.chunks.Length; i++) {
                    chunks[i].bitmask = bitMask.chunks[i];
                }

                maskLen = bitMask.maskLen;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal ref T RefInternal(Entity entity) {
                var eid = entity._id;
                return ref data[eid >> Const.DATA_SHIFT][eid & Const.DATA_ENTITY_MASK];
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Create(ushort componentId, uint entitiesCapacity, IComponentConfig<T, WorldType> config) {
                SetDynamicId(componentId);
                var chunkCount = entitiesCapacity >> Const.ENTITIES_IN_CHUNK_SHIFT;
                chunks = new ComponentsChunk[chunkCount];
                for (var i = 0; i < chunkCount; i++) {
                    chunks[i].entities = new ulong[Const.BLOCK_IN_CHUNK];
                    chunks[i].disabledEntities = new ulong[Const.BLOCK_IN_CHUNK];
                }
                data = new T[entitiesCapacity >> Const.DATA_SHIFT][];
                dataPool =  new T[data.Length][];
                dataMaskCache = Const.DataMasks;
                dataPoolCount = 0;
                onAddHandler = config.OnAdd();
                onPutHandler = config.OnPut();
                onDeleteHandler = config.OnDelete();
                onCopyHandler = config.OnCopy();
                _copyable = config.IsCopyable();
                _clearable = config.IsClearable();
                Serializer.Value.Create(config);
                _queriesToUpdateOnDeleteCount = 0;
                _queriesToUpdateOnAddCount = 0;
                _queriesToUpdateOnDisableCount = 0;
                _queriesToUpdateOnEnableCount = 0;
                _queriesToUpdateOnDelete = new QueryData[Const.MAX_NESTED_QUERY];
                _queriesToUpdateOnAdd = new QueryData[Const.MAX_NESTED_QUERY];
                _queriesToUpdateOnDisable = new QueryData[Const.MAX_NESTED_QUERY];
                _queriesToUpdateOnEnable = new QueryData[Const.MAX_NESTED_QUERY];
                _registered = true;
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                _blockerDelete = 0;
                _blockerAdd = 0;
                _blockerDisable = 0;
                _blockerEnable = 0;
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort DynamicId() {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (Status < WorldStatus.Created) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: DynamicId, World not created");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: DynamicId, Component type not registered");
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
                if (Status < WorldStatus.Created) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: DynamicId, World not created");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: DynamicId, Component type not registered");
                #endif
                return Serializer.Value.guid;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Resize(uint entitiesCapacity) {
                var chunkCount = entitiesCapacity >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var dataCount = entitiesCapacity >> Const.DATA_SHIFT;
                
                if (chunks.Length < chunkCount) {
                    var old = chunks.Length;
                    Array.Resize(ref chunks, (int) chunkCount);
                    for (var i = old; i < chunkCount; i++) {
                        chunks[i].entities = new ulong[Const.BLOCK_IN_CHUNK];
                        chunks[i].disabledEntities = new ulong[Const.BLOCK_IN_CHUNK];
                    }
                }
                Array.Resize(ref data, (int) dataCount);
                Array.Resize(ref dataPool, (int) dataCount);
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
                onAddHandler = null;
                onPutHandler = null;
                onDeleteHandler = null;
                onCopyHandler = null;
                chunks = null;
                dataMaskCache = null;
                data = null;
                dataPool = null;
                dataPoolCount = 0;
                id = 0;
                maskLen = 0;
                idMask = 0;
                idMaskInv = 0;
                idDiv = 0;
                Serializer.Value.Destroy();
                addWithoutValueError = null;
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
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                debugEventListeners = null;
                _blockerDelete = 0;
                _blockerAdd = 0;
                _blockerDisable = 0;
                _blockerEnable = 0;
                #endif
                _registered = false;
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                for (var i = 0; i < data.Length; i++) {
                    var components = data[i];
                    if (components != null) {
                        Array.Clear(components, 0, components.Length);
                    }
                }
                
                for (var i = 0; i < dataPoolCount; i++) {
                    Array.Clear(dataPool[i], 0, dataPool[i].Length);
                }
                for (var i = 0; i < chunks.Length; i++) {
                    ref var chunk = ref chunks[i];
                    Array.Clear(chunk.entities,  0, chunk.entities.Length);
                    Array.Clear(chunk.disabledEntities,  0, chunk.disabledEntities.Length);
                    chunk.notEmptyBlocks = 0;
                    chunk.fullBlocks = 0;
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal ref ComponentsChunk Chunk(uint chunkIdx) {
                return ref chunks[chunkIdx];
            }
            
            [MethodImpl(AggressiveInlining)]
            internal ulong EMask(uint chunkIdx, int blockIdx) {
                return chunks[chunkIdx].entities[blockIdx] & ~chunks[chunkIdx].disabledEntities[blockIdx];
            }
            
            [MethodImpl(AggressiveInlining)]
            internal ulong DMask(uint chunkIdx, int blockIdx) {
                return chunks[chunkIdx].disabledEntities[blockIdx];
            }
            
            [MethodImpl(AggressiveInlining)]
            internal ulong AMask(uint chunkIdx, int blockIdx) {
                return chunks[chunkIdx].entities[blockIdx];
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


            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
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