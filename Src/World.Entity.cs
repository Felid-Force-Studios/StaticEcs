using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FFS.Libraries.StaticPack;
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

        [MethodImpl(AggressiveInlining)]
        public static uint EntitiesCount() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: EntitiesCount, World not initialized");
            #endif
            return Entity.entitiesCount;
        }

        [MethodImpl(AggressiveInlining)]
        public static uint EntitiesCountWithoutDestroyed() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: EntitiesCountWithoutDestroyed, World not initialized");
            #endif
            return Entity.entitiesCount - Entity.deletedEntitiesCount;
        }

        [MethodImpl(AggressiveInlining)]
        public static uint EntitiesCapacity() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            if (Status == WorldStatus.NotCreated) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: GetEntitiesCapacity, World not initialized");
            #endif
            return Entity.entitiesCapacity;
        }

        [MethodImpl(AggressiveInlining)]
        public static EntitiesIterator<WorldType> AllEntities() {
            return new EntitiesIterator<WorldType>(Entity.entitiesCount);
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        public readonly partial struct Entity : IEquatable<Entity>, IEntity {
            internal static Entity[] deletedEntities;
            internal static uint deletedEntitiesCount;
            internal static uint entitiesCapacity;
            internal static uint entitiesCount;

            internal readonly uint _id;

            [MethodImpl(AggressiveInlining)]
            internal Entity(uint id) {
                _id = id;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity FromIdx(uint idx) => new(idx);

            [MethodImpl(AggressiveInlining)]
            public EntityGID Gid() => GIDStore.Value.Get(this);

            uint IEntity.GetId() => _id;

            [MethodImpl(AggressiveInlining)]
            Type IEntity.WorldTypeType() => typeof(WorldType);

            [MethodImpl(AggressiveInlining)]
            IWorld IEntity.World() => Worlds.Get(typeof(WorldType));

            [MethodImpl(AggressiveInlining)]
            public bool IsActual() => GIDStore.Value.Has(this);

            [MethodImpl(AggressiveInlining)]
            public void UpVersion() => GIDStore.Value.IncrementVersion(this);

            public string PrettyString {
                get {
                    var builder = new StringBuilder(128);
                    builder.Append("Entity ID: ");
                    builder.Append(_id);
                    builder.Append(" GID: ");
                    var gid = GIDStore.Value.Get(this);
                    builder.Append(gid.Id());
                    builder.Append(" Version: ");
                    builder.Append(gid.Version());
                    builder.AppendLine();
                    ModuleStandardComponents.Value.ToPrettyStringEntity(builder, this);
                    ModuleComponents.Value.ToPrettyStringEntity(builder, this, true);
                    #if !FFS_ECS_DISABLE_TAGS
                    ModuleTags.Value.ToPrettyStringEntity(builder, this);
                    #endif
                    #if !FFS_ECS_DISABLE_MASKS
                    ModuleMasks.Value.ToPrettyStringEntity(builder, this);
                    #endif
                    return builder.ToString();
                }
            }

            [MethodImpl(AggressiveInlining)]
            public Entity Clone(bool withDisabled = true) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CloneEntity, World not initialized");
                #endif

                var dstEntity = New();
                CopyTo(dstEntity, withDisabled);

                return dstEntity;
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyTo(Entity dstEntity, bool withDisabled = true) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CopyEntityData, World not initialized");
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
            public static implicit operator EntityGID(Entity e) => e.Gid();

            [MethodImpl(AggressiveInlining)]
            public void Destroy() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: DestroyEntity, World not initialized");
                if (!IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: DestroyEntity, Entity already destroyed");
                #endif
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.DestroyEntity(this);
                #endif
                #if !FFS_ECS_DISABLE_MASKS
                ModuleMasks.Value.DestroyEntity(this);
                #endif
                ModuleComponents.Value.DestroyEntity(this);
                ModuleStandardComponents.Value.DestroyEntity(this);
                GIDStore.Value.DestroyEntity(this);
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
                if (IsWorldInitialized() && GIDStore.Value.Has(this)) {
                    Destroy();
                }
            }

            [MethodImpl(AggressiveInlining)]
            bool IEntity.TryAsEntityOf<WT>(out World<WT>.Entity entity) {
                if (typeof(WT) == typeof(WorldType)) {
                    entity = new World<WT>.Entity(_id);
                    return true;
                }
                
                entity = default;
                return false;
            }
            
            [MethodImpl(AggressiveInlining)]
            World<WT>.Entity IEntity.AsEntityOf<WT>() {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (typeof(WT) != typeof(WorldType)) throw new StaticEcsException($"Invalid cast: expected World<{typeof(WorldType).Name}>, got World<{typeof(WT).Name}>.");
                #endif
                
                return new World<WT>.Entity(_id);
            }

            #region NEW_BY_TYPE_SINGLE
            [MethodImpl(AggressiveInlining)]
            #if DEBUG || FFS_ECS_ENABLE_DEBUG || !FFS_ECS_LIFECYCLE_ENTITY
            public static Entity New() {
            #else
            internal static Entity New() {
            #endif
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CreateEntity, World not initialized");
                if (MultiThreadActive) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CreateEntity, this operation is not supported in multithreaded mode");
                #endif
                var entity = CreateEntity();
                GIDStore.Value.New(entity);
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
            public static Entity New(EntityGID gid) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CreateEntity, World not initialized");
                if (MultiThreadActive) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CreateEntity, this operation is not supported in multithreaded mode");
                #endif
                var entity = CreateEntity();
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                GIDStore.Value.Set(entity, gid, false);
                #else
                GIDStore.Value.Set(entity, gid);
                #endif
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
            private static Entity CreateEntity() {
                Entity entity;
                if (deletedEntitiesCount > 0) {
                    entity = deletedEntities[--deletedEntitiesCount];
                } else {
                    entity = new Entity(entitiesCount);
                    if (entitiesCount == entitiesCapacity) {
                        ResizeEntities(entitiesCapacity << 1);
                    }

                    entitiesCount++;
                }

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
            
            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4, C5, C6>(C1 comp1, C2 comp2, C3 comp3, C4 comp4, C5 comp5, C6 comp6)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                var entity = New();
                entity.Put(comp1, comp2, comp3, comp4, comp5);
                entity.Put(comp6);
                return entity;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4, C5, C6, C7>(C1 comp1, C2 comp2, C3 comp3, C4 comp4, C5 comp5, C6 comp6, C7 comp7)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                var entity = New();
                entity.Put(comp1, comp2, comp3, comp4, comp5);
                entity.Put(comp6, comp7);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4, C5, C6, C7, C8>(C1 comp1, C2 comp2, C3 comp3, C4 comp4, C5 comp5, C6 comp6, C7 comp7, C8 comp8)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                var entity = New();
                entity.Put(comp1, comp2, comp3, comp4, comp5);
                entity.Put(comp6, comp7, comp8);
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
                    components1.Add(entity);
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
                    components1.Add(entity);
                    components2.Add(entity);
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
                    components1.Add(entity);
                    components2.Add(entity);
                    components3.Add(entity);
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
                    components1.Add(entity);
                    components2.Add(entity);
                    components3.Add(entity);
                    components4.Add(entity);
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
                    components1.Add(entity);
                    components2.Add(entity);
                    components3.Add(entity);
                    components4.Add(entity);
                    components5.Add(entity);
                    onCreate?.Invoke(entity);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1>(uint count, C1 c1, Action<Entity> onCreate = null) where C1 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                components1.EnsureSize(count);
                foreach (var entity in CreateEntitiesInternal(count)) {
                    components1.Add(entity, c1);
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
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
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
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    components3.Add(entity, c3);
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
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    components3.Add(entity, c3);
                    components4.Add(entity, c4);
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
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    components3.Add(entity, c3);
                    components4.Add(entity, c4);
                    components5.Add(entity, c5);
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
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    components3.Add(entity, c3);
                    components4.Add(entity, c4);
                    components5.Add(entity, c5);
                    components6.Add(entity, c6);
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
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    components3.Add(entity, c3);
                    components4.Add(entity, c4);
                    components5.Add(entity, c5);
                    components6.Add(entity, c6);
                    components7.Add(entity, c7);
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
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    components3.Add(entity, c3);
                    components4.Add(entity, c4);
                    components5.Add(entity, c5);
                    components6.Add(entity, c6);
                    components7.Add(entity, c7);
                    components8.Add(entity, c8);
                    onCreate?.Invoke(entity);
                }
            }
            #endregion

            [MethodImpl(AggressiveInlining)]
            public bool Equals(Entity entity) => _id == entity._id;

            [MethodImpl(AggressiveInlining)]
            public override bool Equals(object obj) => throw new StaticEcsException("Entity` Equals object` not allowed!");

            [MethodImpl(AggressiveInlining)]
            public override int GetHashCode() => (int) _id;

            public override string ToString() => $"Entity ID: {_id}";

            internal static void CreateEntities() {
                entitiesCapacity = cfg.BaseEntitiesCount;
                deletedEntities = new Entity[cfg.BaseDeletedEntitiesCount];
                entitiesCount = 0;
                deletedEntitiesCount = 0;
                GIDStore.Value.Create(cfg);
            }

            internal static void InitializeEntities(ref GIDStore globalIdStore) {
                GIDStore.Value = globalIdStore;
            }

            internal static void DestroyEntities() {
                for (var i = entitiesCount; i > 0; i--) {
                    var entity = FromIdx(i - 1);
                    if (GIDStore.Value.Has(entity)) {
                        entity.Destroy();
                    }
                }

                deletedEntities = null;
                entitiesCount = 0;
                entitiesCapacity = 0;
                deletedEntitiesCount = 0;
                GIDStore.Value.Destroy();
            }

            internal static void ClearEntities() {
                GIDStore.Value.Clear();
                Array.Clear(deletedEntities, 0, deletedEntities.Length);
                entitiesCount = 0;
                deletedEntitiesCount = 0;
            }

            [MethodImpl(AggressiveInlining)]
            internal static CreateEntityEnumerator CreateEntitiesInternal(uint count) {
                #if DEBUG || FFS_ECS_ENABLE_DEBUG
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CreateEntities, World not initialized");
                if (MultiThreadActive) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CreateEntities, this operation is not supported in multithreaded mode");
                #endif
                int newEntitiesCount = (int) (count + 1 - (entitiesCapacity - entitiesCount + deletedEntitiesCount));
                if (newEntitiesCount > 0) {
                    ResizeEntities(Utils.CalculateSize((uint) (entitiesCapacity + newEntitiesCount)));
                }

                return new CreateEntityEnumerator(count);
            }

            private static void ResizeEntities(uint size) {
                entitiesCapacity = size;
                GIDStore.Value.ResizeEntities(entitiesCapacity);
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

            internal struct CreateEntityEnumerator {
                private uint count;
                private Entity entity;

                [MethodImpl(AggressiveInlining)]
                public CreateEntityEnumerator(uint count) {
                    this.count = count;
                    entity = default;
                }

                public readonly Entity Current {
                    [MethodImpl(AggressiveInlining)] get => entity;
                }

                [MethodImpl(AggressiveInlining)]
                public bool MoveNext() {
                    if (count > 0) {
                        count--;
                        if (deletedEntitiesCount <= 0) {
                            entity = new Entity(entitiesCount);
                            GIDStore.Value.New(entity);
                            entitiesCount++;
                        } else {
                            entity = deletedEntities[--deletedEntitiesCount];
                            GIDStore.Value.New(entity);
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
            
            [MethodImpl(AggressiveInlining)]
            internal static void Write(ref BinaryPackWriter writer) {
                writer.WriteUint(entitiesCapacity);
                writer.WriteUint(entitiesCount);
                writer.WriteUint(deletedEntitiesCount);
                writer.WriteArrayUnmanaged(deletedEntities, 0, (int) deletedEntitiesCount);
                GIDStore.Value.Write(ref writer);
            }
                
            [MethodImpl(AggressiveInlining)]
            internal static void Read(ref BinaryPackReader reader) {
                entitiesCapacity = reader.ReadUint();
                entitiesCount = reader.ReadUint();
                deletedEntitiesCount = reader.ReadUint();
                reader.ReadArrayUnmanaged(ref deletedEntities);
                GIDStore.Value.Read(ref reader);
            }
        }
    }

    public partial interface IEntity {
        public uint GetId();

        public Type WorldTypeType();

        public IWorld World();

        public EntityGID Gid();

        public bool IsActual();

        public void Destroy();

        public void TryDestroy();
        
        public bool TryAsEntityOf<WorldType>(out World<WorldType>.Entity entity) where WorldType : struct, IWorldType;
        
        public World<WorldType>.Entity AsEntityOf<WorldType>() where WorldType : struct, IWorldType;
    }
    
    

    #if ENABLE_IL2CPP
    [Il2CppSetOption (Option.NullChecks, false)]
    [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
    #endif
    public ref struct EntitiesIterator<WorldType> where WorldType : struct, IWorldType {
        private World<WorldType>.Entity _current;
        private uint _count;

        [MethodImpl(AggressiveInlining)]
        public EntitiesIterator(uint count) {
            _current = default;
            _count = count;
        }

        public readonly World<WorldType>.Entity Current {
            [MethodImpl(AggressiveInlining)] get => _current;
        }

        [MethodImpl(AggressiveInlining)]
        public bool MoveNext() {
            while (true) {
                if (_count == 0) {
                    return false;
                }

                _count--;
                _current = World<WorldType>.Entity.FromIdx(_count);

                if (_current.IsActual()) {
                    return true;
                }
            }
        }

        [MethodImpl(AggressiveInlining)]
        public readonly EntitiesIterator<WorldType> GetEnumerator() => this;

        [MethodImpl(AggressiveInlining)]
        public List<World<WorldType>.Entity> ToList() {
            var entities = new List<World<WorldType>.Entity>();
            while (MoveNext()) {
                entities.Add(_current);
            }

            return entities;
        }
    }
}