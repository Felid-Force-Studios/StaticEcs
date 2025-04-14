using System;
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
        public readonly partial struct Entity : IEquatable<Entity>, IEntity {
            internal static Entity[] deletedEntities;
            internal static uint entitiesCapacity;
            internal static uint entityVersionsCount;
            internal static uint deletedEntitiesCount;

            internal readonly uint _id;

            internal Entity(uint id) {
                _id = id;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity FromIdx(uint idx) => new(idx);

            [MethodImpl(AggressiveInlining)]
            public short Version() => StandardComponents<EntityVersion>.Value.RefInternal(this).Value;

            uint IEntity.GetId() => _id;

            [MethodImpl(AggressiveInlining)]
            Type IEntity.WorldTypeType() => typeof(WorldType);

            [MethodImpl(AggressiveInlining)]
            IWorld IEntity.World() => Worlds.Get(typeof(WorldType));

            [MethodImpl(AggressiveInlining)]
            public bool IsActual() => StandardComponents<EntityVersion>.Value.RefInternal(this).Value > 0;

            [MethodImpl(AggressiveInlining)]
            public Entity Clone(bool withDisabled = true) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: CloneEntity, World not initialized");
                #endif

                var dstEntity = New();
                CopyTo(dstEntity, withDisabled);

                return dstEntity;
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyTo(Entity dstEntity, bool withDisabled = true) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: CopyEntityData, World not initialized");
                #endif

                ModuleComponents.Value.CopyEntity(this, dstEntity, withDisabled);
                ModuleStandardComponents.Value.CopyEntity(this, dstEntity);
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.CopyEntity(this, dstEntity);
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                ModuleMasks.Value.CopyEntity(this, dstEntity);
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public void MoveTo(Entity dstEntity, bool withDisabled = true) {
                CopyTo(dstEntity, withDisabled);
                Destroy();
            }

            [MethodImpl(AggressiveInlining)]
            public static bool operator ==(Entity left, Entity right) {
                return left.Equals(right);
            }

            [MethodImpl(AggressiveInlining)]
            public static bool operator !=(Entity left, Entity right) {
                return !left.Equals(right);
            }

            [MethodImpl(AggressiveInlining)]
            public PackedEntity Pack() => new() { _entity = _id, _version = StandardComponents<EntityVersion>.Value.RefInternal(this).Value };

            [MethodImpl(AggressiveInlining)]
            public void Destroy() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: DestroyEntity, World not initialized");
                if (StandardComponents<EntityVersion>.Value.RefInternal(this).Value <= 0) throw new Exception($"World<{typeof(WorldType)}>, Method: DestroyEntity, Entity already destroyed");
                #endif
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.DestroyEntity(this);
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                ModuleMasks.Value.DestroyEntity(this);
                #endif
                ModuleComponents.Value.DestroyEntity(this);
                ModuleStandardComponents.Value.DestroyEntity(this);
                StandardComponents<EntityVersion>.Value.RefInternal(this).SetInactive();
                StandardComponents<EntityStatus>.Value.RefInternal(this).Value = EntityStatusType.Enabled;
                if (deletedEntitiesCount == deletedEntities.Length) {
                    Array.Resize(ref deletedEntities, (int) (deletedEntitiesCount << 1));
                }

                deletedEntities[deletedEntitiesCount++] = this;
                #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                if (_debugEventListeners != null) {
                    foreach (var listener in _debugEventListeners) {
                        listener.OnEntityDestroyed(this);
                    }
                }
                #endif
            }
            
            [MethodImpl(AggressiveInlining)]
            public void TryDestroy() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: DestroyEntity, World not initialized");
                #endif
                if (StandardComponents<EntityVersion>.Value.RefInternal(this).Value > 0) {
                    Destroy();
                }
            }

            #region NEW_BY_TYPE_SINGLE
            [MethodImpl(AggressiveInlining)]
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || !FFS_ECS_LIFECYCLE_ENTITY
            public static Entity New() {
            #else
            internal static Entity New() {
            #endif
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: CreateEntity, World not initialized");
                if (MultiThreadActive) throw new Exception($"World<{typeof(WorldType)}>, Method: CreateEntity, this operation is not supported in multithreaded mode");
                #endif
                Entity entity;
                if (deletedEntitiesCount > 0) {
                    entity = deletedEntities[--deletedEntitiesCount];
                    StandardComponents<EntityVersion>.Value.RefInternal(entity).Value *= -1;
                } else {
                    entity = new Entity(entityVersionsCount);

                    if (entityVersionsCount == entitiesCapacity) {
                        entitiesCapacity = entityVersionsCount << 1;
                        ModuleComponents.Value.Resize(entitiesCapacity);
                        ModuleStandardComponents.Value.Resize(entitiesCapacity);
                        #if !FFS_ECS_DISABLE_TAGS
                        ModuleTags.Value.Resize(entitiesCapacity);
                        #endif
                        #if !FFS_ECS_DISABLE_MASKS
                        ModuleMasks.Value.Resize(entitiesCapacity);
                        #endif

                        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                        if (_debugEventListeners != null) {
                            foreach (var listener in _debugEventListeners) {
                                listener.OnWorldResized(entitiesCapacity);
                            }
                        }
                        #endif
                    }

                    StandardComponents<EntityVersion>.Value.RefInternal(entity).Value = 1;
                    entityVersionsCount++;
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
            public static Entity New<C1>() where C1 : struct, IComponent {
                var entity = New();
                entity.Add<C1>();
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                var entity = New();
                entity.Add<C1, C2>();
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                var entity = New();
                entity.Add<C1, C2, C3>();
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                var entity = New();
                entity.Add<C1, C2, C3, C4>();
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4, C5>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                var entity = New();
                entity.Add<C1, C2, C3, C4, C5>();
                return entity;
            }
            #endregion

            #region NEW_BY_VALUE_SINGLE
            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1>(C1 component) where C1 : struct, IComponent {
                var entity = New();
                entity.Put(component);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2>(C1 comp1, C2 comp2)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                var entity = New();
                entity.Put(comp1, comp2);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3>(C1 comp1, C2 comp2, C3 comp3)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                var entity = New();
                entity.Put(comp1, comp2, comp3);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4>(C1 comp1, C2 comp2, C3 comp3, C4 comp4)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                var entity = New();
                entity.Put(comp1, comp2, comp3, comp4);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4, C5>(C1 comp1, C2 comp2, C3 comp3, C4 comp4, C5 comp5)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                var entity = New();
                entity.Put(comp1, comp2, comp3, comp4, comp5);
                return entity;
            }
            #endregion

            #region NEW_BY_RAW_TYPE
            [MethodImpl(AggressiveInlining)]
            public static Entity New(Type componentType) {
                var entity = New();
                GetComponentsPool(componentType).Add(entity);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New(IComponent component) {
                var entity = New();
                GetComponentsPool(component.GetType()).PutRaw(entity, component);
                return entity;
            }
            #endregion

            #region NEW_BY_TYPE_BATCH
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || !FFS_ECS_LIFECYCLE_ENTITY
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes(uint count, Action<Entity> onCreate = null) {
                foreach (var entity in CreateEntitiesInternal(count)) {
                    onCreate?.Invoke(entity);
                }
            }
            #endif

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1>(uint count, Action<Entity> onCreate = null) where C1 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                components1.EnsureSize(count);
                foreach (var entity in CreateEntitiesInternal(count)) {
                    components1.AddDefault(entity);
                    onCreate?.Invoke(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2>(uint count, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                components1.EnsureSize(count);
                components2.EnsureSize(count);
                foreach (var entity in CreateEntitiesInternal(count)) {
                    components1.AddDefault(entity);
                    components2.AddDefault(entity);
                    onCreate?.Invoke(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3>(uint count, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                components1.EnsureSize(count);
                components2.EnsureSize(count);
                components3.EnsureSize(count);
                foreach (var entity in CreateEntitiesInternal(count)) {
                    components1.AddDefault(entity);
                    components2.AddDefault(entity);
                    components3.AddDefault(entity);
                    onCreate?.Invoke(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4>(uint count, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                components1.EnsureSize(count);
                components2.EnsureSize(count);
                components3.EnsureSize(count);
                components4.EnsureSize(count);
                foreach (var entity in CreateEntitiesInternal(count)) {
                    components1.AddDefault(entity);
                    components2.AddDefault(entity);
                    components3.AddDefault(entity);
                    components4.AddDefault(entity);
                    onCreate?.Invoke(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4, C5>(uint count, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                ref var components5 = ref Components<C5>.Value;
                components1.EnsureSize(count);
                components2.EnsureSize(count);
                components3.EnsureSize(count);
                components4.EnsureSize(count);
                components5.EnsureSize(count);
                foreach (var entity in CreateEntitiesInternal(count)) {
                    components1.AddDefault(entity);
                    components2.AddDefault(entity);
                    components3.AddDefault(entity);
                    components4.AddDefault(entity);
                    components5.AddDefault(entity);
                    onCreate?.Invoke(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1>(uint count, C1 c1, Action<Entity> onCreate = null) where C1 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                components1.EnsureSize(count);
                foreach (var entity in CreateEntitiesInternal(count)) {
                    components1.Put(entity, c1);
                    onCreate?.Invoke(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2>(uint count, C1 c1, C2 c2, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                components1.EnsureSize(count);
                components2.EnsureSize(count);
                foreach (var entity in CreateEntitiesInternal(count)) {
                    components1.Put(entity, c1);
                    components2.Put(entity, c2);
                    onCreate?.Invoke(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3>(uint count, C1 c1, C2 c2, C3 c3, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                components1.EnsureSize(count);
                components2.EnsureSize(count);
                components3.EnsureSize(count);
                foreach (var entity in CreateEntitiesInternal(count)) {
                    components1.Put(entity, c1);
                    components2.Put(entity, c2);
                    components3.Put(entity, c3);
                    onCreate?.Invoke(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                components1.EnsureSize(count);
                components2.EnsureSize(count);
                components3.EnsureSize(count);
                components4.EnsureSize(count);
                foreach (var entity in CreateEntitiesInternal(count)) {
                    components1.Put(entity, c1);
                    components2.Put(entity, c2);
                    components3.Put(entity, c3);
                    components4.Put(entity, c4);
                    onCreate?.Invoke(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4, C5>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, C5 c5, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                ref var components5 = ref Components<C5>.Value;
                components1.EnsureSize(count);
                components2.EnsureSize(count);
                components3.EnsureSize(count);
                components4.EnsureSize(count);
                components5.EnsureSize(count);
                foreach (var entity in CreateEntitiesInternal(count)) {
                    components1.Put(entity, c1);
                    components2.Put(entity, c2);
                    components3.Put(entity, c3);
                    components4.Put(entity, c4);
                    components5.Put(entity, c5);
                    onCreate?.Invoke(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4, C5, C6>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, C5 c5, C6 c6, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                ref var components5 = ref Components<C5>.Value;
                ref var components6 = ref Components<C6>.Value;
                components1.EnsureSize(count);
                components2.EnsureSize(count);
                components3.EnsureSize(count);
                components4.EnsureSize(count);
                components5.EnsureSize(count);
                components6.EnsureSize(count);
                foreach (var entity in CreateEntitiesInternal(count)) {
                    components1.Put(entity, c1);
                    components2.Put(entity, c2);
                    components3.Put(entity, c3);
                    components4.Put(entity, c4);
                    components5.Put(entity, c5);
                    components6.Put(entity, c6);
                    onCreate?.Invoke(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4, C5, C6, C7>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, C5 c5, C6 c6, C7 c7, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                ref var components5 = ref Components<C5>.Value;
                ref var components6 = ref Components<C6>.Value;
                ref var components7 = ref Components<C7>.Value;
                components1.EnsureSize(count);
                components2.EnsureSize(count);
                components3.EnsureSize(count);
                components4.EnsureSize(count);
                components5.EnsureSize(count);
                components6.EnsureSize(count);
                components7.EnsureSize(count);
                foreach (var entity in CreateEntitiesInternal(count)) {
                    components1.Put(entity, c1);
                    components2.Put(entity, c2);
                    components3.Put(entity, c3);
                    components4.Put(entity, c4);
                    components5.Put(entity, c5);
                    components6.Put(entity, c6);
                    components7.Put(entity, c7);
                    onCreate?.Invoke(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4, C5, C6, C7, C8>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, C5 c5, C6 c6, C7 c7, C8 c8, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                ref var components5 = ref Components<C5>.Value;
                ref var components6 = ref Components<C6>.Value;
                ref var components7 = ref Components<C7>.Value;
                ref var components8 = ref Components<C8>.Value;
                components1.EnsureSize(count);
                components2.EnsureSize(count);
                components3.EnsureSize(count);
                components4.EnsureSize(count);
                components5.EnsureSize(count);
                components6.EnsureSize(count);
                components7.EnsureSize(count);
                components8.EnsureSize(count);
                foreach (var entity in CreateEntitiesInternal(count)) {
                    components1.Put(entity, c1);
                    components2.Put(entity, c2);
                    components3.Put(entity, c3);
                    components4.Put(entity, c4);
                    components5.Put(entity, c5);
                    components6.Put(entity, c6);
                    components7.Put(entity, c7);
                    components8.Put(entity, c8);
                    onCreate?.Invoke(entity);
                }
            }
            #endregion

            [MethodImpl(AggressiveInlining)]
            public bool Equals(Entity entity) => _id == entity._id;

            [MethodImpl(AggressiveInlining)]
            public override bool Equals(object obj) => throw new Exception("Entity` Equals object` not allowed!");

            [MethodImpl(AggressiveInlining)]
            public override int GetHashCode() => (int) _id;

            [MethodImpl(AggressiveInlining)]
            public string ToPrettyString(bool withDisabled = true) {
                var result = $"Entity ID: {_id}\n";
                result += ModuleStandardComponents.Value.ToPrettyStringEntity(this);
                result += ModuleComponents.Value.ToPrettyStringEntity(this, withDisabled);
                #if !FFS_ECS_DISABLE_TAGS
                result += ModuleTags.Value.ToPrettyStringEntity(this);
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                result += ModuleMasks.Value.ToPrettyStringEntity(this);
                #endif
                return result;
            }

            public override string ToString() => $"Entity ID: {_id}";
            
            internal static void CreateEntities() {
                entitiesCapacity = cfg.BaseEntitiesCount;
                deletedEntities = new Entity[cfg.BaseDeletedEntitiesCount];
                entityVersionsCount = 0;
                deletedEntitiesCount = 0;
            }
            
            internal static void DestroyEntities() {
                for (var i = entityVersionsCount; i > 0; i--) {
                    var entity = FromIdx(i - 1);
                    if (StandardComponents<EntityVersion>.Value.RefInternal(entity).Value > 0) {
                        entity.Destroy();
                    }
                }
                deletedEntities = null;
                entityVersionsCount = 0;
                entitiesCapacity = 0;
                deletedEntitiesCount = 0;
            }
            
            internal static void ClearEntities() {
                Array.Clear(deletedEntities, 0, deletedEntities.Length);
                entityVersionsCount = 0;
                deletedEntitiesCount = 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal static CreateEntityEnumerator CreateEntitiesInternal(uint count) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new Exception($"World<{typeof(WorldType)}>, Method: CreateEntities, World not initialized");
                if (MultiThreadActive) throw new Exception($"World<{typeof(WorldType)}>, Method: CreateEntities, this operation is not supported in multithreaded mode");
                #endif
                int newEntitiesCount = (int) (count + 1 - (entitiesCapacity - entityVersionsCount + deletedEntitiesCount));
                if (newEntitiesCount > 0) {
                    entitiesCapacity = Utils.CalculateSize((uint) (entitiesCapacity + newEntitiesCount));
                    ModuleComponents.Value.Resize(entitiesCapacity);
                    ModuleStandardComponents.Value.Resize(entitiesCapacity);
                    #if !FFS_ECS_DISABLE_TAGS
                    ModuleTags.Value.Resize(entitiesCapacity);
                    #endif
                    #if !FFS_ECS_DISABLE_MASKS
                    ModuleMasks.Value.Resize(entitiesCapacity);
                    #endif

                    #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var listener in _debugEventListeners) {
                            listener.OnWorldResized(entitiesCapacity);
                        }
                    }
                    #endif
                }

                return new CreateEntityEnumerator(count);
            }

            internal struct CreateEntityEnumerator {
                private readonly EntityVersion[] entityVersions;
                private uint count;
                private Entity entity;

                [MethodImpl(AggressiveInlining)]
                public CreateEntityEnumerator(uint count) {
                    this.count = count;
                    entity = default;
                    entityVersions = StandardComponents<EntityVersion>.Value.Data();
                }

                public readonly Entity Current {
                    [MethodImpl(AggressiveInlining)] get => entity;
                }

                [MethodImpl(AggressiveInlining)]
                public bool MoveNext() {
                    if (count > 0) {
                        count--;
                        if (deletedEntitiesCount <= 0) {
                            entity = new Entity(entityVersionsCount);
                            entityVersions[entity._id].Value = 1;
                            entityVersionsCount++;
                        } else {
                            entity = deletedEntities[--deletedEntitiesCount];
                            entityVersions[entity._id].Value *= -1;
                        }
                        #if DEBUG || FFS_ECS_ENABLE_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
                        if (_debugEventListeners != null) {
                            foreach (var listener in _debugEventListeners) {
                                listener.OnEntityCreated(entity);
                            }
                        }
                        #endif
                        ModuleStandardComponents.Value.InitEntity(entity);
                        return true;
                    }

                    return false;
                }

                [MethodImpl(AggressiveInlining)]
                public readonly CreateEntityEnumerator GetEnumerator() => this;
            }
        }
    }

    public partial interface IEntity {
        internal uint GetId();

        internal Type WorldTypeType();

        internal IWorld World();

        public short Version();

        public bool IsActual();

        public PackedEntity Pack();

        public void Destroy();
    }

    public struct PackedEntity : IEquatable<PackedEntity> {
        internal uint _entity;
        internal short _version;

        [MethodImpl(AggressiveInlining)]
        public readonly bool TryUnpack<WorldType>(out World<WorldType>.Entity entity) where WorldType : struct, IWorldType {
            entity = World<WorldType>.Entity.FromIdx(_entity);
            return World<WorldType>.StandardComponents<EntityVersion>.Value.RefInternal(entity).Value == _version;
        }

        [MethodImpl(AggressiveInlining)]
        public bool Equals(PackedEntity entity) {
            return _entity.Equals(entity._entity) && _version == entity._version;
        }

        [MethodImpl(AggressiveInlining)]
        public override bool Equals(object obj) => throw new Exception("PackEntity `Equals object` not allowed!!");

        [MethodImpl(AggressiveInlining)]
        public override int GetHashCode() => throw new Exception("PackEntity `GetHashCode` not allowed!");

        [MethodImpl(AggressiveInlining)]
        public override string ToString() => $"PackEntity ID: {_entity}, Version: {_version}";
    }
}