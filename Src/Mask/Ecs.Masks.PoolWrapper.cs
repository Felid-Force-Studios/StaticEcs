#if !FFS_ECS_DISABLE_MASKS
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
    public abstract partial class Ecs<WorldID> {
        public interface IMasksWrapper: IRawPool {
            public MaskDynId DynamicId();
            
            public IMask GetRaw();

            public void Set(Entity entity);

            public bool Has(Entity entity);

            public void Del(Entity entity);

            public void Copy(Entity srcEntity, Entity dstEntity);

            public string ToStringComponent(Entity entity);

            public bool Is<C>() where C : struct, IMask;

            public bool TryCast<C>(out MasksWrapper<C> wrapper) where C : struct, IMask;

            internal void Clear();

            internal void Destroy();
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public readonly struct MasksWrapper<T> : IMasksWrapper, Stateless where T : struct, IMask {
            [MethodImpl(AggressiveInlining)]
            public MaskDynId DynamicId() => Masks<T>.Value.DynamicId();

            [MethodImpl(AggressiveInlining)]
            public IMask GetRaw() => new T();

            [MethodImpl(AggressiveInlining)]
            public void Set(Entity entity) => Masks<T>.Value.Set(entity);

            [MethodImpl(AggressiveInlining)]
            public bool Has(Entity entity) => Masks<T>.Value.Has(entity);

            [MethodImpl(AggressiveInlining)]
            public void Del(Entity entity) => Masks<T>.Value.Delete(entity);

            [MethodImpl(AggressiveInlining)]
            public void Copy(Entity srcEntity, Entity dstEntity) => ModuleMasks.Value.CopyEntity(srcEntity, dstEntity);

            [MethodImpl(AggressiveInlining)]
            public int Count() => Masks<T>.Value.Count();

            [MethodImpl(AggressiveInlining)]
            public string ToStringComponent(Entity entity) => Masks<T>.Value.ToStringComponent(entity);

            [MethodImpl(AggressiveInlining)]
            public bool Is<C>() where C : struct, IMask {
                return Masks<C>.Value.id == Masks<T>.Value.id;
            }

            [MethodImpl(AggressiveInlining)]
            public bool TryCast<C>(out MasksWrapper<C> wrapper) where C : struct, IMask {
                return Masks<C>.Value.id == Masks<T>.Value.id;
            }

            [MethodImpl(AggressiveInlining)]
            object IRawPool.GetRaw(int entity) => default(T);

            [MethodImpl(AggressiveInlining)]
            void IRawPool.PutRaw(int entity, object value) => Masks<T>.Value.Set(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            bool IRawPool.Has(int entity) => Masks<T>.Value.Has(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Add(int entity) => Masks<T>.Value.Set(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            bool IRawPool.Delete(int entity) {
                var has = Masks<T>.Value.Has(new Entity(entity));
                Masks<T>.Value.Delete(new Entity(entity));
                return has;
            }

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Copy(int srcEntity, int dstEntity) {
                if (Masks<T>.Value.Has(new Entity(srcEntity))) {
                    Masks<T>.Value.Set(new Entity(dstEntity));
                }
            }

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Move(int entity, int target) {
                Masks<T>.Value.Delete(new Entity(entity));
                Masks<T>.Value.Set(new Entity(target));
            }

            [MethodImpl(AggressiveInlining)]
            int IRawPool.Capacity() => -1;

            [MethodImpl(AggressiveInlining)]
            void IMasksWrapper.Clear() => Masks<T>.Value.Clear();

            [MethodImpl(AggressiveInlining)]
            void IMasksWrapper.Destroy() => Masks<T>.Value.Destroy();
        }
    }
}
#endif