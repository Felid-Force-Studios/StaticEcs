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
    public abstract partial class Ecs<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public readonly partial struct Entity : IEquatable<Entity>, IEntity {
            internal readonly uint _id;
            internal Entity(uint id) {
                _id = id;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity FromIdx(uint idx) => new(idx);

            [MethodImpl(AggressiveInlining)]
            public short Version() => StandardComponents<EntityVersion>.Value.RefMutInternal(this).Value;
            
            uint IEntity.GetId() => _id;

            [MethodImpl(AggressiveInlining)]
            Type IEntity.WorldTypeType() => typeof(WorldType);

            [MethodImpl(AggressiveInlining)]
            IWorld IEntity.World() => Worlds.Get(typeof(WorldType));

            [MethodImpl(AggressiveInlining)]
            public bool IsActual() => StandardComponents<EntityVersion>.Value.RefMutInternal(this).Value > 0;

            [MethodImpl(AggressiveInlining)]
            public Entity Clone(bool withDisabled = true) => World.CloneEntity(this, withDisabled);

            [MethodImpl(AggressiveInlining)]
            public void CopyTo(Entity dstEntity, bool withDisabled = true) => World.CopyEntityData(this, dstEntity, withDisabled);

            [MethodImpl(AggressiveInlining)]
            public void MoveTo(Entity dstEntity, bool withDisabled = true) {
                World.CopyEntityData(this, dstEntity, withDisabled);
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
            public PackedEntity Pack() => new() { _entity = _id, _version = StandardComponents<EntityVersion>.Value.RefMutInternal(this).Value };

            [MethodImpl(AggressiveInlining)]
            public void Destroy() => World.DestroyEntity(this);

            #region NEW_BY_TYPE_SINGLE

            #if DEBUG || FFS_ECS_ENABLE_DEBUG || !FFS_ECS_LIFECYCLE_ENTITY
            [MethodImpl(AggressiveInlining)]
            public static Entity New() {
                return World.CreateEntityInternal();
            }
            #endif
            
            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1>() where C1 : struct, IComponent {
                var entity = World.CreateEntityInternal();
                entity.Add<C1>();
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                var entity = World.CreateEntityInternal();
                entity.Add<C1, C2>();
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                var entity = World.CreateEntityInternal();
                entity.Add<C1, C2, C3>();
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                var entity = World.CreateEntityInternal();
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
                var entity = World.CreateEntityInternal();
                entity.Add<C1, C2, C3, C4, C5>();
                return entity;
            }
            #endregion
            
            #region NEW_BY_VALUE_SINGLE
            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1>(C1 component) where C1 : struct, IComponent {
                var entity = World.CreateEntityInternal();
                entity.Put(component);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2>(C1 comp1, C2 comp2)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                var entity = World.CreateEntityInternal();
                entity.Put(comp1, comp2);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3>(C1 comp1, C2 comp2, C3 comp3)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                var entity = World.CreateEntityInternal();
                entity.Put(comp1, comp2, comp3);
                return entity;
            }

            [MethodImpl(AggressiveInlining)]
            public static Entity New<C1, C2, C3, C4>(C1 comp1, C2 comp2, C3 comp3, C4 comp4)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                var entity = World.CreateEntityInternal();
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
                var entity = World.CreateEntityInternal();
                entity.Put(comp1, comp2, comp3, comp4, comp5);
                return entity;
            }
            #endregion
            
            #region NEW_BY_RAW_TYPE
            [MethodImpl(AggressiveInlining)]
            public static Entity New(Type componentType) {
                var entity = World.CreateEntityInternal();
                World.GetComponentsPool(componentType).Add(entity);
                return entity;
            }
            
            [MethodImpl(AggressiveInlining)]
            public static Entity New(IComponent component) {
                var entity = World.CreateEntityInternal();
                World.GetComponentsPool(component.GetType()).PutRaw(entity, component);
                return entity;
            }
            #endregion

            #region NEW_BY_TYPE_BATCH
            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1>(uint count, Action<Entity> onCreate = null) where C1 : struct, IComponent {
                Components<C1>.Value.EnsureSize(count);
                World.CreateEntitiesInternal(
                    count,
                    new AddComponentFunction<WorldType, C1>(),
                    onCreate
                );
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2>(uint count, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Components<C1>.Value.EnsureSize(count);
                Components<C2>.Value.EnsureSize(count);
                World.CreateEntitiesInternal(
                    count,
                    new AddComponentFunction<WorldType, C1, C2>(),
                    onCreate
                );
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3>(uint count, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Components<C1>.Value.EnsureSize(count);
                Components<C2>.Value.EnsureSize(count);
                Components<C3>.Value.EnsureSize(count);
                World.CreateEntitiesInternal(
                    count,
                    new AddComponentFunction<WorldType, C1, C2, C3>(),
                    onCreate
                );
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4>(uint count, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Components<C1>.Value.EnsureSize(count);
                Components<C2>.Value.EnsureSize(count);
                Components<C3>.Value.EnsureSize(count);
                Components<C4>.Value.EnsureSize(count);
                World.CreateEntitiesInternal(
                    count,
                    new AddComponentFunction<WorldType, C1, C2, C3, C4>(),
                    onCreate
                );
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4, C5>(uint count, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Components<C1>.Value.EnsureSize(count);
                Components<C2>.Value.EnsureSize(count);
                Components<C3>.Value.EnsureSize(count);
                Components<C4>.Value.EnsureSize(count);
                Components<C5>.Value.EnsureSize(count);
                World.CreateEntitiesInternal(
                    count,
                    new AddComponentFunction<WorldType, C1, C2, C3, C4, C5>(),
                    onCreate
                );
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1>(uint count, C1 c1, Action<Entity> onCreate = null) where C1 : struct, IComponent {
                Components<C1>.Value.EnsureSize(count);
                World.CreateEntitiesInternal(
                    count,
                    new PutComponentFunction<WorldType, C1>(c1),
                    onCreate
                );
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2>(uint count, C1 c1, C2 c2, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Components<C1>.Value.EnsureSize(count);
                Components<C2>.Value.EnsureSize(count);
                World.CreateEntitiesInternal(
                    count,
                    new PutComponentFunction<WorldType, C1, C2>(c1, c2),
                    onCreate
                );
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3>(uint count, C1 c1, C2 c2, C3 c3, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Components<C1>.Value.EnsureSize(count);
                Components<C2>.Value.EnsureSize(count);
                Components<C3>.Value.EnsureSize(count);
                World.CreateEntitiesInternal(
                    count,
                    new PutComponentFunction<WorldType, C1, C2, C3>(c1, c2, c3),
                    onCreate
                );
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Components<C1>.Value.EnsureSize(count);
                Components<C2>.Value.EnsureSize(count);
                Components<C3>.Value.EnsureSize(count);
                Components<C4>.Value.EnsureSize(count);
                World.CreateEntitiesInternal(
                    count,
                    new PutComponentFunction<WorldType, C1, C2, C3, C4>(c1, c2, c3, c4),
                    onCreate
                );
            }

            [MethodImpl(AggressiveInlining)]
            public static void NewOnes<C1, C2, C3, C4, C5>(uint count, C1 c1, C2 c2, C3 c3, C4 c4, C5 c5, Action<Entity> onCreate = null)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Components<C1>.Value.EnsureSize(count);
                Components<C2>.Value.EnsureSize(count);
                Components<C3>.Value.EnsureSize(count);
                Components<C4>.Value.EnsureSize(count);
                Components<C5>.Value.EnsureSize(count);
                World.CreateEntitiesInternal(
                    count,
                    new PutComponentFunction<WorldType, C1, C2, C3, C4, C5>(c1, c2, c3, c4, c5),
                    onCreate
                );
            }
            #endregion

            [MethodImpl(AggressiveInlining)]
            public bool Equals(Entity entity) => _id == entity._id;

            [MethodImpl(AggressiveInlining)]
            public override bool Equals(object obj) => throw new Exception("Entity` Equals object` not allowed!");

            [MethodImpl(AggressiveInlining)]
            public override int GetHashCode() => (int) _id;

            [MethodImpl(AggressiveInlining)]
            public string ToPrettyString(bool withDisabled = true) => World.ToPrettyStringEntity(this, withDisabled);

            public override string ToString() => $"Entity ID: {_id}";
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
        public readonly bool TryUnpack<WorldType>(out Ecs<WorldType>.Entity entity) where WorldType : struct, IWorldType {
            entity = Ecs<WorldType>.Entity.FromIdx(_entity);
            return Ecs<WorldType>.StandardComponents<EntityVersion>.Value.RefMutInternal(entity).Value == _version;
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