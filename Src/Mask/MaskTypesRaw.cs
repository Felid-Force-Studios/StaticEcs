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
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            Mask.SetMask<WorldID>(bufId);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask1 : IComponentMasks {
        public readonly MaskDynId C1;
        
        [MethodImpl(AggressiveInlining)]
        public Mask1(MaskDynId c1) {
            C1 = c1;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleMasks.Value.BitMask.SetInBuffer(bufId,  C1.Value);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask2 : IComponentMasks {
        public readonly MaskDynId C1;
        public readonly MaskDynId C2;

        [MethodImpl(AggressiveInlining)]
        public Mask2(MaskDynId c1, MaskDynId c2) {
            C1 = c1; C2 = c2;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleMasks.Value.BitMask.SetInBuffer(bufId, C1.Value, C2.Value);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask3 : IComponentMasks {
        public readonly MaskDynId C1;
        public readonly MaskDynId C2;
        public readonly MaskDynId C3;

        [MethodImpl(AggressiveInlining)]
        public Mask3(MaskDynId c1, MaskDynId c2, MaskDynId c3) {
            C1 = c1; C2 = c2; C3 = c3;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleMasks.Value.BitMask.SetInBuffer(bufId, C1.Value, C2.Value, C3.Value);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask4 : IComponentMasks {
        public readonly MaskDynId C1;
        public readonly MaskDynId C2;
        public readonly MaskDynId C3;
        public readonly MaskDynId C4;

        [MethodImpl(AggressiveInlining)]
        public Mask4(MaskDynId c1, MaskDynId c2, MaskDynId c3, MaskDynId c4) {
            C1 = c1; C2 = c2; C3 = c3; C4 = c4;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleMasks.Value.BitMask.SetInBuffer(bufId, C1.Value, C2.Value, C3.Value, C4.Value);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask5 : IComponentMasks {
        public readonly MaskDynId C1;
        public readonly MaskDynId C2;
        public readonly MaskDynId C3;
        public readonly MaskDynId C4;
        public readonly MaskDynId C5;

        [MethodImpl(AggressiveInlining)]
        public Mask5(MaskDynId c1, MaskDynId c2, MaskDynId c3, MaskDynId c4, MaskDynId c5) {
            C1 = c1; C2 = c2; C3 = c3; C4 = c4; C5 = c5;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleMasks.Value.BitMask.SetInBuffer(bufId, C1.Value, C2.Value, C3.Value, C4.Value, C5.Value);
        }
    }

    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask6 : IComponentMasks {
        public readonly MaskDynId C1;
        public readonly MaskDynId C2;
        public readonly MaskDynId C3;
        public readonly MaskDynId C4;
        public readonly MaskDynId C5;
        public readonly MaskDynId C6;

        [MethodImpl(AggressiveInlining)]
        public Mask6(MaskDynId c1, MaskDynId c2, MaskDynId c3, MaskDynId c4, MaskDynId c5, MaskDynId c6) {
            C1 = c1; C2 = c2; C3 = c3; C4 = c4; C5 = c5; C6 = c6;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleMasks.Value.BitMask.SetInBuffer(bufId, C1.Value, C2.Value, C3.Value, C4.Value, C5.Value, C6.Value);
        }
    }
    
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask7 : IComponentMasks {
        public readonly MaskDynId C1;
        public readonly MaskDynId C2;
        public readonly MaskDynId C3;
        public readonly MaskDynId C4;
        public readonly MaskDynId C5;
        public readonly MaskDynId C6;
        public readonly MaskDynId C7;

        [MethodImpl(AggressiveInlining)]
        public Mask7(MaskDynId c1, MaskDynId c2, MaskDynId c3, MaskDynId c4, MaskDynId c5, MaskDynId c6, MaskDynId c7) {
            C1 = c1; C2 = c2; C3 = c3; C4 = c4; C5 = c5; C6 = c6; C7 = c7;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleMasks.Value.BitMask.SetInBuffer(bufId, C1.Value, C2.Value, C3.Value, C4.Value, C5.Value, C6.Value, C7.Value);
        }
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Mask8 : IComponentMasks {
        public readonly MaskDynId C1;
        public readonly MaskDynId C2;
        public readonly MaskDynId C3;
        public readonly MaskDynId C4;
        public readonly MaskDynId C5;
        public readonly MaskDynId C6;
        public readonly MaskDynId C7;
        public readonly MaskDynId C8;

        [MethodImpl(AggressiveInlining)]
        public Mask8(MaskDynId c1, MaskDynId c2, MaskDynId c3, MaskDynId c4, MaskDynId c5, MaskDynId c6, MaskDynId c7, MaskDynId c8) {
            C1 = c1; C2 = c2; C3 = c3; C4 = c4; C5 = c5; C6 = c6; C7 = c7; C8 = c8;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleMasks.Value.BitMask.SetInBuffer(bufId, C1.Value, C2.Value, C3.Value, C4.Value, C5.Value, C6.Value, C7.Value, C8.Value);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct MaskArray : IComponentMasks {
        public readonly MaskDynId[] Mask;
        
        [MethodImpl(AggressiveInlining)]
        public MaskArray(params MaskDynId[] mask) {
            Mask = mask;
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            for (var i = 0; i < Mask.Length; i++) {
                Ecs<WorldID>.ModuleMasks.Value.BitMask.SetInBuffer(bufId, Mask[i].Value);
            }
        }
    }
}
#endif