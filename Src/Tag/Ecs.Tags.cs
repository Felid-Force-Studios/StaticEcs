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
        internal struct ModuleTags {
            public static ModuleTags Value;
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<ITagDebugEventListener> _debugEventListeners;
            #endif

            internal BitMask BitMask;
            
            private ITagsWrapper[] _pools;
            private Dictionary<Type, int> _poolIdxByType;
            private ushort _poolsCount;

            [MethodImpl(AggressiveInlining)]
            internal void Create(uint baseComponentsCapacity) {
                _pools = new ITagsWrapper[baseComponentsCapacity];
                _poolIdxByType = new Dictionary<Type, int>();
                BitMask = new BitMask();
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners ??= new List<ITagDebugEventListener>();
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal void Initialize() {
                BitMask.Create(World.EntitiesCapacity(), 32, Utils.CalculateMaskLen(_poolsCount), false);
            }

            [MethodImpl(AggressiveInlining)]
            internal TagDynId RegisterTagType<C>(uint capacity) where C : struct, ITag {
                if (Tags<C>.Value.IsRegistered()) {
                    return Tags<C>.Value.DynamicId();
                }

                Tags<C>.Value.Create(_poolsCount, BitMask, capacity);
                
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                Tags<C>.Value.debugEventListeners = _debugEventListeners;
                #endif
                
                if (_poolsCount == _pools.Length) {
                    Array.Resize(ref _pools, _poolsCount << 1);
                }

                _pools[_poolsCount] = new TagsWrapper<C>();
                _poolIdxByType[typeof(C)] = _poolsCount;
                _poolsCount++;

                return Tags<C>.Value.DynamicId();
            }
            
            [MethodImpl(AggressiveInlining)]
            internal List<IRawPool> GetAllRawsPools() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetAllRawsPools, World not initialized");
                #endif
                var pools = new List<IRawPool>();
                for (int i = 0; i < _poolsCount; i++) {
                    pools.Add(_pools[i]);
                }
                return pools;
            }

            [MethodImpl(AggressiveInlining)]
            internal ITagsWrapper GetPool(TagDynId id) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.ModuleTags, Method: GetPool, World not initialized");
                if (id.Value >= _poolsCount) throw new Exception($"Ecs<{typeof(WorldType)}>.ModuleTags, Method: GetPool, Tag type for dyn id {id.Value} not registered");
                #endif
                return _pools[id.Value];
            }
            
            [MethodImpl(AggressiveInlining)]
            internal ITagsWrapper GetPool(Type tagType) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.ModuleTags, Method: GetPool, World not initialized");
                if (!_poolIdxByType.ContainsKey(tagType)) throw new Exception($"World<{typeof(WorldType)}>.ModuleTags, Method: GetPool, Tag type {tagType} not registered");
                #endif
                return _pools[_poolIdxByType[tagType]];
            }

            [MethodImpl(AggressiveInlining)]
            internal TagsWrapper<T> GetPool<T>() where T : struct, ITag {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.ModuleTags, Method: GetPool<{typeof(T)}, World not initialized");
                if (!Tags<T>.Value.IsRegistered()) throw new Exception($"Ecs<{typeof(WorldType)}>.ModuleTags, Method: GetPool<{typeof(T)}>, Tag type not registered");
                #endif
                return default;
            }
                        
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(TagDynId id, out ITagsWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.ModuleTags, Method: GetPool, World not initialized");
                #endif
                if (id.Value >= _poolsCount) {
                    pool = default;
                    return false;
                }
                
                pool = _pools[id.Value];
                return true;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(Type tagType, out ITagsWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.ModuleTags, Method: GetPool, World not initialized");
                #endif
                if (!_poolIdxByType.TryGetValue(tagType, out var idx)) {
                    pool = default;
                    return false;
                }
                
                pool = _pools[idx];
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool<T>(out TagsWrapper<T> pool) where T : struct, ITag {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.ModuleTags, Method: GetPool<{typeof(T)}, World not initialized");
                #endif
                pool = default;
                return Tags<T>.Value.IsRegistered();
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort TagsCount(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.ModuleTags, Method: TagsCount, World not initialized");
                #endif
                return BitMask.Len(entity._id);
            }

            [MethodImpl(AggressiveInlining)]
            internal void DestroyEntity(Entity entity) {
                while (BitMask.GetMinIndex(entity._id, out var id)) {
                    _pools[id].Delete(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal string ToPrettyStringEntity(Entity entity) {
                var str = "Tags:\n";
                var bufId = BitMask.BorrowBuf();
                BitMask.CopyToBuffer(entity._id, bufId);
                while (BitMask.GetMinIndexBuffer(bufId, out var id)) {
                    str += _pools[id].ToStringComponent(entity);
                    BitMask.DelInBuffer(bufId, (ushort) id);
                }
                BitMask.DropBuf();
                return str;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void GetAllTags(Entity entity, List<ITag> result) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"Ecs<{typeof(WorldType)}>.ModuleTags, Method: GetAllTags, World not initialized");
                #endif
                result.Clear();
                var bufId = BitMask.BorrowBuf();
                BitMask.CopyToBuffer(entity._id, bufId);
                while (BitMask.GetMinIndexBuffer(bufId, out var id)) {
                    result.Add(_pools[id].GetRaw());
                    BitMask.DelInBuffer(bufId, (ushort) id);
                }
                BitMask.DropBuf();
            }

            [MethodImpl(AggressiveInlining)]
            internal void CopyEntity(Entity srcEntity, Entity dstEntity) {
                var bufId = BitMask.BorrowBuf();
                BitMask.CopyToBuffer(srcEntity._id, bufId);
                while (BitMask.GetMinIndexBuffer(bufId, out var id)) {
                    _pools[id].Copy(srcEntity, dstEntity);
                    BitMask.DelInBuffer(bufId, (ushort) id);
                }

                BitMask.DropBuf();
            }

            [MethodImpl(AggressiveInlining)]
            internal void Resize(uint cap) {
                for (int i = 0, iMax = _poolsCount; i < iMax; i++) {
                    _pools[i].Resize(cap);
                }
                BitMask.ResizeBitMap(cap);
            }
                        
            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Clear();
                }

                BitMask.Clear();
            }

            internal void Destroy() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Destroy();
                }

                BitMask.Destroy();
                BitMask = default;
                _pools = default;
                _poolIdxByType = default;
                _poolsCount = default;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners = default;
                #endif
            }
        }
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
        public interface ITagDebugEventListener {
            void OnTagAdd<T>(Entity entity) where T : struct, ITag;
            void OnTagDelete<T>(Entity entity) where T : struct, ITag;
        }
        #endif

    }
    
    public struct DeleteTagsSystem<WorldType, T> : IUpdateSystem where T : struct, ITag where WorldType : struct, IWorldType {
        [MethodImpl(AggressiveInlining)]
        public void Update() {
            Ecs<WorldType>.World.QueryEntities.For<TagAll<T>>().DeleteTagForAll<T>();
        }
    }
}
#endif