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
    #endif
    public abstract partial class Ecs<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public abstract partial class World {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
            internal static List<IWorldDebugEventListener> _debugEventListeners;
            #endif
            internal static Entity[] _deletedEntities;
            internal static uint _entitiesCapacity;
            internal static uint _entityVersionsCount;
            internal static uint _deletedEntitiesCount;
            public static WorldStatus Status { get; private set; }

            internal static void Create() {
                _entitiesCapacity = cfg.BaseEntitiesCount;
                _deletedEntities = new Entity[cfg.BaseDeletedEntitiesCount];
                ModuleComponents.Value.Create(cfg.BaseComponentTypesCount);
                ModuleStandardComponents.Value.Create(cfg.BaseStandardComponentTypesCount);
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.Create(cfg.BaseTagTypesCount);
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                ModuleMasks.Value.Create(cfg.BaseMaskTypesCount);
                #endif
                Status = WorldStatus.Created;
            }

            internal static void Initialize() {
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
            public static ushort RegisterComponentType<T>(AutoInitHandler<T> autoInit = null, AutoResetHandler<T> autoReset = null, AutoCopyHandler<T> autoCopy = null, uint basePoolCapacity = 128) where T : struct, IComponent {
                if (Status != WorldStatus.Created) {
                    throw new Exception($"World<{typeof(WorldType)}>, Method: RegisterComponentType<{typeof(T)}>, World not created");
                }
                return ModuleComponents.Value.RegisterComponentType(basePoolCapacity, autoInit, autoReset, autoCopy);
            }

            [MethodImpl(AggressiveInlining)]
            public static ushort RegisterMultiComponentType<T, V>(ushort defaultComponentCapacity, AutoInitHandler<T> autoInit = null, uint basePoolCapacity = 128) where T : struct, IMultiComponent<V> where V : struct {
                if (Status != WorldStatus.Created) {
                    throw new Exception($"World<{typeof(WorldType)}>, Method: RegisterMultiComponentType<{typeof(T)}, {typeof(V)}>, World not created");
                }
                return ModuleComponents.Value.RegisterMultiComponentType<T, V>(defaultComponentCapacity, basePoolCapacity, autoInit);
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
            public static StandardComponentDynId RegisterStandardComponentType<T>(AutoInitHandler<T> autoInit = null, AutoResetHandler<T> autoReset = null, AutoCopyHandler<T> autoCopy = null) where T : struct, IStandardComponent {
                if (Status != WorldStatus.Created) {
                    throw new Exception($"World<{typeof(WorldType)}>, Method: RegisterStandardComponentType<{typeof(T)}>, World not created");
                }
                return ModuleStandardComponents.Value.RegisterComponentType(true, autoInit, autoReset, autoCopy);
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
            public static bool IsInitialized() => Status == WorldStatus.Initialized;

            [MethodImpl(AggressiveInlining)]
            internal static Entity CreateEntityInternal() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: CreateEntityInternal, World not initialized");
                #endif
                Entity entity;
                if (_deletedEntitiesCount > 0) {
                    entity = _deletedEntities[--_deletedEntitiesCount];
                    StandardComponents<EntityVersion>.Value.RefMutInternal(entity).Value *= -1;
                } else {
                    entity = new Entity(_entityVersionsCount);
                    
                    if (_entityVersionsCount == _entitiesCapacity) {
                        _entitiesCapacity = _entityVersionsCount << 1;
                        ModuleComponents.Value.Resize(_entitiesCapacity);
                        ModuleStandardComponents.Value.Resize(_entitiesCapacity);
                        #if !FFS_ECS_DISABLE_TAGS
                        ModuleTags.Value.Resize(_entitiesCapacity);
                        #endif
                        #if !FFS_ECS_DISABLE_MASKS
                        ModuleMasks.Value.Resize(_entitiesCapacity);
                        #endif
                        
                        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                        if (_debugEventListeners != null) {
                            foreach (var listener in _debugEventListeners) {
                                listener.OnWorldResized(_entitiesCapacity);
                            }
                        }
                        #endif
                    }
                    
                    StandardComponents<EntityVersion>.Value.RefMutInternal(entity).Value = 1;
                    _entityVersionsCount++;
                }
                ModuleStandardComponents.Value.InitEntity(entity);

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
            internal static void CreateEntitiesInternal<C>(uint count, C onCreateEntity, Action<Entity> additional = null) where C : struct, IOnCreateEntityFunction<WorldType> {
                var newEntitiesCount = count + 1 - (_entitiesCapacity - _entityVersionsCount + _deletedEntitiesCount);
                if (newEntitiesCount > 0) {
                    ResizeFor(newEntitiesCount);
                }

                while (count > 0) {
                    count--;
                    Entity entity;
                    if (_deletedEntitiesCount > 0) {
                        entity = _deletedEntities[--_deletedEntitiesCount];
                        StandardComponents<EntityVersion>.Value.RefMutInternal(entity).Value *= -1;
                    } else {
                        entity = new Entity(_entityVersionsCount);
                        StandardComponents<EntityVersion>.Value.RefMutInternal(entity).Value = 1;
                        _entityVersionsCount++;
                    }
                    #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var listener in _debugEventListeners) {
                            listener.OnEntityCreated(entity);
                        }
                    }
                    #endif
                    ModuleStandardComponents.Value.InitEntity(entity);
                    onCreateEntity.OnCreate(entity);
                    additional?.Invoke(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            private static void ResizeFor(uint count) {
                _entitiesCapacity = Utils.CalculateSize(_entityVersionsCount + count);
                ModuleComponents.Value.Resize(_entitiesCapacity);
                ModuleStandardComponents.Value.Resize(_entitiesCapacity);
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.Resize(_entitiesCapacity);
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                ModuleMasks.Value.Resize(_entitiesCapacity);
                #endif
                
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (_debugEventListeners != null) {
                    foreach (var listener in _debugEventListeners) {
                        listener.OnWorldResized(_entitiesCapacity);
                    }
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public static void DestroyEntity(Entity entity) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: DestroyEntity, World not initialized");
                #endif
                ref var version = ref StandardComponents<EntityVersion>.Value.RefMutInternal(entity).Value;
                if (version > 0) {
                    #if !FFS_ECS_DISABLE_TAGS
                    ModuleTags.Value.DestroyEntity(entity);
                    #endif
                    #if !FFS_ECS_DISABLE_MASKS
                    ModuleMasks.Value.DestroyEntity(entity);
                    #endif
                    ModuleComponents.Value.DestroyEntity(entity);
                    ModuleStandardComponents.Value.DestroyEntity(entity);
                    version = version == short.MaxValue ? (short) -1 : (short) -(version + 1);
                    StandardComponents<EntityStatus>.Value.RefMutInternal(entity).Value = EntityStatusType.Enabled;
                    if (_deletedEntitiesCount == _deletedEntities.Length) {
                        Array.Resize(ref _deletedEntities, (int) (_deletedEntitiesCount << 1));
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
            }

            [MethodImpl(AggressiveInlining)]
            public static void CopyEntityData(Entity srcEntity, Entity dstEntity, bool withDisabled) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: CopyEntityData, World not initialized");
                #endif
                
                ModuleComponents.Value.CopyEntity(srcEntity, dstEntity, withDisabled);
                ModuleStandardComponents.Value.CopyEntity(srcEntity, dstEntity);
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.CopyEntity(srcEntity, dstEntity);
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                ModuleMasks.Value.CopyEntity(srcEntity, dstEntity);
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity CloneEntity(Entity srcEntity, bool withDisabled) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: CloneEntity, World not initialized");
                #endif
                
                var dstEntity = CreateEntityInternal();
                CopyEntityData(srcEntity, dstEntity, withDisabled);

                return dstEntity;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static string ToPrettyStringEntity(Entity entity, bool withDisabled) {
                var result = $"Entity ID: {entity._id}\n";
                result += ModuleStandardComponents.Value.ToPrettyStringEntity(entity);
                result += ModuleComponents.Value.ToPrettyStringEntity(entity, withDisabled);
                #if !FFS_ECS_DISABLE_TAGS
                result += ModuleTags.Value.ToPrettyStringEntity(entity);
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                result += ModuleMasks.Value.ToPrettyStringEntity(entity);
                #endif
                return result;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static uint EntitiesCount() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: EntitiesCount, World not initialized");
                #endif
                return _entityVersionsCount;
            }

            [MethodImpl(AggressiveInlining)]
            public static uint EntitiesCountWithoutDestroyed() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: EntitiesCountWithoutDestroyed, World not initialized");
                #endif
                return _entityVersionsCount - _deletedEntitiesCount;
            }

            [MethodImpl(AggressiveInlining)]
            public static uint EntitiesCapacity() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(Status == WorldStatus.NotCreated) throw new Exception($"World<{typeof(WorldType)}>, Method: GetEntitiesCapacity, World not initialized");
                #endif
                return _entitiesCapacity;
            }

            [MethodImpl(AggressiveInlining)]
            public static void AllEntities(List<Entity> result) {
                result.Clear();
                for (uint i = 0, iMax = _entityVersionsCount; i < iMax; i++) {
                    var entity = Entity.FromIdx(i);
                    if (StandardComponents<EntityVersion>.Value.RefMutInternal(entity).Value > 0) {
                        result.Add(entity);
                    }
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void Clear() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: Clear, World not initialized");
                #endif

                ModuleComponents.Value.Clear();
                ModuleStandardComponents.Value.Clear();
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.Clear();
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                ModuleMasks.Value.Clear();
                #endif

                Array.Clear(_deletedEntities, 0, _deletedEntities.Length);
                _entityVersionsCount = 0;
                _deletedEntitiesCount = 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal static void Destroy() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if(!IsInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: Destroy, World not initialized");
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
                
                for (var i = _entityVersionsCount; i > 0; i--) {
                    var entity = Entity.FromIdx(i - 1);
                    if (StandardComponents<EntityVersion>.Value.RefMutInternal(entity).Value > 0) {
                        DestroyEntity(entity);
                    }
                }

                ModuleComponents.Value.Destroy();
                ModuleStandardComponents.Value.Destroy();
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.Destroy();
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                ModuleMasks.Value.Destroy();
                #endif

                _deletedEntities = null;
                _entityVersionsCount = 0;
                _entitiesCapacity = 0;
                _deletedEntitiesCount = 0;
                Status = WorldStatus.NotCreated;
                Worlds.Delete(typeof(WorldType));
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
        public IEntity NewEntity(Type componentType) => Ecs<WorldType>.Entity.New(componentType);

        [MethodImpl(AggressiveInlining)]
        public IEntity NewEntity(IComponent component) => Ecs<WorldType>.Entity.New(component);

        [MethodImpl(AggressiveInlining)]
        public IEntity NewEntity<T>(T component) where T : struct, IComponent => Ecs<WorldType>.Entity.New(component);

        [MethodImpl(AggressiveInlining)]
        public uint EntitiesCountWithoutDestroyed() => Ecs<WorldType>.World.EntitiesCountWithoutDestroyed();

        [MethodImpl(AggressiveInlining)]
        public uint EntitiesCount() => Ecs<WorldType>.World.EntitiesCount();

        [MethodImpl(AggressiveInlining)]
        public uint EntitiesCapacity() => Ecs<WorldType>.World.EntitiesCapacity();

        [MethodImpl(AggressiveInlining)]
        public void Clear() => Ecs<WorldType>.World.Clear();

        [MethodImpl(AggressiveInlining)]
        public WorldStatus Status() => Ecs<WorldType>.World.Status;

        [MethodImpl(AggressiveInlining)]
        public IContext Context() => Ecs<WorldType>.Context.Value;

        #if !FFS_ECS_DISABLE_EVENTS
        [MethodImpl(AggressiveInlining)]
        public IEvents Events() => new EventsWrapper<WorldType>();

        public List<IEventPoolWrapper> GetAllEventPools() {
            return Ecs<WorldType>.Events.GetAllRawsPools();
        }
        #endif
        
        bool IWorld.TryGetStandardComponentsRawPool(Type type, out IStandardRawPool pool) {
            if (Ecs<WorldType>.World.TryGetStandardComponentsPool(type, out var p)) {
                pool = p;
                return true;
            }
            
            pool = default;
            return false;
        }

        List<IStandardRawPool> IWorld.GetAllStandardComponentsRawPools() {
            return Ecs<WorldType>.ModuleStandardComponents.Value.GetAllRawsPools();
        }

        bool IWorld.TryGetComponentsRawPool(Type type, out IRawComponentPool pool) {
            if (Ecs<WorldType>.World.TryGetComponentsPool(type, out var p)) {
                pool = p;
                return true;
            }
            
            pool = default;
            return false;
        }

        List<IRawPool> IWorld.GetAllComponentsRawPools() {
            return Ecs<WorldType>.ModuleComponents.Value.GetAllRawsPools();
        }

        #if !FFS_ECS_DISABLE_TAGS
        bool IWorld.TryGetTagsRawPool(Type type, out IRawPool pool) {
            if (Ecs<WorldType>.World.TryGetTagsPool(type, out var p)) {
                pool = p;
                return true;
            }
            
            pool = default;
            return false;
        }

        List<IRawPool> IWorld.GetAllTagsRawPools() {
            return Ecs<WorldType>.ModuleTags.Value.GetAllRawsPools();
        }
        #endif

        #if !FFS_ECS_DISABLE_MASKS
        bool IWorld.TryGetMasksRawPool(Type type, out IRawPool pool) {
            if (Ecs<WorldType>.World.TryGetMasksPool(type, out var p)) {
                pool = p;
                return true;
            }
            
            pool = default;
            return false;
        }

        List<IRawPool> IWorld.GetAllMasksRawPools() {
            return Ecs<WorldType>.ModuleMasks.Value.GetAllRawsPools();
        }
        #endif
    }

    public enum WorldStatus {
        NotCreated,
        Created,
        Initialized
    }
}