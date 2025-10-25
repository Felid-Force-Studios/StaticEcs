#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

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

            #region SET
            [MethodImpl(AggressiveInlining)]
            public bool SetTag<C>()
                where C : struct, ITag {
                return Tags<C>.Value.Set(this);
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

            [MethodImpl(AggressiveInlining)]
            public bool ToggleTag<C1>()
                where C1 : struct, ITag {
                return Tags<C1>.Value.Toggle(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void ToggleTag<C1, C2>()
                where C1 : struct, ITag
                where C2 : struct, ITag {
                Tags<C1>.Value.Toggle(this);
                Tags<C2>.Value.Toggle(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void ToggleTag<C1, C2, C3>()
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag {
                Tags<C1>.Value.Toggle(this);
                Tags<C2>.Value.Toggle(this);
                Tags<C3>.Value.Toggle(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void ApplyTag<C>(bool state)
                where C : struct, ITag {
                Tags<C>.Value.Apply(this, state);
            }

            [MethodImpl(AggressiveInlining)]
            public void ApplyTag<C1, C2>(bool stateC1, bool stateC2)
                where C1 : struct, ITag
                where C2 : struct, ITag {
                Tags<C1>.Value.Apply(this, stateC1);
                Tags<C2>.Value.Apply(this, stateC2);
            }

            [MethodImpl(AggressiveInlining)]
            public void ApplyTag<C1, C2, C3>(bool stateC1, bool stateC2, bool stateC3)
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag {
                Tags<C1>.Value.Apply(this, stateC1);
                Tags<C2>.Value.Apply(this, stateC2);
                Tags<C3>.Value.Apply(this, stateC3);
            }
            #endregion

            #region DELETE
            [MethodImpl(AggressiveInlining)]
            public bool DeleteTag<C>() where C : struct, ITag {
                return Tags<C>.Value.Delete(this);
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
            #endregion

            #region COPY
            [MethodImpl(AggressiveInlining)]
            public void CopyTagsTo<C1>(Entity target)
                where C1 : struct, ITag {
                Tags<C1>.Value.Copy(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyTagsTo<C1, C2>(Entity target)
                where C1 : struct, ITag
                where C2 : struct, ITag {
                Tags<C1>.Value.Copy(this, target);
                Tags<C2>.Value.Copy(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyTagsTo<C1, C2, C3>(Entity target)
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag {
                Tags<C1>.Value.Copy(this, target);
                Tags<C2>.Value.Copy(this, target);
                Tags<C3>.Value.Copy(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyTagsTo<C1, C2, C3, C4>(Entity target)
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag
                where C4 : struct, ITag {
                Tags<C1>.Value.Copy(this, target);
                Tags<C2>.Value.Copy(this, target);
                Tags<C3>.Value.Copy(this, target);
                Tags<C4>.Value.Copy(this, target);
            }

            [MethodImpl(AggressiveInlining)]
            public void CopyTagsTo<C1, C2, C3, C4, C5>(Entity target)
                where C1 : struct, ITag
                where C2 : struct, ITag
                where C3 : struct, ITag
                where C4 : struct, ITag
                where C5 : struct, ITag {
                Tags<C1>.Value.Copy(this, target);
                Tags<C2>.Value.Copy(this, target);
                Tags<C3>.Value.Copy(this, target);
                Tags<C4>.Value.Copy(this, target);
                Tags<C5>.Value.Copy(this, target);
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
            public bool RawHasAllOfTags(Type tagType) {
                return ModuleTags.Value.GetPool(tagType).Has(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void RawSetTag(Type tagType) {
                ModuleTags.Value.GetPool(tagType).Set(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void RawDeleteTag(Type tagType) {
                ModuleTags.Value.GetPool(tagType).Delete(this);
            }

            [MethodImpl(AggressiveInlining)]
            public void RawMoveTagsTo(Type tagType, Entity target) {
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

        public void DeleteTag<C>() where C : struct, ITag;

        public bool HasAllOfTags(Type tagType);

        public void SetTag(Type tagType);

        public void DeleteTag(Type tagType);
    }

    public partial class BoxedEntity<WorldType> {
        public int TagsCount() => _entity.TagsCount();
        public void GetAllTags(List<ITag> result) => _entity.GetAllTags(result);
        public bool HasAllOfTags<C>() where C : struct, ITag => _entity.HasAllOfTags<C>();
        public void SetTag<C>() where C : struct, ITag => _entity.SetTag<C>();
        public void DeleteTag<C>() where C : struct, ITag => _entity.DeleteTag<C>();
        public bool HasAllOfTags(Type tagType) => _entity.RawHasAllOfTags(tagType);
        public void SetTag(Type tagType) => _entity.RawSetTag(tagType);
        public void DeleteTag(Type tagType) => _entity.RawDeleteTag(tagType);
    }
}
