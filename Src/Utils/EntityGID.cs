using System;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [Serializable]
    public struct EntityGID : IEquatable<EntityGID> {
        internal const int IdBits = 24;
        internal const int IdMask = 0xFFFFFF;

        internal uint id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityGID(uint id, byte version) {
            this.id = id | ((uint) version << IdBits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityGID(uint value) {
            id = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly uint Id() => id & IdMask;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly byte Version() => (byte) (id >> IdBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void IncrementVersion() {
            var version = Version();
            version = (byte) (version == byte.MaxValue ? 1 : version + 1);
            id = (id & IdMask) | ((uint) version << IdBits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsRegistered<WorldType>() where WorldType : struct, IWorldType {
            return World<WorldType>.GIDStore.Value.IsRegistered(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsLoaded<WorldType>() where WorldType : struct, IWorldType {
            return World<WorldType>.GIDStore.Value.IsLoaded(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsEmpty() {
            return id == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryUnpack<WorldType>(out World<WorldType>.Entity entity) where WorldType : struct, IWorldType {
            return World<WorldType>.GIDStore.Value.TryGetEntity(this, out entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly World<WorldType>.Entity Unpack<WorldType>() where WorldType : struct, IWorldType {
            return World<WorldType>.GIDStore.Value.GetEntity(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(EntityGID other) => id == other.id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => throw new StaticEcsException("EntityGID `Equals object` not allowed!!");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(EntityGID left, EntityGID right) => left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(EntityGID left, EntityGID right) => !left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            return Utils.EntityGidToString(this);
            #endif
            return $"GID {Id()} : Version {Version()}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override int GetHashCode() {
            return (int) id;
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public static class EntityGIDSerializer {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteEntityGID(this ref BinaryPackWriter writer, in EntityGID value) {
            writer.WriteUint(value.id);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EntityGID ReadEntityGID(this ref BinaryPackReader reader) {
            return new EntityGID(reader.ReadUint());
        }
    }
}