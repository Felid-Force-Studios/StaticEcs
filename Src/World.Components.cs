#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    /// <summary>
    /// Marker interface for zero-size tag components. Tags carry no data — they serve as boolean flags
    /// on entities, tracked purely through presence bitmasks.
    /// </summary>
    public interface ITag : IComponentOrTag { }

    /// <summary>
    /// Marker interface for ECS data components. Components carry data (as opposed to <see cref="ITag"/> which is zero-size).
    /// Inherits hooks from <see cref="IComponentOrTag"/>.
    /// </summary>
    public interface IComponent : IComponentOrTag { }

    /// <summary>
    /// Base interface for all ECS types that can be attached to entities (components and tags).
    /// All methods have default (empty) implementations — override only the hooks you need.
    /// Hook detection uses reflection at static initialization time; the JIT eliminates branches
    /// for hooks that are not overridden, so there is zero runtime cost for unused hooks.
    /// </summary>
    public interface IComponentOrTag {
        /// <summary>
        /// Called immediately after this component is added to an entity or after the value is overwritten
        /// (in the <c>Set(entity, value)</c> overload). Use this hook to initialize dependent state,
        /// subscribe to events, or set up cross-component relationships.
        /// <para>
        /// For the value-overwrite case (<c>Set(entity, value)</c> when component already exists),
        /// <see cref="OnDelete{TWorld}"/> is called on the old value first, then the value is replaced,
        /// then this hook is called on the new value.
        /// </para>
        /// </summary>
        /// <typeparam name="TWorld">World type for static context access.</typeparam>
        /// <param name="self">The entity this component was added to.</param>
        public void OnAdd<TWorld>(World<TWorld>.Entity self) where TWorld : struct, IWorldType {}

        /// <summary>
        /// Called when this component is about to be removed from an entity via <c>Delete</c>,
        /// or before being overwritten via <c>Set(entity, value)</c>. Use this hook to clean up
        /// resources, unsubscribe from events, or break cross-component relationships.
        /// <para>
        /// Also called during entity destruction for each component the entity has.
        /// </para>
        /// </summary>
        /// <typeparam name="TWorld">World type for static context access.</typeparam>
        /// <param name="self">The entity this component is being removed from.</param>
        public void OnDelete<TWorld>(World<TWorld>.Entity self, HookReason reason) where TWorld : struct, IWorldType {}

        /// <summary>
        /// Custom copy logic invoked by <see cref="World{TWorld}.Components{T}.Copy"/>.
        /// When implemented, this replaces the default behavior (which is a bitwise copy + Add on destination).
        /// Use this for components that require deep copy, reference counting, or conditional copy logic.
        /// </summary>
        /// <typeparam name="TWorld">World type for static context access.</typeparam>
        /// <param name="self">The source entity being copied from.</param>
        /// <param name="other">The destination entity being copied to.</param>
        /// <param name="disabled">Whether the component is currently disabled on the source entity.</param>
        public void CopyTo<TWorld>(World<TWorld>.Entity self, World<TWorld>.Entity other, bool disabled) where TWorld : struct, IWorldType {}

        /// <summary>
        /// Custom serialization hook for writing this component to a binary stream.
        /// <para>
        /// <b>Always required</b> for entity-level serialization (<c>EntitiesSnapshot</c> via
        /// <c>CreateEntitiesSnapshotWriter</c>), regardless of whether the type is unmanaged.
        /// </para>
        /// <para>
        /// For chunk-level serialization (<c>WorldSnapshot</c>, <c>ClusterSnapshot</c>, <c>ChunkSnapshot</c>),
        /// unmanaged types can use an <see cref="IPackArrayStrategy{T}"/> (e.g. <c>UnmanagedPackArrayStrategy&lt;T&gt;</c>)
        /// for bulk memory copying instead of per-component hooks. If the strategy reports
        /// <c>IsUnmanaged() == true</c>, this hook is not called during chunk writes.
        /// Non-unmanaged types always use this hook in all serialization paths.
        /// </para>
        /// </summary>
        /// <typeparam name="TWorld">World type for static context access.</typeparam>
        /// <param name="writer">Binary writer to serialize data into.</param>
        /// <param name="self">The entity this component belongs to.</param>
        public void Write<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity self) where TWorld : struct, IWorldType {}

        /// <summary>
        /// Custom deserialization hook for reading this component from a binary stream.
        /// <para>
        /// <b>Always required</b> for entity-level deserialization (<c>EntitiesSnapshot</c> via
        /// <c>LoadEntitiesSnapshot</c>), regardless of whether the type is unmanaged.
        /// </para>
        /// <para>
        /// For chunk-level deserialization, unmanaged types with a matching version use
        /// <see cref="IPackArrayStrategy{T}"/> for bulk reads. However, when the serialized version
        /// differs from the current <see cref="ComponentTypeConfig{T}.Version"/>, this hook is called
        /// even for unmanaged types to enable data migration.
        /// Non-unmanaged types always use this hook in all deserialization paths.
        /// </para>
        /// </summary>
        /// <typeparam name="TWorld">World type for static context access.</typeparam>
        /// <param name="reader">Binary reader to deserialize data from.</param>
        /// <param name="self">The entity this component is being loaded onto.</param>
        /// <param name="version">The version byte that was stored during serialization, for migration logic.</param>
        /// <param name="disabled">Whether the component was disabled when it was serialized.</param>
        public void Read<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity self, byte version, bool disabled) where TWorld : struct, IWorldType {}
    }

    internal interface IComponentInternal {
        internal void OnInitialize<TWorld>() where TWorld : struct, IWorldType;
    }

    internal interface IComponentHookOverride {
        internal bool HasOnAdd();
        internal bool HasOnDelete();
        internal bool HasCopyTo();
    }

    internal interface IComponentStrategyOverride {
        internal IPackArrayStrategy<T> ArrayPackStrategy<T>() where T : struct;
    }
    
    /// <summary>
    /// Result of a <see cref="World{TWorld}.Components{T}.Disable"/> or <see cref="World{TWorld}.Components{T}.Enable"/> operation.
    /// </summary>
    public enum ToggleResult : byte {
        /// <summary>The entity does not have this component.</summary>
        MissingComponent,
        /// <summary>The component was already in the target state (already disabled / already enabled).</summary>
        Unchanged,
        /// <summary>The component state was changed (enabled → disabled or disabled → enabled).</summary>
        Changed
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    /// <summary>
    /// Configuration for registering a component type with <c>World&lt;TWorld&gt;.Types().Component&lt;T&gt;()</c>.
    /// Controls serialization identity (Guid), data versioning, and binary read/write strategy.
    /// </summary>
    /// <typeparam name="T">The component type being configured.</typeparam>
    public readonly struct ComponentTypeConfig<T> where T : struct, IComponentOrTag {
        /// <summary>
        /// Stable identifier for this component type used during serialization.
        /// Serializer uses this Guid to match saved data to the correct component type
        /// even if the type is renamed or moved.
        /// Default is computed via <see cref="Utils.GuidFromAQN"/>.
        /// </summary>
        public readonly Guid? Guid;

        /// <summary>
        /// Schema version byte written alongside serialized component data.
        /// When the serialized version differs from the current version, the component's
        /// <see cref="IComponent.Read{TWorld}"/> hook is invoked for data migration instead of
        /// the default binary deserialization strategy.
        /// Increment this when changing the component's data layout to enable backward-compatible loading.
        /// </summary>
        public readonly byte? Version;

        /// <summary>
        /// When <c>false</c> (default), the framework manages component data lifecycle:
        /// new segments are pre-initialized with <see cref="DefaultValue"/>, and data is reset
        /// to <see cref="DefaultValue"/> upon deletion (if no <see cref="IComponent.OnDelete{TWorld}"/> hook).
        /// When <c>true</c>, the framework does not initialize or clear component data — the user
        /// is responsible for all data management. Useful for high-frequency unmanaged types.
        /// When <see cref="IComponent.OnDelete{TWorld}"/> is defined, the hook handles cleanup
        /// regardless of this flag.
        /// </summary>
        public readonly bool? NoDataLifecycle;

        /// <summary>
        /// Strategy for binary serialization of component arrays. Defaults to <see cref="StructPackArrayStrategy{T}"/>
        /// which performs direct memory copies for unmanaged types. Override with a custom strategy
        /// for types requiring specialized serialization.
        /// </summary>
        public readonly IPackArrayStrategy<T> ReadWriteStrategy;

        /// <summary>
        /// Default value for component data lifecycle. Used in two ways:
        /// (1) New storage segments are pre-initialized with this value, so <c>Add&lt;T&gt;()</c> returns it.
        /// (2) On deletion, data is reset to this value (if no <see cref="IComponent.OnDelete{TWorld}"/> hook).
        /// Only effective when <see cref="NoDataLifecycle"/> is <c>false</c> (default).
        /// </summary>
        public readonly T? DefaultValue;

        /// <summary>
        /// When <c>true</c>, tracks component additions — enables use of <c>Added&lt;T&gt;</c> query filter.
        /// Requires calling <see cref="World{TWorld}.ClearTracking"/> to reset tracking state.
        /// </summary>
        public readonly bool? TrackAdded;

        /// <summary>
        /// When <c>true</c>, tracks component deletions — enables use of <c>Deleted&lt;T&gt;</c> query filter.
        /// Requires calling <see cref="World{TWorld}.ClearTracking"/> to reset tracking state.
        /// </summary>
        public readonly bool? TrackDeleted;

        #if !FFS_ECS_DISABLE_CHANGED_TRACKING
        /// <summary>
        /// When <c>true</c>, tracks component changes — enables use of <c>Changed&lt;T&gt;</c> query filter.
        /// A component is marked as changed when mutable access (<c>ref</c>) is obtained via
        /// <c>Mut&lt;T&gt;()</c>, <c>Add&lt;T&gt;()</c>, or query iteration with writable semantics.
        /// Requires calling <see cref="World{TWorld}.ClearTracking"/> to reset tracking state.
        /// </summary>
        public readonly bool? TrackChanged;
        #endif

        /// <summary>
        /// Creates a configuration for component type registration.
        /// </summary>
        /// <param name="guid">Stable serialization identifier. Default is computed via <see cref="Utils.GuidFromAQN"/>.</param>
        /// <param name="version">Schema version for data migration. Default is 0.</param>
        /// <param name="noDataLifecycle">When <c>true</c>, disables framework data lifecycle (no init, no clear). Default is <c>false</c>.</param>
        /// <param name="readWriteStrategy">Custom binary serialization strategy. Default is <see cref="StructPackArrayStrategy{T}"/>.</param>
        /// <param name="defaultValue">Default value for initialization and deletion reset. Default is <c>default(T)</c>.</param>
        /// <param name="trackAdded">When <c>true</c>, enables tracking of component additions for <c>Added&lt;T&gt;</c> query filter.</param>
        /// <param name="trackDeleted">When <c>true</c>, enables tracking of component deletions for <c>Deleted&lt;T&gt;</c> query filter.</param>
        /// <param name="trackChanged">When <c>true</c>, enables tracking of component changes for <c>Changed&lt;T&gt;</c> query filter.</param>
        public ComponentTypeConfig(Guid? guid = null,
                                   byte? version = null,
                                   bool? noDataLifecycle = null,
                                   IPackArrayStrategy<T> readWriteStrategy = null,
                                   T? defaultValue = null,
                                   bool? trackAdded = null,
                                   bool? trackDeleted = null,
                                   bool? trackChanged = null) {
            Guid = guid;
            Version = version;
            NoDataLifecycle = noDataLifecycle;
            ReadWriteStrategy = readWriteStrategy;
            DefaultValue = defaultValue;
            TrackAdded = trackAdded;
            TrackDeleted = trackDeleted;
            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            TrackChanged = trackChanged;
            #endif
        }

        internal ComponentTypeConfig<T> MergeWith(ComponentTypeConfig<T> other) {
            bool? trackChanged = null;
            bool? otherTrackChanged = null;
            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            trackChanged = TrackChanged;
            otherTrackChanged = other.TrackChanged;
            #endif

            return new ComponentTypeConfig<T>(
                guid: Guid ?? other.Guid,
                version: Version ?? other.Version,
                noDataLifecycle: NoDataLifecycle ?? other.NoDataLifecycle,
                readWriteStrategy: ReadWriteStrategy ?? other.ReadWriteStrategy,
                defaultValue: DefaultValue ?? other.DefaultValue,
                trackAdded: TrackAdded ?? other.TrackAdded,
                trackDeleted: TrackDeleted ?? other.TrackDeleted,
                trackChanged: trackChanged ?? otherTrackChanged
            );
        }

        internal static readonly ComponentTypeConfig<T> DefaultComponent = new(
            guid: typeof(T).GuidFromAQN(),
            version: 0,
            noDataLifecycle: false,
            readWriteStrategy: AutoRegistration.TryCreateUnmanagedPackArrayStrategy<T>() ?? new StructPackArrayStrategy<T>(),
            defaultValue: null,
            trackAdded: false,
            trackDeleted: false,
            trackChanged: false
        );

        internal static readonly ComponentTypeConfig<T> DefaultTag = new(
            guid: typeof(T).GuidFromAQN(),
            version: 0,
            trackAdded: false,
            trackDeleted: false
        );
    }

    /// <summary>
    /// Configuration for tag type registration, specifying serialization and tracking options.
    /// </summary>
    // ReSharper disable once UnusedTypeParameter
    public readonly struct TagTypeConfig<T> where T : struct, IComponentOrTag {
        /// <inheritdoc cref="ComponentTypeConfig{T}.Guid"/>>
        public readonly Guid? Guid;

        /// <inheritdoc cref="ComponentTypeConfig{T}.TrackAdded"/>>
        public readonly bool? TrackAdded;

        /// <inheritdoc cref="ComponentTypeConfig{T}.TrackChanged"/>>
        public readonly bool? TrackDeleted;

        public TagTypeConfig(Guid? guid = null, bool? trackAdded = null, bool? trackDeleted = null) {
            Guid = guid;
            TrackAdded = trackAdded;
            TrackDeleted = trackDeleted;
        }

        public ComponentTypeConfig<T> AsComponentConfig => AsComponentConfigInternal(this);
        
        internal static ComponentTypeConfig<T> AsComponentConfigInternal(TagTypeConfig<T> config) {
            return new ComponentTypeConfig<T>(guid: config.Guid, trackAdded: config.TrackAdded, trackDeleted: config.TrackDeleted);
        }
    }

    public interface IComponentConfig<T> where T : struct, IComponentOrTag {
        ComponentTypeConfig<T> Config();
    }

    public interface ITagConfig<T> where T : struct, IComponentOrTag {
        TagTypeConfig<T> Config();
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public abstract partial class World<TWorld> {

        /// <summary>
        /// Static storage and operations for component type <typeparamref name="T"/> within this world.
        /// This struct holds all per-type data: component arrays (SoA layout), presence bitmasks,
        /// heuristic chunks for fast iteration, and query update lists.
        /// <para>
        /// Access via <c>Components&lt;T&gt;.Instance</c> for direct low-level operations,
        /// or use the higher-level <see cref="Entity"/> API (e.g., <c>entity.Add&lt;T&gt;()</c>).
        /// The Entity API delegates to these methods internally.
        /// </para>
        /// <para>
        /// Component data is stored in segments of 256 entries. Segments are allocated lazily on first access
        /// and returned to a pool when all entities in the segment lose this component.
        /// Presence is tracked via <c>ulong</c> bitmasks (64 entities per block, 4 blocks per segment),
        /// with a parallel disabled-mask for per-component enable/disable functionality.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The component type. Must be a struct implementing <see cref="IComponent"/>.</typeparam>
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        [Il2CppEagerStaticClassConstruction]
        #endif
        [StructLayout(LayoutKind.Sequential)]
        public struct Components<
            #if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
            #endif
            T> where T : struct, IComponentOrTag {
            /// <summary>
            /// Singleton instance of this component storage. All public methods operate on this instance.
            /// Initialized when <c>World&lt;TWorld&gt;.Types().Component&lt;T&gt;()</c> is called.
            /// </summary>
            public static Components<T> Instance;

            /// <summary>
            /// Type-erased handle for this component type, providing access via <c>unsafe delegate*</c>
            /// function pointers without generic type parameters. Used by serialization, tooling,
            /// and reflection-driven code. See <see cref="ComponentsHandle"/>.
            /// </summary>
            public static ComponentsHandle Handle;

            #if UNITY_2022_1_OR_NEWER
            [UnityEngine.Scripting.Preserve]
            #endif
            [MethodImpl(NoInlining)]
            internal static void AutoRegister(bool isTag) {
                if (Instance.IsRegistered) {
                    return;
                }
                ComponentTypeConfig<T> config = default;
                if (isTag) {
                    if (default(T) is ITagConfig<T> cfg) {
                        config = cfg.Config().AsComponentConfig;
                    }
                }
                else if (default(T) is IComponentConfig<T> cfg) {
                    config = cfg.Config();
                }

                
                Data.Instance.RegisterComponentOrTagTypeInternal(config, isTag, typeof(T).Name);
            }

            internal HeuristicChunk[] HeuristicChunks;
            internal T[][] ComponentSegments;
            internal ulong[][] EntitiesMaskSegments; // 4 ulong active entities + 4 ulong disabled entities

            private readonly QueryData[] _queriesToUpdateOnDelete;
            private readonly QueryData[] _queriesToUpdateOnAdd;
            private byte _queriesToUpdateOnDeleteCount;
            private byte _queriesToUpdateOnAddCount;

            /// <summary>
            /// Whether this storage represents a tag type (no data, no enable/disable, no change tracking).
            /// </summary>
            public readonly bool IsTag;
            
            internal readonly bool DataLifecycle;
            internal readonly bool HasOnAdd;
            internal readonly bool HasOnDelete;
            internal readonly bool TrackAdded;
            internal readonly bool TrackDeleted;
            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            internal readonly bool TrackChanged;
            #endif
            internal readonly bool TrackAddedOrChanged;
            internal readonly byte DeletedTrackingOffset;
            internal readonly T DefaultValue;
            internal ulong[][] TrackingMaskSegments;
            internal HeuristicComponentsTracking[] TrackingHeuristicChunks;
            internal HeuristicComponentsTracking[][] TrackingHistoryHeuristic;
            internal ulong[][][] TrackingHistoryMasks;
            
            private readonly QueryData[] _queriesToUpdateOnDisable;
            private readonly QueryData[] _queriesToUpdateOnEnable;
            #if FFS_ECS_BURST
            internal ComponentLifecycleHandle<T> LifecycleHandle;
            #endif
            internal ulong[][] _chunkHeuristicWorldMask;
            internal readonly ulong[] _segmentsMaskCache;
            internal T[][] _componentsPool;
            internal ulong[][] _segmentsPool;
            internal int _segmentsPoolCount;
            internal readonly ulong _idMask;
            internal readonly ulong _idMaskInv;
            internal readonly ushort _idDiv;
            internal ushort _chunkHeuristicWorldMaskLen;
            private byte _queriesToUpdateOnDisableCount;
            private byte _queriesToUpdateOnEnableCount;
            internal readonly bool HasDefaultValue;

            /// <summary>
            /// Runtime-assigned numeric identifier for this component type within the world.
            /// Assigned sequentially during registration (starting from 0). Used internally for
            /// bitmask indexing in the chunk heuristic world mask and by <see cref="ComponentsHandle.DynamicId"/>.
            /// </summary>
            public readonly ushort DynamicId;

            internal readonly bool HasCopyTo;
            internal readonly bool HasWrite;
            internal readonly bool HasRead;
            internal readonly bool Unmanaged;

            private readonly IPackArrayStrategy<T> _readWriteArrayStrategy;
            private readonly IPackArrayStrategyResettable _resettableStrategy;

            internal ulong[][] _trackingSegmentsPool;
            internal int _trackingSegmentsPoolCount;

            /// <summary>
            /// Stable serialization identifier for this component type, as specified in
            /// <see cref="ComponentTypeConfig{T}.Guid"/>. Used by the serializer to match
            /// saved data to the correct component type across renames/refactors.
            /// </summary>
            public readonly Guid Guid;

            /// <summary>
            /// Schema version of this component type, as specified in <see cref="ComponentTypeConfig{T}.Version"/>.
            /// Written during serialization and compared during deserialization to trigger data migration
            /// via <see cref="IComponent.Read{TWorld}"/> when versions differ.
            /// </summary>
            public readonly byte Version;

            /// <summary>
            /// Whether this component type has been registered with the world via
            /// <c>Types().Component&lt;T&gt;()</c>. In debug mode always check this before using <see cref="Instance"/>
            /// </summary>
            public readonly bool IsRegistered;

            internal static string TypeName;

            #if FFS_ECS_DEBUG
            internal static readonly string ComponentsTypeName = $"{WorldTypeName}.Components<{typeof(T).GenericName()}>"; 
            
            private int _blockerDelete;
            private int _blockerAdd;
            private int _blockerDisable;
            private int _blockerEnable;
            #endif

            #region PUBLIC API
            /// <summary>
            /// Checks whether the entity has component <typeparamref name="T"/> (either enabled or disabled).
            /// This is a fast bitmask check with no side effects. Returns <c>false</c> if the entity's
            /// segment has not been allocated for this component type.
            /// <para>
            /// <b>Note:</b> Returns <c>true</c> for both enabled AND disabled components.
            /// Use <see cref="HasEnabled"/> to check only enabled components,
            /// or <see cref="HasDisabled"/> to check only disabled components.
            /// </para>
            /// </summary>
            /// <param name="entity">Entity to check. Must not be destroyed or unloaded.</param>
            /// <returns><c>true</c> if the entity has this component (in any state, including disabled).</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly bool Has(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                #endif
                
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);

                var maskSegment = EntitiesMaskSegments[segmentIdx];
                return maskSegment != null && (maskSegment[segmentBlockIdx] & blockEntityMask) != 0;
            }

            /// <summary>
            /// Checks whether component <typeparamref name="T"/> was added to this entity since the system's last tick (or since `fromTick` if specified).
            /// Requires <c>TrackAdded</c> to be enabled (asserted in debug mode).
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAdded(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertComponentTrackAdded<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                #endif
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);
                var trackingSegments = TrackingMaskSegments;
                if (trackingSegments == null) return false;
                var maskSegment = trackingSegments[segmentIdx];
                return maskSegment != null && (maskSegment[segmentBlockIdx] & blockEntityMask) != 0;
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasAdded(Entity entity, ulong fromTick) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertComponentTrackAdded<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                #endif
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);
                ref var data = ref Data.Instance;
                return (AddedMaskHistory(fromTick, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx) & blockEntityMask) != 0;
            }

            /// <summary>
            /// Checks whether component <typeparamref name="T"/> was deleted from this entity since the system's last tick (or since `fromTick` if specified).
            /// Requires <c>TrackDeleted</c> to be enabled (asserted in debug mode).
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasDeleted(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertComponentTrackDeleted<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                #endif
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);
                var trackingSegments = TrackingMaskSegments;
                if (trackingSegments == null) return false;
                var maskSegment = trackingSegments[segmentIdx];
                return maskSegment != null && (maskSegment[segmentBlockIdx + DeletedTrackingOffset] & blockEntityMask) != 0;
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasDeleted(Entity entity, ulong fromTick) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertComponentTrackDeleted<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                #endif
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);
                ref var data = ref Data.Instance;
                return (DeletedMaskHistory(fromTick, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx) & blockEntityMask) != 0;
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            /// <summary>
            /// Checks whether component <typeparamref name="T"/> was changed on this entity since the system's last tick (or since `fromTick` if specified).
            /// Requires <c>TrackChanged</c> to be enabled (asserted in debug mode).
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasChanged(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertIsNotTag<T>(ComponentsTypeName);
                AssertComponentTrackChanged<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                #endif
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);
                var trackingSegments = TrackingMaskSegments;
                if (trackingSegments == null) return false;
                var maskSegment = trackingSegments[segmentIdx];
                return maskSegment != null && (maskSegment[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] & blockEntityMask) != 0;
            }

            [MethodImpl(AggressiveInlining)]
            public readonly bool HasChanged(Entity entity, ulong fromTick) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertIsNotTag<T>(ComponentsTypeName);
                AssertComponentTrackChanged<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                #endif
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);
                ref var data = ref Data.Instance;
                return (ChangedMaskHistory(fromTick, data.CurrentTick, data.TrackingBufferSize, segmentIdx, segmentBlockIdx) & blockEntityMask) != 0;
            }
            #endif

            /// <summary>
            /// Checks whether the entity has component <typeparamref name="T"/> in the disabled state.
            /// A disabled component has its data preserved but is excluded from queries that filter
            /// by enabled components. Returns <c>false</c> if the component is absent or enabled.
            /// </summary>
            /// <param name="entity">Entity to check. Must not be destroyed or unloaded.</param>
            /// <returns><c>true</c> if the entity has this component and it is currently disabled.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasDisabled(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertIsNotTag<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                #endif
                
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);

                var maskSegment = EntitiesMaskSegments[segmentIdx];
                return maskSegment != null && (maskSegment[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] & blockEntityMask) != 0;
            }

            /// <summary>
            /// Checks whether the entity has component <typeparamref name="T"/> in the enabled state.
            /// Returns <c>true</c> only if the component is present AND not disabled.
            /// This is the state that matches standard query filters (without explicit disabled-component filters).
            /// </summary>
            /// <param name="entity">Entity to check. Must not be destroyed or unloaded.</param>
            /// <returns><c>true</c> if the entity has this component and it is currently enabled.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly bool HasEnabled(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertIsNotTag<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                #endif
                
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);

                var maskSegment = EntitiesMaskSegments[segmentIdx];
                return maskSegment != null && (maskSegment[segmentBlockIdx] & ~maskSegment[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] & blockEntityMask) != 0;
            }

            /// <summary>
            /// Disables component <typeparamref name="T"/> on the entity without removing its data.
            /// A disabled component is excluded from standard query iteration but retains its value,
            /// allowing it to be re-enabled later with <see cref="Enable"/>. This is useful for
            /// temporarily deactivating behavior (e.g., disabling a "Poisoned" effect component).
            /// <para>
            /// Returns <see cref="ToggleResult.MissingComponent"/> if the entity does not have this component.
            /// Returns <see cref="ToggleResult.Unchanged"/> if the component is already disabled.
            /// Returns <see cref="ToggleResult.Changed"/> if the component was enabled and is now disabled.
            /// </para>
            /// </summary>
            /// <param name="entity">Target entity. Must not be destroyed or unloaded.</param>
            /// <returns>A <see cref="ToggleResult"/> indicating what happened.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly ToggleResult Disable(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertIsNotTag<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertNotBlockedByQuery(ComponentsTypeName, entity, _blockerDisable);
                AssertNotBlockedByParallelQuery(ComponentsTypeName, entity);
                #endif

                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);

                var maskSegment = EntitiesMaskSegments[segmentIdx];
                if (maskSegment == null || (maskSegment[segmentBlockIdx] & blockEntityMask) == 0) {
                    return ToggleResult.MissingComponent;
                }

                ref var disabledMask = ref maskSegment[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT];
                if ((disabledMask & blockEntityMask) != 0) {
                    return ToggleResult.Unchanged;
                }

                disabledMask |= blockEntityMask;
                for (uint i = 0; i < _queriesToUpdateOnDisableCount; i++) {
                    _queriesToUpdateOnDisable[i].Update(~blockEntityMask, segmentIdx, segmentBlockIdx);
                }
                return ToggleResult.Changed;
            }

            /// <summary>
            /// Re-enables a previously disabled component <typeparamref name="T"/> on the entity.
            /// After enabling, the component will again be visible to standard query iteration.
            /// <para>
            /// Returns <see cref="ToggleResult.MissingComponent"/> if the entity does not have this component.
            /// Returns <see cref="ToggleResult.Unchanged"/> if the component is already enabled.
            /// Returns <see cref="ToggleResult.Changed"/> if the component was disabled and is now enabled.
            /// </para>
            /// </summary>
            /// <param name="entity">Target entity. Must not be destroyed or unloaded.</param>
            /// <returns>A <see cref="ToggleResult"/> indicating what happened.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly ToggleResult Enable(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertIsNotTag<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertNotBlockedByQuery(ComponentsTypeName, entity, _blockerEnable);
                AssertNotBlockedByParallelQuery(ComponentsTypeName, entity);
                #endif

                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);

                var maskSegment = EntitiesMaskSegments[segmentIdx];
                if (maskSegment == null || (maskSegment[segmentBlockIdx] & blockEntityMask) == 0) {
                    return ToggleResult.MissingComponent;
                }

                ref var disabledMask = ref maskSegment[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT];
                if ((disabledMask & blockEntityMask) == 0) {
                    return ToggleResult.Unchanged;
                }

                disabledMask &= ~blockEntityMask;
                for (uint i = 0; i < _queriesToUpdateOnEnableCount; i++) {
                    _queriesToUpdateOnEnable[i].Update(~blockEntityMask, segmentIdx, segmentBlockIdx);
                }
                return ToggleResult.Changed;
            }

            /// <summary>
            /// Returns a mutable reference to component <typeparamref name="T"/> on the given entity.
            /// The entity MUST have this component (asserted in debug mode). Use <see cref="Has"/>
            /// to check first if unsure.
            /// <para>
            /// This is the fastest way to read/write component data — no presence check, no hooks,
            /// no tracking, just a direct array access via segment indexing.
            /// Does NOT mark the component as Changed. Use <see cref="Mut"/> for tracked mutable access.
            /// </para>
            /// </summary>
            /// <param name="entity">Entity that must have component <typeparamref name="T"/>.</param>
            /// <returns>A mutable reference to the stored component data.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly ref T Ref(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertIsNotTag<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertEntityHasComponent<T>(ComponentsTypeName, entity);
                #endif

                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                return ref ComponentSegments[entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT][entityId & Const.ENTITIES_IN_SEGMENT_MASK];
            }

            /// <summary>
            /// Returns a read-only reference to component <typeparamref name="T"/> on the given entity.
            /// Does NOT mark the component as changed. Use when you need to read component data
            /// without triggering change tracking.
            /// </summary>
            /// <param name="entity">Entity that must have component <typeparamref name="T"/>.</param>
            /// <returns>A read-only reference to the stored component data.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly ref readonly T Read(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertIsNotTag<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertEntityHasComponent<T>(ComponentsTypeName, entity);
                #endif

                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                return ref ComponentSegments[entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT][entityId & Const.ENTITIES_IN_SEGMENT_MASK];
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            /// <summary>
            /// Returns a mutable reference to component <typeparamref name="T"/> on the given entity
            /// and marks it as Changed if change tracking is enabled for this component type.
            /// Use when you need tracked mutable access for <c>AllChanged&lt;T&gt;</c> queries.
            /// </summary>
            /// <param name="entity">Entity that must have component <typeparamref name="T"/>.</param>
            /// <returns>A mutable reference to the stored component data.</returns>
            [MethodImpl(AggressiveInlining)]
            public ref T Mut(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertIsNotTag<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertEntityHasComponent<T>(ComponentsTypeName, entity);
                #endif

                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                if (TrackChanged) {
                    var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                    var segmentBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                    var blockEntityMask = 1UL << (byte)(entityId & Const.ENTITIES_IN_BLOCK_MASK);
                    ref var seg = ref TrackingMaskSegments[segmentIdx];
                    seg ??= AllocateTrackingSegment(segmentIdx);
                    seg[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] |= blockEntityMask;
                    var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                    TrackingHeuristicChunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT].ChangedBlocks.SetBit(chunkBlockIdx);
                }
                return ref ComponentSegments[entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT][entityId & Const.ENTITIES_IN_SEGMENT_MASK];
            }
            #endif

            /// <summary>
            /// Ensures entity has component <typeparamref name="T"/> and returns a mutable reference to it.
            /// If the component already exists, returns the existing value without modification and without
            /// triggering any hooks. If the component is new, default-initializes it, updates query caches,
            /// and triggers the <see cref="IComponent.OnAdd{TWorld}"/> hook.
            /// <para>
            /// This is the idiomatic "get-or-add" operation. Use when you want to ensure a component
            /// exists and then modify it via the returned reference:
            /// <c>ref var pos = ref Components&lt;Position&gt;.Instance.Add(entity); pos.X = 100;</c>
            /// </para>
            /// </summary>
            /// <param name="entity">Target entity. Must not be destroyed or unloaded.</param>
            /// <returns>A mutable reference to the component data (existing or newly created).</returns>
            [MethodImpl(AggressiveInlining)]
            public ref T Add(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertIsNotTag<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertNotBlockedByQuery(ComponentsTypeName, entity, _blockerAdd);
                AssertNotBlockedByParallelQuery(ComponentsTypeName, entity);
                #endif

                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentEntityIdx = (byte) (entityId & Const.ENTITIES_IN_SEGMENT_MASK);
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);

                ref var maskSegment = ref EntitiesMaskSegments[segmentIdx];
                ref var componentSegment = ref ComponentSegments[segmentIdx];
                if (maskSegment == null) {
                    (maskSegment, componentSegment) = CreateNewSegment(segmentIdx);
                }

                ref var entitiesMask = ref maskSegment[segmentBlockIdx];

                if ((entitiesMask & blockEntityMask) != 0) {
                    #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                    if (TrackChanged) {
                        SetChangedBit(entityId, segmentIdx, segmentBlockIdx, blockEntityMask);
                    }
                    #endif
                    return ref componentSegment[segmentEntityIdx];
                }

                if (entitiesMask == 0) {
                    var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                    HeuristicChunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT].NotEmptyBlocks.SetBit(chunkBlockIdx);
                }
                entitiesMask |= blockEntityMask;
                if (entitiesMask == ulong.MaxValue) {
                    var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                    HeuristicChunks[segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT].FullBlocks.SetBit(chunkBlockIdx);
                }

                for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                    _queriesToUpdateOnAdd[i].Update(~blockEntityMask, segmentIdx, segmentBlockIdx);
                }

                if (TrackAddedOrChanged) {
                    SetOnAddTrackingBits(entityId, segmentIdx, segmentBlockIdx, blockEntityMask);
                }

                ref var component = ref componentSegment[segmentEntityIdx];
                if (HasOnAdd) {
                    CallOnAdd(ref component, entity);
                }

                return ref component;
            }

            /// <summary>
            /// Ensures entity has component <typeparamref name="T"/> and reports whether it was newly added.
            /// Behaves identically to <see cref="Add(Entity)"/>: if the component already exists,
            /// returns existing data without modification or hooks. If new, default-initializes and triggers OnAdd.
            /// </summary>
            /// <param name="entity">Target entity. Must not be destroyed or unloaded.</param>
            /// <param name="isNew"><c>true</c> if the component was newly added; <c>false</c> if it already existed.</param>
            /// <returns>A mutable reference to the component data.</returns>
            [MethodImpl(AggressiveInlining)]
            public ref T Add(Entity entity, out bool isNew) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertIsNotTag<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertNotBlockedByQuery(ComponentsTypeName, entity, _blockerAdd);
                AssertNotBlockedByParallelQuery(ComponentsTypeName, entity);
                #endif

                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentEntityIdx = (byte) (entityId & Const.ENTITIES_IN_SEGMENT_MASK);
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);

                ref var maskSegment = ref EntitiesMaskSegments[segmentIdx];
                ref var componentSegment = ref ComponentSegments[segmentIdx];
                if (maskSegment == null) {
                    (maskSegment, componentSegment) = CreateNewSegment(segmentIdx);
                }

                ref var entitiesMask = ref maskSegment[segmentBlockIdx];
                
                isNew = (entitiesMask & blockEntityMask) == 0;
                if (!isNew) {
                    #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                    if (TrackChanged) {
                        SetChangedBit(entityId, segmentIdx, segmentBlockIdx, blockEntityMask);
                    }
                    #endif
                    return ref componentSegment[segmentEntityIdx];
                }

                if (entitiesMask == 0) {
                    var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                    HeuristicChunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT].NotEmptyBlocks.SetBit(chunkBlockIdx);
                }
                entitiesMask |= blockEntityMask;
                if (entitiesMask == ulong.MaxValue) {
                    var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                    HeuristicChunks[segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT].FullBlocks.SetBit(chunkBlockIdx);
                }

                for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                    _queriesToUpdateOnAdd[i].Update(~blockEntityMask, segmentIdx, segmentBlockIdx);
                }

                if (TrackAddedOrChanged) {
                    SetOnAddTrackingBits(entityId, segmentIdx, segmentBlockIdx, blockEntityMask);
                }

                ref var component = ref componentSegment[segmentEntityIdx];
                if (HasOnAdd) {
                    CallOnAdd(ref component, entity);
                }

                return ref component;
            }

            /// <summary>
            /// Sets component <typeparamref name="T"/> to the given value on the entity, always overwriting.
            /// <para>
            /// If the component already exists: calls <see cref="IComponent.OnDelete{TWorld}"/> on the old value,
            /// overwrites with <paramref name="value"/>, then calls <see cref="IComponent.OnAdd{TWorld}"/> on the new value.
            /// If the component is new: sets the value, updates query caches, and calls OnAdd.
            /// </para>
            /// <para>
            /// Unlike <see cref="Add(Entity)"/>, this overload always replaces the data, making it suitable
            /// for "set to this exact value" semantics rather than "ensure exists and modify".
            /// </para>
            /// </summary>
            /// <param name="entity">Target entity. Must not be destroyed or unloaded.</param>
            /// <param name="value">The value to store.</param>
            /// <param name="withOnAdd">If <c>true</c> (default), calls OnAdd hook if implemented.</param>
            [MethodImpl(AggressiveInlining)]
            public void Set(Entity entity, T value, bool withOnAdd = true) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertIsNotTag<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertNotBlockedByQuery(ComponentsTypeName, entity, _blockerAdd);
                AssertNotBlockedByParallelQuery(ComponentsTypeName, entity);
                #endif

                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentEntityIdx = (byte) (entityId & Const.ENTITIES_IN_SEGMENT_MASK);
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);

                ref var maskSegment = ref EntitiesMaskSegments[segmentIdx];
                ref var componentSegment = ref ComponentSegments[segmentIdx];
                if (maskSegment == null) {
                    (maskSegment, componentSegment) = CreateNewSegment(segmentIdx);
                }

                ref var entitiesMask = ref maskSegment[segmentBlockIdx];
                
                ref var component = ref componentSegment[segmentEntityIdx];
                if ((entitiesMask & blockEntityMask) == 0) {
                    if (entitiesMask == 0) {
                        var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                        HeuristicChunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT].NotEmptyBlocks.SetBit(chunkBlockIdx);
                    }
                    entitiesMask |= blockEntityMask;
                    if (entitiesMask == ulong.MaxValue) {
                        var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                        HeuristicChunks[segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT].FullBlocks.SetBit(chunkBlockIdx);
                    }

                    for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                        _queriesToUpdateOnAdd[i].Update(~blockEntityMask, segmentIdx, segmentBlockIdx);
                    }

                    if (TrackAdded) {
                        SetAddedBit(entityId, segmentIdx, segmentBlockIdx, blockEntityMask);
                    }
                }
                else if (HasOnDelete) {
                    CallOnDelete(ref component, entity, HookReason.Default);
                }

                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                if (TrackChanged) {
                    SetChangedBit(entityId, segmentIdx, segmentBlockIdx, blockEntityMask);
                }
                #endif

                component = value;

                if (withOnAdd && HasOnAdd) {
                    CallOnAdd(ref component, entity);
                }
            }
            
            [MethodImpl(NoInlining)]
            private void CallOnAdd(ref T component, Entity entity) {
                component.OnAdd(entity);
            }

            /// <summary>
            /// Removes component <typeparamref name="T"/> from the entity.
            /// If the component is present: clears presence and disabled bitmasks, updates query caches,
            /// and optionally calls <see cref="IComponent.OnDelete{TWorld}"/> on the stored value.
            /// If the entity's segment becomes completely empty after deletion, the segment is returned to the pool.
            /// <para>
            /// The <paramref name="reason"/> parameter indicates why the component is being removed
            /// (<see cref="HookReason.Default"/> for explicit removal, <see crefHookReason.UnloadEntityad"/> for entity unloading,
            /// <see cref="HookReason.WorldDestroy"/> for world destruction) and is forwarded to the OnDelete hook.
            /// </para>
            /// </summary>
            /// <param name="entity">Target entity. Must not be destroyed or unloaded.</param>
            /// <param name="reason">The reason for deletion, forwarded to the OnDelete hook.</param>
            /// <returns><c>true</c> if the component was present and removed; <c>false</c> if absent.</returns>
            [MethodImpl(AggressiveInlining)]
            public bool Delete(Entity entity, HookReason reason = HookReason.Default, bool withOnDelete = true) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertNotBlockedByQuery(ComponentsTypeName, entity, _blockerDelete);
                AssertNotBlockedByParallelQuery(ComponentsTypeName, entity);
                #endif
                
                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte)(entityId & Const.ENTITIES_IN_BLOCK_MASK);
                var invertedBlockEntityMask = ~blockEntityMask;
                
                var maskSegment = EntitiesMaskSegments[segmentIdx];

                if (maskSegment == null) {
                    return false;
                }
                
                ref var entitiesMask = ref maskSegment[segmentBlockIdx];
                if ((entitiesMask & blockEntityMask) == 0) {
                    return false;
                }

                if (entitiesMask == ulong.MaxValue) {
                    var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                    HeuristicChunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT].FullBlocks.ClearBit(chunkBlockIdx);
                }

                if (TrackDeleted) {
                    SetDeletedBit(entityId, segmentIdx, segmentBlockIdx, blockEntityMask);
                }

                entitiesMask &= invertedBlockEntityMask;
                if (!IsTag) {
                    maskSegment[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] &= invertedBlockEntityMask;
                }

                for (uint i = 0; i < _queriesToUpdateOnDeleteCount; i++) {
                    _queriesToUpdateOnDelete[i].Update(invertedBlockEntityMask, segmentIdx, segmentBlockIdx);
                }

                if (withOnDelete && HasOnDelete) {
                    var segmentEntityIdx = (byte) (entityId & Const.ENTITIES_IN_SEGMENT_MASK);
                    CallOnDelete(ref ComponentSegments[segmentIdx][segmentEntityIdx], entity, HookReason.Default);
                }
                else if (DataLifecycle) {
                    var segmentEntityIdx = (byte) (entityId & Const.ENTITIES_IN_SEGMENT_MASK);
                    ComponentSegments[segmentIdx][segmentEntityIdx] = DefaultValue;
                }

                                      // V safety during recursive deletion via the OnDelete hook
                if (entitiesMask == 0 && EntitiesMaskSegments[segmentIdx] == maskSegment) {
                    var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                    var notEmptyBlocks = HeuristicChunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT].NotEmptyBlocks.ClearBit(chunkBlockIdx);
                    if ((notEmptyBlocks & _segmentsMaskCache[segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK]) == 0UL) {
                        FreeSegment(entityId >> Const.ENTITIES_IN_CHUNK_SHIFT, segmentIdx, maskSegment, IsTag ? null : ComponentSegments[segmentIdx]);
                    }
                }

                return true;
            }
            
            [MethodImpl(NoInlining)]
            private void CallOnDelete(ref T component, Entity entity, HookReason reason) {
                component.OnDelete(entity, reason);
            }

            /// <summary>
            /// Copies component <typeparamref name="T"/> from <paramref name="source"/> to <paramref name="destination"/>.
            /// If the source entity has this component:
            /// <list type="bullet">
            /// <item>If <see cref="IComponent.CopyTo{TWorld}"/> is implemented: delegates to that hook
            /// (allowing custom deep-copy logic, reference counting, etc.).
            /// <b>Note:</b> The CopyTo hook is responsible for managing the disabled state on the destination entity.
            /// The <paramref name="source"/> disabled state is passed via the <c>disabled</c> parameter.</item>
            /// <item>Otherwise: performs <c>Set(destination, source_value)</c> (bitwise copy).
            /// Also copies the disabled state automatically.</item>
            /// </list>
            /// </summary>
            /// <param name="source">Entity to copy from. Must not be destroyed or unloaded.</param>
            /// <param name="destination">Entity to copy to. Must not be destroyed or unloaded.</param>
            /// <returns><c>true</c> if the source had the component, and it was copied; <c>false</c> if source lacked it.</returns>
            [MethodImpl(AggressiveInlining)]
            public bool Copy(Entity source, Entity destination) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, source);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, destination);
                #endif
                
                var sourceEntityId = source.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = sourceEntityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((sourceEntityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var sourceBlockEntityMask = 1UL << (byte) (sourceEntityId & Const.ENTITIES_IN_BLOCK_MASK);

                var maskSegment = EntitiesMaskSegments[segmentIdx];
                if (maskSegment != null && (maskSegment[segmentBlockIdx] & sourceBlockEntityMask) != 0) {
                    if (IsTag) {
                        Set(destination);
                    } else {
                        var disabled = (maskSegment[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] & sourceBlockEntityMask) != 0;
                        var sourceSegmentEntityIdx = (byte) (sourceEntityId & Const.ENTITIES_IN_SEGMENT_MASK);
                        if (HasCopyTo) {
                            ComponentSegments[segmentIdx][sourceSegmentEntityIdx].CopyTo(source, destination, disabled);
                        }
                        else {
                            Set(destination, ComponentSegments[segmentIdx][sourceSegmentEntityIdx]);
                            if (disabled) {
                                Disable(destination);
                            }
                        }
                    }
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Moves component <typeparamref name="T"/> from <paramref name="source"/> to <paramref name="destination"/>.
            /// Equivalent to <see cref="Copy"/> followed by <see cref="Delete"/> on the source.
            /// After this operation, the source no longer has the component.
            /// </summary>
            /// <param name="source">Entity to move from. Must not be destroyed or unloaded.</param>
            /// <param name="destination">Entity to move to. Must not be destroyed or unloaded.</param>
            /// <returns><c>true</c> if the source had the component, and it was moved; <c>false</c> if source lacked it.</returns>
            [MethodImpl(AggressiveInlining)]
            public bool Move(Entity source, Entity destination) {
                if (IsTag && source.IdWithOffset == destination.IdWithOffset) {
                    return Has(source);
                }
                if (Copy(source, destination)) {
                    Delete(source);
                    return true;
                }

                return false;
            }

            #region TAG_OPERATIONS
            /// <summary>
            /// Sets (adds) tag on the entity. Tag-only operation.
            /// If the tag is already present, this is a no-op and returns <c>false</c>.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public bool Set(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertNotBlockedByQuery(ComponentsTypeName, entity, _blockerAdd);
                AssertNotBlockedByParallelQuery(ComponentsTypeName, entity);
                AssertIsTag<T>(ComponentsTypeName);
                #endif

                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);

                ref var maskSegment = ref EntitiesMaskSegments[segmentIdx];
                maskSegment ??= CreateNewSegment(segmentIdx).Item1;

                ref var entitiesMask = ref maskSegment[segmentBlockIdx];

                if ((entitiesMask & blockEntityMask) != 0) {
                    return false;
                }

                var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                if (entitiesMask == 0) {
                    HeuristicChunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT].NotEmptyBlocks.SetBit(chunkBlockIdx);
                }

                entitiesMask |= blockEntityMask;
                if (entitiesMask == ulong.MaxValue) {
                    HeuristicChunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT].FullBlocks.SetBit(chunkBlockIdx);
                }

                for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                    _queriesToUpdateOnAdd[i].Update(~blockEntityMask, segmentIdx, segmentBlockIdx);
                }

                if (TrackAdded) {
                    SetAddedBit(entityId, segmentIdx, segmentBlockIdx, blockEntityMask);
                }

                return true;
            }

            /// <summary>
            /// Toggles tag on the entity: removes if present, adds if absent. Tag-only operation.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public bool Toggle(Entity entity) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertIsTag<T>(ComponentsTypeName);
                #endif

                if (Delete(entity)) {
                    return false;
                }

                Set(entity);
                return true;
            }

            /// <summary>
            /// Conditionally sets or removes tag based on the state flag. Tag-only operation.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            public void Apply(Entity entity, bool state) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                AssertIsTag<T>(ComponentsTypeName);
                #endif

                if (state) {
                    Set(entity);
                }
                else {
                    Delete(entity);
                }
            }
            #endregion

            /// <summary>
            /// Counts the total number of entities that currently have component <typeparamref name="T"/>
            /// (both enabled and disabled). This is a full scan over all heuristic chunks and bitmasks,
            /// using popcount for efficient bit counting. The result is exact but not O(1) — use
            /// sparingly, not in hot loops.
            /// </summary>
            /// <returns>Total number of entities with this component across the entire world.</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly uint CalculateCount() {
                uint count = 0;

                for (var chunkIdx = 0; chunkIdx < HeuristicChunks.Length; chunkIdx++) {
                    ref var heuristic = ref HeuristicChunks[chunkIdx];
                    var fullBlocks = heuristic.FullBlocks.Value;
                    count += (uint)(fullBlocks.PopCnt() * Const.ENTITIES_IN_BLOCK);

                    var partialMask = heuristic.NotEmptyBlocks.Value & ~fullBlocks;
                    while (partialMask != 0) {
                        var chunkBlockIdx = Utils.PopLsb(ref partialMask);
                        var segmentIdx = (uint) ((chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT) + (chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT));
                        var segmentBlockIdx = (byte) (chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);

                        count += (uint)EntitiesMaskSegments[segmentIdx][segmentBlockIdx].PopCnt();
                    }
                }

                return count;
            }

            /// <summary>
            /// Calculates the current memory capacity for this component type, in entity slots.
            /// Returns the total number of allocated segment slots (including pooled segments),
            /// each holding <c>ENTITIES_IN_SEGMENT</c> (256) entries. This reflects the memory footprint,
            /// not the actual entity count.
            /// </summary>
            /// <returns>Total capacity in entity slots (allocated segments * 256 + pooled segments * 256).</returns>
            [MethodImpl(AggressiveInlining)]
            public readonly int CalculateCapacity() {
                var count = 0;

                for (var segmentIdx = 0; segmentIdx < EntitiesMaskSegments.Length; segmentIdx++) {
                    if (EntitiesMaskSegments[segmentIdx] != null) {
                        count++;
                    }
                }

                count += _segmentsPoolCount;

                return count * Const.ENTITIES_IN_SEGMENT;
            }
            #endregion

            #region INTERNAL API
            #region BATCH
            [MethodImpl(AggressiveInlining)]
            internal void BatchAdd(QueryData queryData, int nextGlobalBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertMultiThreadNotActive(ComponentsTypeName);
                #endif

                while (nextGlobalBlockIdx >= 0) {
                    ref var blocks = ref queryData.Blocks[nextGlobalBlockIdx];

                    var globalBlockIdx = (uint)nextGlobalBlockIdx;
                    nextGlobalBlockIdx = blocks.NextGlobalBlock;

                    var segmentIdx = globalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                    var segmentBlockIdx = (byte) (globalBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);

                    BatchAdd(blocks.EntitiesMask, segmentIdx, segmentBlockIdx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void BatchSet(QueryData queryData, int nextGlobalBlockIdx, T value) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertMultiThreadNotActive(ComponentsTypeName);
                #endif
                if (IsTag) {
                    while (nextGlobalBlockIdx >= 0) {
                        ref var blocks = ref queryData.Blocks[nextGlobalBlockIdx];
                    
                        var globalBlockIdx = (uint)nextGlobalBlockIdx;
                        nextGlobalBlockIdx = blocks.NextGlobalBlock;
                    
                        var segmentIdx = globalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                        var segmentBlockIdx = (byte) (globalBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);

                        BatchSetTag(blocks.EntitiesMask, segmentIdx, segmentBlockIdx);
                    }
                }
                else {
                    while (nextGlobalBlockIdx >= 0) {
                        ref var blocks = ref queryData.Blocks[nextGlobalBlockIdx];

                        var globalBlockIdx = (uint)nextGlobalBlockIdx;
                        nextGlobalBlockIdx = blocks.NextGlobalBlock;

                        var segmentIdx = globalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                        var segmentBlockIdx = (byte)(globalBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);

                        BatchSet(value, blocks.EntitiesMask, segmentIdx, segmentBlockIdx);
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void BatchDelete(QueryData queryData, int nextGlobalBlockIdx, HookReason reason = HookReason.Default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertMultiThreadNotActive(ComponentsTypeName);
                #endif

                if (IsTag) {
                    while (nextGlobalBlockIdx >= 0) {
                        ref var blocks = ref queryData.Blocks[nextGlobalBlockIdx];
                    
                        var globalBlockIdx = (uint)nextGlobalBlockIdx;
                        nextGlobalBlockIdx = blocks.NextGlobalBlock;
                    
                        var segmentIdx = globalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                        var segmentBlockIdx = (byte) (globalBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);

                        BatchDeleteTag(blocks.EntitiesMask, segmentIdx, segmentBlockIdx);
                    }
                }
                else {
                    while (nextGlobalBlockIdx >= 0) {
                        ref var blocks = ref queryData.Blocks[nextGlobalBlockIdx];

                        var globalBlockIdx = (uint)nextGlobalBlockIdx;
                        nextGlobalBlockIdx = blocks.NextGlobalBlock;

                        var segmentIdx = globalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                        var segmentBlockIdx = (byte) (globalBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);

                        BatchDelete(blocks.EntitiesMask, segmentIdx, segmentBlockIdx, reason);
                    }
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void BatchDisable(QueryData queryData, int nextGlobalBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertMultiThreadNotActive(ComponentsTypeName);
                #endif

                while (nextGlobalBlockIdx >= 0) {
                    ref var blocks = ref queryData.Blocks[nextGlobalBlockIdx];
                    
                    var globalBlockIdx = (uint)nextGlobalBlockIdx;
                    nextGlobalBlockIdx = blocks.NextGlobalBlock;
                    
                    var segmentIdx = globalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                    var segmentBlockIdx = (byte) (globalBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);

                    BatchDisable(blocks.EntitiesMask, segmentIdx, segmentBlockIdx);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void BatchEnable(QueryData queryData, int nextGlobalBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertMultiThreadNotActive(ComponentsTypeName);
                #endif

                while (nextGlobalBlockIdx >= 0) {
                    ref var blocks = ref queryData.Blocks[nextGlobalBlockIdx];
                    
                    var globalBlockIdx = (uint)nextGlobalBlockIdx;
                    nextGlobalBlockIdx = blocks.NextGlobalBlock;
                    
                    var segmentIdx = globalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                    var segmentBlockIdx = (byte) (globalBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);

                    BatchEnable(blocks.EntitiesMask, segmentIdx, segmentBlockIdx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void BatchToggle(QueryData queryData, int nextGlobalBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertMultiThreadNotActive(ComponentsTypeName);
                #endif

                while (nextGlobalBlockIdx >= 0) {
                    ref var blocks = ref queryData.Blocks[nextGlobalBlockIdx];
                    
                    var globalBlockIdx = (uint)nextGlobalBlockIdx;
                    nextGlobalBlockIdx = blocks.NextGlobalBlock;
                    
                    var segmentIdx = globalBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                    var segmentBlockIdx = (byte) (globalBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);

                    BatchToggle(blocks.EntitiesMask, segmentIdx, segmentBlockIdx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void BatchApply(QueryData queryData, int nextGlobalBlockIdx, bool state) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertMultiThreadNotActive(ComponentsTypeName);
                #endif
                if (state) {
                    BatchSet(queryData, nextGlobalBlockIdx, default);
                }
                else {
                    BatchDelete(queryData, nextGlobalBlockIdx);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void BatchAdd(ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                #endif

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                var baseGlobalBlockEntityIdx = (uint)((segmentIdx << Const.ENTITIES_IN_SEGMENT_SHIFT) + (segmentBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT));
                var chunkBlockIdx = (byte)((baseGlobalBlockEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                var chunkIdx = baseGlobalBlockEntityIdx >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var baseSegmentEntityIdx = (byte) (baseGlobalBlockEntityIdx & Const.ENTITIES_IN_SEGMENT_MASK);

                ref var maskSegment = ref EntitiesMaskSegments[segmentIdx];
                ref var componentSegment = ref ComponentSegments[segmentIdx];
                if (maskSegment == null) {
                    (maskSegment, componentSegment) = CreateNewSegment(segmentIdx);
                }

                ref var entitiesMask = ref maskSegment[segmentBlockIdx];
                entitiesMaskFilter &= ~entitiesMask;
                if (entitiesMaskFilter == 0) {
                    return;
                }

                if (TrackAddedOrChanged) {
                    SetOnAddTrackingBitsBatch(entitiesMaskFilter, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                }

                if (entitiesMask == 0) {
                    HeuristicChunks[chunkIdx].NotEmptyBlocks.SetBit(chunkBlockIdx);
                }

                if (!HasOnAdd && _queriesToUpdateOnAddCount == 0) {
                    entitiesMask |= entitiesMaskFilter;
                }
                else if (entitiesMaskFilter == ulong.MaxValue && !HasOnAdd) {
                    entitiesMask = ulong.MaxValue;
                    for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                        _queriesToUpdateOnAdd[i].BatchUpdate(segmentIdx, segmentBlockIdx);
                    }
                }
                else if (!HasOnAdd) {
                    var starts = entitiesMaskFilter & ~(entitiesMaskFilter << 1);
                    var ends = entitiesMaskFilter & ~(entitiesMaskFilter >> 1);
                    do {
                        #if NET6_0_OR_GREATER
                        var start = (byte)System.Numerics.BitOperations.TrailingZeroCount(starts);
                        var endIncluded = (byte)System.Numerics.BitOperations.TrailingZeroCount(ends);
                        #else
                        var start = deBruijn[(uint)(((starts & (ulong)-(long)starts) * 0x37E84A99DAE458FUL) >> 58)];
                        var endIncluded = deBruijn[(uint)(((ends & (ulong)-(long)ends) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        starts &= starts - 1UL;
                        ends &= ends - 1UL;

                        var runMask = ((2UL << (endIncluded - start)) - 1) << start;
                        entitiesMask |= runMask;

                        var invertedRunMask = ~runMask;
                        for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                            _queriesToUpdateOnAdd[i].Update(invertedRunMask, segmentIdx, segmentBlockIdx);
                        }
                    } while (starts != 0);
                }
                else {
                    var entity = new Entity(baseGlobalBlockEntityIdx);
                    ref var entityIdRef = ref entity.IdWithOffset;

                    var starts = entitiesMaskFilter & ~(entitiesMaskFilter << 1);
                    var ends = entitiesMaskFilter & ~(entitiesMaskFilter >> 1);
                    do {
                        #if NET6_0_OR_GREATER
                        var start = (byte)System.Numerics.BitOperations.TrailingZeroCount(starts);
                        var endIncluded = (byte)System.Numerics.BitOperations.TrailingZeroCount(ends);
                        #else
                        var start = deBruijn[(uint)(((starts & (ulong)-(long)starts) * 0x37E84A99DAE458FUL) >> 58)];
                        var endIncluded = deBruijn[(uint)(((ends & (ulong)-(long)ends) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        starts &= starts - 1UL;
                        ends &= ends - 1UL;

                        entityIdRef = baseGlobalBlockEntityIdx + start + Const.ENTITY_ID_OFFSET;

                        var blockEntityMask = 1UL << start;
                        while (start <= endIncluded) {
                            entitiesMask |= blockEntityMask;

                            #if FFS_ECS_DEBUG
                            Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                            #endif
                            componentSegment[baseSegmentEntityIdx + start].OnAdd(entity);

                            for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                                _queriesToUpdateOnAdd[i].Update(~blockEntityMask, segmentIdx, segmentBlockIdx);
                            }

                            blockEntityMask <<= 1;
                            entityIdRef++;
                            start++;
                        }
                    } while (starts != 0);
                }

                if (entitiesMask == ulong.MaxValue) {
                    HeuristicChunks[chunkIdx].FullBlocks.SetBit(chunkBlockIdx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void BatchSet(T value, ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                #endif

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                var baseGlobalBlockEntityIdx = (uint)((segmentIdx << Const.ENTITIES_IN_SEGMENT_SHIFT) + (segmentBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT));
                var chunkBlockIdx = (byte)((baseGlobalBlockEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                var chunkIdx = baseGlobalBlockEntityIdx >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var baseSegmentEntityIdx = (byte) (baseGlobalBlockEntityIdx & Const.ENTITIES_IN_SEGMENT_MASK);

                ref var maskSegment = ref EntitiesMaskSegments[segmentIdx];
                ref var componentSegment = ref ComponentSegments[segmentIdx];
                if (maskSegment == null) {
                    (maskSegment, componentSegment) = CreateNewSegment(segmentIdx);
                }

                ref var entitiesMask = ref maskSegment[segmentBlockIdx];
                var existingEntitiesMask = entitiesMaskFilter & entitiesMask;
                var newEntitiesMaskFilter = entitiesMaskFilter & ~entitiesMask;

                if (TrackAddedOrChanged) {
                    SetOnAddTrackingBitsBatch(entitiesMaskFilter, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                }

                if (HasOnDelete && existingEntitiesMask != 0) {
                    var entity = new Entity(baseGlobalBlockEntityIdx);
                    ref var entityIdRef = ref entity.IdWithOffset;

                    var starts = existingEntitiesMask & ~(existingEntitiesMask << 1);
                    var ends = existingEntitiesMask & ~(existingEntitiesMask >> 1);
                    do {
                        #if NET6_0_OR_GREATER
                        var start = (byte)System.Numerics.BitOperations.TrailingZeroCount(starts);
                        var endIncluded = (byte)System.Numerics.BitOperations.TrailingZeroCount(ends);
                        #else
                        var start = deBruijn[(uint)(((starts & (ulong)-(long)starts) * 0x37E84A99DAE458FUL) >> 58)];
                        var endIncluded = deBruijn[(uint)(((ends & (ulong)-(long)ends) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        starts &= starts - 1UL;
                        ends &= ends - 1UL;

                        entityIdRef = baseGlobalBlockEntityIdx + start + Const.ENTITY_ID_OFFSET;
                        while (start <= endIncluded) {
                            #if FFS_ECS_DEBUG
                            Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                            #endif
                            componentSegment[baseSegmentEntityIdx + start].OnDelete(entity, HookReason.Default);
                            entityIdRef++;
                            start++;
                        }
                    } while (starts != 0);

                    if (EntitiesMaskSegments[segmentIdx] != maskSegment) {
                        #pragma warning disable EPC30
                        // ReSharper disable once TailRecursiveCall
                        BatchSet(value, entitiesMaskFilter, segmentIdx, segmentBlockIdx);
                        #pragma warning restore EPC30
                        return;
                    }
                }

                {
                    var starts = entitiesMaskFilter & ~(entitiesMaskFilter << 1);
                    var ends = entitiesMaskFilter & ~(entitiesMaskFilter >> 1);
                    do {
                        #if NET6_0_OR_GREATER
                        var start = (byte)System.Numerics.BitOperations.TrailingZeroCount(starts);
                        var endIncluded = (byte)System.Numerics.BitOperations.TrailingZeroCount(ends);
                        #else
                        var start = deBruijn[(uint)(((starts & (ulong)-(long)starts) * 0x37E84A99DAE458FUL) >> 58)];
                        var endIncluded = deBruijn[(uint)(((ends & (ulong)-(long)ends) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        starts &= starts - 1UL;
                        ends &= ends - 1UL;

                        var count = endIncluded - start + 1;
                        new Span<T>(componentSegment, baseSegmentEntityIdx + start, count).Fill(value);
                    } while (starts != 0);
                }

                if (newEntitiesMaskFilter != 0) {
                    if (entitiesMask == 0) {
                        HeuristicChunks[chunkIdx].NotEmptyBlocks.SetBit(chunkBlockIdx);
                    }

                    if (_queriesToUpdateOnAddCount == 0) {
                        entitiesMask |= newEntitiesMaskFilter;
                    }
                    else if (newEntitiesMaskFilter == ulong.MaxValue) {
                        entitiesMask = ulong.MaxValue;
                        for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                            _queriesToUpdateOnAdd[i].BatchUpdate(segmentIdx, segmentBlockIdx);
                        }
                    }
                    else {
                        var starts = newEntitiesMaskFilter & ~(newEntitiesMaskFilter << 1);
                        var ends = newEntitiesMaskFilter & ~(newEntitiesMaskFilter >> 1);
                        do {
                            #if NET6_0_OR_GREATER
                            var start = (byte)System.Numerics.BitOperations.TrailingZeroCount(starts);
                            var endIncluded = (byte)System.Numerics.BitOperations.TrailingZeroCount(ends);
                            #else
                            var start = deBruijn[(uint)(((starts & (ulong)-(long)starts) * 0x37E84A99DAE458FUL) >> 58)];
                            var endIncluded = deBruijn[(uint)(((ends & (ulong)-(long)ends) * 0x37E84A99DAE458FUL) >> 58)];
                            #endif
                            starts &= starts - 1UL;
                            ends &= ends - 1UL;

                            var runMask = ((2UL << (endIncluded - start)) - 1) << start;
                            entitiesMask |= runMask;

                            var invertedRunMask = ~runMask;
                            for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                                _queriesToUpdateOnAdd[i].Update(invertedRunMask, segmentIdx, segmentBlockIdx);
                            }
                        } while (starts != 0);
                    }

                    if (entitiesMask == ulong.MaxValue) {
                        HeuristicChunks[chunkIdx].FullBlocks.SetBit(chunkBlockIdx);
                    }
                }

                if (HasOnAdd) {
                    var entity = new Entity(baseGlobalBlockEntityIdx);
                    ref var entityIdRef = ref entity.IdWithOffset;

                    var starts = entitiesMaskFilter & ~(entitiesMaskFilter << 1);
                    var ends = entitiesMaskFilter & ~(entitiesMaskFilter >> 1);
                    do {
                        #if NET6_0_OR_GREATER
                        var start = (byte)System.Numerics.BitOperations.TrailingZeroCount(starts);
                        var endIncluded = (byte)System.Numerics.BitOperations.TrailingZeroCount(ends);
                        #else
                        var start = deBruijn[(uint)(((starts & (ulong)-(long)starts) * 0x37E84A99DAE458FUL) >> 58)];
                        var endIncluded = deBruijn[(uint)(((ends & (ulong)-(long)ends) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        starts &= starts - 1UL;
                        ends &= ends - 1UL;

                        entityIdRef = baseGlobalBlockEntityIdx + start + Const.ENTITY_ID_OFFSET;
                        while (start <= endIncluded) {
                            #if FFS_ECS_DEBUG
                            Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                            #endif
                            componentSegment[baseSegmentEntityIdx + start].OnAdd(entity);
                            entityIdRef++;
                            start++;
                        }
                    } while (starts != 0);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void BatchDelete(ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx, HookReason reason = HookReason.Default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                #endif

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                var baseGlobalBlockEntityIdx = (uint)((segmentIdx << Const.ENTITIES_IN_SEGMENT_SHIFT) + (segmentBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT));
                var chunkBlockIdx = (byte)((baseGlobalBlockEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                var chunkIdx = baseGlobalBlockEntityIdx >> Const.ENTITIES_IN_CHUNK_SHIFT;
                var baseSegmentEntityIdx = (byte) (baseGlobalBlockEntityIdx & Const.ENTITIES_IN_SEGMENT_MASK);

                var maskSegment = EntitiesMaskSegments[segmentIdx];
                var componentSegment = ComponentSegments[segmentIdx];
                if (maskSegment == null) {
                    return;
                }

                ref var entitiesMask = ref maskSegment[segmentBlockIdx];
                entitiesMaskFilter &= entitiesMask;
                if (entitiesMaskFilter == 0) {
                    return;
                }

                if (TrackDeleted) {
                    SetDeletedBitBatch(entitiesMaskFilter, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                }

                ref var disabledEntitiesMask = ref maskSegment[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT];
                if (entitiesMask == ulong.MaxValue) {
                    HeuristicChunks[chunkIdx].FullBlocks.ClearBit(chunkBlockIdx);
                }

                if (!HasOnDelete && !DataLifecycle && _queriesToUpdateOnDeleteCount == 0) {
                    entitiesMask &= ~entitiesMaskFilter;
                    disabledEntitiesMask &= ~entitiesMaskFilter;
                }
                else if (entitiesMaskFilter == ulong.MaxValue && !HasOnDelete && !DataLifecycle) {
                    entitiesMask = 0;
                    disabledEntitiesMask = 0;
                    for (uint i = 0; i < _queriesToUpdateOnDeleteCount; i++) {
                        _queriesToUpdateOnDelete[i].BatchUpdate(segmentIdx, segmentBlockIdx);
                    }
                }
                else if (entitiesMaskFilter == ulong.MaxValue && !HasOnDelete) {
                    entitiesMask = 0;
                    disabledEntitiesMask = 0;
                    if (HasDefaultValue) {
                        componentSegment.AsSpan(baseSegmentEntityIdx, Const.ENTITIES_IN_BLOCK).Fill(DefaultValue);
                    } else {
                        Array.Clear(componentSegment, baseSegmentEntityIdx, Const.ENTITIES_IN_BLOCK);
                    }
                    for (uint i = 0; i < _queriesToUpdateOnDeleteCount; i++) {
                        _queriesToUpdateOnDelete[i].BatchUpdate(segmentIdx, segmentBlockIdx);
                    }
                }
                else if (!HasOnDelete) {
                    var starts = entitiesMaskFilter & ~(entitiesMaskFilter << 1);
                    var ends = entitiesMaskFilter & ~(entitiesMaskFilter >> 1);
                    do {
                        #if NET6_0_OR_GREATER
                        var start = (byte)System.Numerics.BitOperations.TrailingZeroCount(starts);
                        var endIncluded = (byte)System.Numerics.BitOperations.TrailingZeroCount(ends);
                        #else
                        var start = deBruijn[(uint)(((starts & (ulong)-(long)starts) * 0x37E84A99DAE458FUL) >> 58)];
                        var endIncluded = deBruijn[(uint)(((ends & (ulong)-(long)ends) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        starts &= starts - 1UL;
                        ends &= ends - 1UL;

                        var count = endIncluded - start + 1;
                        var runMask = ((2UL << (endIncluded - start)) - 1) << start;
                        var invertedRunMask = ~runMask;
                        entitiesMask &= invertedRunMask;
                        disabledEntitiesMask &= invertedRunMask;

                        if (HasDefaultValue) {
                            componentSegment.AsSpan(baseSegmentEntityIdx + start, count).Fill(DefaultValue);
                        } else {
                            Array.Clear(componentSegment, baseSegmentEntityIdx + start, count);
                        }

                        for (uint i = 0; i < _queriesToUpdateOnDeleteCount; i++) {
                            _queriesToUpdateOnDelete[i].Update(invertedRunMask, segmentIdx, segmentBlockIdx);
                        }
                    } while (starts != 0);
                }
                else {
                    var entity = new Entity(baseGlobalBlockEntityIdx);
                    ref var entityIdRef = ref entity.IdWithOffset;

                    var starts = entitiesMaskFilter & ~(entitiesMaskFilter << 1);
                    var ends = entitiesMaskFilter & ~(entitiesMaskFilter >> 1);
                    do {
                        #if NET6_0_OR_GREATER
                        var start = (byte)System.Numerics.BitOperations.TrailingZeroCount(starts);
                        var endIncluded = (byte)System.Numerics.BitOperations.TrailingZeroCount(ends);
                        #else
                        var start = deBruijn[(uint)(((starts & (ulong)-(long)starts) * 0x37E84A99DAE458FUL) >> 58)];
                        var endIncluded = deBruijn[(uint)(((ends & (ulong)-(long)ends) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        starts &= starts - 1UL;
                        ends &= ends - 1UL;

                        entityIdRef = baseGlobalBlockEntityIdx + start + Const.ENTITY_ID_OFFSET;

                        var blockEntityMask = 1UL << start;
                        while (start <= endIncluded) {
                            var invertedBlockEntityMask = ~blockEntityMask;
                            entitiesMask &= invertedBlockEntityMask;
                            disabledEntitiesMask &= invertedBlockEntityMask;

                            #if FFS_ECS_DEBUG
                            Data.Instance.SetCurrentQueryEntity(entity.IdWithOffset);
                            #endif
                            componentSegment[baseSegmentEntityIdx + start].OnDelete(entity, reason);

                            for (uint i = 0; i < _queriesToUpdateOnDeleteCount; i++) {
                                _queriesToUpdateOnDelete[i].Update(invertedBlockEntityMask, segmentIdx, segmentBlockIdx);
                            }

                            blockEntityMask <<= 1;
                            entityIdRef++;
                            start++;
                        }
                    } while (starts != 0);
                }

                if (entitiesMask == 0 && EntitiesMaskSegments[segmentIdx] == maskSegment) {
                    var notEmptyBlocks = HeuristicChunks[chunkIdx].NotEmptyBlocks.ClearBit(chunkBlockIdx);
                    if ((notEmptyBlocks & _segmentsMaskCache[segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK]) == 0UL) {
                        FreeSegment(chunkIdx, segmentIdx, maskSegment, componentSegment);
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void BatchDisable(ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                #endif

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                var maskSegment = EntitiesMaskSegments[segmentIdx];
                if (maskSegment == null) {
                    return;
                }

                entitiesMaskFilter &= maskSegment[segmentBlockIdx];
                if (entitiesMaskFilter == 0) {
                    return;
                }

                ref var disabledEntitiesMask = ref maskSegment[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT];

                if (entitiesMaskFilter == ulong.MaxValue) {
                    disabledEntitiesMask = ulong.MaxValue;
                    for (uint i = 0; i < _queriesToUpdateOnDisableCount; i++) {
                        _queriesToUpdateOnDisable[i].BatchUpdate(segmentIdx, segmentBlockIdx);
                    }
                }
                else if (_queriesToUpdateOnDisableCount == 0) {
                    disabledEntitiesMask |= entitiesMaskFilter;
                }
                else {
                    var starts = entitiesMaskFilter & ~(entitiesMaskFilter << 1);
                    var ends = entitiesMaskFilter & ~(entitiesMaskFilter >> 1);
                    do {
                        #if NET6_0_OR_GREATER
                        var start = (byte)System.Numerics.BitOperations.TrailingZeroCount(starts);
                        var endIncluded = (byte)System.Numerics.BitOperations.TrailingZeroCount(ends);
                        #else
                        var start = deBruijn[(uint)(((starts & (ulong)-(long)starts) * 0x37E84A99DAE458FUL) >> 58)];
                        var endIncluded = deBruijn[(uint)(((ends & (ulong)-(long)ends) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        starts &= starts - 1UL;
                        ends &= ends - 1UL;

                        var runMask = ((2UL << (endIncluded - start)) - 1) << start;
                        disabledEntitiesMask |= runMask;

                        var invertedRunMask = ~runMask;
                        for (uint i = 0; i < _queriesToUpdateOnDisableCount; i++) {
                            _queriesToUpdateOnDisable[i].Update(invertedRunMask, segmentIdx, segmentBlockIdx);
                        }
                    } while (starts != 0);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void BatchEnable(ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                #endif

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                var maskSegment = EntitiesMaskSegments[segmentIdx];
                if (maskSegment == null) {
                    return;
                }

                entitiesMaskFilter &= maskSegment[segmentBlockIdx];
                if (entitiesMaskFilter == 0) {
                    return;
                }

                ref var disabledEntitiesMask = ref maskSegment[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT];

                if (entitiesMaskFilter == ulong.MaxValue) {
                    disabledEntitiesMask = 0;
                    for (uint i = 0; i < _queriesToUpdateOnEnableCount; i++) {
                        _queriesToUpdateOnEnable[i].BatchUpdate(segmentIdx, segmentBlockIdx);
                    }
                }
                else if (_queriesToUpdateOnEnableCount == 0) {
                    disabledEntitiesMask &= ~entitiesMaskFilter;
                }
                else {
                    var starts = entitiesMaskFilter & ~(entitiesMaskFilter << 1);
                    var ends = entitiesMaskFilter & ~(entitiesMaskFilter >> 1);
                    do {
                        #if NET6_0_OR_GREATER
                        var start = (byte)System.Numerics.BitOperations.TrailingZeroCount(starts);
                        var endIncluded = (byte)System.Numerics.BitOperations.TrailingZeroCount(ends);
                        #else
                        var start = deBruijn[(uint)(((starts & (ulong)-(long)starts) * 0x37E84A99DAE458FUL) >> 58)];
                        var endIncluded = deBruijn[(uint)(((ends & (ulong)-(long)ends) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        starts &= starts - 1UL;
                        ends &= ends - 1UL;

                        var runMask = ((2UL << (endIncluded - start)) - 1) << start;
                        var invertedRunMask = ~runMask;
                        disabledEntitiesMask &= invertedRunMask;

                        for (uint i = 0; i < _queriesToUpdateOnEnableCount; i++) {
                            _queriesToUpdateOnEnable[i].Update(invertedRunMask, segmentIdx, segmentBlockIdx);
                        }
                    } while (starts != 0);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void BatchSetTag(ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                #endif

                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                var baseGlobalBlockEntityIdx = (uint)((segmentIdx << Const.ENTITIES_IN_SEGMENT_SHIFT) + (segmentBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT));
                var chunkBlockIdx = (byte)((baseGlobalBlockEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                var chunkIdx = baseGlobalBlockEntityIdx >> Const.ENTITIES_IN_CHUNK_SHIFT;

                ref var maskSegment = ref EntitiesMaskSegments[segmentIdx];
                maskSegment ??= CreateNewSegment(segmentIdx).Item1;

                ref var entitiesMask = ref maskSegment[segmentBlockIdx];
                entitiesMaskFilter &= ~entitiesMask;
                if (entitiesMaskFilter == 0) {
                    return;
                }

                if (TrackAdded) {
                    SetAddedBitBatch(entitiesMaskFilter, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                }

                if (entitiesMask == 0) {
                    HeuristicChunks[chunkIdx].NotEmptyBlocks.SetBit(chunkBlockIdx);
                }

                if (_queriesToUpdateOnAddCount == 0) {
                    entitiesMask |= entitiesMaskFilter;
                }
                else if (entitiesMaskFilter == ulong.MaxValue) {
                    entitiesMask = ulong.MaxValue;
                    for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                        _queriesToUpdateOnAdd[i].BatchUpdate(segmentIdx, segmentBlockIdx);
                    }
                }
                else {
                    var starts = entitiesMaskFilter & ~(entitiesMaskFilter << 1);
                    var ends = entitiesMaskFilter & ~(entitiesMaskFilter >> 1);
                    do {
                        #if NET6_0_OR_GREATER
                        var start = (byte)System.Numerics.BitOperations.TrailingZeroCount(starts);
                        var endIncluded = (byte)System.Numerics.BitOperations.TrailingZeroCount(ends);
                        #else
                        var start = deBruijn[(uint)(((starts & (ulong)-(long)starts) * 0x37E84A99DAE458FUL) >> 58)];
                        var endIncluded = deBruijn[(uint)(((ends & (ulong)-(long)ends) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        starts &= starts - 1UL;
                        ends &= ends - 1UL;

                        var runMask = ((2UL << (endIncluded - start)) - 1) << start;
                        entitiesMask |= runMask;

                        var invertedRunMask = ~runMask;
                        for (uint i = 0; i < _queriesToUpdateOnAddCount; i++) {
                            _queriesToUpdateOnAdd[i].Update(invertedRunMask, segmentIdx, segmentBlockIdx);
                        }
                    } while (starts != 0);
                }

                if (entitiesMask == ulong.MaxValue) {
                    HeuristicChunks[chunkIdx].FullBlocks.SetBit(chunkBlockIdx);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void BatchDeleteTag(ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                #endif

                var baseGlobalBlockEntityIdx = (uint)((segmentIdx << Const.ENTITIES_IN_SEGMENT_SHIFT) + (segmentBlockIdx << Const.ENTITIES_IN_BLOCK_SHIFT));
                var chunkBlockIdx = (byte)((baseGlobalBlockEntityIdx >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                var chunkIdx = baseGlobalBlockEntityIdx >> Const.ENTITIES_IN_CHUNK_SHIFT;

                var maskSegment = EntitiesMaskSegments[segmentIdx];
                if (maskSegment == null) {
                    return;
                }

                ref var entitiesMask = ref maskSegment[segmentBlockIdx];
                entitiesMaskFilter &= entitiesMask;
                if (entitiesMaskFilter == 0) {
                    return;
                }

                if (TrackDeleted) {
                    SetDeletedBitBatch(entitiesMaskFilter, segmentIdx, segmentBlockIdx, chunkBlockIdx, chunkIdx);
                }

                if (entitiesMask == ulong.MaxValue) {
                    HeuristicChunks[chunkIdx].FullBlocks.ClearBit(chunkBlockIdx);
                }

                if (_queriesToUpdateOnDeleteCount == 0) {
                    entitiesMask &= ~entitiesMaskFilter;
                }
                else if (entitiesMaskFilter == ulong.MaxValue) {
                    entitiesMask = 0;
                    for (uint i = 0; i < _queriesToUpdateOnDeleteCount; i++) {
                        _queriesToUpdateOnDelete[i].BatchUpdate(segmentIdx, segmentBlockIdx);
                    }
                }
                else {
                    #if !NET6_0_OR_GREATER
                    var deBruijn = Utils.DeBruijn;
                    #endif

                    var starts = entitiesMaskFilter & ~(entitiesMaskFilter << 1);
                    var ends = entitiesMaskFilter & ~(entitiesMaskFilter >> 1);
                    do {
                        #if NET6_0_OR_GREATER
                        var start = (byte)System.Numerics.BitOperations.TrailingZeroCount(starts);
                        var endIncluded = (byte)System.Numerics.BitOperations.TrailingZeroCount(ends);
                        #else
                        var start = deBruijn[(uint)(((starts & (ulong)-(long)starts) * 0x37E84A99DAE458FUL) >> 58)];
                        var endIncluded = deBruijn[(uint)(((ends & (ulong)-(long)ends) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        starts &= starts - 1UL;
                        ends &= ends - 1UL;

                        var runMask = ((2UL << (endIncluded - start)) - 1) << start;
                        entitiesMask &= ~runMask;

                        var invertedRunMask = ~runMask;
                        for (uint i = 0; i < _queriesToUpdateOnDeleteCount; i++) {
                            _queriesToUpdateOnDelete[i].Update(invertedRunMask, segmentIdx, segmentBlockIdx);
                        }
                    } while (starts != 0);
                }

                if (entitiesMask == 0) {
                    var notEmptyBlocks = HeuristicChunks[chunkIdx].NotEmptyBlocks.ClearBit(chunkBlockIdx);
                    if ((notEmptyBlocks & _segmentsMaskCache[segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK]) == 0UL) {
                        _chunkHeuristicWorldMask[chunkIdx][(segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK) * _chunkHeuristicWorldMaskLen + _idDiv] &= _idMaskInv;

                        EntitiesMaskSegments[segmentIdx] = null;

                        var poolIdx = Interlocked.Increment(ref _segmentsPoolCount) - 1;
                        Volatile.Write(ref _segmentsPool[poolIdx], maskSegment);

                        #if FFS_ECS_BURST
                        LifecycleHandle.OnSegmentPooled(segmentIdx);
                        #endif
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void BatchToggle(ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertMultiThreadNotActive(ComponentsTypeName);
                #endif

                ref var maskSegment = ref EntitiesMaskSegments[segmentIdx];

                if (maskSegment == null) {
                    if (IsTag) {
                        BatchSetTag(entitiesMaskFilter, segmentIdx, segmentBlockIdx);
                    }
                    else {
                        BatchSet(default, entitiesMaskFilter, segmentIdx, segmentBlockIdx);
                    }
                    return;
                }

                var entitiesMask = maskSegment[segmentBlockIdx];
                var toDelete = entitiesMaskFilter & entitiesMask;
                var toSet = entitiesMaskFilter & ~entitiesMask;

                if (toDelete != 0) {
                    if (IsTag) {
                        BatchDeleteTag(toDelete, segmentIdx, segmentBlockIdx);
                    }
                    else {
                        BatchDelete(toDelete, segmentIdx, segmentBlockIdx);
                    }
                }

                if (toSet != 0) {
                    if (IsTag) {
                        BatchSetTag(toSet, segmentIdx, segmentBlockIdx);
                    }
                    else {
                        BatchSet(default, toSet, segmentIdx, segmentBlockIdx);
                    }
                }
            }
            #endregion

            #if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("AOT", "IL2091", Justification = "Component metadata is preserved by the registration path.")]
            #endif
            internal Components(ushort componentId, ComponentTypeConfig<T> config, bool isTag, string typeName) {
                TypeName = typeName;
                IsTag = isTag;
                DynamicId = componentId;
                _idDiv = (ushort) (DynamicId >> Const.U64_SHIFT);
                _idMask = 1UL << (DynamicId & Const.U64_MASK);
                _idMaskInv = ~_idMask;

                Guid = config.Guid!.Value;
                Version = config.Version!.Value;
                _readWriteArrayStrategy = config.ReadWriteStrategy;
                _resettableStrategy = config.ReadWriteStrategy as IPackArrayStrategyResettable;

                _segmentsMaskCache = Const.DataMasks;
                _segmentsPoolCount = 0;
                _queriesToUpdateOnDeleteCount = 0;
                _queriesToUpdateOnAddCount = 0;
                _queriesToUpdateOnDisableCount = 0;
                _queriesToUpdateOnEnableCount = 0;
                _queriesToUpdateOnDelete = new QueryData[Const.MAX_QUERY_DATA_PER_TYPE];
                _queriesToUpdateOnAdd = new QueryData[Const.MAX_QUERY_DATA_PER_TYPE];
                _queriesToUpdateOnDisable = isTag ? null : new QueryData[Const.MAX_QUERY_DATA_PER_TYPE];
                _queriesToUpdateOnEnable = isTag ? null : new QueryData[Const.MAX_QUERY_DATA_PER_TYPE];
                if (isTag) {
                    HasOnAdd = false;
                    HasOnDelete = false;
                    HasCopyTo = false;
                    HasWrite = false;
                    HasRead = false;
                } else {
                    if (default(T) is IComponentHookOverride hookOverride) {
                        HasOnAdd = hookOverride.HasOnAdd();
                        HasOnDelete = hookOverride.HasOnDelete();
                        HasCopyTo = hookOverride.HasCopyTo();
                    } else {
                        HasOnAdd = ComponentType<T>.HasOnAdd();
                        HasOnDelete = ComponentType<T>.HasOnDelete();
                        HasCopyTo = ComponentType<T>.HasCopyTo();
                    }
                    HasWrite = ComponentType<T>.HasWrite();
                    HasRead = ComponentType<T>.HasRead();
                    if (default(T) is IComponentStrategyOverride packOverride) {
                        _readWriteArrayStrategy = packOverride.ArrayPackStrategy<T>();
                        _resettableStrategy = _readWriteArrayStrategy as IPackArrayStrategyResettable;
                    }
                }
                Unmanaged = isTag || !RuntimeHelpers.IsReferenceOrContainsReferences<T>();
                DataLifecycle = !isTag && !config.NoDataLifecycle.Value;
                DefaultValue = config.DefaultValue ?? default;
                HasDefaultValue = config.DefaultValue.HasValue;

                TrackAdded = config.TrackAdded.Value;
                TrackDeleted = config.TrackDeleted.Value;
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                TrackChanged = !isTag && config.TrackChanged.Value;
                #endif
                TrackAddedOrChanged = config.TrackAdded.Value
                    #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                    || (!isTag && config.TrackChanged.Value)
                    #endif
                ;
                DeletedTrackingOffset = (byte)(isTag ? Const.BLOCKS_IN_SEGMENT : Const.BLOCKS_IN_SEGMENT * 2);

                IsRegistered = true;
                #if FFS_ECS_DEBUG
                _blockerDelete = 0;
                _blockerAdd = 0;
                _blockerDisable = 0;
                _blockerEnable = 0;
                #endif

                _chunkHeuristicWorldMask = null;
                ComponentSegments = null;
                _componentsPool = null;
                EntitiesMaskSegments = null;
                HeuristicChunks = null;
                _segmentsPool = null;
                _chunkHeuristicWorldMaskLen = 0;
                TrackingMaskSegments = null;
                TrackingHeuristicChunks = null;
                _trackingSegmentsPool = null;
                _trackingSegmentsPoolCount = 0;
                TrackingHistoryHeuristic = null;
                TrackingHistoryMasks = null;

                #if FFS_ECS_BURST
                LifecycleHandle = default;
                #endif

                #if FFS_ECS_TRACE
                Utils.Trace($"Registered {TypeName}:\n"
                            + $"IsTag {IsTag}\n"
                            + $"DynamicId {DynamicId}\n"
                            + $"Guid {Guid}\n"
                            + $"Version {Version}\n"
                            + $"Unmanaged {Unmanaged}\n"
                            + $"HasDefaultValue {HasDefaultValue}\n"
                            + $"ReadWriteStrategyType {_readWriteArrayStrategy?.GetType().ToString() ?? "null"}\n"
                            + $"HasOnAdd {HasOnAdd}\n"
                            + $"HasOnDelete {HasOnDelete}\n"
                            + $"HasWrite {HasWrite}\n"
                            + $"HasRead {HasRead}\n"
                            + $"TrackAdded {TrackAdded}\n"
                            + $"TrackDeleted {TrackDeleted}\n"
                            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                            + $"TrackChanged {TrackChanged}\n"
                            #endif
                            + "\n"
                );
                #endif
            }
            
            [MethodImpl(AggressiveInlining)]
            internal void Initialize(uint chunksCapacity, ulong[][] chunkHeuristicMask, ushort chunkHeuristicMaskLen) {
                _chunkHeuristicWorldMask = chunkHeuristicMask;
                _chunkHeuristicWorldMaskLen = chunkHeuristicMaskLen;

                var segmentsCapacity = chunksCapacity * Const.SEGMENTS_IN_CHUNK;

                HeuristicChunks = new HeuristicChunk[chunksCapacity];
                EntitiesMaskSegments = new ulong[segmentsCapacity][];
                _segmentsPool = new ulong[segmentsCapacity][];
                if (!IsTag) {
                    ComponentSegments = new T[segmentsCapacity][];
                    _componentsPool = new T[segmentsCapacity][];
                }

                if (TrackAdded || TrackDeleted
                    #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                    || TrackChanged
                    #endif
                ) {
                    var bufferSize = Data.Instance.TrackingBufferSize;
                    var totalSlots = (uint)bufferSize + 1;
                    var poolSize = bufferSize > 0 ? segmentsCapacity * totalSlots : segmentsCapacity;
                    _trackingSegmentsPool = new ulong[poolSize][];
                    _trackingSegmentsPoolCount = 0;
                    if (bufferSize > 0) {
                        TrackingHistoryHeuristic = new HeuristicComponentsTracking[totalSlots][];
                        TrackingHistoryMasks = new ulong[totalSlots][][];
                        for (var i = 0; i < totalSlots; i++) {
                            TrackingHistoryHeuristic[i] = new HeuristicComponentsTracking[chunksCapacity];
                            TrackingHistoryMasks[i] = new ulong[segmentsCapacity][];
                        }
                        var writeSlot = (int)((Data.Instance.CurrentTick + 1) % totalSlots);
                        TrackingHeuristicChunks = TrackingHistoryHeuristic[writeSlot];
                        TrackingMaskSegments = TrackingHistoryMasks[writeSlot];
                    }
                    else {
                        TrackingMaskSegments = new ulong[segmentsCapacity][];
                        TrackingHeuristicChunks = new HeuristicComponentsTracking[chunksCapacity];
                    }
                }

                if (default(T) is IComponentInternal component) {
                    component.OnInitialize<TWorld>();
                }
                
                #if FFS_ECS_BURST
                LifecycleHandle.OnInitialize(segmentsCapacity);
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal void DestroyInternal() {
                #if FFS_ECS_BURST
                LifecycleHandle.OnDestroy();
                #endif
                this = default;
            }

            [MethodImpl(NoInlining)]
            internal void HardResetInternal() {
                var segCount = EntitiesMaskSegments.Length;
                var hasTracking = TrackAdded || TrackDeleted
                    #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                    || TrackChanged
                    #endif
                    ;

                for (var i = 0; i < segCount; i++) {
                    ref var masks = ref EntitiesMaskSegments[i];
                    if (masks != null) {
                        Array.Clear(masks, 0, IsTag ? Const.BLOCKS_IN_SEGMENT : Const.BLOCKS_IN_SEGMENT << 1);
                        
                        if (!IsTag) {
                            ref var components = ref ComponentSegments[i];
                            if (HasDefaultValue) {
                                components.AsSpan().Fill(DefaultValue);
                            } else {
                                Array.Clear(components, 0, Const.ENTITIES_IN_SEGMENT);
                            }
                            _componentsPool[_segmentsPoolCount] = components;
                            components = null;
                        }
                        _segmentsPool[_segmentsPoolCount] = masks;
                        masks = null;
                        _segmentsPoolCount++;
                        
                        #if FFS_ECS_BURST
                        LifecycleHandle.OnSegmentPooled((uint)i);
                        #endif
                    }

                    if (hasTracking) {
                        var bufferSize = Data.Instance.TrackingBufferSize;
                        if (bufferSize > 0) {
                            for (var slot = 0; slot < bufferSize + 1; slot++) {
                                ref var trackingSeg = ref TrackingHistoryMasks[slot][i];
                                if (trackingSeg != null) {
                                    Array.Clear(trackingSeg, 0, trackingSeg.Length);
                                    _trackingSegmentsPool[_trackingSegmentsPoolCount++] = trackingSeg;
                                    trackingSeg = null;
                                }
                            }
                        }
                        else {
                            ref var trackingSeg = ref TrackingMaskSegments[i];
                            if (trackingSeg != null) {
                                Array.Clear(trackingSeg, 0, trackingSeg.Length);
                                #if FFS_ECS_BURST
                                LifecycleHandle.OnTrackingSegmentPooled((uint)i);
                                #endif
                                _trackingSegmentsPool[_trackingSegmentsPoolCount++] = trackingSeg;
                                trackingSeg = null;
                            }
                        }
                    }
                }

                Array.Clear(HeuristicChunks, 0, HeuristicChunks.Length);
                if (hasTracking) {
                    var bufferSize = Data.Instance.TrackingBufferSize;
                    if (bufferSize > 0) {
                        for (var slot = 0; slot < bufferSize + 1; slot++) {
                            Array.Clear(TrackingHistoryHeuristic[slot], 0, TrackingHistoryHeuristic[slot].Length);
                        }
                        var writeSlot = (int)((Data.Instance.CurrentTick + 1) % (uint)(bufferSize + 1));
                        TrackingHeuristicChunks = TrackingHistoryHeuristic[writeSlot];
                        TrackingMaskSegments = TrackingHistoryMasks[writeSlot];
                    }
                    else {
                        Array.Clear(TrackingHeuristicChunks, 0, TrackingHeuristicChunks.Length);
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void Resize(uint chunksCapacity, ulong[][] chunkHeuristicMask) {
                var segmentsCapacity = chunksCapacity * Const.SEGMENTS_IN_CHUNK;
                
                _chunkHeuristicWorldMask = chunkHeuristicMask;
                Array.Resize(ref HeuristicChunks, (int)chunksCapacity);
                Array.Resize(ref EntitiesMaskSegments, (int)segmentsCapacity);
                Array.Resize(ref _segmentsPool, (int)segmentsCapacity);
                if (!IsTag) {
                    Array.Resize(ref ComponentSegments, (int)segmentsCapacity);
                    Array.Resize(ref _componentsPool, (int)segmentsCapacity);
                }

                if (TrackAdded || TrackDeleted
                    #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                    || TrackChanged
                    #endif
                ) {
                    var bufferSize = Data.Instance.TrackingBufferSize;
                    var totalSlots = (uint)bufferSize + 1;
                    var poolSize = bufferSize > 0 ? segmentsCapacity * totalSlots : segmentsCapacity;
                    Array.Resize(ref _trackingSegmentsPool, (int)poolSize);
                    if (bufferSize > 0) {
                        for (var i = 0; i < totalSlots; i++) {
                            Array.Resize(ref TrackingHistoryHeuristic[i], (int)chunksCapacity);
                            Array.Resize(ref TrackingHistoryMasks[i], (int)segmentsCapacity);
                        }
                        var writeSlot = (int)((Data.Instance.CurrentTick + 1) % totalSlots);
                        TrackingHeuristicChunks = TrackingHistoryHeuristic[writeSlot];
                        TrackingMaskSegments = TrackingHistoryMasks[writeSlot];
                    }
                    else {
                        Array.Resize(ref TrackingMaskSegments, (int)segmentsCapacity);
                        Array.Resize(ref TrackingHeuristicChunks, (int)chunksCapacity);
                    }
                }

                #if FFS_ECS_BURST
                LifecycleHandle.OnResize(segmentsCapacity);
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly void TryToStringComponent(StringBuilder builder, Entity entity) {
                if (Has(entity)) {
                    builder.Append(" - [");
                    builder.Append(DynamicId);
                    builder.Append("] ");
                    if (IsTag) {
                        builder.AppendLine(TypeName);
                    } else {
                        if (HasDisabled(entity)) {
                            builder.Append("[Disabled] ");
                        }
                        builder.Append(TypeName);
                        builder.Append(" ( ");
                        builder.Append(Ref(entity));
                        builder.AppendLine(" )");
                    }
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            public readonly bool TryGet(Entity entity, out T value) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(ComponentsTypeName);
                AssertRegisteredComponent<T>(ComponentsTypeName);
                AssertEntityIsNotDestroyedAndLoaded(ComponentsTypeName, entity);
                #endif

                var entityId = entity.IdWithOffset - Const.ENTITY_ID_OFFSET;
                var segmentIdx = entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT;
                var segmentBlockIdx = (byte) ((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_SEGMENT_MASK);
                var blockEntityMask = 1UL << (byte) (entityId & Const.ENTITIES_IN_BLOCK_MASK);

                var maskSegment = EntitiesMaskSegments[segmentIdx];
                if (maskSegment != null && (maskSegment[segmentBlockIdx] & blockEntityMask) != 0) {
                    value = ComponentSegments[entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT][entityId & Const.ENTITIES_IN_SEGMENT_MASK];
                    return true;
                }

                value = default;
                return false;
            }

            #region TRACKING
            [MethodImpl(NoInlining)]
            private void SetAddedBit(uint entityId, uint segmentIdx, byte segmentBlockIdx, ulong blockEntityMask) {
                ref var seg = ref TrackingMaskSegments[segmentIdx];
                seg ??= AllocateTrackingSegment(segmentIdx);
                seg[segmentBlockIdx] |= blockEntityMask;
                var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                TrackingHeuristicChunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT].AddedBlocks.SetBit(chunkBlockIdx);
            }

            [MethodImpl(NoInlining)]
            private void SetDeletedBit(uint entityId, uint segmentIdx, byte segmentBlockIdx, ulong blockEntityMask) {
                ref var seg = ref TrackingMaskSegments[segmentIdx];
                seg ??= AllocateTrackingSegment(segmentIdx);
                seg[segmentBlockIdx + DeletedTrackingOffset] |= blockEntityMask;
                var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                TrackingHeuristicChunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT].DeletedBlocks.SetBit(chunkBlockIdx);
            }

            [MethodImpl(NoInlining)]
            private void SetOnAddTrackingBits(uint entityId, uint segmentIdx, byte segmentBlockIdx, ulong blockEntityMask) {
                ref var seg = ref TrackingMaskSegments[segmentIdx];
                seg ??= AllocateTrackingSegment(segmentIdx);
                var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                ref var heuristic = ref TrackingHeuristicChunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT];
                if (TrackAdded) {
                    seg[segmentBlockIdx] |= blockEntityMask;
                    heuristic.AddedBlocks.SetBit(chunkBlockIdx);
                }
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                if (TrackChanged) {
                    seg[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] |= blockEntityMask;
                    heuristic.ChangedBlocks.SetBit(chunkBlockIdx);
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            private void SetOnAddTrackingBitsBatch(ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx, byte chunkBlockIdx, uint chunkIdx) {
                ref var seg = ref TrackingMaskSegments[segmentIdx];
                seg ??= AllocateTrackingSegment(segmentIdx);
                ref var heuristic = ref TrackingHeuristicChunks[chunkIdx];
                if (TrackAdded) {
                    seg[segmentBlockIdx] |= entitiesMaskFilter;
                    heuristic.AddedBlocks.SetBit(chunkBlockIdx);
                }
                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                if (TrackChanged) {
                    seg[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] |= entitiesMaskFilter;
                    heuristic.ChangedBlocks.SetBit(chunkBlockIdx);
                }
                #endif
            }

            [MethodImpl(AggressiveInlining)]
            private void SetAddedBitBatch(ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx, byte chunkBlockIdx, uint chunkIdx) {
                ref var seg = ref TrackingMaskSegments[segmentIdx];
                seg ??= AllocateTrackingSegment(segmentIdx);
                seg[segmentBlockIdx] |= entitiesMaskFilter;
                TrackingHeuristicChunks[chunkIdx].AddedBlocks.SetBit(chunkBlockIdx);
            }

            [MethodImpl(AggressiveInlining)]
            private void SetDeletedBitBatch(ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx, byte chunkBlockIdx, uint chunkIdx) {
                ref var seg = ref TrackingMaskSegments[segmentIdx];
                seg ??= AllocateTrackingSegment(segmentIdx);
                seg[segmentBlockIdx + DeletedTrackingOffset] |= entitiesMaskFilter;
                TrackingHeuristicChunks[chunkIdx].DeletedBlocks.SetBit(chunkBlockIdx);
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            [MethodImpl(NoInlining)]
            internal void SetChangedBit(uint entityId, uint segmentIdx, byte segmentBlockIdx, ulong blockEntityMask) {
                ref var seg = ref TrackingMaskSegments[segmentIdx];
                seg ??= AllocateTrackingSegment(segmentIdx);
                seg[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] |= blockEntityMask;
                var chunkBlockIdx = (byte)((entityId >> Const.ENTITIES_IN_BLOCK_SHIFT) & Const.BLOCKS_IN_CHUNK_MASK);
                TrackingHeuristicChunks[entityId >> Const.ENTITIES_IN_CHUNK_SHIFT].ChangedBlocks.SetBit(chunkBlockIdx);
            }

            [MethodImpl(AggressiveInlining)]
            internal void SetChangedBitBatch(ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx, byte chunkBlockIdx, uint chunkIdx) {
                ref var seg = ref TrackingMaskSegments[segmentIdx];
                seg ??= AllocateTrackingSegment(segmentIdx);
                seg[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] |= entitiesMaskFilter;
                TrackingHeuristicChunks[chunkIdx].ChangedBlocks.SetBit(chunkBlockIdx);
            }
            #endif

            [MethodImpl(NoInlining)]
            // ReSharper disable once UnusedParameter.Local
            private ulong[] AllocateTrackingSegment(uint segmentIdx) {
                var trackingBlocksCount = IsTag ? Const.BLOCKS_IN_SEGMENT * 2 : Const.BLOCKS_IN_SEGMENT * 3;
                var poolIdx = Interlocked.Decrement(ref _trackingSegmentsPoolCount);
                if (poolIdx >= 0) {
                    var seg = _trackingSegmentsPool[poolIdx];
                    _trackingSegmentsPool[poolIdx] = null;
                    Array.Clear(seg, 0, trackingBlocksCount);
                    #if FFS_ECS_BURST
                    LifecycleHandle.OnTrackingSegmentCreated(segmentIdx, seg, true);
                    #endif
                    return seg;
                }
                Interlocked.Increment(ref _trackingSegmentsPoolCount);
                var newSeg = new ulong[trackingBlocksCount];
                #if FFS_ECS_BURST
                LifecycleHandle.OnTrackingSegmentCreated(segmentIdx, newSeg, false);
                #endif
                return newSeg;
            }

            [MethodImpl(AggressiveInlining)]
            // ReSharper disable once UnusedParameter.Local
            private void ReturnTrackingSegment(uint segmentIdx, ulong[] seg) {
                #if FFS_ECS_BURST
                LifecycleHandle.OnTrackingSegmentPooled(segmentIdx);
                #endif
                _trackingSegmentsPool[Interlocked.Increment(ref _trackingSegmentsPoolCount) - 1] = seg;
            }

            [MethodImpl(NoInlining)]
            internal void AdvanceTrackingSlot() {
                var bufferSize = Data.Instance.TrackingBufferSize;
                if (bufferSize == 0) return;
                var hasTracking = TrackAdded || TrackDeleted
                    #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                    || TrackChanged
                    #endif
                    ;
                if (!hasTracking) return;

                var newSlot = (int)((Data.Instance.CurrentTick + 1) % (uint)(bufferSize + 1));
                var heuristic = TrackingHistoryHeuristic[newSlot];
                var masks = TrackingHistoryMasks[newSlot];

                for (var chunkIdx = 0; chunkIdx < heuristic.Length; chunkIdx++) {
                    ref var h = ref heuristic[chunkIdx];
                    var notEmpty = h.AddedBlocks.Value | h.DeletedBlocks.Value
                        #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                        | h.ChangedBlocks.Value
                        #endif
                        ;
                    if (notEmpty == 0) continue;
                    h.AddedBlocks.Value = 0;
                    h.DeletedBlocks.Value = 0;
                    #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                    h.ChangedBlocks.Value = 0;
                    #endif
                    var baseSegment = (uint)(chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT);
                    while (notEmpty != 0) {
                        #if NET6_0_OR_GREATER
                        var blockIdx = System.Numerics.BitOperations.TrailingZeroCount(notEmpty);
                        #else
                        var blockIdx = Utils.DeBruijn[(uint)(((notEmpty & (ulong)-(long)notEmpty) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        var segmentOffset = blockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                        var segmentIdx = (uint)(baseSegment + segmentOffset);
                        ref var segRef = ref masks[segmentIdx];
                        if (segRef != null) {
                            ReturnTrackingSegment(segmentIdx, segRef);
                            segRef = null;
                        }
                        notEmpty &= ~(0xFUL << (segmentOffset << Const.BLOCKS_IN_SEGMENT_SHIFT));
                    }
                }

                TrackingHeuristicChunks = heuristic;
                TrackingMaskSegments = masks;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly ulong AddedHeuristicHistory(ulong fromTick, ulong toTick, byte bufferSize, uint chunkIdx) {
                #if FFS_ECS_DEBUG
                AssertTrackingBufferNotOverflow(ComponentsTypeName, fromTick, toTick, bufferSize);
                #endif
                var ticksToCheck = toTick - fromTick;
                if (ticksToCheck == 0) return 0UL;
                if (ticksToCheck > bufferSize) ticksToCheck = bufferSize;
                var totalSlots = (ulong)bufferSize + 1;
                if (ticksToCheck == 1) {
                    return TrackingHistoryHeuristic[(int)(toTick % totalSlots)][chunkIdx].AddedBlocks.Value;
                }
                ulong result = 0;
                for (ulong i = 0; i < ticksToCheck; i++) {
                    result |= TrackingHistoryHeuristic[(int)((toTick - i) % totalSlots)][chunkIdx].AddedBlocks.Value;
                }
                return result;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly ulong DeletedHeuristicHistory(ulong fromTick, ulong toTick, byte bufferSize, uint chunkIdx) {
                #if FFS_ECS_DEBUG
                AssertTrackingBufferNotOverflow(ComponentsTypeName, fromTick, toTick, bufferSize);
                #endif
                var ticksToCheck = toTick - fromTick;
                if (ticksToCheck == 0) return 0UL;
                if (ticksToCheck > bufferSize) ticksToCheck = bufferSize;
                var totalSlots = (ulong)bufferSize + 1;
                if (ticksToCheck == 1) {
                    return TrackingHistoryHeuristic[(int)(toTick % totalSlots)][chunkIdx].DeletedBlocks.Value;
                }
                ulong result = 0;
                for (ulong i = 0; i < ticksToCheck; i++) {
                    result |= TrackingHistoryHeuristic[(int)((toTick - i) % totalSlots)][chunkIdx].DeletedBlocks.Value;
                }
                return result;
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            [MethodImpl(AggressiveInlining)]
            internal readonly ulong ChangedHeuristicHistory(ulong fromTick, ulong toTick, byte bufferSize, uint chunkIdx) {
                #if FFS_ECS_DEBUG
                AssertTrackingBufferNotOverflow(ComponentsTypeName, fromTick, toTick, bufferSize);
                #endif
                var ticksToCheck = toTick - fromTick;
                if (ticksToCheck == 0) return 0UL;
                if (ticksToCheck > bufferSize) ticksToCheck = bufferSize;
                var totalSlots = (ulong)bufferSize + 1;
                if (ticksToCheck == 1) {
                    return TrackingHistoryHeuristic[(int)(toTick % totalSlots)][chunkIdx].ChangedBlocks.Value;
                }
                ulong result = 0;
                for (ulong i = 0; i < ticksToCheck; i++) {
                    result |= TrackingHistoryHeuristic[(int)((toTick - i) % totalSlots)][chunkIdx].ChangedBlocks.Value;
                }
                return result;
            }
            #endif

            [MethodImpl(AggressiveInlining)]
            internal readonly ulong AddedMaskHistory(ulong fromTick, ulong toTick, byte bufferSize, uint segmentIdx, int segmentBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertTrackingBufferNotOverflow(ComponentsTypeName, fromTick, toTick, bufferSize);
                #endif
                var ticksToCheck = toTick - fromTick;
                if (ticksToCheck == 0) return 0UL;
                if (ticksToCheck > bufferSize) ticksToCheck = bufferSize;
                var totalSlots = (ulong)bufferSize + 1;
                if (ticksToCheck == 1) {
                    var m = TrackingHistoryMasks[(int)(toTick % totalSlots)][segmentIdx];
                    return m != null ? m[segmentBlockIdx] : 0UL;
                }
                ulong result = 0;
                for (ulong i = 0; i < ticksToCheck; i++) {
                    var m = TrackingHistoryMasks[(int)((toTick - i) % totalSlots)][segmentIdx];
                    if (m != null) result |= m[segmentBlockIdx];
                }
                return result;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly ulong DeletedMaskHistory(ulong fromTick, ulong toTick, byte bufferSize, uint segmentIdx, int segmentBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertTrackingBufferNotOverflow(ComponentsTypeName, fromTick, toTick, bufferSize);
                #endif
                var ticksToCheck = toTick - fromTick;
                if (ticksToCheck == 0) return 0UL;
                if (ticksToCheck > bufferSize) ticksToCheck = bufferSize;
                var totalSlots = (ulong)bufferSize + 1;
                if (ticksToCheck == 1) {
                    var m = TrackingHistoryMasks[(int)(toTick % totalSlots)][segmentIdx];
                    return m != null ? m[segmentBlockIdx + DeletedTrackingOffset] : 0UL;
                }
                ulong result = 0;
                for (ulong i = 0; i < ticksToCheck; i++) {
                    var m = TrackingHistoryMasks[(int)((toTick - i) % totalSlots)][segmentIdx];
                    if (m != null) result |= m[segmentBlockIdx + DeletedTrackingOffset];
                }
                return result;
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            [MethodImpl(AggressiveInlining)]
            internal readonly ulong ChangedMaskHistory(ulong fromTick, ulong toTick, byte bufferSize, uint segmentIdx, int segmentBlockIdx) {
                #if FFS_ECS_DEBUG
                AssertTrackingBufferNotOverflow(ComponentsTypeName, fromTick, toTick, bufferSize);
                #endif
                var ticksToCheck = toTick - fromTick;
                if (ticksToCheck == 0) return 0UL;
                if (ticksToCheck > bufferSize) ticksToCheck = bufferSize;
                var totalSlots = (ulong)bufferSize + 1;
                if (ticksToCheck == 1) {
                    var m = TrackingHistoryMasks[(int)(toTick % totalSlots)][segmentIdx];
                    return m != null ? m[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] : 0UL;
                }
                ulong result = 0;
                for (ulong i = 0; i < ticksToCheck; i++) {
                    var m = TrackingHistoryMasks[(int)((toTick - i) % totalSlots)][segmentIdx];
                    if (m != null) result |= m[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT];
                }
                return result;
            }
            #endif

            /// <summary>
            /// Resets only the Added tracking masks. Deleted tracking state is preserved. When tick-based tracking is active, clears all ring buffer slots.
            /// Returns tracking segments to pool if Deleted state is also empty.
            /// Must be called from the main thread (not during parallel query execution).
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            internal void ClearAddedTracking() {
                #if FFS_ECS_DEBUG
                AssertMultiThreadNotActive(ComponentsTypeName);
                #endif
                if (!TrackAdded) return;
                var bufferSize = Data.Instance.TrackingBufferSize;
                if (bufferSize > 0) {
                    for (var slot = 0; slot < bufferSize + 1; slot++) {
                        ClearAddedTrackingSlot(TrackingHistoryHeuristic[slot], TrackingHistoryMasks[slot]);
                    }
                }
                else {
                    ClearAddedTrackingSlot(TrackingHeuristicChunks, TrackingMaskSegments);
                }
            }

            [MethodImpl(NoInlining)]
            private void ClearAddedTrackingSlot(HeuristicComponentsTracking[] heuristicChunks, ulong[][] maskSegments) {
                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif
                for (var chunkIdx = 0; chunkIdx < heuristicChunks.Length; chunkIdx++) {
                    ref var heuristic = ref heuristicChunks[chunkIdx];
                    var notEmpty = heuristic.AddedBlocks.Value;
                    if (notEmpty == 0) continue;
                    heuristic.AddedBlocks.Value = 0;
                    var baseSegment = (uint)(chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT);
                    while (notEmpty != 0) {
                        #if NET6_0_OR_GREATER
                        var blockIdx = System.Numerics.BitOperations.TrailingZeroCount(notEmpty);
                        #else
                        var blockIdx = deBruijn[(uint)(((notEmpty & (ulong)-(long)notEmpty) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        var segmentOffset = blockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                        var segmentIdx = (uint)(baseSegment + segmentOffset);
                        ref var segRef = ref maskSegments[segmentIdx];
                        if (segRef != null) {
                            Array.Clear(segRef, 0, Const.BLOCKS_IN_SEGMENT);
                            var segmentMask = 0xFUL << (segmentOffset << Const.BLOCKS_IN_SEGMENT_SHIFT);
                            if ((heuristic.DeletedBlocks.Value & segmentMask) == 0
                                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                                && (heuristic.ChangedBlocks.Value & segmentMask) == 0
                                #endif
                            ) {
                                ReturnTrackingSegment(segmentIdx, segRef);
                                segRef = null;
                            }
                        }
                        notEmpty &= ~(0xFUL << (segmentOffset << Const.BLOCKS_IN_SEGMENT_SHIFT));
                    }
                }
            }

            /// <summary>
            /// Resets only the Deleted tracking masks. Added and Changed tracking state is preserved. When tick-based tracking is active, clears all ring buffer slots.
            /// Returns tracking segments to pool if Added and Changed state is also empty.
            /// Must be called from the main thread (not during parallel query execution).
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            internal void ClearDeletedTracking() {
                #if FFS_ECS_DEBUG
                AssertMultiThreadNotActive(ComponentsTypeName);
                #endif
                if (!TrackDeleted) return;
                var bufferSize = Data.Instance.TrackingBufferSize;
                if (bufferSize > 0) {
                    for (var slot = 0; slot < bufferSize + 1; slot++) {
                        ClearDeletedTrackingSlot(TrackingHistoryHeuristic[slot], TrackingHistoryMasks[slot]);
                    }
                }
                else {
                    ClearDeletedTrackingSlot(TrackingHeuristicChunks, TrackingMaskSegments);
                }
            }

            [MethodImpl(NoInlining)]
            private void ClearDeletedTrackingSlot(HeuristicComponentsTracking[] heuristicChunks, ulong[][] maskSegments) {
                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif
                for (var chunkIdx = 0; chunkIdx < heuristicChunks.Length; chunkIdx++) {
                    ref var heuristic = ref heuristicChunks[chunkIdx];
                    var notEmpty = heuristic.DeletedBlocks.Value;
                    if (notEmpty == 0) continue;
                    heuristic.DeletedBlocks.Value = 0;
                    var baseSegment = (uint)(chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT);
                    while (notEmpty != 0) {
                        #if NET6_0_OR_GREATER
                        var blockIdx = System.Numerics.BitOperations.TrailingZeroCount(notEmpty);
                        #else
                        var blockIdx = deBruijn[(uint)(((notEmpty & (ulong)-(long)notEmpty) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        var segmentOffset = blockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                        var segmentIdx = (uint)(baseSegment + segmentOffset);
                        ref var segRef = ref maskSegments[segmentIdx];
                        if (segRef != null) {
                            Array.Clear(segRef, DeletedTrackingOffset, Const.BLOCKS_IN_SEGMENT);
                            var segmentMask = 0xFUL << (segmentOffset << Const.BLOCKS_IN_SEGMENT_SHIFT);
                            if ((heuristic.AddedBlocks.Value & segmentMask) == 0
                                #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                                && (heuristic.ChangedBlocks.Value & segmentMask) == 0
                                #endif
                            ) {
                                ReturnTrackingSegment(segmentIdx, segRef);
                                segRef = null;
                            }
                        }
                        notEmpty &= ~(0xFUL << (segmentOffset << Const.BLOCKS_IN_SEGMENT_SHIFT));
                    }
                }
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            [MethodImpl(AggressiveInlining)]
            internal void ClearChangedTracking() {
                #if FFS_ECS_DEBUG
                AssertMultiThreadNotActive(ComponentsTypeName);
                #endif
                if (!TrackChanged) return;
                var bufferSize = Data.Instance.TrackingBufferSize;
                if (bufferSize > 0) {
                    for (var slot = 0; slot < bufferSize + 1; slot++) {
                        ClearChangedTrackingSlot(TrackingHistoryHeuristic[slot], TrackingHistoryMasks[slot]);
                    }
                }
                else {
                    ClearChangedTrackingSlot(TrackingHeuristicChunks, TrackingMaskSegments);
                }
            }

            [MethodImpl(NoInlining)]
            private void ClearChangedTrackingSlot(HeuristicComponentsTracking[] heuristicChunks, ulong[][] maskSegments) {
                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif
                for (var chunkIdx = 0; chunkIdx < heuristicChunks.Length; chunkIdx++) {
                    ref var heuristic = ref heuristicChunks[chunkIdx];
                    var notEmpty = heuristic.ChangedBlocks.Value;
                    if (notEmpty == 0) continue;
                    heuristic.ChangedBlocks.Value = 0;
                    var baseSegment = (uint)(chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT);
                    while (notEmpty != 0) {
                        #if NET6_0_OR_GREATER
                        var blockIdx = System.Numerics.BitOperations.TrailingZeroCount(notEmpty);
                        #else
                        var blockIdx = deBruijn[(uint)(((notEmpty & (ulong)-(long)notEmpty) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        var segmentOffset = blockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                        var segmentIdx = (uint)(baseSegment + segmentOffset);
                        ref var segRef = ref maskSegments[segmentIdx];
                        if (segRef != null) {
                            Array.Clear(segRef, Const.BLOCKS_IN_SEGMENT, Const.BLOCKS_IN_SEGMENT);
                            var segmentMask = 0xFUL << (segmentOffset << Const.BLOCKS_IN_SEGMENT_SHIFT);
                            if ((heuristic.AddedBlocks.Value & segmentMask) == 0
                                && (heuristic.DeletedBlocks.Value & segmentMask) == 0) {
                                ReturnTrackingSegment(segmentIdx, segRef);
                                segRef = null;
                            }
                        }
                        notEmpty &= ~(0xFUL << (segmentOffset << Const.BLOCKS_IN_SEGMENT_SHIFT));
                    }
                }
            }
            #endif

            /// <summary>
            /// Resets all tracking bitmasks for this component type. When tick-based tracking is active, clears all ring buffer slots.
            /// After calling, <c>Added&lt;T&gt;</c>, <c>Deleted&lt;T&gt;</c>, and <c>Changed&lt;T&gt;</c> query filters will match no entities
            /// until new Add/Delete/Change operations occur. Tracking segments are returned to the internal pool.
            /// </summary>
            [MethodImpl(AggressiveInlining)]
            internal void ClearTrackingInternal() {
                #if FFS_ECS_DEBUG
                AssertMultiThreadNotActive(ComponentsTypeName);
                #endif
                if (!TrackAdded && !TrackDeleted
                    #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                    && !TrackChanged
                    #endif
                ) return;
                var bufferSize = Data.Instance.TrackingBufferSize;
                if (bufferSize > 0) {
                    for (var slot = 0; slot < bufferSize + 1; slot++) {
                        ClearAllTrackingSlot(TrackingHistoryHeuristic[slot], TrackingHistoryMasks[slot]);
                    }
                }
                else {
                    ClearAllTrackingSlot(TrackingHeuristicChunks, TrackingMaskSegments);
                }
            }

            [MethodImpl(NoInlining)]
            private void ClearAllTrackingSlot(HeuristicComponentsTracking[] heuristicChunks, ulong[][] maskSegments) {
                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif
                for (var chunkIdx = 0; chunkIdx < heuristicChunks.Length; chunkIdx++) {
                    ref var heuristic = ref heuristicChunks[chunkIdx];
                    var notEmpty = heuristic.AddedBlocks.Value | heuristic.DeletedBlocks.Value
                        #if !FFS_ECS_DISABLE_CHANGED_TRACKING
                        | heuristic.ChangedBlocks.Value
                        #endif
                        ;
                    if (notEmpty == 0) continue;
                    heuristic = default;
                    var baseSegment = (uint)(chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT);
                    while (notEmpty != 0) {
                        #if NET6_0_OR_GREATER
                        var blockIdx = System.Numerics.BitOperations.TrailingZeroCount(notEmpty);
                        #else
                        var blockIdx = deBruijn[(uint)(((notEmpty & (ulong)-(long)notEmpty) * 0x37E84A99DAE458FUL) >> 58)];
                        #endif
                        var segmentOffset = blockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT;
                        var segmentIdx = (uint)(baseSegment + segmentOffset);
                        ref var segRef = ref maskSegments[segmentIdx];
                        if (segRef != null) {
                            ReturnTrackingSegment(segmentIdx, segRef);
                            segRef = null;
                        }
                        notEmpty &= ~(0xFUL << (segmentOffset << Const.BLOCKS_IN_SEGMENT_SHIFT));
                    }
                }
            }
            #endregion

            [MethodImpl(NoInlining)]
            private (ulong[], T[]) CreateNewSegment(uint segmentIdx) {
                _chunkHeuristicWorldMask[segmentIdx >> Const.SEGMENTS_IN_CHUNK_SHIFT][(segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK) * _chunkHeuristicWorldMaskLen + _idDiv] |= _idMask;

                T[] comps = null;
                ulong[] masks;
                var poolIdx = Interlocked.Decrement(ref _segmentsPoolCount);
                if (poolIdx >= 0) {
                    while ((masks = Volatile.Read(ref _segmentsPool[poolIdx])) == null) {
                        Thread.SpinWait(1);
                    }
                    if (!IsTag) {
                        comps = _componentsPool[poolIdx];
                        _componentsPool[poolIdx] = null;
                    }

                    _segmentsPool[poolIdx] = null;

                    #if FFS_ECS_BURST
                    LifecycleHandle.OnSegmentCreated(segmentIdx, masks, comps, true);
                    #endif
                    return (masks, comps);
                }

                Interlocked.Increment(ref _segmentsPoolCount);
                var size = Const.BLOCKS_IN_SEGMENT;
                if (!IsTag) {
                    size <<= 1;
                    comps = new T[Const.ENTITIES_IN_SEGMENT];
                    if (HasDefaultValue && DataLifecycle) {
                        comps.AsSpan().Fill(DefaultValue);
                    }
                }
                
                masks = new ulong[size];

                #if FFS_ECS_BURST
                LifecycleHandle.OnSegmentCreated(segmentIdx, masks, comps, false);
                #endif
                return (masks, comps);
            }

            [MethodImpl(NoInlining)]
            private void FreeSegment(uint chunkIdx, uint segmentIdx, ulong[] maskSegment, T[] componentSegment) {
                _chunkHeuristicWorldMask[chunkIdx][(segmentIdx & Const.SEGMENTS_IN_CHUNK_MASK) * _chunkHeuristicWorldMaskLen + _idDiv] &= _idMaskInv;

                EntitiesMaskSegments[segmentIdx] = null;
                var poolIdx = Interlocked.Increment(ref _segmentsPoolCount) - 1;
                if (!IsTag) {
                    ComponentSegments[segmentIdx] = null;
                    _componentsPool[poolIdx] = componentSegment;
                }
                Volatile.Write(ref _segmentsPool[poolIdx], maskSegment);

                #if FFS_ECS_BURST
                LifecycleHandle.OnSegmentPooled(segmentIdx);
                #endif
            }

            #region QUERY
            [MethodImpl(AggressiveInlining)]
            internal readonly ulong EnabledMask(uint segmentIdx, int segmentBlockIdx) {
                var masks = EntitiesMaskSegments[segmentIdx];
                if (masks == null) return 0UL;
                return IsTag 
                    ? masks[segmentBlockIdx] 
                    : masks[segmentBlockIdx] & ~masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT];
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly ulong DisabledMask(uint segmentIdx, int segmentBlockIdx) {
                if (IsTag) return 0UL;
                var masks = EntitiesMaskSegments[segmentIdx];
                return masks != null ? masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] : 0UL;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly ulong AddedMask(uint segmentIdx, int segmentBlockIdx) {
                var masks = TrackingMaskSegments[segmentIdx];
                return masks != null ? masks[segmentBlockIdx] : 0UL;
            }

            [MethodImpl(AggressiveInlining)]
            internal readonly ulong DeletedMask(uint segmentIdx, int segmentBlockIdx) {
                var masks = TrackingMaskSegments[segmentIdx];
                return masks != null ? masks[segmentBlockIdx + DeletedTrackingOffset] : 0UL;
            }

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            [MethodImpl(AggressiveInlining)]
            internal readonly ulong ChangedMask(uint segmentIdx, int segmentBlockIdx) {
                var masks = TrackingMaskSegments[segmentIdx];
                return masks != null ? masks[segmentBlockIdx + Const.BLOCKS_IN_SEGMENT] : 0UL;
            }
            #endif

            [MethodImpl(AggressiveInlining)]
            internal readonly ulong AnyMask(uint segmentIdx, int segmentBlockIdx) {
                var masks = EntitiesMaskSegments[segmentIdx];
                return masks != null ? masks[segmentBlockIdx] : 0UL;
            }

            [MethodImpl(AggressiveInlining)]
            internal void PushQueryDataForDelete(QueryData queryData) {
                _queriesToUpdateOnDelete[_queriesToUpdateOnDeleteCount++] = queryData;
            }

            internal void PopQueryDataForDelete() {
                _queriesToUpdateOnDelete[--_queriesToUpdateOnDeleteCount] = default;
            }

            [MethodImpl(AggressiveInlining)]
            internal void PushQueryDataForDeleteDisable(QueryData queryData) {
                _queriesToUpdateOnDelete[_queriesToUpdateOnDeleteCount++] = queryData;
                if (!IsTag) {
                    _queriesToUpdateOnDisable[_queriesToUpdateOnDisableCount++] = queryData;
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void PopQueryDataForDeleteDisable() {
                _queriesToUpdateOnDelete[--_queriesToUpdateOnDeleteCount] = default;
                if (!IsTag) {
                    _queriesToUpdateOnDisable[--_queriesToUpdateOnDisableCount] = default;
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void PushQueryDataForDeleteEnable(QueryData queryData) {
                _queriesToUpdateOnDelete[_queriesToUpdateOnDeleteCount++] = queryData;
                if (!IsTag) {
                    _queriesToUpdateOnEnable[_queriesToUpdateOnEnableCount++] = queryData;
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void PopQueryDataForDeleteEnable() {
                _queriesToUpdateOnDelete[--_queriesToUpdateOnDeleteCount] = default;
                if (!IsTag) {
                    _queriesToUpdateOnEnable[--_queriesToUpdateOnEnableCount] = default;
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void PushQueryDataForAdd(QueryData queryData) {
                _queriesToUpdateOnAdd[_queriesToUpdateOnAddCount++] = queryData;
            }

            [MethodImpl(AggressiveInlining)]
            internal void PopQueryDataForAdd() {
                _queriesToUpdateOnAdd[--_queriesToUpdateOnAddCount] = default;
            }

            [MethodImpl(AggressiveInlining)]
            internal void PushQueryDataForAddEnable(QueryData queryData) {
                _queriesToUpdateOnAdd[_queriesToUpdateOnAddCount++] = queryData;
                if (!IsTag) {
                    _queriesToUpdateOnEnable[_queriesToUpdateOnEnableCount++] = queryData;
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void PopQueryDataForAddEnable() {
                _queriesToUpdateOnAdd[--_queriesToUpdateOnAddCount] = default;
                if (!IsTag) {
                    _queriesToUpdateOnEnable[--_queriesToUpdateOnEnableCount] = default;
                }
            }
            
            #if FFS_ECS_DEBUG
            [MethodImpl(AggressiveInlining)]
            internal void BlockDelete(int val) {
                _blockerDelete += val;
            }

            [MethodImpl(AggressiveInlining)]
            internal void BlockDeleteDisable(int val) {
                _blockerDelete += val;
                if (!IsTag) _blockerDisable += val;
            }

            [MethodImpl(AggressiveInlining)]
            internal void BlockDeleteEnable(int val) {
                _blockerDelete += val;
                if (!IsTag) _blockerEnable += val;
            }

            [MethodImpl(AggressiveInlining)]
            internal void BlockAdd(int val) {
                _blockerAdd += val;
            }

            [MethodImpl(AggressiveInlining)]
            internal void BlockAddEnable(int val) {
                _blockerAdd += val;
                if (!IsTag) _blockerEnable += val;
            }
            #endif
            #endregion

            #region SERIALIZATION
            [MethodImpl(AggressiveInlining)]
            internal void WriteChunk(ref BinaryPackWriter writer, uint chunkIdx) {
                writer.WriteBool(IsTag);
                
                ref var heuristic = ref HeuristicChunks[chunkIdx];
                var notEmptyBlocks = heuristic.NotEmptyBlocks.Value;
                writer.WriteUlong(notEmptyBlocks);
                
                if (notEmptyBlocks != 0) {
                    #if !NET6_0_OR_GREATER
                    var deBruijn = Utils.DeBruijn;
                    #endif

                    writer.WriteUlong(heuristic.FullBlocks.Value);
                    if (IsTag) {
                        var segmentBase = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;
                        for (var segment = 0; segment < Const.SEGMENTS_IN_CHUNK; segment++) {
                            if (((notEmptyBlocks >> (segment << Const.BLOCKS_IN_SEGMENT_SHIFT)) & 0b1111UL) != 0) {
                                var masks = EntitiesMaskSegments[segmentBase + segment];
                                writer.WriteUlong(masks[0]);
                                writer.WriteUlong(masks[1]);
                                writer.WriteUlong(masks[2]);
                                writer.WriteUlong(masks[3]);
                            }
                        }
                    }
                    else {
                        var unmanagedStrategy = _readWriteArrayStrategy.IsUnmanaged();

                        writer.WriteByte(Version);
                        writer.WriteBool(unmanagedStrategy);

                        _resettableStrategy?.Reset();

                        var entity = new Entity();
                        ref var entityId = ref entity.IdWithOffset;
                        var segmentIdx = uint.MaxValue;
                        ulong[] masks = null;
                        T[] components = null;
                        while (notEmptyBlocks != 0) {
                            #if NET6_0_OR_GREATER
                            var chunkBlockIdx = (uint)System.Numerics.BitOperations.TrailingZeroCount(notEmptyBlocks);
                            #else
                            var chunkBlockIdx = (uint)deBruijn[(uint)(((notEmptyBlocks & (ulong)-(long)notEmptyBlocks) * 0x37E84A99DAE458FUL) >> 58)];
                            #endif
                            notEmptyBlocks &= notEmptyBlocks - 1;
                            var blockIdx = (byte)(chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);
                            var curSegmentIdx = (chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT) + (chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT);
                            if (curSegmentIdx != segmentIdx) {
                                segmentIdx = curSegmentIdx;
                                masks = EntitiesMaskSegments[segmentIdx];
                                components = ComponentSegments[segmentIdx];
                            }

                            var entitiesMask = masks![blockIdx];

                            writer.WriteUlong(entitiesMask);
                            writer.WriteUlong(masks![blockIdx + Const.BLOCKS_IN_SEGMENT]);

                            var chunkBlockEntityId = (uint)((segmentIdx << Const.ENTITIES_IN_SEGMENT_SHIFT) +
                                                            (blockIdx << Const.ENTITIES_IN_BLOCK_SHIFT));
                            var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            if (unmanagedStrategy) {
                                #if NET6_0_OR_GREATER
                                var firstBit = (byte)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                                #else
                                var firstBit = deBruijn[(uint)(((entitiesMask & (ulong)-(long)entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                                #endif
                                var lastBit = (byte)Utils.Msb(entitiesMask);
                                var rangeCount = lastBit - firstBit + 1;
                                writer.WriteByte(firstBit);
                                writer.WriteByte((byte)rangeCount);
                                _readWriteArrayStrategy.WriteArray(ref writer, components, (int)(componentOffset + firstBit), rangeCount);
                            }
                            else {
                                #if FFS_ECS_DEBUG
                                if (!HasWrite) throw new StaticEcsException($"Method Write not implemented for component type {typeof(T)}");
                                #endif

                                do {
                                    var isolatedBit = entitiesMask & (ulong)-(long)entitiesMask;
                                    #if NET6_0_OR_GREATER
                                    var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                                    #else
                                    var runStart = deBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                                    #endif

                                    var componentIdx = runStart + componentOffset;
                                    entityId = chunkBlockEntityId + runStart;

                                    do {
                                        components[componentIdx].Write(ref writer, entity);
                                        isolatedBit <<= 1;
                                        componentIdx++;
                                        entityId++;
                                    } while ((entitiesMask & isolatedBit) != 0);

                                    entitiesMask &= ~(isolatedBit - 1);
                                } while (entitiesMask != 0);
                            }
                        }
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal void ReadChunk(ref BinaryPackReader reader, uint chunkIdx) {
                var isTag = reader.ReadBool();
                #if FFS_ECS_DEBUG
                if (isTag != IsTag) throw new StaticEcsException($"isTag != IsTag");
                #endif
                
                ref var heuristic = ref HeuristicChunks[chunkIdx];
                var notEmptyBlocks = reader.ReadUlong();
                heuristic.NotEmptyBlocks.Value = notEmptyBlocks;
                
                if (notEmptyBlocks != 0) {
                    #if !NET6_0_OR_GREATER
                    var deBruijn = Utils.DeBruijn;
                    #endif
                    
                    heuristic.FullBlocks.Value = reader.ReadUlong();
                    if (IsTag) {
                        var segmentBase = chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT;
                        for (var segment = 0; segment < Const.SEGMENTS_IN_CHUNK; segment++) {
                            if (((notEmptyBlocks >> (segment << Const.BLOCKS_IN_SEGMENT_SHIFT)) & 0b1111UL) != 0) {
                                var segmentIdx = segmentBase + segment;
                                ref var masks = ref EntitiesMaskSegments[segmentIdx];
                                masks ??= CreateNewSegment((uint)segmentIdx).Item1;
                                masks[0] = reader.ReadUlong();
                                masks[1] = reader.ReadUlong();
                                masks[2] = reader.ReadUlong();
                                masks[3] = reader.ReadUlong();
                            }
                        }
                    }
                    else {
                        var oldVersion = reader.ReadByte();
                        var unmanagedStrategy = reader.ReadBool();

                        _resettableStrategy?.Reset();

                        var segmentIdx = uint.MaxValue;
                        ulong[] masks = null;
                        T[] components = null;

                        var entity = new Entity();
                        ref var entityId = ref entity.IdWithOffset;
                        while (notEmptyBlocks != 0) {
                            #if NET6_0_OR_GREATER
                            var chunkBlockIdx = (uint)System.Numerics.BitOperations.TrailingZeroCount(notEmptyBlocks);
                            #else
                            var chunkBlockIdx = (uint)deBruijn[(uint)(((notEmptyBlocks & (ulong)-(long)notEmptyBlocks) * 0x37E84A99DAE458FUL) >> 58)];
                            #endif
                            notEmptyBlocks &= notEmptyBlocks - 1;
                            var blockIdx = (byte)(chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);
                            var curSegmentIdx = (chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT) + (chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT);
                            if (curSegmentIdx != segmentIdx) {
                                segmentIdx = curSegmentIdx;

                                masks = EntitiesMaskSegments[segmentIdx];
                                if (masks == null) {
                                    (masks, ComponentSegments[segmentIdx]) = CreateNewSegment(segmentIdx);
                                    EntitiesMaskSegments[segmentIdx] = masks;
                                }

                                components = ComponentSegments[segmentIdx];
                            }

                            var entitiesMask = reader.ReadUlong();
                            var disabledEntitiesMask = reader.ReadUlong();
                            masks![blockIdx] = entitiesMask;
                            masks[blockIdx + Const.BLOCKS_IN_SEGMENT] = disabledEntitiesMask;

                            var chunkBlockEntityId = (uint)((segmentIdx << Const.ENTITIES_IN_SEGMENT_SHIFT) +
                                                            (blockIdx << Const.ENTITIES_IN_BLOCK_SHIFT));
                            var componentOffset = chunkBlockEntityId & Const.ENTITIES_IN_SEGMENT_MASK;
                            chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                            if (unmanagedStrategy) {
                                var firstBit = reader.ReadByte();
                                var rangeCount = (int)reader.ReadByte();
                                if (oldVersion == Version) {
                                    _readWriteArrayStrategy.ReadArray(ref reader, ref components, (int)(componentOffset + firstBit));
                                }
                                else {
                                    #if FFS_ECS_DEBUG
                                    if (!HasRead) throw new StaticEcsException($"Method Read not implemented for component type {typeof(T)}");
                                    #endif
                                    _ = reader.ReadNullFlag();
                                    _ = reader.ReadInt(); // count
                                    var byteSize = reader.ReadUint();
                                    var oneSize = byteSize / (uint)rangeCount;
                                    for (var cur = 0; cur < rangeCount; cur++) {
                                        var bitIdx = firstBit + cur;
                                        entityId = (uint)(chunkBlockEntityId + bitIdx);
                                        var mask = 1UL << bitIdx;
                                        if ((entitiesMask & mask) != 0) {
                                            var disabled = (disabledEntitiesMask & mask) != 0;
                                            components[componentOffset + bitIdx].Read(ref reader, entity, oldVersion, disabled);
                                        }
                                        else {
                                            reader.SkipNext(oneSize);
                                        }
                                    }
                                }
                            }
                            else {
                                #if FFS_ECS_DEBUG
                                if (!HasRead) throw new StaticEcsException($"Method Read not implemented for component type {typeof(T)}");
                                #endif

                                do {
                                    var isolatedBit = entitiesMask & (ulong)-(long)entitiesMask;
                                    #if NET6_0_OR_GREATER
                                    var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                                    #else
                                    var runStart = deBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                                    #endif

                                    var componentIdx = runStart + componentOffset;
                                    entityId = chunkBlockEntityId + runStart;

                                    do {
                                        var disabled = (disabledEntitiesMask & isolatedBit) != 0;
                                        components[componentIdx].Read(ref reader, entity, oldVersion, disabled);
                                        isolatedBit <<= 1;
                                        componentIdx++;
                                        entityId++;
                                    } while ((entitiesMask & isolatedBit) != 0);

                                    entitiesMask &= ~(isolatedBit - 1);
                                } while (entitiesMask != 0);
                            }
                        }
                    }
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal bool WriteEntity(ref BinaryPackWriter writer, Entity entity, bool deleteComponent) {
                #if FFS_ECS_DEBUG
                if (!IsTag && !HasWrite) throw new StaticEcsException($"Method Write not implemented for component type {typeof(T)}");
                #endif
                if (!Has(entity)) {
                    return false;
                }
                
                writer.WriteBool(IsTag);
                if (IsTag) {
                    return true;
                }
                
                var offset = writer.MakePoint(sizeof(ushort));
                writer.WriteByte(Version);
                Ref(entity).Write(ref writer, entity);
                var size = writer.Position - (offset + sizeof(short));
                #if FFS_ECS_DEBUG
                if (size > short.MaxValue) throw new StaticEcsException($"Size of component {typeof(T)} more than {short.MaxValue} bytes");
                #endif
                var disabled = HasDisabled(entity);
                if (disabled) {
                    writer.WriteUshortAt(offset, (ushort)(size | ComponentSerializerUtils.DisabledBit));
                }
                else {
                    writer.WriteUshortAt(offset, (ushort)size);
                }

                if (deleteComponent) {
                    Delete(entity);
                }

                return true;
            }

            [MethodImpl(AggressiveInlining)]
            internal void ReadEntity(ref BinaryPackReader reader, Entity entity) {
                #if FFS_ECS_DEBUG
                if (!IsTag && !HasRead) throw new StaticEcsException($"Method Read not implemented for component type {typeof(T)}");
                #endif

                _ = reader.ReadBool(); // isTag
                if (IsTag) {
                    Set(entity);
                    return;
                }

                var disabled = (reader.ReadUshort() & ComponentSerializerUtils.DisabledBit) == ComponentSerializerUtils.DisabledBit;
                var oldVersion = reader.ReadByte();

                var component = default(T);
                component.Read(ref reader, entity, oldVersion, disabled);
                Set(entity, component, false);

                if (disabled) {
                    Disable(entity);
                }
            }
            #endregion

            #region HANDLE
            [MethodImpl(AggressiveInlining)]
            internal static void _Initialize(uint chunksCapacity, ulong[][] chunkHeuristicMask, ushort chunkHeuristicMaskLen) => Instance.Initialize(chunksCapacity, chunkHeuristicMask, chunkHeuristicMaskLen);

            [MethodImpl(AggressiveInlining)]
            internal static void _Resize(uint chunksCapacity, ulong[][] chunkHeuristicMask) => Instance.Resize(chunksCapacity, chunkHeuristicMask);

            [MethodImpl(AggressiveInlining)]
            internal static void _Destroy() => Instance.DestroyInternal();

            [MethodImpl(AggressiveInlining)]
            internal static void _HardReset() => Instance.HardResetInternal();

            [MethodImpl(AggressiveInlining)]
            internal static void _TryToStringComponent(StringBuilder builder, uint eid) => Instance.TryToStringComponent(builder, new Entity(eid));

            [MethodImpl(AggressiveInlining)]
            internal static void _WriteChunk(ref BinaryPackWriter writer, uint chunkIdx) => Instance.WriteChunk(ref writer, chunkIdx);

            [MethodImpl(AggressiveInlining)]
            internal static void _ReadChunk(ref BinaryPackReader reader, uint chunkIdx) => Instance.ReadChunk(ref reader, chunkIdx);

            [MethodImpl(AggressiveInlining)]
            internal static bool _WriteEntity(ref BinaryPackWriter writer, uint eid, bool deleteComponent) => Instance.WriteEntity(ref writer, new Entity(eid), deleteComponent);

            [MethodImpl(AggressiveInlining)]
            internal static void _ReadEntity(ref BinaryPackReader reader, uint eid) => Instance.ReadEntity(ref reader, new Entity(eid));

            [MethodImpl(AggressiveInlining)]
            internal static HeuristicChunk[] _HeuristicChunks() => Instance.HeuristicChunks;

            [MethodImpl(AggressiveInlining)]
            internal static Array _ComponentsSegments() => Instance.ComponentSegments;

            [MethodImpl(AggressiveInlining)]
            internal static bool _IsSegmentAllocated(uint segmentIdx) => segmentIdx < Instance.EntitiesMaskSegments.Length && Instance.EntitiesMaskSegments[segmentIdx] != null;

            [MethodImpl(AggressiveInlining)]
            internal static ulong _EnabledMask(uint segmentIdx, int blockIdx) => Instance.EnabledMask(segmentIdx, blockIdx);

            [MethodImpl(AggressiveInlining)]
            internal static ulong _DisabledMask(uint segmentIdx, int blockIdx) => Instance.DisabledMask(segmentIdx, blockIdx);

            [MethodImpl(AggressiveInlining)]
            internal static ulong _AnyMask(uint segmentIdx, int blockIdx) => Instance.AnyMask(segmentIdx, blockIdx);

            [MethodImpl(AggressiveInlining)]
            internal static void _BatchDelete(ulong entitiesMaskFilter, uint segmentIdx, byte segmentBlockIdx, HookReason reason) {
                if (Instance.IsTag) {
                    Instance.BatchDeleteTag(entitiesMaskFilter, segmentIdx, segmentBlockIdx);
                }
                else {
                    Instance.BatchDelete(entitiesMaskFilter, segmentIdx, segmentBlockIdx, reason);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal static void _AdvanceTracking() => Instance.AdvanceTrackingSlot();

            [MethodImpl(AggressiveInlining)]
            internal static void _ClearTracking() => Instance.ClearTrackingInternal();

            [MethodImpl(AggressiveInlining)]
            internal static void _ClearAddedTracking() => Instance.ClearAddedTracking();

            [MethodImpl(AggressiveInlining)]
            internal static void _ClearDeletedTracking() => Instance.ClearDeletedTracking();

            #if !FFS_ECS_DISABLE_CHANGED_TRACKING
            [MethodImpl(AggressiveInlining)]
            internal static void _ClearChangedTracking() => Instance.ClearChangedTracking();
            #endif

            [MethodImpl(AggressiveInlining)]
            internal static int _CalculateCapacity() => Instance.CalculateCapacity();

            [MethodImpl(AggressiveInlining)]
            internal static uint _CalculateCount() => Instance.CalculateCount();

            [MethodImpl(AggressiveInlining)]
            internal static bool _TryGetRaw(uint eid, out IComponentOrTag value) {
                var entity = new Entity(eid);
                if (Instance.Has(entity)) {
                    value = Instance.IsTag ? default : Instance.Ref(entity);
                    return true;
                }

                value = default;
                return false;
            }

            [MethodImpl(AggressiveInlining)]
            internal static void _SetRaw(uint eid, IComponentOrTag value) {
                if (Instance.IsTag) {
                    Instance.Set(new Entity(eid));
                } else {
                    Instance.Set(new Entity(eid), (T)value);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal static void _SetRawDirect(uint eid, IComponentOrTag value) {
                if (!Instance.IsTag) {
                    var entityId = eid;
                    Instance.ComponentSegments[entityId >> Const.ENTITIES_IN_SEGMENT_SHIFT]
                        [entityId & Const.ENTITIES_IN_SEGMENT_MASK] = (T) value;
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal static void _Set(uint eid) {
                if (Instance.IsTag) {
                    Instance.Set(new Entity(eid));
                }
                else {
                    Instance.Set(new Entity(eid), default);
                }
            }

            [MethodImpl(AggressiveInlining)]
            internal static bool _Add(uint eid) {
                Instance.Add(new Entity(eid), out var added);
                return added;
            }

            [MethodImpl(AggressiveInlining)]
            internal static bool _Has(uint eid) => Instance.Has(new Entity(eid));

            [MethodImpl(AggressiveInlining)]
            internal static bool _HasEnabled(uint eid) => Instance.HasEnabled(new Entity(eid));

            [MethodImpl(AggressiveInlining)]
            internal static bool _HasDisabled(uint eid) => Instance.HasDisabled(new Entity(eid));

            [MethodImpl(AggressiveInlining)]
            internal static ToggleResult _Enable(uint eid) => Instance.Enable(new Entity(eid));

            [MethodImpl(AggressiveInlining)]
            internal static ToggleResult _Disable(uint eid) => Instance.Disable(new Entity(eid));

            [MethodImpl(AggressiveInlining)]
            internal static bool _Delete(uint eid, HookReason reason) => Instance.Delete(new Entity(eid), reason);

            [MethodImpl(AggressiveInlining)]
            internal static bool _Copy(uint srcEid, uint dstEid) => Instance.Copy(new Entity(srcEid), new Entity(dstEid));

            [MethodImpl(AggressiveInlining)]
            internal static bool _Move(uint srcEid, uint dstEid) => Instance.Move(new Entity(srcEid), new Entity(dstEid));

            [MethodImpl(AggressiveInlining)]
            internal static IComponentOrTag _DefaultValue() => default(T);
            #endregion
            #endregion
        }
    }

    #region SERIALIZER UTILS
    /// <summary>
    /// Delegate for handling deserialization of a component type that has been removed from the codebase.
    /// When the serializer encounters a component Guid that no longer maps to a registered type,
    /// this delegate is invoked to read the saved data (so the binary stream stays aligned)
    /// and optionally migrate it to a replacement component on the entity.
    /// <para>
    /// Register migration handlers via the world serializer configuration to handle
    /// backward-compatible loading of save files after component types are deleted or replaced.
    /// </para>
    /// </summary>
    /// <typeparam name="TWorld">World type.</typeparam>
    /// <param name="reader">Binary reader positioned at the start of the component's serialized data.</param>
    /// <param name="entity">The entity this component was saved on.</param>
    /// <param name="version">The schema version byte that was stored during serialization.</param>
    /// <param name="disabled">Whether the component was in the disabled state when serialized.</param>
    public delegate void EcsComponentDeleteMigrationReader<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity entity, byte version, bool disabled)
        where TWorld : struct, IWorldType;
    
    internal static class ComponentSerializerUtils {
        internal const int DisabledBit = 0b_10000000_00000000;
        internal const int DisabledBitInv = ~DisabledBit;
        
        [MethodImpl(AggressiveInlining)]
        internal static void SkipOneComponent(this ref BinaryPackReader reader) {
            var isTag = reader.ReadBool();
            if (!isTag) {
                var size = reader.ReadUshort() & DisabledBitInv;
                reader.SkipNext((uint) size); 
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        internal static void DeleteOneComponentMigration<TWorld>(this ref BinaryPackReader reader, World<TWorld>.Entity entity, EcsComponentDeleteMigrationReader<TWorld> migration) 
            where TWorld : struct, IWorldType {
            var isTag = reader.ReadBool();
            if (!isTag) {
                var disabled = (reader.ReadUshort() & DisabledBit) == DisabledBit;
                var oldVersion = reader.ReadByte();
                migration(ref reader, entity, oldVersion, disabled);
            }
            else {
                migration(ref reader, entity, 0, false);
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        internal static void DeleteAllComponentMigration<TWorld>(this ref BinaryPackReader reader, EcsComponentDeleteMigrationReader<TWorld> migration, uint chunkIdx) 
            where TWorld : struct, IWorldType {
            var isTag = reader.ReadBool();
            var notEmptyBlocks = reader.ReadUlong();

            if (notEmptyBlocks != 0) {
                #if !NET6_0_OR_GREATER
                var deBruijn = Utils.DeBruijn;
                #endif

                reader.ReadUlong(); // FullBlocks;
                
                var entity = new World<TWorld>.Entity();
                ref var entityId = ref entity.IdWithOffset;
                
                if (isTag) {
                    for (var segment = 0; segment < Const.SEGMENTS_IN_CHUNK; segment++) {
                        var segmentShift = segment << Const.BLOCKS_IN_SEGMENT_SHIFT;
                        var segmentMask = (notEmptyBlocks >> segmentShift) & 0b1111UL;

                        if (segmentMask == 0) continue;

                        var segmentBlockEntityBase = (uint)((chunkIdx << Const.ENTITIES_IN_CHUNK_SHIFT) + (segmentShift << Const.ENTITIES_IN_BLOCK_SHIFT) + Const.ENTITY_ID_OFFSET);

                        for (var blockIdx = 0; blockIdx < Const.BLOCKS_IN_SEGMENT; blockIdx++) {
                            var entitiesMask = reader.ReadUlong();
                            if (entitiesMask == 0) continue;

                            do {
                                var isolatedBit = entitiesMask & (ulong)-(long)entitiesMask;
                                #if NET6_0_OR_GREATER
                                var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                                #else
                                var runStart = deBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                                #endif

                                entityId = (uint)(segmentBlockEntityBase + (blockIdx << Const.ENTITIES_IN_BLOCK_SHIFT) + runStart);

                                do {
                                    migration(ref reader, entity, 0, false);
                                    isolatedBit <<= 1;
                                    entityId++;
                                } while ((entitiesMask & isolatedBit) != 0);

                                entitiesMask &= ~(isolatedBit - 1);
                            } while (entitiesMask != 0);
                        }
                    }
                    return;
                }
                
                var oldVersion = reader.ReadByte();
                var unmanagedStrategy = reader.ReadBool();

                while (notEmptyBlocks != 0) {
                    #if NET6_0_OR_GREATER
                    var chunkBlockIdx = (uint)System.Numerics.BitOperations.TrailingZeroCount(notEmptyBlocks);
                    #else
                    var chunkBlockIdx = (uint)deBruijn[(uint)(((notEmptyBlocks & (ulong)-(long)notEmptyBlocks) * 0x37E84A99DAE458FUL) >> 58)];
                    #endif
                    notEmptyBlocks &= notEmptyBlocks - 1;
                    var blockIdx = (byte)(chunkBlockIdx & Const.BLOCKS_IN_SEGMENT_MASK);
                    var segmentIdx = (chunkIdx << Const.SEGMENTS_IN_CHUNK_SHIFT) + (chunkBlockIdx >> Const.BLOCKS_IN_SEGMENT_SHIFT);
                    var entitiesMask = reader.ReadUlong();
                    var disabledEntitiesMask = reader.ReadUlong();

                    var chunkBlockEntityId = (uint)((segmentIdx << Const.ENTITIES_IN_SEGMENT_SHIFT) +
                                                    (blockIdx << Const.ENTITIES_IN_BLOCK_SHIFT));
                    chunkBlockEntityId += Const.ENTITY_ID_OFFSET;

                    if (unmanagedStrategy) {
                        var firstBit = reader.ReadByte();
                        var rangeCount = (int)reader.ReadByte();
                        _ = reader.ReadNullFlag();
                        _ = reader.ReadInt(); // count
                        var byteSize = reader.ReadUint();
                        var oneSize = byteSize / (uint)rangeCount;
                        for (var cur = 0; cur < rangeCount; cur++) {
                            var bitIdx = firstBit + cur;
                            entityId = (uint)(chunkBlockEntityId + bitIdx);
                            var mask = 1UL << bitIdx;
                            if ((entitiesMask & mask) != 0) {
                                var disabled = (disabledEntitiesMask & mask) != 0;
                                migration(ref reader, entity, oldVersion, disabled);
                            }
                            else {
                                reader.SkipNext(oneSize);
                            }
                        }
                    }
                    else {
                        do {
                            var isolatedBit = entitiesMask & (ulong)-(long)entitiesMask;
                            #if NET6_0_OR_GREATER
                            var runStart = (byte)System.Numerics.BitOperations.TrailingZeroCount(entitiesMask);
                            #else
                            var runStart = deBruijn[(uint)((isolatedBit * 0x37E84A99DAE458FUL) >> 58)];
                            #endif

                            entityId = chunkBlockEntityId + runStart;

                            do {
                                var disabled = (disabledEntitiesMask & isolatedBit) != 0;
                                migration(ref reader, entity, oldVersion, disabled);
                                isolatedBit <<= 1;
                                entityId++;
                            } while ((entitiesMask & isolatedBit) != 0);

                            entitiesMask &= ~(isolatedBit - 1);
                        } while (entitiesMask != 0);
                    }
                }
            }
        }
    }
    #endregion
    
    #region REFLECTION UTILS
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    [Il2CppEagerStaticClassConstruction]
    #endif
    internal static class ComponentType<
        #if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
        #endif
        T> where T : struct, IComponentOrTag {
        private static readonly Type[] OnAddParams = {
            typeof(World<>.Entity)
        };
        private static readonly Type[] OnDeleteParams = {
            typeof(World<>.Entity),
            typeof(HookReason)
        };
        private static readonly Type[] CopyToParams = {
            typeof(World<>.Entity),
            typeof(World<>.Entity),
            typeof(bool)
        };
        private static readonly Type[] WriteParams = {
            typeof(BinaryPackWriter).MakeByRefType(),
            typeof(World<>.Entity)
        };
        private static readonly Type[] ReadParams = {
            typeof(BinaryPackReader).MakeByRefType(),
            typeof(World<>.Entity),
            typeof(byte),
            typeof(bool)
        };

        internal static bool HasOnAdd() {
            return HasMethod(typeof(T), nameof(IComponentOrTag.OnAdd), OnAddParams);
        }
        
        internal static bool HasOnDelete() {
            return HasMethod(typeof(T), nameof(IComponentOrTag.OnDelete), OnDeleteParams);
        }
        
        internal static bool HasCopyTo() {
            return HasMethod(typeof(T), nameof(IComponentOrTag.CopyTo), CopyToParams);
        }
        
        internal static bool HasWrite() {
            return HasMethod(typeof(T), nameof(IComponentOrTag.Write), WriteParams);
        }
        
        internal static bool HasRead() {
            return HasMethod(typeof(T), nameof(IComponentOrTag.Read), ReadParams);
        }
        
        private static bool HasMethod(
            #if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
            #endif
            Type structType, string methodName, Type[] parameterTypes) {
            var methods = structType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var methodInfo in methods) {
                if (methodInfo.Name == methodName && methodInfo.IsGenericMethodDefinition) {
                    var parameters = methodInfo.GetParameters();
                    if (parameters.Length == parameterTypes.Length) {
                        var match = true;
                        for (var i = 0; i < parameters.Length; i++) {
                            if (parameterTypes[i].Name != parameters[i].ParameterType.Name) {
                                match = false;
                                break;
                            }
                        }
                        if (match) return true;
                    }
                }
            }
            return false;
        }
    }
    #endregion

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ComponentLifecycleHandle<T> where T : struct {
        public delegate*<uint, ulong[], T[], bool, void> _OnSegmentCreated;
        public delegate*<uint, void> _OnSegmentPooled;
        public delegate*<uint, void> _OnResize;
        public delegate*<uint, void> _OnInitialize;
        public delegate*<void> _OnDestroy;
        public delegate*<uint, ulong[], bool, void> _OnTrackingSegmentCreated;
        public delegate*<uint, void> _OnTrackingSegmentPooled;

        [MethodImpl(AggressiveInlining)]
        public void OnSegmentCreated(uint segmentIdx, ulong[] masks, T[] comps, bool fromPool) {
            if (_OnSegmentCreated != null) _OnSegmentCreated(segmentIdx, masks, comps, fromPool);
        }

        [MethodImpl(AggressiveInlining)]
        public void OnSegmentPooled(uint segmentIdx) {
            if (_OnSegmentPooled != null) _OnSegmentPooled(segmentIdx);
        }

        [MethodImpl(AggressiveInlining)]
        public void OnResize(uint newSegmentsCapacity) {
            if (_OnResize != null) _OnResize(newSegmentsCapacity);
        }

        [MethodImpl(AggressiveInlining)]
        public void OnInitialize(uint segmentsCapacity) {
            if (_OnInitialize != null) _OnInitialize(segmentsCapacity);
        }

        [MethodImpl(AggressiveInlining)]
        public void OnDestroy() {
            if (_OnDestroy != null) _OnDestroy();
        }

        [MethodImpl(AggressiveInlining)]
        public void OnTrackingSegmentCreated(uint segmentIdx, ulong[] tracking, bool fromPool) {
            if (_OnTrackingSegmentCreated != null) _OnTrackingSegmentCreated(segmentIdx, tracking, fromPool);
        }

        [MethodImpl(AggressiveInlining)]
        public void OnTrackingSegmentPooled(uint segmentIdx) {
            if (_OnTrackingSegmentPooled != null) _OnTrackingSegmentPooled(segmentIdx);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public struct HeuristicComponentsTracking {
        internal AtomicMask AddedBlocks;
        internal AtomicMask DeletedBlocks;
        internal AtomicMask ChangedBlocks;
    }
}