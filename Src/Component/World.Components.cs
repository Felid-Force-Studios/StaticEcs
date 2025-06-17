using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
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
    public abstract partial class World<WorldType> {

        [MethodImpl(AggressiveInlining)]
        public static void RegisterComponentType<T>(IComponentConfig<T, WorldType> config = null, uint basePoolCapacity = 128)
            where T : struct, IComponent {
            if (Status != WorldStatus.Created) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: RegisterComponentType<{typeof(T)}>, World not created");
            }

            config ??= DefaultComponentConfig<T, WorldType>.Default;
            ModuleComponents.Value.RegisterComponentType(basePoolCapacity, config);
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
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
        [MethodImpl(AggressiveInlining)]
        public static void AddComponentsDebugEventListener(IComponentsDebugEventListener listener) {
            ModuleComponents.Value._debugEventListeners ??= new List<IComponentsDebugEventListener>();
            ModuleComponents.Value._debugEventListeners.Add(listener);
        }

        [MethodImpl(AggressiveInlining)]
        public static void RemoveComponentsDebugEventListener(IComponentsDebugEventListener listener) {
            ModuleComponents.Value._debugEventListeners?.Remove(listener);
        }
        #endif
        
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        internal partial struct ModuleComponents {
            public static ModuleComponents Value;
            
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<IComponentsDebugEventListener> _debugEventListeners;
            #endif
            
            internal BitMask BitMask;
            internal int[] TempIndexes;
            private IComponentsWrapper[] _pools;
            private Dictionary<Type, IComponentsWrapper> _poolByType;
            private ushort _poolsCount;

            [MethodImpl(AggressiveInlining)]
            internal void Create(uint baseComponentsCapacity) {
                _pools = new IComponentsWrapper[baseComponentsCapacity];
                _poolByType = new Dictionary<Type, IComponentsWrapper>();
                BitMask = new BitMask();
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners ??= new List<IComponentsDebugEventListener>();
                #endif
                Serializer.Value.Create();
            }

            [MethodImpl(AggressiveInlining)]
            internal void Initialize() {
                BitMask.Create(EntitiesCapacity(), 32, Utils.CalculateMaskLen(_poolsCount), true);
                TempIndexes = new int[_poolsCount];
            }

            [MethodImpl(AggressiveInlining)]
            internal void RegisterComponentType<T>(uint capacity,
                                                   IComponentConfig<T, WorldType> config) where T : struct, IComponent {
                if (Components<T>.Value.IsRegistered()) throw new StaticEcsException($"Component {typeof(T)} already registered");

                if (_poolsCount == _pools.Length) {
                    Array.Resize(ref _pools, _poolsCount << 1);
                }

                _pools[_poolsCount] = new ComponentsWrapper<T>();
                _poolByType[typeof(T)] = new ComponentsWrapper<T>();
                Components<T>.Value.Create(_poolsCount, BitMask, EntitiesCapacity(), config, capacity);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                Components<T>.Value.debugEventListeners = _debugEventListeners;
                #endif
                _poolsCount++;

                Serializer.Value.RegisterComponentType(config);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal List<IRawPool> GetAllRawsPools() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetAllRawsPools, World not initialized");
                #endif
                var pools = new List<IRawPool>();
                for (int i = 0; i < _poolsCount; i++) {
                    pools.Add(_pools[i]);
                }
                return pools;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal IComponentsWrapper GetPool(Type componentType) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetPool, World not initialized");
                if (!_poolByType.ContainsKey(componentType)) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetPool, Component type {componentType} not registered");
                #endif
                return _poolByType[componentType];
            }

            [MethodImpl(AggressiveInlining)]
            internal ComponentsWrapper<T> GetPool<T>() where T : struct, IComponent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, World not initialized");
                if (!Components<T>.Value.IsRegistered()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, Component type not registered");
                #endif
                return default;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(Type componentType, out IComponentsWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetPool, World not initialized");
                #endif
                return _poolByType.TryGetValue(componentType, out pool);
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool<T>(out ComponentsWrapper<T> pool) where T : struct, IComponent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, World not initialized");
                #endif
                pool = default;
                return Components<T>.Value.IsRegistered();
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort ComponentsCount(Entity entity, bool withDisabled) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: ComponentsCount, World not initialized");
                #endif
                return withDisabled ? BitMask.LenWithDisabled(entity._id) : BitMask.Len(entity._id);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void ToPrettyStringEntity(StringBuilder builder, Entity entity, bool withDisabled) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: ToPrettyStringEntity, World not initialized");
                #endif
                builder.AppendLine("Components:");
                var bufId = BitMask.BorrowBuf();
                if (withDisabled) {
                    BitMask.CopyWithDisabledToBuffer(entity._id, bufId);
                } else {
                    BitMask.CopyToBuffer(entity._id, bufId);
                }
                while (BitMask.GetMinIndexBuffer(bufId, out var id)) {
                    _pools[id].ToStringComponent(builder, entity);
                    BitMask.DelInBuffer(bufId, (ushort) id);
                }
                BitMask.DropBuf();
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void GetAllComponents(Entity entity, List<IComponent> result, bool withDisabled) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetComponents, World not initialized");
                #endif
                result.Clear();
                var bufId = BitMask.BorrowBuf();
                if (withDisabled) {
                    BitMask.CopyWithDisabledToBuffer(entity._id, bufId);
                } else {
                    BitMask.CopyToBuffer(entity._id, bufId);
                }
                while (BitMask.GetMinIndexBuffer(bufId, out var id)) {
                    result.Add(_pools[id].GetRaw(entity));
                    BitMask.DelInBuffer(bufId, (ushort) id);
                }
                BitMask.DropBuf();
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void DestroyEntity(Entity entity) {
                BitMask.GetAndDelAllIndexesWithDisabled(entity._id, TempIndexes, out var count);
                for (var i = 0; i < count; i++) {
                    _pools[TempIndexes[i]].DeleteInternalWithoutMask(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void CopyEntity(Entity srcEntity, Entity dstEntity, bool withDisabled) {
                var bufId = BitMask.BorrowBuf();
                if (withDisabled) {
                    BitMask.CopyWithDisabledToBuffer(srcEntity._id, bufId);
                } else {
                    BitMask.CopyToBuffer(srcEntity._id, bufId);
                }
                while (BitMask.GetMinIndexBuffer(bufId, out var id)) {
                    _pools[id].Copy(srcEntity, dstEntity);
                    BitMask.DelInBuffer(bufId, (ushort) id);
                }
                BitMask.DropBuf();
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Resize(uint size) {
                for (uint i = 0, iMax = _poolsCount; i < iMax; i++) {
                    _pools[i].Resize(size);
                }
                BitMask.ResizeBitMap(size);
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Clear();
                }
                BitMask.Clear();
            }

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Destroy();
                }

                BitMask.Destroy();
                BitMask = default;
                TempIndexes = default;
                _pools = default;
                _poolByType = default;
                _poolsCount = default;
                Serializer.Value.Destroy();
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners = null;
                #endif
            }
        }
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
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
            World<WorldType>.QueryEntities.For<All<T>>().DeleteForAll<T>();
        }
    }
}