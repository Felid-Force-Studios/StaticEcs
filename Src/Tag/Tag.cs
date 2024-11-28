#if !FFS_ECS_DISABLE_TAGS
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
    public readonly struct TagDynId : IEquatable<TagDynId> {
        internal readonly ushort Value;
        
        internal TagDynId(ushort value) {
            Value = value;
        }

        [MethodImpl(AggressiveInlining)]
        public bool Equals(TagDynId other) => Value == other.Value;
        
        public override bool Equals(object obj) => throw new Exception("TagDynId` Equals object` not allowed!");

        [MethodImpl(AggressiveInlining)]
        public override int GetHashCode() => Value;

        [MethodImpl(AggressiveInlining)]
        public override string ToString() => $"TagDynamicId ID: {Value}";
                
        [MethodImpl(AggressiveInlining)]
        public static bool operator ==(TagDynId left, TagDynId right) => left.Equals(right);

        [MethodImpl(AggressiveInlining)]
        public static bool operator !=(TagDynId left, TagDynId right) => !left.Equals(right);
    }
}
#endif

namespace FFS.Libraries.StaticEcs {
    public interface ITag { }
}