#if !FFS_ECS_DISABLE_TAGS
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticPack;
using static System.Runtime.CompilerServices.MethodImplOptions;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs {
    
    public delegate void EcsTagDeleteMigrationReader<WorldType>(World<WorldType>.Entity entity)
        where WorldType : struct, IWorldType;

    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    public abstract partial class World<WorldType> {
        #if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        #endif
        public partial struct Tags<T> where T : struct, ITag {
            #if ENABLE_IL2CPP
            [Il2CppSetOption(Option.NullChecks, false)]
            [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            [Il2CppEagerStaticClassConstruction]
            #endif
            public struct Serializer {
                internal static Serializer Value;
                
                internal Guid guid;
                
                [MethodImpl(AggressiveInlining)]
                public void Create(Guid value) {
                    guid = value;
                }

                [MethodImpl(AggressiveInlining)]
                public void Destroy() {
                    guid = default;
                }

                [MethodImpl(AggressiveInlining)]
                internal void WriteAll(ref BinaryPackWriter writer, ref Tags<T> pool) {
                    writer.WriteInt(pool.chunks.Length);
                    writer.WriteInt(Entities.Value.nextActiveChunkIdx);
                    for (var i = 0; i < Entities.Value.nextActiveChunkIdx; i++) {
                        ref var chunk = ref pool.chunks[i];
                        writer.WriteUlong(chunk.notEmptyBlocks);
                        writer.WriteUlong(chunk.fullBlocks);
                        writer.WriteArrayUnmanaged(chunk.entities);
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void ReadAll(ref BinaryPackReader reader, ref Tags<T> pool) {
                    var chunkCapacity = reader.ReadInt();
                    var chunkCount = reader.ReadInt();

                    pool.chunks ??= new TagsChunk[chunkCapacity];
                    if (pool.chunks.Length < chunkCapacity) {
                        Array.Resize(ref pool.chunks, chunkCapacity);
                    }
                    
                    for (uint i = 0; i < chunkCount; i++) {
                        ref var chunk = ref pool.chunks[i];
                        chunk.notEmptyBlocks =  reader.ReadUlong();
                        chunk.fullBlocks =  reader.ReadUlong();
                        reader.ReadArrayUnmanaged(ref chunk.entities);
                    }
                }
            }
        }
    }
    
    internal static class TagSerializerUtils {
        
        [MethodImpl(AggressiveInlining)]
        internal static void DeleteAllTagMigration<WorldType>(this ref BinaryPackReader reader, EcsTagDeleteMigrationReader<WorldType> migration) 
            where WorldType : struct, IWorldType {
            var chunkCapacity = reader.ReadInt();
            var chunkCount = reader.ReadInt();

            var chunks = ArrayPool<TagsChunk>.Shared.Rent(chunkCapacity);
            for (uint i = 0; i < chunkCount; i++) {
                ref var chunk = ref chunks[i];
                chunk.notEmptyBlocks = reader.ReadUlong();
                chunk.fullBlocks = reader.ReadUlong();
                chunk.entities = ArrayPool<ulong>.Shared.Rent(Const.BLOCK_IN_CHUNK);
                reader.ReadArrayUnmanaged(ref chunk.entities);
            }

            #if NET6_0_OR_GREATER
            ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
            #else
            var deBruijn = Utils.DeBruijn;
            #endif
            var entity = new World<WorldType>.Entity();
            ref var eid = ref entity._id;
            for (var chunkIdx = 0; chunkIdx < chunkCount; chunkIdx++) {
                ref var chunk = ref chunks[chunkIdx];
                var chunkMask = chunk.notEmptyBlocks;

                while (chunkMask > 0) {
                    var blockIdx = (uint) deBruijn[(int) (((chunkMask & (ulong) -(long) chunkMask) * 0x37E84A99DAE458FUL) >> 58)];
                    chunkMask &= chunkMask - 1;
                    var entitiesMask = chunk.entities[blockIdx];

                    if (entitiesMask > 0) {
                        var globalBlockIdx = blockIdx + (chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                        var blockEntity = (uint) (globalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT);

                        var idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                        var end = Utils.ApproximateMSB(entitiesMask);
                        var total = Utils.PopCnt(entitiesMask);
                        if (total >= (end - idx) >> 1) {
                            for (; idx < end; idx++) {
                                if ((entitiesMask & (1UL << idx)) > 0) {
                                    eid = blockEntity + idx;
                                    migration(entity);
                                }
                            }
                        } else {
                            do {
                                eid = blockEntity + idx;
                                migration(entity);
                                entitiesMask &= entitiesMask - 1UL;
                                idx = deBruijn[(int) (((entitiesMask & (ulong) -(long) entitiesMask) * 0x37E84A99DAE458FUL) >> 58)];
                            } while (entitiesMask > 0);
                        }
                    }
                }
            }
            
            for (uint i = 0; i < chunkCount; i++) {
                ref var chunk = ref chunks[i];
                ArrayPool<ulong>.Shared.Return(chunk.entities, true);
            }
            
            ArrayPool<TagsChunk>.Shared.Return(chunks, true);
        }
    }
}
#endif