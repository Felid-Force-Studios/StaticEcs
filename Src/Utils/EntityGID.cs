#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
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
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct EntityGID : IEquatable<EntityGID>, IEquatable<EntityGIDCompact> {
        [FieldOffset(0)] public readonly ulong Raw;
        [FieldOffset(0)] public readonly uint Id;
        [FieldOffset(4)] public readonly ushort ClusterId;
        [FieldOffset(6)] public readonly ushort Version;

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

        public uint Chunk {
            [MethodImpl(AggressiveInlining)] get => Id >> Const.ENTITIES_IN_CHUNK_SHIFT;
        }

        [MethodImpl(AggressiveInlining)]
        public bool IsActual<WorldType>() where WorldType : struct, IWorldType {
            return World<WorldType>.Entities.Value.GidIsActual(this);
        }

        [MethodImpl(AggressiveInlining)]
        public bool IsLoaded<WorldType>() where WorldType : struct, IWorldType {
            return World<WorldType>.Entities.Value.GidIsLoaded(this);
        }

        [MethodImpl(AggressiveInlining)]
        public bool IsLoadedAndActual<WorldType>() where WorldType : struct, IWorldType {
            return World<WorldType>.Entities.Value.GidIsLoadedAndActual(this);
        }

        [MethodImpl(AggressiveInlining)]
        public bool TryUnpack<WorldType>(out World<WorldType>.Entity entity) where WorldType : struct, IWorldType {
            entity.id = Id + Const.ENTITY_ID_OFFSET;
            return World<WorldType>.Entities.Value.GidIsLoadedAndActual(this);
        }

        [MethodImpl(AggressiveInlining)]
        public World<WorldType>.Entity Unpack<WorldType>() where WorldType : struct, IWorldType {
            #if FFS_ECS_DEBUG
            World<WorldType>.AssertGidIsLoaded("EntityGID", this);
            World<WorldType>.AssertGidIsActual("EntityGID", this);
            #endif

            return new(Id);
        }

        [MethodImpl(AggressiveInlining)]
        public bool Equals(EntityGID other) => Raw == other.Raw;

        [MethodImpl(AggressiveInlining)]
        public bool Equals(EntityGIDCompact other) => Id == other.Id && Version == other.Version && ClusterId == other.ClusterId;

        [MethodImpl(AggressiveInlining)]
        public override bool Equals(object obj) => throw new StaticEcsException("EntityGID `Equals object` not allowed!!");

        [MethodImpl(AggressiveInlining)]
        public static bool operator ==(EntityGID left, EntityGID right) => left.Equals(right);

        [MethodImpl(AggressiveInlining)]
        public static bool operator !=(EntityGID left, EntityGID right) => !left.Equals(right);

        [MethodImpl(AggressiveInlining)]
        public static implicit operator EntityGIDCompact(EntityGID gid) => new(gid.Id, gid.Version, gid.ClusterId);

        [MethodImpl(AggressiveInlining)]
        public override string ToString() {
            #if FFS_ECS_DEBUG
            return Utils.EntityGidToString(this);
            #else
            return $"GID {Id}, Version {Version}, ClusterId {ClusterId}";
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public override int GetHashCode() => Raw.GetHashCode();
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [Serializable]
    public readonly struct EntityGIDCompact : IEquatable<EntityGIDCompact>, IEquatable<EntityGID> {
        // --- Bit layout ---
        // [31..16] Version (16 bits)
        // [15..14] ClusterId (2 bits)
        // [13..12] Chunk (2 bits)
        // [11..0]  Id (12 bits)
        public readonly uint Raw;

        public EntityGIDCompact(uint raw) {
            Raw = raw;
            #if FFS_ECS_DEBUG
            var chunkIdx = Id >> Const.ENTITIES_IN_CHUNK_SHIFT;
            if (chunkIdx >= 4) throw new StaticEcsException("EntityGIDCompact Chunk must fit in 2 bits (0..3)");
            if (ClusterId >= 4) throw new StaticEcsException("EntityGIDCompact ClusterId must fit in 2 bits (0..3)");
            if (Version == 0) throw new StaticEcsException("EntityGIDCompact version 0 is unsupported.");
            #endif
        }

        public EntityGIDCompact(uint id, ushort version, ushort clusterId) {
            var chunkIdx = (id >> Const.ENTITIES_IN_CHUNK_SHIFT);
            var chunkEntityIdx = (ushort) (id & Const.ENTITIES_IN_CHUNK_OFFSET_MASK);

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

        public uint Chunk {
            [MethodImpl(AggressiveInlining)] get => Id >> Const.ENTITIES_IN_CHUNK_SHIFT;
        }

        public uint Id {
            [MethodImpl(AggressiveInlining)] get => (uint) (((byte) ((Raw >> 12) & 0b11u) << Const.ENTITIES_IN_CHUNK_SHIFT) + (Raw & 0xFFFu));
        }

        public byte ClusterId {
            [MethodImpl(AggressiveInlining)] get => (byte) ((Raw >> 14) & 0b11u);
        }

        public ushort Version {
            [MethodImpl(AggressiveInlining)] get => (ushort) (Raw >> 16);
        }

        [MethodImpl(AggressiveInlining)]
        public bool IsActual<WorldType>() where WorldType : struct, IWorldType {
            return World<WorldType>.Entities.Value.GidIsActual(this);
        }

        [MethodImpl(AggressiveInlining)]
        public bool IsLoaded<WorldType>() where WorldType : struct, IWorldType {
            return World<WorldType>.Entities.Value.GidIsLoaded(this);
        }

        [MethodImpl(AggressiveInlining)]
        public bool IsLoadedAndActual<WorldType>() where WorldType : struct, IWorldType {
            return World<WorldType>.Entities.Value.GidIsLoadedAndActual(this);
        }

        [MethodImpl(AggressiveInlining)]
        public bool TryUnpack<WorldType>(out World<WorldType>.Entity entity) where WorldType : struct, IWorldType {
            entity.id = Id + Const.ENTITY_ID_OFFSET;
            return World<WorldType>.Entities.Value.GidIsLoadedAndActual(this);
        }

        [MethodImpl(AggressiveInlining)]
        public World<WorldType>.Entity Unpack<WorldType>() where WorldType : struct, IWorldType {
            #if FFS_ECS_DEBUG
            World<WorldType>.AssertGidIsLoaded("EntityGID", this);
            World<WorldType>.AssertGidIsActual("EntityGID", this);
            #endif

            return new(Id);
        }

        [MethodImpl(AggressiveInlining)]
        public bool Equals(EntityGIDCompact other) => Raw == other.Raw;

        [MethodImpl(AggressiveInlining)]
        public bool Equals(EntityGID other) => Id == other.Id && Version == other.Version && ClusterId == other.ClusterId;

        [MethodImpl(AggressiveInlining)]
        public override bool Equals(object obj) => throw new StaticEcsException("EntityGIDCompact `Equals object` not allowed!!");

        [MethodImpl(AggressiveInlining)]
        public static bool operator ==(EntityGIDCompact left, EntityGIDCompact right) => left.Equals(right);

        [MethodImpl(AggressiveInlining)]
        public static bool operator !=(EntityGIDCompact left, EntityGIDCompact right) => !left.Equals(right);

        [MethodImpl(AggressiveInlining)]
        public static bool operator ==(EntityGIDCompact left, EntityGID right) => left.Equals(right);

        [MethodImpl(AggressiveInlining)]
        public static bool operator !=(EntityGIDCompact left, EntityGID right) => !left.Equals(right);

        [MethodImpl(AggressiveInlining)]
        public static bool operator ==(EntityGID left, EntityGIDCompact right) => right.Equals(left);

        [MethodImpl(AggressiveInlining)]
        public static bool operator !=(EntityGID left, EntityGIDCompact right) => !right.Equals(left);

        [MethodImpl(AggressiveInlining)]
        public static implicit operator EntityGID(EntityGIDCompact gid) => new(gid.Id, gid.Version, gid.ClusterId);

        [MethodImpl(AggressiveInlining)]
        public override string ToString() {
            #if FFS_ECS_DEBUG
            return Utils.EntityGidToString(new EntityGID(Id, Version, ClusterId));
            #else
            return $"GID {Id}, Version {Version}, ClusterId {ClusterId}";
            #endif
        }

        [MethodImpl(AggressiveInlining)]
        public override int GetHashCode() => Raw.GetHashCode();
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public static class EntityGIDSerializer {
        [MethodImpl(AggressiveInlining)]
        public static void WriteEntityGID(this ref BinaryPackWriter writer, in EntityGID value) => writer.WriteUlong(value.Raw);

        [MethodImpl(AggressiveInlining)]
        public static EntityGID ReadEntityGID(this ref BinaryPackReader reader) => new(reader.ReadUlong());
        
        [MethodImpl(AggressiveInlining)]
        public static void WriteEntityGIDCompact(this ref BinaryPackWriter writer, in EntityGIDCompact value) => writer.WriteUint(value.Raw);

        [MethodImpl(AggressiveInlining)]
        public static EntityGIDCompact ReadEntityGIDCompact(this ref BinaryPackReader reader) => new(reader.ReadUint());
    }
}