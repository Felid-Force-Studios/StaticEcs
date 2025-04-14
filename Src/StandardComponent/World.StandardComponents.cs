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
        internal struct ModuleStandardComponents {
            public static ModuleStandardComponents Value;
            
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<IStandardComponentsDebugEventListener> _debugEventListeners;
            #endif
            
            private IStandardComponentsWrapper[] _pools;
            private IStandardComponentsWrapper[] _publicPools;
            private IStandardComponentsWrapper[] _poolsWithAutoInit;
            private IStandardComponentsWrapper[] _poolsWithAutoReset;
            private Dictionary<Type, int> _poolIdxByType;
            private ushort _poolsCount;
            private ushort _publicPoolsCount;
            private ushort _poolsWithAutoInitCount;
            private ushort _poolsWithAutoResetCount;

            [MethodImpl(AggressiveInlining)]
            internal void Create(uint componentsCapacity) {
                _pools = new IStandardComponentsWrapper[componentsCapacity];
                _poolsWithAutoInit = new IStandardComponentsWrapper[componentsCapacity];
                _poolsWithAutoReset = new IStandardComponentsWrapper[componentsCapacity];
                _publicPools = new IStandardComponentsWrapper[componentsCapacity];
                _poolIdxByType = new Dictionary<Type, int>();
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners ??= new List<IStandardComponentsDebugEventListener>();
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal void Initialize() { }

            [MethodImpl(AggressiveInlining)]
            internal StandardComponentDynId RegisterComponentType<T>(bool publicPool, OnAddHandler<T> onAdd = null, OnDeleteHandler<T> onDelete = null, OnCopyHandler<T> onCopy = null) where T : struct, IStandardComponent {
                if (StandardComponents<T>.Value.IsRegistered()) {
                    return StandardComponents<T>.Value.DynamicId();
                }

                if (_poolsCount == _pools.Length) {
                    Array.Resize(ref _pools, _poolsCount << 1);
                }

                _pools[_poolsCount] = new StandardComponentsWrapper<T>();
                _poolIdxByType[typeof(T)] = _poolsCount;
                StandardComponents<T>.Value.Create(_poolsCount);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                StandardComponents<T>.Value.debugEventListeners = _debugEventListeners;
                #endif
                SetOnAddHandler(onAdd);
                SetOnDeleteHandler(onDelete);
                SetOnCopyHandler(onCopy);

                if (publicPool) {
                    if (_publicPoolsCount == _publicPools.Length) {
                        Array.Resize(ref _publicPools, _publicPoolsCount << 1);
                    }
                    _publicPools[_publicPoolsCount++] = _pools[_poolsCount];
                }
                _poolsCount++;

                return StandardComponents<T>.Value.DynamicId();
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void SetOnAddHandler<T>(OnAddHandler<T> handler) where T : struct, IStandardComponent {
                if (handler != null && StandardComponents<T>.Value.SetAutoInit(handler)) {
                    AddAutoInitPool(StandardComponents<T>.Value.id);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void SetOnDeleteHandler<T>(OnDeleteHandler<T> handler) where T : struct, IStandardComponent {
                if (handler != null && StandardComponents<T>.Value.SetAutoReset(handler)) {
                    AddAutoResetPool(StandardComponents<T>.Value.id);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void SetOnCopyHandler<T>(OnCopyHandler<T> handler) where T : struct, IStandardComponent {
                if (handler != null) {
                    StandardComponents<T>.Value.SetAutoCopy(handler);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal List<IStandardRawPool> GetAllRawsPools() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetAllRawsPools, World not initialized");
                #endif
                var pools = new List<IStandardRawPool>();
                for (int i = 0; i < _poolsCount; i++) {
                    pools.Add(_pools[i]);
                }
                return pools;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal IStandardComponentsWrapper GetPool(StandardComponentDynId id) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool, World not initialized");
                if (id.Value >= _poolsCount) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool, Component type for dyn id {id.Value} not registered");
                #endif
                return _pools[id.Value];
            }
            
            [MethodImpl(AggressiveInlining)]
            internal IStandardComponentsWrapper GetPool(Type componentType) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool, World not initialized");
                if (!_poolIdxByType.ContainsKey(componentType)) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool, Component type {componentType} not registered");
                #endif
                return _pools[_poolIdxByType[componentType]];
            }

            [MethodImpl(AggressiveInlining)]
            internal StandardComponentsWrapper<T> GetPool<T>() where T : struct, IStandardComponent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, World not initialized");
                if (!StandardComponents<T>.Value.IsRegistered()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, Component type not registered");
                #endif
                return default;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(StandardComponentDynId id, out IStandardComponentsWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool, World not initialized");
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
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool, World not initialized");
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
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, World not initialized");
                #endif
                pool = default;
                return StandardComponents<T>.Value.IsRegistered();
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort ComponentsCount() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: ComponentsCount, World not initialized");
                #endif
                return _poolsCount;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal string ToPrettyStringEntity(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: ToPrettyStringEntity, World not initialized");
                #endif
                var str = "Standard components:\n";
                for (var i = 0; i < _poolsCount; i++) {
                    str += _pools[i].ToStringComponent(entity);
                }
                return str;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void GetAllComponents(Entity entity, List<IStandardComponent> result) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: GetComponents, World not initialized");
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
                for (var i = 0; i < _publicPoolsCount; i++) {
                    _publicPools[i].Copy(srcEntity, dstEntity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Resize(uint size) {
                for (int i = 0, iMax = _poolsCount; i < iMax; i++) {
                    _pools[i].Resize(size);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void Clear() {
                for (var i = 0; i < _publicPoolsCount; i++) {
                    _publicPools[i].Clear();
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void AddAutoInitPool(int poolId) {
                if (_poolsWithAutoInitCount == _poolsWithAutoInit.Length) {
                    Array.Resize(ref _poolsWithAutoInit, _poolsWithAutoInitCount << 1);
                }
                _poolsWithAutoInit[_poolsWithAutoInitCount++] = _pools[poolId];
            }
                
            [MethodImpl(AggressiveInlining)]
            internal void AddAutoResetPool(int poolId) {
                if (_poolsWithAutoResetCount == _poolsWithAutoReset.Length) {
                    Array.Resize(ref _poolsWithAutoReset, _poolsWithAutoResetCount << 1);
                }
                _poolsWithAutoReset[_poolsWithAutoResetCount++] = _pools[poolId];
            }

            [MethodImpl(AggressiveInlining)]
            internal void Destroy() {
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Destroy();
                }

                _pools = default;
                _poolsWithAutoInit = default;
                _publicPools = default;
                _publicPoolsCount = default;
                _poolsWithAutoReset = default;
                _poolsWithAutoInitCount = default;
                _poolsWithAutoResetCount = default;
                _poolIdxByType = default;
                _poolsCount = default;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners = null;
                #endif
            }
        }
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
        public interface IStandardComponentsDebugEventListener {
            void OnComponentRef<T>(Entity entity, ref T component) where T : struct, IStandardComponent;
        }
        #endif
    }
}