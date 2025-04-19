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
    public abstract partial class World<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public readonly partial struct ROEntity {

            [MethodImpl(AggressiveInlining)]
            public int MasksCount() => ModuleMasks.Value.MasksCount(_entity);

            [MethodImpl(AggressiveInlining)]
            public void GetAllMasks(List<IMask> result) => ModuleMasks.Value.GetAllMasks(_entity, result);

            #region BY_TYPE
            #region HAS
            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfMasks<C>() where C : struct, IMask {
                return Masks<C>.Value.Has(_entity);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfMasks<C1, C2>()
                where C1 : struct, IMask
                where C2 : struct, IMask {
                return Masks<C1>.Value.Has(_entity) && Masks<C2>.Value.Has(_entity);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfMasks<C1, C2, C3>()
                where C1 : struct, IMask
                where C2 : struct, IMask
                where C3 : struct, IMask {
                return Masks<C1>.Value.Has(_entity) && Masks<C2>.Value.Has(_entity) && Masks<C3>.Value.Has(_entity);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOfMasks<C1, C2>()
                where C1 : struct, IMask
                where C2 : struct, IMask {
                return Masks<C1>.Value.Has(_entity) || Masks<C2>.Value.Has(_entity);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOfMasks<C1, C2, C3>()
                where C1 : struct, IMask
                where C2 : struct, IMask
                where C3 : struct, IMask {
                return Masks<C1>.Value.Has(_entity) || Masks<C2>.Value.Has(_entity) || Masks<C3>.Value.Has(_entity);
            }
            #endregion
            #endregion
            
            #region BY_RAW_TYPE
            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfMasks(Type maskType) {
                return ModuleMasks.Value.GetPool(maskType).Has(_entity);
            }
            #endregion
        }
    }
}
#endif