#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    /// <summary>
    /// Callback invoked during snapshot creation (write phase).
    /// Called once per snapshot operation, before or after the main serialization depending on registration method.
    /// </summary>
    /// <param name="snapshotParams">Parameters describing the snapshot being created, including its <see cref="SnapshotType"/>.</param>
    public delegate void SnapshotOnWriteAction(SnapshotWriteParams snapshotParams);

    /// <summary>
    /// Callback invoked during snapshot loading (read phase).
    /// Called once per snapshot operation, before or after the main deserialization depending on registration method.
    /// </summary>
    /// <param name="snapshotParams">Parameters describing the snapshot being loaded, including its <see cref="SnapshotType"/> and whether entities are loaded as new.</param>
    public delegate void SnapshotOnReadAction(SnapshotReadParams snapshotParams);

    /// <summary>
    /// Per-entity callback invoked during snapshot creation.
    /// Called once for each entity included in the snapshot, after all core entity data has been written.
    /// </summary>
    /// <typeparam name="TWorld">The world type this callback operates on.</typeparam>
    /// <param name="entity">The entity being serialized.</param>
    /// <param name="snapshotParams">Parameters describing the snapshot being created.</param>
    public delegate void SnapshotOnWriteEntityAction<TWorld>(World<TWorld>.Entity entity, SnapshotWriteParams snapshotParams)
        where TWorld : struct, IWorldType;

    /// <summary>
    /// Per-entity callback invoked during snapshot loading.
    /// Called once for each entity restored from the snapshot, after all core entity data has been read and applied.
    /// </summary>
    /// <typeparam name="TWorld">The world type this callback operates on.</typeparam>
    /// <param name="entity">The entity that was restored from the snapshot.</param>
    /// <param name="snapshotParams">Parameters describing the snapshot being loaded.</param>
    public delegate void SnapshotOnReadEntityAction<TWorld>(World<TWorld>.Entity entity, SnapshotReadParams snapshotParams)
        where TWorld : struct, IWorldType;

    /// <summary>
    /// Reader delegate for custom snapshot data sections.
    /// Invoked when a previously registered custom data block (identified by GUID) is encountered during snapshot loading.
    /// The delegate must consume exactly the data that was written by the corresponding <see cref="CustomSnapshotDataWriter"/>.
    /// Unknown GUIDs are automatically skipped via a stored byte-size prefix.
    /// </summary>
    /// <param name="reader">The binary reader positioned at the start of the custom data block.</param>
    /// <param name="version">The schema version that was stored when the data was written, enabling data migration.</param>
    /// <param name="snapshotParams">Parameters describing the snapshot being loaded.</param>
    public delegate void CustomSnapshotDataReader(ref BinaryPackReader reader, ushort version, SnapshotReadParams snapshotParams);

    /// <summary>
    /// Writer delegate for custom snapshot data sections.
    /// Invoked once per snapshot creation for each registered custom data handler.
    /// The data written here is framed with a GUID identifier, version, and byte-size prefix,
    /// enabling forward-compatible skipping if the handler is absent during loading.
    /// </summary>
    /// <param name="writer">The binary writer to serialize custom data into.</param>
    /// <param name="snapshotParams">Parameters describing the snapshot being created.</param>
    public delegate void CustomSnapshotDataWriter(ref BinaryPackWriter writer, SnapshotWriteParams snapshotParams);

    /// <summary>
    /// Per-entity reader delegate for custom snapshot data sections.
    /// Invoked once per entity for each registered custom entity data handler during snapshot loading.
    /// Must consume exactly the data that was written by the corresponding <see cref="CustomSnapshotEntityDataWriter{TWorld}"/>.
    /// </summary>
    /// <typeparam name="TWorld">The world type this reader operates on.</typeparam>
    /// <param name="reader">The binary reader positioned at the start of this entity's custom data.</param>
    /// <param name="entity">The entity whose custom data is being read.</param>
    /// <param name="version">The schema version stored when the data was written.</param>
    /// <param name="snapshotParams">Parameters describing the snapshot being loaded.</param>
    public delegate void CustomSnapshotEntityDataReader<TWorld>(ref BinaryPackReader reader, World<TWorld>.Entity entity, ushort version, SnapshotReadParams snapshotParams) where TWorld : struct, IWorldType;

    /// <summary>
    /// Per-entity writer delegate for custom snapshot data sections.
    /// Invoked once per entity for each registered custom entity data handler during snapshot creation.
    /// </summary>
    /// <typeparam name="TWorld">The world type this writer operates on.</typeparam>
    /// <param name="writer">The binary writer to serialize this entity's custom data into.</param>
    /// <param name="entity">The entity whose custom data is being written.</param>
    /// <param name="snapshotParams">Parameters describing the snapshot being created.</param>
    public delegate void CustomSnapshotEntityDataWriter<TWorld>(ref BinaryPackWriter writer, World<TWorld>.Entity entity, SnapshotWriteParams snapshotParams) where TWorld : struct, IWorldType;

    /// <summary>
    /// Identifies the granularity level of a snapshot operation.
    /// Passed to callbacks via <see cref="SnapshotWriteParams"/> and <see cref="SnapshotReadParams"/>
    /// so that user code can distinguish which type of snapshot is being processed.
    /// </summary>
    public enum SnapshotType: byte {
        /// <summary>
        /// GID store snapshot: serializes only entity metadata (presence masks, versions, cluster assignments)
        /// without component or tag data. Used to pre-allocate entity slots before loading component data separately.
        /// Corresponds to <c>World.Serializer.CreateGIDStoreSnapshot</c> and related methods.
        /// </summary>
        GIDStore,
        /// <summary>
        /// Entities snapshot: serializes individual entities with their full component and tag data.
        /// Supports selective entity serialization and the <c>entitiesAsNew</c> mode for spawning copies.
        /// Corresponds to <see cref="World{TWorld}.Serializer.CreateEntitiesSnapshotWriter"/> and
        /// <see cref="World{TWorld}.Serializer.LoadEntitiesSnapshot(BinaryPackReader, bool, QueryFunctionWithEntity{TWorld})"/>.
        /// </summary>
        Entities,
        /// <summary>
        /// World snapshot: serializes the entire world state — all clusters, chunks, entity metadata,
        /// component data, tag data, and optionally events and custom data.
        /// Corresponds to <see cref="World{TWorld}.Serializer.CreateWorldSnapshot(bool, bool, ChunkWritingStrategy, ReadOnlySpan{ushort}, bool)"/> and related methods.
        /// </summary>
        World,
        /// <summary>
        /// Cluster snapshot: serializes all chunks belonging to a single cluster.
        /// Can optionally include entity metadata (<c>withEntitiesData</c>) for loading into a world
        /// that does not yet have these entities allocated.
        /// Corresponds to <see cref="World{TWorld}.Serializer.CreateClusterSnapshot(ushort, bool, bool, ChunkWritingStrategy, bool)"/> and related methods.
        /// </summary>
        Cluster,
        /// <summary>
        /// Chunk snapshot: serializes a single chunk's entity metadata, component data, and tag data.
        /// Corresponds to <see cref="World{TWorld}.Serializer.CreateChunkSnapshot(uint, bool, bool, bool)"/> and related methods.
        /// </summary>
        Chunk
    }
    
    /// <summary>
    /// Immutable parameters passed to write-phase snapshot callbacks.
    /// Carries the <see cref="SnapshotType"/> so callbacks can distinguish between world, cluster, chunk, and entity snapshots.
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct SnapshotWriteParams {
        /// <summary>The granularity level of the snapshot being created.</summary>
        public readonly SnapshotType Type;

        [MethodImpl(AggressiveInlining)]
        public SnapshotWriteParams(SnapshotType type) {
            Type = type;
        }
    }

    /// <summary>
    /// Immutable parameters passed to read-phase snapshot callbacks.
    /// Carries the <see cref="SnapshotType"/> and whether entities are being loaded as new (duplicated) rather than restored in-place.
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct SnapshotReadParams {
        /// <summary>The granularity level of the snapshot being loaded.</summary>
        public readonly SnapshotType Type;
        /// <summary>
        /// When <c>true</c>, entities from the snapshot are created as brand-new entities with fresh IDs
        /// rather than being restored to their original positions. Useful for spawning prefab copies.
        /// </summary>
        public readonly bool EntitiesAsNew;

        [MethodImpl(AggressiveInlining)]
        public SnapshotReadParams(SnapshotType type, bool entitiesAsNew) {
            Type = type;
            EntitiesAsNew = entitiesAsNew;
        }
    }

    /// <summary>
    /// Configuration for loading cluster or chunk snapshots in "entities as new" mode.
    /// When <see cref="EntitiesAsNew"/> is <c>true</c>, entities from the snapshot are allocated at fresh positions
    /// and assigned to the specified <see cref="ClusterId"/>, rather than restoring them to their original locations.
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public readonly struct EntitiesAsNewParams {
        /// <summary>
        /// The cluster to assign newly created entities to. Only used when <see cref="EntitiesAsNew"/> is <c>true</c>.
        /// </summary>
        public readonly ushort ClusterId;
        /// <summary>
        /// When <c>true</c>, entities are created as new with fresh IDs in <see cref="ClusterId"/>.
        /// When <c>false</c>, entities are restored to their original positions (the snapshot must match the current world layout).
        /// </summary>
        public readonly bool EntitiesAsNew;

        /// <param name="entitiesAsNew">Whether to create entities as new or restore in-place.</param>
        /// <param name="clusterId">Target cluster for new entities. Ignored when <paramref name="entitiesAsNew"/> is <c>false</c>.</param>
        [MethodImpl(AggressiveInlining)]
        public EntitiesAsNewParams(bool entitiesAsNew, ushort clusterId = 0) {
            ClusterId = clusterId;
            EntitiesAsNew = entitiesAsNew;
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public abstract partial class World<TWorld> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
        #endif
        /// <summary>
        /// Provides binary serialization and deserialization of world state at multiple granularity levels.
        /// <para>
        /// <b>Snapshot hierarchy (coarse → fine):</b>
        /// <list type="bullet">
        ///   <item><b>World</b> — all clusters, all chunks, entity metadata, components, tags, optionally events and custom data.</item>
        ///   <item><b>GID Store</b> — entity metadata only (presence masks, versions, cluster assignments). No component/tag data.
        ///         Used to pre-allocate entity slots before loading data from other snapshot types.</item>
        ///   <item><b>Cluster</b> — all chunks of a single cluster, optionally with entity metadata (<c>withEntitiesData</c>).</item>
        ///   <item><b>Chunk</b> — a single chunk's entity metadata and component/tag data.</item>
        ///   <item><b>Entities</b> — selective per-entity serialization with full component/tag data via <see cref="EntitiesWriter"/>.</item>
        ///   <item><b>Events</b> — standalone event ring-buffer serialization.</item>
        /// </list>
        /// </para>
        /// <para>
        /// <b>Custom data extensibility:</b> Register custom data handlers via <see cref="SetSnapshotHandler"/> (global)
        /// or <see cref="SetSnapshotHandlerEachEntity"/> (per-entity). Each handler is identified by a stable <see cref="System.Guid"/>
        /// and carries a schema <c>version</c> for forward-compatible migration. Unrecognized GUIDs are automatically skipped during loading.
        /// </para>
        /// <para>
        /// <b>Lifecycle callbacks:</b> Register pre/post callbacks for snapshot creation and loading via
        /// <see cref="RegisterPreCreateSnapshotCallback"/>, <see cref="RegisterPostCreateSnapshotCallback"/>,
        /// <see cref="RegisterPreLoadSnapshotCallback"/>, <see cref="RegisterPostLoadSnapshotCallback"/>,
        /// <see cref="RegisterPostCreateSnapshotEachEntityCallback"/>, and <see cref="RegisterPostLoadSnapshotEachEntityCallback"/>.
        /// </para>
        /// <para>
        /// <b>Data migration:</b> When a component, tag, or event type is removed between versions, register a delete migrator
        /// via <see cref="SetMigrator"/>, <see cref="SetTagDeleteMigrator"/>, or <see cref="SetEventDeleteMigrator"/>
        /// to handle legacy data during loading instead of silently dropping it.
        /// </para>
        /// <para>
        /// <b>Thread safety:</b> Snapshot operations are NOT thread-safe. Do not create or load snapshots concurrently
        /// with entity operations or parallel queries.
        /// </para>
        /// </summary>
        public static class Serializer {
            internal static Dictionary<Guid, (CustomSnapshotDataWriter writer, CustomSnapshotDataReader reader, ushort version)> SnapshotDataSerializers;
            internal static Dictionary<Guid, (CustomSnapshotEntityDataWriter<TWorld> writer, CustomSnapshotEntityDataReader<TWorld> reader, ushort version)> SnapshotDataEntitySerializers;

            internal static List<SnapshotOnWriteAction> PreCreateSnapshotCallbacks;
            internal static List<SnapshotOnReadAction> PreLoadSnapshotCallbacks;
            internal static List<SnapshotOnWriteAction> PostCreateSnapshotCallbacks;
            internal static List<SnapshotOnReadAction> PostLoadSnapshotCallbacksType;
            internal static List<SnapshotOnWriteEntityAction<TWorld>> OnCreateEntitySnapshotActions;
            internal static List<SnapshotOnReadEntityAction<TWorld>> OnRestoreEntityFromSnapshotActions;

            [MethodImpl(AggressiveInlining)]
            internal static void Create() {
                SnapshotDataSerializers = new Dictionary<Guid, (CustomSnapshotDataWriter, CustomSnapshotDataReader, ushort)>();
                SnapshotDataEntitySerializers = new Dictionary<Guid, (CustomSnapshotEntityDataWriter<TWorld>, CustomSnapshotEntityDataReader<TWorld>, ushort)>();
                PreCreateSnapshotCallbacks = new List<SnapshotOnWriteAction>(16);
                PreLoadSnapshotCallbacks = new List<SnapshotOnReadAction>(16);
                PostCreateSnapshotCallbacks = new List<SnapshotOnWriteAction>(16);
                PostLoadSnapshotCallbacksType = new List<SnapshotOnReadAction>(16);
                OnCreateEntitySnapshotActions = new List<SnapshotOnWriteEntityAction<TWorld>>(16);
                OnRestoreEntityFromSnapshotActions = new List<SnapshotOnReadEntityAction<TWorld>>(16);
            }

            [MethodImpl(AggressiveInlining)]
            internal static void DestroySerializer() {
                SnapshotDataSerializers = default;
                PostCreateSnapshotCallbacks = default;
                PostLoadSnapshotCallbacksType = default;
                OnCreateEntitySnapshotActions = default;
                OnRestoreEntityFromSnapshotActions = default;
                SnapshotDataEntitySerializers = default;
                PreCreateSnapshotCallbacks = default;
                PreLoadSnapshotCallbacks = default;
            }

            /// <summary>
            /// Registers a callback invoked <b>before</b> any snapshot is created (write phase).
            /// Use this to prepare world state (e.g., flush deferred operations) before serialization begins.
            /// </summary>
            /// <param name="action">The callback to invoke. Receives <see cref="SnapshotWriteParams"/> indicating the snapshot type.</param>
            [MethodImpl(AggressiveInlining)]
            public static void RegisterPreCreateSnapshotCallback(SnapshotOnWriteAction action) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                PreCreateSnapshotCallbacks.Add(action);
            }

            /// <summary>
            /// Registers a callback invoked <b>after</b> a snapshot has been fully created (write phase).
            /// Called after all entity data and custom snapshot data have been written.
            /// </summary>
            /// <param name="action">The callback to invoke. Receives <see cref="SnapshotWriteParams"/> indicating the snapshot type.</param>
            [MethodImpl(AggressiveInlining)]
            public static void RegisterPostCreateSnapshotCallback(SnapshotOnWriteAction action) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                PostCreateSnapshotCallbacks.Add(action);
            }

            /// <summary>
            /// Registers a callback invoked <b>before</b> a snapshot is loaded (read phase).
            /// Use this to prepare the world for incoming data (e.g., clear caches, reset state).
            /// </summary>
            /// <param name="action">The callback to invoke. Receives <see cref="SnapshotReadParams"/> indicating the snapshot type and mode.</param>
            [MethodImpl(AggressiveInlining)]
            public static void RegisterPreLoadSnapshotCallback(SnapshotOnReadAction action) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                PreLoadSnapshotCallbacks.Add(action);
            }

            /// <summary>
            /// Registers a callback invoked <b>after</b> a snapshot has been fully loaded (read phase).
            /// Called after all entity data and custom snapshot data have been read and applied.
            /// </summary>
            /// <param name="action">The callback to invoke. Receives <see cref="SnapshotReadParams"/> indicating the snapshot type and mode.</param>
            [MethodImpl(AggressiveInlining)]
            public static void RegisterPostLoadSnapshotCallback(SnapshotOnReadAction action) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                PostLoadSnapshotCallbacksType.Add(action);
            }

            /// <summary>
            /// Registers a per-entity callback invoked <b>after</b> snapshot creation for each entity included in the snapshot.
            /// Called after all core entity data and custom snapshot data have been written.
            /// Useful for post-serialization bookkeeping or tagging serialized entities.
            /// </summary>
            /// <param name="action">The per-entity callback. Receives the entity and <see cref="SnapshotWriteParams"/>.</param>
            [MethodImpl(AggressiveInlining)]
            public static void RegisterPostCreateSnapshotEachEntityCallback(SnapshotOnWriteEntityAction<TWorld> action) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                OnCreateEntitySnapshotActions.Add(action);
            }

            /// <summary>
            /// Registers a per-entity callback invoked <b>after</b> snapshot loading for each entity restored from the snapshot.
            /// Called after all core entity data and custom snapshot data have been read and applied.
            /// Useful for post-deserialization fixup (e.g., resolving cross-references, initializing runtime state).
            /// </summary>
            /// <param name="action">The per-entity callback. Receives the restored entity and <see cref="SnapshotReadParams"/>.</param>
            [MethodImpl(AggressiveInlining)]
            public static void RegisterPostLoadSnapshotEachEntityCallback(SnapshotOnReadEntityAction<TWorld> action) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                OnRestoreEntityFromSnapshotActions.Add(action);
            }

            /// <summary>
            /// Registers a custom global data handler for snapshot serialization.
            /// The handler is identified by a stable <paramref name="guid"/> and carries a <paramref name="version"/>
            /// for schema migration. During snapshot creation, <paramref name="writer"/> is called once to write the data.
            /// During loading, <paramref name="reader"/> is called if the GUID is recognized; otherwise the data is skipped.
            /// Each GUID can have only one handler; calling again with the same GUID replaces the previous handler.
            /// </summary>
            /// <param name="guid">Stable identifier for this custom data block. Must be non-empty.</param>
            /// <param name="version">Schema version written alongside the data, passed to the reader for migration.</param>
            /// <param name="writer">Delegate that serializes the custom data.</param>
            /// <param name="reader">Delegate that deserializes the custom data, receiving the stored version.</param>
            [MethodImpl(AggressiveInlining)]
            public static void SetSnapshotHandler(Guid guid, ushort version, CustomSnapshotDataWriter writer, CustomSnapshotDataReader reader) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                SnapshotDataSerializers[guid] = (writer, reader, version);
            }

            /// <summary>
            /// Registers a custom per-entity data handler for snapshot serialization.
            /// Similar to <see cref="SetSnapshotHandler"/> but the writer/reader are called once per entity in the snapshot.
            /// During creation, the <paramref name="writer"/> is invoked for every entity; during loading,
            /// the <paramref name="reader"/> is invoked for every entity if the GUID is recognized.
            /// </summary>
            /// <param name="guid">Stable identifier for this custom per-entity data block.</param>
            /// <param name="version">Schema version for migration support.</param>
            /// <param name="writer">Delegate that serializes per-entity custom data.</param>
            /// <param name="reader">Delegate that deserializes per-entity custom data.</param>
            [MethodImpl(AggressiveInlining)]
            public static void SetSnapshotHandlerEachEntity(Guid guid, ushort version, CustomSnapshotEntityDataWriter<TWorld> writer, CustomSnapshotEntityDataReader<TWorld> reader) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                SnapshotDataEntitySerializers[guid] = (writer, reader, version);
            }

            /// <summary>
            /// Registers a migration handler for a removed component type.
            /// When a snapshot contains component data with the specified <paramref name="id"/> (GUID) but no matching
            /// component type is registered in the current world, the <paramref name="migrator"/> is called to handle
            /// the legacy data (e.g., transform it into a new component type or discard it gracefully).
            /// Without a migrator, unknown component data is silently skipped.
            /// </summary>
            /// <param name="id">The GUID of the removed component type as it was registered when the snapshot was created.</param>
            /// <param name="migrator">Delegate that processes legacy component data during deserialization.</param>
            [MethodImpl(AggressiveInlining)]
            public static void SetMigrator(Guid id, EcsComponentDeleteMigrationReader<TWorld> migrator) {
                Data.Instance.SetMigrator(id, migrator);
            }

            /// <summary>
            /// Registers a migration handler for a removed event type.
            /// When an events snapshot contains events with the specified <paramref name="id"/> (GUID) but no matching
            /// event type is registered, the <paramref name="migrator"/> is invoked to handle the legacy event data.
            /// Without a migrator, unknown events are silently skipped.
            /// </summary>
            /// <param name="id">The GUID of the removed event type.</param>
            /// <param name="migrator">Delegate that processes legacy event data during deserialization.</param>
            [MethodImpl(AggressiveInlining)]
            public static void SetEventDeleteMigrator(Guid id, EcsEventDeleteMigrationReader migrator) {
                Data.Instance.SetDeleteEventsMigrator(id, migrator);
            }

            /// <summary>
            /// Writes all registered custom global snapshot data into the binary stream.
            /// Each handler registered via <see cref="SetSnapshotHandler"/> is invoked, and its output is framed
            /// with the handler's GUID, version, and a byte-size prefix for forward-compatible skipping.
            /// Typically called internally by snapshot creation methods; can be used for manual snapshot composition.
            /// </summary>
            /// <param name="writer">The binary writer to serialize into.</param>
            /// <param name="snapshotParams">Parameters describing the snapshot being created.</param>
            [MethodImpl(AggressiveInlining)]
            public static void WriteSnapshotData(ref BinaryPackWriter writer, SnapshotWriteParams snapshotParams) {
                writer.WriteInt(SnapshotDataSerializers.Count);
                foreach (var (key, (snapshotDataWriter, _, version)) in SnapshotDataSerializers) {
                    writer.WriteGuid(key);
                    writer.WriteUshort(version);
                    var point = writer.MakePoint(sizeof(uint));
                    snapshotDataWriter(ref writer, snapshotParams);
                    writer.WriteUintAt(point, writer.Position - (point + sizeof(uint)));
                }
            }

            /// <summary>
            /// Reads custom global snapshot data from the binary stream.
            /// For each data block, the GUID is looked up among registered handlers (see <see cref="SetSnapshotHandler"/>).
            /// Recognized blocks are dispatched to their reader delegate; unrecognized blocks are skipped via the stored byte-size prefix.
            /// </summary>
            /// <param name="reader">The binary reader positioned at the custom data section.</param>
            /// <param name="snapshotParams">Parameters describing the snapshot being loaded.</param>
            [MethodImpl(AggressiveInlining)]
            public static void ReadSnapshotData(ref BinaryPackReader reader, SnapshotReadParams snapshotParams) {
                var count = reader.ReadInt();

                for (var i = 0; i < count; i++) {
                    var key = reader.ReadGuid();
                    var version = reader.ReadUshort();
                    var byteSize = reader.ReadUint();
                    if (SnapshotDataSerializers.TryGetValue(key, out var val)) {
                        val.reader(ref reader, version, snapshotParams);
                    } else {
                        reader.SkipNext(byteSize);
                    }
                }
            }

            /// <summary>
            /// Writes all registered custom per-entity snapshot data into the binary stream.
            /// Each handler registered via <see cref="SetSnapshotHandlerEachEntity"/> is invoked once per entity
            /// across the specified <paramref name="chunks"/>. Output is framed with GUID, version, and byte-size prefix.
            /// </summary>
            /// <param name="writer">The binary writer to serialize into.</param>
            /// <param name="snapshotParams">Parameters describing the snapshot being created.</param>
            /// <param name="chunks">The chunk indices whose entities should be included.</param>
            [MethodImpl(AggressiveInlining)]
            public static void WriteEntitySnapshotData(ref BinaryPackWriter writer, SnapshotWriteParams snapshotParams, ReadOnlySpan<uint> chunks) {
                writer.WriteInt(SnapshotDataEntitySerializers.Count);
                foreach (var (key, (snapshotDataEntityWriter, _, version)) in SnapshotDataEntitySerializers) {
                    writer.WriteGuid(key);
                    writer.WriteUshort(version);
                    var point = writer.MakePoint(sizeof(uint));
                    Query().WriteEntitySnapshotData(ref writer, snapshotDataEntityWriter, snapshotParams, chunks, EntityStatusType.Any);
                    writer.WriteUintAt(point, writer.Position - (point + sizeof(uint)));
                }
            }

            /// <summary>
            /// Reads custom per-entity snapshot data from the binary stream.
            /// For each data block, the GUID is matched to handlers registered via <see cref="SetSnapshotHandlerEachEntity"/>.
            /// Recognized blocks invoke the reader delegate once per entity; unrecognized blocks are skipped.
            /// </summary>
            /// <param name="reader">The binary reader positioned at the per-entity custom data section.</param>
            /// <param name="snapshotParams">Parameters describing the snapshot being loaded.</param>
            /// <param name="chunks">The chunk indices whose entities should receive the custom data.</param>
            [MethodImpl(AggressiveInlining)]
            public static void ReadEntitySnapshotData(ref BinaryPackReader reader, SnapshotReadParams snapshotParams, ReadOnlySpan<uint> chunks) {
                var count = reader.ReadInt();

                for (var i = 0; i < count; i++) {
                    var key = reader.ReadGuid();
                    var version = reader.ReadUshort();
                    var byteSize = reader.ReadUint();
                    if (SnapshotDataEntitySerializers.TryGetValue(key, out var val)) {
                        Query().ReadEntitySnapshotData(ref reader, val.reader, version, snapshotParams, chunks, EntityStatusType.Any);
                    } else {
                        reader.SkipNext(byteSize);
                    }
                }
            }

            #region EVENTS
            /// <summary>
            /// Serializes the current event ring-buffer state into an existing binary writer.
            /// Only event types registered with a non-empty GUID are included.
            /// Requires the world to be initialized.
            /// </summary>
            /// <param name="writer">The binary writer to serialize events into.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateEventsSnapshot(ref BinaryPackWriter writer) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized) throw new StaticEcsException($"World<{typeof(TWorld)}>.Event, Method: CreateSnapshot, World not initialized");
                #endif
                Data.Instance.WriteEvents(ref writer);
            }
            
            /// <summary>
            /// Serializes the current event ring-buffer state and returns it as a byte array.
            /// </summary>
            /// <param name="byteSizeHint">Initial buffer size hint to reduce reallocations.</param>
            /// <param name="gzip">When <c>true</c>, the output is gzip-compressed.</param>
            /// <returns>A byte array containing the serialized events, optionally gzip-compressed.</returns>
            [MethodImpl(AggressiveInlining)]
            public static byte[] CreateEventsSnapshot(uint byteSizeHint = 512, bool gzip = false) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized) throw new StaticEcsException($"World<{typeof(TWorld)}>.Event, Method: CreateSnapshot, World not initialized");
                #endif
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                Data.Instance.WriteEvents(ref writer);
                var result = writer.CopyToBytes(gzip);
                writer.Dispose();
                return result;
            }
            
            /// <summary>
            /// Serializes the current event ring-buffer state into an existing byte array, resizing it if necessary.
            /// </summary>
            /// <param name="result">Reference to the destination byte array. Will be resized if the serialized data exceeds its length.</param>
            /// <param name="gzip">When <c>true</c>, the output is gzip-compressed.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateEventsSnapshot(ref byte[] result, bool gzip = false) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized) throw new StaticEcsException($"World<{typeof(TWorld)}>.Event, Method: CreateSnapshot, World not initialized");
                #endif
                var writer = BinaryPackWriter.CreateFromPool((uint) result.Length);
                Data.Instance.WriteEvents(ref writer);
                writer.CopyToBytes(ref result, gzip);
                writer.Dispose();
            }
            
            /// <summary>
            /// Serializes the current event ring-buffer state and writes it directly to a file.
            /// </summary>
            /// <param name="filePath">Destination file path.</param>
            /// <param name="gzip">When <c>true</c>, the output is gzip-compressed before writing.</param>
            /// <param name="flushToDisk">When <c>true</c>, forces the OS to flush file buffers to physical storage.</param>
            /// <param name="byteSizeHint">Initial buffer size hint.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateEventsSnapshot(string filePath, bool gzip = false, bool flushToDisk = false, uint byteSizeHint = 512) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized) throw new StaticEcsException($"World<{typeof(TWorld)}>.Event, Method: CreateSnapshot, World not initialized");
                #endif
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                Data.Instance.WriteEvents(ref writer);
                writer.FlushToFile(filePath, gzip, flushToDisk);
                writer.Dispose();
            }
            
            /// <summary>
            /// Restores the event ring-buffer state from a binary reader.
            /// All existing events are cleared before loading. Event types not registered in the current world
            /// are handled by delete migrators if registered, otherwise silently skipped.
            /// </summary>
            /// <param name="reader">The binary reader containing serialized event data.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadEventsSnapshot(BinaryPackReader reader) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized) throw new StaticEcsException($"World<{typeof(TWorld)}>.Event, Method: LoadSnapshot, World not initialized");
                #endif
                Data.Instance.ReadEvents(ref reader);
            }

            /// <inheritdoc cref="LoadEventsSnapshot(BinaryPackReader)"/>
            /// <param name="snapshot">Byte array containing the serialized event data.</param>
            /// <param name="gzip">When <c>true</c>, the input is decompressed from gzip before reading.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadEventsSnapshot(byte[] snapshot, bool gzip = false) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized) throw new StaticEcsException($"World<{typeof(TWorld)}>.Event, Method: LoadSnapshot, World not initialized");
                #endif
                if (gzip) {
                    var writer = BinaryPackWriter.CreateFromPool((uint) (snapshot.Length * 2));
                    writer.WriteGzipData(snapshot);
                    var reader = writer.AsReader();
                    Data.Instance.ReadEvents(ref reader);
                    writer.Dispose();
                } else {
                    var reader = new BinaryPackReader(snapshot, (uint) snapshot.Length, 0);
                    Data.Instance.ReadEvents(ref reader);
                }
            }

            /// <inheritdoc cref="LoadEventsSnapshot(BinaryPackReader)"/>
            /// <param name="worldSnapshotFilePath">Path to the file containing serialized event data.</param>
            /// <param name="gzip">When <c>true</c>, the file is decompressed from gzip before reading.</param>
            /// <param name="byteSizeHint">Initial buffer size hint for file loading.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadEventsSnapshot(string worldSnapshotFilePath, bool gzip = false, uint byteSizeHint = 512) {
                #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                if (!IsWorldInitialized) throw new StaticEcsException($"World<{typeof(TWorld)}>.Event, Method: LoadSnapshot, World not initialized");
                #endif
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                writer.WriteFromFile(worldSnapshotFilePath, gzip);
                var reader = writer.AsReader();
                Data.Instance.ReadEvents(ref reader);
                writer.Dispose();
            }
            #endregion

            #region WORLD
            /// <summary>
            /// Creates a full world snapshot containing all clusters, chunks, entity metadata, component data, tag data,
            /// and optionally events and custom user data. Returns the result as a byte array.
            /// <para>
            /// This is the most comprehensive snapshot type. On loading via <see cref="LoadWorldSnapshot(BinaryPackReader)"/>,
            /// the world is fully restored: if already initialized it is cleared first; if only created, it is initialized from the snapshot.
            /// </para>
            /// </summary>
            /// <param name="withCustomSnapshotData">When <c>true</c>, includes custom data from handlers registered via
            /// <see cref="SetSnapshotHandler"/> and <see cref="SetSnapshotHandlerEachEntity"/>.</param>
            /// <param name="gzip">When <c>true</c>, the output is gzip-compressed.</param>
            /// <param name="strategy">Controls which chunks to include based on ownership.
            /// <c>All</c> includes all chunks; <c>SelfOwner</c> only self-owned; <c>OtherOwner</c> only externally-owned.</param>
            /// <param name="clusters">Specific cluster IDs to include. When empty (default), all active clusters are included.</param>
            /// <param name="writeEvents">When <c>true</c>, the event ring-buffer is included in the snapshot.</param>
            /// <returns>A byte array containing the serialized world state.</returns>
            [MethodImpl(AggressiveInlining)]
            public static byte[] CreateWorldSnapshot(bool withCustomSnapshotData = true, bool gzip = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, ReadOnlySpan<ushort> clusters = default, bool writeEvents = true) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                WriteWorld(ref writer, withCustomSnapshotData, strategy, clusters, writeEvents);
                var result = writer.CopyToBytes(gzip);
                writer.Dispose();
                return result;
            }

            /// <inheritdoc cref="CreateWorldSnapshot(bool, bool, ChunkWritingStrategy, ReadOnlySpan{ushort}, bool)"/>
            /// <param name="result">Reference to the destination byte array. Resized if necessary.</param>
            /// <returns>Length of writen bytes.</returns>
            [MethodImpl(AggressiveInlining)]
            public static int CreateWorldSnapshot(ref byte[] result, bool withCustomSnapshotData = true, bool gzip = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, ReadOnlySpan<ushort> clusters = default, bool writeEvents = true) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                var writer = BinaryPackWriter.Create(result);
                WriteWorld(ref writer, withCustomSnapshotData, strategy, clusters, writeEvents);
                if (gzip) {
                    return writer.Gzip(ref result, 0, writer.Position);
                }

                result = writer.Buffer;
                return (int) writer.Position;
            }

            /// <summary>
            /// Creates a full world snapshot and writes it directly to a file.
            /// Uses <see cref="ChunkWritingStrategy.All"/> and includes all active clusters.
            /// </summary>
            /// <param name="filePath">Destination file path.</param>
            /// <param name="withCustomSnapshotData">When <c>true</c>, includes custom snapshot handler data.</param>
            /// <param name="gzip">When <c>true</c>, the output is gzip-compressed before writing.</param>
            /// <param name="flushToDisk">When <c>true</c>, forces the OS to flush file buffers to physical storage.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateWorldSnapshot(string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                CreateWorldSnapshot(ref writer, filePath, withCustomSnapshotData, gzip, flushToDisk);
                writer.Dispose();
            }

            /// <summary>
            /// Creates a full world snapshot into an existing binary writer.
            /// The caller is responsible for disposing the writer and extracting the result.
            /// </summary>
            /// <param name="writer">The binary writer to serialize into.</param>
            /// <param name="withCustomSnapshotData">When <c>true</c>, includes custom snapshot handler data.</param>
            /// <param name="strategy">Chunk ownership filter.</param>
            /// <param name="clusters">Specific cluster IDs to include, or empty for all active clusters.</param>
            /// <param name="writeEvents">When <c>true</c>, includes the event ring-buffer.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateWorldSnapshot(ref BinaryPackWriter writer, bool withCustomSnapshotData = true, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, ReadOnlySpan<ushort> clusters = default, bool writeEvents = true) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                WriteWorld(ref writer, withCustomSnapshotData, strategy, clusters, writeEvents);
            }

            /// <summary>
            /// Creates a full world snapshot into an existing binary writer and flushes it to a file.
            /// Combines serialization and file output in a single call.
            /// </summary>
            /// <param name="writer">The binary writer to serialize into.</param>
            /// <param name="filePath">Destination file path.</param>
            /// <param name="withCustomSnapshotData">When <c>true</c>, includes custom snapshot handler data.</param>
            /// <param name="gzip">When <c>true</c>, the output is gzip-compressed.</param>
            /// <param name="flushToDisk">When <c>true</c>, forces the OS to flush file buffers to physical storage.</param>
            /// <param name="strategy">Chunk ownership filter.</param>
            /// <param name="clusters">Specific cluster IDs to include, or empty for all active clusters.</param>
            /// <param name="writeEvents">When <c>true</c>, includes the event ring-buffer.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateWorldSnapshot(ref BinaryPackWriter writer, string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, ReadOnlySpan<ushort> clusters = default, bool writeEvents = true) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                WriteWorld(ref writer, withCustomSnapshotData, strategy, clusters, writeEvents);
                writer.FlushToFile(filePath, gzip, flushToDisk);
            }

            /// <summary>
            /// Loads a full world snapshot from a binary reader, completely restoring world state.
            /// <para>
            /// If the world is already initialized, it is <b>cleared</b> first (all entities, components, tags destroyed).
            /// If the world is in <c>Created</c> state (not yet initialized), it is initialized from the snapshot's chunk capacity.
            /// </para>
            /// <para>Clusters, chunks, entity metadata, component/tag data, events (if present), and custom data are all restored.</para>
            /// </summary>
            /// <param name="reader">The binary reader containing a world snapshot produced by <c>CreateWorldSnapshot</c>.</param>
            /// <param name="hardReset">When <c>true</c>, performs a hard reset (no OnDelete hooks, faster) instead of standard
            /// entity deletion when clearing the existing world before loading. Has no effect if the world is not yet initialized.
            /// Default is <c>false</c>.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadWorldSnapshot(BinaryPackReader reader, bool hardReset = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                ReadWorld(ref reader, hardReset);
            }

            /// <inheritdoc cref="LoadWorldSnapshot(BinaryPackReader, bool)"/>
            /// <param name="snapshot">Byte array containing the serialized world snapshot.</param>
            /// <param name="gzip">When <c>true</c>, the input is decompressed from gzip before reading.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadWorldSnapshot(byte[] snapshot, bool gzip = false, bool hardReset = false) {
                LoadWorldSnapshot(snapshot, 0, snapshot.Length, gzip, hardReset);
            }

            /// <inheritdoc cref="LoadWorldSnapshot(BinaryPackReader, bool)" />
            /// <param name="snapshot">Byte array containing the serialized world snapshot.</param>
            /// <param name="index">The starting offset within <paramref name="snapshot" /> to begin reading from.</param>
            /// <param name="count">The number of bytes to read from <paramref name="snapshot" />.</param>
            /// <param name="gzip">When <c>true</c>, the input is decompressed from gzip before reading.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadWorldSnapshot(byte[] snapshot, int index, int count, bool gzip = false, bool hardReset = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                if (gzip) {
                    var writer = BinaryPackWriter.CreateFromPool((uint) (count * 2));
                    writer.WriteGzipData(snapshot, index, count);
                    var reader = writer.AsReader();
                    ReadWorld(ref reader, hardReset);
                    writer.Dispose();
                } else {
                    var reader = new BinaryPackReader(snapshot, (uint) count, (uint) index);
                    ReadWorld(ref reader, hardReset);
                }
            }

            /// <inheritdoc cref="LoadWorldSnapshot(BinaryPackReader, bool)"/>
            /// <param name="worldSnapshotFilePath">Path to the file containing the world snapshot.</param>
            /// <param name="gzip">When <c>true</c>, the file is decompressed from gzip before reading.</param>
            /// <param name="byteSizeHint">Initial buffer size hint. When 0, automatically calculated from world capacity.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadWorldSnapshot(string worldSnapshotFilePath, bool gzip = false, uint byteSizeHint = 0, bool hardReset = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                CalculateByteSizeHint(ref byteSizeHint);
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                writer.WriteFromFile(worldSnapshotFilePath, gzip);
                var reader = writer.AsReader();
                ReadWorld(ref reader, hardReset);
                writer.Dispose();
            }

            /// <summary>
            /// Creates a snapshot of a single cluster, including all its chunks' component and tag data.
            /// Returns the result as a byte array.
            /// <para>
            /// When <paramref name="withEntitiesData"/> is <c>true</c>, entity metadata (presence masks, versions, cluster assignments)
            /// is included, enabling the snapshot to be loaded into a world that does not yet have these entities allocated
            /// (e.g., via <see cref="EntitiesAsNewParams"/> with <c>EntitiesAsNew = true</c>).
            /// When <c>false</c>, only component/tag data is written, and the target world must already have matching entities.
            /// </para>
            /// </summary>
            /// <param name="clusterId">The cluster to serialize.</param>
            /// <param name="withCustomSnapshotData">When <c>true</c>, includes custom snapshot handler data.</param>
            /// <param name="gzip">When <c>true</c>, the output is gzip-compressed.</param>
            /// <param name="strategy">Chunk ownership filter.</param>
            /// <param name="withEntitiesData">When <c>true</c>, includes entity metadata for standalone loading.</param>
            /// <returns>A byte array containing the serialized cluster data.</returns>
            [MethodImpl(AggressiveInlining)]
            public static byte[] CreateClusterSnapshot(ushort clusterId, bool withCustomSnapshotData = true, bool gzip = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, bool withEntitiesData = false) {
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                WriteCluster(ref writer, withCustomSnapshotData, strategy, clusterId, withEntitiesData);
                var result = writer.CopyToBytes(gzip);
                writer.Dispose();
                return result;
            }

            /// <inheritdoc cref="CreateClusterSnapshot(ushort, bool, bool, ChunkWritingStrategy, bool)"/>
            /// <param name="result">Reference to the destination byte array. Resized if necessary.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateClusterSnapshot(ushort clusterId, ref byte[] result, bool withCustomSnapshotData = true, bool gzip = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, bool withEntitiesData = false) {
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                WriteCluster(ref writer, withCustomSnapshotData, strategy, clusterId, withEntitiesData);
                writer.CopyToBytes(ref result, gzip);
                writer.Dispose();
            }

            /// <inheritdoc cref="CreateClusterSnapshot(ushort, bool, bool, ChunkWritingStrategy, bool)"/>
            /// <param name="filePath">Destination file path.</param>
            /// <param name="flushToDisk">When <c>true</c>, forces the OS to flush file buffers to physical storage.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateClusterSnapshot(ushort clusterId, string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, bool withEntitiesData = false) {
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                CreateClusterSnapshot(clusterId, ref writer, filePath, withCustomSnapshotData, gzip, flushToDisk, strategy, withEntitiesData);
                writer.Dispose();
            }

            /// <summary>
            /// Creates a cluster snapshot into an existing binary writer.
            /// </summary>
            /// <param name="clusterId">The cluster to serialize.</param>
            /// <param name="writer">The binary writer to serialize into.</param>
            /// <param name="withCustomSnapshotData">When <c>true</c>, includes custom snapshot handler data.</param>
            /// <param name="strategy">Chunk ownership filter.</param>
            /// <param name="withEntitiesData">When <c>true</c>, includes entity metadata.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateClusterSnapshot(ushort clusterId, ref BinaryPackWriter writer, bool withCustomSnapshotData = true, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, bool withEntitiesData = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                WriteCluster(ref writer, withCustomSnapshotData, strategy, clusterId, withEntitiesData);
            }

            /// <summary>
            /// Creates a cluster snapshot into an existing binary writer and flushes it to a file.
            /// </summary>
            /// <param name="clusterId">The cluster to serialize.</param>
            /// <param name="writer">The binary writer to serialize into.</param>
            /// <param name="filePath">Destination file path.</param>
            /// <param name="withCustomSnapshotData">When <c>true</c>, includes custom snapshot handler data.</param>
            /// <param name="gzip">When <c>true</c>, the output is gzip-compressed.</param>
            /// <param name="flushToDisk">When <c>true</c>, forces the OS to flush file buffers to physical storage.</param>
            /// <param name="strategy">Chunk ownership filter.</param>
            /// <param name="withEntitiesData">When <c>true</c>, includes entity metadata.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateClusterSnapshot(ushort clusterId, ref BinaryPackWriter writer, string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, bool withEntitiesData = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                WriteCluster(ref writer, withCustomSnapshotData, strategy, clusterId, withEntitiesData);
                writer.FlushToFile(filePath, gzip, flushToDisk);
            }

            /// <summary>
            /// Loads a cluster snapshot from a binary reader, restoring all chunk component and tag data for the cluster.
            /// <para>
            /// When <c>entitiesAsNew.EntitiesAsNew</c> is <c>false</c> (default), the cluster must already be registered
            /// and entities must exist at their original positions. Component/tag data is written over existing entities.
            /// </para>
            /// <para>
            /// When <c>entitiesAsNew.EntitiesAsNew</c> is <c>true</c>, fresh chunks are allocated and entities are created
            /// as new in <c>entitiesAsNew.ClusterId</c>. Requires the snapshot to have been created with <c>withEntitiesData = true</c>.
            /// </para>
            /// </summary>
            /// <param name="reader">The binary reader containing cluster snapshot data.</param>
            /// <param name="entitiesAsNew">Controls whether entities are restored in-place or created as new copies.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadClusterSnapshot(BinaryPackReader reader, EntitiesAsNewParams entitiesAsNew = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                ReadCluster(ref reader, entitiesAsNew);
            }

            /// <inheritdoc cref="LoadClusterSnapshot(BinaryPackReader, EntitiesAsNewParams)"/>
            /// <param name="snapshot">Byte array containing the serialized cluster data.</param>
            /// <param name="gzip">When <c>true</c>, the input is decompressed from gzip before reading.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadClusterSnapshot(byte[] snapshot, bool gzip = false, EntitiesAsNewParams entitiesAsNew = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                if (gzip) {
                    var writer = BinaryPackWriter.CreateFromPool((uint) (snapshot.Length * 2));
                    writer.WriteGzipData(snapshot);
                    var reader = writer.AsReader();
                    ReadCluster(ref reader, entitiesAsNew);
                    writer.Dispose();
                } else {
                    var reader = new BinaryPackReader(snapshot, (uint) snapshot.Length, 0);
                    ReadCluster(ref reader, entitiesAsNew);
                }
            }

            /// <inheritdoc cref="LoadClusterSnapshot(BinaryPackReader, EntitiesAsNewParams)"/>
            /// <param name="worldSnapshotFilePath">Path to the file containing cluster snapshot data.</param>
            /// <param name="gzip">When <c>true</c>, the file is decompressed from gzip before reading.</param>
            /// <param name="byteSizeHint">Initial buffer size hint. When 0, automatically calculated.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadClusterSnapshot(string worldSnapshotFilePath, bool gzip = false, uint byteSizeHint = 0, EntitiesAsNewParams entitiesAsNew = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                CalculateByteSizeHint(ref byteSizeHint);
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                writer.WriteFromFile(worldSnapshotFilePath, gzip);
                var reader = writer.AsReader();
                ReadCluster(ref reader, entitiesAsNew);
                writer.Dispose();
            }

            /// <summary>
            /// Creates a snapshot of a single chunk, including its entity metadata, component data, and tag data.
            /// Returns the result as a byte array.
            /// <para>
            /// When <paramref name="withEntitiesData"/> is <c>true</c>, entity metadata (presence masks, versions) is included,
            /// enabling the snapshot to be loaded with <c>EntitiesAsNew = true</c>. The chunk must be registered in the world.
            /// </para>
            /// </summary>
            /// <param name="chunkIdx">Index of the chunk to serialize. Must be a registered chunk.</param>
            /// <param name="withCustomSnapshotData">When <c>true</c>, includes custom snapshot handler data.</param>
            /// <param name="gzip">When <c>true</c>, the output is gzip-compressed.</param>
            /// <param name="withEntitiesData">When <c>true</c>, includes entity metadata for standalone loading.</param>
            /// <returns>A byte array containing the serialized chunk data.</returns>
            [MethodImpl(AggressiveInlining)]
            public static byte[] CreateChunkSnapshot(uint chunkIdx, bool withCustomSnapshotData = true, bool gzip = false, bool withEntitiesData = false) {
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                WriteChunk(ref writer, withCustomSnapshotData, chunkIdx, withEntitiesData);
                var result = writer.CopyToBytes(gzip);
                writer.Dispose();
                return result;
            }

            /// <inheritdoc cref="CreateChunkSnapshot(uint, bool, bool, bool)"/>
            /// <param name="result">Reference to the destination byte array. Resized if necessary.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateChunkSnapshot(uint chunkIdx, ref byte[] result, bool withCustomSnapshotData = true, bool gzip = false, bool withEntitiesData = false) {
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                WriteChunk(ref writer, withCustomSnapshotData, chunkIdx, withEntitiesData);
                writer.CopyToBytes(ref result, gzip);
                writer.Dispose();
            }

            /// <inheritdoc cref="CreateChunkSnapshot(uint, bool, bool, bool)"/>
            /// <param name="filePath">Destination file path.</param>
            /// <param name="flushToDisk">When <c>true</c>, forces the OS to flush file buffers to physical storage.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateChunkSnapshot(uint chunkIdx, string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false, bool withEntitiesData = false) {
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                CreateChunkSnapshot(chunkIdx, ref writer, filePath, withCustomSnapshotData, gzip, flushToDisk, withEntitiesData);
                writer.Dispose();
            }

            /// <inheritdoc cref="CreateChunkSnapshot(uint, bool, bool, bool)"/>
            /// <param name="writer">The binary writer to serialize into.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateChunkSnapshot(uint chunkIdx, ref BinaryPackWriter writer, bool withCustomSnapshotData = true, bool withEntitiesData = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                WriteChunk(ref writer, withCustomSnapshotData, chunkIdx, withEntitiesData);
            }

            /// <inheritdoc cref="CreateChunkSnapshot(uint, bool, bool, bool)"/>
            /// <param name="writer">The binary writer to serialize into.</param>
            /// <param name="filePath">Destination file path.</param>
            /// <param name="flushToDisk">When <c>true</c>, forces the OS to flush file buffers to physical storage.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateChunkSnapshot(uint chunkIdx, ref BinaryPackWriter writer, string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false, bool withEntitiesData = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                WriteChunk(ref writer, withCustomSnapshotData, chunkIdx, withEntitiesData);
                writer.FlushToFile(filePath, gzip, flushToDisk);
            }

            /// <summary>
            /// Loads a single chunk snapshot from a binary reader.
            /// <para>
            /// When <c>entitiesAsNew.EntitiesAsNew</c> is <c>false</c> (default), the chunk must already be registered.
            /// When <c>true</c>, a fresh chunk is allocated and entities are created as new. Requires the snapshot
            /// to have been created with <c>withEntitiesData = true</c>.
            /// </para>
            /// </summary>
            /// <param name="reader">The binary reader containing chunk snapshot data.</param>
            /// <param name="entitiesAsNew">Controls whether entities are restored in-place or created as new copies.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadChunkSnapshot(BinaryPackReader reader, EntitiesAsNewParams entitiesAsNew = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                ReadChunk(ref reader, entitiesAsNew);
            }

            /// <inheritdoc cref="LoadChunkSnapshot(BinaryPackReader, EntitiesAsNewParams)"/>
            /// <param name="snapshot">Byte array containing the serialized chunk data.</param>
            /// <param name="gzip">When <c>true</c>, the input is decompressed from gzip before reading.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadChunkSnapshot(byte[] snapshot, bool gzip = false, EntitiesAsNewParams entitiesAsNew = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                if (gzip) {
                    var writer = BinaryPackWriter.CreateFromPool((uint) (snapshot.Length * 2));
                    writer.WriteGzipData(snapshot);
                    var reader = writer.AsReader();
                    ReadChunk(ref reader, entitiesAsNew);
                    writer.Dispose();
                } else {
                    var reader = new BinaryPackReader(snapshot, (uint) snapshot.Length, 0);
                    ReadChunk(ref reader, entitiesAsNew);
                }
            }

            /// <inheritdoc cref="LoadChunkSnapshot(BinaryPackReader, EntitiesAsNewParams)"/>
            /// <param name="worldSnapshotFilePath">Path to the file containing chunk snapshot data.</param>
            /// <param name="gzip">When <c>true</c>, the file is decompressed from gzip before reading.</param>
            /// <param name="byteSizeHint">Initial buffer size hint. When 0, automatically calculated.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadChunkSnapshot(string worldSnapshotFilePath, bool gzip = false, uint byteSizeHint = 0, EntitiesAsNewParams entitiesAsNew = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                CalculateByteSizeHint(ref byteSizeHint);
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                writer.WriteFromFile(worldSnapshotFilePath, gzip);
                var reader = writer.AsReader();
                ReadChunk(ref reader, entitiesAsNew);
                writer.Dispose();
            }
            
            [MethodImpl(AggressiveInlining)]
            private static void CalculateByteSizeHint(ref uint hint) {
                if (hint == 0) {
                    hint = CalculateByteSizeHint();
                }
            }

            [MethodImpl(AggressiveInlining)]
            private static uint CalculateByteSizeHint() {
                return (uint) (Data.Instance.HeuristicChunks.Length * 10240 * 4);
            }

            [MethodImpl(AggressiveInlining)]
            private static void WriteWorld(ref BinaryPackWriter writer, bool withCustomSnapshotData, ChunkWritingStrategy strategy, ReadOnlySpan<ushort> clusters, bool writeEvents) {
                var snapshotParams = new SnapshotWriteParams(SnapshotType.World);
                BeforeWrite(snapshotParams);
                
                var tempChunks = TempChunksData.Create();
                writer.WriteBool(writeEvents);
                Data.Instance.Write(ref writer, strategy, clusters, ref tempChunks, true);
                writer.WriteUint(tempChunks.ChunksCount);
                writer.WriteArrayUnmanaged(tempChunks.Chunks, 0, (int) tempChunks.ChunksCount);
                if (writeEvents) {
                    Data.Instance.WriteEvents(ref writer);
                }
                
                var chunks = new ReadOnlySpan<uint>(tempChunks.Chunks, 0, (int) tempChunks.ChunksCount);

                WriteCustomSnapshotData(ref writer, withCustomSnapshotData, snapshotParams, chunks);
                AfterWrite(snapshotParams, chunks);
                tempChunks.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            private static void ReadWorld(ref BinaryPackReader reader, bool hardReset = false) {
                var snapshotParams = new SnapshotReadParams(SnapshotType.World, false);
                BeforeRead(snapshotParams);

                var readEvents = reader.ReadBool();
                Data.Instance.Read(ref reader, true, hardReset);
                var chunksCount = reader.ReadUint();
                var tempChunks = reader.ReadArrayUnmanagedPooled<uint>(out var h).Array;
                if (readEvents) {
                    Data.Instance.ReadEvents(ref reader);
                }
                
                var chunks = new ReadOnlySpan<uint>(tempChunks, 0, (int) chunksCount);

                ReadCustomSnapshotData(ref reader, snapshotParams, chunks);
                AfterRead(snapshotParams, chunks);
                
                h.Return();
            }

            [MethodImpl(AggressiveInlining)]
            private static void WriteCluster(ref BinaryPackWriter writer, bool withCustomSnapshotData, ChunkWritingStrategy strategy, ushort clusterId, bool withEntitiesData) {
                var snapshotParams = new SnapshotWriteParams(SnapshotType.Cluster);
                BeforeWrite(snapshotParams);
                
                var tempChunks = TempChunksData.Create();
                Data.Instance.FillClusterChunks(strategy, clusterId, ref tempChunks);
                
                writer.WriteBool(withEntitiesData);
                writer.WriteUshort(clusterId);
                writer.WriteUshort((ushort)Data.Instance.GetAllComponentsHandles().Length);
                writer.WriteUint(tempChunks.ChunksCount);
                writer.WriteArrayUnmanaged(tempChunks.Chunks, 0, (int) tempChunks.ChunksCount);
                for (uint i = 0; i < tempChunks.ChunksCount; i++) {
                    var chunkIdx = tempChunks.Chunks[i];
                    writer.WriteUint(chunkIdx);
                    if (withEntitiesData) {
                        Data.Instance.WriteChunk(ref writer, chunkIdx, false);
                    }
                    Data.Instance.WriteDataChunk(ref writer, chunkIdx);
                }
                
                var chunks = new ReadOnlySpan<uint>(tempChunks.Chunks, 0, (int) tempChunks.ChunksCount);

                WriteCustomSnapshotData(ref writer, withCustomSnapshotData, snapshotParams, chunks);
                AfterWrite(snapshotParams, chunks);
                tempChunks.Dispose();
            }

            [MethodImpl(AggressiveInlining)]
            private static void ReadCluster(ref BinaryPackReader reader, EntitiesAsNewParams entitiesAsNewParams) {
                var snapshotParams = new SnapshotReadParams(SnapshotType.Cluster, entitiesAsNewParams.EntitiesAsNew);
                BeforeRead(snapshotParams);

                var withEntitiesData = reader.ReadBool();
                var clusterId = reader.ReadUshort();
                
                if (!withEntitiesData && entitiesAsNewParams.EntitiesAsNew) {
                    throw new StaticEcsException($"World<{typeof(TWorld)}>", "ReadChunk", $"Cluster {clusterId} does not have information about entities, use withEntitiesData = true when saving a cluster");
                }
                
                if (entitiesAsNewParams.EntitiesAsNew) {
                    if (Data.Instance.ClusterIsRegisteredInternal(clusterId)) {
                        throw new StaticEcsException($"World<{typeof(TWorld)}>", "ReadCluster", $"Cluster {clusterId} already registered");
                    }
                    
                    RegisterCluster(entitiesAsNewParams.ClusterId);
                } else if (!Data.Instance.ClusterIsRegisteredInternal(clusterId)) {
                    throw new StaticEcsException($"World<{typeof(TWorld)}>", "ReadCluster", $"Cluster {clusterId} is not registered");
                }
                
                var componentsPoolCount = reader.ReadUshort();
                var chunksCount = reader.ReadUint();
                var tempChunks = reader.ReadArrayUnmanagedPooled<uint>(out var h).Array!;
                for (var i = 0; i < chunksCount; i++) {
                    var chunkIdx = reader.ReadUint();
                    if (entitiesAsNewParams.EntitiesAsNew) {
                        chunkIdx = FindNextSelfFreeChunk().ChunkIdx;
                        tempChunks[i] = chunkIdx;
                        RegisterChunk(chunkIdx, ChunkOwnerType.Self, entitiesAsNewParams.ClusterId);
                    }
                    if (withEntitiesData) {
                        Data.Instance.ReadChunk(ref reader, chunkIdx, false);
                    }
                    Data.Instance.ReadDataChunk(ref reader, chunkIdx, componentsPoolCount);
                }

                Data.Instance.LoadCluster(entitiesAsNewParams.EntitiesAsNew ? entitiesAsNewParams.ClusterId : clusterId);
                
                var chunks = new ReadOnlySpan<uint>(tempChunks, 0, (int) chunksCount);

                ReadCustomSnapshotData(ref reader, snapshotParams, chunks);
                AfterRead(snapshotParams, chunks);
                h.Return();
            }

            [MethodImpl(AggressiveInlining)]
            private static void WriteChunk(ref BinaryPackWriter writer, bool withCustomSnapshotData, uint chunkIdx, bool withEntitiesData) {
                if (!Data.Instance.ChunkIsRegisteredInternal(chunkIdx)) {
                    throw new StaticEcsException($"World<{typeof(TWorld)}>", "WriteChunk", $"Chunk {chunkIdx} is not registered");
                }
                
                var snapshotParams = new SnapshotWriteParams(SnapshotType.Chunk);
                BeforeWrite(snapshotParams);
                
                writer.WriteUint(chunkIdx);
                writer.WriteBool(withEntitiesData);

                if (withEntitiesData) {
                    Data.Instance.WriteChunk(ref writer, chunkIdx, false);
                }
                
                writer.WriteUshort((ushort)Data.Instance.GetAllComponentsHandles().Length);
                Data.Instance.WriteDataChunk(ref writer, chunkIdx);
                
                ReadOnlySpan<uint> chunks = stackalloc uint[1] { chunkIdx };

                WriteCustomSnapshotData(ref writer, withCustomSnapshotData, snapshotParams, chunks);
                AfterWrite(snapshotParams, chunks);
            }

            [MethodImpl(AggressiveInlining)]
            private static void ReadChunk(ref BinaryPackReader reader, EntitiesAsNewParams entitiesAsNewParams) {
                var snapshotParams = new SnapshotReadParams(SnapshotType.Chunk, entitiesAsNewParams.EntitiesAsNew);
                BeforeRead(snapshotParams);

                var chunkIdx = reader.ReadUint();
                var withEntitiesData = reader.ReadBool();

                if (!withEntitiesData && entitiesAsNewParams.EntitiesAsNew) {
                    throw new StaticEcsException($"World<{typeof(TWorld)}>", "ReadChunk", $"Chunk {chunkIdx} does not have information about entities, use withEntitiesData = true when saving a chunk");
                }

                if (entitiesAsNewParams.EntitiesAsNew) {
                    if (!ClusterIsRegistered(entitiesAsNewParams.ClusterId)) {
                        RegisterCluster(entitiesAsNewParams.ClusterId);
                    }

                    chunkIdx = FindNextSelfFreeChunk().ChunkIdx;
                    RegisterChunk(chunkIdx, ChunkOwnerType.Self, entitiesAsNewParams.ClusterId);
                    Data.Instance.ReadChunk(ref reader, chunkIdx, false);
                } else if (!Data.Instance.ChunkIsRegisteredInternal(chunkIdx)) {
                    throw new StaticEcsException($"World<{typeof(TWorld)}>", "ReadChunk", $"Chunk {chunkIdx} is not registered");
                }
                
                var componentsPoolCount = reader.ReadUshort();
                Data.Instance.ReadDataChunk(ref reader, chunkIdx, componentsPoolCount);
                Data.Instance.LoadChunk(chunkIdx);
                
                ReadOnlySpan<uint> chunks = stackalloc uint[1] { chunkIdx };
                
                ReadCustomSnapshotData(ref reader, snapshotParams, chunks);
                AfterRead(snapshotParams, chunks);
            }

            [MethodImpl(AggressiveInlining)]
            private static void BeforeWrite(SnapshotWriteParams snapshotParams) {
                for (var i = 0; i < PreCreateSnapshotCallbacks.Count; i++) {
                    PreCreateSnapshotCallbacks[i](snapshotParams);
                }
            }

            [MethodImpl(AggressiveInlining)]
            private static void BeforeRead(SnapshotReadParams snapshotParams) {
                for (var i = 0; i < PreLoadSnapshotCallbacks.Count; i++) {
                    PreLoadSnapshotCallbacks[i](snapshotParams);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            private static void WriteCustomSnapshotData(ref BinaryPackWriter writer, bool withCustomSnapshotData, SnapshotWriteParams snapshotParams, ReadOnlySpan<uint> chunks) {
                if (withCustomSnapshotData) {
                    WriteSnapshotData(ref writer, snapshotParams);
                    WriteEntitySnapshotData(ref writer, snapshotParams, chunks);
                } else {
                    writer.WriteInt(0);
                    writer.WriteInt(0);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            private static void ReadCustomSnapshotData(ref BinaryPackReader reader, SnapshotReadParams snapshotParams, ReadOnlySpan<uint> chunks) {
                ReadSnapshotData(ref reader, snapshotParams);
                ReadEntitySnapshotData(ref reader, snapshotParams, chunks);
            }
            
            [MethodImpl(AggressiveInlining)]
            private static void AfterWrite(SnapshotWriteParams snapshotParams, ReadOnlySpan<uint> chunks) {
                for (var i = 0; i < PostCreateSnapshotCallbacks.Count; i++) {
                    PostCreateSnapshotCallbacks[i](snapshotParams);
                }

                if (OnCreateEntitySnapshotActions.Count > 0) {
                    Query().For(ref snapshotParams, chunks, static (ref SnapshotWriteParams p, Entity entity) => {
                        for (var j = 0; j < OnCreateEntitySnapshotActions.Count; j++) {
                            OnCreateEntitySnapshotActions[j](entity, p);
                        }
                    }, EntityStatusType.Any, QueryMode.Flexible);
                }
            }
            
            [MethodImpl(AggressiveInlining)]
            private static void AfterRead(SnapshotReadParams snapshotParams, ReadOnlySpan<uint> chunks) {
                for (var i = 0; i < PostLoadSnapshotCallbacksType.Count; i++) {
                    PostLoadSnapshotCallbacksType[i](snapshotParams);
                }
                
                if (OnRestoreEntityFromSnapshotActions.Count > 0) {
                    Query().For(ref snapshotParams, chunks, static (ref SnapshotReadParams p, Entity entity) => {
                        for (var j = 0; j < OnRestoreEntityFromSnapshotActions.Count; j++) {
                            OnRestoreEntityFromSnapshotActions[j](entity, p);
                        }
                    }, EntityStatusType.Any, QueryMode.Flexible);
                }
            }
            #endregion

            #region ENTITIES
            /// <summary>
            /// Creates a GID store snapshot: serializes only entity metadata (presence masks, versions, cluster assignments,
            /// entity types) <b>without</b> any component or tag data. Returns the result as a byte array.
            /// <para>
            /// Use this to pre-allocate entity slots in a world before loading component/tag data separately
            /// (e.g., from cluster or chunk snapshots). This is the lightest snapshot type for entity structure.
            /// Load via <see cref="RestoreFromGIDStoreSnapshot(BinaryPackReader)"/>.
            /// </para>
            /// </summary>
            /// <param name="gzip">When <c>true</c>, the output is gzip-compressed.</param>
            /// <param name="strategy">Chunk ownership filter.</param>
            /// <param name="clusters">Specific cluster IDs to include, or empty for all active clusters.</param>
            /// <returns>A byte array containing the serialized entity metadata.</returns>
            [MethodImpl(AggressiveInlining)]
            public static byte[] CreateGIDStoreSnapshot(bool gzip = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, ReadOnlySpan<ushort> clusters = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                var tempChunks = TempChunksData.Create();
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                Data.Instance.Write(ref writer, strategy, clusters, ref tempChunks, false);
                var result = writer.CopyToBytes(gzip);
                writer.Dispose();
                tempChunks.Dispose();
                return result;
            }
            
            /// <inheritdoc cref="CreateGIDStoreSnapshot(bool, ChunkWritingStrategy, ReadOnlySpan{ushort})"/>
            /// <param name="result">Reference to the destination byte array. Resized if necessary.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateGIDStoreSnapshot(ref byte[] result, bool gzip = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, ReadOnlySpan<ushort> clusters = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                var tempChunks = TempChunksData.Create();
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                Data.Instance.Write(ref writer, strategy, clusters, ref tempChunks, false);
                writer.CopyToBytes(ref result, gzip);
                writer.Dispose();
                tempChunks.Dispose();
            }
            
            /// <inheritdoc cref="CreateGIDStoreSnapshot(bool, ChunkWritingStrategy, ReadOnlySpan{ushort})"/>
            /// <param name="filePath">Destination file path.</param>
            /// <param name="flushToDisk">When <c>true</c>, forces the OS to flush file buffers to physical storage.</param>
            [MethodImpl(AggressiveInlining)]
            public static void CreateGIDStoreSnapshot(string filePath, bool gzip = false, bool flushToDisk = false, ChunkWritingStrategy strategy = ChunkWritingStrategy.All, ReadOnlySpan<ushort> clusters = default) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                var tempChunks = TempChunksData.Create();
                var writer = BinaryPackWriter.CreateFromPool(CalculateByteSizeHint());
                Data.Instance.Write(ref writer, strategy, clusters, ref tempChunks, false);
                writer.FlushToFile(filePath, gzip, flushToDisk);
                writer.Dispose();
                tempChunks.Dispose();
            }
            
            /// <summary>
            /// Restores entity metadata (presence masks, versions, cluster assignments) from a GID store snapshot.
            /// No component or tag data is loaded — only entity slots are allocated.
            /// <para>
            /// If the world is already initialized, it is cleared first. If only created, it is initialized from the snapshot.
            /// After this call, use cluster/chunk snapshots or entity snapshots to populate component/tag data.
            /// </para>
            /// </summary>
            /// <param name="reader">The binary reader containing GID store snapshot data.</param>
            /// <param name="hardReset">When <c>true</c>, performs a hard reset (no OnDelete hooks, faster) instead of standard
            /// entity deletion when clearing the existing world before loading. Has no effect if the world is not yet initialized.
            /// Default is <c>false</c>.</param>
            [MethodImpl(AggressiveInlining)]
            public static void RestoreFromGIDStoreSnapshot(BinaryPackReader reader, bool hardReset = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                Data.Instance.Read(ref reader, false, hardReset);
            }

            /// <inheritdoc cref="RestoreFromGIDStoreSnapshot(BinaryPackReader, bool)"/>
            /// <param name="snapshot">Byte array containing the GID store snapshot.</param>
            /// <param name="gzip">When <c>true</c>, the input is decompressed from gzip before reading.</param>
            [MethodImpl(AggressiveInlining)]
            public static void RestoreFromGIDStoreSnapshot(byte[] snapshot, bool gzip = false, bool hardReset = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                if (gzip) {
                    var writer = BinaryPackWriter.CreateFromPool((uint) (snapshot.Length * 2));
                    writer.WriteGzipData(snapshot);
                    var reader = writer.AsReader();
                    Data.Instance.Read(ref reader, false, hardReset);
                    writer.Dispose();
                } else {
                    var reader = new BinaryPackReader(snapshot, (uint) snapshot.Length, 0);
                    Data.Instance.Read(ref reader, false, hardReset);
                }
            }

            /// <inheritdoc cref="RestoreFromGIDStoreSnapshot(BinaryPackReader, bool)"/>
            /// <param name="worldSnapshotFilePath">Path to the file containing GID store snapshot data.</param>
            /// <param name="gzip">When <c>true</c>, the file is decompressed from gzip before reading.</param>
            /// <param name="byteSizeHint">Initial buffer size hint. When 0, automatically calculated.</param>
            [MethodImpl(AggressiveInlining)]
            public static void RestoreFromGIDStoreSnapshot(string worldSnapshotFilePath, bool gzip = false, uint byteSizeHint = 0, bool hardReset = false) {
                #if FFS_ECS_DEBUG
                AssertWorldIsCreatedOrInitialized(WorldTypeName);
                #endif
                CalculateByteSizeHint(ref byteSizeHint);
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                writer.WriteFromFile(worldSnapshotFilePath, gzip);
                var reader = writer.AsReader();
                Data.Instance.Read(ref reader, false, hardReset);
                writer.Dispose();
            }

            /// <summary>
            /// Creates a reusable <see cref="EntitiesWriter"/> for selectively serializing individual entities.
            /// <para>
            /// Unlike world/cluster/chunk snapshots that serialize entire spatial regions, the entities writer
            /// allows cherry-picking specific entities. Call <see cref="EntitiesWriter.Write"/> or
            /// <see cref="EntitiesWriter.WriteAllEntities"/> to add entities, then <see cref="EntitiesWriter.CreateSnapshot(bool, bool)"/>
            /// to produce the byte output. The writer can be reused for multiple snapshots before disposal.
            /// Load the result via <see cref="LoadEntitiesSnapshot(BinaryPackReader, bool, QueryFunctionWithEntity{TWorld})"/>.
            /// </para>
            /// </summary>
            /// <param name="byteSizeHint">Initial buffer size hint. When 0, automatically calculated from world capacity.</param>
            /// <returns>A new <see cref="EntitiesWriter"/> instance. Must be disposed when no longer needed.</returns>
            [MethodImpl(AggressiveInlining)]
            public static EntitiesWriter CreateEntitiesSnapshotWriter(uint byteSizeHint = 0) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                
                CalculateByteSizeHint(ref byteSizeHint);
                return new EntitiesWriter(
                    byteSizeHint: byteSizeHint
                );
            }

            /// <summary>
            /// Loads an entities snapshot, restoring individual entities with their full component and tag data.
            /// <para>
            /// Each entity in the snapshot is either restored to its original position (when <paramref name="entitiesAsNew"/> is <c>false</c>,
            /// using <see cref="EntityGID"/> for identity matching) or created as a brand-new entity (when <c>true</c>).
            /// Components are matched by GUID; unknown component/tag types are handled by delete migrators or silently skipped.
            /// </para>
            /// </summary>
            /// <param name="reader">The binary reader containing the entities snapshot data.</param>
            /// <param name="entitiesAsNew">When <c>true</c>, entities are created with fresh IDs instead of restoring original positions.</param>
            /// <param name="onLoad">Optional callback invoked for each entity after it has been fully restored. Can be used for post-load initialization.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadEntitiesSnapshot(BinaryPackReader reader, bool entitiesAsNew = false, QueryFunctionWithEntity<TWorld> onLoad = null) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif

                var snapshotParams = new SnapshotReadParams(SnapshotType.Entities, entitiesAsNew);

                BeforeRead(snapshotParams);

                var entitiesCount = reader.ReadUint();
                var componentGuidById = reader.ReadArrayUnmanagedPooled<Guid>(out var h1).Array;
                
                var dynamicComponentsPoolMap = ArrayPool<ComponentsHandle>.Shared.Rent(componentGuidById!.Length);
                for (var i = 0; i < componentGuidById.Length; i++) {
                    if (Data.Instance.TryGetComponentPool(componentGuidById[i], out var pool)) {
                        dynamicComponentsPoolMap[i] = pool;
                    } else {
                        dynamicComponentsPoolMap[i] = default;
                    }
                }

                var entities = ArrayPool<Entity>.Shared.Rent((int) entitiesCount);

                for (var i = 0; i < entitiesCount; i++) {
                    var gid = reader.ReadEntityGID();
                    var type = reader.ReadByte();

                    Entity entity;
                    if (entitiesAsNew) {
                        Data.Instance.CreateEntity(type, gid.ClusterId, out entity);
                    } else {
                        Data.Instance.LoadEntity(gid, out entity);
                    }
                    entities[i] = entity;
                    var disabled = reader.ReadBool();
                    if (disabled) {
                        entity.Disable();
                    }
                    Data.Instance.ReadEntity(ref reader, componentGuidById, dynamicComponentsPoolMap, entity);
                }

                ArrayPool<ComponentsHandle>.Shared.Return(dynamicComponentsPoolMap, true);

                h1.Return();

                ReadSnapshotData(ref reader, snapshotParams);
                var count = reader.ReadInt();

                for (var i = 0; i < count; i++) {
                    var key = reader.ReadGuid();
                    var version = reader.ReadUshort();
                    var byteSize = reader.ReadUint();
                    if (SnapshotDataEntitySerializers.TryGetValue(key, out var val)) {
                        for (var j = 0; j < entitiesCount; j++) {
                            val.reader(ref reader, entities[j], version, snapshotParams);
                        }
                    } else {
                        reader.SkipNext(byteSize);
                    }
                }

                for (var i = 0; i < PostLoadSnapshotCallbacksType.Count; i++) {
                    PostLoadSnapshotCallbacksType[i](snapshotParams);
                }

                if (OnRestoreEntityFromSnapshotActions.Count > 0) {
                    for (var i = 0; i < entitiesCount; i++) {
                        for (var j = 0; j < OnRestoreEntityFromSnapshotActions.Count; j++) {
                            OnRestoreEntityFromSnapshotActions[j](entities[i], snapshotParams);
                        }
                    }
                }

                if (onLoad != null) {
                    for (var j = 0; j < entitiesCount; j++) {
                        onLoad.Invoke(entities[j]);
                    }
                }

                ArrayPool<Entity>.Shared.Return(entities);
            }

            /// <inheritdoc cref="LoadEntitiesSnapshot(BinaryPackReader, bool, QueryFunctionWithEntity{TWorld})"/>
            /// <param name="data">Byte array containing the serialized entities snapshot.</param>
            /// <param name="gzip">When <c>true</c>, the input is decompressed from gzip before reading.</param>
            /// <param name="gzipByteSizeHint">Buffer size hint for gzip decompression. When 0, automatically calculated.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadEntitiesSnapshot(byte[] data, bool entitiesAsNew = false, QueryFunctionWithEntity<TWorld> onLoad = null, bool gzip = false, uint gzipByteSizeHint = 0) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                if (gzip) {
                    CalculateByteSizeHint(ref gzipByteSizeHint);
                    var writer = BinaryPackWriter.CreateFromPool(gzipByteSizeHint);
                    writer.WriteGzipData(data);
                    LoadEntitiesSnapshot(writer.AsReader(), entitiesAsNew, onLoad);
                    writer.Dispose();
                } else {
                    var reader = new BinaryPackReader(data, (uint) data.Length, 0);
                    LoadEntitiesSnapshot(reader, entitiesAsNew, onLoad);
                }
            }

            /// <inheritdoc cref="LoadEntitiesSnapshot(BinaryPackReader, bool, QueryFunctionWithEntity{TWorld})"/>
            /// <param name="filePath">Path to the file containing the entities snapshot.</param>
            /// <param name="gzip">When <c>true</c>, the file is decompressed from gzip before reading.</param>
            /// <param name="byteSizeHint">Buffer size hint. When 0, automatically calculated.</param>
            [MethodImpl(AggressiveInlining)]
            public static void LoadEntitiesSnapshot(string filePath, bool entitiesAsNew = false, QueryFunctionWithEntity<TWorld> onLoad = null, bool gzip = false, uint byteSizeHint = 0) {
                #if FFS_ECS_DEBUG
                AssertWorldIsInitialized(WorldTypeName);
                #endif
                CalculateByteSizeHint(ref byteSizeHint);
                var writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                writer.WriteFromFile(filePath, gzip);
                var reader = writer.AsReader();
                LoadEntitiesSnapshot(reader, entitiesAsNew, onLoad);
                writer.Dispose();
            }

            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
            #endif
            /// <summary>
            /// A reusable, disposable writer for selectively serializing individual entities into a snapshot.
            /// <para>
            /// <b>Usage pattern:</b>
            /// <code>
            /// using var writer = Serializer.CreateEntitiesSnapshotWriter();
            /// writer.Write(entity1);
            /// writer.Write(entity2);
            /// byte[] snapshot = writer.CreateSnapshot();
            /// // Writer is automatically reset — can write more entities and create another snapshot.
            /// </code>
            /// </para>
            /// <para>
            /// Each entity is serialized with its <see cref="EntityGID"/>, entity type, disabled state,
            /// and all components (via <see cref="IComponent.Write{TW}"/>) and tags.
            /// The <see cref="WriteAndUnload"/> variant additionally unloads the entity after writing,
            /// removing all its components and tags from the world while preserving its slot for later reload.
            /// </para>
            /// <para>
            /// The writer maintains an internal buffer and entity ID list. Call <see cref="Dispose"/> when finished
            /// to return pooled arrays and the binary writer to their pools.
            /// </para>
            /// </summary>
            public struct EntitiesWriter : IDisposable {
                internal BinaryPackWriter Writer;
                internal Entity[] EntityIds;
                internal uint EntitiesCount;
                internal uint StartWriterPosition;
                #if FFS_ECS_DEBUG
                internal bool Destroyed;
                #endif

                /// <param name="byteSizeHint">Initial buffer size hint for the internal binary writer.</param>
                [MethodImpl(AggressiveInlining)]
                public EntitiesWriter(uint byteSizeHint) {
                    EntitiesCount = 0;
                    EntityIds = ArrayPool<Entity>.Shared.Rent(Data.Instance.HeuristicChunks.Length << Const.ENTITIES_IN_CHUNK_SHIFT);
                    Writer = BinaryPackWriter.CreateFromPool(byteSizeHint);
                    Writer.Skip(sizeof(uint));
                    Data.Instance.WriteGuids(ref Writer);
                    StartWriterPosition = Writer.Position;
                    
                    BeforeWrite(new SnapshotWriteParams(SnapshotType.Entities));
                    
                    #if FFS_ECS_DEBUG
                    Destroyed = false;
                    #endif
                }

                /// <summary>
                /// Serializes a single entity into the writer's internal buffer.
                /// Writes the entity's GID, type, disabled state, all components (with their data via Write hooks), and all tags.
                /// The entity remains alive and unchanged in the world after this call.
                /// </summary>
                /// <param name="entity">The entity to serialize. Must be alive and loaded.</param>
                [MethodImpl(AggressiveInlining)]
                public void Write(Entity entity) {
                    #if FFS_ECS_DEBUG
                    Assert("EntitiesWriter", !Destroyed, "EntitiesWriter is destroyed");
                    #endif
  
                    Data.Instance.WriteEntity(ref Writer, entity, false);
                    EntityIds[EntitiesCount++] = entity;
                }

                /// <summary>
                /// Serializes a single entity and then unloads it from the world.
                /// After this call, the entity's components and tags are removed, but its slot remains allocated
                /// (the entity is marked as unloaded, not destroyed). It can be reloaded later from a snapshot.
                /// </summary>
                /// <param name="entity">The entity to serialize and unload. Must be alive and loaded.</param>
                [MethodImpl(AggressiveInlining)]
                public void WriteAndUnload(Entity entity) {
                    #if FFS_ECS_DEBUG
                    Assert("EntitiesWriter", !Destroyed, "EntitiesWriter is destroyed");
                    #endif

                    Data.Instance.WriteEntity(ref Writer, entity, true);
                    EntityIds[EntitiesCount++] = entity;
                }

                /// <summary>
                /// Serializes all entities in the world (both active and disabled) into the writer's buffer.
                /// Equivalent to iterating all entities and calling <see cref="Write"/> on each.
                /// </summary>
                [MethodImpl(AggressiveInlining)]
                public void WriteAllEntities() {
                    #if FFS_ECS_DEBUG
                    Assert("EntitiesWriter", !Destroyed, "EntitiesWriter is destroyed");
                    #endif
                    
                    Query().For(ref this, (ref EntitiesWriter self, Entity entity) => self.Write(entity), EntityStatusType.Any);
                }

                /// <summary>
                /// Serializes all entities in the world and unloads each one after serialization.
                /// Equivalent to iterating all entities and calling <see cref="WriteAndUnload"/> on each.
                /// After this call, no entities remain loaded in the world.
                /// </summary>
                [MethodImpl(AggressiveInlining)]
                public void WriteAndUnloadAllEntities() {
                    #if FFS_ECS_DEBUG
                    Assert("EntitiesWriter", !Destroyed, "EntitiesWriter is destroyed");
                    #endif
                    
                    Query().For(ref this, (ref EntitiesWriter self, Entity entity) => self.WriteAndUnload(entity), EntityStatusType.Any);
                }

                /// <summary>
                /// Finalizes the current batch of written entities into a snapshot byte array.
                /// After this call, the writer is reset and ready for a new batch of <see cref="Write"/> calls.
                /// </summary>
                /// <param name="withCustomSnapshotData">When <c>true</c>, includes custom snapshot handler data.</param>
                /// <param name="gzip">When <c>true</c>, the output is gzip-compressed.</param>
                /// <returns>A byte array containing the entities snapshot, loadable via <c>LoadEntitiesSnapshot</c>.</returns>
                [MethodImpl(AggressiveInlining)]
                public byte[] CreateSnapshot(bool withCustomSnapshotData = true, bool gzip = false) {
                    #if FFS_ECS_DEBUG
                    Assert("EntitiesWriter", !Destroyed, "EntitiesWriter is destroyed");
                    #endif
                    Writer.WriteUintAt(0, EntitiesCount);
                    CreateCustomSnapshot(withCustomSnapshotData);
                    var result = Writer.CopyToBytes(gzip);
                    Writer.Position = StartWriterPosition;
                    EntitiesCount = 0;
                    return result;
                }

                /// <inheritdoc cref="CreateSnapshot(bool, bool)"/>
                /// <param name="result">Reference to the destination byte array. Resized if necessary.</param>
                [MethodImpl(AggressiveInlining)]
                public void CreateSnapshot(ref byte[] result, bool withCustomSnapshotData = true, bool gzip = false) {
                    #if FFS_ECS_DEBUG
                    Assert("EntitiesWriter", !Destroyed, "EntitiesWriter is destroyed");
                    #endif
                    Writer.WriteUintAt(0, EntitiesCount);
                    CreateCustomSnapshot(withCustomSnapshotData);
                    Writer.CopyToBytes(ref result, gzip);
                    Writer.Position = StartWriterPosition;
                    EntitiesCount = 0;
                }

                /// <inheritdoc cref="CreateSnapshot(bool, bool)"/>
                /// <param name="filePath">Destination file path.</param>
                /// <param name="flushToDisk">When <c>true</c>, forces the OS to flush file buffers to physical storage.</param>
                [MethodImpl(AggressiveInlining)]
                public void CreateSnapshot(string filePath, bool withCustomSnapshotData = true, bool gzip = false, bool flushToDisk = false) {
                    #if FFS_ECS_DEBUG
                    Assert("EntitiesWriter", !Destroyed, "EntitiesWriter is destroyed");
                    #endif
                    Writer.WriteUintAt(0, EntitiesCount);
                    CreateCustomSnapshot(withCustomSnapshotData);
                    Writer.FlushToFile(filePath, gzip, flushToDisk);
                    Writer.Position = StartWriterPosition;
                    EntitiesCount = 0;
                }

                [MethodImpl(AggressiveInlining)]
                private void CreateCustomSnapshot(bool withCustomSnapshotData) {
                    var snapshotParams = new SnapshotWriteParams(SnapshotType.Entities);
                    
                    if (withCustomSnapshotData) {
                        WriteSnapshotData(ref Writer, snapshotParams);
                        Writer.WriteInt(SnapshotDataEntitySerializers.Count);
                        foreach (var (key, (writer, _, version)) in SnapshotDataEntitySerializers) {
                            Writer.WriteGuid(key);
                            Writer.WriteUshort(version);
                            var point = Writer.MakePoint(sizeof(uint));
                            for (var i = 0; i < EntitiesCount; i++) {
                                writer(ref Writer, EntityIds[i], snapshotParams);
                            }

                            Writer.WriteUintAt(point, Writer.Position - (point + sizeof(uint)));
                        }
                    } else {
                        Writer.WriteInt(0);
                        Writer.WriteInt(0);
                    }

                    for (var i = 0; i < PostCreateSnapshotCallbacks.Count; i++) {
                        PostCreateSnapshotCallbacks[i](snapshotParams);
                    }

                    if (OnCreateEntitySnapshotActions.Count > 0) {
                        for (var i = 0; i < EntitiesCount; i++) {
                            for (var j = 0; j < OnCreateEntitySnapshotActions.Count; j++) {
                                OnCreateEntitySnapshotActions[j](EntityIds[i], snapshotParams);
                            }
                        }
                    }
                }

                /// <summary>
                /// Releases the internal binary writer and entity ID buffer back to their respective pools.
                /// Must be called when the writer is no longer needed. After disposal, do not call any other methods.
                /// </summary>
                [MethodImpl(AggressiveInlining)]
                public void Dispose() {
                    Writer.Dispose();
                    if (EntityIds != null) {
                        ArrayPool<Entity>.Shared.Return(EntityIds);
                    }
                }
            }
            #endregion
        }
    }
}