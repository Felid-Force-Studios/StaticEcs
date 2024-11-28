using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    public interface IComponentTypes {
        public void SetData<WorldID>(ref int minCount, ref int[] _entities) where WorldID : struct, IWorldId;

        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId;

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        public void Dispose<WorldID>() where WorldID : struct, IWorldId;
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public static class ComponentTypesExtensions {
        [MethodImpl(AggressiveInlining)]
        public static TypesBox Box<T>(this T types) where T : struct, IComponentTypes {
            return new TypesBox(types);
        }
        
        [MethodImpl(AggressiveInlining)]
        public static AllTypes<T> All<T>(this T types) where T : struct, IComponentTypes {
            return new AllTypes<T>(types);
        }
        
        [MethodImpl(AggressiveInlining)]
        public static AllAndNoneTypes<T, W> AllAndNone<T, W>(this T types, W without = default) where T : struct, IComponentTypes 
                                                                                                where W : struct, IComponentTypes {
            return new AllAndNoneTypes<T, W>(types, without);
        }
        
        [MethodImpl(AggressiveInlining)]
        public static NoneTypes<T> None<T>(this T types) where T : struct, IComponentTypes {
            return new NoneTypes<T>(types);
        }
        
        [MethodImpl(AggressiveInlining)]
        public static AnyTypes<T> Any<T>(this T types) where T : struct, IComponentTypes {
            return new AnyTypes<T>(types);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types<C1> : IComponentTypes, Stateless
        where C1 : struct, IComponent {
        
        [MethodImpl(AggressiveInlining)]
        public static All<C1> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static None<C1> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static AllAndNoneTypes<Types<C1>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTypes => default;
        
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif
            
            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId,  Ecs<WorldID>.Components<C1>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types<C1, C2> : IComponentTypes, Stateless
        where C1 : struct, IComponent
        where C2 : struct, IComponent {
        
        [MethodImpl(AggressiveInlining)]
        public static All<C1, C2> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static None<C1, C2> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static AllAndNoneTypes<Types<C1, C2>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTypes => default;
        
        [MethodImpl(AggressiveInlining)]
        public static Any<C1, C2> Any() => default;
        
       
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif

            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId, Ecs<WorldID>.Components<C1>.Value.id, Ecs<WorldID>.Components<C2>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C2>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types<C1, C2, C3> : IComponentTypes, Stateless
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent {
        
        [MethodImpl(AggressiveInlining)]
        public static All<C1, C2, C3> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static None<C1, C2, C3> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static AllAndNoneTypes<Types<C1, C2, C3>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTypes => default;
        
        [MethodImpl(AggressiveInlining)]
        public static Any<C1, C2, C3> Any() => default;
       
       [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif

            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId,
                                                          Ecs<WorldID>.Components<C1>.Value.id,
                                                          Ecs<WorldID>.Components<C2>.Value.id,
                                                          Ecs<WorldID>.Components<C3>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C2>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C3>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types<C1, C2, C3, C4> : IComponentTypes, Stateless
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent {
        
        [MethodImpl(AggressiveInlining)]
        public static All<C1, C2, C3, C4> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static None<C1, C2, C3, C4> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static AllAndNoneTypes<Types<C1, C2, C3, C4>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTypes => default;
        
        [MethodImpl(AggressiveInlining)]
        public static Any<C1, C2, C3, C4> Any() => default;
       
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif

            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId,
                                                          Ecs<WorldID>.Components<C1>.Value.id,
                                                          Ecs<WorldID>.Components<C2>.Value.id,
                                                          Ecs<WorldID>.Components<C3>.Value.id,
                                                          Ecs<WorldID>.Components<C4>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C2>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C3>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C4>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types<C1, C2, C3, C4, C5> : IComponentTypes, Stateless
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent {
        
        [MethodImpl(AggressiveInlining)]
        public static All<C1, C2, C3, C4, C5> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static None<C1, C2, C3, C4, C5> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static AllAndNoneTypes<Types<C1, C2, C3, C4, C5>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTypes => default;
        
        [MethodImpl(AggressiveInlining)]
        public static Any<C1, C2, C3, C4, C5> Any() => default;
       
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C5>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif

            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId,
                                                          Ecs<WorldID>.Components<C1>.Value.id,
                                                          Ecs<WorldID>.Components<C2>.Value.id,
                                                          Ecs<WorldID>.Components<C3>.Value.id,
                                                          Ecs<WorldID>.Components<C4>.Value.id,
                                                          Ecs<WorldID>.Components<C5>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C2>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C3>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C4>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C5>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types<C1, C2, C3, C4, C5, C6> : IComponentTypes, Stateless
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent {
        
        [MethodImpl(AggressiveInlining)]
        public static All<C1, C2, C3, C4, C5, C6> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static None<C1, C2, C3, C4, C5, C6> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static AllAndNoneTypes<Types<C1, C2, C3, C4, C5, C6>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTypes => default;
        
        [MethodImpl(AggressiveInlining)]
        public static Any<C1, C2, C3, C4, C5, C6> Any() => default;
       
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C5>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C6>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif

            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId,
                                                          Ecs<WorldID>.Components<C1>.Value.id,
                                                          Ecs<WorldID>.Components<C2>.Value.id,
                                                          Ecs<WorldID>.Components<C3>.Value.id,
                                                          Ecs<WorldID>.Components<C4>.Value.id,
                                                          Ecs<WorldID>.Components<C5>.Value.id,
                                                          Ecs<WorldID>.Components<C6>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C2>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C3>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C4>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C5>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C6>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types<C1, C2, C3, C4, C5, C6, C7> : IComponentTypes, Stateless
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent {
        
        [MethodImpl(AggressiveInlining)]
        public static All<C1, C2, C3, C4, C5, C6, C7> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static None<C1, C2, C3, C4, C5, C6, C7> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static AllAndNoneTypes<Types<C1, C2, C3, C4, C5, C6, C7>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTypes => default;
        
        [MethodImpl(AggressiveInlining)]
        public static Any<C1, C2, C3, C4, C5, C6, C7> Any() => default;
       
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C5>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C6>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C7>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif

            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId,
                                                          Ecs<WorldID>.Components<C1>.Value.id,
                                                          Ecs<WorldID>.Components<C2>.Value.id,
                                                          Ecs<WorldID>.Components<C3>.Value.id,
                                                          Ecs<WorldID>.Components<C4>.Value.id,
                                                          Ecs<WorldID>.Components<C5>.Value.id,
                                                          Ecs<WorldID>.Components<C6>.Value.id,
                                                          Ecs<WorldID>.Components<C7>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C2>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C3>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C4>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C5>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C6>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C7>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Types<C1, C2, C3, C4, C5, C6, C7, C8> : IComponentTypes, Stateless
        where C1 : struct, IComponent
        where C2 : struct, IComponent
        where C3 : struct, IComponent
        where C4 : struct, IComponent
        where C5 : struct, IComponent
        where C6 : struct, IComponent
        where C7 : struct, IComponent
        where C8 : struct, IComponent {
        
        [MethodImpl(AggressiveInlining)]
        public static All<C1, C2, C3, C4, C5, C6, C7, C8> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static None<C1, C2, C3, C4, C5, C6, C7, C8> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static AllAndNoneTypes<Types<C1, C2, C3, C4, C5, C6, C7, C8>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTypes => default;
        
        [MethodImpl(AggressiveInlining)]
        public static Any<C1, C2, C3, C4, C5, C6, C7, C8> Any() => default;

       
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref int[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C5>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C6>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C7>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.Components<C8>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockComponents<WorldID>(1);
            #endif

            Ecs<WorldID>.ModuleComponents.Value.BitMask.SetInBuffer(bufId,
                                                          Ecs<WorldID>.Components<C1>.Value.id,
                                                          Ecs<WorldID>.Components<C2>.Value.id,
                                                          Ecs<WorldID>.Components<C3>.Value.id,
                                                          Ecs<WorldID>.Components<C4>.Value.id,
                                                          Ecs<WorldID>.Components<C5>.Value.id,
                                                          Ecs<WorldID>.Components<C6>.Value.id,
                                                          Ecs<WorldID>.Components<C7>.Value.id,
                                                          Ecs<WorldID>.Components<C8>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockComponents<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockComponents<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.Components<C1>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C2>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C3>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C4>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C5>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C6>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C7>.Value.AddBlocker(val);
            Ecs<WorldID>.Components<C8>.Value.AddBlocker(val);
        }
        #endif
    }
}