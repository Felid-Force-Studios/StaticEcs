#if !FFS_ECS_DISABLE_TAGS
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public interface ITag { }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        [MethodImpl(AggressiveInlining)]
        public static void RegisterTagType<T>(Guid guid = default) where T : struct, ITag {
            if (Status != WorldStatus.Created) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: RegisterTagType<{typeof(T)}>, World not created");
            }

            ModuleTags.Value.RegisterTagType<T>(guid);
        }

        [MethodImpl(AggressiveInlining)]
        public static ITagsWrapper GetTagsPool(Type tagType) {
            return ModuleTags.Value.GetPool(tagType);
        }

        [MethodImpl(AggressiveInlining)]
        public static TagsWrapper<T> GetTagsPool<T>() where T : struct, ITag {
            return ModuleTags.Value.GetPool<T>();
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetTagsPool(Type tagType, out ITagsWrapper pool) {
            return ModuleTags.Value.TryGetPool(tagType, out pool);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetTagsPool<T>(out TagsWrapper<T> pool) where T : struct, ITag {
            return ModuleTags.Value.TryGetPool(out pool);
        }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
        [MethodImpl(AggressiveInlining)]
        public static void AddTagDebugEventListener(ITagDebugEventListener listener) {
            ModuleTags.Value._debugEventListeners ??= new List<ITagDebugEventListener>();
            ModuleTags.Value._debugEventListeners.Add(listener);
        }

        [MethodImpl(AggressiveInlining)]
        public static void RemoveTagDebugEventListener(ITagDebugEventListener listener) {
            ModuleTags.Value._debugEventListeners?.Remove(listener);
        }
        #endif
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        internal partial struct ModuleTags {
            public static ModuleTags Value;
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<ITagDebugEventListener> _debugEventListeners;
            #endif

            private BitMask _bitMask;
            private ITagsWrapper[] _pools;
            private Dictionary<Type, ITagsWrapper> _poolIdxByType;
            private ushort _poolsCount;

            [MethodImpl(AggressiveInlining)]
            internal void Create(uint baseComponentsCapacity) {
                _pools = new ITagsWrapper[baseComponentsCapacity];
                _poolIdxByType = new Dictionary<Type, ITagsWrapper>();
                _bitMask = new BitMask();
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners ??= new List<ITagDebugEventListener>();
                #endif
                Serializer.Value.Create();
            }

            [MethodImpl(AggressiveInlining)]
            internal void Initialize() {
                _bitMask.Create(CalculateEntitiesCapacity(), (ushort) (_poolsCount.Normalize(Const.BITS_PER_LONG) >> Const.LONG_SHIFT));
                for (int i = 0; i < _poolsCount; i++) {
                    _pools[i].UpdateBitMask(_bitMask);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void RegisterTagType<C>(Guid guid) where C : struct, ITag {
                if (Tags<C>.Value.IsRegistered()) throw new StaticEcsException($"Tags {typeof(C)} already registered");

                Tags<C>.Value.Create(guid, _poolsCount, CalculateEntitiesCapacity());
                
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                Tags<C>.Value.debugEventListeners = _debugEventListeners;
                #endif
                
                if (_poolsCount == _pools.Length) {
                    Array.Resize(ref _pools, _poolsCount << 1);
                }

                _pools[_poolsCount] = new TagsWrapper<C>();
                _poolIdxByType[typeof(C)] = new TagsWrapper<C>();
                _poolsCount++;
                Serializer.Value.RegisterTagType<C>(guid);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal List<IRawPool> GetAllRawsPools() {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetAllRawsPools, World not initialized");
                #endif
                var pools = new List<IRawPool>();
                for (int i = 0; i < _poolsCount; i++) {
                    pools.Add(_pools[i]);
                }
                return pools;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal ITagsWrapper GetPool(Type tagType) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleTags, Method: GetPool, World not initialized");
                if (!_poolIdxByType.ContainsKey(tagType)) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleTags, Method: GetPool, Tag type {tagType} not registered");
                #endif
                return _poolIdxByType[tagType];
            }

            [MethodImpl(AggressiveInlining)]
            internal TagsWrapper<T> GetPool<T>() where T : struct, ITag {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleTags, Method: GetPool<{typeof(T)}, World not initialized");
                if (!Tags<T>.Value.IsRegistered()) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleTags, Method: GetPool<{typeof(T)}>, Tag type not registered");
                #endif
                return default;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(Type tagType, out ITagsWrapper pool) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleTags, Method: GetPool, World not initialized");
                #endif
                return _poolIdxByType.TryGetValue(tagType, out pool);
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool<T>(out TagsWrapper<T> pool) where T : struct, ITag {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleTags, Method: GetPool<{typeof(T)}, World not initialized");
                #endif
                pool = default;
                return Tags<T>.Value.IsRegistered();
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort TagsCount(Entity entity) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleTags, Method: TagsCount, World not initialized");
                #endif
                return _bitMask.Len(entity._id);
            }

            [MethodImpl(AggressiveInlining)]
            internal void DestroyEntity(Entity entity) {
                if (_poolsCount == 0) return;
                
                var maskLen = _bitMask.MaskLen;
                var masks = _bitMask.Chunk(entity._id);
                var start = (entity._id & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen;
                for (ushort i = 0; i < maskLen; i++) {
                    ref var mask = ref masks[start + i];
                    var offset = i << Const.LONG_SHIFT;
                    while (mask > 0) {
                        var id = Utils.PopLsb(ref mask) + offset;
                        _pools[id].DeleteWithoutMask(entity);
                    }
                }                
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void ToPrettyStringEntity(StringBuilder builder, Entity entity) {
                builder.AppendLine("Tags:");
                if (_poolsCount == 0) return;
                var maskLen = _bitMask.MaskLen;
                var masks = _bitMask.Chunk(entity._id);
                var start = (entity._id & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen;
                for (ushort i = 0; i < maskLen; i++) {
                    var mask = masks[start + i];
                    var offset = i << Const.LONG_SHIFT;
                    while (mask > 0) {
                        var id = Utils.PopLsb(ref mask) + offset;
                        _pools[id].ToStringComponent(builder, entity);
                    }
                } 
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void GetAllTags(Entity entity, List<ITag> result) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.ModuleTags, Method: GetAllTags, World not initialized");
                #endif
                result.Clear();
                if (_poolsCount == 0) return;
                var maskLen = _bitMask.MaskLen;
                var masks = _bitMask.Chunk(entity._id);
                var start = (entity._id & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen;
                for (ushort i = 0; i < maskLen; i++) {
                    var mask = masks[start + i];
                    var offset = i << Const.LONG_SHIFT;
                    while (mask > 0) {
                        var id = Utils.PopLsb(ref mask) + offset;
                        result.Add(_pools[id].GetRaw());
                    }
                } 
            }

            [MethodImpl(AggressiveInlining)]
            internal void CopyEntity(Entity srcEntity, Entity dstEntity) {
                if (_poolsCount == 0) return;
                var maskLen = _bitMask.MaskLen;
                var masks = _bitMask.Chunk(srcEntity._id);
                var start = (srcEntity._id & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen;
                for (ushort i = 0; i < maskLen; i++) {
                    var mask = masks[start + i];
                    var offset = i << Const.LONG_SHIFT;
                    while (mask > 0) {
                        var id = Utils.PopLsb(ref mask) + offset;
                        _pools[id].Copy(srcEntity, dstEntity);
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void Resize(uint cap) {
                _bitMask.ResizeBitMap(cap);
                for (int i = 0, iMax = _poolsCount; i < iMax; i++) {
                    _pools[i].Resize(cap);
                    _pools[i].UpdateBitMask(_bitMask);
                }
            }
                        
            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Clear();
                }

                _bitMask.Clear(Entities.Value.nextActiveChunkIdx);
            }

            internal void Destroy() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Destroy();
                }

                _bitMask.Destroy();
                _bitMask = default;
                _pools = default;
                _poolIdxByType = default;
                _poolsCount = default;
                Serializer.Value.Destroy();
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners = default;
                #endif
            }
        }
        
        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
        public interface ITagDebugEventListener {
            void OnTagAdd<T>(Entity entity) where T : struct, ITag;
            void OnTagDelete<T>(Entity entity) where T : struct, ITag;
        }
        #endif

    }
}
#endif