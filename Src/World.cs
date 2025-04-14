using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        internal static MultiThreadStatus MTStatus = new();
        internal static bool MultiThreadActive {
            get => MTStatus.Active;
            set => MTStatus.Active = value;
        }
        #endif

        public static void Create(WorldConfig config) {
            if (Status != WorldStatus.NotCreated) {
                throw new Exception($"World<{typeof(WorldType)}>, Method: Create, World already created");
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
            Status = WorldStatus.Created;

            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            MultiThreadActive = false;
            #endif
            
            #if !FFS_ECS_DISABLE_EVENTS
            Events.Create();
            #endif
            if (runtimeVersion == ushort.MaxValue) {
                runtimeVersion = 1;
            } else {
                runtimeVersion++;
            }
        }

        public static void Initialize() {
            if (Status != WorldStatus.Created) {
                throw new Exception($"World<{typeof(WorldType)}>, Method: Create, World not created");
            }
            ModuleStandardComponents.Value.RegisterComponentType<EntityVersion>(false);
            ModuleStandardComponents.Value.RegisterComponentType<EntityStatus>(false);

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
            if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: Destroy, World not initialized");
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
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            MultiThreadActive = false;
            FileLogger?.Disable();
            FileLogger = default;
            #endif
        }
        
        [MethodImpl(AggressiveInlining)]
        public static void Clear() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: Clear, World not initialized");
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
        }

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

        [MethodImpl(AggressiveInlining)]
        public static void AddComponentsDebugEventListener(IComponentsDebugEventListener listener) {
            ModuleComponents.Value._debugEventListeners ??= new List<IComponentsDebugEventListener>();
            ModuleComponents.Value._debugEventListeners.Add(listener);
        }

        [MethodImpl(AggressiveInlining)]
        public static void RemoveComponentsDebugEventListener(IComponentsDebugEventListener listener) {
            ModuleComponents.Value._debugEventListeners?.Remove(listener);
        }

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

        [MethodImpl(AggressiveInlining)]
        public static ushort RegisterComponentType<T>(OnAddHandler<T> onAdd = null, OnDeleteHandler<T> onDelete = null, OnCopyHandler<T> onCopy = null, uint basePoolCapacity = 128)
            where T : struct, IComponent {
            if (Status != WorldStatus.Created) {
                throw new Exception($"World<{typeof(WorldType)}>, Method: RegisterComponentType<{typeof(T)}>, World not created");
            }

            return ModuleComponents.Value.RegisterComponentType(basePoolCapacity, onAdd, onDelete, onCopy);
        }

        [MethodImpl(AggressiveInlining)]
        public static ushort RegisterMultiComponentType<T, V>(ushort defaultComponentCapacity, OnAddHandler<T> onAdd = null, uint basePoolCapacity = 128)
            where T : struct, IMultiComponent<V> where V : struct {
            if (Status != WorldStatus.Created) {
                throw new Exception($"World<{typeof(WorldType)}>, Method: RegisterMultiComponentType<{typeof(T)}, {typeof(V)}>, World not created");
            }

            return ModuleComponents.Value.RegisterMultiComponentType<T, V>(defaultComponentCapacity, basePoolCapacity, onAdd);
        }

        [MethodImpl(AggressiveInlining)]
        public static ushort GetComponentDynId<T>() where T : struct, IComponent {
            return Components<T>.Value.DynamicId();
        }

        [MethodImpl(AggressiveInlining)]
        public static IComponentsWrapper GetComponentsPool(ushort id) {
            return ModuleComponents.Value.GetPool(id);
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
        public static bool TryGetComponentsPool(ushort id, out IComponentsWrapper pool) {
            return ModuleComponents.Value.TryGetPool(id, out pool);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetComponentsPool(Type componentType, out IComponentsWrapper pool) {
            return ModuleComponents.Value.TryGetPool(componentType, out pool);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetComponentsPool<T>(out ComponentsWrapper<T> pool) where T : struct, IComponent {
            return ModuleComponents.Value.TryGetPool(out pool);
        }

        [MethodImpl(AggressiveInlining)]
        public static StandardComponentDynId RegisterStandardComponentType<T>(OnAddHandler<T> onAdd = null, OnDeleteHandler<T> onDelete = null, OnCopyHandler<T> onCopy = null)
            where T : struct, IStandardComponent {
            if (Status != WorldStatus.Created) {
                throw new Exception($"World<{typeof(WorldType)}>, Method: RegisterStandardComponentType<{typeof(T)}>, World not created");
            }

            return ModuleStandardComponents.Value.RegisterComponentType(true, onAdd, onDelete, onCopy);
        }

        [MethodImpl(AggressiveInlining)]
        public static StandardComponentDynId GetStandardComponentDynId<T>() where T : struct, IStandardComponent {
            return StandardComponents<T>.Value.DynamicId();
        }

        [MethodImpl(AggressiveInlining)]
        public static IStandardComponentsWrapper GetStandardComponentsPool(StandardComponentDynId id) {
            return ModuleStandardComponents.Value.GetPool(id);
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
        public static bool TryGetStandardComponentsPool(StandardComponentDynId id, out IStandardComponentsWrapper pool) {
            return ModuleStandardComponents.Value.TryGetPool(id, out pool);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetStandardComponentsPool(Type componentType, out IStandardComponentsWrapper pool) {
            return ModuleStandardComponents.Value.TryGetPool(componentType, out pool);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetStandardComponentsPool<T>(out StandardComponentsWrapper<T> pool) where T : struct, IStandardComponent {
            return ModuleStandardComponents.Value.TryGetPool(out pool);
        }

        #if !FFS_ECS_DISABLE_TAGS
        [MethodImpl(AggressiveInlining)]
        public static ushort RegisterTagType<T>(uint basePoolCapacity = 128) where T : struct, ITag {
            if (Status != WorldStatus.Created) {
                throw new Exception($"World<{typeof(WorldType)}>, Method: RegisterTagType<{typeof(T)}>, World not created");
            }

            return ModuleTags.Value.RegisterTagType<T>(basePoolCapacity);
        }

        [MethodImpl(AggressiveInlining)]
        public static ushort GetTagDynId<T>() where T : struct, ITag {
            return Tags<T>.Value.DynamicId();
        }

        [MethodImpl(AggressiveInlining)]
        public static ITagsWrapper GetTagsPool(ushort id) {
            return ModuleTags.Value.GetPool(id);
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
        public static bool TryGetTagsPool(ushort id, out ITagsWrapper pool) {
            return ModuleTags.Value.TryGetPool(id, out pool);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetTagsPool(Type tagType, out ITagsWrapper pool) {
            return ModuleTags.Value.TryGetPool(tagType, out pool);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetTagsPool<T>(out TagsWrapper<T> pool) where T : struct, ITag {
            return ModuleTags.Value.TryGetPool(out pool);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
        [MethodImpl(AggressiveInlining)]
        public static void AddTagDebugEventListener(ITagDebugEventListener listener) {
            ModuleTags.Value._debugEventListeners ??= new List<ITagDebugEventListener>();
            ModuleTags.Value._debugEventListeners.Add(listener);
        }

        [MethodImpl(AggressiveInlining)]
        public static void RemoveTagDebugEventListener(ITagDebugEventListener listener) {
            ModuleTags.Value._debugEventListeners?.Remove(listener);
        }
        #endif
        #endif

        #if !FFS_ECS_DISABLE_MASKS
        [MethodImpl(AggressiveInlining)]
        public static ushort RegisterMaskType<M>() where M : struct, IMask {
            if (Status != WorldStatus.Created) {
                throw new Exception($"World<{typeof(WorldType)}>, Method: RegisterMaskType<{typeof(M)}>, World not created");
            }

            return ModuleMasks.Value.RegisterMaskType<M>();
        }

        [MethodImpl(AggressiveInlining)]
        public static ushort GetMaskDynId<T>() where T : struct, IMask {
            return Masks<T>.Value.DynamicId();
        }

        [MethodImpl(AggressiveInlining)]
        public static IMasksWrapper GetMasksPool(ushort id) {
            return ModuleMasks.Value.GetPool(id);
        }

        [MethodImpl(AggressiveInlining)]
        public static IMasksWrapper GetMasksPool(Type maskType) {
            return ModuleMasks.Value.GetPool(maskType);
        }

        [MethodImpl(AggressiveInlining)]
        public static MasksWrapper<T> GetMasksPool<T>() where T : struct, IMask {
            return ModuleMasks.Value.GetPool<T>();
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetMasksPool(ushort id, out IMasksWrapper pool) {
            return ModuleMasks.Value.TryGetPool(id, out pool);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetMasksPool(Type maskType, out IMasksWrapper pool) {
            return ModuleMasks.Value.TryGetPool(maskType, out pool);
        }

        [MethodImpl(AggressiveInlining)]
        public static bool TryGetMasksPool<T>(out MasksWrapper<T> pool) where T : struct, IMask {
            return ModuleMasks.Value.TryGetPool(out pool);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
        [MethodImpl(AggressiveInlining)]
        public static void AddMaskDebugEventListener(IMaskDebugEventListener listener) {
            ModuleMasks.Value._debugEventListeners ??= new List<IMaskDebugEventListener>();
            ModuleMasks.Value._debugEventListeners.Add(listener);
        }

        [MethodImpl(AggressiveInlining)]
        public static void RemoveMaskDebugEventListener(IMaskDebugEventListener listener) {
            ModuleMasks.Value._debugEventListeners?.Remove(listener);
        }
        #endif
        #endif

        [MethodImpl(AggressiveInlining)]
        public static bool IsWorldInitialized() => Status == WorldStatus.Initialized;

        [MethodImpl(AggressiveInlining)]
        public static uint EntitiesCount() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: EntitiesCount, World not initialized");
            #endif
            return Entity.entityVersionsCount;
        }

        [MethodImpl(AggressiveInlining)]
        public static uint EntitiesCountWithoutDestroyed() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: EntitiesCountWithoutDestroyed, World not initialized");
            #endif
            return Entity.entityVersionsCount - Entity.deletedEntitiesCount;
        }

        [MethodImpl(AggressiveInlining)]
        public static uint EntitiesCapacity() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (Status == WorldStatus.NotCreated) throw new Exception($"World<{typeof(WorldType)}>, Method: GetEntitiesCapacity, World not initialized");
            #endif
            return Entity.entitiesCapacity;
        }

        [MethodImpl(AggressiveInlining)]
        public static void AllEntities(List<Entity> result) {
            result.Clear();
            for (uint i = 0, iMax = Entity.entityVersionsCount; i < iMax; i++) {
                var entity = Entity.FromIdx(i);
                if (StandardComponents<EntityVersion>.Value.RefInternal(entity).Value > 0) {
                    result.Add(entity);
                }
            }
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
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
    [Il2CppEagerStaticClassConstruction]
    #endif
    public static class Worlds {
        internal static readonly Dictionary<Type, IWorld> _worlds = new();

        [MethodImpl(AggressiveInlining)]
        public static IWorld Get(Type WorldTypeType) {
            return _worlds[WorldTypeType];
        }

        [MethodImpl(AggressiveInlining)]
        public static bool Has(Type WorldTypeType) {
            return _worlds.ContainsKey(WorldTypeType);
        }

        [MethodImpl(AggressiveInlining)]
        public static IReadOnlyCollection<IWorld> GetAll() {
            return _worlds.Values;
        }

        [MethodImpl(AggressiveInlining)]
        internal static void Set(Type WorldTypeType, IWorld world) {
            _worlds[WorldTypeType] = world;
        }

        [MethodImpl(AggressiveInlining)]
        internal static void Delete(Type WorldTypeType) {
            _worlds.Remove(WorldTypeType);
        }
    }

    public interface IWorld {
        public IEntity NewEntity(Type componentType);

        public IEntity NewEntity(IComponent component);

        public IEntity NewEntity<T>(T component) where T : struct, IComponent;

        public uint EntitiesCountWithoutDestroyed();

        public uint EntitiesCount();

        public uint EntitiesCapacity();

        public void Clear();

        public WorldStatus Status();

        public IContext Context();

        #if !FFS_ECS_DISABLE_EVENTS
        public IEvents Events();
        public List<IEventPoolWrapper> GetAllEventPools();
        #endif

        internal bool TryGetStandardComponentsRawPool(Type type, out IStandardRawPool pool);

        internal List<IStandardRawPool> GetAllStandardComponentsRawPools();

        internal bool TryGetComponentsRawPool(Type type, out IRawComponentPool pool);

        internal List<IRawPool> GetAllComponentsRawPools();


        #if !FFS_ECS_DISABLE_TAGS
        internal bool TryGetTagsRawPool(Type type, out IRawPool pool);

        internal List<IRawPool> GetAllTagsRawPools();
        #endif

        #if !FFS_ECS_DISABLE_MASKS
        internal bool TryGetMasksRawPool(Type type, out IRawPool pool);

        internal List<IRawPool> GetAllMasksRawPools();
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct WorldWrapper<WorldType> : IWorld where WorldType : struct, IWorldType {
        [MethodImpl(AggressiveInlining)]
        public IEntity NewEntity(Type componentType) => World<WorldType>.Entity.New(componentType);

        [MethodImpl(AggressiveInlining)]
        public IEntity NewEntity(IComponent component) => World<WorldType>.Entity.New(component);

        [MethodImpl(AggressiveInlining)]
        public IEntity NewEntity<T>(T component) where T : struct, IComponent => World<WorldType>.Entity.New(component);

        [MethodImpl(AggressiveInlining)]
        public uint EntitiesCountWithoutDestroyed() => World<WorldType>.EntitiesCountWithoutDestroyed();

        [MethodImpl(AggressiveInlining)]
        public uint EntitiesCount() => World<WorldType>.EntitiesCount();

        [MethodImpl(AggressiveInlining)]
        public uint EntitiesCapacity() => World<WorldType>.EntitiesCapacity();

        [MethodImpl(AggressiveInlining)]
        public void Clear() => World<WorldType>.Clear();

        [MethodImpl(AggressiveInlining)]
        public WorldStatus Status() => World<WorldType>.Status;

        [MethodImpl(AggressiveInlining)]
        public IContext Context() => World<WorldType>.Context.Value;

        #if !FFS_ECS_DISABLE_EVENTS
        [MethodImpl(AggressiveInlining)]
        public IEvents Events() => new EventsWrapper<WorldType>();

        public List<IEventPoolWrapper> GetAllEventPools() {
            return World<WorldType>.Events.GetAllRawsPools();
        }
        #endif

        bool IWorld.TryGetStandardComponentsRawPool(Type type, out IStandardRawPool pool) {
            if (World<WorldType>.TryGetStandardComponentsPool(type, out var p)) {
                pool = p;
                return true;
            }

            pool = default;
            return false;
        }

        List<IStandardRawPool> IWorld.GetAllStandardComponentsRawPools() {
            return World<WorldType>.ModuleStandardComponents.Value.GetAllRawsPools();
        }

        bool IWorld.TryGetComponentsRawPool(Type type, out IRawComponentPool pool) {
            if (World<WorldType>.TryGetComponentsPool(type, out var p)) {
                pool = p;
                return true;
            }

            pool = default;
            return false;
        }

        List<IRawPool> IWorld.GetAllComponentsRawPools() {
            return World<WorldType>.ModuleComponents.Value.GetAllRawsPools();
        }

        #if !FFS_ECS_DISABLE_TAGS
        bool IWorld.TryGetTagsRawPool(Type type, out IRawPool pool) {
            if (World<WorldType>.TryGetTagsPool(type, out var p)) {
                pool = p;
                return true;
            }

            pool = default;
            return false;
        }

        List<IRawPool> IWorld.GetAllTagsRawPools() {
            return World<WorldType>.ModuleTags.Value.GetAllRawsPools();
        }
        #endif

        #if !FFS_ECS_DISABLE_MASKS
        bool IWorld.TryGetMasksRawPool(Type type, out IRawPool pool) {
            if (World<WorldType>.TryGetMasksPool(type, out var p)) {
                pool = p;
                return true;
            }

            pool = default;
            return false;
        }

        List<IRawPool> IWorld.GetAllMasksRawPools() {
            return World<WorldType>.ModuleMasks.Value.GetAllRawsPools();
        }
        #endif
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