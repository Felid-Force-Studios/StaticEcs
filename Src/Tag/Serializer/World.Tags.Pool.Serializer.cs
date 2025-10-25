#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
#define FFS_ECS_DEBUG
#endif
#if FFS_ECS_DEBUG || FFS_ECS_ENABLE_DEBUG_EVENTS
#define FFS_ECS_EVENTS
#endif

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
                internal void WriteChunk(ref BinaryPackWriter writer, ref Tags<T> pool, uint chunkIdx) {
                    ref var chunk = ref pool.chunks[chunkIdx];
                    writer.WriteUlong(chunk.notEmptyBlocks);
                    if (chunk.notEmptyBlocks != 0) {
                        writer.WriteUlong(chunk.fullBlocks);
                        writer.WriteArrayUnmanaged(chunk.entities);
                    }
                }

                [MethodImpl(AggressiveInlining)]
                internal void ReadChunk(ref BinaryPackReader reader, ref Tags<T> pool, uint chunkIdx) {
                    ref var chunk = ref pool.chunks[chunkIdx];
                        
                    chunk.notEmptyBlocks =  reader.ReadUlong();
                    if (chunk.notEmptyBlocks != 0) {
                        pool.InitChunkSimple(ref chunk, chunkIdx);
                        chunk.fullBlocks = reader.ReadUlong();
                        reader.ReadArrayUnmanaged(ref chunk.entities);
                    } else {
                        chunk.fullBlocks = 0;
                    }
                }
            }
        }
    }
    
    internal static class TagSerializerUtils {
        
        [MethodImpl(AggressiveInlining)]
        internal static void DeleteAllTagMigration<WorldType>(this ref BinaryPackReader reader, EcsTagDeleteMigrationReader<WorldType> migration, uint chunkIdx) 
            where WorldType : struct, IWorldType {
            
            
            var notEmptyBlocks =  reader.ReadUlong();
            if (notEmptyBlocks != 0) {
                var entities = ArrayPool<ulong>.Shared.Rent(Const.BLOCK_IN_CHUNK);
                
                reader.ReadUlong(); // fullBlocks
                reader.ReadArrayUnmanaged(ref entities);
                
                #if NET6_0_OR_GREATER
                ReadOnlySpan<byte> deBruijn = new(Utils.DeBruijn);
                #else
                var deBruijn = Utils.DeBruijn;
                #endif
                
                var entity = new World<WorldType>.Entity();
                ref var eid = ref entity.id;
                while (notEmptyBlocks > 0) {
                    var blockIdx = (uint) deBruijn[(int) (((notEmptyBlocks & (ulong) -(long) notEmptyBlocks) * 0x37E84A99DAE458FUL) >> 58)];
                    notEmptyBlocks &= notEmptyBlocks - 1;
                    var entitiesMask = entities[blockIdx];

                    if (entitiesMask > 0) {
                        var globalBlockIdx = blockIdx + (chunkIdx << Const.BLOCK_IN_CHUNK_SHIFT);
                        var blockEntity = (uint) (globalBlockIdx << Const.BLOCK_IN_CHUNK_SHIFT) + Const.ENTITY_ID_OFFSET;

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
                
                ArrayPool<ulong>.Shared.Return(entities, true);
            }
        }
    }
}
