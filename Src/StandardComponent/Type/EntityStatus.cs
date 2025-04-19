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
    public struct EntityStatus : IStandardComponent {
        internal EntityStatusType Value;

        internal EntityStatus(EntityStatusType value) {
            Value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() {
            return Value == EntityStatusType.Disabled ? "Disabled" : "Enabled";
        }
    }
    
    public class EntityStatusConfig<TWorldType> : DefaultStandardComponentConfig<EntityStatus, TWorldType> 
        where TWorldType : struct, IWorldType {

        public override Guid Id() => new("A146915B-1196-4530-BF33-F41603BAA614");

        public override byte Version() => 0;

        public override BinaryWriter<EntityStatus> Writer() {
            return (ref BinaryPackWriter writer, in EntityStatus value) => {
                writer.WriteByte((byte) value.Value);
            };
        }

        public override BinaryReader<EntityStatus> Reader() {
            return (ref BinaryPackReader reader) => new EntityStatus((EntityStatusType) reader.ReadByte());
        }

        public override IPackArrayStrategy<EntityStatus> ReadWriteStrategy() {
            return new UnmanagedPackArrayStrategy<EntityStatus>();
        }
    }
}