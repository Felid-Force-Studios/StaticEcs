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
        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
        internal static List<IWorldDebugEventListener> _debugEventListeners;
        #endif

        internal static WorldConfig cfg;
        public static WorldStatus Status { get; private set; }

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
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
            cfg.Normalize();
            
            Entities.Value.Create(cfg.baseEntitiesCapacity);
            ModuleComponents.Value.Create(cfg.BaseComponentTypesCount);
            #if !FFS_ECS_DISABLE_TAGS
            ModuleTags.Value.Create(cfg.BaseTagTypesCount);
            #endif
            Serializer.Create();
            Status = WorldStatus.Created;
            CurrentQuery.QueryDataCount = 0;

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            MultiThreadActive = false;
            GidStoreLoaded = false;
            CurrentQuery.QueryMode = 0;
            #endif
            
            #if !FFS_ECS_DISABLE_EVENTS
            Events.Create();
            #endif
            
            BinaryPack.RegisterWithCollections<EntityGID, UnmanagedPackArrayStrategy<EntityGID>>(EntityGIDSerializer.WriteEntityGID, EntityGIDSerializer.ReadEntityGID);

            if (!BinaryPack.IsRegistered<GIDStoreSnapshot>()) {
                BinaryPack.Register(GIDStoreSnapshot.Write, GIDStoreSnapshot.Read);
            }
        }

        public static void Initialize() {
            if (Status != WorldStatus.Created) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: Create, World not created");
            }

            Initialize(new GIDStore().Create(cfg.baseEntitiesCapacity));
        }

        public static void Initialize(GIDStoreSnapshot gidStoreSnapshot) {
            if (Status != WorldStatus.Created) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: Create, World not created");
            }
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            GidStoreLoaded = true;
            #endif
            Initialize(new GIDStore(gidStoreSnapshot));
        }

        internal static void Initialize(GIDStore gidStore) {
            if (Status != WorldStatus.Created) {
                throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: Create, World not created");
            }

            Entities.Value.Initialize(ref gidStore);
            ModuleComponents.Value.Initialize();
            #if !FFS_ECS_DISABLE_TAGS
            ModuleTags.Value.Initialize();
            #endif
            Worlds.Set(typeof(WorldType), new WorldWrapper<WorldType>());
            ParallelRunner<WorldType>.Create(cfg);
            
            Status = WorldStatus.Initialized;
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
            if (_debugEventListeners != null) {
                foreach (var listener in _debugEventListeners) {
                    listener.OnWorldInitialized();
                }
            }
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public static void Destroy() {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: Destroy, World not initialized");
            #endif
            
            #if !FFS_ECS_DISABLE_EVENTS
            Events.Destroy();
            #endif

            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
            if (_debugEventListeners != null) {
                for (var i = _debugEventListeners.Count - 1; i >= 0; i--) {
                    var listener = _debugEventListeners[i];
                    listener.OnWorldDestroyed();
                }
            }

            _debugEventListeners = null;
            #endif

            Entities.Value.Destroy();
            ModuleComponents.Value.Destroy();
            #if !FFS_ECS_DISABLE_TAGS
            ModuleTags.Value.Destroy();
            #endif

            Status = WorldStatus.NotCreated;
            Worlds.Delete(typeof(WorldType));
            ParallelRunner<WorldType>.Destroy();

            cfg = default;
            Context.Value.Clear();
            NamedContext.Clear();
            Serializer.Destroy();
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            MultiThreadActive = false;
            FileLogger?.Disable();
            FileLogger = default;
            GidStoreLoaded = false;
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public static void DestroyAllEntities() {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: DestroyAllEntities, World not initialized");
            #endif
            Query.For(ent => ent.Destroy(), EntityStatusType.Any, QueryMode.Flexible);
        }
        
        [MethodImpl(AggressiveInlining)]
        public static void Clear() {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: Clear, World not initialized");
            #endif

            ModuleComponents.Value.Clear();
            #if !FFS_ECS_DISABLE_TAGS
            ModuleTags.Value.Clear();
            #endif
            Entities.Value.Clear();
            
            #if !FFS_ECS_DISABLE_EVENTS
            Events.Clear();
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public static bool IsWorldInitialized() => Status == WorldStatus.Initialized;

        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
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
        public uint baseEntitiesCapacity;
        public uint BaseComponentTypesCount;
        public uint BaseTagTypesCount;
        public ParallelQueryType ParallelQueryType;
        public uint CustomThreadCount;
        public bool DefaultQueryModeStrict;

        public static WorldConfig Default() => new() {
                baseEntitiesCapacity = Const.ENTITIES_IN_CHUNK,
                BaseComponentTypesCount = 64,
                BaseTagTypesCount = 64,
                ParallelQueryType = ParallelQueryType.Disabled,
                CustomThreadCount = 1,
                DefaultQueryModeStrict = true
            };
        
        internal void Normalize() {
            baseEntitiesCapacity.NormalizeThis(Const.ENTITIES_IN_CHUNK);
        }
    }
    
    public enum ParallelQueryType {
        Disabled,
        MaxThreadsCount,
        CustomThreadsCount
    }
    
    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
    internal class MultiThreadStatus {
        public bool Active;
    }
    #endif
}