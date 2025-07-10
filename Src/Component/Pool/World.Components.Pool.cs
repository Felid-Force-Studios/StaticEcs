using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
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
        public partial struct Components<T> where T : struct, IComponent {
            public static Components<T> Value;
            
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<IComponentsDebugEventListener> debugEventListeners;
            #endif
            
            internal OnComponentHandler<T> onAddHandler;
            internal OnComponentHandler<T> onPutHandler;
            internal OnComponentHandler<T> onDeleteHandler;
            internal OnCopyHandler<T> onCopyHandler;
            
            private uint[] _entities;
            private T[] _data;
            private uint[] _dataIdxByEntityId;
            private BitMask _bitMask;
            private uint _componentsCount;
            internal ushort id;
            private bool _registered;
            private bool _copyable;

            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            private int _blockers;
            private int _blockersAdd;
            #endif
            internal string AddWithoutValueError;

            #region PUBLIC
            [MethodImpl(AggressiveInlining)]
            public ref T Ref(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Ref, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, cannot access Entity ID - {id} from deleted entity");
                if (!Has(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Ref, ID - {entity._id} is missing on an entity");
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
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Add, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, cannot access Entity ID - {id} from deleted entity");
                if (Has(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, ID - {entity._id} is already on an entity");
                if (IsBlocked()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, component pool cannot be changed, it is in read-only mode due to multiple accesses");
                if (IsBlockedAdd()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, component pool cannot be changed, it is in read-only mode due to multiple accesses");
                if (MultiThreadActive) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, this operation is not supported in multithreaded mode");
                if (AddWithoutValueError != null) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, {AddWithoutValueError}");
                #endif

                if (_entities.Length == _componentsCount) {
                    ResizeData(_componentsCount << 1);
                }

                var eid = entity._id;
                _entities[_componentsCount] = eid;
                _dataIdxByEntityId[eid] = _componentsCount;

                _bitMask.Set(eid, id);
                
                ref var data = ref _data[_componentsCount];
                _componentsCount++;
                onAddHandler?.Invoke(entity, ref data);

                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentAdd(entity, ref data);
                }
                #endif
                
                return ref data;
            }
            
            [MethodImpl(AggressiveInlining)]
            public ref T Add(Entity entity, T val) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Add, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, cannot access Entity ID - {id} from deleted entity");
                if (Has(entity)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, ID - {entity._id} is already on an entity");
                if (IsBlocked()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, component pool cannot be changed, it is in read-only mode due to multiple accesses");
                if (IsBlockedAdd()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, component pool cannot be changed, it is in read-only mode due to multiple accesses");
                if (MultiThreadActive) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Add, this operation is not supported in multithreaded mode");
                #endif

                if (_entities.Length == _componentsCount) {
                    ResizeData(_componentsCount << 1);
                }

                var eid = entity._id;
                _entities[_componentsCount] = eid;
                _dataIdxByEntityId[eid] = _componentsCount;

                _bitMask.Set(eid, id);
                
                ref var data = ref _data[_componentsCount];
                data = val;
                _componentsCount++;
                onAddHandler?.Invoke(entity, ref data);

                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentAdd(entity, ref data);
                }
                #endif
                return ref data;
            }

            [MethodImpl(AggressiveInlining)]
            internal void AddInternal(Entity entity, T component) {
                if (_entities.Length == _componentsCount) {
                    ResizeData(_componentsCount << 1);
                }

                var eid = entity._id;
                _entities[_componentsCount] = eid;
                _dataIdxByEntityId[eid] = _componentsCount;

                _bitMask.Set(eid, id);

                ref var data = ref _data[_componentsCount];
                data = component;
                _componentsCount++;
                
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentAdd(entity, ref data);
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public void Put(Entity entity, T component) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Put, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, cannot access ID - {id} from deleted entity");
                if (IsBlocked()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, component pool cannot be changed, it is in read-only mode due to multiple accesses");
                if (IsBlockedAdd()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, component pool cannot be changed, it is in read-only mode due to multiple accesses");
                if (MultiThreadActive) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Put, this operation is not supported in multithreaded mode");
                #endif
                var eid = entity._id;
                ref var dataId = ref _dataIdxByEntityId[eid];
                if (dataId != Const.EmptyComponentMask) {
                    ref var data = ref _data[dataId & Const.DisabledComponentMaskInv];
                    data = component;
                    onPutHandler?.Invoke(entity, ref data);
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                    foreach (var listener in debugEventListeners) {
                        listener.OnComponentPut(entity, ref data);
                    }
                    #endif
                    return;
                }

                if (_entities.Length == _componentsCount) {
                    ResizeData(_componentsCount << 1);
                }

                _entities[_componentsCount] = eid;
                dataId = _componentsCount;
                _componentsCount++;

                _bitMask.Set(eid, id);
                ref var val = ref _data[dataId];
                val = component;
                onPutHandler?.Invoke(entity, ref val);

                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentPut(entity, ref val);
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
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Has, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Has, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Has, cannot access ID - {id} from deleted entity");
                #endif
                return _dataIdxByEntityId[entity._id] != Const.EmptyComponentMask;
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasDisabled(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: HasDisabled, World not initialized");
                if (!_registered) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: HasDisabled, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: HasDisabled, cannot access ID - {id} from deleted entity");
                #endif
                return (_dataIdxByEntityId[entity._id] & Const.EmptyAndDisabledComponentMask) == Const.DisabledComponentMask;
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasEnabled(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}> Method: HasEnabled, World not initialized");
                if (!_registered) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: HasEnabled, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: HasEnabled, cannot access ID - {id} from deleted entity");
                #endif
                return (_dataIdxByEntityId[entity._id] & Const.EmptyAndDisabledComponentMask) == 0;
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Disable(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Disable, World not initialized");
                if (!_registered) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Disable, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Disable, cannot access ID - {id} from deleted entity");
                if (!Has(entity)) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Disable, ID - {entity._id} is missing on an entity");
                if (MultiThreadActive) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Disable: Disable, this operation is not supported in multithreaded mode");
                #endif
                _dataIdxByEntityId[entity._id] |= Const.DisabledComponentMask;
                _bitMask.MoveToDisabled(entity._id, id);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Enable(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Enable, World not initialized");
                if (!_registered) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Enable, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Enable, cannot access ID - {id} from deleted entity");
                if (!Has(entity)) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Enable, ID - {entity._id} is missing on an entity");
                if (MultiThreadActive) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Enable, this operation is not supported in multithreaded mode");
                #endif
                _dataIdxByEntityId[entity._id] &= Const.DisabledComponentMaskInv;
                _bitMask.MoveToEnabled(entity._id, id);
            }

            [MethodImpl(AggressiveInlining)]
            public void Delete(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Delete, World not initialized");
                if (!_registered) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete, Component type not registered");
                if (!entity.IsActual()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete, cannot access ID - {id} from deleted entity");
                if (!Has(entity)) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete, cannot access ID - {id} component not added");
                if (IsBlocked()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete,  component pool cannot be changed, it is in read-only mode due to multiple accesses");
                if (MultiThreadActive) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Delete, this operation is not supported in multithreaded mode");
                #endif
                _bitMask.DelWithDisabled(entity._id, id);
                DeleteInternalWithoutMask(entity);
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
                if (!IsWorldInitialized()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Copy, World not initialized");
                if (!_registered) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, Component type not registered");
                if (!src.IsActual()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
                if (!dst.IsActual()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
                if (IsBlocked()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy,  component pool cannot be changed, it is in read-only mode due to multiple accesses");
                if (MultiThreadActive) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Copy, this operation is not supported in multithreaded mode");
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
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: TryCopy, World not initialized");
                if (!_registered) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: TryCopy, Component type not registered");
                if (!src.IsActual()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: TryCopy, cannot access ID - {id} from deleted entity");
                if (!dst.IsActual()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: TryCopy, cannot access ID - {id} from deleted entity");
                if (IsBlocked()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: TryCopy,  component pool cannot be changed, it is in read-only mode due to multiple accesses");
                if (MultiThreadActive) throw new StaticEcsException($"World<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: TryCopy, this operation is not supported in multithreaded mode");
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
                if (!IsWorldInitialized()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Count, World not initialized");
                if (!_registered) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Count, Component type not registered");
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
                if (!IsWorldInitialized()) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}> Method: Data, World not initialized");
                if (!_registered) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: Data, Component type not registered");
                #endif
                return _data;
            }
            #endregion

            #region INTERNAL
            [MethodImpl(AggressiveInlining)]
            internal ref T RefInternal(Entity entity) {
                return ref _data[_dataIdxByEntityId[entity._id] & Const.DisabledComponentMaskInv];
            }
            
            internal void Create(ushort componentId, BitMask bitMask, uint entitiesCapacity,
                                 IComponentConfig<T, WorldType> config,
                                 uint baseCapacity = 128) {
                _bitMask = bitMask;
                id = componentId;
                _entities = new uint[baseCapacity];
                _data = new T[baseCapacity];
                _componentsCount = 0;
                _dataIdxByEntityId = new uint[entitiesCapacity];
                for (uint i = 0; i < _dataIdxByEntityId.Length; i++) {
                    _dataIdxByEntityId[i] = Const.EmptyComponentMask;
                }
                onAddHandler = config.OnAdd();
                onPutHandler = config.OnPut();
                onDeleteHandler = config.OnDelete();
                onCopyHandler = config.OnCopy();
                _copyable = config.IsCopyable();
                Serializer.Value.Create(config);
                _registered = true;
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort DynamicId() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (Status < WorldStatus.Created) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: DynamicId, World not created");
                if (!_registered) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: DynamicId, Component type not registered");
                #endif
                return id;
            }

            [MethodImpl(AggressiveInlining)]
            internal Guid Guid() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (Status < WorldStatus.Created) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: DynamicId, World not created");
                if (!_registered) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: DynamicId, Component type not registered");
                #endif
                return Serializer.Value.guid;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void DeleteInternalWithoutMask(Entity entity) {
                _componentsCount--;
                ref var idxRef = ref _dataIdxByEntityId[entity._id];
                var idx = idxRef & Const.DisabledComponentMaskInv;
                idxRef = Const.EmptyComponentMask;

                if (idx < _componentsCount) {
                    var lastEntity = _entities[_componentsCount];
                    _entities[idx] = lastEntity;
                    ref var lastEntityIdx = ref _dataIdxByEntityId[lastEntity];
                    lastEntityIdx = idx | (lastEntityIdx & Const.DisabledComponentMask);
                    ref var a = ref _data[_componentsCount];
                    ref var b = ref _data[idx];
                    var temp = a;
                    a = b;
                    b = temp;
                    idx = _componentsCount;
                }

                ref var data = ref _data[idx];
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnComponentDelete(entity, ref data);
                }
                #endif
                
                if (onDeleteHandler == null) {
                    data = default;
                } else {
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    AddBlockerAdd(1);
                    #endif
                    onDeleteHandler(entity, ref data);
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG
                    AddBlockerAdd(-1);
                    #endif
                }
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
            internal void ToStringComponent(StringBuilder builder, Entity entity) {
                builder.Append(" - [");
                builder.Append(id);
                builder.Append("] ");
                if (HasDisabled(entity)) {
                    builder.Append("[Disabled] ");
                }
                builder.Append(typeof(T).Name);
                builder.Append(" ( ");
                builder.Append(_data[_dataIdxByEntityId[entity._id] & Const.DisabledComponentMaskInv].ToString());
                builder.AppendLine(" )");
            }

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                onAddHandler = null;
                onPutHandler = null;
                onDeleteHandler = null;
                onCopyHandler = null;
                _entities = null;
                _data = null;
                _dataIdxByEntityId = null;
                _bitMask = null;
                _componentsCount = 0;
                id = 0;
                Serializer.Value.Destroy();
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                debugEventListeners = null;
                #endif
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                _blockers = 0;
                _blockersAdd = 0;
                #endif
                AddWithoutValueError = default;
                _copyable = false;
                _registered = false;
            }

            [MethodImpl(AggressiveInlining)]
            internal void EnsureSize(uint size) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (Status < WorldStatus.Created) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: EnsureSize, World not created");
                if (!_registered) throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: EnsureSize, Component type not registered");
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
                if (onDeleteHandler != null) {
                    for (var i = 0; i < _componentsCount; i++) {
                        onDeleteHandler(Entity.FromIdx(_entities[i]), ref _data[i]);
                    }
                } else {
                    Utils.LoopFallbackClear(_data, 0, (int) _componentsCount);
                } 
                Utils.LoopFallbackClear(_entities, 0, (int) _componentsCount);
                
                for (uint i = 0; i < _componentsCount; i++) {
                    _dataIdxByEntityId[i] = Const.EmptyComponentMask;
                }
                _componentsCount = 0;
            }
            
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            [MethodImpl(AggressiveInlining)]
            internal void AddBlocker(int amount) {
                _blockers += amount;
                if (_blockers < 0) {
                    throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: AddBlocker, incorrect pool user balance when attempting to release");
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal bool IsBlocked() {
                return _blockers > 1;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void AddBlockerAdd(int amount) {
                _blockersAdd += amount;
                if (_blockersAdd < 0) {
                    throw new StaticEcsException($"Ecs<{typeof(WorldType)}>.Components<{typeof(T)}>, Method: AddBlockerAdd, incorrect pool user balance when attempting to release");
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal bool IsBlockedAdd() {
                return _blockersAdd > 1;
            }
            #endif
            #endregion
        }

        public delegate void OnComponentHandler<T>(Entity entity, ref T component) where T : struct;

        public delegate void OnCopyHandler<T>(Entity srcEntity, Entity dstEntity, ref T src, ref T dst) where T : struct;
    }
}