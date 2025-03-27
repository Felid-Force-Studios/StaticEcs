using System;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    public interface IStandardComponent { }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct StandardComponentDynId : IEquatable<StandardComponentDynId> {
        public readonly ushort Value;
        
        internal StandardComponentDynId(ushort value) {
            Value = value;
        }

        [MethodImpl(AggressiveInlining)]
        public bool Equals(StandardComponentDynId other) => Value == other.Value;
        
        public override bool Equals(object obj) => throw new Exception("StandardComponentDynId` Equals object` not allowed!");

        [MethodImpl(AggressiveInlining)]
        public override int GetHashCode() => Value;

        [MethodImpl(AggressiveInlining)]
        public override string ToString() => $"StandardComponentDynId ID: {Value}";
        
        [MethodImpl(AggressiveInlining)]
        public static bool operator ==(StandardComponentDynId left, StandardComponentDynId right) => left.Equals(right);

        [MethodImpl(AggressiveInlining)]
        public static bool operator !=(StandardComponentDynId left, StandardComponentDynId right) => !left.Equals(right);
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct EntityVersion : IStandardComponent {
        internal short Value;

        [MethodImpl(AggressiveInlining)]
        public override string ToString() {
            return $"{Value}";
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    internal struct EntityStatus : IStandardComponent {
        internal EntityStatusType Value;

        [MethodImpl(AggressiveInlining)]
        public override string ToString() {
            return Value == EntityStatusType.Disabled ? "Disabled" : "Enabled";
        }
    }
}