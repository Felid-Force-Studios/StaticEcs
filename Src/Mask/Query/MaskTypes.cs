#if !FFS_ECS_DISABLE_MASKS
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
    public readonly struct MaskBox : IComponentMasks {
        public readonly IComponentMasks Mask;
        
        [MethodImpl(AggressiveInlining)]
        public MaskBox(IComponentMasks mask) {
            Mask = mask;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            Mask.SetBitMask<WorldType>(bufId);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask<C1> : IComponentMasks, Stateless
        where C1 : struct, IMask {
        
        [MethodImpl(AggressiveInlining)]
        public static MaskAll<C1> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static MaskNone<C1> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleMasks.Value.BitMask.SetInBuffer(0, World<WorldType>.Masks<C1>.Value.id);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask<C1, C2> : IComponentMasks, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask {
        
        [MethodImpl(AggressiveInlining)]
        public static MaskAll<C1, C2> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static MaskNone<C1, C2> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static MaskAny<C1, C2> Any() => default;
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleMasks.Value.BitMask.SetInBuffer(bufId,
                                                     World<WorldType>.Masks<C1>.Value.id,
                                                     World<WorldType>.Masks<C2>.Value.id);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask<C1, C2, C3> : IComponentMasks, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask {
        
        [MethodImpl(AggressiveInlining)]
        public static MaskAll<C1, C2, C3> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static MaskNone<C1, C2, C3> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static MaskAny<C1, C2, C3> Any() => default;
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleMasks.Value.BitMask.SetInBuffer(bufId,
                                                     World<WorldType>.Masks<C1>.Value.id,
                                                     World<WorldType>.Masks<C2>.Value.id,
                                                     World<WorldType>.Masks<C3>.Value.id);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask<C1, C2, C3, C4> : IComponentMasks, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask {
        
        [MethodImpl(AggressiveInlining)]
        public static MaskAll<C1, C2, C3, C4> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static MaskNone<C1, C2, C3, C4> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static MaskAny<C1, C2, C3, C4> Any() => default;
        
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleMasks.Value.BitMask.SetInBuffer(bufId,
                                                     World<WorldType>.Masks<C1>.Value.id,
                                                     World<WorldType>.Masks<C2>.Value.id,
                                                     World<WorldType>.Masks<C3>.Value.id,
                                                     World<WorldType>.Masks<C4>.Value.id);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask<C1, C2, C3, C4, C5> : IComponentMasks, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask {
        
        [MethodImpl(AggressiveInlining)]
        public static MaskAll<C1, C2, C3, C4, C5> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static MaskNone<C1, C2, C3, C4, C5> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static MaskAny<C1, C2, C3, C4, C5> Any() => default;
        
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleMasks.Value.BitMask.SetInBuffer(bufId,
                                                     World<WorldType>.Masks<C1>.Value.id,
                                                     World<WorldType>.Masks<C2>.Value.id,
                                                     World<WorldType>.Masks<C3>.Value.id,
                                                     World<WorldType>.Masks<C4>.Value.id,
                                                     World<WorldType>.Masks<C5>.Value.id);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask<C1, C2, C3, C4, C5, C6> : IComponentMasks, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask {
        
        [MethodImpl(AggressiveInlining)]
        public static MaskAll<C1, C2, C3, C4, C5, C6> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static MaskNone<C1, C2, C3, C4, C5, C6> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static MaskAny<C1, C2, C3, C4, C5, C6> Any() => default;
        
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleMasks.Value.BitMask.SetInBuffer(bufId,
                                                     World<WorldType>.Masks<C1>.Value.id,
                                                     World<WorldType>.Masks<C2>.Value.id,
                                                     World<WorldType>.Masks<C3>.Value.id,
                                                     World<WorldType>.Masks<C4>.Value.id,
                                                     World<WorldType>.Masks<C5>.Value.id,
                                                     World<WorldType>.Masks<C6>.Value.id);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask<C1, C2, C3, C4, C5, C6, C7> : IComponentMasks, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask
        where C7 : struct, IMask {
        
        [MethodImpl(AggressiveInlining)]
        public static MaskAll<C1, C2, C3, C4, C5, C6, C7> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static MaskNone<C1, C2, C3, C4, C5, C6, C7> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static MaskAny<C1, C2, C3, C4, C5, C6, C7> Any() => default;
        
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleMasks.Value.BitMask.SetInBuffer(bufId,
                                                     World<WorldType>.Masks<C1>.Value.id,
                                                     World<WorldType>.Masks<C2>.Value.id,
                                                     World<WorldType>.Masks<C3>.Value.id,
                                                     World<WorldType>.Masks<C4>.Value.id,
                                                     World<WorldType>.Masks<C5>.Value.id,
                                                     World<WorldType>.Masks<C6>.Value.id,
                                                     World<WorldType>.Masks<C7>.Value.id);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask<C1, C2, C3, C4, C5, C6, C7, C8> : IComponentMasks, Stateless
        where C1 : struct, IMask
        where C2 : struct, IMask
        where C3 : struct, IMask
        where C4 : struct, IMask
        where C5 : struct, IMask
        where C6 : struct, IMask
        where C7 : struct, IMask
        where C8 : struct, IMask {
        
        [MethodImpl(AggressiveInlining)]
        public static MaskAll<C1, C2, C3, C4, C5, C6, C7, C8> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static MaskNone<C1, C2, C3, C4, C5, C6, C7, C8> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static MaskAny<C1, C2, C3, C4, C5, C6, C7, C8> Any() => default;

        
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleMasks.Value.BitMask.SetInBuffer(bufId,
                                                     World<WorldType>.Masks<C1>.Value.id,
                                                     World<WorldType>.Masks<C2>.Value.id,
                                                     World<WorldType>.Masks<C3>.Value.id,
                                                     World<WorldType>.Masks<C4>.Value.id,
                                                     World<WorldType>.Masks<C5>.Value.id,
                                                     World<WorldType>.Masks<C6>.Value.id,
                                                     World<WorldType>.Masks<C7>.Value.id,
                                                     World<WorldType>.Masks<C8>.Value.id);
        }
    }
}
#endif