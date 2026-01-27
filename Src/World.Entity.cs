#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public abstract partial class World<TWorld> {

        /// <summary>
        /// Lightweight runtime handle representing a single entity in <c>World&lt;TWorld&gt;</c>.
        /// <para>
        /// An Entity is a thin wrapper around a <c>uint</c> slot index (with an offset applied so
        /// that the default/zeroed struct is never a valid entity). It provides the primary API for
        /// working with an entity's components, tags, and lifecycle.
        /// </para>
        /// <para>
        /// <b>Important:</b> Entity is a value type that does NOT embed a generation counter.
        /// If the entity is destroyed and its slot is reused, a stale Entity handle will silently
        /// refer to the new occupant. For persistent/serializable references that survive
        /// destroy/recreate cycles, use <see cref="GID"/> (<see cref="EntityGID"/>) instead and
        /// validate with <see cref="EntityGID.TryUnpack{TWorld}(out Entity)"/> before use.
        /// </para>
        /// <para>
        /// Entities are created via <see cref="World{TWorld}.NewEntity(ushort)"/> and destroyed via
        /// <see cref="Destroy"/>. All component/tag operations on a destroyed entity will throw
        /// in debug builds.
        /// </para>
        /// </summary>
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        public struct Entity : IEquatable<Entity> {
            internal uint IdWithOffset;

            #region BASE
            [MethodImpl(AggressiveInlining)]
            internal Entity(uint id) => IdWithOffset = id + Const.ENTITY_ID_OFFSET;

            /// <summary>
            /// The raw internal slot index (without offset). This is the same value stored in
            /// <see cref="EntityGID.Id"/>. Useful for low-level operations and debugging.
            /// </summary>
            public readonly uint ID {
                [MethodImpl(AggressiveInlining)] get => IdWithOffset - Const.ENTITY_ID_OFFSET;
            }

            /// <summary>
            /// Returns the full-size global identifier (<see cref="EntityGID"/>, 8 bytes) for this entity.
            /// The GID includes the entity's <see cref="Version"/>, <see cref="ClusterId"/>, and slot ID,
            /// making it safe for serialization, network sync, and detecting stale references.
            /// </summary>
            public readonly EntityGID GID {
                [MethodImpl(AggressiveInlining)] get => Data.Instance.EntityGID(this);
            }

            /// <summary>
            /// Returns the compact global identifier (<see cref="EntityGIDCompact"/>, 4 bytes) for this entity.
            /// Same semantics as <see cref="GID"/> but uses 2-bit Chunk and ClusterId fields, limiting
            /// to 4 chunks and 4 clusters. See <see cref="EntityGIDCompact"/> for details.
            /// </summary>
            public readonly EntityGIDCompact GIDCompact {
                [MethodImpl(AggressiveInlining)] get => Data.Instance.EntityGIDCompact(this);
            }

            /// <summary>
            /// Whether this entity has been destroyed (its slot is free or reused).
            /// A destroyed entity should not be used for any component/tag operations.
            /// </summary>
            public readonly bool IsDestroyed {
                [MethodImpl(AggressiveInlining)] get => !Data.Instance.EntityIsNotDestroyed(this);
            }

            /// <summary>
            /// Whether this entity is alive (not destroyed). Inverse of <see cref="IsDestroyed"/>.
            /// Use this as a guard before operating on an entity of uncertain validity.
            /// </summary>
            public readonly bool IsNotDestroyed {
                [MethodImpl(AggressiveInlining)] get => Data.Instance.EntityIsNotDestroyed(this);
            }

            /// <summary>
            /// Whether this entity is disabled. Disabled entities are excluded from standard queries
            /// (those using <c>EntityStatusType.Enabled</c>) but retain all their components and tags.
            /// Useful for "freezing" an entity without destroying it.
            /// </summary>
            public readonly bool IsDisabled {
                [MethodImpl(AggressiveInlining)] get => !Data.Instance.IsEnabledEntity(this);
            }

            /// <summary>
            /// Whether this entity is enabled (participating in standard queries).
            /// Inverse of <see cref="IsDisabled"/>.
            /// </summary>
            public readonly bool IsEnabled {
                [MethodImpl(AggressiveInlining)] get => Data.Instance.IsEnabledEntity(this);
            }

            /// <summary>
            /// Whether this entity resides in a self-owned chunk (owned by this world instance).
            /// Returns <c>false</c> for entities in externally-owned chunks (e.g. received from network).
            /// </summary>
            public readonly bool IsSelfOwned {
                [MethodImpl(AggressiveInlining)] get => Data.Instance.EntityIsSelfOwned(this);
            }

            /// <summary>
            /// The generation counter for this entity's slot. Incremented each time the slot is
            /// recycled after destruction. Used by <see cref="EntityGID"/> to detect stale references.
            /// </summary>
            public readonly ushort Version {
                [MethodImpl(AggressiveInlining)] get => Data.Instance.EntityVersion(this);
            }

            /// <summary>
            /// The cluster ID this entity belongs to. Clusters are logical groupings of chunks
            /// for spatial partitioning, streaming, and ownership management.
            /// </summary>
            public readonly ushort ClusterId {
                [MethodImpl(AggressiveInlining)] get => Data.Instance.EntityClusterId(this);
            }

            /// <summary>
            /// The entity type (0–255) assigned at creation. Entities with the same type within
            /// a cluster are co-located in the same segments, reducing fragmentation and improving
            /// iteration performance.
            /// </summary>
            public readonly byte EntityType {
                [MethodImpl(AggressiveInlining)] get => Data.Instance.EntityType(this);
            }

            /// <summary>
            /// Checks whether this entity's type matches the specified <typeparamref name="T"/>.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool Is<T>() where T : struct, IEntityType {
                return EntityType == EntityTypeInfo<T>.Id;
            }

            /// <summary>
            /// Checks whether this entity's type matches any of the specified types.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool IsAny<T0, T1>()
                where T0 : struct, IEntityType
                where T1 : struct, IEntityType {
                return EntityType == EntityTypeInfo<T0>.Id || EntityType == EntityTypeInfo<T1>.Id;
            }

            /// <summary>
            /// Checks whether this entity's type matches any of the specified types.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool IsAny<T0, T1, T2>()
                where T0 : struct, IEntityType
                where T1 : struct, IEntityType
                where T2 : struct, IEntityType {
                return EntityType == EntityTypeInfo<T0>.Id || EntityType == EntityTypeInfo<T1>.Id || EntityType == EntityTypeInfo<T2>.Id;
            }

            /// <summary>
            /// Checks whether this entity's type does NOT match the specified <typeparamref name="T"/>.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool IsNot<T>() where T : struct, IEntityType {
                return EntityType != EntityTypeInfo<T>.Id;
            }

            /// <summary>
            /// Checks whether this entity's type does NOT match any of the specified types.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool IsNot<T0, T1>()
                where T0 : struct, IEntityType
                where T1 : struct, IEntityType {
                return EntityType != EntityTypeInfo<T0>.Id && EntityType != EntityTypeInfo<T1>.Id;
            }

            /// <summary>
            /// Checks whether this entity's type does NOT match any of the specified types.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool IsNot<T0, T1, T2>()
                where T0 : struct, IEntityType
                where T1 : struct, IEntityType
                where T2 : struct, IEntityType {
                return EntityType != EntityTypeInfo<T0>.Id && EntityType != EntityTypeInfo<T1>.Id && EntityType != EntityTypeInfo<T2>.Id;
            }

            /// <summary>
            /// The chunk index this entity resides in. Each chunk holds 4096 entity slots.
            /// Derived from the entity's internal ID.
            /// </summary>
            public readonly uint ChunkID {
                [MethodImpl(AggressiveInlining)]
                get {
                    #if FFS_ECS_DEBUG
                    AssertWorldIsInitialized(EntityTypeName);
                    AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, this);
                    #endif
                    return (IdWithOffset - Const.ENTITY_ID_OFFSET) >> Const.ENTITIES_IN_CHUNK_SHIFT;
                }
            }

            /// <summary>
            /// Returns a human-readable string describing this entity, including its ID,
            /// version, cluster, and all attached components and tags. Intended for debug output
            /// and inspector tools.
            /// </summary>
            public readonly string PrettyString {
                [MethodImpl(AggressiveInlining)] get => Data.Instance.EntityToPrettyString(this);
            }

            /// <summary>
            /// Manually increments this entity's generation version counter. After calling this,
            /// any previously obtained <see cref="EntityGID"/> for this entity will report
            /// <c>Status() == GIDStatus.NotActual</c>, effectively invalidating all external references.
            /// <para>
            /// Use this to force-invalidate cached GIDs without destroying the entity — for example,
            /// after a major state reset where you want holders of old GIDs to detect the change.
            /// </para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly void UpVersion() => Data.Instance.UpEntityVersion(this);

            /// <summary>
            /// Creates a full clone of this entity: allocates a new entity in the same cluster and
            /// copies all components and tags from this entity to the clone. Component CopyTo hooks
            /// are triggered for each copied component.
            /// </summary>
            /// <returns>The newly created clone entity with all components and tags duplicated.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Clone() {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, this);
                #endif

                Data.Instance.CreateEntity(EntityType, ClusterId, out var dstEntity);
                CopyTo(dstEntity);

                return dstEntity;
            }

            /// <summary>
            /// Creates a full clone of this entity in a specified cluster. Behaves like
            /// <see cref="Clone()"/> but places the new entity in <paramref name="clusterId"/>
            /// instead of the source entity's cluster.
            /// </summary>
            /// <param name="clusterId">Cluster to create the clone in.</param>
            /// <returns>The newly created clone entity.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Clone(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, this);
                #endif

                Data.Instance.CreateEntity(EntityType, clusterId, out var dstEntity);
                CopyTo(dstEntity);

                return dstEntity;
            }

            /// <summary>
            /// Copies all components and tags from this entity to <paramref name="dstEntity"/>.
            /// The destination entity receives copies of all data; this entity is not modified.
            /// If the destination already has some of the same components, their values are overwritten.
            /// Component CopyTo hooks are triggered for each component.
            /// </summary>
            /// <param name="dstEntity">Target entity to copy all data to.</param>
            [MethodImpl(AggressiveInlining)]
            public readonly void CopyTo(Entity dstEntity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, this);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, dstEntity);
                #endif

                Data.Instance.CopyEntity(this, dstEntity);
            }

            /// <summary>
            /// Moves this entity to a different cluster: creates a new entity in
            /// <paramref name="clusterId"/>, copies all components and tags to it, then destroys
            /// the original. Returns the new entity handle.
            /// <para>
            /// Since entities are slot-based and slots are chunk-specific, moving to a different
            /// cluster requires creating a new entity. All previously held Entity handles to the
            /// original become invalid after this call.
            /// </para>
            /// </summary>
            /// <param name="clusterId">Target cluster for the moved entity.</param>
            /// <returns>The new entity in the target cluster (with all data transferred).</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity MoveTo(ushort clusterId) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, this);
                #endif

                Data.Instance.CreateEntity(EntityType, clusterId, out var dstEntity);
                CopyTo(dstEntity);
                Destroy();
                return dstEntity;
            }

            /// <summary>
            /// Moves all components and tags from this entity to <paramref name="dstEntity"/>,
            /// then destroys this entity. The destination receives all data; the source is destroyed.
            /// This is equivalent to <see cref="CopyTo"/> followed by <see cref="Destroy"/>.
            /// </summary>
            /// <param name="dstEntity">Target entity to receive all data.</param>
            [MethodImpl(AggressiveInlining)]
            public readonly void MoveTo(Entity dstEntity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(EntityTypeName);
                AssertEntityIsNotDestroyedAndLoaded(EntityTypeName, this);
                #endif

                CopyTo(dstEntity);
                Destroy();
            }

            /// <summary> Equality operator. Compares entities by their internal slot index. </summary>
            [MethodImpl(AggressiveInlining)]
            public static bool operator ==(Entity left, Entity right) {
                return left.Equals(right);
            }

            /// <summary> Inequality operator. </summary>
            [MethodImpl(AggressiveInlining)]
            public static bool operator !=(Entity left, Entity right) {
                return !left.Equals(right);
            }

            /// <summary>
            /// Implicit conversion from Entity to <see cref="EntityGID"/>.
            /// Convenience for storing entities as persistent identifiers.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static implicit operator EntityGID(Entity e) => e.GID;

            /// <summary>
            /// Explicit conversion from Entity to <see cref="EntityGIDCompact"/>.
            /// This is a narrowing conversion — subject to compact format limitations (max 4 chunks, 4 clusters).
            /// In debug builds, throws if limits are exceeded. In release builds, values are silently truncated.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public static explicit operator EntityGIDCompact(Entity e) => e.GIDCompact;

            /// <summary>
            /// Disables this entity. Disabled entities are excluded from standard queries
            /// (<c>EntityStatusType.Enabled</c>) but retain all their components and tags.
            /// They can still be found by queries using <c>EntityStatusType.Disabled</c> or
            /// <c>EntityStatusType.Any</c>. Call <see cref="Enable"/> to re-activate.
            /// <para>
            /// Useful for "pausing" entities (e.g. pooled objects waiting for reuse, entities
            /// outside the player's view in a streaming scenario).
            /// </para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly void Disable() => Data.Instance.DisableEntity(this);

            /// <summary>
            /// Re-enables a previously disabled entity. The entity will again participate
            /// in standard queries.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly void Enable() => Data.Instance.EnableEntity(this);

            /// <summary>
            /// Destroys this entity: removes all components (triggering OnDelete hooks), removes
            /// all tags, frees the entity slot, and increments the slot's generation version.
            /// After this call, the Entity handle is invalid. Any <see cref="EntityGID"/> obtained
            /// before destruction will report <c>Status() == GIDStatus.NotActual</c>.
            /// <para>
            /// Idempotent: if the entity is already destroyed, returns <c>false</c> without side effects.
            /// </para>
            /// </summary>
            /// <returns><c>true</c> if the entity was destroyed; <c>false</c> if already destroyed.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly bool Destroy() => Data.Instance.DestroyEntity(this);

            /// <summary>
            /// Unloads this entity: marks it as not loaded, making it invisible to queries and
            /// inaccessible via entity handles, but preserves its ID in memory. The entity's
            /// <see cref="EntityGID"/> will report <c>Status() == GIDStatus.NotLoaded</c>.
            /// <para>
            /// Used in streaming scenarios where entity ID should be preserved but temporarily
            /// removed from active simulation.
            /// </para>
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly void Unload() => Data.Instance.UnloadEntity(this);

            /// <summary>
            /// Compares two entities by their internal slot index. Two Entity values are equal
            /// if they refer to the same slot, regardless of generation — this is a fast pointer-like
            /// comparison without version checking.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool Equals(Entity entity) => IdWithOffset == entity.IdWithOffset;

            /// <inheritdoc />
            /// <remarks>Intentionally throws to prevent accidental boxing comparisons.</remarks>
            [MethodImpl(AggressiveInlining)]
            public readonly override bool Equals(object obj) => throw new StaticEcsException("Entity.Equals(object) not allowed!");

            /// <inheritdoc />
            [MethodImpl(AggressiveInlining)]
            public readonly override int GetHashCode() => (int)IdWithOffset;

            /// <inheritdoc />
            public readonly override string ToString() => $"Entity ID: {IdWithOffset - Const.ENTITY_ID_OFFSET}";
            #endregion

            #region COMPONENTS
            /// <summary>
            /// Returns the number of components currently attached to this entity
            /// (both enabled and disabled).
            /// </summary>
            /// <returns>Component count.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly int ComponentsCount() => Data.Instance.ComponentsCount(this);

            /// <summary>
            /// Collects all components attached to this entity into the provided list as boxed
            /// <see cref="IComponent"/> values.
            /// Intended for debug/inspector tools, not for hot-path code.
            /// </summary>
            /// <param name="result">List to append component values to.</param>
            [MethodImpl(AggressiveInlining)]
            public readonly void GetAllComponents(List<IComponent> result) => Data.Instance.GetAllComponents(this, result);

            #region REF
            /// <summary>
            /// Returns a mutable reference to the component of type <typeparamref name="T"/> on this entity.
            /// The entity must have this component — in debug builds, throws if it does not.
            /// <para>
            /// This is the fastest way to read and modify component data — zero overhead, no tracking.
            /// Does NOT mark the component as Changed. Use <see cref="Mut{T}"/> for tracked mutable access.
            /// The reference is valid until the component is removed or the entity is destroyed.
            /// </para>
            /// </summary>
            /// <typeparam name="T">Component type.</typeparam>
            /// <returns>A mutable reference to the component data.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly ref T Ref<T>()
                where T : struct, IComponent {
                return ref Components<T>.Instance.Ref(this);
            }

            /// <summary>
            /// Returns a read-only reference to component <typeparamref name="T"/> on this entity.
            /// Does NOT mark the component as changed. Use when you need to read component data
            /// without triggering change tracking.
            /// </summary>
            /// <typeparam name="T">Component type.</typeparam>
            /// <returns>A read-only reference to the component data.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly ref readonly T Read<T>()
                where T : struct, IComponent {
                return ref Components<T>.Instance.Read(this);
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            /// <summary>
            /// Returns a mutable reference to component <typeparamref name="T"/> on this entity
            /// and marks it as Changed if change tracking is enabled for this component type.
            /// Use when you need tracked mutable access for <c>AllChanged&lt;T&gt;</c> queries.
            /// </summary>
            /// <typeparam name="T">Component type.</typeparam>
            /// <returns>A mutable reference to the component data.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly ref T Mut<T>()
                where T : struct, IComponent {
                return ref Components<T>.Instance.Mut(this);
            }
            #endif

            #endregion

            #region HAS
            /// <summary>
            /// Checks whether this entity has a component of type <typeparamref name="T"/>
            /// (in any state — enabled or disabled).
            /// </summary>
            /// <typeparam name="T">Component type to check.</typeparam>
            /// <returns><c>true</c> if the entity has this component.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly bool Has<T>() where T : struct, IComponentOrTag {
                return Components<T>.Instance.Has(this);
            }

            /// <summary>
            /// Checks whether this entity has all of the specified component types (any state).
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool Has<T1, T2>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                return Components<T1>.Instance.Has(this) && Components<T2>.Instance.Has(this);
            }

            /// <inheritdoc cref="Has{T1, T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool Has<T1, T2, T3>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                return Components<T1>.Instance.Has(this) && Components<T2>.Instance.Has(this) && Components<T3>.Instance.Has(this);
            }
            #endregion

            #region TRACKING
            /// <summary>
            /// Checks whether component <typeparamref name="T"/> was added to this entity since the system's last tick (or since `fromTick` if specified).
            /// Requires <c>TrackAdded</c> to be enabled for the component type.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAdded<T>() where T : struct, IComponentOrTag {
                return Components<T>.Instance.HasAdded(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAdded<T>(ulong fromTick) where T : struct, IComponentOrTag {
                return Components<T>.Instance.HasAdded(this, fromTick);
            }

            /// <inheritdoc cref="HasAdded{T}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAdded<T1, T2>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasAdded(this) && Components<T2>.Instance.HasAdded(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAdded<T1, T2>(ulong fromTick)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasAdded(this, fromTick) && Components<T2>.Instance.HasAdded(this, fromTick);
            }

            /// <inheritdoc cref="HasAdded{T}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAdded<T1, T2, T3>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasAdded(this) && Components<T2>.Instance.HasAdded(this) && Components<T3>.Instance.HasAdded(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAdded<T1, T2, T3>(ulong fromTick)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasAdded(this, fromTick) && Components<T2>.Instance.HasAdded(this, fromTick) && Components<T3>.Instance.HasAdded(this, fromTick);
            }

            /// <summary>
            /// Checks whether component <typeparamref name="T"/> was deleted from this entity since the system's last tick (or since `fromTick` if specified).
            /// Requires <c>TrackDeleted</c> to be enabled for the component type.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasDeleted<T>() where T : struct, IComponentOrTag {
                return Components<T>.Instance.HasDeleted(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasDeleted<T>(ulong fromTick) where T : struct, IComponentOrTag {
                return Components<T>.Instance.HasDeleted(this, fromTick);
            }

            /// <inheritdoc cref="HasDeleted{T}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasDeleted<T1, T2>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasDeleted(this) && Components<T2>.Instance.HasDeleted(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasDeleted<T1, T2>(ulong fromTick)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasDeleted(this, fromTick) && Components<T2>.Instance.HasDeleted(this, fromTick);
            }

            /// <inheritdoc cref="HasDeleted{T}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasDeleted<T1, T2, T3>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasDeleted(this) && Components<T2>.Instance.HasDeleted(this) && Components<T3>.Instance.HasDeleted(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasDeleted<T1, T2, T3>(ulong fromTick)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasDeleted(this, fromTick) && Components<T2>.Instance.HasDeleted(this, fromTick) && Components<T3>.Instance.HasDeleted(this, fromTick);
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            /// <summary>
            /// Checks whether component <typeparamref name="T"/> was changed on this entity since the system's last tick (or since `fromTick` if specified).
            /// Requires <c>TrackChanged</c> to be enabled for the component type.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasChanged<T>() where T : struct, IComponent {
                return Components<T>.Instance.HasChanged(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasChanged<T>(ulong fromTick) where T : struct, IComponent {
                return Components<T>.Instance.HasChanged(this, fromTick);
            }

            /// <inheritdoc cref="HasChanged{T}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasChanged<T1, T2>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent {
                return Components<T1>.Instance.HasChanged(this) && Components<T2>.Instance.HasChanged(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasChanged<T1, T2>(ulong fromTick)
                where T1 : struct, IComponent
                where T2 : struct, IComponent {
                return Components<T1>.Instance.HasChanged(this, fromTick) && Components<T2>.Instance.HasChanged(this, fromTick);
            }

            /// <inheritdoc cref="HasChanged{T}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasChanged<T1, T2, T3>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent {
                return Components<T1>.Instance.HasChanged(this) && Components<T2>.Instance.HasChanged(this) && Components<T3>.Instance.HasChanged(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasChanged<T1, T2, T3>(ulong fromTick)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent {
                return Components<T1>.Instance.HasChanged(this, fromTick) && Components<T2>.Instance.HasChanged(this, fromTick) && Components<T3>.Instance.HasChanged(this, fromTick);
            }

            /// <summary>
            /// Checks whether at least one of the specified component types was changed on this entity since the system's last tick (or since `fromTick` if specified).
            /// Requires <c>TrackChanged</c> to be enabled for the component types.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAnyChanged<T1, T2>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent {
                return Components<T1>.Instance.HasChanged(this) || Components<T2>.Instance.HasChanged(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAnyChanged<T1, T2>(ulong fromTick)
                where T1 : struct, IComponent
                where T2 : struct, IComponent {
                return Components<T1>.Instance.HasChanged(this, fromTick) || Components<T2>.Instance.HasChanged(this, fromTick);
            }

            /// <inheritdoc cref="HasAnyChanged{T1,T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAnyChanged<T1, T2, T3>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent {
                return Components<T1>.Instance.HasChanged(this) || Components<T2>.Instance.HasChanged(this) || Components<T3>.Instance.HasChanged(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAnyChanged<T1, T2, T3>(ulong fromTick)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent {
                return Components<T1>.Instance.HasChanged(this, fromTick) || Components<T2>.Instance.HasChanged(this, fromTick) || Components<T3>.Instance.HasChanged(this, fromTick);
            }
            #endif

            /// <summary>
            /// Checks whether at least one of the specified component types was added to this entity since the system's last tick (or since `fromTick` if specified).
            /// Requires <c>TrackAdded</c> to be enabled for the component types.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAnyAdded<T1, T2>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasAdded(this) || Components<T2>.Instance.HasAdded(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAnyAdded<T1, T2>(ulong fromTick)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasAdded(this, fromTick) || Components<T2>.Instance.HasAdded(this, fromTick);
            }

            /// <inheritdoc cref="HasAnyAdded{T1,T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAnyAdded<T1, T2, T3>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasAdded(this) || Components<T2>.Instance.HasAdded(this) || Components<T3>.Instance.HasAdded(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAnyAdded<T1, T2, T3>(ulong fromTick)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasAdded(this, fromTick) || Components<T2>.Instance.HasAdded(this, fromTick) || Components<T3>.Instance.HasAdded(this, fromTick);
            }

            /// <summary>
            /// Checks whether at least one of the specified component types was deleted from this entity since the system's last tick (or since `fromTick` if specified).
            /// Requires <c>TrackDeleted</c> to be enabled for the component types.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAnyDeleted<T1, T2>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasDeleted(this) || Components<T2>.Instance.HasDeleted(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAnyDeleted<T1, T2>(ulong fromTick)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasDeleted(this, fromTick) || Components<T2>.Instance.HasDeleted(this, fromTick);
            }

            /// <inheritdoc cref="HasAnyDeleted{T1,T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAnyDeleted<T1, T2, T3>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasDeleted(this) || Components<T2>.Instance.HasDeleted(this) || Components<T3>.Instance.HasDeleted(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAnyDeleted<T1, T2, T3>(ulong fromTick)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                return Components<T1>.Instance.HasDeleted(this, fromTick) || Components<T2>.Instance.HasDeleted(this, fromTick) || Components<T3>.Instance.HasDeleted(this, fromTick);
            }
            #endregion

            #region HAS
            /// <summary>
            /// Checks whether this entity has at least one of the specified component types (any state).
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAny<T1, T2>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                return Components<T1>.Instance.Has(this) || Components<T2>.Instance.Has(this);
            }

            /// <inheritdoc cref="HasAny{T1, T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAny<T1, T2, T3>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                return Components<T1>.Instance.Has(this) || Components<T2>.Instance.Has(this) || Components<T3>.Instance.Has(this);
            }

            /// <summary>
            /// Checks whether this entity has a component in the <b>disabled</b> state.
            /// Disabled components exist on the entity but are excluded from standard queries.
            /// </summary>
            /// <typeparam name="T1">Component type to check.</typeparam>
            /// <returns><c>true</c> if the entity has this component and it is disabled.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasDisabled<T1>()
                where T1 : struct, IComponent {
                return Components<T1>.Instance.HasDisabled(this);
            }

            /// <summary>
            /// Checks whether this entity has all specified components in the disabled state.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasDisabled<T1, T2>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent {
                return Components<T1>.Instance.HasDisabled(this) && Components<T2>.Instance.HasDisabled(this);
            }

            /// <inheritdoc cref="HasDisabled{T1, T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasDisabled<T1, T2, T3>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent {
                return Components<T1>.Instance.HasDisabled(this) && Components<T2>.Instance.HasDisabled(this) && Components<T3>.Instance.HasDisabled(this);
            }

            /// <summary>
            /// Checks whether this entity has at least one of the specified components in the disabled state.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasDisabledAny<T1, T2>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent {
                return Components<T1>.Instance.HasDisabled(this) || Components<T2>.Instance.HasDisabled(this);
            }

            /// <inheritdoc cref="HasDisabledAny{T1, T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasDisabledAny<T1, T2, T3>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent {
                return Components<T1>.Instance.HasDisabled(this) || Components<T2>.Instance.HasDisabled(this) || Components<T3>.Instance.HasDisabled(this);
            }

            /// <summary>
            /// Checks whether this entity has a component in the <b>enabled</b> state.
            /// Enabled components participate in standard queries.
            /// </summary>
            /// <typeparam name="T1">Component type to check.</typeparam>
            /// <returns><c>true</c> if the entity has this component and it is enabled.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasEnabled<T1>()
                where T1 : struct, IComponent {
                return Components<T1>.Instance.HasEnabled(this);
            }

            /// <summary>
            /// Checks whether this entity has all specified components in the enabled state.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasEnabled<T1, T2>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent {
                return Components<T1>.Instance.HasEnabled(this) && Components<T2>.Instance.HasEnabled(this);
            }

            /// <inheritdoc cref="HasEnabled{T1, T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasEnabled<T1, T2, T3>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent {
                return Components<T1>.Instance.HasEnabled(this) && Components<T2>.Instance.HasEnabled(this) && Components<T3>.Instance.HasEnabled(this);
            }

            /// <summary>
            /// Checks whether this entity has at least one of the specified components in the enabled state.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasEnabledAny<T1, T2>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent {
                return Components<T1>.Instance.HasEnabled(this) || Components<T2>.Instance.HasEnabled(this);
            }

            /// <inheritdoc cref="HasEnabledAny{T1, T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasEnabledAny<T1, T2, T3>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent {
                return Components<T1>.Instance.HasEnabled(this) || Components<T2>.Instance.HasEnabled(this) || Components<T3>.Instance.HasEnabled(this);
            }
            #endregion

            #region DISABLE
            /// <summary>
            /// Disables a component on this entity. The component data is preserved but the entity
            /// will no longer appear in standard queries filtering for this component type.
            /// Use <see cref="Enable{T}"/> to re-enable.
            /// <para>
            /// Useful for temporarily "hiding" a component without the cost of removing and re-adding
            /// (e.g. disabling a physics component while an entity is in a cutscene).
            /// </para>
            /// </summary>
            /// <typeparam name="T">Component type to disable.</typeparam>
            /// <returns>A <see cref="ToggleResult"/> indicating what happened.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly ToggleResult Disable<T>()
                where T : struct, IComponent {
                return Components<T>.Instance.Disable(this);
            }

            /// <summary>
            /// Disables multiple component types at once on this entity.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly void Disable<T1, T2>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent {
                Components<T1>.Instance.Disable(this);
                Components<T2>.Instance.Disable(this);
            }

            /// <inheritdoc cref="Disable{T1, T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly void Disable<T1, T2, T3>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent {
                Components<T1>.Instance.Disable(this);
                Components<T2>.Instance.Disable(this);
                Components<T3>.Instance.Disable(this);
            }
            #endregion

            #region ENABLE
            /// <summary>
            /// Re-enables a previously disabled component on this entity. After enabling, the entity
            /// will appear in standard queries filtering for this component type.
            /// </summary>
            /// <typeparam name="T">Component type to enable.</typeparam>
            /// <returns>A <see cref="ToggleResult"/> indicating what happened.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly ToggleResult Enable<T>()
                where T : struct, IComponent {
                return Components<T>.Instance.Enable(this);
            }

            /// <summary>
            /// Re-enables multiple component types at once on this entity.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly void Enable<T1, T2>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent {
                Components<T1>.Instance.Enable(this);
                Components<T2>.Instance.Enable(this);
            }

            /// <inheritdoc cref="Enable{T1, T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly void Enable<T1, T2, T3>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent {
                Components<T1>.Instance.Enable(this);
                Components<T2>.Instance.Enable(this);
                Components<T3>.Instance.Enable(this);
            }
            #endregion

            #region ADD
            /// <summary>
            /// Adds a component to this entity and returns a mutable reference to it,
            /// reporting whether it was newly added. If the entity already has this component, <paramref name="isNew"/>
            /// is <c>false</c> and the existing value is returned (not overwritten).
            /// Triggers the OnAdd hook if newly added.
            /// </summary>
            /// <typeparam name="T">Component type to add.</typeparam>
            /// <param name="isNew"><c>true</c> if the component was newly added; <c>false</c> if it already existed.</param>
            /// <returns>A mutable reference to the component data on the entity.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly ref T Add<T>(out bool isNew)
                where T : struct, IComponent {
                return ref Components<T>.Instance.Add(this, out isNew);
            }

            /// <summary>
            /// Adds a component to this entity and returns a mutable reference to it.
            /// If the entity already has this component, the existing value is returned without modification.
            /// Triggers the OnAdd hook if newly added.
            /// <para>
            /// This is the most common way to ensure a component exists and get a reference to modify it:
            /// <c>ref var pos = ref entity.Add&lt;Position&gt;(); pos.X = 100;</c>
            /// </para>
            /// </summary>
            /// <typeparam name="T">Component type to add.</typeparam>
            /// <returns>A mutable reference to the component data.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly ref T Add<T>()
                where T : struct, IComponent {
                return ref Components<T>.Instance.Add(this);
            }

            /// <summary>
            /// Adds multiple components to this entity at once (2–5 types).
            /// More concise than calling <see cref="Add{T}()"/> multiple times.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Add<T1, T2>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent {
                Components<T1>.Instance.Add(this);
                Components<T2>.Instance.Add(this);
                return this;
            }

            /// <inheritdoc cref="Add{T1, T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Add<T1, T2, T3>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent {
                Components<T1>.Instance.Add(this);
                Components<T2>.Instance.Add(this);
                Components<T3>.Instance.Add(this);
                return this;
            }

            /// <inheritdoc cref="Add{T1, T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Add<T1, T2, T3, T4>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent {
                Components<T1>.Instance.Add(this);
                Components<T2>.Instance.Add(this);
                Components<T3>.Instance.Add(this);
                Components<T4>.Instance.Add(this);
                return this;
            }

            /// <inheritdoc cref="Add{T1, T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Add<T1, T2, T3, T4, T5>()
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                Components<T1>.Instance.Add(this);
                Components<T2>.Instance.Add(this);
                Components<T3>.Instance.Add(this);
                Components<T4>.Instance.Add(this);
                Components<T5>.Instance.Add(this);
                return this;
            }

            /// <summary>
            /// Sets a component to the given value on this entity, always overwriting.
            /// If the entity already has this component, the existing value is overwritten with <paramref name="c1"/>:
            /// OnDelete is called on the old value first, then the value is replaced, then OnAdd is called on the new value.
            /// If the component is new, only OnAdd is triggered.
            /// </summary>
            /// <typeparam name="T1">Component type.</typeparam>
            /// <param name="c1">The value to store.</param>
            /// <returns>This entity for chaining.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Set<T1>(T1 c1)
                where T1 : struct, IComponent {
                Components<T1>.Instance.Set(this, c1);
                return this;
            }

            /// <summary>
            /// Sets multiple components with values at once (2–8 types).
            /// Each component is set to the corresponding provided value.
            /// If a component already exists on the entity, OnDelete is called on the old value before overwriting,
            /// then OnAdd is called on the new value. For newly added components, only OnAdd is triggered.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Set<T1, T2>(T1 c1, T2 c2)
                where T1 : struct, IComponent
                where T2 : struct, IComponent {
                Components<T1>.Instance.Set(this, c1);
                Components<T2>.Instance.Set(this, c2);
                return this;
            }

            /// <inheritdoc cref="Set{T1, T2}(T1, T2)"/>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Set<T1, T2, T3>(T1 c1, T2 c2, T3 c3)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent {
                Components<T1>.Instance.Set(this, c1);
                Components<T2>.Instance.Set(this, c2);
                Components<T3>.Instance.Set(this, c3);
                return this;
            }

            /// <inheritdoc cref="Set{T1, T2}(T1, T2)"/>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Set<T1, T2, T3, T4>(T1 c1, T2 c2, T3 c3, T4 c4)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent {
                Components<T1>.Instance.Set(this, c1);
                Components<T2>.Instance.Set(this, c2);
                Components<T3>.Instance.Set(this, c3);
                Components<T4>.Instance.Set(this, c4);
                return this;
            }

            /// <inheritdoc cref="Set{T1, T2}(T1, T2)"/>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Set<T1, T2, T3, T4, T5>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent {
                Components<T1>.Instance.Set(this, c1);
                Components<T2>.Instance.Set(this, c2);
                Components<T3>.Instance.Set(this, c3);
                Components<T4>.Instance.Set(this, c4);
                Components<T5>.Instance.Set(this, c5);
                return this;
            }

            /// <inheritdoc cref="Set{T1, T2}(T1, T2)"/>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Set<T1, T2, T3, T4, T5, T6>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where T6 : struct, IComponent {
                Components<T1>.Instance.Set(this, c1);
                Components<T2>.Instance.Set(this, c2);
                Components<T3>.Instance.Set(this, c3);
                Components<T4>.Instance.Set(this, c4);
                Components<T5>.Instance.Set(this, c5);
                Components<T6>.Instance.Set(this, c6);
                return this;
            }

            /// <inheritdoc cref="Set{T1, T2}(T1, T2)"/>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Set<T1, T2, T3, T4, T5, T6, T7>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where T6 : struct, IComponent
                where T7 : struct, IComponent {
                Components<T1>.Instance.Set(this, c1);
                Components<T2>.Instance.Set(this, c2);
                Components<T3>.Instance.Set(this, c3);
                Components<T4>.Instance.Set(this, c4);
                Components<T5>.Instance.Set(this, c5);
                Components<T6>.Instance.Set(this, c6);
                Components<T7>.Instance.Set(this, c7);
                return this;
            }

            /// <inheritdoc cref="Set{T1, T2}(T1, T2)"/>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Set<T1, T2, T3, T4, T5, T6, T7, T8>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7, T8 c8)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where T6 : struct, IComponent
                where T7 : struct, IComponent
                where T8 : struct, IComponent {
                Components<T1>.Instance.Set(this, c1);
                Components<T2>.Instance.Set(this, c2);
                Components<T3>.Instance.Set(this, c3);
                Components<T4>.Instance.Set(this, c4);
                Components<T5>.Instance.Set(this, c5);
                Components<T6>.Instance.Set(this, c6);
                Components<T7>.Instance.Set(this, c7);
                Components<T8>.Instance.Set(this, c8);
                return this;
            }

            /// <inheritdoc cref="Set{T1, T2}(T1, T2)"/>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Set<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7, T8 c8, T9 c9)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where T6 : struct, IComponent
                where T7 : struct, IComponent
                where T8 : struct, IComponent
                where T9 : struct, IComponent {
                Components<T1>.Instance.Set(this, c1);
                Components<T2>.Instance.Set(this, c2);
                Components<T3>.Instance.Set(this, c3);
                Components<T4>.Instance.Set(this, c4);
                Components<T5>.Instance.Set(this, c5);
                Components<T6>.Instance.Set(this, c6);
                Components<T7>.Instance.Set(this, c7);
                Components<T8>.Instance.Set(this, c8);
                Components<T9>.Instance.Set(this, c9);
                return this;
            }

            /// <inheritdoc cref="Set{T1, T2}(T1, T2)"/>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7, T8 c8, T9 c9, T10 c10)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where T6 : struct, IComponent
                where T7 : struct, IComponent
                where T8 : struct, IComponent
                where T9 : struct, IComponent
                where T10 : struct, IComponent {
                Components<T1>.Instance.Set(this, c1);
                Components<T2>.Instance.Set(this, c2);
                Components<T3>.Instance.Set(this, c3);
                Components<T4>.Instance.Set(this, c4);
                Components<T5>.Instance.Set(this, c5);
                Components<T6>.Instance.Set(this, c6);
                Components<T7>.Instance.Set(this, c7);
                Components<T8>.Instance.Set(this, c8);
                Components<T9>.Instance.Set(this, c9);
                Components<T10>.Instance.Set(this, c10);
                return this;
            }

            /// <inheritdoc cref="Set{T1, T2}(T1, T2)"/>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7, T8 c8, T9 c9, T10 c10, T11 c11)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where T6 : struct, IComponent
                where T7 : struct, IComponent
                where T8 : struct, IComponent
                where T9 : struct, IComponent
                where T10 : struct, IComponent
                where T11 : struct, IComponent {
                Components<T1>.Instance.Set(this, c1);
                Components<T2>.Instance.Set(this, c2);
                Components<T3>.Instance.Set(this, c3);
                Components<T4>.Instance.Set(this, c4);
                Components<T5>.Instance.Set(this, c5);
                Components<T6>.Instance.Set(this, c6);
                Components<T7>.Instance.Set(this, c7);
                Components<T8>.Instance.Set(this, c8);
                Components<T9>.Instance.Set(this, c9);
                Components<T10>.Instance.Set(this, c10);
                Components<T11>.Instance.Set(this, c11);
                return this;
            }

            /// <inheritdoc cref="Set{T1, T2}(T1, T2)"/>
            [MethodImpl(AggressiveInlining)]
            public readonly Entity Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7, T8 c8, T9 c9, T10 c10, T11 c11, T12 c12)
                where T1 : struct, IComponent
                where T2 : struct, IComponent
                where T3 : struct, IComponent
                where T4 : struct, IComponent
                where T5 : struct, IComponent
                where T6 : struct, IComponent
                where T7 : struct, IComponent
                where T8 : struct, IComponent
                where T9 : struct, IComponent
                where T10 : struct, IComponent
                where T11 : struct, IComponent
                where T12 : struct, IComponent {
                Components<T1>.Instance.Set(this, c1);
                Components<T2>.Instance.Set(this, c2);
                Components<T3>.Instance.Set(this, c3);
                Components<T4>.Instance.Set(this, c4);
                Components<T5>.Instance.Set(this, c5);
                Components<T6>.Instance.Set(this, c6);
                Components<T7>.Instance.Set(this, c7);
                Components<T8>.Instance.Set(this, c8);
                Components<T9>.Instance.Set(this, c9);
                Components<T10>.Instance.Set(this, c10);
                Components<T11>.Instance.Set(this, c11);
                Components<T12>.Instance.Set(this, c12);
                return this;
            }
            #endregion

            #region DELETE
            /// <summary>
            /// Removes a component from this entity. Triggers the OnDelete hook if the component existed.
            /// The component data is zeroed out and the entity will no longer match queries for this type.
            /// </summary>
            /// <typeparam name="T">Component type to remove.</typeparam>
            /// <returns><c>true</c> if the component was present and removed; <c>false</c> if the entity didn't have it.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly bool Delete<T>() where T : struct, IComponentOrTag {
                return Components<T>.Instance.Delete(this);
            }

            /// <summary>
            /// Removes multiple component types from this entity at once (2–5 types).
            /// OnDelete hooks are triggered for each removed component.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly void Delete<T1, T2>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                Components<T1>.Instance.Delete(this);
                Components<T2>.Instance.Delete(this);
            }

            /// <inheritdoc cref="Delete{T1, T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly void Delete<T1, T2, T3>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                Components<T1>.Instance.Delete(this);
                Components<T2>.Instance.Delete(this);
                Components<T3>.Instance.Delete(this);
            }

            /// <inheritdoc cref="Delete{T1, T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly void Delete<T1, T2, T3, T4>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag
                where T4 : struct, IComponentOrTag {
                Components<T1>.Instance.Delete(this);
                Components<T2>.Instance.Delete(this);
                Components<T3>.Instance.Delete(this);
                Components<T4>.Instance.Delete(this);
            }

            /// <inheritdoc cref="Delete{T1, T2}()"/>
            [MethodImpl(AggressiveInlining)]
            public readonly void Delete<T1, T2, T3, T4, T5>()
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag
                where T4 : struct, IComponentOrTag
                where T5 : struct, IComponentOrTag {
                Components<T1>.Instance.Delete(this);
                Components<T2>.Instance.Delete(this);
                Components<T3>.Instance.Delete(this);
                Components<T4>.Instance.Delete(this);
                Components<T5>.Instance.Delete(this);
            }
            #endregion

            #region COPY
            /// <summary>
            /// Copies specific component types from this entity to <paramref name="target"/>.
            /// Only the specified types are copied — other components on the source are not touched.
            /// If the target doesn't have a component, it is added. Triggers CopyTo hooks.
            /// <para>
            /// Unlike <see cref="CopyTo(Entity)"/> which copies ALL components and tags,
            /// this method selectively copies only the specified types.
            /// </para>
            /// </summary>
            /// <typeparam name="T1">Component type to copy.</typeparam>
            /// <param name="target">Target entity.</param>
            /// <returns><c>true</c> if the source had the component and it was copied.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly bool CopyTo<T1>(Entity target)
                where T1 : struct, IComponentOrTag {
                return Components<T1>.Instance.Copy(this, target);
            }

            /// <summary>
            /// Copies multiple specific component types from this entity to the target (2–5 types).
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly void CopyTo<T1, T2>(Entity target)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                Components<T1>.Instance.Copy(this, target);
                Components<T2>.Instance.Copy(this, target);
            }

            /// <inheritdoc cref="CopyTo{T1,T2}"/>
            [MethodImpl(AggressiveInlining)]
            public readonly void CopyTo<T1, T2, T3>(Entity target)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                Components<T1>.Instance.Copy(this, target);
                Components<T2>.Instance.Copy(this, target);
                Components<T3>.Instance.Copy(this, target);
            }

            /// <inheritdoc cref="CopyTo{T1,T2}"/>
            [MethodImpl(AggressiveInlining)]
            public readonly void CopyTo<T1, T2, T3, T4>(Entity target)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag
                where T4 : struct, IComponentOrTag {
                Components<T1>.Instance.Copy(this, target);
                Components<T2>.Instance.Copy(this, target);
                Components<T3>.Instance.Copy(this, target);
                Components<T4>.Instance.Copy(this, target);
            }

            /// <inheritdoc cref="CopyTo{T1,T2}"/>
            [MethodImpl(AggressiveInlining)]
            public readonly void CopyTo<T1, T2, T3, T4, T5>(Entity target)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag
                where T4 : struct, IComponentOrTag
                where T5 : struct, IComponentOrTag {
                Components<T1>.Instance.Copy(this, target);
                Components<T2>.Instance.Copy(this, target);
                Components<T3>.Instance.Copy(this, target);
                Components<T4>.Instance.Copy(this, target);
                Components<T5>.Instance.Copy(this, target);
            }
            #endregion

            #region MOVE
            /// <summary>
            /// Moves specific component types from this entity to <paramref name="target"/>:
            /// copies the value to the target and removes it from this entity.
            /// Triggers OnDelete on this entity and OnAdd/CopyTo on the target for each type.
            /// <para>
            /// Useful for transferring specific components between entities (e.g. transferring
            /// inventory from a dying entity to a container).
            /// </para>
            /// </summary>
            /// <typeparam name="T1">Component type to move.</typeparam>
            /// <param name="target">Target entity.</param>
            /// <returns><c>true</c> if the source had the component and it was moved.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly bool MoveTo<T1>(Entity target)
                where T1 : struct, IComponentOrTag {
                return Components<T1>.Instance.Move(this, target);
            }

            /// <summary>
            /// Moves multiple specific component types from this entity to the target (2–5 types).
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly void MoveTo<T1, T2>(Entity target)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag {
                Components<T1>.Instance.Move(this, target);
                Components<T2>.Instance.Move(this, target);
            }

            /// <inheritdoc cref="MoveTo{T1,T2}"/>
            [MethodImpl(AggressiveInlining)]
            public readonly void MoveTo<T1, T2, T3>(Entity target)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag {
                Components<T1>.Instance.Move(this, target);
                Components<T2>.Instance.Move(this, target);
                Components<T3>.Instance.Move(this, target);
            }

            /// <inheritdoc cref="MoveTo{T1,T2}"/>
            [MethodImpl(AggressiveInlining)]
            public readonly void MoveTo<T1, T2, T3, T4>(Entity target)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag
                where T4 : struct, IComponentOrTag {
                Components<T1>.Instance.Move(this, target);
                Components<T2>.Instance.Move(this, target);
                Components<T3>.Instance.Move(this, target);
                Components<T4>.Instance.Move(this, target);
            }

            /// <inheritdoc cref="MoveTo{T1,T2}"/>
            [MethodImpl(AggressiveInlining)]
            public readonly void MoveTo<T1, T2, T3, T4, T5>(Entity target)
                where T1 : struct, IComponentOrTag
                where T2 : struct, IComponentOrTag
                where T3 : struct, IComponentOrTag
                where T4 : struct, IComponentOrTag
                where T5 : struct, IComponentOrTag {
                Components<T1>.Instance.Move(this, target);
                Components<T2>.Instance.Move(this, target);
                Components<T3>.Instance.Move(this, target);
                Components<T4>.Instance.Move(this, target);
                Components<T5>.Instance.Move(this, target);
            }
            #endregion
            #endregion

            #region TAGS
            /// <summary>
            /// Returns the number of tags currently set on this entity.
            /// </summary>
            /// <returns>Tag count.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly int TagsCount() => Data.Instance.TagsCount(this);

            /// <summary>
            /// Collects all tags set on this entity into the provided list as boxed <see cref="ITag"/> values.
            /// The list is cleared before populating.
            /// Intended for debug/inspector tools.
            /// </summary>
            /// <param name="result">List to append tags to.</param>
            [MethodImpl(AggressiveInlining)]
            public readonly void GetAllTags(List<ITag> result) => Data.Instance.GetAllTags(this, result);
            
            /// <summary>
            /// Sets (adds) a tag on this entity. Tags are binary — either present or absent.
            /// If the tag is already set, this is a no-op.
            /// </summary>
            /// <typeparam name="T">Tag type to set.</typeparam>
            /// <returns><c>true</c> if the tag was newly added; <c>false</c> if already present.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly bool Set<T>() where T : struct, ITag {
                return Components<T>.Instance.Set(this);
            }

            /// <summary>
            /// Sets multiple tags on this entity at once.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly void Set<T1, T2>()
                where T1 : struct, ITag
                where T2 : struct, ITag {
                Components<T1>.Instance.Set(this);
                Components<T2>.Instance.Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly void Set<T1, T2, T3>()
                where T1 : struct, ITag
                where T2 : struct, ITag
                where T3 : struct, ITag {
                Components<T1>.Instance.Set(this);
                Components<T2>.Instance.Set(this);
                Components<T3>.Instance.Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly void Set<T1, T2, T3, T4>()
                where T1 : struct, ITag
                where T2 : struct, ITag
                where T3 : struct, ITag
                where T4 : struct, ITag {
                Components<T1>.Instance.Set(this);
                Components<T2>.Instance.Set(this);
                Components<T3>.Instance.Set(this);
                Components<T4>.Instance.Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly void Set<T1, T2, T3, T4, T5>()
                where T1 : struct, ITag
                where T2 : struct, ITag
                where T3 : struct, ITag
                where T4 : struct, ITag
                where T5 : struct, ITag {
                Components<T1>.Instance.Set(this);
                Components<T2>.Instance.Set(this);
                Components<T3>.Instance.Set(this);
                Components<T4>.Instance.Set(this);
                Components<T5>.Instance.Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool Toggle<T>() where T : struct, ITag {
                return Components<T>.Instance.Toggle(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly void Toggle<T1, T2>()
                where T1 : struct, ITag
                where T2 : struct, ITag {
                Components<T1>.Instance.Toggle(this);
                Components<T2>.Instance.Toggle(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly void Toggle<T1, T2, T3>()
                where T1 : struct, ITag
                where T2 : struct, ITag
                where T3 : struct, ITag {
                Components<T1>.Instance.Toggle(this);
                Components<T2>.Instance.Toggle(this);
                Components<T3>.Instance.Toggle(this);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly void Apply<T>(bool state)
                where T : struct, ITag {
                Components<T>.Instance.Apply(this, state);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly void Apply<T1, T2>(bool stateT1, bool stateT2)
                where T1 : struct, ITag
                where T2 : struct, ITag {
                Components<T1>.Instance.Apply(this, stateT1);
                Components<T2>.Instance.Apply(this, stateT2);
            }

            [MethodImpl(AggressiveInlining)]
            public readonly void Apply<T1, T2, T3>(bool stateT1, bool stateT2, bool stateT3)
                where T1 : struct, ITag
                where T2 : struct, ITag
                where T3 : struct, ITag {
                Components<T1>.Instance.Apply(this, stateT1);
                Components<T2>.Instance.Apply(this, stateT2);
                Components<T3>.Instance.Apply(this, stateT3);
            }
            #endregion

            #region RELATION
            /// <summary>
            /// Creates a typed link reference (<see cref="Link{TLinkType}"/>) from this entity's GID.
            /// Links are used in the relations system to establish typed connections between entities
            /// (e.g. parent-child, target, owner). The link stores the GID, making it safe across
            /// serialization and entity recycling.
            /// </summary>
            /// <typeparam name="TLinkType">
            /// The link type defining the relationship semantics (must implement <see cref="ILinkType"/>).
            /// </typeparam>
            /// <returns>A <see cref="Link{TLinkType}"/> referencing this entity.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly Link<TLinkType> AsLink<TLinkType>() where TLinkType : unmanaged, ILinkType {
                return new Link<TLinkType>(GID);
            }
            #endregion

        }
    }
    
    /// <summary>
    /// Marker interface for entity type classification. Implement on an empty struct to define
    /// a logical entity category (e.g. Bullet, Enemy, Effect). Entities of the same type are
    /// co-located in the same memory segments for optimal cache locality.
    /// <para>
    /// Optionally override <see cref="OnCreate{TWorld}"/> and <see cref="OnDestroy{TWorld}"/>
    /// to define lifecycle hooks. The struct can carry fields to parameterize creation.
    /// </para>
    /// <para>
    /// Register with <c>Types().EntityType&lt;T&gt;(id)</c>, passing a stable byte identifier (0–255).
    /// For auto-registration via <c>RegisterAll</c>, declare a <c>public static readonly byte Id</c> field.
    /// Id 0 is reserved for <see cref="Default"/>.
    /// </para>
    /// </summary>
    public interface IEntityType {
        /// <summary>
        /// Called after entity creation. Override to add components, tags, or perform setup.
        /// The struct instance may carry configuration data accessible via <c>this</c>.
        /// <para>
        /// When called through the generic API (<c>NewEntity&lt;T&gt;()</c>), this is a constrained
        /// call on a value type — the JIT devirtualizes and inlines it at zero cost.
        /// </para>
        /// </summary>
        void OnCreate<TWorld>(World<TWorld>.Entity entity) where TWorld : struct, IWorldType {}

        /// <summary>
        /// Called before entity destruction (before component OnDelete hooks).
        /// Override for cleanup logic, event sending, pool return, etc.
        /// <para>
        /// All entity components and tags are still accessible at this point.
        /// </para>
        /// </summary>
        void OnDestroy<TWorld>(World<TWorld>.Entity entity, HookReason reason) where TWorld : struct, IWorldType {}
    }

    /// <summary>
    /// Built-in default entity type (Id = 0). Automatically registered when the world is created.
    /// Used when no specific entity type is needed.
    /// </summary>
    public readonly struct Default : IEntityType {
        public static readonly byte Id = 0;
    }
}