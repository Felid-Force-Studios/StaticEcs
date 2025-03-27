#if !FFS_ECS_DISABLE_TAGS
using System;
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
        public interface ITagsWrapper: IRawPool {
            public TagDynId DynamicId();
            
            public ITag GetRaw();

            public void Set(Entity entity);

            public bool Has(Entity entity);

            public bool TryDelete(Entity entity);

            public void Delete(Entity entity);

            public void Copy(Entity srcEntity, Entity dstEntity);
            
            public void Move(Entity entity, Entity target);

            public string ToStringComponent(Entity entity);

            public bool Is<C>() where C : struct, ITag;

            public bool TryCast<C>(out TagsWrapper<C> wrapper) where C : struct, ITag;

            internal uint[] EntitiesData();

            internal void SetDataIfCountLess(ref uint count, ref uint[] entities);

            internal void SetDataIfCountMore(ref uint count, ref uint[] entities);

            internal void Resize(uint cap);

            internal void Destroy();

            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            internal void AddBlocker(int val);
            #endif

            internal void Clear();
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public readonly struct TagsWrapper<T> : ITagsWrapper, Stateless where T : struct, ITag {
            [MethodImpl(AggressiveInlining)]
            public TagDynId DynamicId() => Tags<T>.Value.DynamicId();

            [MethodImpl(AggressiveInlining)]
            public ITag GetRaw() => new T();

            [MethodImpl(AggressiveInlining)]
            public void Set(Entity entity) => Tags<T>.Value.Set(entity);

            [MethodImpl(AggressiveInlining)]
            public bool Has(Entity entity) => Tags<T>.Value.Has(entity);

            [MethodImpl(AggressiveInlining)]
            public bool TryDelete(Entity entity) => Tags<T>.Value.TryDelete(entity);

            [MethodImpl(AggressiveInlining)]
            public void Delete(Entity entity) => Tags<T>.Value.Delete(entity);

            [MethodImpl(AggressiveInlining)]
            public void Copy(Entity srcEntity, Entity dstEntity) => Tags<T>.Value.Copy(srcEntity, dstEntity);

            [MethodImpl(AggressiveInlining)]
            public void Move(Entity srcEntity, Entity dstEntity) => Tags<T>.Value.Move(srcEntity, dstEntity);

            [MethodImpl(AggressiveInlining)]
            public uint Count() => Tags<T>.Value.Count();

            [MethodImpl(AggressiveInlining)]
            public string ToStringComponent(Entity entity) => Tags<T>.Value.ToStringComponent(entity);

            [MethodImpl(AggressiveInlining)]
            public bool Is<C>() where C : struct, ITag {
                return Tags<C>.Value.id == Tags<T>.Value.id;
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryCast<C>(out TagsWrapper<C> wrapper) where C : struct, ITag {
                return Tags<C>.Value.id == Tags<T>.Value.id;
            }

            [MethodImpl(AggressiveInlining)]
            Type IStandardRawPool.GetElementType() => typeof(T);

            [MethodImpl(AggressiveInlining)]
            object IStandardRawPool.GetRaw(uint entity) => default(T);

            [MethodImpl(AggressiveInlining)]
            void IStandardRawPool.PutRaw(uint entity, object value) => Tags<T>.Value.Set(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            bool IRawPool.Has(uint entity) => Tags<T>.Value.Has(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Add(uint entity) => Tags<T>.Value.Set(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            bool IRawPool.TryDelete(uint entity) => Tags<T>.Value.TryDelete(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Delete(uint entity) => Tags<T>.Value.Delete(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IStandardRawPool.Copy(uint srcEntity, uint dstEntity) => Tags<T>.Value.Copy(new Entity(srcEntity), new Entity(dstEntity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Move(uint entity, uint target) => Tags<T>.Value.Move(new Entity(entity), new Entity(target));

            [MethodImpl(AggressiveInlining)]
            uint IStandardRawPool.Capacity() => (uint) Tags<T>.Value.EntitiesData().Length;

            [MethodImpl(AggressiveInlining)]
            uint[] ITagsWrapper.EntitiesData() => Tags<T>.Value.EntitiesData();

            [MethodImpl(AggressiveInlining)]
            void ITagsWrapper.SetDataIfCountLess(ref uint count, ref uint[] entities) => Tags<T>.Value.SetDataIfCountLess(ref count, ref entities, out var _);

            [MethodImpl(AggressiveInlining)]
            void ITagsWrapper.SetDataIfCountMore(ref uint count, ref uint[] entities) => Tags<T>.Value.SetDataIfCountMore(ref count, ref entities, out var _);

            [MethodImpl(AggressiveInlining)]
            void ITagsWrapper.Resize(uint cap) => Tags<T>.Value.Resize(cap);

            [MethodImpl(AggressiveInlining)]
            void ITagsWrapper.Destroy() => Tags<T>.Value.Destroy();

            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            void ITagsWrapper.AddBlocker(int val) {
                Tags<T>.Value.AddBlocker(val);
            }
            #endif

            [MethodImpl(AggressiveInlining)]
            void ITagsWrapper.Clear() => Tags<T>.Value.Clear();
        }
    }
}
#endif