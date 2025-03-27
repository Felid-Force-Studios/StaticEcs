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
            public int ComponentsCount(bool withDisabled = true) => ModuleComponents.Value.ComponentsCount(this, withDisabled);

            [MethodImpl(AggressiveInlining)]
            public void GetAllComponents(List<IComponent> result, bool withDisabled = true) => ModuleComponents.Value.GetAllComponents(this, result, withDisabled);

            #region BY_TYPE
            #region REF
            [MethodImpl(AggressiveInlining)]
            public ref C RefMut<C>()
                where C : struct, IComponent {
                return ref Components<C>.Value.RefMut(this);
            }

            [MethodImpl(AggressiveInlining)]
            public ref readonly C Ref<C>()
                where C : struct, IComponent {
                return ref Components<C>.Value.Ref(this);
            }
            #endregion

            #region HAS
            [MethodImpl(AggressiveInlining)]
            public bool HasAllOf<C>() where C : struct, IComponent {
                return Components<C>.Value.Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOf<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                return Components<C1>.Value.Has(this) && Components<C2>.Value.Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOf<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                return Components<C1>.Value.Has(this) && Components<C2>.Value.Has(this) && Components<C3>.Value.Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOf<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                return Components<C1>.Value.Has(this) || Components<C2>.Value.Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOf<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                return Components<C1>.Value.Has(this) || Components<C2>.Value.Has(this) || Components<C3>.Value.Has(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasDisabledAllOf<C1>()
                where C1 : struct, IComponent {
                return Components<C1>.Value.HasDisabled(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasDisabledAllOf<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                return Components<C1>.Value.HasDisabled(this) && Components<C2>.Value.HasDisabled(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasDisabledAllOf<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                return Components<C1>.Value.HasDisabled(this) && Components<C2>.Value.HasDisabled(this) && Components<C3>.Value.HasDisabled(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasDisabledAnyOf<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                return Components<C1>.Value.HasDisabled(this) || Components<C2>.Value.HasDisabled(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasDisabledAnyOf<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                return Components<C1>.Value.HasDisabled(this) || Components<C2>.Value.HasDisabled(this) || Components<C3>.Value.HasDisabled(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasEnabledAllOf<C1>()
                where C1 : struct, IComponent {
                return Components<C1>.Value.HasEnabled(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasEnabledAllOf<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                return Components<C1>.Value.HasEnabled(this) && Components<C2>.Value.HasEnabled(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasEnabledAllOf<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                return Components<C1>.Value.HasEnabled(this) && Components<C2>.Value.HasEnabled(this) && Components<C3>.Value.HasEnabled(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasEnabledAnyOf<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                return Components<C1>.Value.HasEnabled(this) || Components<C2>.Value.HasEnabled(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool HasEnabledAnyOf<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                return Components<C1>.Value.HasEnabled(this) || Components<C2>.Value.HasEnabled(this) || Components<C3>.Value.HasEnabled(this);
            }
            #endregion
            
            #region DISABLE
            [MethodImpl(AggressiveInlining)]
            public void Disable<C>()
                where C : struct, IComponent {
                Components<C>.Value.Disable(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Disable<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Components<C1>.Value.Disable(this);
                Components<C2>.Value.Disable(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Disable<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Components<C1>.Value.Disable(this);
                Components<C2>.Value.Disable(this);
                Components<C3>.Value.Disable(this);
            }
            #endregion

            #region ENABLE
            [MethodImpl(AggressiveInlining)]
            public void Enable<C>()
                where C : struct, IComponent {
                Components<C>.Value.Disable(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Enable<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Components<C1>.Value.Disable(this);
                Components<C2>.Value.Disable(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Enable<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Components<C1>.Value.Disable(this);
                Components<C2>.Value.Disable(this);
                Components<C3>.Value.Disable(this);
            }
            #endregion
            
            #region ADD
            [MethodImpl(AggressiveInlining)]
            public ref C Add<C>()
                where C : struct, IComponent {
                return ref Components<C>.Value.Add(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Add<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Components<C1>.Value.Add(this);
                Components<C2>.Value.Add(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Add<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Components<C1>.Value.Add(this);
                Components<C2>.Value.Add(this);
                Components<C3>.Value.Add(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Add<C1, C2, C3, C4>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Components<C1>.Value.Add(this);
                Components<C2>.Value.Add(this);
                Components<C3>.Value.Add(this);
                Components<C4>.Value.Add(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Add<C1, C2, C3, C4, C5>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Components<C1>.Value.Add(this);
                Components<C2>.Value.Add(this);
                Components<C3>.Value.Add(this);
                Components<C4>.Value.Add(this);
                Components<C5>.Value.Add(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Add<C1>(C1 c1)
                where C1 : struct, IComponent {
                Components<C1>.Value.Add(this) = c1;
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Add<C1, C2>(C1 c1, C2 c2)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Components<C1>.Value.Add(this) = c1;
                Components<C2>.Value.Add(this) = c2;
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Add<C1, C2, C3>(C1 c1, C2 c2, C3 c3)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Components<C1>.Value.Add(this) = c1;
                Components<C2>.Value.Add(this) = c2;
                Components<C3>.Value.Add(this) = c3;
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Add<C1, C2, C3, C4>(C1 c1, C2 c2, C3 c3, C4 c4)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Components<C1>.Value.Add(this) = c1;
                Components<C2>.Value.Add(this) = c2;
                Components<C3>.Value.Add(this) = c3;
                Components<C4>.Value.Add(this) = c4;
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Add<C1, C2, C3, C4, C5>(C1 c1, C2 c2, C3 c3, C4 c4, C5 c5)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Components<C1>.Value.Add(this) = c1;
                Components<C2>.Value.Add(this) = c2;
                Components<C3>.Value.Add(this) = c3;
                Components<C4>.Value.Add(this) = c4;
                Components<C5>.Value.Add(this) = c5;
            }
            
            [MethodImpl(AggressiveInlining)]
            public ref C TryAdd<C>()
                where C : struct, IComponent {
                return ref Components<C>.Value.TryAdd(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Components<C1>.Value.TryAdd(this);
                Components<C2>.Value.TryAdd(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Components<C1>.Value.TryAdd(this);
                Components<C2>.Value.TryAdd(this);
                Components<C3>.Value.TryAdd(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd<C1, C2, C3, C4>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Components<C1>.Value.TryAdd(this);
                Components<C2>.Value.TryAdd(this);
                Components<C3>.Value.TryAdd(this);
                Components<C4>.Value.TryAdd(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd<C1, C2, C3, C4, C5>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Components<C1>.Value.TryAdd(this);
                Components<C2>.Value.TryAdd(this);
                Components<C3>.Value.TryAdd(this);
                Components<C4>.Value.TryAdd(this);
                Components<C5>.Value.TryAdd(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public ref C TryAdd<C>(out bool added)
                where C : struct, IComponent {
                return ref Components<C>.Value.TryAdd(this, out added);
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd<C1, C2>(out bool added)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Components<C1>.Value.TryAdd(this, out var added1);
                Components<C2>.Value.TryAdd(this, out var added2);
                added = added1 || added2;
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd<C1, C2, C3>(out bool added)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Components<C1>.Value.TryAdd(this, out var added1);
                Components<C2>.Value.TryAdd(this, out var added2);
                Components<C3>.Value.TryAdd(this, out var added3);
                added = added1 || added2 || added3;
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd<C1, C2, C3, C4>(out bool added)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Components<C1>.Value.TryAdd(this, out var added1);
                Components<C2>.Value.TryAdd(this, out var added2);
                Components<C3>.Value.TryAdd(this, out var added3);
                Components<C4>.Value.TryAdd(this, out var added4);
                added = added1 || added2 || added3 || added4;
            }
            
            [MethodImpl(AggressiveInlining)]
            public void TryAdd<C1, C2, C3, C4, C5>(out bool added)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Components<C1>.Value.TryAdd(this, out var added1);
                Components<C2>.Value.TryAdd(this, out var added2);
                Components<C3>.Value.TryAdd(this, out var added3);
                Components<C4>.Value.TryAdd(this, out var added4);
                Components<C5>.Value.TryAdd(this, out var added5);
                added = added1 || added2 || added3 || added4 || added5;
            }
            #endregion

            #region PUT
            [MethodImpl(AggressiveInlining)]
            public void Put<C>(C comp)
                where C : struct, IComponent {
                Components<C>.Value.Put(this, comp);
            }

            [MethodImpl(AggressiveInlining)]
            public void Put<C1, C2>(C1 comp1, C2 comp2)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Components<C1>.Value.Put(this, comp1);
                Components<C2>.Value.Put(this, comp2);
            }

            [MethodImpl(AggressiveInlining)]
            public void Put<C1, C2, C3>(C1 comp1, C2 comp2, C3 comp3)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Components<C1>.Value.Put(this, comp1);
                Components<C2>.Value.Put(this, comp2);
                Components<C3>.Value.Put(this, comp3);
            }

            [MethodImpl(AggressiveInlining)]
            public void Put<C1, C2, C3, C4>(C1 comp1, C2 comp2, C3 comp3, C4 comp4)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Components<C1>.Value.Put(this, comp1);
                Components<C2>.Value.Put(this, comp2);
                Components<C3>.Value.Put(this, comp3);
                Components<C4>.Value.Put(this, comp4);
            }

            [MethodImpl(AggressiveInlining)]
            public void Put<C1, C2, C3, C4, C5>(C1 comp1, C2 comp2, C3 comp3, C4 comp4, C5 comp5)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Components<C1>.Value.Put(this, comp1);
                Components<C2>.Value.Put(this, comp2);
                Components<C3>.Value.Put(this, comp3);
                Components<C4>.Value.Put(this, comp4);
                Components<C5>.Value.Put(this, comp5);
            }
            #endregion

            #region DELETE
            [MethodImpl(AggressiveInlining)]
            public void Delete<C>() where C : struct, IComponent {
                Components<C>.Value.Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Delete<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Components<C1>.Value.Delete(this);
                Components<C2>.Value.Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Delete<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Components<C1>.Value.Delete(this);
                Components<C2>.Value.Delete(this);
                Components<C3>.Value.Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Delete<C1, C2, C3, C4>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Components<C1>.Value.Delete(this);
                Components<C2>.Value.Delete(this);
                Components<C3>.Value.Delete(this);
                Components<C4>.Value.Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Delete<C1, C2, C3, C4, C5>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Components<C1>.Value.Delete(this);
                Components<C2>.Value.Delete(this);
                Components<C3>.Value.Delete(this);
                Components<C4>.Value.Delete(this);
                Components<C5>.Value.Delete(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryDelete<C>() where C : struct, IComponent {
                return Components<C>.Value.TryDelete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryDelete<C1, C2>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                var delC1 = Components<C1>.Value.TryDelete(this);
                var delC2 = Components<C2>.Value.TryDelete(this);
                return delC1 && delC2;
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryDelete<C1, C2, C3>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                var delC1 = Components<C1>.Value.TryDelete(this);
                var delC2 = Components<C2>.Value.TryDelete(this);
                var delC3 = Components<C3>.Value.TryDelete(this);

                return delC1 && delC2 && delC3;
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryDelete<C1, C2, C3, C4>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                var delC1 = Components<C1>.Value.TryDelete(this);
                var delC2 = Components<C2>.Value.TryDelete(this);
                var delC3 = Components<C3>.Value.TryDelete(this);
                var delC4 = Components<C4>.Value.TryDelete(this);

                return delC1 && delC2 && delC3 && delC4;
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryDelete<C1, C2, C3, C4, C5>()
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                var delC1 = Components<C1>.Value.TryDelete(this);
                var delC2 = Components<C2>.Value.TryDelete(this);
                var delC3 = Components<C3>.Value.TryDelete(this);
                var delC4 = Components<C4>.Value.TryDelete(this);
                var delC5 = Components<C5>.Value.TryDelete(this);

                return delC1 && delC2 && delC3 && delC4 && delC5;
            }
            #endregion

            #region COPY
            [MethodImpl(AggressiveInlining)]
            public void CopyComponentsTo<C1>(Entity target)
                where C1 : struct, IComponent {
                Components<C1>.Value.Copy(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void CopyComponentsTo<C1, C2>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Components<C1>.Value.Copy(this, target);
                Components<C2>.Value.Copy(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void CopyComponentsTo<C1, C2, C3>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Components<C1>.Value.Copy(this, target);
                Components<C2>.Value.Copy(this, target);
                Components<C3>.Value.Copy(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void CopyComponentsTo<C1, C2, C3, C4>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Components<C1>.Value.Copy(this, target);
                Components<C2>.Value.Copy(this, target);
                Components<C3>.Value.Copy(this, target);
                Components<C4>.Value.Copy(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void CopyComponentsTo<C1, C2, C3, C4, C5>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Components<C1>.Value.Copy(this, target);
                Components<C2>.Value.Copy(this, target);
                Components<C3>.Value.Copy(this, target);
                Components<C4>.Value.Copy(this, target);
                Components<C5>.Value.Copy(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryCopyComponentsTo<C1>(Entity target)
                where C1 : struct, IComponent {
                return Components<C1>.Value.TryCopy(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryCopyComponentsTo<C1, C2>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                var copied1 = Components<C1>.Value.TryCopy(this, target);
                var copied2 = Components<C2>.Value.TryCopy(this, target);
                return copied1 && copied2;
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryCopyComponentsTo<C1, C2, C3>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                var copied1 = Components<C1>.Value.TryCopy(this, target);
                var copied2 = Components<C2>.Value.TryCopy(this, target);
                var copied3 = Components<C3>.Value.TryCopy(this, target);
                return copied1 && copied2 && copied3;
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryCopyComponentsTo<C1, C2, C3, C4>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                var copied1 = Components<C1>.Value.TryCopy(this, target);
                var copied2 = Components<C2>.Value.TryCopy(this, target);
                var copied3 = Components<C3>.Value.TryCopy(this, target);
                var copied4 = Components<C4>.Value.TryCopy(this, target);
                return copied1 && copied2 && copied3 && copied4;
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryCopyComponentsTo<C1, C2, C3, C4, C5>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                var copied1 = Components<C1>.Value.TryCopy(this, target);
                var copied2 = Components<C2>.Value.TryCopy(this, target);
                var copied3 = Components<C3>.Value.TryCopy(this, target);
                var copied4 = Components<C4>.Value.TryCopy(this, target);
                var copied5 = Components<C5>.Value.TryCopy(this, target);
                return copied1 && copied2 && copied3 && copied4 && copied5;
            }
            #endregion
            
            #region MOVE
            [MethodImpl(AggressiveInlining)]
            public void MoveComponentsTo<C1>(Entity target)
                where C1 : struct, IComponent {
                Components<C1>.Value.Move(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void MoveComponentsTo<C1, C2>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                Components<C1>.Value.Move(this, target);
                Components<C2>.Value.Move(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void MoveComponentsTo<C1, C2, C3>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                Components<C1>.Value.Move(this, target);
                Components<C2>.Value.Move(this, target);
                Components<C3>.Value.Move(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void MoveComponentsTo<C1, C2, C3, C4>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                Components<C1>.Value.Move(this, target);
                Components<C2>.Value.Move(this, target);
                Components<C3>.Value.Move(this, target);
                Components<C4>.Value.Move(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void MoveComponentsTo<C1, C2, C3, C4, C5>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                Components<C1>.Value.Move(this, target);
                Components<C2>.Value.Move(this, target);
                Components<C3>.Value.Move(this, target);
                Components<C4>.Value.Move(this, target);
                Components<C5>.Value.Move(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryMoveComponentsTo<C1>(Entity target)
                where C1 : struct, IComponent {
                return Components<C1>.Value.TryMove(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryMoveComponentsTo<C1, C2>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent {
                var moved1 = Components<C1>.Value.TryMove(this, target);
                var moved2 = Components<C2>.Value.TryMove(this, target);
                return moved1 && moved2;
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryMoveComponentsTo<C1, C2, C3>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent {
                var moved1 = Components<C1>.Value.TryMove(this, target);
                var moved2 = Components<C2>.Value.TryMove(this, target);
                var moved3 = Components<C3>.Value.TryMove(this, target);
                return moved1 && moved2 && moved3;
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryMoveComponentsTo<C1, C2, C3, C4>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent {
                var moved1 = Components<C1>.Value.TryMove(this, target);
                var moved2 = Components<C2>.Value.TryMove(this, target);
                var moved3 = Components<C3>.Value.TryMove(this, target);
                var moved4 = Components<C4>.Value.TryMove(this, target);
                return moved1 && moved2 && moved3 && moved4;
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryMoveComponentsTo<C1, C2, C3, C4, C5>(Entity target)
                where C1 : struct, IComponent
                where C2 : struct, IComponent
                where C3 : struct, IComponent
                where C4 : struct, IComponent
                where C5 : struct, IComponent {
                var moved1 = Components<C1>.Value.TryMove(this, target);
                var moved2 = Components<C2>.Value.TryMove(this, target);
                var moved3 = Components<C3>.Value.TryMove(this, target);
                var moved4 = Components<C4>.Value.TryMove(this, target);
                var moved5 = Components<C5>.Value.TryMove(this, target);
                return moved1 && moved2 && moved3 && moved4 && moved5;
            }
            #endregion
            #endregion

            #region BY_ID
            #region HAS
            [MethodImpl(AggressiveInlining)]
            public bool HasAllOf(ComponentDynId c) {
                return ModuleComponents.Value.GetPool(c).Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOf(ComponentDynId c1, ComponentDynId c2) {
                return ModuleComponents.Value.GetPool(c1).Has(this) && ModuleComponents.Value.GetPool(c2).Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOf(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3) {
                return ModuleComponents.Value.GetPool(c1).Has(this) && ModuleComponents.Value.GetPool(c2).Has(this) && ModuleComponents.Value.GetPool(c3).Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOf(ComponentDynId c1, ComponentDynId c2) {
                return ModuleComponents.Value.GetPool(c1).Has(this) || ModuleComponents.Value.GetPool(c2).Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOf(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3) {
                return ModuleComponents.Value.GetPool(c1).Has(this) || ModuleComponents.Value.GetPool(c2).Has(this) || ModuleComponents.Value.GetPool(c3).Has(this);
            }
            #endregion
            
            #region ADD
            [MethodImpl(AggressiveInlining)]
            public void Add(ComponentDynId c) {
                ModuleComponents.Value.GetPool(c).Add(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Add(ComponentDynId c1, ComponentDynId c2) {
                ModuleComponents.Value.GetPool(c1).Add(this);
                ModuleComponents.Value.GetPool(c2).Add(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Add(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3) {
                ModuleComponents.Value.GetPool(c1).Add(this);
                ModuleComponents.Value.GetPool(c2).Add(this);
                ModuleComponents.Value.GetPool(c3).Add(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Add(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4) {
                ModuleComponents.Value.GetPool(c1).Add(this);
                ModuleComponents.Value.GetPool(c2).Add(this);
                ModuleComponents.Value.GetPool(c3).Add(this);
                ModuleComponents.Value.GetPool(c4).Add(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Add(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4, ComponentDynId c5) {
                ModuleComponents.Value.GetPool(c1).Add(this);
                ModuleComponents.Value.GetPool(c2).Add(this);
                ModuleComponents.Value.GetPool(c3).Add(this);
                ModuleComponents.Value.GetPool(c4).Add(this);
                ModuleComponents.Value.GetPool(c5).Add(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd(ComponentDynId c) {
                ModuleComponents.Value.GetPool(c).TryAdd(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd(ComponentDynId c1, ComponentDynId c2) {
                ModuleComponents.Value.GetPool(c1).TryAdd(this);
                ModuleComponents.Value.GetPool(c2).TryAdd(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3) {
                ModuleComponents.Value.GetPool(c1).TryAdd(this);
                ModuleComponents.Value.GetPool(c2).TryAdd(this);
                ModuleComponents.Value.GetPool(c3).TryAdd(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4) {
                ModuleComponents.Value.GetPool(c1).TryAdd(this);
                ModuleComponents.Value.GetPool(c2).TryAdd(this);
                ModuleComponents.Value.GetPool(c3).TryAdd(this);
                ModuleComponents.Value.GetPool(c4).TryAdd(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4, ComponentDynId c5) {
                ModuleComponents.Value.GetPool(c1).TryAdd(this);
                ModuleComponents.Value.GetPool(c2).TryAdd(this);
                ModuleComponents.Value.GetPool(c3).TryAdd(this);
                ModuleComponents.Value.GetPool(c4).TryAdd(this);
                ModuleComponents.Value.GetPool(c5).TryAdd(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void TryAdd(ComponentDynId c, out bool added) {
                ModuleComponents.Value.GetPool(c).TryAdd(this, out added);
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd(ComponentDynId c1, ComponentDynId c2, out bool added) {
                ModuleComponents.Value.GetPool(c1).TryAdd(this, out var added1);
                ModuleComponents.Value.GetPool(c2).TryAdd(this, out var added2);
                added = added1 || added2;
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, out bool added) {
                ModuleComponents.Value.GetPool(c1).TryAdd(this, out var added1);
                ModuleComponents.Value.GetPool(c2).TryAdd(this, out var added2);
                ModuleComponents.Value.GetPool(c3).TryAdd(this, out var added3);
                added = added1 || added2 || added3;
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4, out bool added) {
                ModuleComponents.Value.GetPool(c1).TryAdd(this, out var added1);
                ModuleComponents.Value.GetPool(c2).TryAdd(this, out var added2);
                ModuleComponents.Value.GetPool(c3).TryAdd(this, out var added3);
                ModuleComponents.Value.GetPool(c4).TryAdd(this, out var added4);
                added = added1 || added2 || added3 || added4;
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4, ComponentDynId c5, out bool added) {
                ModuleComponents.Value.GetPool(c1).TryAdd(this, out var added1);
                ModuleComponents.Value.GetPool(c2).TryAdd(this, out var added2);
                ModuleComponents.Value.GetPool(c3).TryAdd(this, out var added3);
                ModuleComponents.Value.GetPool(c4).TryAdd(this, out var added4);
                ModuleComponents.Value.GetPool(c5).TryAdd(this, out var added5);
                added = added1 || added2 || added3 || added4 || added5;
            }
            #endregion
            
            #region DELETE
            [MethodImpl(AggressiveInlining)]
            public void Delete(ComponentDynId c) {
                ModuleComponents.Value.GetPool(c).Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Delete(ComponentDynId c1, ComponentDynId c2) {
                ModuleComponents.Value.GetPool(c1).Delete(this);
                ModuleComponents.Value.GetPool(c2).Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Delete(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3) {
                ModuleComponents.Value.GetPool(c1).Delete(this);
                ModuleComponents.Value.GetPool(c2).Delete(this);
                ModuleComponents.Value.GetPool(c3).Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Delete(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4) {
                ModuleComponents.Value.GetPool(c1).Delete(this);
                ModuleComponents.Value.GetPool(c2).Delete(this);
                ModuleComponents.Value.GetPool(c3).Delete(this);
                ModuleComponents.Value.GetPool(c4).Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void Delete(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4, ComponentDynId c5) {
                ModuleComponents.Value.GetPool(c1).Delete(this);
                ModuleComponents.Value.GetPool(c2).Delete(this);
                ModuleComponents.Value.GetPool(c3).Delete(this);
                ModuleComponents.Value.GetPool(c4).Delete(this);
                ModuleComponents.Value.GetPool(c5).Delete(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryDelete(ComponentDynId c) {
                return ModuleComponents.Value.GetPool(c).TryDelete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryDelete(ComponentDynId c1, ComponentDynId c2) {
                var delC1 = ModuleComponents.Value.GetPool(c1).TryDelete(this);
                var delC2 = ModuleComponents.Value.GetPool(c2).TryDelete(this);

                return delC1 && delC2;
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryDelete(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3) {
                var delC1 = ModuleComponents.Value.GetPool(c1).TryDelete(this);
                var delC2 = ModuleComponents.Value.GetPool(c2).TryDelete(this);
                var delC3 = ModuleComponents.Value.GetPool(c3).TryDelete(this);

                return delC1 && delC2 && delC3;
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryDelete(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4) {
                var delC1 = ModuleComponents.Value.GetPool(c1).TryDelete(this);
                var delC2 = ModuleComponents.Value.GetPool(c2).TryDelete(this);
                var delC3 = ModuleComponents.Value.GetPool(c3).TryDelete(this);
                var delC4 = ModuleComponents.Value.GetPool(c4).TryDelete(this);

                return delC1 && delC2 && delC3 && delC4;
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryDelete(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4, ComponentDynId c5) {
                var delC1 = ModuleComponents.Value.GetPool(c1).TryDelete(this);
                var delC2 = ModuleComponents.Value.GetPool(c2).TryDelete(this);
                var delC3 = ModuleComponents.Value.GetPool(c3).TryDelete(this);
                var delC4 = ModuleComponents.Value.GetPool(c4).TryDelete(this);
                var delC5 = ModuleComponents.Value.GetPool(c5).TryDelete(this);

                return delC1 && delC2 && delC3 && delC4 && delC5;
            }
            #endregion
            
            #region COPY
            [MethodImpl(AggressiveInlining)]
            public void CopyComponentsTo(ComponentDynId c, Entity target) {
                ModuleComponents.Value.GetPool(c).Copy(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyComponentsTo(ComponentDynId c1, ComponentDynId c2, Entity target) {
                ModuleComponents.Value.GetPool(c1).Copy(this, target);
                ModuleComponents.Value.GetPool(c2).Copy(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyComponentsTo(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, Entity target) {
                ModuleComponents.Value.GetPool(c1).Copy(this, target);
                ModuleComponents.Value.GetPool(c2).Copy(this, target);
                ModuleComponents.Value.GetPool(c3).Copy(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyComponentsTo(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4, Entity target) {
                ModuleComponents.Value.GetPool(c1).Copy(this, target);
                ModuleComponents.Value.GetPool(c2).Copy(this, target);
                ModuleComponents.Value.GetPool(c3).Copy(this, target);
                ModuleComponents.Value.GetPool(c4).Copy(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyComponentsTo(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4, ComponentDynId c5, Entity target) {
                ModuleComponents.Value.GetPool(c1).Copy(this, target);
                ModuleComponents.Value.GetPool(c2).Copy(this, target);
                ModuleComponents.Value.GetPool(c3).Copy(this, target);
                ModuleComponents.Value.GetPool(c4).Copy(this, target);
                ModuleComponents.Value.GetPool(c5).Copy(this, target);
            }
            #endregion
            
            #region MOVE
            [MethodImpl(AggressiveInlining)]
            public void MoveComponentsTo(ComponentDynId c, Entity target) {
                ModuleComponents.Value.GetPool(c).Move(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void MoveComponentsTo(ComponentDynId c1, ComponentDynId c2, Entity target) {
                ModuleComponents.Value.GetPool(c1).Move(this, target);
                ModuleComponents.Value.GetPool(c2).Move(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void MoveComponentsTo(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, Entity target) {
                ModuleComponents.Value.GetPool(c1).Move(this, target);
                ModuleComponents.Value.GetPool(c2).Move(this, target);
                ModuleComponents.Value.GetPool(c3).Move(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void MoveComponentsTo(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4, Entity target) {
                ModuleComponents.Value.GetPool(c1).Move(this, target);
                ModuleComponents.Value.GetPool(c2).Move(this, target);
                ModuleComponents.Value.GetPool(c3).Move(this, target);
                ModuleComponents.Value.GetPool(c4).Move(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void MoveComponentsTo(ComponentDynId c1, ComponentDynId c2, ComponentDynId c3, ComponentDynId c4, ComponentDynId c5, Entity target) {
                ModuleComponents.Value.GetPool(c1).Move(this, target);
                ModuleComponents.Value.GetPool(c2).Move(this, target);
                ModuleComponents.Value.GetPool(c3).Move(this, target);
                ModuleComponents.Value.GetPool(c4).Move(this, target);
                ModuleComponents.Value.GetPool(c5).Move(this, target);
            }
            #endregion
            #endregion
            
            #region BY_RAW_TYPE
            [MethodImpl(AggressiveInlining)]
            public bool HasAllOf(Type componentType) {
                return ModuleComponents.Value.GetPool(componentType).Has(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Add(Type componentType) {
                ModuleComponents.Value.GetPool(componentType).Add(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public IComponent GetRaw(Type componentType) {
               return ModuleComponents.Value.GetPool(componentType).GetRaw(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void PutRaw(IComponent component) {
                ModuleComponents.Value.GetPool(component.GetType()).PutRaw(this, component);
            }

            [MethodImpl(AggressiveInlining)]
            public void TryAdd(Type componentType) {
                ModuleComponents.Value.GetPool(componentType).TryAdd(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void TryAdd(Type componentType, out bool added) {
                ModuleComponents.Value.GetPool(componentType).TryAdd(this, out added);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryDelete(Type componentType) {
                return ModuleComponents.Value.GetPool(componentType).TryDelete(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void Delete(Type componentType) {
                ModuleComponents.Value.GetPool(componentType).Delete(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void CopyComponentsTo(Type componentType, Entity target) {
                ModuleComponents.Value.GetPool(componentType).Copy(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void MoveComponentsTo(Type componentType, Entity target) {
                ModuleComponents.Value.GetPool(componentType).Move(this, target);
            }
            #endregion
        }
    }
    
    public partial interface IEntity {
        public int ComponentsCount(bool withDisabled = true);

        public void GetAllComponents(List<IComponent> result, bool withDisabled = true);
        
        public ref C RefMut<C>() where C : struct, IComponent;

        public ref readonly C Ref<C>() where C : struct, IComponent;
        
        public bool HasAllOf<C>() where C : struct, IComponent;
        
        public bool HasDisabledAllOf<C>() where C : struct, IComponent;
        
        public bool HasEnabledAllOf<C>() where C : struct, IComponent;

        public ref C Add<C>() where C : struct, IComponent;
        
        public void Add<C>(C component) where C : struct, IComponent;

        public void Put<C>(C component) where C : struct, IComponent;

        public void Disable<C>() where C : struct, IComponent;

        public void Enable<C>() where C : struct, IComponent;

        public ref C TryAdd<C>() where C : struct, IComponent;
        
        public ref C TryAdd<C>(out bool added) where C : struct, IComponent;

        public bool TryDelete<C>() where C : struct, IComponent;
        
        public void Delete<C>() where C : struct, IComponent;

        public bool HasAllOf(ComponentDynId c);
        
        public void Add(ComponentDynId c);

        public void TryAdd(ComponentDynId c);
        
        public void TryAdd(ComponentDynId c, out bool added);

        public bool TryDelete(ComponentDynId c);

        public void Delete(ComponentDynId c);

        public bool HasAllOf(Type componentType);

        public void Add(Type componentType);

        public IComponent GetRaw(Type componentType);

        public void PutRaw(IComponent component);

        public void TryAdd(Type componentType);

        public void TryAdd(Type componentType, out bool added);

        public bool TryDelete(Type componentType);
        
        public void Delete(Type componentType);
    }
}