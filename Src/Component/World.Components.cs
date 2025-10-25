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
using System.Threading;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public interface IComponent { }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal class FreeChunkCommandBuffer {
        internal (ushort poolId, uint chunkIdx)[] Buffer;
        internal int BufferCount;

        [MethodImpl(AggressiveInlining)]
        public void Create(uint chunkCapacity, ushort poolCount) {
            Buffer = new (ushort poolId, uint chunkIdx)[chunkCapacity * poolCount];
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Resize(uint chunkCapacity, ushort poolCount) {
            Array.Resize(ref Buffer, (int) (chunkCapacity * poolCount));
        }
        
        [MethodImpl(AggressiveInlining)]
        public void Add(uint chunkIdx, ushort poolId) {
            Buffer[Interlocked.Increment(ref BufferCount) - 1] = (poolId, chunkIdx);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {

        [MethodImpl(AggressiveInlining)]
        public static void RegisterComponentType<T>(IComponentConfig<T, WorldType> config = null)
            where T : struct, IComponent {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            config ??= DefaultComponentConfig<T, WorldType>.Default;
            ModuleComponents.Value.RegisterComponentType(config);
        }

        [MethodImpl(AggressiveInlining)]
        public static IComponentsWrapper GetComponentsPool(Type componentType) {
            return ModuleComponents.Value.GetPool(componentType);
        }

        [MethodImpl(AggressiveInlining)]
        public static ComponentsWrapper<T> GetComponentsPool<T>() where T : struct, IComponent {
            return ModuleComponents.Value.GetPool<T>();
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetComponentsPool(Type componentType, out IComponentsWrapper pool) {
            return ModuleComponents.Value.TryGetPool(componentType, out pool);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetComponentsPool<T>(out ComponentsWrapper<T> pool) where T : struct, IComponent {
            return ModuleComponents.Value.TryGetPool(out pool);
        }
        
        #if FFS_ECS_EVENTS
        public partial struct DEBUG {
            [MethodImpl(AggressiveInlining)]
            public static void AddComponentsDebugEventListener(IComponentsDebugEventListener listener) {
                ModuleComponents.Value._debugEventListeners ??= new List<IComponentsDebugEventListener>();
                ModuleComponents.Value._debugEventListeners.Add(listener);
            }

            [MethodImpl(AggressiveInlining)]
            public static void RemoveComponentsDebugEventListener(IComponentsDebugEventListener listener) {
                ModuleComponents.Value._debugEventListeners?.Remove(listener);
            }
        }
        #endif
        
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        internal partial struct ModuleComponents {
            public static ModuleComponents Value;
            
            #if FFS_ECS_EVENTS
            internal List<IComponentsDebugEventListener> _debugEventListeners;
            #endif
            
            internal BitMask bitMask;
            private FreeChunkCommandBuffer _freeChunkCommandBuffer;
            private IComponentsWrapper[] _pools;
            private Dictionary<Type, IComponentsWrapper> _poolByType;
            internal ushort poolsCount;

            [MethodImpl(AggressiveInlining)]
            internal void Create(uint baseComponentsCapacity) {
                _pools = new IComponentsWrapper[baseComponentsCapacity];
                _poolByType = new Dictionary<Type, IComponentsWrapper>();
                bitMask = new BitMask();
                _freeChunkCommandBuffer = new FreeChunkCommandBuffer();
                #if FFS_ECS_EVENTS
                _debugEventListeners ??= new List<IComponentsDebugEventListener>();
                #endif
                Serializer.Value.Create();
            }

            [MethodImpl(AggressiveInlining)]
            internal void Initialize(uint chunksCapacity) {
                bitMask.Create(chunksCapacity, (ushort) (poolsCount.Normalize(Const.BITS_PER_LONG) >> Const.LONG_SHIFT));
                _freeChunkCommandBuffer.Create((uint) Entities.Value.chunks.Length, poolsCount);
                for (var i = 0; i < poolsCount; i++) {
                    _pools[i].Initialize(chunksCapacity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void RegisterComponentType<T>(IComponentConfig<T, WorldType> config) where T : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertNotRegisteredComponent<T>(WorldTypeName);
                #endif

                if (poolsCount == _pools.Length) {
                    Array.Resize(ref _pools, poolsCount << 1);
                }

                _pools[poolsCount] = new ComponentsWrapper<T>();
                _poolByType[typeof(T)] = new ComponentsWrapper<T>();
                Components<T>.Value.Create(poolsCount, config, _freeChunkCommandBuffer, bitMask);
                #if FFS_ECS_EVENTS
                Components<T>.Value.debugEventListeners = _debugEventListeners;
                #endif
                poolsCount++;

                Serializer.Value.RegisterComponentType(config);
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
            internal IComponentsWrapper GetPool(Type componentType) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                Assert(WorldTypeName, _poolByType.ContainsKey(componentType), $"Component type {componentType} not registered");
                #endif
                return _poolByType[componentType];
            }

            [MethodImpl(AggressiveInlining)]
            internal ComponentsWrapper<T> GetPool<T>() where T : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                AssertRegisteredComponent<T>(WorldTypeName);
                #endif
                return new ComponentsWrapper<T>();
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(Type componentType, out IComponentsWrapper pool) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                return _poolByType.TryGetValue(componentType, out pool);
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool<T>(out ComponentsWrapper<T> pool) where T : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                pool = default;
                return Components<T>.Value.IsRegistered();
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort ComponentsCount(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                return bitMask.Len(eid);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void ToPrettyStringEntity(StringBuilder builder, Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                builder.AppendLine("Components:");
                
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
            internal void GetAllComponents(Entity entity, List<IComponent> result) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                result.Clear();
                
                var maskLen = bitMask.MaskLen;
                var eid = entity.id - Const.ENTITY_ID_OFFSET;
                var masks = bitMask.Chunk(eid);
                var start = (eid & Const.ENTITIES_IN_CHUNK_OFFSET_MASK) * maskLen;
                for (ushort i = 0; i < maskLen; i++) {
                    var mask = masks[start + i];
                    var offset = i << Const.LONG_SHIFT;
                    while (mask > 0) {
                        var id = Utils.PopLsb(ref mask) + offset;
                        result.Add(_pools[id].GetRaw(entity));
                    }
                }
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
                            _pools[id].DeleteInternalWithoutMask(entity);
                        }
                    }
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void CopyEntity(Entity srcEntity, Entity dstEntity) {
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
                    for (uint i = 0, iMax = poolsCount; i < iMax; i++) {
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

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                for (var i = 0; i < poolsCount; i++) {
                    _pools[i].Destroy();
                }

                bitMask.Destroy();
                bitMask = default;
                _freeChunkCommandBuffer = default;
                _pools = default;
                _poolByType = default;
                poolsCount = default;
                Serializer.Value.Destroy();
                #if FFS_ECS_EVENTS
                _debugEventListeners = null;
                #endif
            }
        }
        
        #if FFS_ECS_EVENTS
        public interface IComponentsDebugEventListener {
            void OnComponentRef<T>(Entity entity, ref T component) where T : struct, IComponent;
            void OnComponentAdd<T>(Entity entity, ref T component) where T : struct, IComponent;
            void OnComponentPut<T>(Entity entity, ref T component) where T : struct, IComponent;
            void OnComponentDelete<T>(Entity entity, ref T component) where T : struct, IComponent;
        }
        #endif
    }
    
    public struct DeleteComponentsSystem<WorldType, T> : IUpdateSystem where T : struct, IComponent where WorldType : struct, IWorldType {
    
        [MethodImpl(AggressiveInlining)]
        public void Update() {
            World<WorldType>.Query.Entities<All<T>>().DeleteForAll<T>();
        }
    }
}