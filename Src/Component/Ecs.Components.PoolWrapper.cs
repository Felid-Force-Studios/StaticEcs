using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public interface IRawPool : IStandardRawPool {
            
        internal bool Has(int entity);

        internal void Add(int entity);

        internal bool Delete(int entity);
            
        internal void Move(int entity, int target);
    }
    
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class Ecs<WorldType> {
        public interface IComponentsWrapper: IRawPool {
            public ComponentDynId DynamicId();
            
            public IComponent GetRaw(Entity entity);
            
            public void PutRaw(Entity entity, IComponent component);
            
            public bool Has(Entity entity);

            public void Add(Entity entity);

            public void TryAdd(Entity entity);

            public void TryAdd(Entity entity, out bool added);

            public bool Delete(Entity entity);

            public void Copy(Entity srcEntity, Entity dstEntity);
            
            public void Move(Entity entity, Entity target);
            
            public bool Is<C>() where C : struct, IComponent;

            public bool TryCast<C>(out ComponentsWrapper<C> wrapper) where C : struct, IComponent;
            
            internal int[] EntitiesData();

            internal void Resize(int cap);
            
            internal bool DeleteFromWorld(Entity entity);

            internal void Destroy();

            internal void SetDataIfCountLess(ref int count, ref int[] entities);
            
            internal void SetDataIfCountMore(ref int count, ref int[] entities);

            internal string ToStringComponent(Entity entity);

            internal void Clear();

            internal void EnsureSize(int size);

            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            internal void AddBlocker(int val);
            #endif
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public readonly struct ComponentsWrapper<T> : IComponentsWrapper, Stateless
            where T : struct, IComponent {
            
            [MethodImpl(AggressiveInlining)]
            public ComponentDynId DynamicId() => Components<T>.Value.DynamicId();
            
            [MethodImpl(AggressiveInlining)]
            internal ref T RefMut(Entity entity) => ref Components<T>.Value.RefMut(entity);

            [MethodImpl(AggressiveInlining)]
            internal ref readonly T Ref(Entity entity) => ref Components<T>.Value.Ref(entity);
            
            [MethodImpl(AggressiveInlining)]
            public IComponent GetRaw(Entity entity) => Components<T>.Value.Ref(entity);

            [MethodImpl(AggressiveInlining)]
            public void PutRaw(Entity entity, IComponent component) => Components<T>.Value.Put(entity, (T) component);

            [MethodImpl(AggressiveInlining)]
            public void Put(Entity entity, T component) => Components<T>.Value.Put(entity, component);

            [MethodImpl(AggressiveInlining)]
            public void Add(Entity entity) => Components<T>.Value.Add(entity);

            [MethodImpl(AggressiveInlining)]
            public void TryAdd(Entity entity) => Components<T>.Value.TryAdd(entity);

            [MethodImpl(AggressiveInlining)]
            public void TryAdd(Entity entity, out bool added) => Components<T>.Value.TryAdd(entity, out added);

            [MethodImpl(AggressiveInlining)]
            public bool Has(Entity entity) => Components<T>.Value.Has(entity);

            [MethodImpl(AggressiveInlining)]
            public bool Delete(Entity entity) => Components<T>.Value.Delete(entity);

            [MethodImpl(AggressiveInlining)]
            public T[] Data() => Components<T>.Value.Data();

            [MethodImpl(AggressiveInlining)]
            public void Copy(Entity srcEntity, Entity dstEntity) => Components<T>.Value.Copy(srcEntity, dstEntity);
            
            [MethodImpl(AggressiveInlining)]
            public void Move(Entity srcEntity, Entity dstEntity) => Components<T>.Value.Move(srcEntity, dstEntity);

            [MethodImpl(AggressiveInlining)]
            object IStandardRawPool.GetRaw(int entity) => Components<T>.Value.RefMutInternal(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IStandardRawPool.PutRaw(int entity, object value) => Components<T>.Value.Put(new Entity(entity), (T) value);

            [MethodImpl(AggressiveInlining)]
            bool IRawPool.Has(int entity) => Components<T>.Value.Has(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Add(int entity) => Components<T>.Value.Add(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            bool IRawPool.Delete(int entity) => Components<T>.Value.Delete(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IStandardRawPool.Copy(int srcEntity, int dstEntity) => Components<T>.Value.Copy(new Entity(srcEntity), new Entity(dstEntity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Move(int entity, int target) => Components<T>.Value.Move(new Entity(entity), new Entity(target));

            [MethodImpl(AggressiveInlining)]
            int IStandardRawPool.Capacity() => Components<T>.Value.EntitiesData().Length;

            [MethodImpl(AggressiveInlining)]
            public int Count() => Components<T>.Value.Count();

            [MethodImpl(AggressiveInlining)]
            public bool Is<C>() where C : struct, IComponent => Components<C>.Value.id == Components<T>.Value.id;

            [MethodImpl(AggressiveInlining)]
            public bool TryCast<C>(out ComponentsWrapper<C> wrapper) where C : struct, IComponent => Components<C>.Value.id == Components<T>.Value.id;

            [MethodImpl(AggressiveInlining)]
            int[] IComponentsWrapper.EntitiesData() => Components<T>.Value.EntitiesData();

            [MethodImpl(AggressiveInlining)]
            string IComponentsWrapper.ToStringComponent(Entity entity) => Components<T>.Value.ToStringComponent(entity);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.SetDataIfCountLess(ref int count, ref int[] entities) => Components<T>.Value.SetDataIfCountLess(ref count, ref entities);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.SetDataIfCountMore(ref int count, ref int[] entities) => Components<T>.Value.SetDataIfCountMore(ref count, ref entities);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.Resize(int cap) => Components<T>.Value.Resize(cap);

            [MethodImpl(AggressiveInlining)]
            bool IComponentsWrapper.DeleteFromWorld(Entity entity) => Components<T>.Value.DeleteFromWorld(entity);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.Destroy() => Components<T>.Value.Destroy();

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.EnsureSize(int size) => Components<T>.Value.EnsureSize(size);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.Clear() => Components<T>.Value.Clear();

            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.AddBlocker(int val) => Components<T>.Value.AddBlocker(val);
            #endif
        }
    }
}