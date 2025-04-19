using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public interface IStandardComponent { }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {

        [MethodImpl(AggressiveInlining)]
        public static void RegisterStandardComponentType<T>(IStandardComponentConfig<T, WorldType> config = null)
            where T : struct, IStandardComponent {
            if (Status != WorldStatus.Created) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: RegisterStandardComponentType<{typeof(T)}>, World not created");
            }
            config ??= DefaultStandardComponentConfig<T, WorldType>.Default;
            ModuleStandardComponents.Value.RegisterComponentType(config, true);
        }

        [MethodImpl(AggressiveInlining)]
        public static IStandardComponentsWrapper GetStandardComponentsPool(Type componentType) {
            return ModuleStandardComponents.Value.GetPool(componentType);
        }

        [MethodImpl(AggressiveInlining)]
        public static StandardComponentsWrapper<T> GetStandardComponentsPool<T>() where T : struct, IStandardComponent {
            return ModuleStandardComponents.Value.GetPool<T>();
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetStandardComponentsPool(Type componentType, out IStandardComponentsWrapper pool) {
            return ModuleStandardComponents.Value.TryGetPool(componentType, out pool);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetStandardComponentsPool<T>(out StandardComponentsWrapper<T> pool) where T : struct, IStandardComponent {
            return ModuleStandardComponents.Value.TryGetPool(out pool);
        }
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
        [MethodImpl(AggressiveInlining)]
        public static void AddStandardComponentsDebugEventListener(IStandardComponentsDebugEventListener listener) {
            ModuleStandardComponents.Value._debugEventListeners ??= new List<IStandardComponentsDebugEventListener>();
            ModuleStandardComponents.Value._debugEventListeners.Add(listener);
        }

        [MethodImpl(AggressiveInlining)]
        public static void RemoveStandardComponentsDebugEventListener(IStandardComponentsDebugEventListener listener) {
            ModuleStandardComponents.Value._debugEventListeners?.Remove(listener);
        }
        #endif
        
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        internal partial struct ModuleStandardComponents {
            public static ModuleStandardComponents Value;
            
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal List<IStandardComponentsDebugEventListener> _debugEventListeners;
            #endif
            
            private IStandardComponentsWrapper[] _pools;
            private IStandardComponentsWrapper[] _publicPools;
            private IStandardComponentsWrapper[] _poolsWithAutoInit;
            private IStandardComponentsWrapper[] _poolsWithAutoReset;
            private Dictionary<Type, IStandardComponentsWrapper> _poolIdxByType;
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
                _poolIdxByType = new Dictionary<Type, IStandardComponentsWrapper>();
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                _debugEventListeners ??= new List<IStandardComponentsDebugEventListener>();
                #endif
                Serializer.Value.Create();
            }

            [MethodImpl(AggressiveInlining)]
            internal void Initialize() { }

            [MethodImpl(AggressiveInlining)]
            internal void RegisterComponentType<T>(IStandardComponentConfig<T, WorldType> config, bool publicPool) where T : struct, IStandardComponent {
                if (StandardComponents<T>.Value.IsRegistered()) throw new StaticEcsException($"StandardComponents {typeof(T)} already registered");

                if (_poolsCount == _pools.Length) {
                    Array.Resize(ref _pools, _poolsCount << 1);
                }

                _pools[_poolsCount] = new StandardComponentsWrapper<T>();
                _poolIdxByType[typeof(T)] = new StandardComponentsWrapper<T>();;
                StandardComponents<T>.Value.Create(config, _poolsCount);
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                StandardComponents<T>.Value.debugEventListeners = _debugEventListeners;
                #endif
                SetOnAddHandler(config.OnAdd());
                SetOnDeleteHandler(config.OnDelete());
                SetOnCopyHandler(config.OnCopy());

                if (publicPool) {
                    if (_publicPoolsCount == _publicPools.Length) {
                        Array.Resize(ref _publicPools, _publicPoolsCount << 1);
                    }
                    _publicPools[_publicPoolsCount++] = _pools[_poolsCount];
                }
                _poolsCount++;

                Serializer.Value.RegisterComponentType(config);
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void SetOnAddHandler<T>(OnComponentHandler<T> handler) where T : struct, IStandardComponent {
                if (handler != null && StandardComponents<T>.Value.SetAutoInit(handler)) {
                    AddAutoInitPool(StandardComponents<T>.Value.id);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void SetOnDeleteHandler<T>(OnComponentHandler<T> handler) where T : struct, IStandardComponent {
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
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetAllRawsPools, World not initialized");
                #endif
                var pools = new List<IStandardRawPool>();
                for (int i = 0; i < _poolsCount; i++) {
                    pools.Add(_pools[i]);
                }
                return pools;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal IStandardComponentsWrapper GetPool(Type componentType) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetPool, World not initialized");
                if (!_poolIdxByType.ContainsKey(componentType)) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetPool, Component type {componentType} not registered");
                #endif
                return _poolIdxByType[componentType];
            }

            [MethodImpl(AggressiveInlining)]
            internal StandardComponentsWrapper<T> GetPool<T>() where T : struct, IStandardComponent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, World not initialized");
                if (!StandardComponents<T>.Value.IsRegistered()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, Component type not registered");
                #endif
                return default;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool(Type componentType, out IStandardComponentsWrapper pool) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetPool, World not initialized");
                #endif
                return _poolIdxByType.TryGetValue(componentType, out pool);
            }

            [MethodImpl(AggressiveInlining)]
            internal bool TryGetPool<T>(out StandardComponentsWrapper<T> pool) where T : struct, IStandardComponent {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetPool<{typeof(T)}>, World not initialized");
                #endif
                pool = default;
                return StandardComponents<T>.Value.IsRegistered();
            }

            [MethodImpl(AggressiveInlining)]
            internal ushort ComponentsCount() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: ComponentsCount, World not initialized");
                #endif
                return _poolsCount;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void ToPrettyStringEntity(StringBuilder builder, Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: ToPrettyStringEntity, World not initialized");
                #endif
                builder.AppendLine("Standard components:");
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].ToStringComponent(builder, entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void GetAllComponents(Entity entity, List<IStandardComponent> result) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetComponents, World not initialized");
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
                for (var i = 0; i < _poolsCount; i++) {
                    _pools[i].Clear();
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
                Serializer.Value.Destroy();
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