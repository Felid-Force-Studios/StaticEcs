#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

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

    /// <summary>
    /// Type-erased handle for accessing a <see cref="World{TWorld}"/> without knowing the concrete
    /// <c>TWorld</c> type at compile time. Bridges the static-generic ECS world to runtime-dynamic
    /// access via function pointers.
    /// <para>
    /// <b>Primary use cases:</b> editor/inspector tools, debug visualizers, serialization frameworks,
    /// and plugin systems that need to interact with any world instance without generic type parameters.
    /// All operations go through unsafe function pointers stored at creation time, so there is minimal
    /// overhead beyond the indirection itself.
    /// </para>
    /// <para>
    /// Obtain a WorldHandle via <c>World&lt;TWorld&gt;.Handle</c> or internal creation methods.
    /// The handle remains valid as long as the world exists. Entity IDs used in this handle are
    /// raw <c>uint</c> values.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct WorldHandle {
        private readonly unsafe delegate*<EntityGID, GIDStatus> _gidStatus;
        private readonly unsafe delegate*<byte, ushort, EntityGID> _newEntity;
        private readonly unsafe delegate*<uint> _calculateEntitiesCount;
        private readonly unsafe delegate*<uint> _calculateEntitiesCapacity;
        private readonly unsafe delegate*<EntityGID, bool> _destroyEntity;
        private readonly unsafe delegate*<void> _destroyAllEntities;
        private readonly unsafe delegate*<ushort, void> _destroyAllEntitiesInCluster;
        private readonly unsafe delegate*<uint, void> _destroyAllEntitiesInChunk;
        private readonly unsafe delegate*<WorldStatus> _status;
        private readonly unsafe delegate*<Type, out ComponentsHandle, bool> _tryGetComponentsHandle;
        private readonly unsafe delegate*<Type, ref ComponentsHandle> _getComponentsHandle;
        private readonly unsafe delegate*<ReadOnlySpan<ComponentsHandle>> _getAllComponentsHandles;
        private readonly unsafe delegate*<Type, out EventsHandle, bool> _tryGetEventsHandle;
        private readonly unsafe delegate*<Type, ref EventsHandle> _getEventsHandle;
        private readonly unsafe delegate*<ReadOnlySpan<EventsHandle>> _getAllEventsHandles;
        private readonly unsafe delegate*<Type, bool> _hasResource;
        private readonly unsafe delegate*<Type, object, bool, void> _setResource;
        private readonly unsafe delegate*<Type, object> _getResource;
        private readonly unsafe delegate*<Type, void> _removeResource;
        private readonly unsafe delegate*<string, bool> _hasResourceByKey;
        private readonly unsafe delegate*<string, object> _getResourceByKey;
        private readonly unsafe delegate*<string, object, bool, void> _setResourceByKey;
        private readonly unsafe delegate*<string, void> _removeResourceByKey;
        private readonly unsafe delegate*<IReadOnlyCollection<string>> _getAllResourcesKeys;
        private readonly unsafe delegate*<IReadOnlyCollection<Type>> _getAllResourcesTypes;
        private readonly unsafe delegate*<byte, bool> _isEntityTypeRegistered;
        private readonly unsafe delegate*<byte, uint> _calculateEntitiesCountByType;
        private readonly unsafe delegate*<byte, uint> _calculateEntitiesCapacityByType;

        /// <summary>
        /// The <see cref="Type"/> of the <c>TWorld</c> struct that this handle represents.
        /// Useful for distinguishing between handles for different worlds at runtime.
        /// </summary>
        public readonly Type WorldType;

        internal static unsafe WorldHandle Create<TWorld>()
            where TWorld : struct, IWorldType {
            return new WorldHandle(
                typeof(TWorld),
                &World<TWorld>._GidStatus,
                &World<TWorld>._NewEntity,
                &World<TWorld>._CalculateEntitiesCount,
                &World<TWorld>._CalculateEntitiesCapacity,
                &World<TWorld>._DestroyEntity,
                &World<TWorld>._DestroyAllEntities,
                &World<TWorld>._DestroyAllEntitiesInCluster,
                &World<TWorld>._DestroyAllEntitiesInChunk,
                &World<TWorld>._WorldStatus,
                &World<TWorld>._TryGetComponentsHandle,
                &World<TWorld>._GetComponentsHandle,
                &World<TWorld>._GetAllComponentsHandles,
                &World<TWorld>._TryGetEventsHandle,
                &World<TWorld>._GetEventsHandle,
                &World<TWorld>._GetAllEventsHandles,
                &World<TWorld>._HasResource,
                &World<TWorld>._SetResource,
                &World<TWorld>._GetResource,
                &World<TWorld>._RemoveResource,
                &World<TWorld>._HasResourceByKey,
                &World<TWorld>._GetResourceByKey,
                &World<TWorld>._SetResourceByKey,
                &World<TWorld>._RemoveResourceByKey,
                &World<TWorld>._GetAllResourcesKeys,
                &World<TWorld>._GetAllResourcesTypes,
                &World<TWorld>._IsEntityTypeRegistered,
                &World<TWorld>._CalculateEntitiesCountByType,
                &World<TWorld>._CalculateEntitiesCapacityByType
            );
        }

        internal unsafe WorldHandle(
            Type worldType,
            delegate*<EntityGID, GIDStatus> gidStatus,
            delegate*<byte, ushort, EntityGID> newEntity,
            delegate*<uint> calculateEntitiesCount,
            delegate*<uint> calculateEntitiesCapacity,
            delegate*<EntityGID, bool> destroyEntity,
            delegate*<void> destroyAllEntities,
            delegate*<ushort, void> destroyAllEntitiesInCluster,
            delegate*<uint, void> destroyAllEntitiesInChunk,
            delegate*<WorldStatus> status,
            delegate*<Type, out ComponentsHandle, bool> tryGetComponentsHandle,
            delegate*<Type, ref ComponentsHandle> getComponentsHandle,
            delegate*<ReadOnlySpan<ComponentsHandle>> getAllComponentsHandles,
            delegate*<Type, out EventsHandle, bool> tryGetEventsHandle,
            delegate*<Type, ref EventsHandle> getEventsHandle,
            delegate*<ReadOnlySpan<EventsHandle>> getAllEventsHandles,
            delegate*<Type, bool> hasResource,
            delegate*<Type, object, bool, void> setResource,
            delegate*<Type, object> getResource,
            delegate*<Type, void> removeResource,
            delegate*<string, bool> hasResourceByKey,
            delegate*<string, object> getResourceByKey,
            delegate*<string, object, bool, void> setResourceByKey,
            delegate*<string, void> removeResourceByKey,
            delegate*<IReadOnlyCollection<string>> getAllResourcesKeys,
            delegate*<IReadOnlyCollection<Type>> getAllResourcesTypes,
            delegate*<byte, bool> isEntityTypeRegistered,
            delegate*<byte, uint> calculateEntitiesCountByType,
            delegate*<byte, uint> calculateEntitiesCapacityByType) {
            WorldType = worldType;
            _gidStatus = gidStatus;
            _newEntity = newEntity;
            _calculateEntitiesCount = calculateEntitiesCount;
            _calculateEntitiesCapacity = calculateEntitiesCapacity;
            _destroyEntity = destroyEntity;
            _destroyAllEntities = destroyAllEntities;
            _destroyAllEntitiesInCluster = destroyAllEntitiesInCluster;
            _destroyAllEntitiesInChunk = destroyAllEntitiesInChunk;
            _status = status;
            _tryGetComponentsHandle = tryGetComponentsHandle;
            _getComponentsHandle = getComponentsHandle;
            _getAllComponentsHandles = getAllComponentsHandles;
            _tryGetEventsHandle = tryGetEventsHandle;
            _getEventsHandle = getEventsHandle;
            _getAllEventsHandles = getAllEventsHandles;
            _hasResource = hasResource;
            _setResource = setResource;
            _getResource = getResource;
            _removeResource = removeResource;
            _hasResourceByKey = hasResourceByKey;
            _getResourceByKey = getResourceByKey;
            _setResourceByKey = setResourceByKey;
            _removeResourceByKey = removeResourceByKey;
            _getAllResourcesKeys = getAllResourcesKeys;
            _getAllResourcesTypes = getAllResourcesTypes;
            _isEntityTypeRegistered = isEntityTypeRegistered;
            _calculateEntitiesCountByType = calculateEntitiesCountByType;
            _calculateEntitiesCapacityByType = calculateEntitiesCapacityByType;
        }

        /// <summary>
        /// Returns the current lifecycle status of the world (NotCreated, Created, or Initialized).
        /// </summary>
        /// <returns>Current <see cref="WorldStatus"/>.</returns>
        [MethodImpl(AggressiveInlining)]
        public WorldStatus Status() {
            unsafe { return _status(); }
        }

        #region ENTITIES
        /// <summary>
        /// Returns the <see cref="GIDStatus"/> of the entity referenced by <paramref name="gid"/>.
        /// This is the type-erased equivalent of <see cref="EntityGID.Status{TWorld}()"/>.
        /// </summary>
        /// <param name="gid">The global entity identifier to check.</param>
        /// <returns>
        /// <see cref="StaticEcs.GIDStatus.Active"/> if the entity exists, version matches, and it is loaded,
        /// <see cref="StaticEcs.GIDStatus.NotActual"/> if the entity does not exist or version/cluster doesn't match,
        /// <see cref="StaticEcs.GIDStatus.NotLoaded"/> if the entity exists and version matches but is unloaded.
        /// </returns>
        [MethodImpl(AggressiveInlining)]
        public GIDStatus GIDStatus(EntityGID gid) {
            unsafe { return _gidStatus(gid); }
        }

        /// <summary>
        /// Creates a new entity with the specified type in the specified cluster and returns its <see cref="EntityGID"/>.
        /// Unlike the generic <c>World&lt;TWorld&gt;.NewEntity</c> which returns an <c>Entity</c>
        /// struct, this returns a GID suitable for storage in type-erased contexts (editors, tools).
        /// </summary>
        /// <param name="entityType">Entity type (0–255). Entities of the same type are grouped into the same segments within a cluster.</param>
        /// <param name="clusterId">Cluster to create the entity in.</param>
        /// <returns>The <see cref="EntityGID"/> of the newly created entity.</returns>
        [MethodImpl(AggressiveInlining)]
        public EntityGID NewEntity(byte entityType, ushort clusterId = 0) {
            unsafe { return _newEntity(entityType, clusterId); }
        }

        /// <summary>
        /// Calculates the total number of alive entities across all chunks in the world.
        /// See <see cref="World{TWorld}.CalculateEntitiesCount"/>.
        /// </summary>
        /// <returns>Total entity count.</returns>
        [MethodImpl(AggressiveInlining)]
        public uint CalculateEntitiesCount() {
            unsafe { return _calculateEntitiesCount(); }
        }

        /// <summary>
        /// Calculates the total entity slot capacity across all registered chunks.
        /// See <see cref="World{TWorld}.CalculateEntitiesCapacity"/>.
        /// </summary>
        /// <returns>Total allocated entity slot count.</returns>
        [MethodImpl(AggressiveInlining)]
        public uint CalculateEntitiesCapacity() {
            unsafe { return _calculateEntitiesCapacity(); }
        }

        /// <summary>
        /// Returns whether the entity type with the specified byte ID is registered in this world.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public bool IsEntityTypeRegistered(byte id) {
            unsafe { return _isEntityTypeRegistered(id); }
        }

        /// <summary>
        /// Calculates the number of alive entities of the specified type.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public uint CalculateEntitiesCountByType(byte entityType) {
            unsafe { return _calculateEntitiesCountByType(entityType); }
        }

        /// <summary>
        /// Calculates the capacity (allocated segments × 256) for the specified entity type.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public uint CalculateEntitiesCapacityByType(byte entityType) {
            unsafe { return _calculateEntitiesCapacityByType(entityType); }
        }

        /// <summary>
        /// Destroys the entity referenced by <paramref name="gid"/>. Idempotent: returns <c>false</c>
        /// if the entity is already destroyed or the GID is stale.
        /// </summary>
        /// <param name="gid">The global entity identifier to destroy.</param>
        /// <returns><c>true</c> if the entity was destroyed; <c>false</c> if already destroyed or GID is stale.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool DestroyEntity(EntityGID gid) {
            unsafe { return _destroyEntity(gid); }
        }

        /// <summary>
        /// Destroys every loaded entity in the world, triggering OnDelete hooks.
        /// The world remains initialized. See <see cref="World{TWorld}.DestroyAllLoadedEntities"/>.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public void DestroyAllLoadedEntities() {
            unsafe { _destroyAllEntities(); }
        }

        /// <summary>
        /// Destroys all entities within a specific cluster.
        /// See <see cref="World{TWorld}.DestroyAllEntitiesInCluster"/>.
        /// </summary>
        /// <param name="clusterId">Cluster identifier.</param>
        [MethodImpl(AggressiveInlining)]
        public void DestroyAllEntitiesInCluster(ushort clusterId) {
            unsafe { _destroyAllEntitiesInCluster(clusterId); }
        }

        /// <summary>
        /// Destroys all entities within a specific chunk.
        /// See <see cref="World{TWorld}.DestroyAllEntitiesInChunk"/>.
        /// </summary>
        /// <param name="chunkId">Chunk index.</param>
        [MethodImpl(AggressiveInlining)]
        public void DestroyAllEntitiesInChunk(uint chunkId) {
            unsafe { _destroyAllEntitiesInChunk(chunkId); }
        }
        #endregion

        #region COMPONENTS
        /// <summary>
        /// Attempts to obtain a type-erased <see cref="ComponentsHandle"/> for the specified
        /// component type. Returns <c>false</c> if the component type is not registered in this world.
        /// <para>
        /// Use this to dynamically discover and manipulate components when the concrete type
        /// is only known at runtime (e.g. editor inspectors iterating over all components on an entity).
        /// </para>
        /// </summary>
        /// <param name="componentType">The <see cref="Type"/> of the component struct (must implement <see cref="IComponent"/>).</param>
        /// <param name="handle">The resulting handle. Valid only when the method returns <c>true</c>.</param>
        /// <returns><c>true</c> if the component type is registered and the handle was created.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool TryGetComponentsHandle(Type componentType, out ComponentsHandle handle) {
            unsafe { return _tryGetComponentsHandle(componentType, out handle); }
        }

        /// <summary>
        /// Returns a reference to the <see cref="ComponentsHandle"/> for the specified component type.
        /// Throws <see cref="StaticEcsException"/> if the component type is not registered.
        /// <para>
        /// Unlike <see cref="TryGetComponentsHandle"/>, this method avoids copying the entire handle
        /// struct by returning a direct reference into the internal storage.
        /// Use when the component type is guaranteed to be registered.
        /// </para>
        /// </summary>
        /// <param name="componentType">The <see cref="Type"/> of the component struct (must implement <see cref="IComponent"/>).</param>
        /// <returns>A readonly reference to the <see cref="ComponentsHandle"/>.</returns>
        [MethodImpl(AggressiveInlining)]
        public ref readonly ComponentsHandle GetComponentsHandle(Type componentType) {
            unsafe { return ref _getComponentsHandle(componentType); }
        }

        /// <summary>
        /// Returns handles for all component types registered in this world.
        /// Useful for editor tools that need to enumerate and display all available component types.
        /// </summary>
        /// <returns>Read-only span of all registered <see cref="ComponentsHandle"/> instances.</returns>
        [MethodImpl(AggressiveInlining)]
        public ReadOnlySpan<ComponentsHandle> GetAllComponentsHandles() {
            unsafe { return _getAllComponentsHandles(); }
        }
        #endregion

        #region EVENTS
        /// <summary>
        /// Attempts to obtain a type-erased <see cref="EventsHandle"/> for the specified event type.
        /// Returns <c>false</c> if the event type is not registered.
        /// </summary>
        /// <param name="eventType">The <see cref="Type"/> of the event struct (must implement <see cref="IEvent"/>).</param>
        /// <param name="handle">The resulting handle. Valid only when the method returns <c>true</c>.</param>
        /// <returns><c>true</c> if the event type is registered.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool TryGetEventsHandle(Type eventType, out EventsHandle handle) {
            unsafe { return _tryGetEventsHandle(eventType, out handle); }
        }

        /// <summary>
        /// Returns a reference to the <see cref="EventsHandle"/> for the specified event type.
        /// Throws <see cref="StaticEcsException"/> if the event type is not registered.
        /// <para>
        /// Unlike <see cref="TryGetEventsHandle"/>, this method avoids copying the entire handle
        /// struct by returning a direct reference into the internal storage.
        /// Use when the event type is guaranteed to be registered.
        /// </para>
        /// </summary>
        /// <param name="eventType">The <see cref="Type"/> of the event struct (must implement <see cref="IEvent"/>).</param>
        /// <returns>A readonly reference to the <see cref="EventsHandle"/>.</returns>
        [MethodImpl(AggressiveInlining)]
        public ref readonly EventsHandle GetEventsHandle(Type eventType) {
            unsafe { return ref _getEventsHandle(eventType); }
        }

        /// <summary>
        /// Returns handles for all event types registered in this world.
        /// </summary>
        /// <returns>Read-only span of all registered <see cref="EventsHandle"/> instances.</returns>
        [MethodImpl(AggressiveInlining)]
        public ReadOnlySpan<EventsHandle> GetAllEventsHandles() {
            unsafe { return _getAllEventsHandles(); }
        }
        #endregion

        #region RESOURCES
        /// <summary>
        /// Checks whether a singleton resource of the given runtime <see cref="Type"/> exists.
        /// Type-erased equivalent of <see cref="World{TWorld}.HasResource{TResource}()"/>.
        /// </summary>
        /// <param name="type">Runtime type of the resource.</param>
        /// <returns><c>true</c> if the resource exists.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool HasResource(Type type) {
            unsafe { return _hasResource(type); }
        }

        /// <summary>
        /// Checks whether a keyed resource with the given string key exists.
        /// </summary>
        /// <param name="key">String key identifying the resource.</param>
        /// <returns><c>true</c> if the keyed resource exists.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool HasResource(string key) {
            unsafe { return _hasResourceByKey(key); }
        }

        /// <summary>
        /// Returns the singleton resource of the given runtime type as a boxed object.
        /// The resource must have been previously set.
        /// </summary>
        /// <param name="type">Runtime type of the resource.</param>
        /// <returns>The resource value, boxed as <see cref="object"/>.</returns>
        [MethodImpl(AggressiveInlining)]
        public object GetResource(Type type) {
            unsafe { return _getResource(type); }
        }

        /// <summary>
        /// Returns a keyed resource as a boxed object.
        /// </summary>
        /// <param name="key">String key identifying the resource.</param>
        /// <returns>The resource value, boxed as <see cref="object"/>.</returns>
        [MethodImpl(AggressiveInlining)]
        public object GetResource(string key) {
            unsafe { return _getResourceByKey(key); }
        }

        /// <summary>
        /// Removes the singleton resource of the given runtime type.
        /// </summary>
        /// <param name="type">Runtime type of the resource to remove.</param>
        [MethodImpl(AggressiveInlining)]
        public void RemoveResource(Type type) {
            unsafe { _removeResource(type); }
        }

        /// <summary>
        /// Removes a keyed resource.
        /// </summary>
        /// <param name="key">String key of the resource to remove.</param>
        [MethodImpl(AggressiveInlining)]
        public void RemoveResource(string key) {
            unsafe { _removeResourceByKey(key); }
        }

        /// <summary>
        /// Sets (or replaces) a singleton resource, accepting the value as a boxed object.
        /// </summary>
        /// <param name="type">Runtime type of the resource.</param>
        /// <param name="value">The resource value (will be stored as-is; must match the type).</param>
        /// <param name="clearOnDestroy">If <c>true</c>, the resource is cleared when the world is destroyed.</param>
        [MethodImpl(AggressiveInlining)]
        public void SetResource(Type type, object value, bool clearOnDestroy) {
            unsafe { _setResource(type, value, clearOnDestroy); }
        }

        /// <summary>
        /// Sets (or replaces) a keyed resource, accepting the value as a boxed object.
        /// </summary>
        /// <param name="key">String key for this resource instance.</param>
        /// <param name="value">The resource value.</param>
        /// <param name="clearOnDestroy">If <c>true</c>, the resource is cleared when the world is destroyed.</param>
        [MethodImpl(AggressiveInlining)]
        public void SetResource(string key, object value, bool clearOnDestroy) {
            unsafe { _setResourceByKey(key, value, clearOnDestroy); }
        }

        /// <summary>
        /// Returns all string keys of currently stored keyed resources.
        /// Useful for editor/inspector enumeration.
        /// </summary>
        /// <returns>Read-only collection of resource keys.</returns>
        [MethodImpl(AggressiveInlining)]
        public IReadOnlyCollection<string> GetAllResourcesKeys() {
            unsafe { return _getAllResourcesKeys(); }
        }

        /// <summary>
        /// Returns the <see cref="Type"/> objects of all currently stored singleton (non-keyed) resources.
        /// Useful for editor/inspector enumeration.
        /// </summary>
        /// <returns>Read-only collection of resource types.</returns>
        [MethodImpl(AggressiveInlining)]
        public IReadOnlyCollection<Type> GetAllResourcesTypes() {
            unsafe { return _getAllResourcesTypes(); }
        }
        #endregion
    }

    /// <summary>
    /// Type-erased handle for accessing a specific component type's storage without knowing
    /// the concrete component type at compile time. Provides runtime-dynamic operations on
    /// components: add, remove, read/write (boxed), enable/disable, copy, move, and bitmap queries.
    /// <para>
    /// Obtained via <see cref="WorldHandle.TryGetComponentsHandle"/> or
    /// <see cref="WorldHandle.GetAllComponentsHandles"/>. All entity IDs used here are raw
    /// <c>uint</c> values (with <c>ENTITY_ID_OFFSET</c> applied).
    /// </para>
    /// <para>
    /// <b>Performance note:</b> These operations involve boxing for component values (via
    /// <see cref="IComponent"/>) and function pointer indirection. For hot-path code, use the
    /// generic <c>World&lt;TWorld&gt;.Components&lt;T&gt;</c> API instead. ComponentsHandle is
    /// intended for editor tools, serialization, and other non-performance-critical scenarios.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct ComponentsHandle {
        // base
        private readonly unsafe delegate*<uint, ulong[][], ushort, void> _initialize;
        private readonly unsafe delegate*<uint, ulong[][], void> _resize;
        private readonly unsafe delegate*<void> _destroy;
        private readonly unsafe delegate*<void> _hardReset;
        private readonly unsafe delegate*<StringBuilder, uint, void> _tryToStringComponent;
        // serialization
        private readonly unsafe delegate*<ref BinaryPackWriter, uint, void> _writeChunk;
        private readonly unsafe delegate*<ref BinaryPackReader, uint, void> _readChunk;
        private readonly unsafe delegate*<ref BinaryPackWriter, uint, bool, bool> _writeEntity;
        private readonly unsafe delegate*<ref BinaryPackReader, uint, void> _readEntity;
        // chunks & segments
        private readonly unsafe delegate*<HeuristicChunk[]> _heuristicChunks;
        private readonly unsafe delegate*<Array> _componentsSegments;
        private readonly unsafe delegate*<uint, bool> _isSegmentAllocated;
        private readonly unsafe delegate*<uint, int, ulong> _enabledMask;
        private readonly unsafe delegate*<uint, int, ulong> _disabledMask;
        private readonly unsafe delegate*<uint, int, ulong> _anyMask;
        private readonly unsafe delegate*<ulong, uint, byte, HookReason, void> _batchDelete;
        private readonly unsafe delegate*<int> _calculateCapacity;
        private readonly unsafe delegate*<uint> _calculateCount;
        // entity
        private readonly unsafe delegate*<uint, out IComponentOrTag, bool> _tryGetRaw;
        private readonly unsafe delegate*<uint, IComponentOrTag, void> _setRaw;
        private readonly unsafe delegate*<uint, IComponentOrTag, void> _setRawDirect;
        private readonly unsafe delegate*<uint, bool> _add;
        private readonly unsafe delegate*<uint, bool> _has;
        private readonly unsafe delegate*<uint, bool> _hasEnabled;
        private readonly unsafe delegate*<uint, bool> _hasDisabled;
        private readonly unsafe delegate*<uint, ToggleResult> _enable;
        private readonly unsafe delegate*<uint, ToggleResult> _disable;
        private readonly unsafe delegate*<uint, HookReason, bool> _delete;
        private readonly unsafe delegate*<uint, uint, bool> _copy;
        private readonly unsafe delegate*<uint, uint, bool> _move;
        private readonly unsafe delegate*<uint, void> _set;
        private readonly unsafe delegate*<IComponentOrTag> _defaultValue;
        private readonly unsafe delegate*<void> _clearTracking;
        private readonly unsafe delegate*<void> _clearAddedTracking;
        private readonly unsafe delegate*<void> _clearDeletedTracking;
        #if !FFS_ECS_DISABLE_CHANGED_TRACKING
        private readonly unsafe delegate*<void> _clearChangedTracking;
        #endif
        private readonly unsafe delegate*<void> _advanceTracking;

        /// <summary>
        /// The <see cref="Type"/> of the component struct this handle operates on.
        /// </summary>
        public readonly Type ComponentType;

        /// <summary>
        /// The <see cref="Type"/> of the <c>TWorld</c> struct this handle's world belongs to.
        /// </summary>
        public readonly Type WorldType;

        /// <summary>
        /// Stable GUID assigned to this component type, used for serialization identification.
        /// Allows renaming the component type without breaking saved data.
        /// </summary>
        public readonly Guid Guid;

        /// <summary>
        /// Runtime-assigned numeric ID for this component type within the world.
        /// Used internally for bitmap indexing. Not stable across runs — do not serialize.
        /// </summary>
        public readonly ushort DynamicId;

        /// <summary>
        /// Whether this handle represents a tag type (zero-size marker component with no data arrays).
        /// </summary>
        public readonly bool IsTag;

        internal static unsafe ComponentsHandle Create<TWorld, TComponent>()
            where TWorld : struct, IWorldType
            where TComponent : struct, IComponentOrTag {
            return new ComponentsHandle(
                typeof(TComponent),
                typeof(TWorld),
                World<TWorld>.Components<TComponent>.Instance.Guid,
                World<TWorld>.Components<TComponent>.Instance.DynamicId,
                World<TWorld>.Components<TComponent>.Instance.IsTag,
                &World<TWorld>.Components<TComponent>._Initialize,
                &World<TWorld>.Components<TComponent>._Resize,
                &World<TWorld>.Components<TComponent>._Destroy,
                &World<TWorld>.Components<TComponent>._HardReset,
                &World<TWorld>.Components<TComponent>._TryToStringComponent,
                &World<TWorld>.Components<TComponent>._WriteChunk,
                &World<TWorld>.Components<TComponent>._ReadChunk,
                &World<TWorld>.Components<TComponent>._WriteEntity,
                &World<TWorld>.Components<TComponent>._ReadEntity,
                &World<TWorld>.Components<TComponent>._HeuristicChunks,
                &World<TWorld>.Components<TComponent>._ComponentsSegments,
                &World<TWorld>.Components<TComponent>._IsSegmentAllocated,
                &World<TWorld>.Components<TComponent>._EnabledMask,
                &World<TWorld>.Components<TComponent>._DisabledMask,
                &World<TWorld>.Components<TComponent>._AnyMask,
                &World<TWorld>.Components<TComponent>._BatchDelete,
                &World<TWorld>.Components<TComponent>._CalculateCapacity,
                &World<TWorld>.Components<TComponent>._CalculateCount,
                &World<TWorld>.Components<TComponent>._TryGetRaw,
                &World<TWorld>.Components<TComponent>._SetRaw,
                &World<TWorld>.Components<TComponent>._SetRawDirect,
                &World<TWorld>.Components<TComponent>._Add,
                &World<TWorld>.Components<TComponent>._Has,
                &World<TWorld>.Components<TComponent>._HasEnabled,
                &World<TWorld>.Components<TComponent>._HasDisabled,
                &World<TWorld>.Components<TComponent>._Enable,
                &World<TWorld>.Components<TComponent>._Disable,
                &World<TWorld>.Components<TComponent>._Delete,
                &World<TWorld>.Components<TComponent>._Copy,
                &World<TWorld>.Components<TComponent>._Move,
                &World<TWorld>.Components<TComponent>._Set,
                &World<TWorld>.Components<TComponent>._DefaultValue,
                &World<TWorld>.Components<TComponent>._ClearTracking,
                &World<TWorld>.Components<TComponent>._ClearAddedTracking,
                &World<TWorld>.Components<TComponent>._ClearDeletedTracking,
                &World<TWorld>.Components<TComponent>._AdvanceTracking
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                , &World<TWorld>.Components<TComponent>._ClearChangedTracking
                #endif
            );
        }

        internal unsafe ComponentsHandle(
            Type componentType,
            Type worldType,
            Guid guid,
            ushort dynamicId,
            bool isTag,
            delegate*<uint, ulong[][], ushort, void> initialize,
            delegate*<uint, ulong[][], void> resize,
            delegate*<void> destroy,
            delegate*<void> hardReset,
            delegate*<StringBuilder, uint, void> tryToStringComponent,
            delegate*<ref BinaryPackWriter, uint, void> writeChunk,
            delegate*<ref BinaryPackReader, uint, void> readChunk,
            delegate*<ref BinaryPackWriter, uint, bool, bool> writeEntity,
            delegate*<ref BinaryPackReader, uint, void> readEntity,
            delegate*<HeuristicChunk[]> heuristicChunks,
            delegate*<Array> componentsSegments,
            delegate*<uint, bool> isSegmentAllocated,
            delegate*<uint, int, ulong> enabledMask,
            delegate*<uint, int, ulong> disabledMask,
            delegate*<uint, int, ulong> anyMask,
            delegate*<ulong, uint, byte, HookReason, void> batchDelete,
            delegate*<int> calculateCapacity,
            delegate*<uint> calculateCount,
            delegate*<uint, out IComponentOrTag, bool> tryGetRaw,
            delegate*<uint, IComponentOrTag, void> setRaw,
            delegate*<uint, IComponentOrTag, void> setRawDirect,
            delegate*<uint, bool> add,
            delegate*<uint, bool> has,
            delegate*<uint, bool> hasEnabled,
            delegate*<uint, bool> hasDisabled,
            delegate*<uint, ToggleResult> enable,
            delegate*<uint, ToggleResult> disable,
            delegate*<uint, HookReason, bool> delete,
            delegate*<uint, uint, bool> copy,
            delegate*<uint, uint, bool> move,
            delegate*<uint, void> set,
            delegate*<IComponentOrTag> defaultValue,
            delegate*<void> clearTracking,
            delegate*<void> clearAddedTracking,
            delegate*<void> clearDeletedTracking,
            delegate*<void> advanceTracking
            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            , delegate*<void> clearChangedTracking
            #endif
            ) {
            ComponentType = componentType;
            WorldType = worldType;
            Guid = guid;
            DynamicId = dynamicId;
            IsTag = isTag;
            _initialize = initialize;
            _resize = resize;
            _destroy = destroy;
            _hardReset = hardReset;
            _tryToStringComponent = tryToStringComponent;
            _writeChunk = writeChunk;
            _readChunk = readChunk;
            _writeEntity = writeEntity;
            _readEntity = readEntity;
            _heuristicChunks = heuristicChunks;
            _componentsSegments = componentsSegments;
            _isSegmentAllocated = isSegmentAllocated;
            _enabledMask = enabledMask;
            _disabledMask = disabledMask;
            _anyMask = anyMask;
            _batchDelete = batchDelete;
            _calculateCapacity = calculateCapacity;
            _calculateCount = calculateCount;
            _tryGetRaw = tryGetRaw;
            _setRaw = setRaw;
            _setRawDirect = setRawDirect;
            _add = add;
            _has = has;
            _hasEnabled = hasEnabled;
            _hasDisabled = hasDisabled;
            _enable = enable;
            _disable = disable;
            _delete = delete;
            _copy = copy;
            _move = move;
            _set = set;
            _defaultValue = defaultValue;
            _clearTracking = clearTracking;
            _clearAddedTracking = clearAddedTracking;
            _clearDeletedTracking = clearDeletedTracking;
            _advanceTracking = advanceTracking;
            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            _clearChangedTracking = clearChangedTracking;
            #endif
        }

        #region BASE
        [MethodImpl(AggressiveInlining)]
        internal void Initialize(uint chunksCapacity, ulong[][] chunkHeuristicMask, ushort chunkHeuristicMaskLen) {
            unsafe { _initialize(chunksCapacity, chunkHeuristicMask, chunkHeuristicMaskLen); }
        }

        [MethodImpl(AggressiveInlining)]
        internal void Resize(uint chunksCapacity, ulong[][] chunkHeuristicMask) {
            unsafe { _resize(chunksCapacity, chunkHeuristicMask); }
        }

        [MethodImpl(AggressiveInlining)]
        internal void Destroy() {
            unsafe { _destroy(); }
        }

        [MethodImpl(AggressiveInlining)]
        internal void HardReset() {
            unsafe { _hardReset(); }
        }

        [MethodImpl(AggressiveInlining)]
        public unsafe void ClearTracking() { _clearTracking(); }

        [MethodImpl(AggressiveInlining)]
        public unsafe void ClearAddedTracking() { _clearAddedTracking(); }

        [MethodImpl(AggressiveInlining)]
        public unsafe void ClearDeletedTracking() { _clearDeletedTracking(); }

        #if !FFS_ECS_DISABLE_CHANGED_TRACKING
        [MethodImpl(AggressiveInlining)]
        public unsafe void ClearChangedTracking() { _clearChangedTracking(); }
        #endif

        [MethodImpl(AggressiveInlining)]
        internal unsafe void AdvanceTracking() { _advanceTracking(); }
        #endregion

        #region SERIALIZATION
        [MethodImpl(AggressiveInlining)]
        internal void WriteChunk(ref BinaryPackWriter writer, uint chunkIdx) {
            unsafe { _writeChunk(ref writer, chunkIdx); }
        }

        [MethodImpl(AggressiveInlining)]
        internal void ReadChunk(ref BinaryPackReader reader, uint chunkIdx) {
            unsafe { _readChunk(ref reader, chunkIdx); }
        }

        [MethodImpl(AggressiveInlining)]
        internal bool WriteEntity(ref BinaryPackWriter writer, uint entityId, bool deleteComponent) {
            unsafe { return _writeEntity(ref writer, entityId, deleteComponent); }
        }

        [MethodImpl(AggressiveInlining)]
        internal void ReadEntity(ref BinaryPackReader reader, uint entityId) {
            unsafe { _readEntity(ref reader, entityId); }
        }
        #endregion

        #region CHUNKS_AND_SEGMENTS
        /// <summary>
        /// Returns the <see cref="StaticEcs.HeuristicChunk"/> for this component type at the given chunk.
        /// The heuristic chunk contains bitmap pairs (NotEmptyBlocks / FullBlocks) that enable
        /// fast chunk-level filtering during queries — checking if a chunk has any entities with
        /// this component without scanning individual blocks.
        /// </summary>
        /// <param name="chunkIdx">Chunk index.</param>
        /// <returns>Heuristic bitmap data for the specified chunk.</returns>
        [MethodImpl(AggressiveInlining)]
        public HeuristicChunk HeuristicChunk(uint chunkIdx) {
            unsafe { return _heuristicChunks()[chunkIdx]; }
        }

        /// <summary>
        /// Returns the 64-bit bitmask of entities that have this component in the <b>enabled</b>
        /// state within the specified segment and block. Each set bit corresponds to an entity
        /// that has the component, and it is enabled.
        /// <para>
        /// Used for low-level iteration, custom query implementations, and debug visualization.
        /// A segment contains 256 entities divided into 4 blocks of 64.
        /// </para>
        /// </summary>
        /// <param name="segmentIdx">Segment index (entityId >> ENTITIES_IN_SEGMENT_SHIFT).</param>
        /// <param name="blockIdx">Block index within the segment (0..3).</param>
        /// <returns>Bitmask where set bits indicate entities with this component enabled.</returns>
        [MethodImpl(AggressiveInlining)]
        public ulong EnabledMask(uint segmentIdx, int blockIdx) {
            unsafe { return _enabledMask(segmentIdx, blockIdx); }
        }

        /// <summary>
        /// Returns the 64-bit bitmask of entities that have this component in the <b>disabled</b>
        /// state within the specified segment and block. Disabled components exist on the entity
        /// but are excluded from standard queries.
        /// </summary>
        /// <param name="segmentIdx">Segment index.</param>
        /// <param name="blockIdx">Block index within the segment (0..3).</param>
        /// <returns>Bitmask where set bits indicate entities with this component disabled.</returns>
        [MethodImpl(AggressiveInlining)]
        public ulong DisabledMask(uint segmentIdx, int blockIdx) {
            unsafe { return _disabledMask(segmentIdx, blockIdx); }
        }

        /// <summary>
        /// Returns the 64-bit bitmask of entities that have this component in <b>any</b> state
        /// (enabled or disabled) within the specified segment and block.
        /// Equivalent to <c>EnabledMask | DisabledMask</c>.
        /// </summary>
        /// <param name="segmentIdx">Segment index.</param>
        /// <param name="blockIdx">Block index within the segment (0..3).</param>
        /// <returns>Bitmask where set bits indicate entities that have this component (any state).</returns>
        [MethodImpl(AggressiveInlining)]
        public ulong AnyMask(uint segmentIdx, int blockIdx) {
            unsafe { return _anyMask(segmentIdx, blockIdx); }
        }

        /// <summary>
        /// Batch-deletes this component from all entities indicated by the set bits of
        /// <paramref name="entitiesMaskFilter"/> within the specified segment and block.
        /// Triggers OnDelete hooks for each affected entity.
        /// <para>
        /// Used for bulk cleanup operations, e.g. removing a component from all entities
        /// in a segment block as part of chunk unloading or mass entity destruction.
        /// </para>
        /// </summary>
        /// <param name="entitiesMaskFilter">64-bit mask selecting which entities in the block to process.</param>
        /// <param name="segmentIdx">Segment index.</param>
        /// <param name="segmentBlockIdx">Block index within the segment (0..3).</param>
        /// <param name="reason">The reason for deletion, forwarded to OnDelete hooks.</param>
        [MethodImpl(AggressiveInlining)]
        public void BatchDelete(ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx, HookReason reason = HookReason.Default) {
            unsafe { _batchDelete(entitiesMaskFilter, segmentIdx, segmentBlockIdx, reason); }
        }

        /// <summary>
        /// Checks whether the specified segment has been allocated for this component type.
        /// A segment is allocated when at least one entity in its range has (or had) this component.
        /// Unallocated segments have no memory footprint for this component.
        /// </summary>
        /// <param name="segmentIdx">Segment index.</param>
        /// <returns><c>true</c> if the segment is allocated; <c>false</c> if null (no memory).</returns>
        [MethodImpl(AggressiveInlining)]
        public bool IsSegmentAllocated(uint segmentIdx) {
            unsafe { return _isSegmentAllocated(segmentIdx); }
        }

        /// <summary>
        /// Calculates the current storage capacity (number of allocated component slots)
        /// for this component type. Grows automatically as chunks are registered.
        /// </summary>
        /// <returns>Total allocated slot count.</returns>
        [MethodImpl(AggressiveInlining)]
        public int CalculateCapacity() {
            unsafe { return _calculateCapacity(); }
        }

        /// <summary>
        /// Calculates the number of entities that currently have this component (enabled or disabled).
        /// Scans the bitmaps — not a cached counter.
        /// </summary>
        /// <returns>Number of entities with this component.</returns>
        [MethodImpl(AggressiveInlining)]
        public uint CalculateCount() {
            unsafe { return _calculateCount(); }
        }
        #endregion

        #region ENTITY
        /// <summary>
        /// Appends a string representation of this component's value on the given entity
        /// to the <see cref="StringBuilder"/>. Does nothing if the entity doesn't have this component.
        /// Used by debug visualizers and entity inspector tools to display component data.
        /// </summary>
        /// <param name="builder">StringBuilder to append the component description to.</param>
        /// <param name="entityId">Raw entity ID (with ENTITY_ID_OFFSET applied).</param>
        [MethodImpl(AggressiveInlining)]
        public void TryToStringComponent(StringBuilder builder, uint entityId) {
            unsafe { _tryToStringComponent(builder, entityId); }
        }

        /// <summary>
        /// Attempts to retrieve the component/tag value for an entity as a boxed <see cref="IComponent"/>.
        /// Returns <c>false</c> if the entity does not have this component.
        /// <para>
        /// The returned value is a boxed copy — modifications to it do not affect the stored component.
        /// Use <see cref="SetRaw"/> to write back modified values.
        /// </para>
        /// </summary>
        /// <param name="entityId">Raw entity ID.</param>
        /// <param name="value">The boxed component value (valid only when returning <c>true</c>).</param>
        /// <returns><c>true</c> if the entity has this component.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool TryGetRaw(uint entityId, out IComponentOrTag value) {
            unsafe { return _tryGetRaw(entityId, out value); }
        }

        /// <summary>
        /// Sets the component/tag value on an entity from a boxed <see cref="IComponent"/>.
        /// The entity must already have this component (call <see cref="Add(uint)"/> first if needed).
        /// The boxed value is unboxed and copied into the component storage.
        /// </summary>
        /// <param name="entityId">Raw entity ID.</param>
        /// <param name="value">The boxed component value to write. Must match the component type.</param>
        [MethodImpl(AggressiveInlining)]
        public void SetRaw(uint entityId, IComponentOrTag value) {
            unsafe { _setRaw(entityId, value); }
        }

        [MethodImpl(AggressiveInlining)]
        internal void SetRawDirect(uint entityId, IComponentOrTag value) {
            unsafe { _setRawDirect(entityId, value); }
        }

        [MethodImpl(AggressiveInlining)]
        internal ref T Ref<T>(uint entityId) where T : struct, IComponent {
            unsafe {
                var segments = (T[][])_componentsSegments();
                return ref segments[entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT][entityId & Const.ENTITIES_IN_SEGMENT_MASK];
            }
        }

        /// <summary>
        /// Adds this component (default-initialized) to an entity. Reports whether the component
        /// was actually added via the <paramref name="added"/> out parameter — returns <c>false</c>
        /// if the entity already had this component (no-op in that case).
        /// Triggers the OnAdd hook if the component was newly added.
        /// </summary>
        /// <param name="entityId">Raw entity ID.</param>
        /// <param name="added"><c>true</c> if the component was newly added; <c>false</c> if it already existed.</param>
        [MethodImpl(AggressiveInlining)]
        public void Add(uint entityId, out bool added) {
            unsafe { added = _add(entityId); }
        }

        /// <summary>
        /// Adds this component (default-initialized) to an entity.
        /// If the entity already has the component, this is a no-op.
        /// Triggers the OnAdd hook if newly added.
        /// </summary>
        /// <param name="entityId">Raw entity ID.</param>
        [MethodImpl(AggressiveInlining)]
        public void Add(uint entityId) {
            unsafe { _add(entityId); }
        }

        /// <summary>
        /// Checks whether an entity has this component in any state (enabled or disabled).
        /// </summary>
        /// <param name="entityId">Raw entity ID.</param>
        /// <returns><c>true</c> if the entity has this component.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool Has(uint entityId) {
            unsafe { return _has(entityId); }
        }

        /// <summary>
        /// Checks whether an entity has this component in the <b>enabled</b> state.
        /// Enabled components participate in standard queries.
        /// </summary>
        /// <param name="entityId">Raw entity ID.</param>
        /// <returns><c>true</c> if the entity has this component and it is enabled.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool HasEnabled(uint entityId) {
            unsafe { return _hasEnabled(entityId); }
        }

        /// <summary>
        /// Checks whether an entity has this component in the <b>disabled</b> state.
        /// Disabled components are excluded from standard queries but retain their data.
        /// </summary>
        /// <param name="entityId">Raw entity ID.</param>
        /// <returns><c>true</c> if the entity has this component and it is disabled.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool HasDisabled(uint entityId) {
            unsafe { return _hasDisabled(entityId); }
        }

        /// <summary>
        /// Enables a previously disabled component on an entity. The component data is preserved.
        /// After enabling, the entity will appear in standard queries filtering for this component.
        /// </summary>
        /// <param name="entityId">Raw entity ID.</param>
        /// <returns>A <see cref="ToggleResult"/> indicating what happened.</returns>
        [MethodImpl(AggressiveInlining)]
        public ToggleResult Enable(uint entityId) {
            unsafe { return _enable(entityId); }
        }

        /// <summary>
        /// Disables an enabled component on an entity without removing it. The component data
        /// is preserved but the entity will no longer appear in standard queries filtering for
        /// this component. Useful for temporarily "hiding" a component (e.g. pausing a behavior)
        /// without the cost of removing and re-adding.
        /// </summary>
        /// <param name="entityId">Raw entity ID.</param>
        /// <returns>A <see cref="ToggleResult"/> indicating what happened.</returns>
        [MethodImpl(AggressiveInlining)]
        public ToggleResult Disable(uint entityId) {
            unsafe { return _disable(entityId); }
        }

        /// <summary>
        /// Removes this component from an entity entirely (both data and bitmap presence).
        /// Triggers the OnDelete hook if the component existed.
        /// </summary>
        /// <param name="entityId">Raw entity ID.</param>
        /// <param name="reason">The reason for deletion, forwarded to the OnDelete hook.</param>
        /// <returns><c>true</c> if the component was present and removed; <c>false</c> if the entity didn't have it.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool Delete(uint entityId, HookReason reason = HookReason.Default) {
            unsafe { return _delete(entityId, reason); }
        }

        /// <summary>
        /// Copies this component's value from one entity to another. If the destination entity
        /// doesn't have the component, it is added first. The source entity retains its component.
        /// Triggers the component's CopyTo hook if defined.
        /// </summary>
        /// <param name="srcEntityId">Source entity's raw ID.</param>
        /// <param name="dstEntityId">Destination entity's raw ID.</param>
        /// <returns><c>true</c> if the source had the component and it was copied.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool Copy(uint srcEntityId, uint dstEntityId) {
            unsafe { return _copy(srcEntityId, dstEntityId); }
        }

        /// <summary>
        /// Moves this component from one entity to another: copies the value to the destination
        /// and removes it from the source. If the destination already has the component, its value
        /// is overwritten. Triggers OnDelete on the source and OnAdd (or CopyTo) on the destination.
        /// </summary>
        /// <param name="srcEntityId">Source entity's raw ID (component will be removed).</param>
        /// <param name="dstEntityId">Destination entity's raw ID (component will be added/overwritten).</param>
        /// <returns><c>true</c> if the source had the component and it was moved.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool Move(uint srcEntityId, uint dstEntityId) {
            unsafe { return _move(srcEntityId, dstEntityId); }
        }

        /// <summary>
        /// Sets a default components/tag on an entity. Tags are binary — either present or absent.
        /// </summary>
        /// <param name="entityId">Raw entity ID.</param>
        [MethodImpl(AggressiveInlining)]
        public void Set(uint entityId) {
            unsafe { _set(entityId); }
        }

        /// <summary>
        /// Returns a boxed default instance of the component/tag type.
        /// this always returns a default-constructed instance. Useful for reflection-based tools
        /// that need a prototype value.
        /// </summary>
        /// <returns>A boxed default instance of the component or tag.</returns>
        [MethodImpl(AggressiveInlining)]
        public IComponentOrTag DefaultValue() {
            unsafe { return _defaultValue(); }
        }
        #endregion
    }

    /// <summary>
    /// Type-erased handle for accessing a specific event type's storage without knowing the concrete
    /// event type at compile time. Provides runtime-dynamic operations: send events (boxed),
    /// read events by index, delete, query pool state, and manage the event buffer.
    /// <para>
    /// Events in StaticEcs use a ring-buffer pool model. Each event gets an index in the pool.
    /// Receivers track their own read cursor. Events can be individually deleted (soft-deleted)
    /// and the pool tracks deletion state per slot.
    /// </para>
    /// <para>
    /// Obtained via <see cref="WorldHandle.TryGetEventsHandle"/> or
    /// <see cref="WorldHandle.GetAllEventsHandles"/>.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct EventsHandle {
        private readonly unsafe delegate*<IEvent, bool> _addRaw;
        private readonly unsafe delegate*<bool> _add;
        private readonly unsafe delegate*<int, IEvent> _getRaw;
        private readonly unsafe delegate*<int, void> _delete;
        private readonly unsafe delegate*<void> _destroy;
        private readonly unsafe delegate*<int, bool> _isDeleted;
        private readonly unsafe delegate*<int, int> _unreadCount;
        private readonly unsafe delegate*<int> _notDeletedCount;
        private readonly unsafe delegate*<int> _capacity;
        private readonly unsafe delegate*<int> _receiversCount;
        private readonly unsafe delegate*<int> _last;
        private readonly unsafe delegate*<int, ushort> _version;
        private readonly unsafe delegate*<int, IEvent, void> _putRaw;
        private readonly unsafe delegate*<void> _reset;
        private readonly unsafe delegate*<ref BinaryPackWriter, void> _writeAll;
        private readonly unsafe delegate*<ref BinaryPackReader, void> _readAll;

        /// <summary>
        /// The <see cref="Type"/> of the event struct this handle operates on.
        /// </summary>
        public readonly Type EventType;

        /// <summary>
        /// Stable GUID assigned to this event type for serialization identification.
        /// </summary>
        public readonly Guid Guid;

        /// <summary>
        /// Runtime-assigned numeric ID for this event type within the world.
        /// </summary>
        public readonly ushort DynamicId;

        internal static unsafe EventsHandle Create<TWorld, TEvent>()
            where TWorld : struct, IWorldType
            where TEvent : struct, IEvent {
            return new EventsHandle(
                typeof(TEvent),
                World<TWorld>.Events<TEvent>.Instance.Guid,
                World<TWorld>.Events<TEvent>.Instance.Id,
                &World<TWorld>.Events<TEvent>._AddRaw,
                &World<TWorld>.Events<TEvent>._Add,
                &World<TWorld>.Events<TEvent>._GetRaw,
                &World<TWorld>.Events<TEvent>._Del,
                &World<TWorld>.Events<TEvent>._Destroy,
                &World<TWorld>.Events<TEvent>._IsDeleted,
                &World<TWorld>.Events<TEvent>._UnreadCount,
                &World<TWorld>.Events<TEvent>._NotDeletedCount,
                &World<TWorld>.Events<TEvent>._Capacity,
                &World<TWorld>.Events<TEvent>._ReceiversCount,
                &World<TWorld>.Events<TEvent>._Last,
                &World<TWorld>.Events<TEvent>._Version,
                &World<TWorld>.Events<TEvent>._PutRaw,
                &World<TWorld>.Events<TEvent>._Reset,
                &World<TWorld>.Events<TEvent>._WriteAll,
                &World<TWorld>.Events<TEvent>._ReadAll
            );
        }

        internal unsafe EventsHandle(
            Type eventType,
            Guid guid,
            ushort dynamicId,
            delegate*<IEvent, bool> addRaw,
            delegate*<bool> add,
            delegate*<int, IEvent> getRaw,
            delegate*<int, void> delete,
            delegate*<void> destroy,
            delegate*<int, bool> isDeleted,
            delegate*<int, int> unreadCount,
            delegate*<int> notDeletedCount,
            delegate*<int> capacity,
            delegate*<int> receiversCount,
            delegate*<int> last,
            delegate*<int, ushort> version,
            delegate*<int, IEvent, void> putRaw,
            delegate*<void> reset,
            delegate*<ref BinaryPackWriter, void> writeAll,
            delegate*<ref BinaryPackReader, void> readAll) {
            EventType = eventType;
            Guid = guid;
            DynamicId = dynamicId;
            _addRaw = addRaw;
            _add = add;
            _getRaw = getRaw;
            _delete = delete;
            _destroy = destroy;
            _isDeleted = isDeleted;
            _unreadCount = unreadCount;
            _notDeletedCount = notDeletedCount;
            _capacity = capacity;
            _receiversCount = receiversCount;
            _last = last;
            _version = version;
            _putRaw = putRaw;
            _reset = reset;
            _writeAll = writeAll;
            _readAll = readAll;
        }

        #region POOL_OPERATIONS
        /// <summary>
        /// Sends an event with the given boxed value to all receivers.
        /// The value is unboxed internally and added to the event pool.
        /// </summary>
        /// <param name="value">Boxed event value. Must match the event type.</param>
        /// <returns><c>true</c> if the event was added to the pool; <c>false</c> if the pool is full.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool AddRaw(IEvent value) {
            unsafe { return _addRaw(value); }
        }

        /// <summary>
        /// Sends a default-initialized event to all receivers.
        /// Useful for signal-style events that carry no data.
        /// </summary>
        /// <returns><c>true</c> if the event was added; <c>false</c> if the pool is full.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool Add() {
            unsafe { return _add(); }
        }

        /// <summary>
        /// Retrieves the event at the given pool index as a boxed <see cref="IEvent"/>.
        /// The index must be within the valid range of the pool.
        /// </summary>
        /// <param name="idx">Pool index of the event.</param>
        /// <returns>The boxed event value.</returns>
        [MethodImpl(AggressiveInlining)]
        public IEvent GetRaw(int idx) {
            unsafe { return _getRaw(idx); }
        }

        /// <summary>
        /// Soft-deletes the event at the given pool index. The slot is marked as deleted but
        /// the pool index remains occupied until the pool wraps around. Deleted events are
        /// skipped by receivers.
        /// </summary>
        /// <param name="idx">Pool index of the event to delete.</param>
        [MethodImpl(AggressiveInlining)]
        public void Delete(int idx) {
            unsafe { _delete(idx); }
        }

        [MethodImpl(AggressiveInlining)]
        internal void Destroy() {
            unsafe { _destroy(); }
        }

        /// <summary>
        /// Checks whether the event at the given pool index has been soft-deleted.
        /// </summary>
        /// <param name="idx">Pool index to check.</param>
        /// <returns><c>true</c> if the event at this index is deleted.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool IsDeleted(int idx) {
            unsafe { return _isDeleted(idx); }
        }

        /// <summary>
        /// Returns the number of unread events for a specific receiver (identified by receiver index).
        /// Each receiver maintains its own read cursor.
        /// </summary>
        /// <param name="idx">Receiver index.</param>
        /// <returns>Number of events the receiver has not yet read.</returns>
        [MethodImpl(AggressiveInlining)]
        public int UnreadCount(int idx) {
            unsafe { return _unreadCount(idx); }
        }

        /// <summary>
        /// Returns the total number of events in the pool that have not been soft-deleted.
        /// </summary>
        /// <returns>Count of live (non-deleted) events.</returns>
        [MethodImpl(AggressiveInlining)]
        public int NotDeletedCount() {
            unsafe { return _notDeletedCount(); }
        }

        /// <summary>
        /// Returns the current capacity of the event pool (maximum number of events
        /// it can hold before old events are overwritten).
        /// </summary>
        /// <returns>Pool capacity.</returns>
        [MethodImpl(AggressiveInlining)]
        public int Capacity() {
            unsafe { return _capacity(); }
        }

        /// <summary>
        /// Returns the number of currently registered receivers for this event type.
        /// </summary>
        /// <returns>Receiver count.</returns>
        [MethodImpl(AggressiveInlining)]
        public int ReceiversCount() {
            unsafe { return _receiversCount(); }
        }

        /// <summary>
        /// Returns the pool index of the most recently added event.
        /// Returns -1 or an invalid value if no events have been added.
        /// </summary>
        /// <returns>Index of the last added event.</returns>
        [MethodImpl(AggressiveInlining)]
        public int Last() {
            unsafe { return _last(); }
        }

        /// <summary>
        /// Returns the version counter for the event slot at the given index.
        /// The version increments each time the slot is reused (overwritten by a new event).
        /// Used internally to detect stale event references.
        /// </summary>
        /// <param name="idx">Pool index.</param>
        /// <returns>Version counter for this slot.</returns>
        [MethodImpl(AggressiveInlining)]
        public ushort Version(int idx) {
            unsafe { return _version(idx); }
        }

        /// <summary>
        /// Directly writes a boxed event value into a specific pool slot, overwriting whatever
        /// was there. This is a low-level operation — prefer <see cref="AddRaw"/> for normal
        /// event sending. Used by serialization and migration tools.
        /// </summary>
        /// <param name="idx">Pool index to write to.</param>
        /// <param name="value">Boxed event value.</param>
        [MethodImpl(AggressiveInlining)]
        public void PutRaw(int idx, IEvent value) {
            unsafe { _putRaw(idx, value); }
        }

        /// <summary>
        /// Clears all events from the pool and resets all receiver cursors.
        /// All unread events are lost.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public void Reset() {
            unsafe { _reset(); }
        }

        [MethodImpl(AggressiveInlining)]
        internal void WriteAll(ref BinaryPackWriter writer) {
            unsafe { _writeAll(ref writer); }
        }

        [MethodImpl(AggressiveInlining)]
        internal void ReadAll(ref BinaryPackReader reader) {
            unsafe { _readAll(ref reader); }
        }
        #endregion
    }
}