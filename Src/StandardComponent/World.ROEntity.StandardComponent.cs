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
            public int StandardComponentsCount() => ModuleStandardComponents.Value.ComponentsCount();
            
            [MethodImpl(AggressiveInlining)]
            public bool IsDisabled() => StandardComponents<EntityStatus>.Value.RefInternal(_entity).Value == EntityStatusType.Disabled;
            
            [MethodImpl(AggressiveInlining)]
            public bool IsEnabled() => StandardComponents<EntityStatus>.Value.RefInternal(_entity).Value == EntityStatusType.Enabled;

            #region BY_TYPE
            #region REF
            [MethodImpl(AggressiveInlining)]
            public ref C RefStandard<C>()
                where C : struct, IStandardComponent {
                return ref StandardComponents<C>.Value.Ref(_entity);
            }
            #endregion
            #endregion
            
            #region BY_RAW_TYPE
            [MethodImpl(AggressiveInlining)]
            public IStandardComponent GetRawStandard(Type componentType) {
               return ModuleStandardComponents.Value.GetPool(componentType).GetRaw(_entity);
            }
            #endregion
        }
    }
}