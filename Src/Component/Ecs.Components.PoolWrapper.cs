using System;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public interface IRawPool : IStandardRawPool {
            
        internal bool Has(uint entity);

        internal void Add(uint entity);

        internal bool TryDelete(uint entity);

        internal void Delete(uint entity);
            
        internal void Move(uint entity, uint target);
    }
    
    public interface IRawComponentPool : IRawPool {
            
        internal bool HasDisabled(uint entity);

        internal bool HasEnabled(uint entity);

        internal void Enable(uint entity);

        internal void Disable(uint entity);
    }
    
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class Ecs<WorldType> {
        public interface IComponentsWrapper: IRawComponentPool {
            public ComponentDynId DynamicId();
            
            public IComponent GetRaw(Entity entity);
            
            public void PutRaw(Entity entity, IComponent component);
            
            public bool Has(Entity entity);

            public bool HasDisabled(Entity entity);

            public bool HasEnabled(Entity entity);

            public void Enable(Entity entity);

            public void Disable(Entity entity);

            public void Add(Entity entity);

            public void TryAdd(Entity entity);

            public void TryAdd(Entity entity, out bool added);

            public bool TryDelete(Entity entity);
            
            public void Delete(Entity entity);

            public void Copy(Entity srcEntity, Entity dstEntity);

            public bool TryCopy(Entity srcEntity, Entity dstEntity);
            
            public void Move(Entity entity, Entity target);
            
            public bool TryMove(Entity entity, Entity target);
            
            public bool Is<C>() where C : struct, IComponent;

            public bool TryCast<C>(out ComponentsWrapper<C> wrapper) where C : struct, IComponent;
            
            internal uint[] EntitiesData();

            internal void Resize(uint cap);
            
            internal void DeleteInternal(Entity entity);

            internal void Destroy();

            internal void SetDataIfCountLess(ref uint count, ref uint[] entities);
            
            internal void SetDataIfCountMore(ref uint count, ref uint[] entities);

            internal string ToStringComponent(Entity entity);

            internal void Clear();

            internal void EnsureSize(uint size);

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
            public bool HasEnabled(Entity entity) => Components<T>.Value.HasEnabled(entity);

            [MethodImpl(AggressiveInlining)]
            public bool HasDisabled(Entity entity) => Components<T>.Value.HasDisabled(entity);

            [MethodImpl(AggressiveInlining)]
            public void Enable(Entity entity) => Components<T>.Value.Enable(entity);

            [MethodImpl(AggressiveInlining)]
            public void Disable(Entity entity) => Components<T>.Value.Disable(entity);

            [MethodImpl(AggressiveInlining)]
            public bool TryDelete(Entity entity) => Components<T>.Value.TryDelete(entity);

            [MethodImpl(AggressiveInlining)]
            public void Delete(Entity entity) => Components<T>.Value.Delete(entity);

            [MethodImpl(AggressiveInlining)]
            public T[] Data() => Components<T>.Value.Data();

            [MethodImpl(AggressiveInlining)]
            public void Copy(Entity srcEntity, Entity dstEntity) => Components<T>.Value.Copy(srcEntity, dstEntity);

            [MethodImpl(AggressiveInlining)]
            public bool TryCopy(Entity srcEntity, Entity dstEntity) => Components<T>.Value.TryCopy(srcEntity, dstEntity);
            
            [MethodImpl(AggressiveInlining)]
            public void Move(Entity srcEntity, Entity dstEntity) => Components<T>.Value.Move(srcEntity, dstEntity);
            
            [MethodImpl(AggressiveInlining)]
            public bool TryMove(Entity srcEntity, Entity dstEntity) => Components<T>.Value.TryMove(srcEntity, dstEntity);

            [MethodImpl(AggressiveInlining)]
            Type IStandardRawPool.GetElementType() => typeof(T);

            [MethodImpl(AggressiveInlining)]
            object IStandardRawPool.GetRaw(uint entity) => Components<T>.Value.RefMutInternal(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IStandardRawPool.PutRaw(uint entity, object value) => Components<T>.Value.Put(new Entity(entity), (T) value);

            [MethodImpl(AggressiveInlining)]
            bool IRawPool.Has(uint entity) => Components<T>.Value.Has(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Add(uint entity) => Components<T>.Value.Add(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            bool IRawPool.TryDelete(uint entity) => Components<T>.Value.TryDelete(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Delete(uint entity) => Components<T>.Value.Delete(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IStandardRawPool.Copy(uint srcEntity, uint dstEntity) => Components<T>.Value.Copy(new Entity(srcEntity), new Entity(dstEntity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Move(uint entity, uint target) => Components<T>.Value.Move(new Entity(entity), new Entity(target));

            [MethodImpl(AggressiveInlining)]
            uint IStandardRawPool.Capacity() => (uint) Components<T>.Value.EntitiesData().Length;

            [MethodImpl(AggressiveInlining)]
            public uint Count() => Components<T>.Value.Count();

            [MethodImpl(AggressiveInlining)]
            public bool Is<C>() where C : struct, IComponent => Components<C>.Value.id == Components<T>.Value.id;

            [MethodImpl(AggressiveInlining)]
            public bool TryCast<C>(out ComponentsWrapper<C> wrapper) where C : struct, IComponent => Components<C>.Value.id == Components<T>.Value.id;

            [MethodImpl(AggressiveInlining)]
            uint[] IComponentsWrapper.EntitiesData() => Components<T>.Value.EntitiesData();

            [MethodImpl(AggressiveInlining)]
            string IComponentsWrapper.ToStringComponent(Entity entity) => Components<T>.Value.ToStringComponent(entity);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.SetDataIfCountLess(ref uint count, ref uint[] entities) => Components<T>.Value.SetDataIfCountLess(ref count, ref entities, out var _);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.SetDataIfCountMore(ref uint count, ref uint[] entities) => Components<T>.Value.SetDataIfCountMore(ref count, ref entities, out var _);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.Resize(uint cap) => Components<T>.Value.Resize(cap);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.DeleteInternal(Entity entity) => Components<T>.Value.DeleteInternal(entity);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.Destroy() => Components<T>.Value.Destroy();

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.EnsureSize(uint size) => Components<T>.Value.EnsureSize(size);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.Clear() => Components<T>.Value.Clear();

            #if DEBUG || FFS_ECS_ENABLE_DEBUG
            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.AddBlocker(int val) => Components<T>.Value.AddBlocker(val);
            #endif
            
            [MethodImpl(AggressiveInlining)]
            bool IRawComponentPool.HasDisabled(uint entity) => Components<T>.Value.HasDisabled(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            bool IRawComponentPool.HasEnabled(uint entity) => Components<T>.Value.HasEnabled(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawComponentPool.Enable(uint entity) => Components<T>.Value.Enable(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawComponentPool.Disable(uint entity) => Components<T>.Value.Disable(new Entity(entity));
        }
    }
}