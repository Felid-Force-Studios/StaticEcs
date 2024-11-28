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
    public abstract partial class Ecs<WorldID> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        internal struct ModuleComponents {
            public static ModuleComponents Value;
            
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<IComponentsDebugEventListener> _debugEventListeners;
            #endif
            
            internal BitMask BitMask;
            private IComponentsWrapper[] _pools;
            private Dictionary<Type, int> _poolIdxByType;
            private ushort _poolsCount;

            [MethodImpl(AggressiveInlining)]
            internal void Create(uint baseComponentsCapacity) {
                _pools = new IComponentsWrapper[baseComponentsCapacity];
                _poolIdxByType = new Dictionary<Type, int>();
                BitMask = new BitMask();
            }

            [MethodImpl(AggressiveInlining)]
            internal void Initialize() {
                BitMask.Create(World.EntitiesCapacity(), 32, Utils.CalculateMaskLen(_poolsCount));
            }

            [MethodImpl(AggressiveInlining)]
            internal ComponentDynId RegisterComponent<T>(uint capacity) where T : struct, IComponent {
                if (ComponentInfo<T>.IsRegistered()) {
                    return Components<T>.Value.DynamicId();
                }

                if (_poolsCount == _pools.Length) {
                    Array.Resize(ref _pools, _poolsCount << 1);
                }

                _pools[_poolsCount] = new ComponentsWrapper<T>();
                _poolIdxByType[typeof(T)] = _poolsCount;
                Components<T>.Value.Create(_poolsCount, BitMask, capacity);
                _poolsCount++;
                
                return Components<T>.Value.DynamicId();
            }
            
            [MethodImpl(AggressiveInlining)]
            internal IComponentsWrapper GetPool(ComponentDynId id) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: GetPool, World not initialized");
                if (id.Value >= _poolsCount) throw new Exception($"World<{typeof(WorldID)}>, Method: GetPool, Component type for dyn id {id.Value} not registered");
                #endif
                return _pools[id.Value];
            }
            
            [MethodImpl(AggressiveInlining)]
            internal IComponentsWrapper GetPool(Type componentType) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: GetPool, World not initialized");
                if (!_poolIdxByType.ContainsKey(componentType)) throw new Exception($"World<{typeof(WorldID)}>, Method: GetPool, Component type {componentType} not registered");
                #endif
                return _pools[_poolIdxByType[componentType]];
            }

            [MethodImpl(AggressiveInlining)]
            internal ComponentsWrapper<T> GetPool<T>() where T : struct, IComponent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: GetPool<{typeof(T)}>, World not initialized");
                if (!ComponentInfo<T>.IsRegistered()) throw new Exception($"World<{typeof(WorldID)}>, Method: GetPool<{typeof(T)}>, Component type not registered");
                #endif
                return default;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(ComponentDynId id, out IComponentsWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: GetPool, World not initialized");
                #endif
                if (id.Value >= _poolsCount) {
                    pool = default;
                    return false;
                }
                
                pool = _pools[id.Value];
                return true;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(Type componentType, out IComponentsWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: GetPool, World not initialized");
                #endif
                if (!_poolIdxByType.TryGetValue(componentType, out var idx)) {
                    pool = default;
                    return false;
                }
                
                pool = _pools[idx];
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool<T>(out ComponentsWrapper<T> pool) where T : struct, IComponent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: GetPool<{typeof(T)}>, World not initialized");
                #endif
                pool = default;
                return ComponentInfo<T>.IsRegistered();
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort ComponentsCount(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: ComponentsCount, World not initialized");
                #endif
                return BitMask.Len(entity._id);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal string ToPrettyStringEntity(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: ToPrettyStringEntity, World not initialized");
                #endif
                var str = "Components:\n";
                var bufId = BitMask.BorrowBuf();
                BitMask.CopyToBuffer(entity._id, bufId);
                var id = BitMask.GetMinIndexBuffer(bufId);
                while (id >= 0) {
                    str += _pools[id].ToStringComponent(entity);
                    BitMask.DelInBuffer(bufId, (ushort) id);
                    id = BitMask.GetMinIndexBuffer(bufId);
                }
                BitMask.DropBuf();
                return str;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void GetAllComponents(Entity entity, List<IComponent> result) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: GetComponents, World not initialized");
                #endif
                result.Clear();
                var bufId = BitMask.BorrowBuf();
                BitMask.CopyToBuffer(entity._id, bufId);
                var id = BitMask.GetMinIndexBuffer(bufId);
                while (id >= 0) {
                    result.Add(_pools[id].GetRaw(entity));
                    BitMask.DelInBuffer(bufId, (ushort) id);
                    id = BitMask.GetMinIndexBuffer(bufId);
                }
                BitMask.DropBuf();
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void DestroyEntity(Entity entity) {
                var id = BitMask.GetMinIndex(entity._id);
                while (id >= 0) {
                    _pools[id].DeleteFromWorld(entity);
                    id = BitMask.GetMinIndex(entity._id);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void CopyEntity(Entity srcEntity, Entity dstEntity) {
                var bufId = BitMask.BorrowBuf();
                BitMask.CopyToBuffer(srcEntity._id, bufId);
                var id = BitMask.GetMinIndexBuffer(bufId);
                while (id >= 0) {
                    _pools[id].Copy(srcEntity, dstEntity);
                    BitMask.DelInBuffer(bufId, (ushort) id);
                    id = BitMask.GetMinIndexBuffer(bufId);
                }

                BitMask.DropBuf();
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Resize(int size) {
                for (int i = 0, iMax = _poolsCount; i < iMax; i++) {
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
                _pools = default;
                _poolIdxByType = default;
                _poolsCount = default;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners = null;
                #endif
            }

            #if ENABLE_IL2CPP
            [Il2CppEagerStaticClassConstruction]
            #endif
            public struct ComponentInfo<T> where T : struct, IComponent {
                private static bool Registered;

                [MethodImpl(AggressiveInlining)]
                internal static void Register() => Registered = true;

                [MethodImpl(AggressiveInlining)]
                internal static void Reset() => Registered = false;

                [MethodImpl(AggressiveInlining)]
                public static bool IsRegistered() => Registered;
            }
        }
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
        public interface IComponentsDebugEventListener {
            void OnComponentRef<T>(Entity entity, ref T component) where T : struct, IComponent;
            void OnComponentRefMut<T>(Entity entity, ref T component) where T : struct, IComponent;
            void OnComponentAdd<T>(Entity entity, ref T component) where T : struct, IComponent;
            void OnComponentPut<T>(Entity entity, ref T component) where T : struct, IComponent;
            void OnComponentDelete<T>(Entity entity, ref T component) where T : struct, IComponent;
        }
        #endif
    }
    
    public struct DeleteComponentsSystem<WorldId, T> : IUpdateSystem where T : struct, IComponent where WorldId : struct, IWorldId {
        [MethodImpl(AggressiveInlining)]
        public void Update() {
            Ecs<WorldId>.World.QueryEntities.For<All<T>>().DeleteForAll<T>();
        }
    }
}