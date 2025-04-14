using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public interface IComponentMasks {
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType;
    }
    
    public interface IComponentTypes : IComponentMasks {
        public void SetMinData<WorldType>(ref uint minCount, ref uint[] _entities) where WorldType : struct, IWorldType;

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        public void Block<WorldType>(int val) where WorldType : struct, IWorldType;
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct TypesBox : IComponentTypes {
        public readonly IComponentTypes Types;

        [MethodImpl(AggressiveInlining)]
        public TypesBox(IComponentTypes types) {
            Types = types;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetMinData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            Types.SetMinData<WorldType>(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            Types.SetBitMask<WorldType>(bufId);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Block<WorldType>(int val) where WorldType : struct, IWorldType {
            Types.Block<WorldType>(val);
        }
        #endif
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
        public void SetMinData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleComponents.Value.BitMask.SetInBuffer(bufId,  World<WorldType>.Components<C1>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Block<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
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
        public static Any<C1, C2> Any() => default;
        
       
        [MethodImpl(AggressiveInlining)]
        public void SetMinData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleComponents.Value.BitMask.SetInBuffer(bufId, World<WorldType>.Components<C1>.Value.id, World<WorldType>.Components<C2>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Block<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
            World<WorldType>.Components<C2>.Value.AddBlocker(val);
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
        public static Any<C1, C2, C3> Any() => default;
       
       [MethodImpl(AggressiveInlining)]
        public void SetMinData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleComponents.Value.BitMask.SetInBuffer(bufId,
                                                          World<WorldType>.Components<C1>.Value.id,
                                                          World<WorldType>.Components<C2>.Value.id,
                                                          World<WorldType>.Components<C3>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Block<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
            World<WorldType>.Components<C2>.Value.AddBlocker(val);
            World<WorldType>.Components<C3>.Value.AddBlocker(val);
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
        public static Any<C1, C2, C3, C4> Any() => default;
       
        [MethodImpl(AggressiveInlining)]
        public void SetMinData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleComponents.Value.BitMask.SetInBuffer(bufId,
                                                          World<WorldType>.Components<C1>.Value.id,
                                                          World<WorldType>.Components<C2>.Value.id,
                                                          World<WorldType>.Components<C3>.Value.id,
                                                          World<WorldType>.Components<C4>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Block<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
            World<WorldType>.Components<C2>.Value.AddBlocker(val);
            World<WorldType>.Components<C3>.Value.AddBlocker(val);
            World<WorldType>.Components<C4>.Value.AddBlocker(val);
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
        public static Any<C1, C2, C3, C4, C5> Any() => default;
       
        [MethodImpl(AggressiveInlining)]
        public void SetMinData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleComponents.Value.BitMask.SetInBuffer(bufId,
                                                          World<WorldType>.Components<C1>.Value.id,
                                                          World<WorldType>.Components<C2>.Value.id,
                                                          World<WorldType>.Components<C3>.Value.id,
                                                          World<WorldType>.Components<C4>.Value.id,
                                                          World<WorldType>.Components<C5>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Block<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
            World<WorldType>.Components<C2>.Value.AddBlocker(val);
            World<WorldType>.Components<C3>.Value.AddBlocker(val);
            World<WorldType>.Components<C4>.Value.AddBlocker(val);
            World<WorldType>.Components<C5>.Value.AddBlocker(val);
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
        public static Any<C1, C2, C3, C4, C5, C6> Any() => default;
       
        [MethodImpl(AggressiveInlining)]
        public void SetMinData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleComponents.Value.BitMask.SetInBuffer(bufId,
                                                          World<WorldType>.Components<C1>.Value.id,
                                                          World<WorldType>.Components<C2>.Value.id,
                                                          World<WorldType>.Components<C3>.Value.id,
                                                          World<WorldType>.Components<C4>.Value.id,
                                                          World<WorldType>.Components<C5>.Value.id,
                                                          World<WorldType>.Components<C6>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Block<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
            World<WorldType>.Components<C2>.Value.AddBlocker(val);
            World<WorldType>.Components<C3>.Value.AddBlocker(val);
            World<WorldType>.Components<C4>.Value.AddBlocker(val);
            World<WorldType>.Components<C5>.Value.AddBlocker(val);
            World<WorldType>.Components<C6>.Value.AddBlocker(val);
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
        public static Any<C1, C2, C3, C4, C5, C6, C7> Any() => default;
       
        [MethodImpl(AggressiveInlining)]
        public void SetMinData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleComponents.Value.BitMask.SetInBuffer(bufId,
                                                          World<WorldType>.Components<C1>.Value.id,
                                                          World<WorldType>.Components<C2>.Value.id,
                                                          World<WorldType>.Components<C3>.Value.id,
                                                          World<WorldType>.Components<C4>.Value.id,
                                                          World<WorldType>.Components<C5>.Value.id,
                                                          World<WorldType>.Components<C6>.Value.id,
                                                          World<WorldType>.Components<C7>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Block<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
            World<WorldType>.Components<C2>.Value.AddBlocker(val);
            World<WorldType>.Components<C3>.Value.AddBlocker(val);
            World<WorldType>.Components<C4>.Value.AddBlocker(val);
            World<WorldType>.Components<C5>.Value.AddBlocker(val);
            World<WorldType>.Components<C6>.Value.AddBlocker(val);
            World<WorldType>.Components<C7>.Value.AddBlocker(val);
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
        public static Any<C1, C2, C3, C4, C5, C6, C7, C8> Any() => default;

       
        [MethodImpl(AggressiveInlining)]
        public void SetMinData<WorldType>(ref uint minCount, ref uint[] entities) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C5>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C6>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C7>.Value.SetDataIfCountLess(ref minCount, ref entities);
            World<WorldType>.Components<C8>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetBitMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            World<WorldType>.ModuleComponents.Value.BitMask.SetInBuffer(bufId,
                                                          World<WorldType>.Components<C1>.Value.id,
                                                          World<WorldType>.Components<C2>.Value.id,
                                                          World<WorldType>.Components<C3>.Value.id,
                                                          World<WorldType>.Components<C4>.Value.id,
                                                          World<WorldType>.Components<C5>.Value.id,
                                                          World<WorldType>.Components<C6>.Value.id,
                                                          World<WorldType>.Components<C7>.Value.id,
                                                          World<WorldType>.Components<C8>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Block<WorldType>(int val) where WorldType : struct, IWorldType {
            World<WorldType>.Components<C1>.Value.AddBlocker(val);
            World<WorldType>.Components<C2>.Value.AddBlocker(val);
            World<WorldType>.Components<C3>.Value.AddBlocker(val);
            World<WorldType>.Components<C4>.Value.AddBlocker(val);
            World<WorldType>.Components<C5>.Value.AddBlocker(val);
            World<WorldType>.Components<C6>.Value.AddBlocker(val);
            World<WorldType>.Components<C7>.Value.AddBlocker(val);
            World<WorldType>.Components<C8>.Value.AddBlocker(val);
        }
        #endif
    }
}