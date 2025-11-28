#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

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
        #if FFS_ECS_EVENTS
        private static List<IWorldDebugEventListener> WorldDebugEventListeners;
        #endif

        internal static WorldConfig config;
        public static WorldStatus Status { get; private set; }

        internal static volatile bool MultiThreadActive;

        public static void Create(WorldConfig worldConfig) {
            #if FFS_ECS_DEBUG
            AssertWorldIsNotCreated(WorldTypeName);
            #endif

            config = worldConfig.Normalize();

            Entities.Value.Create(config.Independent, config.BaseClustersCapacity);
            ModuleComponents.Value.Create(config.BaseComponentTypesCount);
            ModuleTags.Value.Create(config.BaseTagTypesCount);
            Events.Create();
            Serializer.Create();
            CurrentQuery.QueryDataCount = 0;
            ParallelRunner<WorldType>.Create(config.ParallelQueryType, config.CustomThreadCount);
            MultiThreadActive = false;

            #if FFS_ECS_DEBUG
            CurrentQuery.QueryMode = 0;
            #endif

            if (!BinaryPack.IsRegistered<EntityGID>()) {
                BinaryPack.RegisterWithCollections<EntityGID, UnmanagedPackArrayStrategy<EntityGID>>(EntityGIDSerializer.WriteEntityGID, EntityGIDSerializer.ReadEntityGID);
            }

            if (!BinaryPack.IsRegistered<EntityGIDCompact>()) {
                BinaryPack.RegisterWithCollections<EntityGIDCompact, UnmanagedPackArrayStrategy<EntityGIDCompact>>(EntityGIDSerializer.WriteEntityGIDCompact, EntityGIDSerializer.ReadEntityGIDCompact);
            }

            Worlds.Set(typeof(WorldType), new WorldWrapper<WorldType>());

            Status = WorldStatus.Created;
        }

        public static void Initialize(uint baseEntitiesCapacity = Const.ENTITIES_IN_CHUNK * 4) {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif
            baseEntitiesCapacity.NormalizeThis(Const.ENTITIES_IN_CHUNK);
            
            var chunksCapacity = baseEntitiesCapacity >> Const.ENTITIES_IN_CHUNK_SHIFT;
            Entities.Value.Initialize(chunksCapacity);
            Entities.Value.RegisterCluster(DEFAULT_CLUSTER);
            
            Status = WorldStatus.Initialized;

            #if FFS_ECS_EVENTS
            OnWorldInitialized();
            #endif
        }

        public static void InitializeFromGIDStoreSnapshot(BinaryPackReader reader) {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            Serializer.RestoreFromGIDStoreSnapshot(reader);
            Status = WorldStatus.Initialized;

            #if FFS_ECS_EVENTS
            OnWorldInitialized();
            #endif
        }

        public static void InitializeFromGIDStoreSnapshot(byte[] snapshot, bool gzip = false) {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            Serializer.RestoreFromGIDStoreSnapshot(snapshot, gzip);
            Status = WorldStatus.Initialized;

            #if FFS_ECS_EVENTS
            OnWorldInitialized();
            #endif
        }

        public static void InitializeFromGIDStoreSnapshot(string worldSnapshotFilePath, bool gzip = false, uint byteSizeHint = 0) {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            Serializer.RestoreFromGIDStoreSnapshot(worldSnapshotFilePath, gzip, byteSizeHint);
            Status = WorldStatus.Initialized;

            #if FFS_ECS_EVENTS
            OnWorldInitialized();
            #endif
        }

        public static void InitializeFromWorldSnapshot(BinaryPackReader reader) {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            Serializer.LoadWorldSnapshot(reader);
            Status = WorldStatus.Initialized;

            #if FFS_ECS_EVENTS
            OnWorldInitialized();
            #endif
        }

        public static void InitializeFromWorldSnapshot(byte[] snapshot, bool gzip = false) {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            Serializer.LoadWorldSnapshot(snapshot, gzip);
            Status = WorldStatus.Initialized;

            #if FFS_ECS_EVENTS
            OnWorldInitialized();
            #endif
        }

        public static void InitializeFromWorldSnapshot(string worldSnapshotFilePath, bool gzip = false, uint byteSizeHint = 0) {
            #if FFS_ECS_DEBUG
            AssertWorldIsCreated(WorldTypeName);
            #endif

            Serializer.LoadWorldSnapshot(worldSnapshotFilePath, gzip, byteSizeHint);
            Status = WorldStatus.Initialized;

            #if FFS_ECS_EVENTS
            OnWorldInitialized();
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public static void Destroy() {
            #if FFS_ECS_DEBUG
            AssertWorldIsInitialized(WorldTypeName);
            #endif

            #if FFS_ECS_EVENTS
            OnWorldDestroyed();
            WorldDebugEventListeners = null;
            #endif

            DestroyAllEntities();

            Events.Destroy();
            Entities.Value.Destroy();
            ModuleComponents.Value.Destroy();
            ModuleTags.Value.Destroy();

            config = default;
            MultiThreadActive = false;
            Context.Value.Clear();
            NamedContext.Clear();
            Serializer.Destroy();
            ParallelRunner<WorldType>.Destroy();

            Status = WorldStatus.NotCreated;
            Worlds.Delete(typeof(WorldType));

            #if FFS_ECS_DEBUG
            FileLogger?.Disable();
            FileLogger = default;
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public static bool IsWorldInitialized() => Status == WorldStatus.Initialized;

        [MethodImpl(AggressiveInlining)]
        public static bool IsIndependent() => config.Independent;

        #region DEBUG_EVENTS
        #if FFS_ECS_EVENTS
        public partial struct DEBUG {
            [MethodImpl(AggressiveInlining)]
            public static void AddWorldDebugEventListener(IWorldDebugEventListener listener) {
                WorldDebugEventListeners ??= new List<IWorldDebugEventListener>();
                WorldDebugEventListeners.Add(listener);
            }

            [MethodImpl(AggressiveInlining)]
            public static void RemoveWorldDebugEventListener(IWorldDebugEventListener listener) {
                WorldDebugEventListeners?.Remove(listener);
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal static void OnWorldResized(uint chunksCapacity) {
            if (WorldDebugEventListeners != null) {
                for (var i = WorldDebugEventListeners.Count - 1; i >= 0; i--) {
                    WorldDebugEventListeners[i].OnWorldResized(chunksCapacity);
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal static void OnWorldDestroyed() {
            if (WorldDebugEventListeners != null) {
                for (var i = WorldDebugEventListeners.Count - 1; i >= 0; i--) {
                    WorldDebugEventListeners[i].OnWorldDestroyed();
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal static void OnWorldInitialized() {
            if (WorldDebugEventListeners != null) {
                for (var i = WorldDebugEventListeners.Count - 1; i >= 0; i--) {
                    WorldDebugEventListeners[i].OnWorldInitialized();
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal static void OnEntityCreated(Entity entity) {
            if (WorldDebugEventListeners != null) {
                for (var i = WorldDebugEventListeners.Count - 1; i >= 0; i--) {
                    WorldDebugEventListeners[i].OnEntityCreated(entity);
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        internal static void OnEntityDestroyed(Entity entity) {
            if (WorldDebugEventListeners != null) {
                for (var i = WorldDebugEventListeners.Count - 1; i >= 0; i--) {
                    WorldDebugEventListeners[i].OnEntityDestroyed(entity);
                }
            }
        }

        public interface IWorldDebugEventListener {
            void OnEntityCreated(Entity entity);
            void OnEntityDestroyed(Entity entity);
            void OnWorldInitialized();
            void OnWorldDestroyed();
            void OnWorldResized(uint chunksCapacity);
        }
        #endif
        #endregion

        [MethodImpl(AggressiveInlining)]
        private static void CalculateByteSizeHint(ref uint hint) {
            if (hint == 0) {
                hint = CalculateByteSizeHint();
            }
        }

        [MethodImpl(AggressiveInlining)]
        private static uint CalculateByteSizeHint() {
            return (uint) (Entities.Value.chunks.Length * 10240 * 4);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppEagerStaticClassConstruction]
    #endif
    public static class Worlds {
        internal static readonly Dictionary<Type, IWorld> WorldsMap = new();

        [MethodImpl(AggressiveInlining)]
        public static IWorld Get(Type WorldTypeType) => WorldsMap[WorldTypeType];

        [MethodImpl(AggressiveInlining)]
        public static bool Has(Type WorldTypeType) => WorldsMap.ContainsKey(WorldTypeType);

        [MethodImpl(AggressiveInlining)]
        public static IReadOnlyCollection<IWorld> GetAll() => WorldsMap.Values;

        [MethodImpl(AggressiveInlining)]
        internal static void Set(Type WorldTypeType, IWorld world) => WorldsMap[WorldTypeType] = world;

        [MethodImpl(AggressiveInlining)]
        internal static void Delete(Type WorldTypeType) => WorldsMap.Remove(WorldTypeType);
    }

    public enum WorldStatus {
        NotCreated,
        Created,
        Initialized
    }

    public struct WorldConfig {
        public uint BaseComponentTypesCount;
        public uint BaseTagTypesCount;
        public ParallelQueryType ParallelQueryType;
        public uint CustomThreadCount;
        public ushort BaseClustersCapacity;
        public bool DefaultQueryModeStrict;
        public bool Independent;

        public static WorldConfig Default(bool independent = true) =>
            new() {
                BaseComponentTypesCount = 64,
                BaseTagTypesCount = 64,
                ParallelQueryType = ParallelQueryType.Disabled,
                CustomThreadCount = 1,
                BaseClustersCapacity = 16,
                DefaultQueryModeStrict = true,
                Independent = independent
            };

        public static WorldConfig MaxThreads(bool independent = true) =>
            new() {
                BaseComponentTypesCount = 64,
                BaseTagTypesCount = 64,
                BaseClustersCapacity = 16,
                ParallelQueryType = ParallelQueryType.MaxThreadsCount,
                DefaultQueryModeStrict = true,
                Independent = independent
            };

        internal WorldConfig Normalize() {
            BaseClustersCapacity = (ushort) Math.Max((uint) 16, BaseClustersCapacity);
            return this;
        }
    }

    public enum ParallelQueryType {
        Disabled,
        MaxThreadsCount,
        CustomThreadsCount
    }

    #if FFS_ECS_DEBUG
    internal struct MultiThreadStatus {
        public Func<bool> Active;
    }
    #endif
}