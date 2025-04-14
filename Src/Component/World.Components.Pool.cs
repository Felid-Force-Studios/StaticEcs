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
    public abstract partial class World<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        public struct Components<T> where T : struct, IComponent {
            public static Components<T> Value;
            
            private OnAddHandler<T> _onAddHandler;
            private OnAddHandler<T> _onPutHandler;
            private OnDeleteHandler<T> _onDeleteHandler;
            private OnCopyHandler<T> _onCopyHandler;
            
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<IComponentsDebugEventListener> debugEventListeners;
            #endif
            
            private uint[] _entities;
            private T[] _data;
            private uint[] _dataIdxByEntityId;
            private BitMask _bitMask;
            private uint _componentsCount;
            internal ushort id;
            private bool _registered;

            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            private int _blockers;
            #endif

            #region PUBLIC
            [MethodImpl(AggressiveInlining)]
            public ref T Ref(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Ref, World not initialized");
                if (!_registered) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, cannot access Entity ID - {id} from deleted entity");
                if (!Has(entity)) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, ID - {entity._id} is missing on an entity");
                #endif
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                ref var val = ref _data[_dataIdxByEntityId[entity._id] & Const.DisabledComponentMaskInv];
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentRef(entity, ref val);
                }
                return ref val;
                #else
                return ref _data[_dataIdxByEntityId[entity._id] & Const.DisabledComponentMaskInv];
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public ref T Add(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Add, World not initialized");
                if (!_registered) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, cannot access Entity ID - {id} from deleted entity");
                if (Has(entity)) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, ID - {entity._id} is already on an entity");
                if (IsBlocked()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, component pool cannot be changed, it is in read-only mode due to multiple accesses");
                if (MultiThreadActive) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, this operation is not supported in multithreaded mode");
                #endif

                if (_entities.Length == _componentsCount) {
                    ResizeData(_componentsCount << 1);
                }

                var eid = entity._id;
                _entities[_componentsCount] = eid;
                _dataIdxByEntityId[eid] = _componentsCount;

                _bitMask.Set(eid, id);
                
                ref var data = ref _data[_componentsCount];
                _onAddHandler?.Invoke(ref data);

                _componentsCount++;
                
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentAdd(entity, ref data);
                }
                #endif
                
                return ref data;
            }

            [MethodImpl(AggressiveInlining)]
            public void AddDefault(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Add, World not initialized");
                if (!_registered) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, cannot access Entity ID - {id} from deleted entity");
                if (Has(entity)) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, ID - {entity._id} is already on an entity");
                if (IsBlocked()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, component pool cannot be changed, it is in read-only mode due to multiple accesses");
                if (MultiThreadActive) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, this operation is not supported in multithreaded mode");
                #endif

                if (_entities.Length == _componentsCount) {
                    ResizeData(_componentsCount << 1);
                }

                var eid = entity._id;
                _entities[_componentsCount] = eid;
                _dataIdxByEntityId[eid] = _componentsCount;

                _bitMask.Set(eid, id);
                
                _onAddHandler?.Invoke(ref _data[_componentsCount]);

                _componentsCount++;
                
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentAdd(entity, ref _data[_componentsCount]);
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public void Put(Entity entity, T component) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Put, World not initialized");
                if (!_registered) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, cannot access ID - {id} from deleted entity");
                if (IsBlocked()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, component pool cannot be changed, it is in read-only mode due to multiple accesses");
                if (MultiThreadActive) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, this operation is not supported in multithreaded mode");
                #endif
                var eid = entity._id;
                ref var dataId = ref _dataIdxByEntityId[eid];
                if (dataId != Const.EmptyComponentMask) {
                    _data[dataId & Const.DisabledComponentMaskInv] = component;
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                    foreach (var listener in debugEventListeners) {
                        listener.OnComponentPut(entity, ref _data[dataId & Const.DisabledComponentMaskInv]);
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

                _onPutHandler?.Invoke(ref component);
                _data[_componentsCount] = component;
                _componentsCount++;
                
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentPut(entity, ref _data[dataId]);
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
            public void TryAddDefault(Entity entity) {
                if (!Has(entity)) {
                    AddDefault(entity);
                }
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
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Has, World not initialized");
                if (!_registered) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Has, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Has, cannot access ID - {id} from deleted entity");
                #endif
                return _dataIdxByEntityId[entity._id] != Const.EmptyComponentMask;
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasDisabled(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: HasDisabled, World not initialized");
                if (!_registered) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: HasDisabled, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: HasDisabled, cannot access ID - {id} from deleted entity");
                #endif
                return (_dataIdxByEntityId[entity._id] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask;
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasEnabled(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: HasEnabled, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: HasEnabled, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: HasEnabled, cannot access ID - {id} from deleted entity");
                #endif
                return (_dataIdxByEntityId[entity._id] & Const.EmptyAndDisabledComponentMask) == 0;
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Disable(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Disable, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Disable, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Disable, cannot access ID - {id} from deleted entity");
                if (!Has(entity)) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Disable, ID - {entity._id} is missing on an entity");
                if (MultiThreadActive) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Disable: Disable, this operation is not supported in multithreaded mode");
                #endif
                _dataIdxByEntityId[entity._id] |= Const.DisabledComponentMask;
                _bitMask.MoveToDisabled(entity._id, id);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Enable(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Enable, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Enable, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Enable, cannot access ID - {id} from deleted entity");
                if (!Has(entity)) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Enable, ID - {entity._id} is missing on an entity");
                if (MultiThreadActive) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Enable, this operation is not supported in multithreaded mode");
                #endif
                _dataIdxByEntityId[entity._id] &= Const.DisabledComponentMaskInv;
                _bitMask.MoveToEnabled(entity._id, id);
            }

            [MethodImpl(AggressiveInlining)]
            public void Delete(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Delete, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete, Component type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete, cannot access ID - {id} from deleted entity");
                if (!Has(entity)) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete, cannot access ID - {id} component not added");
                if (IsBlocked()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete,  component pool cannot be changed, it is in read-only mode due to multiple accesses");
                if (MultiThreadActive) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete, this operation is not supported in multithreaded mode");
                #endif
                DeleteInternalWithoutMask(entity);
                _bitMask.DelWithDisabled(entity._id, id);
                #if FFS_ECS_LIFECYCLE_ENTITY
                if (_bitMask.IsEmptyWithDisabled(entity._id)) {
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
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Copy, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, Component type not registered");
                if (!src.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
                if (!dst.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
                if (IsBlocked()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy,  component pool cannot be changed, it is in read-only mode due to multiple accesses");
                if (MultiThreadActive) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, this operation is not supported in multithreaded mode");
                #endif

                if (_onCopyHandler == null) {
                    TryAdd(dst) = Ref(src);
                } else {
                    _onCopyHandler(ref Ref(src), ref TryAdd(dst));
                }

                if (HasDisabled(src)) {
                    Disable(dst);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryCopy(Entity src, Entity dst) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: TryCopy, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: TryCopy, Component type not registered");
                if (!src.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: TryCopy, cannot access ID - {id} from deleted entity");
                if (!dst.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: TryCopy, cannot access ID - {id} from deleted entity");
                if (IsBlocked()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: TryCopy,  component pool cannot be changed, it is in read-only mode due to multiple accesses");
                if (MultiThreadActive) throw new Exception($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: TryCopy, this operation is not supported in multithreaded mode");
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
            public uint Count() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Count, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Count, Component type not registered");
                #endif
                return _componentsCount;
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool IsRegistered() {
                return _registered;
            }
            
            [MethodImpl(AggressiveInlining)]
            public T[] Data() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Data, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Data, Component type not registered");
                #endif
                return _data;
            }
            #endregion

            #region INTERNAL
            [MethodImpl(AggressiveInlining)]
            internal ref T RefInternal(Entity entity) {
                return ref _data[_dataIdxByEntityId[entity._id] & Const.DisabledComponentMaskInv];
            }
            
            internal void Create(ushort componentId, BitMask bitMask, uint entitiesCapacity, OnAddHandler<T> onAdd = null, OnDeleteHandler<T> onDelete = null, OnCopyHandler<T> onCopy = null, OnAddHandler<T> autoPutInit = null, uint baseCapacity = 128) {
                _bitMask = bitMask;
                id = componentId;
                _entities = new uint[baseCapacity];
                _data = new T[baseCapacity];
                _componentsCount = 0;
                _dataIdxByEntityId = new uint[entitiesCapacity];
                for (uint i = 0; i < _dataIdxByEntityId.Length; i++) {
                    _dataIdxByEntityId[i] = Const.EmptyComponentMask;
                }
                _onAddHandler = onAdd;
                _onDeleteHandler = onDelete;
                _onCopyHandler = onCopy;
                _onPutHandler = autoPutInit;
                _registered = true;
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort DynamicId() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (Status < WorldStatus.Created) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: DynamicId, World not created");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: DynamicId, Component type not registered");
                #endif
                return id;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void DeleteInternalWithoutMask(Entity entity) {
                _componentsCount--;
                ref var idxRef = ref _dataIdxByEntityId[entity._id];
                ref var data = ref _data[idxRef & Const.DisabledComponentMaskInv];
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentDelete(entity, ref data);
                }
                #endif
                    
                if (_onDeleteHandler == null) {
                    data = default;
                } else {
                    _onDeleteHandler(ref data);
                }

                if (idxRef < _componentsCount) {
                    var lastEntity = _entities[_componentsCount];
                    _entities[idxRef & Const.DisabledComponentMaskInv] = lastEntity;
                    _dataIdxByEntityId[lastEntity] = idxRef;
                    (_data[_componentsCount], data) = (data, _data[_componentsCount]);
                }
                idxRef = Const.EmptyComponentMask;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal uint[] GetDataIdxByEntityId() {
                return _dataIdxByEntityId;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Resize(uint cap) {
                var lastLength = (uint) _dataIdxByEntityId.Length;
                Array.Resize(ref _dataIdxByEntityId, (int) cap);
                for (var i = lastLength; i < cap; i++) {
                    _dataIdxByEntityId[i] = Const.EmptyComponentMask;
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void SetDataIfCountLess(ref uint count, ref uint[] entities) {
                if (_componentsCount < count) {
                    count = _componentsCount;
                    entities = _entities;
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal uint[] EntitiesData() {
                return _entities;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal string ToStringComponent(Entity entity) {
                return HasDisabled(entity) 
                    ? $" - [{id}] [Disabled] {typeof(T).Name} ( {_data[_dataIdxByEntityId[entity._id] & Const.DisabledComponentMaskInv]} )\n" 
                    : $" - [{id}] {typeof(T).Name} ( {_data[_dataIdxByEntityId[entity._id] & Const.DisabledComponentMaskInv]} )\n";
            }

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                _onAddHandler = null;
                _onDeleteHandler = null;
                _onCopyHandler = null;
                _onPutHandler = null;
                _entities = null;
                _data = null;
                _dataIdxByEntityId = null;
                _bitMask = null;
                _componentsCount = 0;
                id = 0;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                debugEventListeners = null;
                #endif
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                _blockers = 0;
                #endif
                _registered = false;
            }

            [MethodImpl(AggressiveInlining)]
            internal void EnsureSize(uint size) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (Status < WorldStatus.Created) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: EnsureSize, World not created");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: EnsureSize, Component type not registered");
                #endif
                if (_componentsCount + size >= _entities.Length) {
                    ResizeData(Utils.CalculateSize(_componentsCount + size));
                }
            }

            [MethodImpl(NoInlining)]
            private void ResizeData(uint newSize) {
                Array.Resize(ref _entities, (int) newSize);
                Array.Resize(ref _data, (int) newSize);
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                Array.Clear(_entities, 0, _entities.Length);
                Array.Clear(_data, 0, _data.Length);
                for (uint i = 0; i < _dataIdxByEntityId.Length; i++) {
                    _dataIdxByEntityId[i] = Const.EmptyComponentMask;
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

    public delegate void OnAddHandler<T>(ref T component) where T : struct;

    public delegate void OnDeleteHandler<T>(ref T component) where T : struct;

    public delegate void OnCopyHandler<T>(ref T src, ref T dst) where T : struct;
}