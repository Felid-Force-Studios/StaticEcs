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
            public int ComponentsCount(bool withDisabled = true) => ModuleComponents.Value.ComponentsCount(_entity, withDisabled);

            #region BY_TYPE
            #region REF
            [MethodImpl(AggressiveInlining)]
            public ref C Ref<C>()
                where C : struct, IComponent {
                return ref Components<C>.Value.Ref(_entity);
            }
            #endregion

            #region HAS
            [MethodImpl(AggressiveInlining)]
            public bool HasAllOf<C>() where C : struct, IComponent {
                return Components<C>.Value.Has(_entity);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOf<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                return Components<C1>.Value.Has(_entity) && Components<C2>.Value.Has(_entity);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOf<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                return Components<C1>.Value.Has(_entity) && Components<C2>.Value.Has(_entity) && Components<C3>.Value.Has(_entity);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOf<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                return Components<C1>.Value.Has(_entity) || Components<C2>.Value.Has(_entity);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOf<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                return Components<C1>.Value.Has(_entity) || Components<C2>.Value.Has(_entity) || Components<C3>.Value.Has(_entity);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasDisabledAllOf<C1>()
                where C1 : struct, IComponent {
                return Components<C1>.Value.HasDisabled(_entity);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasDisabledAllOf<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                return Components<C1>.Value.HasDisabled(_entity) && Components<C2>.Value.HasDisabled(_entity);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasDisabledAllOf<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                return Components<C1>.Value.HasDisabled(_entity) && Components<C2>.Value.HasDisabled(_entity) && Components<C3>.Value.HasDisabled(_entity);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasDisabledAnyOf<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                return Components<C1>.Value.HasDisabled(_entity) || Components<C2>.Value.HasDisabled(_entity);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasDisabledAnyOf<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                return Components<C1>.Value.HasDisabled(_entity) || Components<C2>.Value.HasDisabled(_entity) || Components<C3>.Value.HasDisabled(_entity);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasEnabledAllOf<C1>()
                where C1 : struct, IComponent {
                return Components<C1>.Value.HasEnabled(_entity);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasEnabledAllOf<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                return Components<C1>.Value.HasEnabled(_entity) && Components<C2>.Value.HasEnabled(_entity);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasEnabledAllOf<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                return Components<C1>.Value.HasEnabled(_entity) && Components<C2>.Value.HasEnabled(_entity) && Components<C3>.Value.HasEnabled(_entity);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasEnabledAnyOf<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                return Components<C1>.Value.HasEnabled(_entity) || Components<C2>.Value.HasEnabled(_entity);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasEnabledAnyOf<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                return Components<C1>.Value.HasEnabled(_entity) || Components<C2>.Value.HasEnabled(_entity) || Components<C3>.Value.HasEnabled(_entity);
            }
            #endregion
            #endregion
            
            #region BY_RAW_TYPE
            [MethodImpl(AggressiveInlining)]
            public bool HasAllOf(Type componentType) {
                return ModuleComponents.Value.GetPool(componentType).Has(_entity);
            }
            
            [MethodImpl(AggressiveInlining)]
            public IComponent GetRaw(Type componentType) {
               return ModuleComponents.Value.GetPool(componentType).GetRaw(_entity);
            }
            #endregion
        }
    }
}