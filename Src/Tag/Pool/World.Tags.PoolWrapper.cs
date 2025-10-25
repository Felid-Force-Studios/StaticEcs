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
    
    public interface IRawTagPool : IRawPool {
            
        internal ref TagsChunk Chunk(uint chunkIdx);

        internal ulong EMask(uint chunkIdx, int blockIdx);
    }
    
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        
        public interface ITagsWrapper: IRawTagPool {
            
            public ITag GetRaw();

            public bool Set(Entity entity);

            public bool Has(Entity entity);

            public bool Delete(Entity entity);

            public void DeleteWithoutMask(Entity entity);

            public void Copy(Entity srcEntity, Entity dstEntity);
            
            public void Move(Entity entity, Entity target);

            public void ToStringComponent(StringBuilder builder, Entity entity);

            public bool Is<C>() where C : struct, ITag;

            public bool TryCast<C>(out TagsWrapper<C> wrapper) where C : struct, ITag;

            internal void Resize(uint chunksCapacity);

            internal void Destroy();

            internal void ClearChunk(uint chunkIdx);

            internal void UpdateBitMask(uint chunkIdx);

            internal void Initialize(uint chunksCapacity);

            internal void TryMoveChunkToPool(uint chunkIdx);

        }

        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public readonly struct TagsWrapper<T> : ITagsWrapper where T : struct, ITag {
            [MethodImpl(AggressiveInlining)]
            public ushort DynamicId() => Tags<T>.Value.DynamicId();

            [MethodImpl(AggressiveInlining)]
            public Guid Guid() => Tags<T>.Value.Guid();

            [MethodImpl(AggressiveInlining)]
            public ITag GetRaw() => new T();

            [MethodImpl(AggressiveInlining)]
            public bool Set(Entity entity) => Tags<T>.Value.Set(entity);

            [MethodImpl(AggressiveInlining)]
            public bool Has(Entity entity) => Tags<T>.Value.Has(entity);

            [MethodImpl(AggressiveInlining)]
            public bool Delete(Entity entity) => Tags<T>.Value.Delete(entity);

            [MethodImpl(AggressiveInlining)]
            public void DeleteWithoutMask(Entity entity) => Tags<T>.Value.Delete(entity, false);

            [MethodImpl(AggressiveInlining)]
            public void Copy(Entity srcEntity, Entity dstEntity) => Tags<T>.Value.Copy(srcEntity, dstEntity);

            [MethodImpl(AggressiveInlining)]
            public void Move(Entity srcEntity, Entity dstEntity) => Tags<T>.Value.Move(srcEntity, dstEntity);

            [MethodImpl(AggressiveInlining)]
            public uint CalculateCount() => Tags<T>.Value.CalculateCount();

            [MethodImpl(AggressiveInlining)]
            public void ToStringComponent(StringBuilder builder, Entity entity) => Tags<T>.Value.ToStringComponent(builder, entity);

            [MethodImpl(AggressiveInlining)]
            public bool Is<C>() where C : struct, ITag => Tags<C>.Value.id == Tags<T>.Value.id;

            [MethodImpl(AggressiveInlining)]
            public bool TryCast<C>(out TagsWrapper<C> wrapper) where C : struct, ITag => Tags<C>.Value.id == Tags<T>.Value.id;

            [MethodImpl(AggressiveInlining)]
            Type IRawPool.GetElementType() => typeof(T);

            [MethodImpl(AggressiveInlining)]
            object IRawPool.GetRaw(uint entity) => default(T);
            
            [MethodImpl(AggressiveInlining)]
            void ITagsWrapper.UpdateBitMask(uint chunkIdx) => Tags<T>.Value.UpdateBitMask(chunkIdx);

            [MethodImpl(AggressiveInlining)]
            void ITagsWrapper.Initialize(uint chunksCapacity) => Tags<T>.Value.Initialize(chunksCapacity);

            [MethodImpl(AggressiveInlining)]
            void ITagsWrapper.TryMoveChunkToPool(uint chunkIdx) => Tags<T>.Value.TryMoveChunkToPool(chunkIdx);

            [MethodImpl(AggressiveInlining)]
            ref TagsChunk IRawTagPool.Chunk(uint chunkIdx) => ref Tags<T>.Value.Chunk(chunkIdx);
            
            [MethodImpl(AggressiveInlining)]
            ulong IRawTagPool.EMask(uint chunkIdx, int blockIdx) => Tags<T>.Value.EMask(chunkIdx, blockIdx);

            [MethodImpl(AggressiveInlining)]
            void IRawPool.WriteChunk(ref BinaryPackWriter writer, uint chunkIdx) => Tags<T>.Serializer.Value.WriteChunk(ref writer, ref Tags<T>.Value, chunkIdx);

            [MethodImpl(AggressiveInlining)]
            void IRawPool.ReadChunk(ref BinaryPackReader reader, uint chunkIdx) => Tags<T>.Serializer.Value.ReadChunk(ref reader, ref Tags<T>.Value, chunkIdx);

            [MethodImpl(AggressiveInlining)]
            void IRawPool.PutRaw(uint entity, object value) => Tags<T>.Value.Set(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            bool IRawPool.Has(uint entity) => Tags<T>.Value.Has(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Add(uint entity) => Tags<T>.Value.Set(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            bool IRawPool.TryDelete(uint entity) {
                var has = Tags<T>.Value.Has(new Entity(entity));
                if (has) {
                    Tags<T>.Value.Delete(new Entity(entity));
                }
                return has;
            }

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Delete(uint entity) => Tags<T>.Value.Delete(new Entity(entity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Copy(uint srcEntity, uint dstEntity) => Tags<T>.Value.Copy(new Entity(srcEntity), new Entity(dstEntity));

            [MethodImpl(AggressiveInlining)]
            void IRawPool.Move(uint entity, uint target) => Tags<T>.Value.Move(new Entity(entity), new Entity(target));

            [MethodImpl(AggressiveInlining)]
            int IRawPool.CalculateCapacity() => -1;

            [MethodImpl(AggressiveInlining)]
            void ITagsWrapper.Resize(uint chunksCapacity) => Tags<T>.Value.Resize(chunksCapacity);

            [MethodImpl(AggressiveInlining)]
            void ITagsWrapper.Destroy() => Tags<T>.Value.Destroy();

            [MethodImpl(AggressiveInlining)]
            void ITagsWrapper.ClearChunk(uint chunkIdx) => Tags<T>.Value.ClearChunk(chunkIdx);
        }
    }
}
