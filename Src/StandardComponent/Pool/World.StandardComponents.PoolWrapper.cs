using System;
using System.Runtime.CompilerServices;
using System.Text;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    public interface IStandardRawPool {
        public ushort DynamicId();

        internal void SetDynamicId(ushort id);

        public Guid Guid();

        internal Type GetElementType();

        internal object GetRaw(uint entity);

        internal void PutRaw(uint entity, object value);

        internal void Copy(uint srcEntity, uint dstEntity);

        internal uint Capacity();

        public uint Count();
        
        internal void WriteAll(ref BinaryPackWriter writer);

        internal void ReadAll(ref BinaryPackReader reader);
    }


    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        public interface IStandardComponentsWrapper : IStandardRawPool {

            public IStandardComponent GetRaw(Entity entity);

            public void PutRaw(Entity entity, IStandardComponent component);

            public void Copy(Entity srcEntity, Entity dstEntity);

            public bool Is<C>() where C : struct, IStandardComponent;

            public bool TryCast<C>(out StandardComponentsWrapper<C> wrapper) where C : struct, IStandardComponent;

            internal void Resize(uint cap);

            internal void Destroy();

            internal void ToStringComponent(StringBuilder builder, Entity entity);

            internal void Clear();

            internal void AutoReset(Entity entity);

            internal void AutoInit(Entity entity);

            public void Write(ref BinaryPackWriter writer, Entity entity);
            
            public void Read(ref BinaryPackReader reader, Entity entity);

        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public readonly struct StandardComponentsWrapper<T> : IStandardComponentsWrapper, Stateless
            where T : struct, IStandardComponent {
            [MethodImpl(AggressiveInlining)]
            public ushort DynamicId() => StandardComponents<T>.Value.DynamicId();

            [MethodImpl(AggressiveInlining)]
            void IStandardRawPool.SetDynamicId(ushort id) => StandardComponents<T>.Value.id = id;

            [MethodImpl(AggressiveInlining)]
            public Guid Guid() => StandardComponents<T>.Value.Guid();

            [MethodImpl(AggressiveInlining)]
            internal ref T Ref(Entity entity) => ref StandardComponents<T>.Value.Ref(entity);

            [MethodImpl(AggressiveInlining)]
            public IStandardComponent GetRaw(Entity entity) => StandardComponents<T>.Value.Ref(entity);

            [MethodImpl(AggressiveInlining)]
            public void PutRaw(Entity entity, IStandardComponent component) => StandardComponents<T>.Value.Ref(entity) = (T) component;

            [MethodImpl(AggressiveInlining)]
            public T[] Data() => StandardComponents<T>.Value.Data();

            [MethodImpl(AggressiveInlining)]
            public void Copy(Entity srcEntity, Entity dstEntity) => StandardComponents<T>.Value.Copy(srcEntity, dstEntity);

            [MethodImpl(AggressiveInlining)]
            Type IStandardRawPool.GetElementType() => typeof(T);

            [MethodImpl(AggressiveInlining)]
            object IStandardRawPool.GetRaw(uint entity) => StandardComponents<T>.Value.RefInternal(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IStandardRawPool.PutRaw(uint entity, object value) => StandardComponents<T>.Value.Ref(new Entity(entity)) = (T) value;

            [MethodImpl(AggressiveInlining)]
            void IStandardRawPool.Copy(uint srcEntity, uint dstEntity) => StandardComponents<T>.Value.Copy(new Entity(srcEntity), new Entity(dstEntity));

            [MethodImpl(AggressiveInlining)]
            uint IStandardRawPool.Capacity() => (uint) StandardComponents<T>.Value.Data().Length;

            [MethodImpl(AggressiveInlining)]
            public uint Count() => EntitiesCountWithoutDestroyed();

            [MethodImpl(AggressiveInlining)]
            void IStandardRawPool.WriteAll(ref BinaryPackWriter writer) {
                StandardComponents<T>.Serializer.Value.WriteAll(ref writer, ref StandardComponents<T>.Value);
            }

            [MethodImpl(AggressiveInlining)]
            void IStandardRawPool.ReadAll(ref BinaryPackReader reader) {
                StandardComponents<T>.Serializer.Value.ReadAll(ref reader, ref StandardComponents<T>.Value);
            }

            [MethodImpl(AggressiveInlining)]
            public void Write(ref BinaryPackWriter writer, Entity entity) {
                StandardComponents<T>.Serializer.Value.Write(ref writer, ref StandardComponents<T>.Value, entity);
            }

            [MethodImpl(AggressiveInlining)]
            public void Read(ref BinaryPackReader reader, Entity entity) {
                StandardComponents<T>.Serializer.Value.Read(ref reader, ref StandardComponents<T>.Value, entity);
            }

            [MethodImpl(AggressiveInlining)]
            public bool Is<C>() where C : struct, IStandardComponent => StandardComponents<C>.Value.id == StandardComponents<T>.Value.id;

            [MethodImpl(AggressiveInlining)]
            public bool TryCast<C>(out StandardComponentsWrapper<C> wrapper) where C : struct, IStandardComponent => StandardComponents<C>.Value.id == StandardComponents<T>.Value.id;

            [MethodImpl(AggressiveInlining)]
            void IStandardComponentsWrapper.ToStringComponent(StringBuilder builder, Entity entity) => StandardComponents<T>.Value.ToStringComponent(builder, entity);

            [MethodImpl(AggressiveInlining)]
            void IStandardComponentsWrapper.Resize(uint cap) => StandardComponents<T>.Value.Resize(cap);

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