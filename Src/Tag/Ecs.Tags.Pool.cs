#if !FFS_ECS_DISABLE_TAGS
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
        public struct Tags<T> where T : struct, ITag {
            public static Tags<T> Value;
            
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<ITagDebugEventListener> debugEventListeners;
            #endif
            
            private BitMask _bitMask;
            private uint[] _entities;
            private uint[] _dataIdxByEntityId;
            private uint _tagCount;
            internal ushort id;
            private bool _registered;

            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            private int _blockers;
            #endif

            #region PUBLIC
            [MethodImpl(AggressiveInlining)]
            public void Set(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Add, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Add, Tag type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Add, cannot access ID - {id} from deleted entity");
                if (IsBlocked()) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Add,  component pool cannot be changed, it is in read-only mode due to multiple accesses");
                #endif

                var eid = entity._id;
                ref var idx = ref _dataIdxByEntityId[entity._id];
                if (idx != Const.EmptyComponentMask) {
                    return;
                }
                
                if (_entities.Length == _tagCount) {
                    Array.Resize(ref _entities, (int) (_tagCount << 1));
                }

                _entities[_tagCount] = eid;
                idx = _tagCount;
                _bitMask.Set(eid, id);
                _tagCount++;

                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnTagAdd<T>(entity);
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public bool Has(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Has, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Has, Tag type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Has, cannot access ID - {id} from deleted entity");
                #endif
                return _dataIdxByEntityId[entity._id] != Const.EmptyComponentMask;
            }

            [MethodImpl(AggressiveInlining)]
            public void Delete(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Delete, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Delete, Tag type not registered");
                if (!entity.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Delete, cannot access ID - {id} from deleted entity");
                if (!Has(entity)) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Delete, cannot access ID - {id} tag not added");
                if (IsBlocked()) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Delete,  component pool cannot be changed, it is in read-only mode due to multiple accesses");
                #endif

                ref var idx = ref _dataIdxByEntityId[entity._id];
                _tagCount--;

                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                foreach (var listener in debugEventListeners) {
                    listener.OnTagDelete<T>(entity);
                }
                #endif
                    
                var lastEntity = _entities[_tagCount];
                _entities[idx] = lastEntity;
                _dataIdxByEntityId[lastEntity] = idx;
                idx = Const.EmptyComponentMask;
                _bitMask.Del(entity._id, id);
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
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Copy, World not initialized");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: Copy, Tag type not registered");
                if (!src.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
                if (!dst.IsActual()) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Copy, cannot access ID - {id} from deleted entity");
                if (IsBlocked()) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, {typeof(T)}>, Method: Copy,  component pool cannot be changed, it is in read-only mode due to multiple accesses");
                #endif

                if (Has(src)) {
                    Set(dst);
                }
            }
                        
            [MethodImpl(AggressiveInlining)]
            public void Move(Entity src, Entity dst) {
                Copy(src, dst);
                Delete(src);
            }

            [MethodImpl(AggressiveInlining)]
            public uint Count() => _tagCount;
                       
            [MethodImpl(AggressiveInlining)]
            public bool IsRegistered() {
                return _registered;
            }

            [MethodImpl(AggressiveInlining)]
            public string ToStringComponent(Entity entity) {
                return $" - [{id}] {typeof(T).Name}\n";
            }
            #endregion

            #region INTERNAL
            internal void Create(ushort tagId, BitMask bitMask, uint baseCapacity = 128) {
                _bitMask = bitMask;
                id = tagId;
                _entities = new uint[baseCapacity];
                _tagCount = 0;
                _dataIdxByEntityId = new uint[World.EntitiesCapacity()];
                for (uint i = 0; i < _dataIdxByEntityId.Length; i++) {
                    _dataIdxByEntityId[i] = Const.EmptyComponentMask;
                }

                _registered = true;
            }
            
            
            [MethodImpl(AggressiveInlining)]
            public TagDynId DynamicId() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (World.Status < WorldStatus.Created) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: DynamicId, World not created");
                if (!_registered) throw new Exception($"Ecs<{typeof(WorldType)}>.Tags<{typeof(T)}>, Method: DynamicId, Tag type not registered");
                #endif
                return new TagDynId(id);
            }

            [MethodImpl(AggressiveInlining)]
            internal void SetDataIfCountLess(ref uint count, ref uint[] entities, out ushort poolId) {
                if (_tagCount < count) {
                    count = _tagCount;
                    entities = _entities;
                }

                poolId = id;
            }

            [MethodImpl(AggressiveInlining)]
            internal uint[] EntitiesData() => _entities;

            [MethodImpl(AggressiveInlining)]
            internal void Resize(uint cap) {
                var lastLength = (uint) _dataIdxByEntityId.Length;
                Array.Resize(ref _dataIdxByEntityId, (int) cap);
                for (var i = lastLength; i < cap; i++) {
                    _dataIdxByEntityId[i] = Const.EmptyComponentMask;
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void SetDataIfCountMore(ref uint count, ref uint[] entities, out ushort poolId) {
                if (_tagCount > count) {
                    count = _tagCount;
                    entities = _entities;
                }

                poolId = id;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal uint[] GetDataIdxByEntityId() {
                return _dataIdxByEntityId;
            }

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                _entities = null;
                _dataIdxByEntityId = null;
                _tagCount = 0;
                id = 0;
                _registered = false;
                _bitMask = null;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                _blockers = 0;
                #endif
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                debugEventListeners = null;
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                Array.Clear(_entities, 0, _entities.Length);
                for (var i = 0; i < _dataIdxByEntityId.Length; i++) {
                    _dataIdxByEntityId[i] = Const.EmptyComponentMask;
                }
                _tagCount = 0;
            }

            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            [MethodImpl(AggressiveInlining)]
            internal void AddBlocker(int amount) {
                _blockers += amount;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (_blockers < 0) throw new Exception($"TagsPool<{typeof(WorldType)}, {typeof(T)}>, Method: AddBlocker, incorrect pool user balance when attempting to release");
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal bool IsBlocked() {
                return _blockers > 1;
            }
            #endif
            #endregion
        }
    }
}
#endif