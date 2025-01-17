using System;
using System.Collections.Generic;
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
    public abstract partial class Ecs<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        public struct Components<T> where T : struct, IComponent {
            public static Components<T> Value;
            internal const int Empty = -1;
            
            private static AutoInitHandler<T> AutoInitHandler;
            private static AutoResetHandler<T> AutoResetHandler;
            private static AutoCopyHandler<T> AutoCopyHandler;
            
            private int[] _entities;
            private T[] _data;
            private int[] _dataIdxByEntityId;
            private BitMask _bitMask;
            private int _componentsCount;
            internal ushort id;

            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            private int _blockers;
            #endif

            #region PUBLIC
            [MethodImpl(AggressiveInlining)]
            public ref T RefMut(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: RefMut, World not initialized");
                if (!ModuleComponents.ComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: RefMut, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: RefMut, cannot access Entity ID - {id} from deleted entity");
                if (!Has(entity)) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: RefMut, ID - {entity._id} is missing on an entity");
                #endif
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                ref var val = ref _data[_dataIdxByEntityId[entity._id]];
                if (ModuleComponents.Value._debugEventListeners != null) {
                    foreach (var listener in ModuleComponents.Value._debugEventListeners) {
                        listener.OnComponentRefMut(entity, ref val);
                    }
                }
                return ref val;
                #else
                return ref _data[_dataIdxByEntityId[entity._id]];
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public ref readonly T Ref(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Ref, World not initialized");
                if (!ModuleComponents.ComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, cannot access Entity ID - {id} from deleted entity");
                if (!Has(entity)) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, ID - {entity._id} is missing on an entity");
                #endif
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                ref var val = ref _data[_dataIdxByEntityId[entity._id]];
                if (ModuleComponents.Value._debugEventListeners != null) {
                    foreach (var listener in ModuleComponents.Value._debugEventListeners) {
                        listener.OnComponentRef(entity, ref val);
                    }
                }
                return ref val;
                #else
                return ref _data[_dataIdxByEntityId[entity._id]];
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public ref T Add(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Add, World not initialized");
                if (!ModuleComponents.ComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, cannot access Entity ID - {id} from deleted entity");
                if (Has(entity)) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, ID - {entity._id} is already on an entity");
                if (IsBlocked()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, component pool cannot be changed, it is in read-only mode due to multiple accesses");
                #endif

                if (_entities.Length == _componentsCount) {
                    ResizeData(_componentsCount << 1);
                }

                var eid = entity._id;
                _entities[_componentsCount] = eid;
                _dataIdxByEntityId[eid] = _componentsCount;

                _bitMask.Set(eid, id);
                
                ref var data = ref _data[_componentsCount];
                AutoInitHandler?.Invoke(ref data);

                _componentsCount++;
                
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (ModuleComponents.Value._debugEventListeners != null) {
                    foreach (var listener in ModuleComponents.Value._debugEventListeners) {
                        listener.OnComponentAdd(entity, ref data);
                    }
                }
                #endif
                
                return ref data;
            }

            [MethodImpl(AggressiveInlining)]
            public void Put(Entity entity, T component) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Put, World not initialized");
                if (!ModuleComponents.ComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, cannot access ID - {id} from deleted entity");
                if (IsBlocked()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, component pool cannot be changed, it is in read-only mode due to multiple accesses");
                #endif
                var eid = entity._id;
                ref var dataId = ref _dataIdxByEntityId[eid];
                if (dataId >= 0) {
                    _data[dataId] = component;
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (ModuleComponents.Value._debugEventListeners != null) {
                        foreach (var listener in ModuleComponents.Value._debugEventListeners) {
                            listener.OnComponentPut(entity, ref _data[dataId]);
                        }
                    }
                    #endif
                    return;
                }

                if (_entities.Length == _componentsCount) {
                    ResizeData(_componentsCount << 1);
                }

                _entities[_componentsCount] = eid;
                dataId = _componentsCount;

                _bitMask.Set(eid, id);

                _data[_componentsCount] = component;
                _componentsCount++;
                
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (ModuleComponents.Value._debugEventListeners != null) {
                    foreach (var listener in ModuleComponents.Value._debugEventListeners) {
                        listener.OnComponentPut(entity, ref _data[dataId]);
                    }
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public ref T TryAdd(Entity entity) {
                return ref Has(entity)
                    ? ref RefMut(entity)
                    : ref Add(entity);
            }

            [MethodImpl(AggressiveInlining)]
            public ref T TryAdd(Entity entity, out bool added) {
                added = !Has(entity);
                return ref added
                    ? ref Add(entity)
                    : ref RefMut(entity);
            }

            [MethodImpl(AggressiveInlining)]
            public bool Has(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Has, World not initialized");
                if (!ModuleComponents.ComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Has, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Has, cannot access ID - {id} from deleted entity");
                #endif
                return _dataIdxByEntityId[entity._id] >= 0;
            }

            [MethodImpl(AggressiveInlining)]
            public bool Delete(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Delete, World not initialized");
                if (!ModuleComponents.ComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete, cannot access ID - {id} from deleted entity");
                if (IsBlocked()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete,  component pool cannot be changed, it is in read-only mode due to multiple accesses");
                #endif
                return DelInternal(entity, false);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Copy(Entity src, Entity dst) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Copy, World not initialized");
                if (!ModuleComponents.ComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, Component type not registered");
                if (!src.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
                if (!dst.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
                if (IsBlocked()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy,  component pool cannot be changed, it is in read-only mode due to multiple accesses");
                #endif

                if (Has(src)) {
                    if (AutoCopyHandler != null) {
                        if (!Has(dst)) {
                            AutoCopyHandler(ref RefMut(src), ref Add(dst));
                        } else {
                            AutoCopyHandler(ref RefMut(src), ref RefMut(dst));
                        }
                        return;
                    }
                    
                    if (!Has(dst)) {
                        Add(dst) = RefMut(src);
                    } else {
                        RefMut(dst) = RefMut(src);
                    }
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Move(Entity src, Entity dst) {
                Copy(src, dst);
                Delete(src);
            }

            [MethodImpl(AggressiveInlining)]
            public int Count() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Count, World not initialized");
                if (!ModuleComponents.ComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Count, Component type not registered");
                #endif
                return _componentsCount;
            }
            
            [MethodImpl(AggressiveInlining)]
            public T[] Data() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Data, World not initialized");
                if (!ModuleComponents.ComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Data, Component type not registered");
                #endif
                return _data;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void SetAutoInit(AutoInitHandler<T> handler) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (handler == null) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoInit, handler is null");
                if (!ModuleComponents.ComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: SetAutoInit, Component type not registered");
                if (World.Status != WorldStatus.Created) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoInit, world status not `Created`");
                #endif
                AutoInitHandler = handler;
            }

            [MethodImpl(AggressiveInlining)]
            public static void SetAutoReset(AutoResetHandler<T> handler) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (handler == null) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoReset, handler is null");
                if (!ModuleComponents.ComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: SetAutoReset, Component type not registered");
                if (World.Status != WorldStatus.Created) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoReset, world status not `Created`");
                #endif
                AutoResetHandler = handler;
            }

            [MethodImpl(AggressiveInlining)]
            public static void SetAutoCopy(AutoCopyHandler<T> handler) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (handler == null) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoCopy, handler is null");
                if (!ModuleComponents.ComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: SetAutoCopy, Component type not registered");
                if (World.Status != WorldStatus.Created) throw new Exception($"Components.AutoHandlers<{typeof(WorldType)}, {typeof(T)}>, Method: SetAutoCopy, world status not `Created`");
                #endif
                AutoCopyHandler = handler;
            }
            #endregion

            #region INTERNAL
            
            [MethodImpl(AggressiveInlining)]
            internal ref T RefMutInternal(Entity entity) {
                return ref _data[_dataIdxByEntityId[entity._id]];
            }
            
            internal void Create(ushort componentId, BitMask bitMask, uint baseCapacity = 128) {
                _bitMask = bitMask;
                id = componentId;
                _entities = new int[baseCapacity];
                _data = new T[baseCapacity];
                _componentsCount = 0;
                _dataIdxByEntityId = new int[World.EntitiesCapacity()];
                for (var i = 0; i < _dataIdxByEntityId.Length; i++) {
                    _dataIdxByEntityId[i] = Empty;
                }
                ModuleComponents.ComponentInfo<T>.Register();
            }

            [MethodImpl(AggressiveInlining)]
            internal ComponentDynId DynamicId() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (World.Status < WorldStatus.Created) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: DynamicId, World not created");
                if (!ModuleComponents.ComponentInfo<T>.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: DynamicId, Component type not registered");
                #endif
                return new ComponentDynId(id);
            }

            [MethodImpl(AggressiveInlining)]
            internal bool DeleteFromWorld(Entity entity) {
                return DelInternal(entity, true);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal int[] GetDataIdxByEntityId() {
                return _dataIdxByEntityId;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Resize(int cap) {
                var lastLength = _dataIdxByEntityId.Length;
                Array.Resize(ref _dataIdxByEntityId, cap);
                for (var i = lastLength; i < cap; i++) {
                    _dataIdxByEntityId[i] = Empty;
                }
            }

            [MethodImpl(AggressiveInlining)]
            private bool DelInternal(Entity entity, bool fromWorld) {
                ref var idxRef = ref _dataIdxByEntityId[entity._id];
                if (idxRef >= 0) {
                    _componentsCount--;
                    
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (ModuleComponents.Value._debugEventListeners != null) {
                        foreach (var listener in ModuleComponents.Value._debugEventListeners) {
                            listener.OnComponentDelete(entity, ref _data[idxRef]);
                        }
                    }
                    #endif
                    ResetComponent(idxRef);

                    if (idxRef != _componentsCount) {
                        var lastEntity = _entities[_componentsCount];
                        _entities[idxRef] = lastEntity;
                        _dataIdxByEntityId[lastEntity] = idxRef;
                        (_data[idxRef], _data[_componentsCount]) = (_data[_componentsCount], _data[idxRef]);
                    }

                    idxRef = Empty;
                    _bitMask.Del(entity._id, id);
                    if (!fromWorld && _bitMask.IsEmpty(entity._id)) {
                        World.DestroyEntity(entity);
                    }
                    return true;
                }

                return false;
            }

            private void ResetComponent(int idx) {
                if (AutoResetHandler != null) {
                    AutoResetHandler(ref _data[idx]);
                } else {
                    _data[idx] = default;
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void SetDataIfCountLess(ref int count, ref int[] entities) {
                if (_componentsCount < count) {
                    count = _componentsCount;
                    entities = _entities;
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void SetDataIfCountMore(ref int count, ref int[] entities) {
                if (_componentsCount > count) {
                    count = _componentsCount;
                    entities = _entities;
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal int[] EntitiesData() {
                return _entities;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal string ToStringComponent(Entity entity) {
                return $" - [{id}] {typeof(T).Name} ( {_data[_dataIdxByEntityId[entity._id]]} )\n";
            }

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                AutoInitHandler = null;
                AutoResetHandler = null;
                AutoCopyHandler = null;
                _entities = null;
                _data = null;
                _dataIdxByEntityId = null;
                _bitMask = null;
                _componentsCount = 0;
                id = 0;
                ModuleComponents.ComponentInfo<T>.Reset();
            }

            [MethodImpl(AggressiveInlining)]
            internal void EnsureSize(int size) {
                if (_componentsCount + size >= _entities.Length) {
                    ResizeData(Utils.CalculateSize(_componentsCount + size));
                }
            }

            [MethodImpl(NoInlining)]
            private void ResizeData(int newSize) {
                Array.Resize(ref _entities, newSize);
                Array.Resize(ref _data, newSize);
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                Array.Clear(_entities, 0, _entities.Length);
                Array.Clear(_data, 0, _data.Length);
                for (int i = 0; i < _dataIdxByEntityId.Length; i++) {
                    _dataIdxByEntityId[i] = Empty;
                }
                _componentsCount = 0;
            }
            
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            [MethodImpl(AggressiveInlining)]
            internal void AddBlocker(int amount) {
                _blockers += amount;
                if (_blockers < 0) {
                    throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: AddBlocker, incorrect pool user balance when attempting to release");
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal bool IsBlocked() {
                return _blockers > 1;
            }
            #endif
            #endregion
        }
    }

    public delegate void AutoInitHandler<T>(ref T component) where T : struct;

    public delegate void AutoResetHandler<T>(ref T component) where T : struct;

    public delegate void AutoCopyHandler<T>(ref T src, ref T dst) where T : struct;
}