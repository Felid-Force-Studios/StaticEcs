using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    public interface IWorldType { }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppEagerStaticClassConstruction]
    #endif
    public abstract partial class World<WorldType> {
        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
        internal static List<IWorldDebugEventListener> _debugEventListeners;
        #endif

        internal static WorldConfig cfg;
        internal static ushort runtimeVersion;
        public static WorldStatus Status { get; private set; }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        internal static bool GidStoreLoaded;
        internal static MultiThreadStatus MTStatus = new();
        internal static bool MultiThreadActive {
            get => MTStatus.Active;
            set => MTStatus.Active = value;
        }
        #endif

        public static void Create(WorldConfig config) {
            if (Status != WorldStatus.NotCreated) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: Create, World already created");
            }
            cfg = config;
            Entity.CreateEntities();
            ModuleComponents.Value.Create(cfg.BaseComponentTypesCount);
            ModuleStandardComponents.Value.Create(cfg.BaseStandardComponentTypesCount);
            #if !FFS_ECS_DISABLE_TAGS
            ModuleTags.Value.Create(cfg.BaseTagTypesCount);
            #endif
            #if !FFS_ECS_DISABLE_MASKS
            ModuleMasks.Value.Create(cfg.BaseMaskTypesCount);
            #endif
            Serializer.Create();
            Status = WorldStatus.Created;

            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            MultiThreadActive = false;
            GidStoreLoaded = false;
            #endif
            
            #if !FFS_ECS_DISABLE_EVENTS
            Events.Create();
            #endif
            IncrementRuntimeVersion();
            
            BinaryPack.RegisterWithCollections<EntityGID, UnmanagedPackArrayStrategy<EntityGID>>(EntityGIDSerializer.WriteEntityGID, EntityGIDSerializer.ReadEntityGID);

            if (!BinaryPack.IsRegistered<GIDStoreSnapshot>()) {
                BinaryPack.Register(GIDStoreSnapshot.Write, GIDStoreSnapshot.Read);
            }
            
            ModuleStandardComponents.Value.RegisterComponentType(new EntityStatusConfig<WorldType>(), false);
        }

        public static void Initialize() {
            if (Status != WorldStatus.Created) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: Create, World not created");
            }

            Initialize(new GIDStore().Create(cfg));
        }

        public static void Initialize(GIDStoreSnapshot gidStoreSnapshot) {
            if (Status != WorldStatus.Created) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: Create, World not created");
            }
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            GidStoreLoaded = true;
            #endif
            Initialize(new GIDStore(gidStoreSnapshot));
        }

        internal static void Initialize(GIDStore gidStore) {
            if (Status != WorldStatus.Created) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: Create, World not created");
            }

            Entity.InitializeEntities(ref gidStore);
            ModuleComponents.Value.Initialize();
            ModuleStandardComponents.Value.Initialize();
            #if !FFS_ECS_DISABLE_TAGS
            ModuleTags.Value.Initialize();
            #endif
            #if !FFS_ECS_DISABLE_MASKS
            ModuleMasks.Value.Initialize();
            #endif
            Worlds.Set(typeof(WorldType), new WorldWrapper<WorldType>());
            ParallelRunner<WorldType>.Create(cfg);
            
            Status = WorldStatus.Initialized;
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            if (_debugEventListeners != null) {
                foreach (var listener in _debugEventListeners) {
                    listener.OnWorldInitialized();
                }
            }
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public static void Destroy() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: Destroy, World not initialized");
            #endif
            
            #if !FFS_ECS_DISABLE_EVENTS
            Events.Destroy();
            #endif

            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            if (_debugEventListeners != null) {
                for (var i = _debugEventListeners.Count - 1; i >= 0; i--) {
                    var listener = _debugEventListeners[i];
                    listener.OnWorldDestroyed();
                }
            }

            _debugEventListeners = null;
            #endif

            Entity.DestroyEntities();
            ModuleComponents.Value.Destroy();
            ModuleStandardComponents.Value.Destroy();
            #if !FFS_ECS_DISABLE_TAGS
            ModuleTags.Value.Destroy();
            #endif
            #if !FFS_ECS_DISABLE_MASKS
            ModuleMasks.Value.Destroy();
            #endif

            Status = WorldStatus.NotCreated;
            Worlds.Delete(typeof(WorldType));
            ParallelRunner<WorldType>.Destroy();

            cfg = default;
            Context.Value.Clear();
            NamedContext.Clear();
            Serializer.Destroy();
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            MultiThreadActive = false;
            FileLogger?.Disable();
            FileLogger = default;
            GidStoreLoaded = false;
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public static void DestroyAllEntities() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: DestroyAllEntities, World not initialized");
            #endif
            for (var i = Entity.entitiesCount; i > 0; i--) {
                var entity = Entity.FromIdx(i - 1);
                if (GIDStore.Value.Has(entity)) {
                    entity.Destroy();
                }
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public static void Clear() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: Clear, World not initialized");
            #endif

            ModuleComponents.Value.Clear();
            ModuleStandardComponents.Value.Clear();
            #if !FFS_ECS_DISABLE_TAGS
            ModuleTags.Value.Clear();
            #endif
            #if !FFS_ECS_DISABLE_MASKS
            ModuleMasks.Value.Clear();
            #endif
            Entity.ClearEntities();
            
            #if !FFS_ECS_DISABLE_EVENTS
            Events.Clear();
            #endif
            IncrementRuntimeVersion();
        }

        [MethodImpl(AggressiveInlining)]
        private static void IncrementRuntimeVersion() {
            if (runtimeVersion == ushort.MaxValue) {
                runtimeVersion = 1;
            } else {
                runtimeVersion++;
            }
        }

        [MethodImpl(AggressiveInlining)]
        public static bool IsWorldInitialized() => Status == WorldStatus.Initialized;

        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
        [MethodImpl(AggressiveInlining)]
        public static void AddWorldDebugEventListener(IWorldDebugEventListener listener) {
            _debugEventListeners ??= new List<IWorldDebugEventListener>();
            _debugEventListeners.Add(listener);
        }

        [MethodImpl(AggressiveInlining)]
        public static void RemoveWorldDebugEventListener(IWorldDebugEventListener listener) {
            _debugEventListeners?.Remove(listener);
        }

        public interface IWorldDebugEventListener {
            void OnEntityCreated(Entity entity);
            void OnEntityDestroyed(Entity entity);
            void OnWorldInitialized();
            void OnWorldDestroyed();
            void OnWorldResized(uint capacity);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppEagerStaticClassConstruction]
    #endif
    public static class Worlds {
        internal static readonly Dictionary<Type, IWorld> _worlds = new();

        [MethodImpl(AggressiveInlining)]
        public static IWorld Get(Type WorldTypeType) => _worlds[WorldTypeType];

        [MethodImpl(AggressiveInlining)]
        public static bool Has(Type WorldTypeType) => _worlds.ContainsKey(WorldTypeType);

        [MethodImpl(AggressiveInlining)]
        public static IReadOnlyCollection<IWorld> GetAll() => _worlds.Values;

        [MethodImpl(AggressiveInlining)]
        internal static void Set(Type WorldTypeType, IWorld world) => _worlds[WorldTypeType] = world;

        [MethodImpl(AggressiveInlining)]
        internal static void Delete(Type WorldTypeType) => _worlds.Remove(WorldTypeType);
    }

    public enum WorldStatus {
        NotCreated,
        Created,
        Initialized
    }

    public struct WorldConfig {
        public uint BaseEntitiesCount;
        public uint BaseDeletedEntitiesCount;
        public uint BaseStandardComponentTypesCount;
        public uint BaseComponentTypesCount;
        public uint BaseMaskTypesCount;
        public uint BaseTagTypesCount;
        public uint BaseEventPoolCount;
        public ParallelQueryType ParallelQueryType;
        public uint CustomThreadCount;

        public static WorldConfig Default() =>
            new() {
                BaseEntitiesCount = 256,
                BaseDeletedEntitiesCount = 256,
                BaseStandardComponentTypesCount = 4,
                BaseComponentTypesCount = 64,
                BaseMaskTypesCount = 64,
                BaseTagTypesCount = 64,
                BaseEventPoolCount = 32,
                ParallelQueryType = ParallelQueryType.Disabled
            };
    }
    
    public enum ParallelQueryType {
        Disabled,
        MaxThreadsCount,
        CustomThreadsCount
    }
    
    #if DEBUG || FFS_ECS_ENABLE_DEBUG
    internal class MultiThreadStatus {
        public bool Active;
    }
    #endif
}