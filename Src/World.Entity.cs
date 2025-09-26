using System;
using System.Runtime.CompilerServices;
using System.Text;
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
        public partial struct Entity : IEquatable<Entity> {
            internal uint _id;

            [MethodImpl(AggressiveInlining)]
            internal Entity(uint id) => _id = id;

            [MethodImpl(AggressiveInlining)]
            public static Entity FromIdx(uint idx) => new(idx);

            [MethodImpl(AggressiveInlining)]
            public BoxedEntity<WorldType> Box() => new (this);

            [MethodImpl(AggressiveInlining)]
            public EntityGID Gid() => GIDStore.Value.Get(this);

            [MethodImpl(AggressiveInlining)]
            public bool IsActual() => GIDStore.Value.Has(this);

            [MethodImpl(AggressiveInlining)]
            public void UpVersion() => GIDStore.Value.IncrementVersion(this);

            public string PrettyString {
                get {
                    var builder = new StringBuilder(256);
                    builder.Append("Entity ID: ");
                    builder.Append(_id);
                    builder.Append(" GID: ");
                    var gid = GIDStore.Value.Get(this);
                    builder.Append(gid.Id());
                    builder.Append(" Version: ");
                    builder.Append(gid.Version());
                    if (IsDisabled()) {
                        builder.Append(" [Disabled]");
                    }
                    builder.AppendLine();
                    ModuleComponents.Value.ToPrettyStringEntity(builder, this);
                    #if !FFS_ECS_DISABLE_TAGS
                    ModuleTags.Value.ToPrettyStringEntity(builder, this);
                    #endif
                    return builder.ToString();
                }
            }

            [MethodImpl(AggressiveInlining)]
            public Entity Clone() {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CloneEntity, World not initialized");
                #endif

                var dstEntity = New();
                CopyTo(dstEntity);

                return dstEntity;
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyTo(Entity dstEntity) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>, Method: CopyEntityData, World not initialized");
                #endif

                ModuleComponents.Value.CopyEntity(this, dstEntity);
                #if !FFS_ECS_DISABLE_TAGS
                ModuleTags.Value.CopyEntity(this, dstEntity);
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            public void MoveTo(Entity dstEntity) {
                CopyTo(dstEntity);
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
            public bool IsDisabled() {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity Method: IsDisabled, World not initialized");
                if (!IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity, Method: IsDisabled, cannot call for deleted entity");
                #endif
                var chunkIdx = _id >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (_id & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityIdx = chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK;
                return (Entities.Value.chunks[chunkIdx].disabledEntities[blockIdx] & (1UL << blockEntityIdx)) != 0;
            }

            [MethodImpl(AggressiveInlining)]
            public bool IsEnabled() {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity Method: IsEnabled, World not initialized");
                if (!IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity, Method: IsEnabled, cannot call for deleted entity");
                #endif
                var chunkIdx = _id >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (_id & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityIdx = chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK;
                return (Entities.Value.chunks[chunkIdx].disabledEntities[blockIdx] & (1UL << blockEntityIdx)) == 0;
            }

            [MethodImpl(AggressiveInlining)]
            public void Disable() {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity Method: Disable, World not initialized");
                if (!IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity, Method: Disable, cannot call for deleted entity");
                if (Entities.Value._blockerDisable > 0 && CurrentQuery.IsNotCurrentEntity(this)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity, Method: Disable, is blocked, use QueryMode.Flexible");
                if (MultiThreadActive && CurrentQuery.IsNotCurrentEntity(this)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity, Method: Disable, is blocked, it is forbidden to modify a non-current entity in a parallel query");
                #endif
                var chunkIdx = _id >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (_id & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityIdx = chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK;
                Entities.Value.chunks[chunkIdx].disabledEntities[blockIdx] |= 1UL << blockEntityIdx;
                for (uint i = 0; i < Entities.Value.queriesToUpdateOnDisableCount; i++) {
                    Entities.Value.queriesToUpdateOnDisable[i].Update(~(1UL << blockEntityIdx), _id);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public void Enable() {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity Method: Enable, World not initialized");
                if (!IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity, Method: Enable, cannot call for deleted entity");
                if (Entities.Value._blockerEnable > 0 && CurrentQuery.IsNotCurrentEntity(this)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity, Method: Enable, is blocked, use QueryMode.Flexible");
                if (MultiThreadActive && CurrentQuery.IsNotCurrentEntity(this)) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity, Method: Enable, is blocked, it is forbidden to modify a non-current entity in a parallel query");
                #endif
                var chunkIdx = _id >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var chunkEntityIdx = (ushort) (_id & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);
                var blockIdx = chunkEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT;
                var blockEntityIdx = chunkEntityIdx & Const.ENTITIES_IN_BLOCK_OFFSET_MASK;
                Entities.Value.chunks[chunkIdx].disabledEntities[blockIdx] &= ~(1UL << blockEntityIdx);
                for (uint i = 0; i < Entities.Value.queriesToUpdateOnEnableCount; i++) {
                    Entities.Value.queriesToUpdateOnEnable[i].Update(~(1UL << blockEntityIdx), _id);
                }
            }

            [MethodImpl(AggressiveInlining)]
            public void Destroy() {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity Method: Destroy, World not initialized");
                if (!IsActual()) throw new StaticEcsException($"World<{typeof(WorldType)}>.Entity, Method: Destroy, cannot call for deleted entity");
                #endif
                Entities.Value.DestroyEntity(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void TryDestroy() {
                if (IsWorldInitialized() && GIDStore.Value.Has(this)) {
                    Destroy();
                }
            }

            [MethodImpl(AggressiveInlining)]
            public bool Equals(Entity entity) => _id == entity._id;

            [MethodImpl(AggressiveInlining)]
            public override bool Equals(object obj) => throw new StaticEcsException("Entity` Equals object` not allowed!");

            [MethodImpl(AggressiveInlining)]
            public override int GetHashCode() => (int) _id;

            public override string ToString() => $"Entity ID: {_id}";
            
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || !FFS_ECS_LIFECYCLE_ENTITY
            [MethodImpl(AggressiveInlining)]
            public static Entity New() => new(Entities.Value.CreateEntity());
            #endif
            
            [MethodImpl(AggressiveInlining)]
            public static Entity New(EntityGID gid) => new(Entities.Value.CreateEntity(gid));
            
            #region NEW_BY_TYPE_SINGLE
            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1>() where C1 : struct, IComponent {
                var entity = new Entity(Entities.Value.CreateEntity());
                Components<C1>.Value.Add(entity);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                var entity = new Entity(Entities.Value.CreateEntity());
                Components<C1>.Value.Add(entity);
                Components<C2>.Value.Add(entity);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                var entity = new Entity(Entities.Value.CreateEntity());
                Components<C1>.Value.Add(entity);
                Components<C2>.Value.Add(entity);
                Components<C3>.Value.Add(entity);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                var entity = new Entity(Entities.Value.CreateEntity());
                Components<C1>.Value.Add(entity);
                Components<C2>.Value.Add(entity);
                Components<C3>.Value.Add(entity);
                Components<C4>.Value.Add(entity);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4, C5>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                var entity = new Entity(Entities.Value.CreateEntity());
                Components<C1>.Value.Add(entity);
                Components<C2>.Value.Add(entity);
                Components<C3>.Value.Add(entity);
                Components<C4>.Value.Add(entity);
                Components<C5>.Value.Add(entity);
                return entity;
            }
            #endregion
            
            #region NEW_BY_VALUE_SINGLE
            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1>(C1 component) where C1 : struct, IComponent {
                var entity = new Entity(Entities.Value.CreateEntity());
                Components<C1>.Value.Put(entity, component);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2>(C1 comp1, C2 comp2)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                var entity = new Entity(Entities.Value.CreateEntity());
                Components<C1>.Value.Put(entity, comp1);
                Components<C2>.Value.Put(entity, comp2);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3>(C1 comp1, C2 comp2, C3 comp3)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                var entity = new Entity(Entities.Value.CreateEntity());
                Components<C1>.Value.Put(entity, comp1);
                Components<C2>.Value.Put(entity, comp2);
                Components<C3>.Value.Put(entity, comp3);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4>(C1 comp1, C2 comp2, C3 comp3, C4 comp4)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                var entity = new Entity(Entities.Value.CreateEntity());
                Components<C1>.Value.Put(entity, comp1);
                Components<C2>.Value.Put(entity, comp2);
                Components<C3>.Value.Put(entity, comp3);
                Components<C4>.Value.Put(entity, comp4);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4, C5>(C1 comp1, C2 comp2, C3 comp3, C4 comp4, C5 comp5)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                var entity = new Entity(Entities.Value.CreateEntity());
                Components<C1>.Value.Put(entity, comp1);
                Components<C2>.Value.Put(entity, comp2);
                Components<C3>.Value.Put(entity, comp3);
                Components<C4>.Value.Put(entity, comp4);
                Components<C5>.Value.Put(entity, comp5);
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
                var entity = new Entity(Entities.Value.CreateEntity());
                Components<C1>.Value.Put(entity, comp1);
                Components<C2>.Value.Put(entity, comp2);
                Components<C3>.Value.Put(entity, comp3);
                Components<C4>.Value.Put(entity, comp4);
                Components<C5>.Value.Put(entity, comp5);
                Components<C6>.Value.Put(entity, comp6);
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
                var entity = new Entity(Entities.Value.CreateEntity());
                Components<C1>.Value.Put(entity, comp1);
                Components<C2>.Value.Put(entity, comp2);
                Components<C3>.Value.Put(entity, comp3);
                Components<C4>.Value.Put(entity, comp4);
                Components<C5>.Value.Put(entity, comp5);
                Components<C6>.Value.Put(entity, comp6);
                Components<C7>.Value.Put(entity, comp7);
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
                var entity = new Entity(Entities.Value.CreateEntity());
                Components<C1>.Value.Put(entity, comp1);
                Components<C2>.Value.Put(entity, comp2);
                Components<C3>.Value.Put(entity, comp3);
                Components<C4>.Value.Put(entity, comp4);
                Components<C5>.Value.Put(entity, comp5);
                Components<C6>.Value.Put(entity, comp6);
                Components<C7>.Value.Put(entity, comp7);
                Components<C8>.Value.Put(entity, comp8);
                return entity;
            }
            #endregion
            
            #region NEW_BY_TYPE_BATCH
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || !FFS_ECS_LIFECYCLE_ENTITY
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes(uint count, QueryFunctionWithEntity<WorldType> onCreate) {
                ref var entities = ref Entities.Value;

                var entity = new Entity();
                ref var eid = ref entity._id;
                while (count > 0) {
                    count--;
                    eid = entities.CreateEntityInternal();
                    GIDStore.Value.New(eid);
                    onCreate(entity);

                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var val in _debugEventListeners) val.OnEntityCreated(entity);
                    }
                    #endif
                }
            }
            #endif
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1>(uint count, QueryFunctionWithEntity<WorldType> onCreate = null) where C1 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var entities = ref Entities.Value;

                var entity = new Entity();
                ref var eid = ref entity._id;
                while (count > 0) {
                    count--;
                    eid = entities.CreateEntityInternal();
                    GIDStore.Value.New(eid);
                    components1.Add(entity);
                    onCreate?.Invoke(entity);

                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var val in _debugEventListeners) val.OnEntityCreated(entity);
                    }
                    #endif
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2>(uint count, QueryFunctionWithEntity<WorldType> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var entities = ref Entities.Value;

                var entity = new Entity();
                ref var eid = ref entity._id;
                while (count > 0) {
                    count--;
                    eid = entities.CreateEntityInternal();
                    GIDStore.Value.New(eid);
                    components1.Add(entity);
                    components2.Add(entity);
                    onCreate?.Invoke(entity);

                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var val in _debugEventListeners) val.OnEntityCreated(entity);
                    }
                    #endif
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3>(uint count, QueryFunctionWithEntity<WorldType> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var entities = ref Entities.Value;

                var entity = new Entity();
                ref var eid = ref entity._id;
                while (count > 0) {
                    count--;
                    eid = entities.CreateEntityInternal();
                    GIDStore.Value.New(eid);
                    components1.Add(entity);
                    components2.Add(entity);
                    components3.Add(entity);
                    onCreate?.Invoke(entity);

                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var val in _debugEventListeners) val.OnEntityCreated(entity);
                    }
                    #endif
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4>(uint count, QueryFunctionWithEntity<WorldType> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                ref var entities = ref Entities.Value;

                var entity = new Entity();
                ref var eid = ref entity._id;
                while (count > 0) {
                    count--;
                    eid = entities.CreateEntityInternal();
                    GIDStore.Value.New(eid);
                    components1.Add(entity);
                    components2.Add(entity);
                    components3.Add(entity);
                    components4.Add(entity);
                    onCreate?.Invoke(entity);

                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var val in _debugEventListeners) val.OnEntityCreated(entity);
                    }
                    #endif
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4, C5>(uint count, QueryFunctionWithEntity<WorldType> onCreate = null)
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
                ref var entities = ref Entities.Value;

                var entity = new Entity();
                ref var eid = ref entity._id;
                while (count > 0) {
                    count--;
                    eid = entities.CreateEntityInternal();
                    GIDStore.Value.New(eid);
                    components1.Add(entity);
                    components2.Add(entity);
                    components3.Add(entity);
                    components4.Add(entity);
                    components5.Add(entity);
                    onCreate?.Invoke(entity);

                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var val in _debugEventListeners) val.OnEntityCreated(entity);
                    }
                    #endif
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1>(uint count, C1 c1, QueryFunctionWithEntity<WorldType> onCreate = null) where C1 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var entities = ref Entities.Value;

                var entity = new Entity();
                ref var eid = ref entity._id;
                while (count > 0) {
                    count--;
                    eid = entities.CreateEntityInternal();
                    GIDStore.Value.New(eid);
                    components1.Add(entity, c1);
                    onCreate?.Invoke(entity);

                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var val in _debugEventListeners) val.OnEntityCreated(entity);
                    }
                    #endif
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2>(uint count, C1 c1, C2 c2, QueryFunctionWithEntity<WorldType> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var entities = ref Entities.Value;

                var entity = new Entity();
                ref var eid = ref entity._id;
                while (count > 0) {
                    count--;
                    eid = entities.CreateEntityInternal();
                    GIDStore.Value.New(eid);
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    onCreate?.Invoke(entity);

                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var val in _debugEventListeners) val.OnEntityCreated(entity);
                    }
                    #endif
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3>(uint count, C1 c1, C2 c2, C3 c3, QueryFunctionWithEntity<WorldType> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var entities = ref Entities.Value;

                var entity = new Entity();
                ref var eid = ref entity._id;
                while (count > 0) {
                    count--;
                    eid = entities.CreateEntityInternal();
                    GIDStore.Value.New(eid);
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    components3.Add(entity, c3);
                    onCreate?.Invoke(entity);

                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var val in _debugEventListeners) val.OnEntityCreated(entity);
                    }
                    #endif
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, QueryFunctionWithEntity<WorldType> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                ref var entities = ref Entities.Value;

                var entity = new Entity();
                ref var eid = ref entity._id;
                while (count > 0) {
                    count--;
                    eid = entities.CreateEntityInternal();
                    GIDStore.Value.New(eid);
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    components3.Add(entity, c3);
                    components4.Add(entity, c4);
                    onCreate?.Invoke(entity);

                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var val in _debugEventListeners) val.OnEntityCreated(entity);
                    }
                    #endif
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4, C5>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, C5 c5, QueryFunctionWithEntity<WorldType> onCreate = null)
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
                ref var entities = ref Entities.Value;

                var entity = new Entity();
                ref var eid = ref entity._id;
                while (count > 0) {
                    count--;
                    eid = entities.CreateEntityInternal();
                    GIDStore.Value.New(eid);
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    components3.Add(entity, c3);
                    components4.Add(entity, c4);
                    components5.Add(entity, c5);
                    onCreate?.Invoke(entity);

                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var val in _debugEventListeners) val.OnEntityCreated(entity);
                    }
                    #endif
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4, C5, C6>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, C5 c5, C6 c6, QueryFunctionWithEntity<WorldType> onCreate = null)
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
                ref var entities = ref Entities.Value;

                var entity = new Entity();
                ref var eid = ref entity._id;
                while (count > 0) {
                    count--;
                    eid = entities.CreateEntityInternal();
                    GIDStore.Value.New(eid);
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    components3.Add(entity, c3);
                    components4.Add(entity, c4);
                    components5.Add(entity, c5);
                    components6.Add(entity, c6);
                    onCreate?.Invoke(entity);

                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var val in _debugEventListeners) val.OnEntityCreated(entity);
                    }
                    #endif
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4, C5, C6, C7>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, C5 c5, C6 c6, C7 c7, QueryFunctionWithEntity<WorldType> onCreate = null)
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
                ref var entities = ref Entities.Value;

                var entity = new Entity();
                ref var eid = ref entity._id;
                while (count > 0) {
                    count--;
                    eid = entities.CreateEntityInternal();
                    GIDStore.Value.New(eid);
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    components3.Add(entity, c3);
                    components4.Add(entity, c4);
                    components5.Add(entity, c5);
                    components6.Add(entity, c6);
                    components7.Add(entity, c7);
                    onCreate?.Invoke(entity);

                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var val in _debugEventListeners) val.OnEntityCreated(entity);
                    }
                    #endif
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4, C5, C6, C7, C8>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, C5 c5, C6 c6, C7 c7, C8 c8, QueryFunctionWithEntity<WorldType> onCreate = null)
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
                ref var entities = ref Entities.Value;

                var entity = new Entity();
                ref var eid = ref entity._id;
                while (count > 0) {
                    count--;
                    eid = entities.CreateEntityInternal();
                    GIDStore.Value.New(eid);
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    components3.Add(entity, c3);
                    components4.Add(entity, c4);
                    components5.Add(entity, c5);
                    components6.Add(entity, c6);
                    components7.Add(entity, c7);
                    components8.Add(entity, c8);
                    onCreate?.Invoke(entity);

                    #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG) || FFS_ECS_ENABLE_DEBUG_EVENTS
                    if (_debugEventListeners != null) {
                        foreach (var val in _debugEventListeners) val.OnEntityCreated(entity);
                    }
                    #endif
                }
            }
            #endregion

            #region NEW_BY_RAW_TYPE
            [MethodImpl(AggressiveInlining)]
            public static Entity New(Type componentType) {
                var entity = Entities.Value.CreateEntity();
                GetComponentsPool(componentType).Add(entity);
                return new Entity(entity);
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New(IComponent component) {
                var entity = Entities.Value.CreateEntity();
                GetComponentsPool(component.GetType()).PutRaw(entity, component);
                return new Entity(entity);
            }
            #endregion
        }
    }

    public partial interface IEntity {
        public uint GetId();

        public Type WorldTypeType();

        public IWorld World();

        public EntityGID Gid();

        public bool IsActual();
        
        public bool IsEnabled();
        
        public bool IsDisabled();
        
        public void Enable();
        
        public void Disable();

        public void Destroy();

        public void TryDestroy();
        
        public bool TryAsEntityOf<WorldType>(out World<WorldType>.Entity entity) where WorldType : struct, IWorldType;
        
        public World<WorldType>.Entity AsEntityOf<WorldType>() where WorldType : struct, IWorldType;
    }

    public partial class BoxedEntity<WorldType> : IEntity where WorldType : struct, IWorldType {
        private readonly uint _entity;
        
        public BoxedEntity(World<WorldType>.Entity entity) {
            _entity = entity._id;
        }

        public uint GetId() => _entity;

        public Type WorldTypeType() => typeof(WorldType);

        public IWorld World() => Worlds.Get(typeof(WorldType));
        
        public EntityGID Gid() => new World<WorldType>.Entity(_entity).Gid();
        
        public bool IsActual() => new World<WorldType>.Entity(_entity).IsActual();
        
        public bool IsEnabled() => new World<WorldType>.Entity(_entity).IsEnabled();
        
        public bool IsDisabled() => new World<WorldType>.Entity(_entity).IsDisabled();
        
        public void Enable() => new World<WorldType>.Entity(_entity).Enable();
        
        public void Disable() => new World<WorldType>.Entity(_entity).Disable();
        
        public void Destroy() => new World<WorldType>.Entity(_entity).Destroy();
        
        public void TryDestroy() => new World<WorldType>.Entity(_entity).TryDestroy();
        
        public bool TryAsEntityOf<WT>(out World<WT>.Entity entity) where WT : struct, IWorldType {
            if (typeof(WT) == typeof(WorldType)) {
                entity = new World<WT>.Entity(_entity);
                return true;
            }
                
            entity = default;
            return false;
        }
        
        public World<WT>.Entity AsEntityOf<WT>() where WT : struct, IWorldType {
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (typeof(WT) != typeof(WorldType)) throw new StaticEcsException($"Invalid cast: expected World<{typeof(WorldType).Name}>, got World<{typeof(WT).Name}>.");
            #endif
                
            return new World<WT>.Entity(_entity);
        }
    } 
}