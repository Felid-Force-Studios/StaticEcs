#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {

    /// <summary>
    /// Status of an <see cref="EntityGID"/> or <see cref="EntityGIDCompact"/> within a world.
    /// Returned by <see cref="EntityGID.Status{TWorld}"/> and <see cref="EntityGIDCompact.Status{TWorld}"/>.
    /// </summary>
    public enum GIDStatus : byte {
        /// <summary>The entity exists, its version matches, and it is loaded — the entity is alive and accessible.</summary>
        Active = 0,
        /// <summary>The entity does not exist or its version/cluster doesn't match — stale reference.</summary>
        NotActual = 1,
        /// <summary>The entity exists and its version matches, but it is currently unloaded.</summary>
        NotLoaded = 2
    }
    
    
    /// <summary>
    /// Full-size global identifier for an entity (8 bytes, explicit union layout over ulong).
    /// <para>
    /// EntityGID serves as a persistent, serializable reference to an entity that remains meaningful
    /// across world reloads, save/load cycles, and network synchronization. Unlike <see cref="World{TWorld}.Entity"/>,
    /// which is a lightweight runtime handle valid only while the entity is alive, EntityGID embeds a
    /// <see cref="Version"/> counter that detects stale references — if the entity was destroyed and its
    /// slot reused, the version will no longer match, making it safe to detect dangling references.
    /// </para>
    /// <para>
    /// The struct uses <see cref="LayoutKind.Explicit"/> so that <see cref="Raw"/> (ulong) overlaps
    /// <see cref="Id"/> (uint, offset 0), <see cref="ClusterId"/> (ushort, offset 4), and
    /// <see cref="Version"/> (ushort, offset 6). This allows zero-cost reinterpretation between the
    /// individual fields and the packed 8-byte representation used for serialization and hashing.
    /// </para>
    /// <para>
    /// For scenarios with tight memory budgets (e.g. network packets), consider using
    /// <see cref="EntityGIDCompact"/> which packs the same information into 4 bytes at the cost of
    /// reduced range for Chunk (2 bits) and ClusterId (2 bits).
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct EntityGID : IEquatable<EntityGID>, IEquatable<EntityGIDCompact> {
        /// <summary>
        /// Raw 8-byte representation that overlaps all other fields via explicit layout.
        /// Useful for fast equality checks, hashing, and binary serialization without
        /// touching individual fields. Two EntityGIDs are equal if and only if their Raw values match.
        /// </summary>
        [FieldOffset(0)] public readonly ulong Raw;

        /// <summary>
        /// Internal entity index without the <c>ENTITY_ID_OFFSET</c> applied.
        /// This is the zero-based slot index used internally by the world's storage.
        /// To obtain the value expected by <see cref="World{TWorld}.Entity"/>, use <see cref="EntityID"/>.
        /// Occupies bytes [0..3] of <see cref="Raw"/>.
        /// </summary>
        [FieldOffset(0)] public readonly uint Id;

        /// <summary>
        /// Cluster identifier this entity belongs to. Clusters are logical groupings of entities
        /// (e.g. spatial regions, gameplay zones, network authority groups) that the world uses
        /// for chunk-level partitioning and selective loading/unloading.
        /// Occupies bytes [4..5] of <see cref="Raw"/>.
        /// </summary>
        [FieldOffset(4)] public readonly ushort ClusterId;

        /// <summary>
        /// Generation counter for the entity slot. Each time an entity is destroyed and a new entity
        /// is created in the same slot, the version is incremented. Comparing this value against the
        /// world's current version for the slot detects stale/dangling references.
        /// A version of 0 is reserved as "invalid/uninitialized" and will throw in debug builds.
        /// Occupies bytes [6..7] of <see cref="Raw"/>.
        /// </summary>
        [FieldOffset(6)] public readonly ushort Version;

        /// <summary>
        /// Creates an EntityGID from individual components.
        /// </summary>
        /// <param name="id">
        /// Internal entity index (without <c>ENTITY_ID_OFFSET</c>).
        /// Typically obtained from <c>entity.Id</c> or the world's entity allocation.
        /// </param>
        /// <param name="version">
        /// Generation counter for this entity slot. Must be greater than 0.
        /// In debug builds, passing 0 throws <see cref="StaticEcsException"/>.
        /// </param>
        /// <param name="clusterId">Cluster identifier this entity belongs to.</param>
        [MethodImpl(AggressiveInlining)]
        public EntityGID(uint id, ushort version, ushort clusterId) {
            Raw = 0;
            Id = id;
            Version = version;
            ClusterId = clusterId;
            #if FFS_ECS_DEBUG
            if (version == 0) throw new StaticEcsException("EntityGID version 0 is unsupported.");
            #endif
        }

        /// <summary>
        /// Creates an EntityGID from a raw ulong value, typically obtained from deserialization
        /// or from a previously stored <see cref="Raw"/> value. The individual fields
        /// (<see cref="Id"/>, <see cref="ClusterId"/>, <see cref="Version"/>) are populated
        /// automatically via the explicit union layout.
        /// </summary>
        /// <param name="rawValue">The packed 8-byte value containing all fields.</param>
        [MethodImpl(AggressiveInlining)]
        public EntityGID(ulong rawValue) {
            Id = 0;
            ClusterId = 0;
            Version = 0;
            Raw = rawValue;
            #if FFS_ECS_DEBUG
            if (Version == 0) throw new StaticEcsException("EntityGID version 0 is unsupported.");
            #endif
        }

        /// <summary>
        /// Zero-based chunk index derived from <see cref="Id"/>.
        /// Each chunk holds <c>Const.ENTITIES_IN_CHUNK</c> (4096) entities.
        /// Used internally for chunk-level operations such as loading, unloading, and iteration.
        /// </summary>
        public uint Chunk {
            [MethodImpl(AggressiveInlining)] get => Id >> Const.ENTITIES_IN_CHUNK_SHIFT;
        }

        /// <summary>
        /// Entity identifier with <c>ENTITY_ID_OFFSET</c> applied — the value that can be used
        /// to construct a <see cref="World{TWorld}.Entity"/>. The offset exists because entity ID 0
        /// is reserved as a null/invalid sentinel.
        /// </summary>
        public uint EntityID {
            [MethodImpl(AggressiveInlining)] get => Id + Const.ENTITY_ID_OFFSET;
        }

        /// <summary>
        /// Returns the current status of this GID in the specified world.
        /// <para>
        /// <see cref="GIDStatus.Active"/> — the entity exists, version matches, and it is loaded.<br/>
        /// <see cref="GIDStatus.NotActual"/> — the entity does not exist or its version/cluster doesn't match (stale reference).<br/>
        /// <see cref="GIDStatus.NotLoaded"/> — the entity exists and version matches, but it is currently unloaded.
        /// </para>
        /// </summary>
        /// <typeparam name="TWorld">The world type to check against.</typeparam>
        /// <returns>The <see cref="GIDStatus"/> of this GID.</returns>
        [MethodImpl(AggressiveInlining)]
        public GIDStatus Status<TWorld>() where TWorld : struct, IWorldType {
            return World<TWorld>.Data.Instance.GIDStatus(this);
        }

        /// <summary>
        /// Attempts to resolve this GID into a live <see cref="World{TWorld}.Entity"/> handle.
        /// Returns <c>true</c> and a valid entity if the GID is loaded and actual;
        /// returns <c>false</c> if either condition fails (entity not loaded or version mismatch).
        /// <para>
        /// This is the primary safe way to convert a persisted/serialized GID back into a
        /// runtime entity handle. Always prefer this over <see cref="Unpack{TWorld}"/> unless
        /// you have already validated the GID.
        /// </para>
        /// </summary>
        /// <typeparam name="TWorld">The world type to resolve against.</typeparam>
        /// <param name="entity">
        /// The resolved entity handle. Only valid when the method returns <c>true</c>.
        /// On <c>false</c>, the value is partially initialized and must not be used.
        /// </param>
        /// <returns><c>true</c> if the GID was successfully unpacked into a valid entity.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool TryUnpack<TWorld>(out World<TWorld>.Entity entity) where TWorld : struct, IWorldType {
            entity.IdWithOffset = Id + Const.ENTITY_ID_OFFSET;
            return World<TWorld>.Data.Instance.GIDStatus(this) == GIDStatus.Active;
        }

        /// <summary>
        /// Attempts to resolve this GID into a live entity handle, with failure diagnostics.
        /// Behaves like <see cref="TryUnpack{TWorld}(out World{TWorld}.Entity)"/> but additionally
        /// reports the <see cref="GIDStatus"/> indicating why the resolution failed.
        /// </summary>
        /// <typeparam name="TWorld">The world type to resolve against.</typeparam>
        /// <param name="entity">
        /// The resolved entity handle. Only valid when the method returns <c>true</c>.
        /// </param>
        /// <param name="status">
        /// The <see cref="GIDStatus"/> of this GID:
        /// <see cref="GIDStatus.Active"/> if the entity is alive and loaded,
        /// <see cref="GIDStatus.NotActual"/> if the entity does not exist or version/cluster doesn't match,
        /// <see cref="GIDStatus.NotLoaded"/> if the entity exists and version matches but is unloaded.
        /// </param>
        /// <returns><c>true</c> if the GID was successfully unpacked into a valid entity.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool TryUnpack<TWorld>(out World<TWorld>.Entity entity, out GIDStatus status) where TWorld : struct, IWorldType {
            entity.IdWithOffset = Id + Const.ENTITY_ID_OFFSET;
            status = World<TWorld>.Data.Instance.GIDStatus(this);
            return status == GIDStatus.Active;
        }

        /// <summary>
        /// Unconditionally converts this GID into an entity handle without runtime validation
        /// (in Release builds). In Debug builds, throws <see cref="StaticEcsException"/> if the
        /// GID is not loaded or not actual.
        /// <para>
        /// Use only when you have already verified the GID via <see cref="Status{TWorld}()"/>
        /// or in hot paths where the validity is guaranteed by design. Prefer <see cref="TryUnpack{TWorld}(out World{TWorld}.Entity)"/>
        /// for general-purpose code.
        /// </para>
        /// </summary>
        /// <typeparam name="TWorld">The world type to resolve against.</typeparam>
        /// <returns>A live entity handle corresponding to this GID.</returns>
        [MethodImpl(AggressiveInlining)]
        public World<TWorld>.Entity Unpack<TWorld>() where TWorld : struct, IWorldType {
            #if FFS_ECS_DEBUG
            World<TWorld>.AssertGidIsActive("EntityGID", this);
            #endif

            return new World<TWorld>.Entity(Id);
        }

        /// <summary>
        /// Compares two EntityGIDs by their <see cref="Raw"/> values. Since all fields are
        /// packed into Raw via explicit layout, this is a single 8-byte integer comparison.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public bool Equals(EntityGID other) => Raw == other.Raw;

        /// <summary>
        /// Cross-type equality: compares this EntityGID with an <see cref="EntityGIDCompact"/>
        /// by individually comparing <see cref="Id"/>, <see cref="Version"/>, and <see cref="ClusterId"/>,
        /// since the two types have different binary layouts.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public bool Equals(EntityGIDCompact other) => Id == other.Id && Version == other.Version && ClusterId == other.ClusterId;

        /// <inheritdoc />
        /// <remarks>
        /// Intentionally throws to prevent accidental boxing comparisons. Always use the typed
        /// <see cref="Equals(EntityGID)"/> or <see cref="Equals(EntityGIDCompact)"/> overloads.
        /// </remarks>
        [MethodImpl(AggressiveInlining)]
        public override bool Equals(object obj) => throw new StaticEcsException("EntityGID `Equals object` not allowed!!");

        /// <summary> Equality operator. Compares two EntityGIDs by <see cref="Raw"/> value. </summary>
        [MethodImpl(AggressiveInlining)]
        public static bool operator ==(EntityGID left, EntityGID right) => left.Equals(right);

        /// <summary> Inequality operator. Compares two EntityGIDs by <see cref="Raw"/> value. </summary>
        [MethodImpl(AggressiveInlining)]
        public static bool operator !=(EntityGID left, EntityGID right) => !left.Equals(right);

        /// <summary>
        /// Explicit conversion from EntityGID to <see cref="EntityGIDCompact"/>.
        /// This is a narrowing conversion — will throw in debug builds if <see cref="Chunk"/>
        /// or <see cref="ClusterId"/> exceed the 2-bit limits of the compact format.
        /// In release builds, values are silently truncated.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static explicit operator EntityGIDCompact(EntityGID gid) => new(gid.Id, gid.Version, gid.ClusterId);

        /// <inheritdoc />
        [MethodImpl(AggressiveInlining)]
        public override string ToString() {
            #if FFS_ECS_DEBUG
            return Utils.EntityGidToString(this);
            #else
            return $"GID {Id}, Version {Version}, ClusterId {ClusterId}";
            #endif
        }

        /// <inheritdoc />
        [MethodImpl(AggressiveInlining)]
        public override int GetHashCode() => Raw.GetHashCode();
    }

    /// <summary>
    /// Compact global identifier for an entity (4 bytes).
    /// <para>
    /// A space-optimized alternative to <see cref="EntityGID"/> that packs entity identity into a
    /// single uint. Ideal for network packets, compressed save files, and memory-constrained
    /// collections where halving the GID size matters.
    /// </para>
    /// <para>
    /// Bit layout of <see cref="Raw"/>:<br/>
    /// <c>[31..16]</c> Version (16 bits) — same range as EntityGID<br/>
    /// <c>[15..14]</c> ClusterId (2 bits, values 0..3)<br/>
    /// <c>[13..12]</c> Chunk index (2 bits, values 0..3 — max 4 chunks × 4096 = 16384 entities)<br/>
    /// <c>[11..0]</c> Entity index within chunk (12 bits, values 0..4095)
    /// </para>
    /// <para>
    /// Limitations compared to <see cref="EntityGID"/>: supports at most 4 chunks and 4 clusters.
    /// Exceeding these limits throws in debug builds.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    [Serializable]
    public readonly struct EntityGIDCompact : IEquatable<EntityGIDCompact>, IEquatable<EntityGID> {
        /// <summary>
        /// Packed 4-byte representation containing all fields.
        /// See the type-level documentation for the bit layout.
        /// Can be serialized/deserialized directly via <see cref="EntityGIDSerializer.WriteEntityGIDCompact"/>
        /// and <see cref="EntityGIDSerializer.ReadEntityGIDCompact"/>.
        /// </summary>
        public readonly uint Raw;

        /// <summary>
        /// Creates an EntityGIDCompact from a raw uint value, typically obtained from deserialization
        /// or from a previously stored <see cref="Raw"/>. The individual fields are extracted
        /// on demand via bit operations. In debug builds, validates the embedded values.
        /// </summary>
        /// <param name="raw">The packed 4-byte value.</param>
        public EntityGIDCompact(uint raw) {
            Raw = raw;
            #if FFS_ECS_DEBUG
            var chunkIdx = Id >> Const.ENTITIES_IN_CHUNK_SHIFT;
            if (chunkIdx >= 4) throw new StaticEcsException("EntityGIDCompact Chunk must fit in 2 bits (0..3)");
            if (ClusterId >= 4) throw new StaticEcsException("EntityGIDCompact ClusterId must fit in 2 bits (0..3)");
            if (Version == 0) throw new StaticEcsException("EntityGIDCompact version 0 is unsupported.");
            #endif
        }

        /// <summary>
        /// Creates an EntityGIDCompact from individual components, packing them into the bit layout.
        /// </summary>
        /// <param name="id">
        /// Internal entity index (without <c>ENTITY_ID_OFFSET</c>).
        /// The chunk portion (upper bits) must fit in 2 bits (max chunk index 3).
        /// </param>
        /// <param name="version">Generation counter. Must be greater than 0.</param>
        /// <param name="clusterId">Cluster identifier. Must fit in 2 bits (0..3).</param>
        public EntityGIDCompact(uint id, ushort version, ushort clusterId) {
            var chunkIdx = (id >> Const.ENTITIES_IN_CHUNK_SHIFT);
            var chunkEntityIdx = (ushort) (id & Const.ENTITIES_IN_CHUNK_MASK);

            #if FFS_ECS_DEBUG
            if (chunkIdx >= 4) throw new StaticEcsException("EntityGIDCompact Chunk must fit in 2 bits (0..3)");
            if (clusterId >= 4) throw new StaticEcsException("EntityGIDCompact ClusterId must fit in 2 bits (0..3)");
            if (version == 0) throw new StaticEcsException("EntityGIDCompact version 0 is unsupported.");
            #endif
            Raw = (chunkEntityIdx & 0xFFFu)
                  | ((chunkIdx & 0b11u) << 12)
                  | ((clusterId & 0b11u) << 14)
                  | ((uint) version << 16);
        }

        /// <summary>
        /// Zero-based chunk index extracted from the packed representation.
        /// Range: 0..3 (2-bit field).
        /// </summary>
        public uint Chunk {
            [MethodImpl(AggressiveInlining)] get => Id >> Const.ENTITIES_IN_CHUNK_SHIFT;
        }

        /// <summary>
        /// Entity identifier with <c>ENTITY_ID_OFFSET</c> applied — the value that can be used
        /// to construct a <see cref="World{TWorld}.Entity"/>.
        /// </summary>
        public uint EntityID {
            [MethodImpl(AggressiveInlining)] get => Id + Const.ENTITY_ID_OFFSET;
        }

        /// <summary>
        /// Internal entity index reconstructed from the packed bit fields (chunk index + intra-chunk offset).
        /// Equivalent to <see cref="EntityGID.Id"/> but extracted via bit manipulation from <see cref="Raw"/>.
        /// </summary>
        public uint Id {
            [MethodImpl(AggressiveInlining)] get => (((Raw >> 12) & 0b11u) << Const.ENTITIES_IN_CHUNK_SHIFT) + (Raw & 0xFFFu);
        }

        /// <summary>
        /// Cluster identifier (2-bit field, values 0..3).
        /// See <see cref="EntityGID.ClusterId"/> for the semantic meaning.
        /// </summary>
        public byte ClusterId {
            [MethodImpl(AggressiveInlining)] get => (byte) ((Raw >> 14) & 0b11u);
        }

        /// <summary>
        /// Generation counter for the entity slot (upper 16 bits of <see cref="Raw"/>).
        /// Same semantics as <see cref="EntityGID.Version"/>.
        /// </summary>
        public ushort Version {
            [MethodImpl(AggressiveInlining)] get => (ushort) (Raw >> 16);
        }

        /// <summary>
        /// Returns the current status of this compact GID in the specified world.
        /// See <see cref="EntityGID.Status{TWorld}"/> for full semantics.
        /// </summary>
        /// <typeparam name="TWorld">The world type to check against.</typeparam>
        /// <returns>The <see cref="GIDStatus"/> of this GID.</returns>
        [MethodImpl(AggressiveInlining)]
        public GIDStatus Status<TWorld>() where TWorld : struct, IWorldType {
            return World<TWorld>.Data.Instance.GIDStatus(this);
        }

        /// <summary>
        /// Attempts to resolve this compact GID into a live entity handle.
        /// See <see cref="EntityGID.TryUnpack{TWorld}(out World{TWorld}.Entity)"/> for full semantics.
        /// </summary>
        /// <typeparam name="TWorld">The world type to resolve against.</typeparam>
        /// <param name="entity">The resolved entity. Valid only when the method returns <c>true</c>.</param>
        /// <returns><c>true</c> if the GID was successfully unpacked.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool TryUnpack<TWorld>(out World<TWorld>.Entity entity) where TWorld : struct, IWorldType {
            entity.IdWithOffset = Id + Const.ENTITY_ID_OFFSET;
            return World<TWorld>.Data.Instance.GIDStatus(this) == GIDStatus.Active;
        }
        
        /// <summary>
        /// Attempts to resolve this compact GID into a live entity handle, with failure diagnostics.
        /// See <see cref="EntityGID.TryUnpack{TWorld}(out World{TWorld}.Entity, out GIDStatus)"/> for full semantics.
        /// </summary>
        /// <typeparam name="TWorld">The world type to resolve against.</typeparam>
        /// <param name="entity">The resolved entity. Valid only when the method returns <c>true</c>.</param>
        /// <param name="status">The <see cref="GIDStatus"/> of this GID.</param>
        /// <returns><c>true</c> if the GID was successfully unpacked.</returns>
        [MethodImpl(AggressiveInlining)]
        public bool TryUnpack<TWorld>(out World<TWorld>.Entity entity, out GIDStatus status) where TWorld : struct, IWorldType {
            entity.IdWithOffset = Id + Const.ENTITY_ID_OFFSET;
            status = World<TWorld>.Data.Instance.GIDStatus(this);
            return status == GIDStatus.Active;
        }

        /// <summary>
        /// Unconditionally converts this compact GID into an entity handle.
        /// In Debug builds, throws if the GID is not loaded or not actual.
        /// See <see cref="EntityGID.Unpack{TWorld}"/> for full semantics and usage guidance.
        /// </summary>
        /// <typeparam name="TWorld">The world type to resolve against.</typeparam>
        /// <returns>A live entity handle.</returns>
        [MethodImpl(AggressiveInlining)]
        public World<TWorld>.Entity Unpack<TWorld>() where TWorld : struct, IWorldType {
            #if FFS_ECS_DEBUG
            World<TWorld>.AssertGidIsActive("EntityGID", this);
            #endif

            return new(Id);
        }

        /// <summary>
        /// Compares two EntityGIDCompact values by their packed <see cref="Raw"/> representation.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public bool Equals(EntityGIDCompact other) => Raw == other.Raw;

        /// <summary>
        /// Cross-type equality with <see cref="EntityGID"/>.
        /// Compares individual fields since the binary layouts differ.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public bool Equals(EntityGID other) => Id == other.Id && Version == other.Version && ClusterId == other.ClusterId;

        /// <inheritdoc />
        /// <remarks>
        /// Intentionally throws to prevent accidental boxing. Use typed overloads.
        /// </remarks>
        [MethodImpl(AggressiveInlining)]
        public override bool Equals(object obj) => throw new StaticEcsException("EntityGIDCompact `Equals object` not allowed!!");

        /// <summary> Equality operator for two EntityGIDCompact values. </summary>
        [MethodImpl(AggressiveInlining)]
        public static bool operator ==(EntityGIDCompact left, EntityGIDCompact right) => left.Equals(right);

        /// <summary> Inequality operator for two EntityGIDCompact values. </summary>
        [MethodImpl(AggressiveInlining)]
        public static bool operator !=(EntityGIDCompact left, EntityGIDCompact right) => !left.Equals(right);

        /// <summary> Cross-type equality operator (EntityGIDCompact == EntityGID). </summary>
        [MethodImpl(AggressiveInlining)]
        public static bool operator ==(EntityGIDCompact left, EntityGID right) => left.Equals(right);

        /// <summary> Cross-type inequality operator (EntityGIDCompact != EntityGID). </summary>
        [MethodImpl(AggressiveInlining)]
        public static bool operator !=(EntityGIDCompact left, EntityGID right) => !left.Equals(right);

        /// <summary> Cross-type equality operator (EntityGID == EntityGIDCompact). </summary>
        [MethodImpl(AggressiveInlining)]
        public static bool operator ==(EntityGID left, EntityGIDCompact right) => right.Equals(left);

        /// <summary> Cross-type inequality operator (EntityGID != EntityGIDCompact). </summary>
        [MethodImpl(AggressiveInlining)]
        public static bool operator !=(EntityGID left, EntityGIDCompact right) => !right.Equals(left);

        /// <summary>
        /// Implicit widening conversion from EntityGIDCompact to <see cref="EntityGID"/>.
        /// Always safe — all values representable in compact form fit in the full-size format.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static implicit operator EntityGID(EntityGIDCompact gid) => new(gid.Id, gid.Version, gid.ClusterId);

        /// <inheritdoc />
        [MethodImpl(AggressiveInlining)]
        public override string ToString() {
            #if FFS_ECS_DEBUG
            return Utils.EntityGidToString(new EntityGID(Id, Version, ClusterId));
            #else
            return $"GID {Id}, Version {Version}, ClusterId {ClusterId}";
            #endif
        }

        /// <inheritdoc />
        [MethodImpl(AggressiveInlining)]
        public override int GetHashCode() => Raw.GetHashCode();
    }

    /// <summary>
    /// Extension methods for binary serialization and deserialization of <see cref="EntityGID"/>
    /// and <see cref="EntityGIDCompact"/> via StaticPack's <see cref="BinaryPackWriter"/> and
    /// <see cref="BinaryPackReader"/>.
    /// <para>
    /// EntityGID is serialized as 8 bytes (ulong), EntityGIDCompact as 4 bytes (uint).
    /// Both write the raw packed representation directly — no field-by-field encoding.
    /// </para>
    /// </summary>
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, Const.IL2CPPNullChecks)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, Const.IL2CPPArrayBoundsChecks)]
    #endif
    public static class EntityGIDSerializer {
        /// <summary>
        /// Writes an <see cref="EntityGID"/> to the binary stream as a ulong (8 bytes).
        /// </summary>
        /// <param name="writer">The binary writer to write to.</param>
        /// <param name="value">The EntityGID to serialize.</param>
        [MethodImpl(AggressiveInlining)]
        public static void WriteEntityGID(this ref BinaryPackWriter writer, in EntityGID value) => writer.WriteUlong(value.Raw);

        /// <summary>
        /// Reads an <see cref="EntityGID"/> from the binary stream (consumes 8 bytes).
        /// </summary>
        /// <param name="reader">The binary reader to read from.</param>
        /// <returns>The deserialized EntityGID.</returns>
        [MethodImpl(AggressiveInlining)]
        public static EntityGID ReadEntityGID(this ref BinaryPackReader reader) => new(reader.ReadUlong());

        /// <summary>
        /// Writes an <see cref="EntityGIDCompact"/> to the binary stream as a uint (4 bytes).
        /// </summary>
        /// <param name="writer">The binary writer to write to.</param>
        /// <param name="value">The EntityGIDCompact to serialize.</param>
        [MethodImpl(AggressiveInlining)]
        public static void WriteEntityGIDCompact(this ref BinaryPackWriter writer, in EntityGIDCompact value) => writer.WriteUint(value.Raw);

        /// <summary>
        /// Reads an <see cref="EntityGIDCompact"/> from the binary stream (consumes 4 bytes).
        /// </summary>
        /// <param name="reader">The binary reader to read from.</param>
        /// <returns>The deserialized EntityGIDCompact.</returns>
        [MethodImpl(AggressiveInlining)]
        public static EntityGIDCompact ReadEntityGIDCompact(this ref BinaryPackReader reader) => new(reader.ReadUint());
    }
}