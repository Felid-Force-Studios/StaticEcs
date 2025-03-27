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
    public abstract partial class Ecs<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public readonly partial struct Entity {

            [MethodImpl(AggressiveInlining)]
            public int StandardComponentsCount() => ModuleStandardComponents.Value.ComponentsCount();

            [MethodImpl(AggressiveInlining)]
            public void GetAllStandardComponents(List<IStandardComponent> result) => ModuleStandardComponents.Value.GetAllComponents(this, result);
            
            [MethodImpl(AggressiveInlining)]
            public bool IsDisabled() => StandardComponents<EntityStatus>.Value.RefMutInternal(this).Value == EntityStatusType.Disabled;
            
            [MethodImpl(AggressiveInlining)]
            public bool IsEnabled() => StandardComponents<EntityStatus>.Value.RefMutInternal(this).Value == EntityStatusType.Enabled;
            
            [MethodImpl(AggressiveInlining)]
            public void Disable() => StandardComponents<EntityStatus>.Value.RefMutInternal(this).Value = EntityStatusType.Disabled;
            
            [MethodImpl(AggressiveInlining)]
            public void Enable() => StandardComponents<EntityStatus>.Value.RefMutInternal(this).Value = EntityStatusType.Enabled;

            #region BY_TYPE
            #region REF
            [MethodImpl(AggressiveInlining)]
            public ref C RefMutStandard<C>()
                where C : struct, IStandardComponent {
                return ref StandardComponents<C>.Value.RefMut(this);
            }

            [MethodImpl(AggressiveInlining)]
            public ref readonly C RefStandard<C>()
                where C : struct, IStandardComponent {
                return ref StandardComponents<C>.Value.Ref(this);
            }
            #endregion

            #region COPY
            [MethodImpl(AggressiveInlining)]
            public void CopyStandardComponentsTo<C1>(Entity target)
                where C1 : struct, IStandardComponent {
                StandardComponents<C1>.Value.Copy(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void CopyStandardComponentsTo<C1, C2>(Entity target)
                where C1 : struct, IStandardComponent
                where C2 : struct, IStandardComponent {
                StandardComponents<C1>.Value.Copy(this, target);
                StandardComponents<C2>.Value.Copy(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void CopyStandardComponentsTo<C1, C2, C3>(Entity target)
                where C1 : struct, IStandardComponent
                where C2 : struct, IStandardComponent
                where C3 : struct, IStandardComponent {
                StandardComponents<C1>.Value.Copy(this, target);
                StandardComponents<C2>.Value.Copy(this, target);
                StandardComponents<C3>.Value.Copy(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void CopyStandardComponentsTo<C1, C2, C3, C4>(Entity target)
                where C1 : struct, IStandardComponent
                where C2 : struct, IStandardComponent
                where C3 : struct, IStandardComponent
                where C4 : struct, IStandardComponent {
                StandardComponents<C1>.Value.Copy(this, target);
                StandardComponents<C2>.Value.Copy(this, target);
                StandardComponents<C3>.Value.Copy(this, target);
                StandardComponents<C4>.Value.Copy(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void CopyStandardComponentsTo<C1, C2, C3, C4, C5>(Entity target)
                where C1 : struct, IStandardComponent
                where C2 : struct, IStandardComponent
                where C3 : struct, IStandardComponent
                where C4 : struct, IStandardComponent
                where C5 : struct, IStandardComponent {
                StandardComponents<C1>.Value.Copy(this, target);
                StandardComponents<C2>.Value.Copy(this, target);
                StandardComponents<C3>.Value.Copy(this, target);
                StandardComponents<C4>.Value.Copy(this, target);
                StandardComponents<C5>.Value.Copy(this, target);
            }
            #endregion
            #endregion

            #region BY_ID
            #region COPY
            [MethodImpl(AggressiveInlining)]
            public void CopyStandardComponentsTo(StandardComponentDynId c, Entity target) {
                ModuleStandardComponents.Value.GetPool(c).Copy(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyStandardComponentsTo(StandardComponentDynId c1, StandardComponentDynId c2, Entity target) {
                ModuleStandardComponents.Value.GetPool(c1).Copy(this, target);
                ModuleStandardComponents.Value.GetPool(c2).Copy(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyStandardComponentsTo(StandardComponentDynId c1, StandardComponentDynId c2, StandardComponentDynId c3, Entity target) {
                ModuleStandardComponents.Value.GetPool(c1).Copy(this, target);
                ModuleStandardComponents.Value.GetPool(c2).Copy(this, target);
                ModuleStandardComponents.Value.GetPool(c3).Copy(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyStandardComponentsTo(StandardComponentDynId c1, StandardComponentDynId c2, StandardComponentDynId c3, StandardComponentDynId c4, Entity target) {
                ModuleStandardComponents.Value.GetPool(c1).Copy(this, target);
                ModuleStandardComponents.Value.GetPool(c2).Copy(this, target);
                ModuleStandardComponents.Value.GetPool(c3).Copy(this, target);
                ModuleStandardComponents.Value.GetPool(c4).Copy(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyStandardComponentsTo(StandardComponentDynId c1, StandardComponentDynId c2, StandardComponentDynId c3, StandardComponentDynId c4, StandardComponentDynId c5, Entity target) {
                ModuleStandardComponents.Value.GetPool(c1).Copy(this, target);
                ModuleStandardComponents.Value.GetPool(c2).Copy(this, target);
                ModuleStandardComponents.Value.GetPool(c3).Copy(this, target);
                ModuleStandardComponents.Value.GetPool(c4).Copy(this, target);
                ModuleStandardComponents.Value.GetPool(c5).Copy(this, target);
            }
            #endregion
            #endregion
            
            #region BY_RAW_TYPE
            [MethodImpl(AggressiveInlining)]
            public IStandardComponent GetRawStandard(Type componentType) {
               return ModuleStandardComponents.Value.GetPool(componentType).GetRaw(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void SetRawStandard(IStandardComponent component) {
                ModuleStandardComponents.Value.GetPool(component.GetType()).PutRaw(this, component);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void CopyStandardComponentsTo(Type componentType, Entity target) {
                ModuleStandardComponents.Value.GetPool(componentType).Copy(this, target);
            }
            #endregion
        }
    }
    
    public partial interface IEntity {
        public int StandardComponentsCount();

        public void GetAllStandardComponents(List<IStandardComponent> result);
        
        public ref C RefMutStandard<C>() where C : struct, IStandardComponent;

        public ref readonly C RefStandard<C>() where C : struct, IStandardComponent;

        public IStandardComponent GetRawStandard(Type componentType);

        public void SetRawStandard(IStandardComponent component);

    }
}