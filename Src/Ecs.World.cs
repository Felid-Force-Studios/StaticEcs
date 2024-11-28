using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    public interface IWorldId { }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class Ecs<WorldID> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public abstract partial class World {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal static List<IWorldDebugEventListener> _debugEventListeners;
            #endif
            internal static short[] _entityVersions;
            internal static Entity[] _deletedEntities;
            internal static int _entityVersionsCount;
            internal static int _deletedEntitiesCount;
            public static WorldStatus Status { get; private set; }

            internal static void Create() {
                _entityVersions = new short[cfg.BaseEntitiesCount];
                _deletedEntities = new Entity[cfg.BaseDeletedEntitiesCount];
                ModuleComponents.Value.Create(cfg.BaseComponentTypesCount);
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.Create(cfg.BaseTagTypesCount);
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                ModuleMasks.Value.Create(cfg.BaseMaskTypesCount);
                #endif
                Status = WorldStatus.Created;
            }

            internal static void Initialize() {
                ModuleComponents.Value.Initialize();
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.Initialize();
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                ModuleMasks.Value.Initialize();
                #endif
                Worlds.Set(typeof(WorldID), new WorldWrapper<WorldID>());
                Status = WorldStatus.Initialized;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (_debugEventListeners != null) {
                    foreach (var listener in _debugEventListeners) {
                        listener.OnWorldInitialized();
                    }
                }
                #endif
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
            #endif

            [MethodImpl(AggressiveInlining)]
            public static ComponentDynId RegisterComponent<T>(uint basePoolCapacity = 128) where T : struct, IComponent {
                if (Status != WorldStatus.Created) {
                    throw new Exception($"World<{typeof(WorldID)}>, Method: RegisterComponent<{typeof(T)}>, World not created");
                }
                return ModuleComponents.Value.RegisterComponent<T>(basePoolCapacity);
            }
            
            [MethodImpl(AggressiveInlining)]
            public static ComponentDynId GetComponentDynId<T>() where T : struct, IComponent {
                return Components<T>.Value.DynamicId();
            }
            
            [MethodImpl(AggressiveInlining)]
            public static IComponentsWrapper GetComponentsPool(ComponentDynId id) {
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
            public static bool TryGetComponentsPool(ComponentDynId id, out IComponentsWrapper pool) {
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

            #if !FFS_ECS_DISABLE_TAGS
            [MethodImpl(AggressiveInlining)]
            public static TagDynId RegisterTag<T>(uint basePoolCapacity = 128) where T : struct, ITag {
                if (Status != WorldStatus.Created) {
                    throw new Exception($"World<{typeof(WorldID)}>, Method: RegisterTag<{typeof(T)}>, World not created");
                }
                return ModuleTags.Value.RegisterTag<T>(basePoolCapacity);
            }
                        
            [MethodImpl(AggressiveInlining)]
            public static TagDynId GetTagDynId<T>() where T : struct, ITag {
                return Tags<T>.Value.DynamicId();
            }
            
            [MethodImpl(AggressiveInlining)]
            public static ITagsWrapper GetTagsPool(TagDynId id) {
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
            public static bool TryGetTagsPool(TagDynId id, out ITagsWrapper pool) {
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
            public static MaskDynId RegisterMask<M>() where M : struct, IMask {
                if (Status != WorldStatus.Created) {
                    throw new Exception($"World<{typeof(WorldID)}>, Method: RegisterMask<{typeof(M)}>, World not created");
                }
                return ModuleMasks.Value.RegisterMask<M>();
            }
            
            [MethodImpl(AggressiveInlining)]
            public static MaskDynId GetMaskDynId<T>() where T : struct, IMask {
                return Masks<T>.Value.DynamicId();
            }
            
            [MethodImpl(AggressiveInlining)]
            public static IMasksWrapper GetMasksPool(MaskDynId id) {
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
            public static bool TryGetMasksPool(MaskDynId id, out IMasksWrapper pool) {
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
            public static bool IsInitialized() => Status == WorldStatus.Initialized;

            [MethodImpl(AggressiveInlining)]
            internal static Entity CreateEntityInternal() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: CreateEntityInternal, World not initialized");
                #endif
                Entity entity;
                if (_deletedEntitiesCount > 0) {
                    entity = _deletedEntities[--_deletedEntitiesCount];
                    _entityVersions[entity._id] *= -1;
                } else {
                    entity = new Entity(_entityVersionsCount);
                    
                    if (_entityVersionsCount == _entityVersions.Length) {
                        Array.Resize(ref _entityVersions, _entityVersionsCount << 1);
                        ModuleComponents.Value.Resize(_entityVersionsCount << 1);
                        #if !FFS_ECS_DISABLE_TAGS
                        ModuleTags.Value.Resize(_entityVersionsCount << 1);
                        #endif
                        #if !FFS_ECS_DISABLE_MASKS
                        ModuleMasks.Value.Resize(_entityVersionsCount << 1);
                        #endif
                        
                        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                        if (_debugEventListeners != null) {
                            foreach (var listener in _debugEventListeners) {
                                listener.OnWorldResized(_entityVersionsCount << 1);
                            }
                        }
                        #endif
                    }
                    
                    _entityVersions[_entityVersionsCount++] = 1;
                }

                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (_debugEventListeners != null) {
                    foreach (var listener in _debugEventListeners) {
                        listener.OnEntityCreated(entity);
                    }
                }
                #endif
                return entity;
            }
            
            [MethodImpl(AggressiveInlining)]
            internal static void CreateEntitiesInternal<C>(int count, C onCreateEntity, Action<Entity> additional = null) where C : struct, IOnCreateEntityFunction<WorldID> {
                var newEntitiesCount = count - (_entityVersions.Length - _entityVersionsCount + _deletedEntitiesCount);
                if (newEntitiesCount >= 0) {
                    ResizeFor(newEntitiesCount);
                }

                while (count > 0) {
                    count--;
                    Entity entity;
                    if (_deletedEntitiesCount > 0) {
                        entity = _deletedEntities[--_deletedEntitiesCount];
                        _entityVersions[entity._id] *= -1;
                    } else {
                        entity = new Entity(_entityVersionsCount);
                        _entityVersions[_entityVersionsCount++] = 1;
                    }
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var listener in _debugEventListeners) {
                            listener.OnEntityCreated(entity);
                        }
                    }
                    #endif
                    onCreateEntity.OnCreate(entity);
                    additional?.Invoke(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            private static void ResizeFor(int count) {
                var newSize = Utils.CalculateSize(_entityVersionsCount + count);
                Array.Resize(ref _entityVersions, newSize);
                ModuleComponents.Value.Resize(newSize);
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.Resize(newSize);
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                ModuleMasks.Value.Resize(newSize);
                #endif
                
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (_debugEventListeners != null) {
                    foreach (var listener in _debugEventListeners) {
                        listener.OnWorldResized(newSize);
                    }
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public static void DestroyEntity(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: DestroyEntity, World not initialized");
                #endif
                ref var version = ref _entityVersions[entity._id];
                if (version < 0) {
                    return;
                }

                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.DestroyEntity(entity);
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                ModuleMasks.Value.DestroyEntity(entity);
                #endif
                ModuleComponents.Value.DestroyEntity(entity);
                version = version == short.MaxValue ? (short) -1 : (short) -(version + 1);
                if (_deletedEntitiesCount == _deletedEntities.Length) {
                    Array.Resize(ref _deletedEntities, _deletedEntitiesCount << 1);
                }

                _deletedEntities[_deletedEntitiesCount++] = entity;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (_debugEventListeners != null) {
                    foreach (var listener in _debugEventListeners) {
                        listener.OnEntityDestroyed(entity);
                    }
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public static void CopyEntityData(Entity srcEntity, Entity dstEntity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: CopyEntityData, World not initialized");
                #endif
                
                ModuleComponents.Value.CopyEntity(srcEntity, dstEntity);
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.CopyEntity(srcEntity, dstEntity);
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                ModuleMasks.Value.CopyEntity(srcEntity, dstEntity);
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity CloneEntity(Entity srcEntity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: CloneEntity, World not initialized");
                #endif
                
                var dstEntity = CreateEntityInternal();
                CopyEntityData(srcEntity, dstEntity);

                return dstEntity;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static string ToPrettyStringEntity(Entity entity) {
                var result = $"Entity ID: {entity._id}, Version: {EntityVersion(entity)}\n";
                result += ModuleComponents.Value.ToPrettyStringEntity(entity);
                #if !FFS_ECS_DISABLE_TAGS
                result += ModuleTags.Value.ToPrettyStringEntity(entity);
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                result += ModuleMasks.Value.ToPrettyStringEntity(entity);
                #endif
                return result;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static int EntitiesCount() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: EntitiesCount, World not initialized");
                #endif
                return _entityVersionsCount;
            }

            [MethodImpl(AggressiveInlining)]
            public static int EntitiesCountWithoutDestroyed() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: EntitiesCountWithoutDestroyed, World not initialized");
                #endif
                return _entityVersionsCount - _deletedEntitiesCount;
            }

            [MethodImpl(AggressiveInlining)]
            public static int EntitiesCapacity() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(Status == WorldStatus.NotCreated) throw new Exception($"World<{typeof(WorldID)}>, Method: GetEntitiesCapacity, World not initialized");
                #endif
                return _entityVersions.Length;
            }

            [MethodImpl(AggressiveInlining)]
            public static short EntityVersion(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: EntityVersion, World not initialized");
                #endif
                return _entityVersions[entity._id];
            }

            [MethodImpl(AggressiveInlining)]
            public static void AllEntities(List<Entity> result) {
                result.Clear();
                for (int i = 0, iMax = _entityVersionsCount; i < iMax; i++) {
                    if (_entityVersions[i] > 0) {
                        result.Add(Entity.FromIdx(i));
                    }
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void Clear() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: Clear, World not initialized");
                #endif

                ModuleComponents.Value.Clear();
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.Clear();
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                ModuleMasks.Value.Clear();
                #endif

                Array.Clear(_entityVersions, 0, _entityVersions.Length);
                Array.Clear(_deletedEntities, 0, _deletedEntities.Length);
                _entityVersionsCount = 0;
                _deletedEntitiesCount = 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal static void Destroy() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldID)}>, Method: Destroy, World not initialized");
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
                
                for (var i = _entityVersionsCount - 1; i >= 0; i--) {
                    if (_entityVersions[i] > 0) {
                        DestroyEntity(Entity.FromIdx(i));
                    }
                }

                ModuleComponents.Value.Destroy();
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.Destroy();
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                ModuleMasks.Value.Destroy();
                #endif

                _entityVersions = null;
                _deletedEntities = null;
                _entityVersionsCount = 0;
                _deletedEntitiesCount = 0;
                Status = WorldStatus.NotCreated;
                Worlds.Delete(typeof(WorldID));
            }
        }
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
        public interface IWorldDebugEventListener {
            void OnEntityCreated(Entity entity);
            void OnEntityDestroyed(Entity entity);
            void OnWorldInitialized();
            void OnWorldDestroyed();
            void OnWorldResized(int capacity);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppEagerStaticClassConstruction]
    #endif
    public static class Worlds {
        internal static readonly Dictionary<Type, IWorld> _worlds = new();

        [MethodImpl(AggressiveInlining)]
        public static IWorld Get(Type worldIdType) {
            return _worlds[worldIdType];
        }

        [MethodImpl(AggressiveInlining)]
        public static bool Has(Type worldIdType) {
            return _worlds.ContainsKey(worldIdType);
        }
        
        [MethodImpl(AggressiveInlining)]
        public static IReadOnlyCollection<IWorld> GetAll() {
            return _worlds.Values;
        }
        
        [MethodImpl(AggressiveInlining)]
        internal static void Set(Type worldIdType, IWorld world) {
            _worlds[worldIdType] = world;
        }
        
        [MethodImpl(AggressiveInlining)]
        internal static void Delete(Type worldIdType) {
            _worlds.Remove(worldIdType);
        }
    }

    public interface IWorld {
        
        public IEntity NewEntity(Type componentType);
        
        public IEntity NewEntity(IComponent component);

        public IEntity NewEntity<T>(T component) where T : struct, IComponent;

        public int EntitiesCountWithoutDestroyed();

        public int EntitiesCount();
        
        public int EntitiesCapacity();
        
        public void Clear();

        public IContext Context();
        
        #if !FFS_ECS_DISABLE_EVENTS
        public IEvents Events();
        #endif

        internal bool TryGetComponentsRawPool(Type type, out IRawPool pool);
        
        #if !FFS_ECS_DISABLE_TAGS
        internal bool TryGetTagsRawPool(Type type, out IRawPool pool);
        #endif
        
        #if !FFS_ECS_DISABLE_MASKS
        internal bool TryGetMasksRawPool(Type type, out IRawPool pool);
        #endif
        
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public struct WorldWrapper<WorldId> : IWorld where WorldId : struct, IWorldId {
        
        [MethodImpl(AggressiveInlining)]
        public IEntity NewEntity(Type componentType) => Ecs<WorldId>.Entity.New(componentType);

        [MethodImpl(AggressiveInlining)]
        public IEntity NewEntity(IComponent component) => Ecs<WorldId>.Entity.New(component);

        [MethodImpl(AggressiveInlining)]
        public IEntity NewEntity<T>(T component) where T : struct, IComponent => Ecs<WorldId>.Entity.New(component);

        [MethodImpl(AggressiveInlining)]
        public int EntitiesCountWithoutDestroyed() => Ecs<WorldId>.World.EntitiesCountWithoutDestroyed();

        [MethodImpl(AggressiveInlining)]
        public int EntitiesCount() => Ecs<WorldId>.World.EntitiesCount();

        [MethodImpl(AggressiveInlining)]
        public int EntitiesCapacity() => Ecs<WorldId>.World.EntitiesCapacity();

        [MethodImpl(AggressiveInlining)]
        public void Clear() => Ecs<WorldId>.World.Clear();

        [MethodImpl(AggressiveInlining)]
        public IContext Context() => Ecs<WorldId>.Context.Value;

        #if !FFS_ECS_DISABLE_EVENTS
        [MethodImpl(AggressiveInlining)]
        public IEvents Events() => new EventsWrapper<WorldId>();
        #endif

        bool IWorld.TryGetComponentsRawPool(Type type, out IRawPool pool) {
            if (Ecs<WorldId>.World.TryGetComponentsPool(type, out var p)) {
                pool = p;
                return true;
            }
            
            pool = default;
            return false;
        }

        #if !FFS_ECS_DISABLE_TAGS
        bool IWorld.TryGetTagsRawPool(Type type, out IRawPool pool) {
            if (Ecs<WorldId>.World.TryGetTagsPool(type, out var p)) {
                pool = p;
                return true;
            }
            
            pool = default;
            return false;
        }
        #endif

        #if !FFS_ECS_DISABLE_MASKS
        bool IWorld.TryGetMasksRawPool(Type type, out IRawPool pool) {
            if (Ecs<WorldId>.World.TryGetMasksPool(type, out var p)) {
                pool = p;
                return true;
            }
            
            pool = default;
            return false;
        }
        #endif
    }

    public enum WorldStatus {
        NotCreated,
        Created,
        Initialized
    }
}