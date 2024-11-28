#if !FFS_ECS_DISABLE_MASKS
using System;
using System.Collections.Generic;
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
    public abstract partial class Ecs<WorldID> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public readonly partial struct Entity {

            [MethodImpl(AggressiveInlining)]
            public int MasksCount() => ModuleMasks.Value.MasksCount(this);

            [MethodImpl(AggressiveInlining)]
            public void GetAllMasks(List<IMask> result) => ModuleMasks.Value.GetAllMasks(this, result);

            #region BY_TYPE
            #region HAS
            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfMasks<C>() where C : struct, IMask {
                return Masks<C>.Value.Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfMasks<C1, C2>()
                where C1 : struct, IMask
                where C2 : struct, IMask {
                return Masks<C1>.Value.Has(this) && Masks<C2>.Value.Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfMasks<C1, C2, C3>()
                where C1 : struct, IMask
                where C2 : struct, IMask
                where C3 : struct, IMask {
                return Masks<C1>.Value.Has(this) && Masks<C2>.Value.Has(this) && Masks<C3>.Value.Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOfMasks<C1, C2>()
                where C1 : struct, IMask
                where C2 : struct, IMask {
                return Masks<C1>.Value.Has(this) || Masks<C2>.Value.Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOfMasks<C1, C2, C3>()
                where C1 : struct, IMask
                where C2 : struct, IMask
                where C3 : struct, IMask {
                return Masks<C1>.Value.Has(this) || Masks<C2>.Value.Has(this) || Masks<C3>.Value.Has(this);
            }
            #endregion

            #region SET
            [MethodImpl(AggressiveInlining)]
            public void SetMask<C>()
                where C : struct, IMask {
                Masks<C>.Value.Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void SetMask<C1, C2>()
                where C1 : struct, IMask
                where C2 : struct, IMask {
                Masks<C1>.Value.Set(this);
                Masks<C2>.Value.Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void SetMask<C1, C2, C3>()
                where C1 : struct, IMask
                where C2 : struct, IMask
                where C3 : struct, IMask {
                Masks<C1>.Value.Set(this);
                Masks<C2>.Value.Set(this);
                Masks<C3>.Value.Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void SetMask<C1, C2, C3, C4>()
                where C1 : struct, IMask
                where C2 : struct, IMask
                where C3 : struct, IMask
                where C4 : struct, IMask {
                Masks<C1>.Value.Set(this);
                Masks<C2>.Value.Set(this);
                Masks<C3>.Value.Set(this);
                Masks<C4>.Value.Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void SetMask<C1, C2, C3, C4, C5>()
                where C1 : struct, IMask
                where C2 : struct, IMask
                where C3 : struct, IMask
                where C4 : struct, IMask
                where C5 : struct, IMask {
                Masks<C1>.Value.Set(this);
                Masks<C2>.Value.Set(this);
                Masks<C3>.Value.Set(this);
                Masks<C4>.Value.Set(this);
                Masks<C5>.Value.Set(this);
            }
            #endregion

            #region DELETE
            [MethodImpl(AggressiveInlining)]
            public void DeleteMask<C>() where C : struct, IMask {
                Masks<C>.Value.Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void DeleteMask<C1, C2>()
                where C1 : struct, IMask
                where C2 : struct, IMask {
                Masks<C1>.Value.Delete(this);
                Masks<C2>.Value.Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void DeleteMask<C1, C2, C3>()
                where C1 : struct, IMask
                where C2 : struct, IMask
                where C3 : struct, IMask {
                Masks<C1>.Value.Delete(this);
                Masks<C2>.Value.Delete(this);
                Masks<C3>.Value.Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void DeleteMask<C1, C2, C3, C4>()
                where C1 : struct, IMask
                where C2 : struct, IMask
                where C3 : struct, IMask
                where C4 : struct, IMask {
                Masks<C1>.Value.Delete(this);
                Masks<C2>.Value.Delete(this);
                Masks<C3>.Value.Delete(this);
                Masks<C4>.Value.Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void DeleteMask<C1, C2, C3, C4, C5>()
                where C1 : struct, IMask
                where C2 : struct, IMask
                where C3 : struct, IMask
                where C4 : struct, IMask
                where C5 : struct, IMask {
                Masks<C1>.Value.Delete(this);
                Masks<C2>.Value.Delete(this);
                Masks<C3>.Value.Delete(this);
                Masks<C4>.Value.Delete(this);
                Masks<C5>.Value.Delete(this);
            }
            #endregion
            #endregion

            #region BY_ID
            #region HAS
            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfMasks(MaskDynId c) {
                return ModuleMasks.Value.GetPool(c).Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfMasks(MaskDynId c1, MaskDynId c2) {
                return ModuleMasks.Value.GetPool(c1).Has(this) && ModuleMasks.Value.GetPool(c2).Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfMasks(MaskDynId c1, MaskDynId c2, MaskDynId c3) {
                return ModuleMasks.Value.GetPool(c1).Has(this) && ModuleMasks.Value.GetPool(c2).Has(this) && ModuleMasks.Value.GetPool(c3).Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOfMasks(MaskDynId c1, MaskDynId c2) {
                return ModuleMasks.Value.GetPool(c1).Has(this) || ModuleMasks.Value.GetPool(c2).Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOfMasks(MaskDynId c1, MaskDynId c2, MaskDynId c3) {
                return ModuleMasks.Value.GetPool(c1).Has(this) || ModuleMasks.Value.GetPool(c2).Has(this) || ModuleMasks.Value.GetPool(c3).Has(this);
            }
            #endregion
            
            #region ADD
            [MethodImpl(AggressiveInlining)]
            public void SetMask(MaskDynId c) {
                ModuleMasks.Value.GetPool(c).Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void SetMask(MaskDynId c1, MaskDynId c2) {
                ModuleMasks.Value.GetPool(c1).Set(this);
                ModuleMasks.Value.GetPool(c2).Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void SetMask(MaskDynId c1, MaskDynId c2, MaskDynId c3) {
                ModuleMasks.Value.GetPool(c1).Set(this);
                ModuleMasks.Value.GetPool(c2).Set(this);
                ModuleMasks.Value.GetPool(c3).Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void SetMask(MaskDynId c1, MaskDynId c2, MaskDynId c3, MaskDynId c4) {
                ModuleMasks.Value.GetPool(c1).Set(this);
                ModuleMasks.Value.GetPool(c2).Set(this);
                ModuleMasks.Value.GetPool(c3).Set(this);
                ModuleMasks.Value.GetPool(c4).Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void SetMask(MaskDynId c1, MaskDynId c2, MaskDynId c3, MaskDynId c4, MaskDynId c5) {
                ModuleMasks.Value.GetPool(c1).Set(this);
                ModuleMasks.Value.GetPool(c2).Set(this);
                ModuleMasks.Value.GetPool(c3).Set(this);
                ModuleMasks.Value.GetPool(c4).Set(this);
                ModuleMasks.Value.GetPool(c5).Set(this);
            }
            #endregion
            
            #region DELETE
            [MethodImpl(AggressiveInlining)]
            public void DeleteMask(MaskDynId c) {
                ModuleMasks.Value.GetPool(c).Del(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void DeleteMask(MaskDynId c1, MaskDynId c2) {
                ModuleMasks.Value.GetPool(c1).Del(this);
                ModuleMasks.Value.GetPool(c2).Del(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void DeleteMask(MaskDynId c1, MaskDynId c2, MaskDynId c3) {
                ModuleMasks.Value.GetPool(c1).Del(this);
                ModuleMasks.Value.GetPool(c2).Del(this);
                ModuleMasks.Value.GetPool(c3).Del(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void DeleteMask(MaskDynId c1, MaskDynId c2, MaskDynId c3, MaskDynId c4) {
                ModuleMasks.Value.GetPool(c1).Del(this);
                ModuleMasks.Value.GetPool(c2).Del(this);
                ModuleMasks.Value.GetPool(c3).Del(this);
                ModuleMasks.Value.GetPool(c4).Del(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void DeleteMask(MaskDynId c1, MaskDynId c2, MaskDynId c3, MaskDynId c4, MaskDynId c5) {
                ModuleMasks.Value.GetPool(c1).Del(this);
                ModuleMasks.Value.GetPool(c2).Del(this);
                ModuleMasks.Value.GetPool(c3).Del(this);
                ModuleMasks.Value.GetPool(c4).Del(this);
                ModuleMasks.Value.GetPool(c5).Del(this);
            }
            #endregion
            #endregion
            
            #region BY_RAW_TYPE
            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfMasks(Type maskType) {
                return ModuleMasks.Value.GetPool(maskType).Has(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void SetMask(Type maskType) {
                ModuleMasks.Value.GetPool(maskType).Set(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void DeleteMask(Type maskType) {
                ModuleMasks.Value.GetPool(maskType).Del(this);
            }
            #endregion
        }
    }
    
    public partial interface IEntity {
        public int MasksCount();

        public void GetAllMasks(List<IMask> result);
        
        public bool HasAllOfMasks<C>() where C : struct, IMask;

        public void SetMask<C>() where C : struct, IMask;

        public void DeleteMask<C>() where C : struct, IMask;

        public bool HasAllOfMasks(Type maskType);

        public void SetMask(Type maskType);

        public void DeleteMask(Type maskType);

    }
}
#endif