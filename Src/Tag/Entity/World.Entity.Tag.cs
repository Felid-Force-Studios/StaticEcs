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
        public partial struct Entity {

            [MethodImpl(AggressiveInlining)]
            public int TagsCount() => ModuleTags.Value.TagsCount(this);

            [MethodImpl(AggressiveInlining)]
            public void GetAllTags(List<ITag> result) => ModuleTags.Value.GetAllTags(this, result);

            #region BY_TYPE
            #region HAS
            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfTags<C>() where C : struct, ITag {
                return Tags<C>.Value.Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfTags<C1, C2>()
                where C1 : struct, ITag
                where C2 : struct, ITag {
                return Tags<C1>.Value.Has(this) && Tags<C2>.Value.Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfTags<C1, C2, C3>()
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag {
                return Tags<C1>.Value.Has(this) && Tags<C2>.Value.Has(this) && Tags<C3>.Value.Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOfTags<C1, C2>()
                where C1 : struct, ITag
                where C2 : struct, ITag {
                return Tags<C1>.Value.Has(this) || Tags<C2>.Value.Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool HasAnyOfTags<C1, C2, C3>()
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag {
                return Tags<C1>.Value.Has(this) || Tags<C2>.Value.Has(this) || Tags<C3>.Value.Has(this);
            }
            #endregion

            #region ADD
            [MethodImpl(AggressiveInlining)]
            public void SetTag<C>()
                where C : struct, ITag {
                Tags<C>.Value.Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void SetTag<C1, C2>()
                where C1 : struct, ITag
                where C2 : struct, ITag {
                Tags<C1>.Value.Set(this);
                Tags<C2>.Value.Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void SetTag<C1, C2, C3>()
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag {
                Tags<C1>.Value.Set(this);
                Tags<C2>.Value.Set(this);
                Tags<C3>.Value.Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void SetTag<C1, C2, C3, C4>()
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag
                where C4 : struct, ITag {
                Tags<C1>.Value.Set(this);
                Tags<C2>.Value.Set(this);
                Tags<C3>.Value.Set(this);
                Tags<C4>.Value.Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void SetTag<C1, C2, C3, C4, C5>()
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag
                where C4 : struct, ITag
                where C5 : struct, ITag {
                Tags<C1>.Value.Set(this);
                Tags<C2>.Value.Set(this);
                Tags<C3>.Value.Set(this);
                Tags<C4>.Value.Set(this);
                Tags<C5>.Value.Set(this);
            }
            #endregion

            #region DELETE
            [MethodImpl(AggressiveInlining)]
            public void DeleteTag<C>() where C : struct, ITag {
                Tags<C>.Value.Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void DeleteTag<C1, C2>()
                where C1 : struct, ITag
                where C2 : struct, ITag {
                Tags<C1>.Value.Delete(this);
                Tags<C2>.Value.Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void DeleteTag<C1, C2, C3>()
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag {
                Tags<C1>.Value.Delete(this);
                Tags<C2>.Value.Delete(this);
                Tags<C3>.Value.Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void DeleteTag<C1, C2, C3, C4>()
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag
                where C4 : struct, ITag {
                Tags<C1>.Value.Delete(this);
                Tags<C2>.Value.Delete(this);
                Tags<C3>.Value.Delete(this);
                Tags<C4>.Value.Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void DeleteTag<C1, C2, C3, C4, C5>()
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag
                where C4 : struct, ITag
                where C5 : struct, ITag {
                Tags<C1>.Value.Delete(this);
                Tags<C2>.Value.Delete(this);
                Tags<C3>.Value.Delete(this);
                Tags<C4>.Value.Delete(this);
                Tags<C5>.Value.Delete(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryDeleteTag<C>() where C : struct, ITag {
                return Tags<C>.Value.TryDelete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryDeleteTag<C1, C2>()
                where C1 : struct, ITag
                where C2 : struct, ITag {
                var delC1 = Tags<C1>.Value.TryDelete(this);
                var delC2 = Tags<C2>.Value.TryDelete(this);
                return delC1 && delC2;
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryDeleteTag<C1, C2, C3>()
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag {
                var delC1 = Tags<C1>.Value.TryDelete(this);
                var delC2 = Tags<C2>.Value.TryDelete(this);
                var delC3 = Tags<C3>.Value.TryDelete(this);

                return delC1 && delC2 && delC3;
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryDeleteTag<C1, C2, C3, C4>()
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag
                where C4 : struct, ITag {
                var delC1 = Tags<C1>.Value.TryDelete(this);
                var delC2 = Tags<C2>.Value.TryDelete(this);
                var delC3 = Tags<C3>.Value.TryDelete(this);
                var delC4 = Tags<C4>.Value.TryDelete(this);

                return delC1 && delC2 && delC3 && delC4;
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryDeleteTag<C1, C2, C3, C4, C5>()
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag
                where C4 : struct, ITag
                where C5 : struct, ITag {
                var delC1 = Tags<C1>.Value.TryDelete(this);
                var delC2 = Tags<C2>.Value.TryDelete(this);
                var delC3 = Tags<C3>.Value.TryDelete(this);
                var delC4 = Tags<C4>.Value.TryDelete(this);
                var delC5 = Tags<C5>.Value.TryDelete(this);

                return delC1 && delC2 && delC3 && delC4 && delC5;
            }
            #endregion
            
            #region MOVE
            [MethodImpl(AggressiveInlining)]
            public void MoveTagsTo<C1>(Entity target)
                where C1 : struct, ITag {
                Tags<C1>.Value.Move(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void MoveTagsTo<C1, C2>(Entity target)
                where C1 : struct, ITag
                where C2 : struct, ITag {
                Tags<C1>.Value.Move(this, target);
                Tags<C2>.Value.Move(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void MoveTagsTo<C1, C2, C3>(Entity target)
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag {
                Tags<C1>.Value.Move(this, target);
                Tags<C2>.Value.Move(this, target);
                Tags<C3>.Value.Move(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void MoveTagsTo<C1, C2, C3, C4>(Entity target)
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag
                where C4 : struct, ITag {
                Tags<C1>.Value.Move(this, target);
                Tags<C2>.Value.Move(this, target);
                Tags<C3>.Value.Move(this, target);
                Tags<C4>.Value.Move(this, target);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void MoveTagsTo<C1, C2, C3, C4, C5>(Entity target)
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag
                where C4 : struct, ITag
                where C5 : struct, ITag {
                Tags<C1>.Value.Move(this, target);
                Tags<C2>.Value.Move(this, target);
                Tags<C3>.Value.Move(this, target);
                Tags<C4>.Value.Move(this, target);
                Tags<C5>.Value.Move(this, target);
            }
            #endregion
            #endregion
            
            #region BY_RAW_TYPE
            [MethodImpl(AggressiveInlining)]
            public bool HasAllOfTags(Type tagType) {
                return ModuleTags.Value.GetPool(tagType).Has(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void SetTag(Type tagType) {
                ModuleTags.Value.GetPool(tagType).Set(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public bool TryDeleteTag(Type tagType) {
                return ModuleTags.Value.GetPool(tagType).TryDelete(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void DeleteTag(Type tagType) {
                ModuleTags.Value.GetPool(tagType).Delete(this);
            }
            
            [MethodImpl(AggressiveInlining)]
            public void MoveTagsTo(Type tagType, Entity target) {
                ModuleTags.Value.GetPool(tagType).Move(this, target);
            }
            #endregion
        }
    }
    
    public partial interface IEntity {
        public int TagsCount();

        public void GetAllTags(List<ITag> result);

        public bool HasAllOfTags<C>() where C : struct, ITag;

        public void SetTag<C>() where C : struct, ITag;

        public bool TryDeleteTag<C>() where C : struct, ITag;

        public void DeleteTag<C>() where C : struct, ITag;

        public bool HasAllOfTags(Type tagType);

        public void SetTag(Type tagType);

        public bool TryDeleteTag(Type tagType);

        public void DeleteTag(Type tagType);

    }
}
#endif