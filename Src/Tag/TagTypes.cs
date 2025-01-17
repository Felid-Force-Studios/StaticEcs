#if !FFS_ECS_DISABLE_TAGS
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    public interface IComponentTags {
        public void SetData<WorldType>(ref int minCount, ref int[] _entities) where WorldType : struct, IWorldType;

        public void SetMask<WorldType>(byte bufId) where WorldType : struct, IWorldType;

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        public void Dispose<WorldType>() where WorldType : struct, IWorldType;
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public static class ComponentTagsExtensions {
        
        [MethodImpl(AggressiveInlining)]
        public static TagBox Box<T>(this T types) where T : IComponentTags {
            return new TagBox(types);
        }
                
        [MethodImpl(AggressiveInlining)]
        public static TagAllTypes<T> All<T>(this T types) where T : struct, IComponentTags {
            return new TagAllTypes<T>(types);
        }
        
        [MethodImpl(AggressiveInlining)]
        public static TagAllAndNoneTypes<T, W> AllAndNone<T, W>(this T types, W without = default) where T : struct, IComponentTags 
                                                                                              where W : struct, IComponentTags {
            return new TagAllAndNoneTypes<T, W>(types, without);
        }
        
        [MethodImpl(AggressiveInlining)]
        public static TagNoneTypes<T> None<T>(this T types) where T : struct, IComponentTags {
            return new TagNoneTypes<T>(types);
        }
        
        [MethodImpl(AggressiveInlining)]
        public static TagAnyTypes<T> Any<T>(this T types) where T : struct, IComponentTags {
            return new TagAnyTypes<T>(types);
        }
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag<C1> : IComponentTags, Stateless
        where C1 : struct, ITag {
        
        [MethodImpl(AggressiveInlining)]
        public static TagAll<C1> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagNone<C1> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagAllAndNoneTypes<Tag<C1>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTags => default;
        
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockTags<WorldType>(1);
            #endif

            Ecs<WorldType>.ModuleTags.Value.BitMask.SetInBuffer(bufId, Ecs<WorldType>.Tags<C1>.Value.id);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            BlockTags<WorldType>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldType>(int val) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag<C1, C2> : IComponentTags, Stateless
        where C1 : struct, ITag
        where C2 : struct, ITag {
        
        [MethodImpl(AggressiveInlining)]
        public static TagAll<C1, C2> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagNone<C1, C2> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagAllAndNoneTypes<Tag<C1, C2>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTags => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagAny<C1, C2> Any() => default;
        
       
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockTags<WorldType>(1);
            #endif

            Ecs<WorldType>.ModuleTags.Value.BitMask.SetInBuffer(bufId, 
                                                    Ecs<WorldType>.Tags<C1>.Value.id,
                                                    Ecs<WorldType>.Tags<C2>.Value.id);
        }


        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            BlockTags<WorldType>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldType>(int val) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag<C1, C2, C3> : IComponentTags, Stateless
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag {
        
        [MethodImpl(AggressiveInlining)]
        public static TagAll<C1, C2, C3> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagNone<C1, C2, C3> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagAllAndNoneTypes<Tag<C1, C2, C3>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTags => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagAny<C1, C2, C3> Any() => default;
       
       [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockTags<WorldType>(1);
            #endif

            Ecs<WorldType>.ModuleTags.Value.BitMask.SetInBuffer(bufId, 
                                                    Ecs<WorldType>.Tags<C1>.Value.id,
                                                    Ecs<WorldType>.Tags<C2>.Value.id,
                                                    Ecs<WorldType>.Tags<C3>.Value.id);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            BlockTags<WorldType>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldType>(int val) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C3>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag<C1, C2, C3, C4> : IComponentTags, Stateless
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag {
        
        [MethodImpl(AggressiveInlining)]
        public static TagAll<C1, C2, C3, C4> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagNone<C1, C2, C3, C4> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagAllAndNoneTypes<Tag<C1, C2, C3, C4>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTags => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagAny<C1, C2, C3, C4> Any() => default;
        
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }

        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockTags<WorldType>(1);
            #endif

            Ecs<WorldType>.ModuleTags.Value.BitMask.SetInBuffer(bufId, 
                                                    Ecs<WorldType>.Tags<C1>.Value.id,
                                                    Ecs<WorldType>.Tags<C2>.Value.id,
                                                    Ecs<WorldType>.Tags<C3>.Value.id,
                                                    Ecs<WorldType>.Tags<C4>.Value.id);
        }

        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            BlockTags<WorldType>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldType>(int val) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C3>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C4>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag<C1, C2, C3, C4, C5> : IComponentTags, Stateless
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag {
        
        [MethodImpl(AggressiveInlining)]
        public static TagAll<C1, C2, C3, C4, C5> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagNone<C1, C2, C3, C4, C5> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagAllAndNoneTypes<Tag<C1, C2, C3, C4, C5>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTags => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagAny<C1, C2, C3, C4, C5> Any() => default;
       
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C5>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockTags<WorldType>(1);
            #endif

            Ecs<WorldType>.ModuleTags.Value.BitMask.SetInBuffer(bufId, 
                                                    Ecs<WorldType>.Tags<C1>.Value.id,
                                                    Ecs<WorldType>.Tags<C2>.Value.id,
                                                    Ecs<WorldType>.Tags<C3>.Value.id,
                                                    Ecs<WorldType>.Tags<C4>.Value.id,
                                                    Ecs<WorldType>.Tags<C5>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            BlockTags<WorldType>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldType>(int val) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C3>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C4>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C5>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag<C1, C2, C3, C4, C5, C6> : IComponentTags, Stateless
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag {
        
        [MethodImpl(AggressiveInlining)]
        public static TagAll<C1, C2, C3, C4, C5, C6> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagNone<C1, C2, C3, C4, C5, C6> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagAllAndNoneTypes<Tag<C1, C2, C3, C4, C5, C6>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTags => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagAny<C1, C2, C3, C4, C5, C6> Any() => default;
       
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C5>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C6>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockTags<WorldType>(1);
            #endif

            Ecs<WorldType>.ModuleTags.Value.BitMask.SetInBuffer(bufId, 
                                                    Ecs<WorldType>.Tags<C1>.Value.id,
                                                    Ecs<WorldType>.Tags<C2>.Value.id,
                                                    Ecs<WorldType>.Tags<C3>.Value.id,
                                                    Ecs<WorldType>.Tags<C4>.Value.id,
                                                    Ecs<WorldType>.Tags<C5>.Value.id,
                                                    Ecs<WorldType>.Tags<C6>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            BlockTags<WorldType>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldType>(int val) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C3>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C4>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C5>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C6>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag<C1, C2, C3, C4, C5, C6, C7> : IComponentTags, Stateless
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag {
        
        [MethodImpl(AggressiveInlining)]
        public static TagAll<C1, C2, C3, C4, C5, C6, C7> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagNone<C1, C2, C3, C4, C5, C6, C7> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagAllAndNoneTypes<Tag<C1, C2, C3, C4, C5, C6, C7>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTags => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagAny<C1, C2, C3, C4, C5, C6, C7> Any() => default;
       
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C5>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C6>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C7>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockTags<WorldType>(1);
            #endif

            Ecs<WorldType>.ModuleTags.Value.BitMask.SetInBuffer(bufId, 
                                                    Ecs<WorldType>.Tags<C1>.Value.id,
                                                    Ecs<WorldType>.Tags<C2>.Value.id,
                                                    Ecs<WorldType>.Tags<C3>.Value.id,
                                                    Ecs<WorldType>.Tags<C4>.Value.id,
                                                    Ecs<WorldType>.Tags<C5>.Value.id,
                                                    Ecs<WorldType>.Tags<C6>.Value.id,
                                                    Ecs<WorldType>.Tags<C7>.Value.id);
        }
       
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            BlockTags<WorldType>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldType>(int val) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C3>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C4>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C5>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C6>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C7>.Value.AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag<C1, C2, C3, C4, C5, C6, C7, C8> : IComponentTags, Stateless
        where C1 : struct, ITag
        where C2 : struct, ITag
        where C3 : struct, ITag
        where C4 : struct, ITag
        where C5 : struct, ITag
        where C6 : struct, ITag
        where C7 : struct, ITag
        where C8 : struct, ITag {
        
        [MethodImpl(AggressiveInlining)]
        public static TagAll<C1, C2, C3, C4, C5, C6, C7, C8> All() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagNone<C1, C2, C3, C4, C5, C6, C7, C8> None() => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagAllAndNoneTypes<Tag<C1, C2, C3, C4, C5, C6, C7, C8>, T> AllAndNone<T>(T without = default) where T : struct, IComponentTags => default;
        
        [MethodImpl(AggressiveInlining)]
        public static TagAny<C1, C2, C3, C4, C5, C6, C7, C8> Any() => default;

       
        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldType>(ref int minCount, ref int[] entities) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C2>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C3>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C4>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C5>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C6>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C7>.Value.SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldType>.Tags<C8>.Value.SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldType>(byte bufId) where WorldType : struct, IWorldType {
            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            BlockTags<WorldType>(1);
            #endif

            Ecs<WorldType>.ModuleTags.Value.BitMask.SetInBuffer(bufId, 
                                                    Ecs<WorldType>.Tags<C1>.Value.id,
                                                    Ecs<WorldType>.Tags<C2>.Value.id,
                                                    Ecs<WorldType>.Tags<C3>.Value.id,
                                                    Ecs<WorldType>.Tags<C4>.Value.id,
                                                    Ecs<WorldType>.Tags<C5>.Value.id,
                                                    Ecs<WorldType>.Tags<C6>.Value.id,
                                                    Ecs<WorldType>.Tags<C7>.Value.id,
                                                    Ecs<WorldType>.Tags<C8>.Value.id);
        }
        
        #if DEBUG || FFS_ECS_ENABLE_DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldType>() where WorldType : struct, IWorldType {
            BlockTags<WorldType>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldType>(int val) where WorldType : struct, IWorldType {
            Ecs<WorldType>.Tags<C1>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C2>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C3>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C4>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C5>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C6>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C7>.Value.AddBlocker(val);
            Ecs<WorldType>.Tags<C8>.Value.AddBlocker(val);
        }
        #endif
    }
}
#endif