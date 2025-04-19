#if !FFS_ECS_DISABLE_TAGS
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
            public int TagsCount() => ModuleTags.Value.TagsCount(_entity);

            #region BY_TYPE
            #region HAS
            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfTags<C>() where C : struct, ITag {
                return Tags<C>.Value.Has(_entity);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfTags<C1, C2>()
                where C1 : struct, ITag
                where C2 : struct, ITag {
                return Tags<C1>.Value.Has(_entity) && Tags<C2>.Value.Has(_entity);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfTags<C1, C2, C3>()
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag {
                return Tags<C1>.Value.Has(_entity) && Tags<C2>.Value.Has(_entity) && Tags<C3>.Value.Has(_entity);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOfTags<C1, C2>()
                where C1 : struct, ITag
                where C2 : struct, ITag {
                return Tags<C1>.Value.Has(_entity) || Tags<C2>.Value.Has(_entity);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOfTags<C1, C2, C3>()
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag {
                return Tags<C1>.Value.Has(_entity) || Tags<C2>.Value.Has(_entity) || Tags<C3>.Value.Has(_entity);
            }
            #endregion
            #endregion
            
            #region BY_RAW_TYPE
            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfTags(Type tagType) {
                return ModuleTags.Value.GetPool(tagType).Has(_entity);
            }
            #endregion
        }
    }
}
#endif