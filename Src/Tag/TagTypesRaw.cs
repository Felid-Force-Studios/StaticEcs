﻿#if !FFS_ECS_DISABLE_TAGS
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
    public readonly struct TagBox : IComponentTags {
        public readonly IComponentTags Tags;
        
        [MethodImpl(AggressiveInlining)]
        public TagBox(IComponentTags tags) {
            Tags = tags;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            Tags.SetData(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            Tags.SetMask<WorldID>(bufId);
        }

        #if DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            Tags.Dispose<WorldID>();
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag1 : IComponentTags {
        public readonly TagDynId C1;
        
        [MethodImpl(AggressiveInlining)]
        public Tag1(TagDynId c1) {
            C1 = c1;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG
            BlockTags<WorldID>(1);
            #endif

            BitMaskUtils<WorldID, ITag>.SetInBuffer(bufId,  C1.Val);
        }

        #if DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockTags<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).AddBlocker(val);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag2 : IComponentTags {
        public readonly TagDynId C1;
        public readonly TagDynId C2;

        [MethodImpl(AggressiveInlining)]
        public Tag2(TagDynId c1, TagDynId c2) {
            C1 = c1;
            C2 = c2;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C2).SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG            
            BlockTags<WorldID>(1);
            #endif
            
            BitMaskUtils<WorldID, ITag>.SetInBuffer(bufId, C1.Val, C2.Val);
        }
       
        #if DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockTags<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C2).AddBlocker(val);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag3 : IComponentTags {
        public readonly TagDynId C1;
        public readonly TagDynId C2;
        public readonly TagDynId C3;

        [MethodImpl(AggressiveInlining)]
        public Tag3(TagDynId c1, TagDynId c2, TagDynId c3) {
            C1 = c1;
            C2 = c2;
            C3 = c3;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C2).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C3).SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG            
            BlockTags<WorldID>(1);
            #endif

            BitMaskUtils<WorldID, ITag>.SetInBuffer(bufId, C1.Val, C2.Val, C3.Val);
        }
       
        #if DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockTags<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C2).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C3).AddBlocker(val);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag4 : IComponentTags {
        public readonly TagDynId C1;
        public readonly TagDynId C2;
        public readonly TagDynId C3;
        public readonly TagDynId C4;

        [MethodImpl(AggressiveInlining)]
        public Tag4(TagDynId c1, TagDynId c2, TagDynId c3, TagDynId c4) {
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C2).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C3).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C4).SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG            
            BlockTags<WorldID>(1);
            #endif

            BitMaskUtils<WorldID, ITag>.SetInBuffer(bufId, C1.Val, C2.Val, C3.Val, C4.Val);
        }

       
        #if DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockTags<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C2).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C3).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C4).AddBlocker(val);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag5 : IComponentTags {
        public readonly TagDynId C1;
        public readonly TagDynId C2;
        public readonly TagDynId C3;
        public readonly TagDynId C4;
        public readonly TagDynId C5;

        [MethodImpl(AggressiveInlining)]
        public Tag5(TagDynId c1, TagDynId c2, TagDynId c3, TagDynId c4, TagDynId c5) {
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
            C5 = c5;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C2).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C3).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C4).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C5).SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG            
            BlockTags<WorldID>(1);
            #endif
            
            BitMaskUtils<WorldID, ITag>.SetInBuffer(bufId, C1.Val, C2.Val, C3.Val, C4.Val, C5.Val);
        }
       
        #if DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockTags<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C2).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C3).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C4).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C5).AddBlocker(val);
        }
        #endif
    }

    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag6 : IComponentTags {
        public readonly TagDynId C1;
        public readonly TagDynId C2;
        public readonly TagDynId C3;
        public readonly TagDynId C4;
        public readonly TagDynId C5;
        public readonly TagDynId C6;

        [MethodImpl(AggressiveInlining)]
        public Tag6(TagDynId c1, TagDynId c2, TagDynId c3, TagDynId c4, TagDynId c5, TagDynId c6) {
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
            C5 = c5;
            C6 = c6;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C2).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C3).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C4).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C5).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C6).SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG            
            BlockTags<WorldID>(1);
            #endif

            BitMaskUtils<WorldID, ITag>.SetInBuffer(bufId, C1.Val, C2.Val, C3.Val, C4.Val, C5.Val, C6.Val);
        }
       
        #if DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockTags<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C2).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C3).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C4).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C5).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C6).AddBlocker(val);
        }
        #endif
    }
    
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag7 : IComponentTags {
        public readonly TagDynId C1;
        public readonly TagDynId C2;
        public readonly TagDynId C3;
        public readonly TagDynId C4;
        public readonly TagDynId C5;
        public readonly TagDynId C6;
        public readonly TagDynId C7;

        [MethodImpl(AggressiveInlining)]
        public Tag7(TagDynId c1, TagDynId c2, TagDynId c3, TagDynId c4, TagDynId c5, TagDynId c6, TagDynId c7) {
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
            C5 = c5;
            C6 = c6;
            C7 = c7;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C2).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C3).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C4).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C5).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C6).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C7).SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG            
            BlockTags<WorldID>(1);
            #endif

            BitMaskUtils<WorldID, ITag>.SetInBuffer(bufId, C1.Val, C2.Val, C3.Val, C4.Val, C5.Val, C6.Val, C7.Val);
        }
       
        #if DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockTags<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C2).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C3).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C4).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C5).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C6).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C7).AddBlocker(val);
        }
        #endif
    }

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct Tag8 : IComponentTags {
        public readonly TagDynId C1;
        public readonly TagDynId C2;
        public readonly TagDynId C3;
        public readonly TagDynId C4;
        public readonly TagDynId C5;
        public readonly TagDynId C6;
        public readonly TagDynId C7;
        public readonly TagDynId C8;

        [MethodImpl(AggressiveInlining)]
        public Tag8(TagDynId c1, TagDynId c2, TagDynId c3, TagDynId c4, TagDynId c5, TagDynId c6, TagDynId c7, TagDynId c8) {
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
            C5 = c5;
            C6 = c6;
            C7 = c7;
            C8 = c8;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C2).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C3).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C4).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C5).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C6).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C7).SetDataIfCountLess(ref minCount, ref entities);
            Ecs<WorldID>.ModuleTags.GetPool(C8).SetDataIfCountLess(ref minCount, ref entities);
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            #if DEBUG            
            BlockTags<WorldID>(1);
            #endif

            BitMaskUtils<WorldID, ITag>.SetInBuffer(bufId, C1.Val, C2.Val, C3.Val, C4.Val, C5.Val, C6.Val, C7.Val, C8.Val);
        }

        #if DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            BlockTags<WorldID>(-1);
        }

        [MethodImpl(AggressiveInlining)]
        public void BlockTags<WorldID>(int val) where WorldID : struct, IWorldId {
            Ecs<WorldID>.ModuleTags.GetPool(C1).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C2).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C3).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C4).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C5).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C6).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C7).AddBlocker(val);
            Ecs<WorldID>.ModuleTags.GetPool(C8).AddBlocker(val);
        }
        #endif
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public readonly struct TagArray : IComponentTags {
        public readonly TagDynId[] Tags;
        
        [MethodImpl(AggressiveInlining)]
        public TagArray(params TagDynId[] tags) {
            Tags = tags;
        }

        [MethodImpl(AggressiveInlining)]
        public void SetData<WorldID>(ref int minCount, ref Ecs<WorldID>.Entity[] entities) where WorldID : struct, IWorldId {
            foreach (var type in Tags) {
                Ecs<WorldID>.ModuleTags.GetPool(type).SetDataIfCountLess(ref minCount, ref entities);
            }
        }
        
        [MethodImpl(AggressiveInlining)]
        public void SetMask<WorldID>(byte bufId) where WorldID : struct, IWorldId {
            foreach (var type in Tags) {
                BitMaskUtils<WorldID, ITag>.SetInBuffer(bufId, type.Val);
                #if DEBUG
                Ecs<WorldID>.ModuleTags.GetPool(type).AddBlocker(1);
                #endif
            }
        }
       
        #if DEBUG
        [MethodImpl(AggressiveInlining)]
        public void Dispose<WorldID>() where WorldID : struct, IWorldId {
            foreach (var type in Tags) {
                Ecs<WorldID>.ModuleTags.GetPool(type).AddBlocker(-1);
            }
        }
        #endif
    }
}
#endif