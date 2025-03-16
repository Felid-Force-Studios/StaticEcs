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
    public abstract partial class Ecs<WorldType> {
        public interface IMasksWrapper: IRawPool {
            public MaskDynId DynamicId();
            
            public IMask GetRaw();

            public void Set(Entity entity);

            public bool Has(Entity entity);

            public bool TryDelete(Entity entity);
            
            public void Delete(Entity entity);

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
            public bool TryDelete(Entity entity) => Masks<T>.Value.TryDelete(entity);

            [MethodImpl(AggressiveInlining)]
            public void Delete(Entity entity) => Masks<T>.Value.Delete(entity);

            [MethodImpl(AggressiveInlining)]
            public void Copy(Entity srcEntity, Entity dstEntity) => ModuleMasks.Value.CopyEntity(srcEntity, dstEntity);

            [MethodImpl(AggressiveInlining)]
            public uint Count() => Masks<T>.Value.Count();

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
            object IStandardRawPool.GetRaw(uint entity) => default(T);

            [MethodImpl(AggressiveInlining)]
            void IStandardRawPool.PutRaw(uint entity, object value) => Masks<T>.Value.Set(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            bool IRawPool.Has(uint entity) => Masks<T>.Value.Has(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Add(uint entity) => Masks<T>.Value.Set(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            bool IRawPool.TryDelete(uint entity) => Masks<T>.Value.TryDelete(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Delete(uint entity) => Masks<T>.Value.Delete(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IStandardRawPool.Copy(uint srcEntity, uint dstEntity) {
                if (Masks<T>.Value.Has(new Entity(srcEntity))) {
                    Masks<T>.Value.Set(new Entity(dstEntity));
                }
            }

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Move(uint entity, uint target) {
                if (Masks<T>.Value.TryDelete(new Entity(entity))) {
                    Masks<T>.Value.Set(new Entity(target));
                }
            }

            [MethodImpl(AggressiveInlining)]
            uint IStandardRawPool.Capacity() => 0;

            [MethodImpl(AggressiveInlining)]
            void IMasksWrapper.Clear() => Masks<T>.Value.Clear();

            [MethodImpl(AggressiveInlining)]
            void IMasksWrapper.Destroy() => Masks<T>.Value.Destroy();
        }
    }
}
#endif