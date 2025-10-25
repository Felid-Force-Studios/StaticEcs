#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

using System;
using System.Runtime.CompilerServices;
using System.Text;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    public interface IRawPool {
        public ushort DynamicId();

        public Guid Guid();

        internal Type GetElementType();

        internal object GetRaw(uint entity);

        internal void PutRaw(uint entity, object value);

        internal void Copy(uint srcEntity, uint dstEntity);

        internal int CalculateCapacity();

        public uint CalculateCount();
        
        internal void WriteChunk(ref BinaryPackWriter writer, uint chunkIdx);

        internal void ReadChunk(ref BinaryPackReader reader, uint chunkIdx);
        
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
            
        internal ref ComponentsChunk Chunk(uint chunkIdx);

        internal ulong EMask(uint chunkIdx, int blockIdx);
            
        internal ulong DMask(uint chunkIdx, int blockIdx);
            
        internal ulong AMask(uint chunkIdx, int blockIdx);
    }
    
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        public interface IComponentsWrapper: IRawComponentPool {
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

            internal void Resize(uint chunksCapacity);
            
            internal void DeleteInternalWithoutMask(Entity entity);

            internal void Destroy();

            internal void ToStringComponent(StringBuilder builder, Entity entity);

            internal void ClearChunk(uint chunkIdx);
        
            void Write(ref BinaryPackWriter writer, Entity entity);

            void Read(ref BinaryPackReader reader, Entity entity);

            internal void UpdateBitMask(uint chunkIdx);

            internal void Initialize(uint chunksCapacity);

            internal void TryMoveChunkToPool(uint chunkIdx);
        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public readonly struct ComponentsWrapper<T> : IComponentsWrapper
            where T : struct, IComponent {
            
            [MethodImpl(AggressiveInlining)]
            public Guid Guid() => Components<T>.Value.Guid();
            
            [MethodImpl(AggressiveInlining)]
            public ushort DynamicId() => Components<T>.Value.DynamicId();

            [MethodImpl(AggressiveInlining)]
            internal ref T Ref(Entity entity) => ref Components<T>.Value.Ref(entity);
            
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
            public void Copy(Entity srcEntity, Entity dstEntity) => Components<T>.Value.Copy(srcEntity, dstEntity);

            [MethodImpl(AggressiveInlining)]
            public bool TryCopy(Entity srcEntity, Entity dstEntity) => Components<T>.Value.TryCopy(srcEntity, dstEntity);
            
            [MethodImpl(AggressiveInlining)]
            public void Move(Entity srcEntity, Entity dstEntity) => Components<T>.Value.Move(srcEntity, dstEntity);
            
            [MethodImpl(AggressiveInlining)]
            public bool TryMove(Entity srcEntity, Entity dstEntity) => Components<T>.Value.TryMove(srcEntity, dstEntity);

            [MethodImpl(AggressiveInlining)]
            Type IRawPool.GetElementType() => typeof(T);

            [MethodImpl(AggressiveInlining)]
            object IRawPool.GetRaw(uint entity) => Components<T>.Value.RefInternal(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.PutRaw(uint entity, object value) => Components<T>.Value.Put(new Entity(entity), (T) value);

            [MethodImpl(AggressiveInlining)]
            bool IRawPool.Has(uint entity) => Components<T>.Value.Has(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Add(uint entity) => Components<T>.Value.Add(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            bool IRawPool.TryDelete(uint entity) => Components<T>.Value.TryDelete(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Delete(uint entity) => Components<T>.Value.Delete(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Copy(uint srcEntity, uint dstEntity) => Components<T>.Value.Copy(new Entity(srcEntity), new Entity(dstEntity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Move(uint entity, uint target) => Components<T>.Value.Move(new Entity(entity), new Entity(target));

            [MethodImpl(AggressiveInlining)]
            int IRawPool.CalculateCapacity() => Components<T>.Value.CalculateCapacity();

            [MethodImpl(AggressiveInlining)]
            public uint CalculateCount() => Components<T>.Value.CalculateCount();

            [MethodImpl(AggressiveInlining)]
            public bool Is<C>() where C : struct, IComponent => Components<C>.Value.id == Components<T>.Value.id;

            [MethodImpl(AggressiveInlining)]
            public bool TryCast<C>(out ComponentsWrapper<C> wrapper) where C : struct, IComponent => Components<C>.Value.id == Components<T>.Value.id;

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.ToStringComponent(StringBuilder builder, Entity entity) => Components<T>.Value.ToStringComponent(builder, entity);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.Resize(uint chunksCapacity) => Components<T>.Value.Resize(chunksCapacity);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.DeleteInternalWithoutMask(Entity entity) => Components<T>.Value.Delete(entity, false);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.Destroy() => Components<T>.Value.Destroy();

            [MethodImpl(AggressiveInlining)]
            public void Write(ref BinaryPackWriter writer, Entity entity) => Components<T>.Serializer.Value.Write(ref writer, ref Components<T>.Value, entity);

            [MethodImpl(AggressiveInlining)]
            public void Read(ref BinaryPackReader reader, Entity entity) => Components<T>.Serializer.Value.Read(ref reader, ref Components<T>.Value, entity);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.UpdateBitMask(uint chunkIdx) => Components<T>.Value.UpdateBitMask(chunkIdx);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.Initialize(uint chunksCapacity) => Components<T>.Value.Initialize(chunksCapacity);
            
            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.TryMoveChunkToPool(uint chunkIdx) => Components<T>.Value.TryMoveChunkToPool(chunkIdx);

            [MethodImpl(AggressiveInlining)]
            ref ComponentsChunk IRawComponentPool.Chunk(uint chunkIdx) => ref Components<T>.Value.Chunk(chunkIdx);
            
            [MethodImpl(AggressiveInlining)]
            ulong IRawComponentPool.EMask(uint chunkIdx, int blockIdx) => Components<T>.Value.EMask(chunkIdx, blockIdx);
            
            [MethodImpl(AggressiveInlining)]
            ulong IRawComponentPool.DMask(uint chunkIdx, int blockIdx) => Components<T>.Value.DMask(chunkIdx, blockIdx);
            
            [MethodImpl(AggressiveInlining)]
            ulong IRawComponentPool.AMask(uint chunkIdx, int blockIdx) => Components<T>.Value.AMask(chunkIdx, blockIdx);

            [MethodImpl(AggressiveInlining)]
            void IRawPool.WriteChunk(ref BinaryPackWriter writer, uint chunkIdx) => Components<T>.Serializer.Value.WriteChunk(ref writer, ref Components<T>.Value, chunkIdx);

            [MethodImpl(AggressiveInlining)]
            void IRawPool.ReadChunk(ref BinaryPackReader reader, uint chunkIdx) => Components<T>.Serializer.Value.ReadChunk(ref reader, ref Components<T>.Value, chunkIdx);

            [MethodImpl(AggressiveInlining)]
            void IComponentsWrapper.ClearChunk(uint chunkIdx) => Components<T>.Value.ClearChunk(chunkIdx);
            
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