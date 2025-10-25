#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

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
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

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

        #if FFS_ECS_EVENTS
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        public partial struct DEBUG {
            [MethodImpl(AggressiveInlining)]
            public static void AddTagDebugEventListener(ITagDebugEventListener listener) {
                ModuleTags.Value._debugEventListeners ??= new List<ITagDebugEventListener>();
                ModuleTags.Value._debugEventListeners.Add(listener);
            }

            [MethodImpl(AggressiveInlining)]
            public static void RemoveTagDebugEventListener(ITagDebugEventListener listener) {
                ModuleTags.Value._debugEventListeners?.Remove(listener);
            }
        }
        #endif
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        internal partial struct ModuleTags {
            public static ModuleTags Value;
            #if FFS_ECS_EVENTS
            internal List<ITagDebugEventListener> _debugEventListeners;
            #endif

            internal BitMask bitMask;
            private FreeChunkCommandBuffer _freeChunkCommandBuffer;
            private ITagsWrapper[] _pools;
            private Dictionary<Type, ITagsWrapper> _poolIdxByType;
            internal ushort poolsCount;

            [MethodImpl(AggressiveInlining)]
            internal void Create(uint baseComponentsCapacity) {
                _pools = new ITagsWrapper[baseComponentsCapacity];
                _poolIdxByType = new Dictionary<Type, ITagsWrapper>();
                bitMask = new BitMask();
                _freeChunkCommandBuffer = new FreeChunkCommandBuffer();
                #if FFS_ECS_EVENTS
                _debugEventListeners ??= new List<ITagDebugEventListener>();
                #endif
                Serializer.Value.Create();
            }

            [MethodImpl(AggressiveInlining)]
            internal void Initialize(uint chunksCapacity) {
                bitMask.Create(chunksCapacity, (ushort) (poolsCount.Normalize(Const.BITS_PER_LONG) >> Const.LONG_SHIFT));
                _freeChunkCommandBuffer.Create((uint) Entities.Value.chunks.Length, poolsCount);
                for (int i = 0; i < poolsCount; i++) {
                    _pools[i].Initialize(chunksCapacity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void RegisterTagType<C>(Guid guid) where C : struct, ITag {
                #if FFS_ECS_DEBUG
                AssertNotRegisteredTag<C>(WorldTypeName);
                #endif

                Tags<C>.Value.Create(guid, poolsCount, _freeChunkCommandBuffer, bitMask);
                
                #if FFS_ECS_EVENTS
                Tags<C>.Value.debugEventListeners = _debugEventListeners;
                #endif
                
                if (poolsCount == _pools.Length) {
                    Array.Resize(ref _pools, poolsCount << 1);
                }

                _pools[poolsCount] = new TagsWrapper<C>();
                _poolIdxByType[typeof(C)] = new TagsWrapper<C>();
                poolsCount++;
                Serializer.Value.RegisterTagType<C>(guid);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal List<IRawPool> GetAllRawsPools() {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                var pools = new List<IRawPool>();
                for (int i = 0; i < poolsCount; i++) {
                    pools.Add(_pools[i]);
                }
                return pools;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal ITagsWrapper GetPool(Type tagType) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                Assert(WorldTypeName, _poolIdxByType.ContainsKey(tagType), $"Tag type {tagType} not registered.");
                #endif
                return _poolIdxByType[tagType];
            }

            [MethodImpl(AggressiveInlining)]
            internal TagsWrapper<T> GetPool<T>() where T : struct, ITag {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertRegisteredTag<T>(WorldTypeName);
                #endif
                return new TagsWrapper<T>();
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(Type tagType, out ITagsWrapper pool) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                return _poolIdxByType.TryGetValue(tagType, out pool);
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool<T>(out TagsWrapper<T> pool) where T : struct, ITag {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                pool = new TagsWrapper<T>();
                return Tags<T>.Value.IsRegistered();
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort TagsCount(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                return bitMask.Len(eid);
            }

            [MethodImpl(AggressiveInlining)]
            internal void DestroyEntity(Entity entity) {
                if (poolsCount > 0) {
                    var maskLen = bitMask.MaskLen;
                    var eid = entity.id - Const.ENTITY_ID_OFFSET;
                    var masks = bitMask.Chunk(eid);
                    var start = (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen;
                    for (ushort i = 0; i < maskLen; i++) {
                        ref var mask = ref masks[start + i];
                        var offset = i << Const.LONG_SHIFT;
                        while (mask > 0) {
                            var id = Utils.PopLsb(ref mask) + offset;
                            _pools[id].DeleteWithoutMask(entity);
                        }
                    } 
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void ToPrettyStringEntity(StringBuilder builder, Entity entity) {
                builder.AppendLine("Tags:");
                if (poolsCount == 0) return;
                var maskLen = bitMask.MaskLen;
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var masks = bitMask.Chunk(eid);
                var start = (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen;
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
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                result.Clear();
                if (poolsCount == 0) return;
                var maskLen = bitMask.MaskLen;
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var masks = bitMask.Chunk(eid);
                var start = (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen;
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
                if (poolsCount == 0) return;
                var maskLen = bitMask.MaskLen;
                var srcEid = srcEntity.id - Const.ENTITY_ID_OFFSET;
                var masks = bitMask.Chunk(srcEid);
                var start = (srcEid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen;
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
            internal void MoveChunksToPool() {
                var buffer = _freeChunkCommandBuffer.Buffer;
                for (var i = _freeChunkCommandBuffer.BufferCount - 1; i >= 0; i--) {
                    var (poolId, chunkIdx) = buffer[i];
                    _pools[poolId].TryMoveChunkToPool(chunkIdx);
                }

                _freeChunkCommandBuffer.BufferCount = 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal void Resize(uint chunksCapacity) {
                if (IsWorldInitialized()) {
                    bitMask.ResizeBitMap(chunksCapacity);
                    _freeChunkCommandBuffer.Resize(chunksCapacity, poolsCount);
                    for (int i = 0, iMax = poolsCount; i < iMax; i++) {
                        _pools[i].Resize(chunksCapacity);
                    }
                } else {
                    Initialize(chunksCapacity);
                }
            }
                        
            [MethodImpl(AggressiveInlining)]
            internal void ClearChunk(uint chunkIdx) {
                bitMask.ClearChunk(chunkIdx);

                for (var i = 0; i < poolsCount; i++) {
                    _pools[i].ClearChunk(chunkIdx);
                }
            }

            internal void Destroy() {
                for (var i = 0; i < poolsCount; i++) {
                    _pools[i].Destroy();
                }

                bitMask.Destroy();
                bitMask = default;
                _freeChunkCommandBuffer = default;
                _pools = default;
                _poolIdxByType = default;
                poolsCount = default;
                Serializer.Value.Destroy();
                #if FFS_ECS_EVENTS
                _debugEventListeners = default;
                #endif
            }
        }
        
        #if FFS_ECS_EVENTS
        public interface ITagDebugEventListener {
            void OnTagAdd<T>(Entity entity) where T : struct, ITag;
            void OnTagDelete<T>(Entity entity) where T : struct, ITag;
        }
        #endif

    }
    
    public struct DeleteTagsSystem<WorldType, T> : IUpdateSystem where T : struct, ITag where WorldType : struct, IWorldType {
    
        [MethodImpl(AggressiveInlining)]
        public void Update() {
            World<WorldType>.Query.Entities<TagAll<T>>().DeleteTagForAll<T>();
        }
    }
}
