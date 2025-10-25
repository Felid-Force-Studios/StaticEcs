#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

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
            internal uint id;

            [MethodImpl(AggressiveInlining)]
            internal Entity(uint id) => this.id = id + Const.ENTITY_ID_OFFSET;

            [MethodImpl(AggressiveInlining)]
            public static Entity FromIdx(uint idx) => new(idx);

            [MethodImpl(AggressiveInlining)]
            public BoxedEntity<WorldType> Box() => new(this);

            [MethodImpl(AggressiveInlining)]
            public EntityGID Gid() => Entities.Value.EntityGID(this);

            [MethodImpl(AggressiveInlining)]
            public EntityGIDCompact GidCompact() => Entities.Value.EntityGIDCompact(this);

            [MethodImpl(AggressiveInlining)]
            public bool IsDestroyed() => !Entities.Value.EntityIsNotDestroyed(this);

            [MethodImpl(AggressiveInlining)]
            public bool IsNotDestroyed() => Entities.Value.EntityIsNotDestroyed(this);

            [MethodImpl(AggressiveInlining)]
            public void UpVersion() => Entities.Value.UpEntityVersion(this);

            [MethodImpl(AggressiveInlining)]
            public ushort Version() => Entities.Value.EntityVersion(this);

            [MethodImpl(AggressiveInlining)]
            public ushort ClusterId() => Entities.Value.EntityClusterId(this);

            [MethodImpl(AggressiveInlining)]
            public bool IsSelfOwned() => Entities.Value.EntityIsSelfOwned(this);

            [MethodImpl(AggressiveInlining)]
            public uint Chunk() {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, this);
                #endif
                return (id - Const.ENTITY_ID_OFFSET) >> Const.ENTITIES_IN_CHUNK_SHIFT;
            }

            public string PrettyString {
                get {
                    #if FFS_ECS_DEBUG
                    AssertWorldIsInitialized(EntityTypeName);
                    AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, this);
                    #endif
                    var builder = new StringBuilder(256);
                    builder.Append("Entity ID: ");
                    builder.Append(id - Const.ENTITY_ID_OFFSET);
                    builder.Append(" Version: ");
                    builder.Append(Version());
                    builder.Append(" Cluster ID: ");
                    builder.Append(ClusterId());
                    builder.Append(" Owner: ");
                    builder.Append(IsSelfOwned() ? "Self" : "Other");
                    if (IsDisabled()) {
                        builder.Append(" [Disabled]");
                    }
                    builder.AppendLine();
                    ModuleComponents.Value.ToPrettyStringEntity(builder, this);
                    ModuleTags.Value.ToPrettyStringEntity(builder, this);
                    return builder.ToString();
                }
            }

            [MethodImpl(AggressiveInlining)]
            public Entity Clone() {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, this);
                #endif

                var dstEntity = New(ClusterId());
                CopyTo(dstEntity);

                return dstEntity;
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyTo(Entity dstEntity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, this);
                #endif

                ModuleComponents.Value.CopyEntity(this, dstEntity);
                ModuleTags.Value.CopyEntity(this, dstEntity);
            }

            [MethodImpl(AggressiveInlining)]
            public void MoveTo(Entity dstEntity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, this);
                #endif
                
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
            public static implicit operator EntityGIDCompact(Entity e) => e.GidCompact();
            
            [MethodImpl(AggressiveInlining)]
            public bool IsDisabled() => Entities.Value.IsDisabledEntity(this);

            [MethodImpl(AggressiveInlining)]
            public bool IsEnabled() => Entities.Value.IsEnabledEntity(this);

            [MethodImpl(AggressiveInlining)]
            public void Disable() => Entities.Value.DisableEntity(this);

            [MethodImpl(AggressiveInlining)]
            public void Enable() => Entities.Value.EnableEntity(this);

            [MethodImpl(AggressiveInlining)]
            public void Destroy() => Entities.Value.DestroyEntity(this);

            [MethodImpl(AggressiveInlining)]
            public void Unload() => Entities.Value.UnloadEntity(this);

            [MethodImpl(AggressiveInlining)]
            public void TryDestroy() {
                if (IsWorldInitialized() && Entities.Value.EntityIsLoaded(this) && IsNotDestroyed()) {
                    Destroy();
                }
            }

            [MethodImpl(AggressiveInlining)]
            public bool Equals(Entity entity) => id == entity.id;

            [MethodImpl(AggressiveInlining)]
            public override bool Equals(object obj) => throw new StaticEcsException("Entity` Equals object` not allowed!");

            [MethodImpl(AggressiveInlining)]
            public override int GetHashCode() => (int) id;

            public override string ToString() => $"Entity ID: {id - Const.ENTITY_ID_OFFSET}";
            
            [MethodImpl(AggressiveInlining)]
            public static Entity New(ushort clusterId = DEFAULT_CLUSTER) {
                Entities.Value.CreateEntity(clusterId, out var entity);
                return entity;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static Entity New(uint chunkIdx) {
                Entities.Value.CreateEntity(chunkIdx, out var entity);
                return entity;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static bool TryNew(out Entity entity, ushort clusterId) => Entities.Value.TryCreateEntity(clusterId, out entity);

            [MethodImpl(AggressiveInlining)]
            public static bool TryNew(out Entity entity, uint chunkIdx) => Entities.Value.TryCreateEntity(chunkIdx, out entity);

            [MethodImpl(AggressiveInlining)]
            public static Entity New(EntityGID gid) {
                Entities.Value.CreateEntity(gid, out var entity);
                return entity;
            }

            #region NEW_BY_TYPE_SINGLE
            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1>(ushort clusterId = DEFAULT_CLUSTER) where C1 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                Entities.Value.CreateEntity(clusterId, out var entity);
                Components<C1>.Value.Add(entity);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2>(ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                Entities.Value.CreateEntity(clusterId, out var entity);
                Components<C1>.Value.Add(entity);
                Components<C2>.Value.Add(entity);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3>(ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                Entities.Value.CreateEntity(clusterId, out var entity);
                Components<C1>.Value.Add(entity);
                Components<C2>.Value.Add(entity);
                Components<C3>.Value.Add(entity);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4>(ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                Entities.Value.CreateEntity(clusterId, out var entity);
                Components<C1>.Value.Add(entity);
                Components<C2>.Value.Add(entity);
                Components<C3>.Value.Add(entity);
                Components<C4>.Value.Add(entity);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4, C5>(ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                Entities.Value.CreateEntity(clusterId, out var entity);
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
            public static Entity New<C1>(C1 component, ushort clusterId = DEFAULT_CLUSTER) where C1 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                Entities.Value.CreateEntity(clusterId, out var entity);
                Components<C1>.Value.Add(entity, component);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2>(C1 comp1, C2 comp2, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                Entities.Value.CreateEntity(clusterId, out var entity);
                Components<C1>.Value.Add(entity, comp1);
                Components<C2>.Value.Add(entity, comp2);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3>(C1 comp1, C2 comp2, C3 comp3, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                Entities.Value.CreateEntity(clusterId, out var entity);
                Components<C1>.Value.Add(entity, comp1);
                Components<C2>.Value.Add(entity, comp2);
                Components<C3>.Value.Add(entity, comp3);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4>(C1 comp1, C2 comp2, C3 comp3, C4 comp4, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                Entities.Value.CreateEntity(clusterId, out var entity);
                Components<C1>.Value.Add(entity, comp1);
                Components<C2>.Value.Add(entity, comp2);
                Components<C3>.Value.Add(entity, comp3);
                Components<C4>.Value.Add(entity, comp4);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4, C5>(C1 comp1, C2 comp2, C3 comp3, C4 comp4, C5 comp5, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                Entities.Value.CreateEntity(clusterId, out var entity);
                Components<C1>.Value.Add(entity, comp1);
                Components<C2>.Value.Add(entity, comp2);
                Components<C3>.Value.Add(entity, comp3);
                Components<C4>.Value.Add(entity, comp4);
                Components<C5>.Value.Add(entity, comp5);
                return entity;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4, C5, C6>(C1 comp1, C2 comp2, C3 comp3, C4 comp4, C5 comp5, C6 comp6, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                Entities.Value.CreateEntity(clusterId, out var entity);
                Components<C1>.Value.Add(entity, comp1);
                Components<C2>.Value.Add(entity, comp2);
                Components<C3>.Value.Add(entity, comp3);
                Components<C4>.Value.Add(entity, comp4);
                Components<C5>.Value.Add(entity, comp5);
                Components<C6>.Value.Add(entity, comp6);
                return entity;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4, C5, C6, C7>(C1 comp1, C2 comp2, C3 comp3, C4 comp4, C5 comp5, C6 comp6, C7 comp7, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                Entities.Value.CreateEntity(clusterId, out var entity);
                Components<C1>.Value.Add(entity, comp1);
                Components<C2>.Value.Add(entity, comp2);
                Components<C3>.Value.Add(entity, comp3);
                Components<C4>.Value.Add(entity, comp4);
                Components<C5>.Value.Add(entity, comp5);
                Components<C6>.Value.Add(entity, comp6);
                Components<C7>.Value.Add(entity, comp7);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4, C5, C6, C7, C8>(C1 comp1, C2 comp2, C3 comp3, C4 comp4, C5 comp5, C6 comp6, C7 comp7, C8 comp8, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                Entities.Value.CreateEntity(clusterId, out var entity);
                Components<C1>.Value.Add(entity, comp1);
                Components<C2>.Value.Add(entity, comp2);
                Components<C3>.Value.Add(entity, comp3);
                Components<C4>.Value.Add(entity, comp4);
                Components<C5>.Value.Add(entity, comp5);
                Components<C6>.Value.Add(entity, comp6);
                Components<C7>.Value.Add(entity, comp7);
                Components<C8>.Value.Add(entity, comp8);
                return entity;
            }
            #endregion
            
            #region NEW_BY_TYPE_BATCH
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes(uint count, QueryFunctionWithEntity<WorldType> onCreate, ushort clusterId = DEFAULT_CLUSTER) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                ref var entities = ref Entities.Value;

                Entity entity;
                while (count > 0) {
                    count--;
                    entities.CreateEntity(clusterId, out entity);
                    onCreate(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1>(uint count, QueryFunctionWithEntity<WorldType> onCreate = null, ushort clusterId = DEFAULT_CLUSTER) where C1 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                ref var components1 = ref Components<C1>.Value;
                ref var entities = ref Entities.Value;

                Entity entity;
                while (count > 0) {
                    count--;
                    entities.CreateEntity(clusterId, out entity);
                    components1.Add(entity);
                    onCreate?.Invoke(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2>(uint count, QueryFunctionWithEntity<WorldType> onCreate = null, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var entities = ref Entities.Value;

                Entity entity;
                while (count > 0) {
                    count--;
                    entities.CreateEntity(clusterId, out entity);
                    components1.Add(entity);
                    components2.Add(entity);
                    onCreate?.Invoke(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3>(uint count, QueryFunctionWithEntity<WorldType> onCreate = null, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var entities = ref Entities.Value;

                Entity entity;
                while (count > 0) {
                    count--;
                    entities.CreateEntity(clusterId, out entity);
                    components1.Add(entity);
                    components2.Add(entity);
                    components3.Add(entity);
                    onCreate?.Invoke(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4>(uint count, QueryFunctionWithEntity<WorldType> onCreate = null, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                ref var entities = ref Entities.Value;

                Entity entity;
                while (count > 0) {
                    count--;
                    entities.CreateEntity(clusterId, out entity);
                    components1.Add(entity);
                    components2.Add(entity);
                    components3.Add(entity);
                    components4.Add(entity);
                    onCreate?.Invoke(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4, C5>(uint count, QueryFunctionWithEntity<WorldType> onCreate = null, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                ref var components5 = ref Components<C5>.Value;
                ref var entities = ref Entities.Value;

                Entity entity;
                while (count > 0) {
                    count--;
                    entities.CreateEntity(clusterId, out entity);
                    components1.Add(entity);
                    components2.Add(entity);
                    components3.Add(entity);
                    components4.Add(entity);
                    components5.Add(entity);
                    onCreate?.Invoke(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1>(uint count, C1 c1, QueryFunctionWithEntity<WorldType> onCreate = null, ushort clusterId = DEFAULT_CLUSTER) where C1 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                ref var components1 = ref Components<C1>.Value;
                ref var entities = ref Entities.Value;

                Entity entity;
                while (count > 0) {
                    count--;
                    entities.CreateEntity(clusterId, out entity);
                    components1.Add(entity, c1);
                    onCreate?.Invoke(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2>(uint count, C1 c1, C2 c2, QueryFunctionWithEntity<WorldType> onCreate = null, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var entities = ref Entities.Value;

                Entity entity;
                while (count > 0) {
                    count--;
                    entities.CreateEntity(clusterId, out entity);
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    onCreate?.Invoke(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3>(uint count, C1 c1, C2 c2, C3 c3, QueryFunctionWithEntity<WorldType> onCreate = null, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var entities = ref Entities.Value;

                Entity entity;
                while (count > 0) {
                    count--;
                    entities.CreateEntity(clusterId, out entity);
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    components3.Add(entity, c3);
                    onCreate?.Invoke(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, QueryFunctionWithEntity<WorldType> onCreate = null, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                ref var entities = ref Entities.Value;

                Entity entity;
                while (count > 0) {
                    count--;
                    entities.CreateEntity(clusterId, out entity);
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    components3.Add(entity, c3);
                    components4.Add(entity, c4);
                    onCreate?.Invoke(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4, C5>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, C5 c5, QueryFunctionWithEntity<WorldType> onCreate = null, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                ref var components5 = ref Components<C5>.Value;
                ref var entities = ref Entities.Value;

                Entity entity;
                while (count > 0) {
                    count--;
                    entities.CreateEntity(clusterId, out entity);
                    components1.Add(entity, c1);
                    components2.Add(entity, c2);
                    components3.Add(entity, c3);
                    components4.Add(entity, c4);
                    components5.Add(entity, c5);
                    onCreate?.Invoke(entity);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4, C5, C6>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, C5 c5, C6 c6, QueryFunctionWithEntity<WorldType> onCreate = null, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                ref var components5 = ref Components<C5>.Value;
                ref var components6 = ref Components<C6>.Value;
                ref var entities = ref Entities.Value;

                Entity entity;
                while (count > 0) {
                    count--;
                    entities.CreateEntity(clusterId, out entity);
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
            public static void NewOnes<C1, C2, C3, C4, C5, C6, C7>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, C5 c5, C6 c6, C7 c7, QueryFunctionWithEntity<WorldType> onCreate = null, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                ref var components5 = ref Components<C5>.Value;
                ref var components6 = ref Components<C6>.Value;
                ref var components7 = ref Components<C7>.Value;
                ref var entities = ref Entities.Value;

                Entity entity;
                while (count > 0) {
                    count--;
                    entities.CreateEntity(clusterId, out entity);
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
            public static void NewOnes<C1, C2, C3, C4, C5, C6, C7, C8>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, C5 c5, C6 c6, C7 c7, C8 c8, QueryFunctionWithEntity<WorldType> onCreate = null, ushort clusterId = DEFAULT_CLUSTER)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent
                where C6 : struct, IComponent
                where C7 : struct, IComponent
                where C8 : struct, IComponent {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                ref var components1 = ref Components<C1>.Value;
                ref var components2 = ref Components<C2>.Value;
                ref var components3 = ref Components<C3>.Value;
                ref var components4 = ref Components<C4>.Value;
                ref var components5 = ref Components<C5>.Value;
                ref var components6 = ref Components<C6>.Value;
                ref var components7 = ref Components<C7>.Value;
                ref var components8 = ref Components<C8>.Value;
                ref var entities = ref Entities.Value;

                Entity entity;
                while (count > 0) {
                    count--;
                    entities.CreateEntity(clusterId, out entity);
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

            #region NEW_BY_RAW_TYPE
            [MethodImpl(AggressiveInlining)]
            public static Entity New(Type componentType, ushort clusterId = DEFAULT_CLUSTER) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                Entities.Value.CreateEntity(clusterId, out var entity);
                GetComponentsPool(componentType).Add(entity);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New(IComponent component, ushort clusterId = DEFAULT_CLUSTER) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertMultiThreadNotActive(EntityTypeName);
                AssertClusterIsRegistered(EntityTypeName, clusterId);
                #endif
                Entities.Value.CreateEntity(clusterId, out var entity);
                GetComponentsPool(component.GetType()).PutRaw(entity, component);
                return entity;
            }
            #endregion
        }
    }

    public partial interface IEntity {
        internal void ChangeId(uint idWithOffset);

        public uint GetId();

        public Type WorldTypeType();

        public IWorld World();

        public EntityGID Gid();

        public bool IsNotDestroyed();
        
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
        private World<WorldType>.Entity _entity;
        
        public BoxedEntity(World<WorldType>.Entity entity) {
            _entity = entity;
        }

        void IEntity.ChangeId(uint idWithOffset) => _entity.id = idWithOffset;
        
        public uint GetId() => _entity.id - Const.ENTITY_ID_OFFSET;

        public Type WorldTypeType() => typeof(WorldType);

        public IWorld World() => Worlds.Get(typeof(WorldType));
        
        public EntityGID Gid() => _entity.Gid();
        
        public bool IsNotDestroyed() => _entity.IsNotDestroyed();
        
        public bool IsDestroyed() => _entity.IsDestroyed();
        
        public bool IsEnabled() => _entity.IsEnabled();
        
        public bool IsDisabled() => _entity.IsDisabled();
        
        public void Enable() => _entity.Enable();
        
        public void Disable() => _entity.Disable();
        
        public void Destroy() => _entity.Destroy();
        
        public void TryDestroy() => _entity.TryDestroy();
        
        public bool TryAsEntityOf<WT>(out World<WT>.Entity entity) where WT : struct, IWorldType {
            if (typeof(WT) == typeof(WorldType)) {
                entity = new World<WT>.Entity {
                    id = _entity.id,
                };
                return true;
            }
                
            entity = default;
            return false;
        }
        
        public World<WT>.Entity AsEntityOf<WT>() where WT : struct, IWorldType {
            #if FFS_ECS_DEBUG
            if (typeof(WT) != typeof(WorldType)) throw new StaticEcsException($"Invalid cast: expected World<{typeof(WorldType).Name}>, got World<{typeof(WT).Name}>.");
            #endif
            var entity = new World<WT>.Entity {
                id = _entity.id,
            };
            return entity;
        }
    } 
}