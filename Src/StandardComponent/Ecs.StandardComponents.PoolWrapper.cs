﻿using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public interface IStandardRawPool {
        internal object GetRaw(int entity);
            
        internal void SetRaw(int entity, object value);

        internal void Copy(int srcEntity, int dstEntity);

        internal int Capacity();
        
        public int Count();
    }
    
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class Ecs<WorldType> {
        public interface IStandardComponentsWrapper: IStandardRawPool {
            public StandardComponentDynId DynamicId();
            
            public IStandardComponent GetRaw(Entity entity);
            
            public void SetRaw(Entity entity, IStandardComponent component);

            public void Copy(Entity srcEntity, Entity dstEntity);
            
            public bool Is<C>() where C : struct, IStandardComponent;

            public bool TryCast<C>(out StandardComponentsWrapper<C> wrapper) where C : struct, IStandardComponent;

            internal void Resize(int cap);

            internal void Destroy();

            internal string ToStringComponent(Entity entity);

            internal void Clear();
            
            internal void AutoReset(Entity entity);
            
            internal void AutoInit(Entity entity);
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public readonly struct StandardComponentsWrapper<T> : IStandardComponentsWrapper, Stateless
            where T : struct, IStandardComponent {
            
            [MethodImpl(AggressiveInlining)]
            public StandardComponentDynId DynamicId() => StandardComponents<T>.Value.DynamicId();
            
            [MethodImpl(AggressiveInlining)]
            internal ref T RefMut(Entity entity) => ref StandardComponents<T>.Value.RefMut(entity);

            [MethodImpl(AggressiveInlining)]
            internal ref readonly T Ref(Entity entity) => ref StandardComponents<T>.Value.Ref(entity);
            
            [MethodImpl(AggressiveInlining)]
            public IStandardComponent GetRaw(Entity entity) => StandardComponents<T>.Value.Ref(entity);

            [MethodImpl(AggressiveInlining)]
            public void SetRaw(Entity entity, IStandardComponent component) => StandardComponents<T>.Value.RefMut(entity) = (T) component;

            [MethodImpl(AggressiveInlining)]
            public T[] Data() => StandardComponents<T>.Value.Data();

            [MethodImpl(AggressiveInlining)]
            public void Copy(Entity srcEntity, Entity dstEntity) => StandardComponents<T>.Value.Copy(srcEntity, dstEntity);

            [MethodImpl(AggressiveInlining)]
            object IStandardRawPool.GetRaw(int entity) => StandardComponents<T>.Value.RefMutInternal(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IStandardRawPool.SetRaw(int entity, object value) => StandardComponents<T>.Value.RefMut(new Entity(entity)) = (T) value;

            [MethodImpl(AggressiveInlining)]
            void IStandardRawPool.Copy(int srcEntity, int dstEntity) => StandardComponents<T>.Value.Copy(new Entity(srcEntity), new Entity(dstEntity));

            [MethodImpl(AggressiveInlining)]
            int IStandardRawPool.Capacity() => StandardComponents<T>.Value.Data().Length;

            [MethodImpl(AggressiveInlining)]
            public int Count() => StandardComponents<T>.Value.Data().Length;

            [MethodImpl(AggressiveInlining)]
            public bool Is<C>() where C : struct, IStandardComponent => StandardComponents<C>.Value.id == StandardComponents<T>.Value.id;

            [MethodImpl(AggressiveInlining)]
            public bool TryCast<C>(out StandardComponentsWrapper<C> wrapper) where C : struct, IStandardComponent => StandardComponents<C>.Value.id == StandardComponents<T>.Value.id;

            [MethodImpl(AggressiveInlining)]
            string IStandardComponentsWrapper.ToStringComponent(Entity entity) => StandardComponents<T>.Value.ToStringComponent(entity);

            [MethodImpl(AggressiveInlining)]
            void IStandardComponentsWrapper.Resize(int cap) => StandardComponents<T>.Value.Resize(cap);

            [MethodImpl(AggressiveInlining)]
            void IStandardComponentsWrapper.Destroy() => StandardComponents<T>.Value.Destroy();

            [MethodImpl(AggressiveInlining)]
            void IStandardComponentsWrapper.Clear() => StandardComponents<T>.Value.Clear();

            [MethodImpl(AggressiveInlining)]
            void IStandardComponentsWrapper.AutoReset(Entity entity) => StandardComponents<T>.Value.AutoReset(entity);

            [MethodImpl(AggressiveInlining)]
            void IStandardComponentsWrapper.AutoInit(Entity entity) => StandardComponents<T>.Value.AutoInit(entity);
        }
    }
}