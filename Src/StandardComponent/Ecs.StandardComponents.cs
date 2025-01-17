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
        internal struct ModuleStandardComponents {
            public static ModuleStandardComponents Value;
            
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<IStandardComponentsDebugEventListener> _debugEventListeners;
            #endif
            
            private IStandardComponentsWrapper[] _pools;
            private IStandardComponentsWrapper[] _poolsWithAutoInit;
            private IStandardComponentsWrapper[] _poolsWithAutoReset;
            private Dictionary<Type, int> _poolIdxByType;
            private ushort _poolsCount;
            private ushort _poolsWithAutoInitCount;
            private ushort _poolsWithAutoResetCount;

            [MethodImpl(AggressiveInlining)]
            internal void Create(uint componentsCapacity) {
                _pools = new IStandardComponentsWrapper[componentsCapacity];
                _poolsWithAutoInit = new IStandardComponentsWrapper[componentsCapacity];
                _poolsWithAutoReset = new IStandardComponentsWrapper[componentsCapacity];
                _poolIdxByType = new Dictionary<Type, int>();
            }

            [MethodImpl(AggressiveInlining)]
            internal void Initialize() { }

            [MethodImpl(AggressiveInlining)]
            internal StandardComponentDynId RegisterComponentType<T>() where T : struct, IStandardComponent {
                if (StandardComponentInfo<T>.IsRegistered()) {
                    return StandardComponents<T>.Value.DynamicId();
                }

                if (_poolsCount == _pools.Length) {
                    Array.Resize(ref _pools, _poolsCount << 1);
                    Array.Resize(ref _poolsWithAutoInit, _poolsCount << 1);
                    Array.Resize(ref _poolsWithAutoReset, _poolsCount << 1);
                }

                _pools[_poolsCount] = new StandardComponentsWrapper<T>();
                _poolIdxByType[typeof(T)] = _poolsCount;
                StandardComponents<T>.Value.Create(_poolsCount);
                _poolsCount++;
                
                return StandardComponents<T>.Value.DynamicId();
            }
            
            [MethodImpl(AggressiveInlining)]
            internal IStandardComponentsWrapper GetPool(StandardComponentDynId id) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool, World not initialized");
                if (id.Value >= _poolsCount) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool, Component type for dyn id {id.Value} not registered");
                #endif
                return _pools[id.Value];
            }
            
            [MethodImpl(AggressiveInlining)]
            internal IStandardComponentsWrapper GetPool(Type componentType) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool, World not initialized");
                if (!_poolIdxByType.ContainsKey(componentType)) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool, Component type {componentType} not registered");
                #endif
                return _pools[_poolIdxByType[componentType]];
            }

            [MethodImpl(AggressiveInlining)]
            internal StandardComponentsWrapper<T> GetPool<T>() where T : struct, IStandardComponent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, World not initialized");
                if (!StandardComponentInfo<T>.IsRegistered()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, Component type not registered");
                #endif
                return default;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(StandardComponentDynId id, out IStandardComponentsWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool, World not initialized");
                #endif
                if (id.Value >= _poolsCount) {
                    pool = default;
                    return false;
                }
                
                pool = _pools[id.Value];
                return true;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(Type componentType, out IStandardComponentsWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool, World not initialized");
                #endif
                if (!_poolIdxByType.TryGetValue(componentType, out var idx)) {
                    pool = default;
                    return false;
                }
                
                pool = _pools[idx];
                return true;
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool<T>(out StandardComponentsWrapper<T> pool) where T : struct, IStandardComponent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, World not initialized");
                #endif
                pool = default;
                return StandardComponentInfo<T>.IsRegistered();
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort ComponentsCount() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: ComponentsCount, World not initialized");
                #endif
                return _poolsCount;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal string ToPrettyStringEntity(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: ToPrettyStringEntity, World not initialized");
                #endif
                var str = "Components:\n";
                for (var i = 0; i < _poolsCount; i++) {
                    str += _pools[i].ToStringComponent(entity);
                }
                return str;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void GetAllComponents(Entity entity, List<IStandardComponent> result) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!World.IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetComponents, World not initialized");
                #endif
                result.Clear();
                for (var i = 0; i < _poolsCount; i++) {
                    result.Add(_pools[i].GetRaw(entity));
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void InitEntity(Entity entity) {
                for (var i = 0; i < _poolsWithAutoInitCount; i++) {
                    _poolsWithAutoInit[i].AutoInit(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void DestroyEntity(Entity entity) {
                for (var i = 0; i < _poolsWithAutoResetCount; i++) {
                    _poolsWithAutoReset[i].AutoReset(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void CopyEntity(Entity srcEntity, Entity dstEntity) {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Copy(srcEntity, dstEntity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Resize(int size) {
                for (int i = 0, iMax = _poolsCount; i < iMax; i++) {
                    _pools[i].Resize(size);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Clear();
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void AddAutoInitPool(int poolId) {
                _poolsWithAutoInit[_poolsWithAutoInitCount++] = _pools[poolId];
            }
                
            [MethodImpl(AggressiveInlining)]
            internal void AddAutoResetPool(int poolId) {
                _poolsWithAutoReset[_poolsWithAutoResetCount++] = _pools[poolId];
            }

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Destroy();
                }

                _pools = default;
                _poolsWithAutoInit = default;
                _poolsWithAutoReset = default;
                _poolsWithAutoInitCount = default;
                _poolsWithAutoResetCount = default;
                _poolIdxByType = default;
                _poolsCount = default;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners = null;
                #endif
            }

            #if ENABLE_IL2CPP
            [Il2CppEagerStaticClassConstruction]
            #endif
            public struct StandardComponentInfo<T> where T : struct, IStandardComponent {
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
        public interface IStandardComponentsDebugEventListener {
            void OnComponentRef<T>(Entity entity, ref T component) where T : struct, IStandardComponent;
            void OnComponentRefMut<T>(Entity entity, ref T component) where T : struct, IStandardComponent;
        }
        #endif
    }
}